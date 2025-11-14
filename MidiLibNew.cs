using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ephemera.NBagOfTricks;
using NAudio.Midi;


namespace MidiLibNew
{
    public class Defs // from MidiDefs and/or MidiLibDefs
    {
        /// <summary>Midi caps.</summary>
        public const int MIN_MIDI = 0;

        /// <summary>Midi caps.</summary>
        public const int MAX_MIDI = 127;

        /// <summary>Midi caps.</summary>
        public const int NUM_CHANNELS = 16;

        /// <summary>The normal drum channel.</summary>
        public const int DEFAULT_DRUM_CHANNEL = 10;

        /// <summary>Default value.</summary>
        public const double DEFAULT_VOLUME = 0.8;

        /// <summary>Allow UI controls some more headroom.</summary>
        public const double MAX_VOLUME = 2.0;
    }



    public class Presets
    {
        public static string[] Load(string fn)
        {
            // TODO1 read file lines: 011 GlassFlute using LoadDefFile()
            var vals = new string[Defs.MAX_MIDI + 1];
            for (int i = 0; i < vals.Length; i++)
            {
                vals[i] = $"Patch{i} {i}"; // fake for now
            }
            return vals;
        }

        // TODO1 these needed? MidiDefs.cs* string to int - script parsing  Nebulator  (not Nebulua  MidiGenerator)
        public int GetControllerNumber(string which) { return 9999; }
        public int GetDrumKitNumber(string which) { return 9999; }
        public int GetDrumNumber(string which) { return 9999; }
        public int GetInstrumentNumber(string which) { return 9999; }
        public int GetInstrumentOrDrumKitNumber(string which) { return 9999; }
        // MidiDefs.cs* int to string - MidiExport
        public string GetControllerName(int which) { return "9999"; }
        public string GetDrumKitName(int which) { return "9999"; }
        public string GetDrumName(int which) { return "9999"; }
        public string GetInstrumentName(int which) { return "9999"; } //+ PatchPicker, ChannelControl(s)

    }



    ///////////////////////////////// from MidiLib - works as is: ///////////////////////////

    /// <summary>
    /// Midi (real or sim) has received something. It's up to the client to make sense of it.
    /// Property value of -1 indicates invalid or not pertinent e.g a controller event doesn't have velocity.
    /// </summary>
    public class InputReceiveEventArgs : EventArgs
    {
        /// <summary>Channel number 1-based. Required.</summary>
        public int Channel { get; set; } = 0;

        /// <summary>The note number to play. NoteOn/Off only.</summary>
        public int Note { get; set; } = -1;

        /// <summary>Specific controller id.</summary>
        public int Controller { get; set; } = -1;

        /// <summary>For Note = velocity. For controller = payload.</summary>
        public int Value { get; set; } = -1;

        /// <summary>Something to tell the client.</summary>
        public string ErrorInfo { get; set; } = "";

        /// <summary>Special controller id to carry pitch info.</summary>
        public const int PITCH_CONTROL = 1000;

        /// <summary>Read me.</summary>
        public override string ToString()
        {
            StringBuilder sb = new($"Channel:{Channel} ");

            if (ErrorInfo != "")
            {
                sb.Append($"Error:{ErrorInfo} ");
            }
            else
            {
                sb.Append($"Channel:{Channel} Note:{Note} Controller:{Controller} Value:{Value}");
            }

            return sb.ToString();
        }
    }


    ////////////////////// IDevice.cs //////////////////////////
    /// <summary>Abstraction layer to support all midi-like devices.</summary>
    public interface IDevice : IDisposable
    {
        #region Properties
        /// <summary>Device name as defined by the system.</summary>
        string DeviceName { get; }

        /// <summary>Are we ok?</summary>
        bool Valid { get; }

        /// <summary>Log traffic at Trace level.</summary>
        bool LogEnable { get; set; }
        #endregion
    }

    /// <summary>Abstraction layer to support input devices.</summary>
    public interface IInputDevice : IDevice
    {
        #region Properties
        /// <summary>Capture on/off.</summary>
        bool CaptureEnable { get; set; }
        #endregion

        #region Events
        /// <summary>Handler for message arrived.</summary>
        event EventHandler<InputReceiveEventArgs>? InputReceive;
        #endregion
    }

    /// <summary>Abstraction layer to support output devices.</summary>
    public interface IOutputDevice : IDevice
    {
        #region Properties

        #endregion

        #region Functions
        /// <summary>Send midi event.</summary>
        /// <param name="evt"></param>
        void SendEvent(MidiEvent evt);
        #endregion
    }


    //////////////////////////////////////////// MidiInput.cs ///////////////////////////////
    /// <summary>
    /// Midi input handler.
    /// </summary>
    public sealed class MidiInput : IInputDevice
    {
        #region Fields
        /// <summary>Midi input device.</summary>
        readonly MidiIn? _midiIn = null;

        /// <summary>Midi send logging.</summary>
        readonly Logger _logger = LogManager.CreateLogger("MidiInput");

