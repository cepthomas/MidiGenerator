using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ephemera.NBagOfTricks;
using MidiGenerator;
using NAudio.Midi;


// Copy of stuff from original MidiLib to support minimal apps. Works as is unless noted.

namespace Ephemera.MidiLib
{
    public class MidiDefs // ==> MidiDefs.cs
    {
        /// <summary>Midi caps.</summary>
        public const int MIN_MIDI = 0;

        /// <summary>Midi caps.</summary>
        public const int MAX_MIDI = 127;

        /// <summary>Midi caps.</summary>
        public const int NUM_CHANNELS = 16;



        #region From original TODO2

        // int => string  UI + export
        // Searching 1281 files for "GetPatchName" (case sensitive, whole word)

        // string => int  parse + (script runtime?)
        // Searching 1282 files for "GetPatchNumber" (case sensitive, whole word)


        // Default - each channel has its own.
        static readonly Dictionary<int, string> _patches = new();
        //
        static readonly Dictionary<int, string> _controllers = new();
        //
        static readonly Dictionary<int, string> _drumKits = new();
        //
        static readonly Dictionary<int, string> _drums = new();




        /// <summary>Reverse lookup.</summary>
        static readonly Dictionary<string, int> _patchesRev = new();
        /// <summary>Reverse lookup.</summary>
        static readonly Dictionary<string, int> _drumKitsRev = new();
        /// <summary>Reverse lookup.</summary>
        static readonly Dictionary<string, int> _drumsRev = new();
        /// <summary>Reverse lookup.</summary>
        static readonly Dictionary<string, int> _controllersRev = new();



        // /// <summary>
        // /// Get patch name.
        // /// </summary>
        // /// <param name="which"></param>
        // /// <returns>The name.</returns>
        // public static string GetPatchName(int which)
        // {
        //     string ret = which switch
        //     {
        //         -1 => "NoPatch",
        //         >= 0 and < MAX_MIDI => _patchs[which],
        //         _ => throw new ArgumentOutOfRangeException(nameof(which)),
        //     };
        //     return ret;
        // }

        // /// <summary>
        // /// Get the patch/patch number.
        // /// </summary>
        // /// <param name="which"></param>
        // /// <returns>The midi number or -1 if invalid.</returns>
        // public static int GetPatchNumber(string which)
        // {
        //     if (_patchNumbers.ContainsKey(which))
        //     {
        //         return _patchNumbers[which];
        //     }
        //     //throw new ArgumentException($"Invalid patch: {which}");
        //     return -1;
        // }

        /// <summary>
        /// Get drum name.
        /// </summary>
        /// <param name="which"></param>
        /// <returns>The drum name or a fabricated one if unknown.</returns>
        public static string GetDrumName(int which)
        {
            return _drums.ContainsKey(which) ? _drums[which] : $"DRUM_{which}";
        }

        /// <summary>
        /// Get drum number.
        /// </summary>
        /// <param name="which"></param>
        /// <returns>The midi number or -1 if invalid.</returns>
        public static int GetDrumNumber(string which)
        {
            if (_drumsRev.ContainsKey(which))
            {
                return _drumsRev[which];
            }
            //throw new ArgumentException($"Invalid drum: {which}");
            return -1;
        }

        /// <summary>
        /// Get controller name.
        /// </summary>
        /// <param name="which"></param>
        /// <returns>The controller name or a fabricated one if unknown.</returns>
        public static string GetControllerName(int which)
        {
            return _controllers.ContainsKey(which) ? _controllers[which] : $"CTLR_{which}";
        }

        /// <summary>
        /// Get the controller number.
        /// </summary>
        /// <param name="which"></param>
        /// <returns>The midi number or -1 if invalid.</returns>
        public static int GetControllerNumber(string which)
        {
            if (_controllersRev.ContainsKey(which))
            {
                return _controllersRev[which];
            }
            //throw new ArgumentException($"Invalid controller: {which}");
            return -1;
        }

        /// <summary>
        /// Get GM drum kit name.
        /// </summary>
        /// <param name="which"></param>
        /// <returns>The drumkit name or a fabricated one if unknown.</returns>
        public static string GetDrumKitName(int which)
        {
            return _drumKits.ContainsKey(which) ? _drumKits[which] : $"KIT_{which}";
        }

        /// <summary>
        /// Get GM drum kit number.
        /// </summary>
        /// <param name="which"></param>
        /// <returns>The midi number or -1 if invalid.</returns>
        public static int GetDrumKitNumber(string which)
        {
            if(_drumKitsRev.ContainsKey(which))
            {
                return _drumKitsRev[which];
            }
            //throw new ArgumentException($"Invalid drum kit: {which}");
            return -1;
        }

        // /// <summary>
        // /// Get the patch/patch or drum number.
        // /// </summary>
        // /// <param name="which"></param>
        // /// <returns>The midi number or -1 if invalid.</returns>
        // public static int GetPatchOrDrumKitNumber(string which)
        // {
        //     if (_patchNumbers.ContainsKey(which))
        //     {
        //         return _patchNumbers[which];
        //     }
        //     else if (_drumKitNumbers.ContainsKey(which))
        //     {
        //         return _drumKitNumbers[which];
        //     }
        //     //throw new ArgumentException($"Invalid patch or drum: {which}");
        //     return -1;
        // }


