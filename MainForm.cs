using System;
using System.Collections.Generic;
// using System.ComponentModel;
// using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
// using System.Threading.Tasks;
using System.Windows.Forms;
// using System.IO;
using System.Diagnostics;
// using System.Drawing.Drawing2D;
// using System.Drawing.Imaging;
using NAudio.Midi;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;
//using Ephemera.MidiLib;


//try
//{
//}
//catch (Exception ex)
//{
//    _logger.Exception(ex);
//}


namespace MidiGenerator
{
    public partial class MainForm : Form
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("MainForm");

        /// <summary>User settings.</summary>
        readonly UserSettings _settings;

        /// <summary>Midi output component.</summary>
//        readonly IOutputDevice _outputDevice = new NullOutputDevice();

        /// <summary>The fast timer.</summary>
        readonly MmTimerEx _mmTimer = new();
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm()
        {
            // Must do this first before initializing.
            string appDir = MiscUtils.GetAppDataDir("MidiGenerator", "Ephemera");

            var vvv = SettingsCore.Load(appDir, typeof(UserSettings));


            _settings = (UserSettings)SettingsCore.Load(appDir, typeof(UserSettings));
            // Tell the libs about their settings.
            //MidiSettings.LibSettings = _settings.MidiSettings;
            //C:\Users\cepth\AppData\Roaming\Ephemera\MidiGenerator\settings.json
            //

            InitializeComponent();

            // Init logging.
            LogManager.MinLevelFile = _settings.FileLogLevel;
            LogManager.MinLevelNotif = _settings.NotifLogLevel;
            LogManager.LogMessage += LogManager_LogMessage;
            LogManager.Run();

            ///// Configure UI. /////
            toolStrip1.Renderer = new ToolStripCheckBoxRenderer() { SelectedColor = _settings.ControlColor };
            txtViewer.Font = Font;
            txtViewer.MatchText.Add("ERR", Color.LightPink);
            txtViewer.MatchText.Add("WRN", Color.Plum);

//            About_Click(null, null);

            btnLogMidi.Checked = _settings.LogMidi;
//            _outputDevice.LogEnable = _settings.LogMidi;
            btnKillMidi.Click += (_, __) =>
            {
                Kill(_settings.VkeyChannel.ChannelNumber);
                Kill(_settings.ClickClackChannel.ChannelNumber);
            };

            ///// Set up midi device. /////
            //for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            //{
            //    Debug.WriteLine($"output:{MidiOut.DeviceInfo(i).ProductName}");
            //}
            //for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            //{
            //    Debug.WriteLine($"input:{MidiIn.DeviceInfo(i).ProductName}");
            //}
            //output: VirtualMIDISynth #1
            //output:Microsoft GS Wavetable Synth
            //output: loopMIDI Port 2
            //output: loopMIDI Port 1
            //input: loopMIDI Port 2
            //input: loopMIDI Port 1


            /// <summary>
            /// Normal constructor.
            /// </summary>
            /// <param name="deviceName">Client must supply name of device.</param>
            // Figure out which midi output device.
            string deviceName = "VirtualMIDISynth #1";
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                if (deviceName == MidiOut.DeviceInfo(i).ProductName)
                {
                    _midiOut = new MidiOut(i);
                    break;
                }
            }

            if (_midiOut is null)
            {
                _logger.Error($"Invalid midi output device:{deviceName}");
            }

            //foreach (var dev in _settings.MidiSettings.OutputDevices)
            //{
            //    _outputDevice = new MidiOutput(dev.DeviceName);
            //    if (_outputDevice.Valid)
            //    {
            //        break;
            //    }
            //}
            //if (!_outputDevice.Valid)
            //{
            //    _logger.Error($"Invalid midi output device:{_outputDevice.DeviceName}");
            //}