        /// <summary>Control.</summary>
        bool _capturing = false;
        #endregion

        #region Properties
        /// <inheritdoc />
        public string DeviceName { get; }

        /// <inheritdoc />
        public bool Valid { get { return _midiIn is not null; } }

        /// <inheritdoc />
        public bool LogEnable { get { return _logger.Enable; } set { _logger.Enable = value; } }

        /// <summary>Capture on/off.</summary>
        public bool CaptureEnable
        {
            get { return _capturing; }
            set { if (value) _midiIn?.Start(); else _midiIn?.Stop(); _capturing = value; }
        }
        #endregion

        #region Events
        /// <inheritdoc />
        public event EventHandler<InputReceiveEventArgs>? InputReceive;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        /// <param name="deviceName">Client must supply name of device.</param>
        public MidiInput(string deviceName)
        {
            DeviceName = deviceName;
            LogEnable = false;
            
            // Figure out which midi input device.
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                if (deviceName == MidiIn.DeviceInfo(i).ProductName)
                {
                    _midiIn = new MidiIn(i);
                    _midiIn.MessageReceived += MidiIn_MessageReceived;
                    _midiIn.ErrorReceived += MidiIn_ErrorReceived;
                    break;
                }
            }
        }

        /// <summary>
        /// Resource clean up.
        /// </summary>
        public void Dispose()
        {
            _midiIn?.Stop();
            _midiIn?.Dispose();
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Process input midi event.
        /// </summary>
        void MidiIn_MessageReceived(object? sender, MidiInMessageEventArgs e)
        {
            // Decode the message. We only care about a few.
            MidiEvent me = MidiEvent.FromRawMessage(e.RawMessage);
            InputReceiveEventArgs? mevt = null;

            switch (me)
            {
                case NoteOnEvent evt:
                    mevt = new InputReceiveEventArgs()
                    {
                        Channel = evt.Channel,
                        Note = evt.NoteNumber,
                        Value = evt.Velocity
                    };
                    break;

                case NoteEvent evt:
                    mevt = new InputReceiveEventArgs()
                    {
                        Channel = evt.Channel,
                        Note = evt.NoteNumber,
                        Value = 0
                    };
                    break;

                case ControlChangeEvent evt:
                    mevt = new InputReceiveEventArgs()
                    {
                        Channel = evt.Channel,
                        Controller = (int)evt.Controller,
                        Value = evt.ControllerValue
                    };
                    break;

                case PitchWheelChangeEvent evt:
                    mevt = new InputReceiveEventArgs()
                    {
                        Channel = evt.Channel,
                        Controller = InputReceiveEventArgs.PITCH_CONTROL,
                        Value = evt.Pitch
                    };
                    break;

                default:
                    // Ignore.
                    break;
            }

            if (mevt is not null && InputReceive is not null)
            {
                // Pass it up for client handling.
                InputReceive.Invoke(this, mevt);
                Log(mevt);
            }
        }

        /// <summary>
        /// Process error midi event - Parameter 1 is invalid.
        /// </summary>
        void MidiIn_ErrorReceived(object? sender, MidiInMessageEventArgs e)
        {
            InputReceiveEventArgs evt = new()
            {
                ErrorInfo = $"Message:0x{e.RawMessage:X8}"
            };
            Log(evt);
        }

        /// <summary>
        /// Send event information to the client to sort out.
        /// </summary>
        /// <param name="evt"></param>
        void Log(InputReceiveEventArgs evt)
        {
            if (LogEnable)
            {
                _logger.Trace(evt.ToString());
            }
        }
        #endregion
    }


    //////////////////////////////////////////// MidiOutput.cs ///////////////////////////////
    /// <summary>
    /// A midi output layer - associated with a single device.
    /// </summary>
    public sealed class MidiOutput : IOutputDevice
    {
        #region Fields
        /// <summary>Low level midi output device.</summary>
        readonly MidiOut? _midiOut = null;

        /// <summary>Midi send logging.</summary>
        readonly Logger _logger = LogManager.CreateLogger("MidiOutput");
        #endregion

        #region Properties
        /// <inheritdoc />
        public string DeviceName { get; }

        /// <inheritdoc />
        public bool Valid { get { return _midiOut is not null; } }

        /// <inheritdoc />
        public bool LogEnable { get { return _logger.Enable; } set { _logger.Enable = value; } }
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        /// <param name="deviceName">Client must supply name of device.</param>
        public MidiOutput(string deviceName)
        {
            DeviceName = deviceName;

            LogEnable = false;

            // Figure out which midi output device.
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                if (deviceName == MidiOut.DeviceInfo(i).ProductName)
                {
                    _midiOut = new MidiOut(i);
                    break;
                }
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            // Resources.
            _midiOut?.Dispose();
        }
        #endregion

        #region Public functions - midi
        /// <inheritdoc />
        public void SendEvent(MidiEvent evt)
        {
            _midiOut?.Send(evt.GetAsShortMessage());
            if (LogEnable)
            {
                _logger.Trace(evt.ToString());
            }
        }
        #endregion
    }
}