        //public static List<string> FormatDoc()
        //{
        //    return ["aaa"];
        //}
        #endregion

        #region New or modified TODO2

        /// <summary>
        /// Initialize some collections.
        /// </summary>
        static MidiDefs()
        {
            bool valid = true;

            try
            {
                _patches = DoOne(@"C:\Dev\Apps\MidiGenerator\gm_patches.ini");
                _controllers = DoOne(@"C:\Dev\Apps\MidiGenerator\gm_controllers.ini");
                _drums = DoOne(@"C:\Dev\Apps\MidiGenerator\gm_drums.ini");
                _drumKits = DoOne(@"C:\Dev\Apps\MidiGenerator\gm_drumkits.ini");

                // TODO1 reverses?
                // _patchNumbers = _patchs.ToDictionary(x => x, x => _patchs.IndexOf(x));
                // _drumKitNumbers = _drumKits.ToDictionary(x => x.Value, x => x.Key);
                // _drumNumbers = _drums.ToDictionary(x => x.Value, x => x.Key);
                // _controllerNumbers = _controllers.ToDictionary(x => x.Value, x => x.Key);

            }
            catch (Exception)
            {
                //TODO1 notify?
                valid = false;
            }
        }

        ///////////////////
        public static Dictionary<int, string> DoOne(string fn)
        {
            Dictionary<int, string> res = [];

            var ir = new IniReader(fn);

            var defs = ir.Contents["midi_defs"];

            defs.Values.ForEach(kv =>
            {
                // ["011", "GlassFlute"]
                int index = int.Parse(kv.Key); // can throw
                if (index < 0 || index > MidiDefs.MAX_MIDI) { throw new Exception(); }
                res[index] = kv.Value.Length > 0 ? kv.Value : "";
            });

            return res;
        }



        // public enum DefType { Patch, Controller, Drum, DrumKit }

        // static readonly Dictionary<DefType, string[]> _defs = [];

        // public static bool Load(DefType dt, string fn)
        // {
        //     bool valid = true;
        //     try
        //     {
        //         var vals = Utils.CreateInitializedMidiArray(dt.ToString());

        //         var ir = new IniReader(fn);

        //         var defs = ir.Contents["midi_defs"];

        //         defs.Values.ForEach(kv =>
        //         {
        //             // ["011", "GlassFlute"]
        //             int index = int.Parse(kv.Key); // can throw
        //             if (index < 0 || index > MidiDefs.MAX_MIDI) { throw new Exception(); }
        //             vals[index] = kv.Value.Length > 0 ? kv.Value : "";
        //         });

        //         _defs[dt] = vals;



        //         //// Read file lines: 011 GlassFlute -> vals[11] = "GlassFlute"
        //         //var ls = Utils.LoadDefFile(fn);

        //         //var vals = xxxnew string[MidiDefs.MAX_MIDI + 1];
        //         //for (int i = 0; i < vals.Length; i++) // set defaults
        //         //{
        //         //    vals[i] = $"Patch{i}";
        //         //}

        //         //foreach (var parts in ls)
        //         //{
        //         //    // ["011", "GlassFlute"]

        //         //    if (parts.Count != 2)
        //         //    {
        //         //        throw new Exception();
        //         //    }

        //         //    int index = int.Parse(parts[0]); // can throw

        //         //    if (index < 0 || index > MidiDefs.MAX_MIDI)
        //         //    {
        //         //        throw new Exception();
        //         //    }

        //         //    if (parts[1].Length > 0)
        //         //    {
        //         //        vals[index] = parts[1];
        //         //    }

        //         //    _defs[dt] = vals;
        //         //}
        //     }
        //     catch (Exception)
        //     {
        //         //TODO1 notify?
        //         valid = false;
        //     }

        //     return valid;
        // }
        #endregion
    }

    public class MidiLibDefs // ==> MidiCommon.cs
    {
        /// <summary>The normal drum channel.</summary>
        public const int DEFAULT_DRUM_CHANNEL = 10;

        /// <summary>Default value.</summary>
        public const double DEFAULT_VOLUME = 0.8;

        /// <summary>Allow UI controls some more headroom.</summary>
        public const double MAX_VOLUME = 2.0;
    }

    /// <summary>
    /// Midi (real or sim) has received something. It's up to the client to make sense of it.
    /// Property value of -1 indicates invalid or not pertinent e.g a controller event doesn't have velocity.
    /// </summary>
    public class InputReceiveEventArgs : EventArgs // ==> IDevice.cs
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


    /// <summary>Abstraction layer to support all midi-like devices.</summary>
    public interface IDevice : IDisposable // ==> IDevice.cs
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
    public interface IInputDevice : IDevice // ==> IDevice.cs
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
    public interface IOutputDevice : IDevice // ==> IDevice.cs
    {
        #region Properties

        #endregion

        #region Functions
        /// <summary>Send midi event.</summary>
        /// <param name="evt"></param>
        void SendEvent(MidiEvent evt);
        #endregion
    }



    /// <summary>
    /// Midi input handler.
    /// </summary>
    public sealed class MidiInput : IInputDevice // ==> MidiInput.cs
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



    /// <summary>
    /// A midi output layer - associated with a single device.
    /// </summary>
    public sealed class MidiOutput : IOutputDevice // ==> MidiOutput.cs
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