            ///// Create the channels and their corresponding controls. /////
            //Channel chVkey = new()
            //{
            //    ChannelName = $"vkey",
            //    ChannelNumber = _settings.VkeyChannel.ChannelNumber,
            //    Device = _outputDevice,
            //    DeviceId = _outputDevice.DeviceName,
            //    Volume = MidiLibDefs.VOLUME_DEFAULT,
            //    State = ChannelState.Normal,
            //    Patch = _settings.VkeyChannel.Patch,
            //    IsDrums = false,
            //    Selected = false,
            //};
            //ctrlVkey.BoundChannel = chVkey;
            ctrlVkey.SelectedColor = _settings.ControlColor;
            //ctrlVkey.Volume = 999;
            ctrlVkey.ChannelNumber = _settings.VkeyChannel.ChannelNumber;
            ctrlVkey.Patch = _settings.VkeyChannel.Patch;
            ctrlVkey.ChannelChange += Channel_ChannelChange;
            // Good time to send initial patch.
            SendPatch(ctrlVkey.ChannelNumber, ctrlVkey.Patch);

            //Channel chCc = new()
            //{
            //    ChannelName = $"cc",
            //    ChannelNumber = _settings.ClickClackChannel.ChannelNumber,
            //    Device = _outputDevice,
            //    DeviceId = _outputDevice.DeviceName,
            //    Volume = MidiLibDefs.VOLUME_DEFAULT,
            //    State = ChannelState.Normal,
            //    Patch = _settings.ClickClackChannel.Patch,
            //    IsDrums = false,
            //    Selected = false,
            //};
            //ctrlCc.BoundChannel = chCc;
            ctrlCc.SelectedColor = _settings.ControlColor;
            //ctrlCc.Volume = 999;
            ctrlCc.ChannelNumber = _settings.ClickClackChannel.ChannelNumber;
            ctrlCc.Patch = _settings.ClickClackChannel.Patch;
            ctrlCc.ChannelChange += Channel_ChannelChange;
            // Good time to send initial patch.
            SendPatch(ctrlCc.ChannelNumber, ctrlCc.Patch);

            ///// Init the fancy controls. /////
            // cc.MinX = 24; // C0
            // cc.MaxX = 96; // C6
            // cc.GridX = [36, 48, 60, 72, 84];
            // cc.MinY = 0; // min velocity == note off
            // cc.MaxY = 127; // max velocity
            // cc.GridY = [32, 64, 96]; //TODO1?? make these doubles

            cc.UserClick += UserClickEvent;
            vkey.UserClick += UserClickEvent;
           // cc.Move += MoveEvent;

            vkey.Enabled = true;
         //   vkey.VkeyClick += VkeyClickEvent;

            ///// Finish up. /////
            Location = _settings.FormGeometry.Location;
            Size = _settings.FormGeometry.Size;
            // Fast timer for future use.
            SetTimer(100);
        }


        // InputReceive?.Invoke(this, new() { Note = note, Value = value or 0 for off
        // void Listener_InputReceive(object? sender, InputReceiveEventArgs e)
        // {
        //     _logger.Trace($"Listener:{sender} Note:{e.Note} Controller:{e.Controller} Value:{e.Value}");

        //     // Translate and pass to output.
        //     var channel = _channels["chan16"];
        //     NoteEvent nevt = e.Value > 0 ?
        //        new NoteOnEvent(0, channel.ChannelNumber, e.Note % MidiDefs.MAX_MIDI, e.Value % MidiDefs.MAX_MIDI, 0) :
        //        new NoteEvent(0, channel.ChannelNumber, MidiCommandCode.NoteOff, e.Note, 0);
        //     channel.SendEvent(nevt);
        // }







        /// <summary>
        /// Form is legal now. Init things that want to log or play with controls..
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            _logger.Info($"Hello!");

            if (_midiOut is null)
            {
                _logger.Error($"Invalid midi output device");
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Save stuff.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LogManager.Stop();
            SaveSettings();

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            // Resources.
            _mmTimer.Stop();
            _mmTimer.Dispose();

            // Wait a bit in case there are some lingering events.
            System.Threading.Thread.Sleep(100);

            _midiOut?.Dispose();

            if (disposing && (components is not null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

/*
        /// <summary>
        /// Create all I/O devices from user settings.
        /// </summary>
        /// <returns>Success</returns>
        bool CreateDevices()
        {
            bool ok = true;

            // First...
            DestroyDevices();

            // Set up input device.
            foreach (var dev in _settings.MidiSettings.InputDevices)
            {
                switch (dev.DeviceName)
                {
                    case nameof(VirtualKeyboard):
                        vkey.InputReceive += Listener_InputReceive;
                        _inputDevice = vkey;
                        break;

                    case nameof(BingBong):
                        bb.InputReceive += Listener_InputReceive;
                        _inputDevice = bb;
                        break;

                    default:
                        // Should be a real device.
                        _inputDevice = new MidiInput(dev.DeviceName);

                        if (!_inputDevice.Valid)
                        {
                            _logger.Error($"Something wrong with your input device:{dev.DeviceName}");
                            ok = false;
                        }
                        else
                        {
                            _inputDevice.CaptureEnable = true;
                            _inputDevice.InputReceive += Listener_InputReceive;
                        }
                        break;
                }
            }

            // Set up output device.
            foreach (var dev in _settings.MidiSettings.OutputDevices)
            {
                switch (dev.DeviceName)
                {
                    default:
                        // Try midi.
                        _outputDevice = new MidiOutput(dev.DeviceName);
                        if (!_outputDevice.Valid)
                        {
                            _logger.Error($"Something wrong with your output device:{_outputDevice.DeviceName}");
                            ok = false;
                        }
                        else
                        {
                            _outputDevice.LogEnable = btnLogMidi.Checked;
                        }
                        break;
                }
            }

            return ok;
        }


        /// <summary>
        /// Clean up.
        /// </summary>
        void DestroyDevices()
        {
            _inputDevice?.Dispose();
            _outputDevice?.Dispose();
        }
*/




        #region MM timer TODO
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempo"></param>
        void SetTimer(double tempo)
        {
            //MidiTimeConverter mt = new(0, tempo);
            //double period = mt.RoundedInternalPeriod();
            //_mmTimer.SetTimer((int)Math.Round(period), MmTimerCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalElapsed"></param>
        /// <param name="periodElapsed"></param>
        void MmTimerCallback(double totalElapsed, double periodElapsed)
        {
            try
            {
                // Could do some timed work here.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region User settings
        /// <summary>
        /// Collect and save user settings.
        /// </summary>
        void SaveSettings()
        {
            _settings.FormGeometry = new Rectangle(Location, Size);

            _settings.VkeyChannel.ChannelNumber = ctrlVkey.ChannelNumber;
            _settings.VkeyChannel.Patch = ctrlVkey.Patch;
            _settings.VkeyChannel.Volume = ctrlVkey.Volume;

            _settings.ClickClackChannel.ChannelNumber = ctrlCc.ChannelNumber;
            _settings.ClickClackChannel.Patch = ctrlCc.Patch;
            _settings.ClickClackChannel.Volume = ctrlCc.Volume;

            _settings.Save();
        }

        /// <summary>
        /// Edit the common options in a property grid.
        /// </summary>
        void Settings_Click(object? sender, EventArgs e)
        {
            SettingsEditor.Edit(_settings, "User Settings", 400);

            MessageBox.Show("Restart required for changes to take effect");

            SaveSettings();
        }
        #endregion





        #region Event handlers

        //void VkeyClickEvent(object? sender, NoteEventArgs e) // TODO1 consolidate both of these
        //{
        //    _logger.Trace($"Vkey N:{e.Note} V:{e.Velocity}");

        //    int chanNum = sender == vkey ?
        //        ctrlVkey.ChannelNumber :
        //        ctrlCc.ChannelNumber;

        //    NoteEvent nevt = e.Velocity > 0 ?
        //        new NoteOnEvent(0, chanNum, e.Note % MidiDefs.MAX_MIDI, e.Velocity % MidiDefs.MAX_MIDI, 0) :
        //        new NoteEvent(0, chanNum, MidiCommandCode.NoteOff, e.Note, 0);

        //    _outputDevice.SendEvent(nevt);

        //    //  .InputReceive += Device_InputReceive;
        //}


        /// <summary>
        /// User clicked something. Send some midi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UserClickEvent(object? sender, NoteEventArgs e)
        {
            if (e.Note != -1 && e.Velocity != -1)
            {
                //string name = ((ClickClack)sender!).Name;
                //int x = (int)e.X; // note
                //int y = (int)e.Y; // velocity

                // InjectMidiInEvent(name, 1, x, y);


                try
                {
                    int chanNum = sender == vkey ?
                    ctrlVkey.ChannelNumber :
                    ctrlCc.ChannelNumber;

                    _logger.Trace($"Ch:{chanNum} N:{e.Note} V:{e.Velocity}");

                    NoteEvent nevt = e.Velocity > 0 ?
                        new NoteOnEvent(0, chanNum, e.Note, e.Velocity, 0) :
                        new NoteEvent(0, chanNum, MidiCommandCode.NoteOff, e.Note, 0);

                    SendEvent(nevt);
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex);
                }
            }
        }

        ///// <summary>
        ///// Do something with events.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void Device_InputReceive(object? sender, InputReceiveEventArgs e)
        //{
        //    _logger.Trace($"VirtDev N:{e.Note} V:{e.Value}");

        //    int chanNum = sender == vkey ?
        //        ctrlVkey.ChannelNumber :
        //        ctrlCc.ChannelNumber;

        //    NoteEvent nevt = e.Value > 0 ?
        //        new NoteOnEvent(0, chanNum, e.Note % MidiDefs.MAX_MIDI, e.Value % MidiDefs.MAX_MIDI, 0) :
        //        new NoteEvent(0, chanNum, MidiCommandCode.NoteOff, e.Note, 0);

        //    _outputDevice.SendEvent(nevt);
        //}



        ///// <summary>
        ///// Provide tool tip text to control.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void CcMoveEvent(object? sender, NoteEventArgs e)
        //{
        //    e.Text = $"{MusicDefinitions.NoteNumberToName((int)e.X!)} V:{e.Y}";
        //}

        /// <summary>
        /// User requests a patch change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Channel_ChannelChange(object? sender, ChannelChangeEventArgs e)
        {
            var cc = sender as ChannelControl;
            if(e.PatchChange || e.ChannelNumberChange)
            {
               // cc!.BoundChannel.SendPatch();
            }
        }

        /// <summary>
        /// Show log events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogManager_LogMessage(object? sender, LogMessageEventArgs e)
        {
            // Usually come from a different thread.
            if (IsHandleCreated)
            {
                this.InvokeIfRequired(_ => { txtViewer.AppendLine($"{e.Message}"); });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogMidi_Click(object? sender, EventArgs e)
        {
            _settings.LogMidi = btnLogMidi.Checked;
         //   _outputDevice.LogEnable = _settings.LogMidi;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////




        /// <summary>Low level midi output device.</summary>
        MidiOut? _midiOut = null;



        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            // Resources.
            _midiOut?.Dispose();
        }



        /// <inheritdoc />
        public void SendEvent(MidiEvent evt)
        {
            _midiOut?.Send(evt.GetAsShortMessage());
            if (_settings.LogMidi)
            {
                _logger.Trace(evt.ToString());
            }
        }




        //private void Send(MidiGenEventArgs e)
        //{
        //    _logger.Trace($"VirtDev N:{e.Note} V:{e.Value}");

        //    int chanNum = sender == vkey ?
        //        ctrlVkey.ChannelNumber :
        //        ctrlCc.ChannelNumber;

        //    NoteEvent nevt = e.Value > 0 ?
        //        new NoteOnEvent(0, chanNum, e.Note % MidiDefs.MAX_MIDI, e.Value % MidiDefs.MAX_MIDI, 0) :
        //        new NoteEvent(0, chanNum, MidiCommandCode.NoteOff, e.Note, 0);

        //    _outputDevice.SendEvent(nevt);

        //    //  .InputReceive += Device_InputReceive;
        //}


        ///// <summary>
        ///// Generic event sender.
        ///// </summary>
        ///// <param name="evt"></param>
        //public void SendEvent(MidiEvent evt)
        //{

        //    _outputDevice.SendEvent(evt);
        //}


        public void SendNote(int chan, int note, int velocity)
        {
            //    _logger.Trace($"Send {evt.GetType()} N:{e.Note} V:{e.Value}");

            NoteEvent evt = velocity > 0 ?
                new NoteOnEvent(0, chan, note % MidiDefs.MAX_MIDI, velocity % MidiDefs.MAX_MIDI, 0) :
                new NoteEvent(0, chan, MidiCommandCode.NoteOff, note, 0);
            SendEvent(evt);
        }



        /// <summary>
        /// General patch sender.
        /// </summary>
        public void SendPatch(int chan, int patch)
        {
            if(patch >= MidiDefs.MIN_MIDI && patch <= MidiDefs.MAX_MIDI)
            {
                PatchChangeEvent evt = new(0, chan, patch);
                SendEvent(evt);
            }
        }

        /// <summary>
        /// Send a controller.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="val"></param>
        public void SendController(int chan, MidiController controller, int val)
        {
            ControlChangeEvent evt = new(0, chan, controller, val);
            SendEvent(evt);
        }

        /// <summary>
        /// Send midi all notes off.
        /// </summary>
        public void Kill(int chan)
        {
            ControlChangeEvent evt = new(0, chan, MidiController.AllNotesOff, 0);
            SendEvent(evt);
        }




        #endregion


        /// <summary>
        /// The meaning of life.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void About_Click(object? sender, EventArgs e)
        {
            // Show the builtin definitions and user devices.
            List<string> ls = [];

            // Show them what they have.
            ls.Add($"# Your Midi Devices");
            ls.Add($"");
            ls.Add($"## Outputs");
            ls.Add($"");
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                ls.Add($"- \"{MidiOut.DeviceInfo(i).ProductName}\"");
            }

            ls.Add($"");
            ls.Add($"## Inputs");
            ls.Add($"");
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                ls.Add($"- \"{MidiIn.DeviceInfo(i).ProductName}\"");
            }

            MessageBox.Show(string.Join(Environment.NewLine, ls));

            //// Generate definitions content.
            //var srcDir = MiscUtils.GetSourcePath().Replace("\\", "/");
            //var luaPath = $"{srcDir}/LBOT/?.lua;{srcDir}/lua/?.lua;;";

            //List<string> s = [
            //    "local mid = require('midi_defs')",
            //    "local mus = require('music_defs')",
            //    "for _,v in ipairs(mid.gen_md()) do print(v) end",
            //    "for _,v in ipairs(mus.gen_md()) do print(v) end",
            //    ];

            //var (_, sres) = ExecuteLuaChunk(s);

            //ls.Add(sres);
            //ls.Add($"");

            //// Show readme.
            //var html = Tools.MarkdownToHtml([.. ls], Tools.MarkdownMode.DarkApi, false);

            //// Show midi stuff.
            //string docfn = Path.GetTempFileName() + ".html";
            //try
            //{
            //    File.WriteAllText(docfn, html);
            //    var proc = new Process { StartInfo = new ProcessStartInfo(docfn) { UseShellExecute = true } };

            //    proc.Exited += (_, __) => File.Delete(docfn);
            //    proc.Start();
            //}
            //catch (Exception ex)
            //{
            //    _loggerApp.Exception(ex);
            //}
        }



    }
}
