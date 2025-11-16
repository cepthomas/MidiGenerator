using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Text.Json.Serialization;
using System.Windows.Forms.Design;
using System.IO;
using Ephemera.NBagOfTricks;
using Ephemera.MidiLib;


namespace MidiGenerator
{
    [Serializable]
    public sealed class UserSettings : SettingsCore
    {
        // /// <summary>The current settings.</summary>
        // public static UserSettings Current { get; set; } = new();

        #region Persisted editable properties
        [DisplayName("Control Color")]
        [Description("The color used for active control surfaces.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonColorConverter))]
        public Color ControlColor { get; set; } = Color.MediumOrchid;

        [DisplayName("File Log Level")]
        [Description("Log level for file write.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel FileLogLevel { get; set; } = LogLevel.Trace;

        [DisplayName("File Log Level")]
        [Description("Log level for UI notification.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel NotifLogLevel { get; set; } = LogLevel.Debug;

        [DisplayName("Output Device")]
        [Description("Valid output device.")]
        [Browsable(true)]
        [Editor(typeof(DevicesTypeEditor), typeof(UITypeEditor))]
        public string OutputDevice { get; set; } = "???";
        #endregion

        #region Persisted Non-editable Properties
        [Browsable(false)]
        public ChannelSettings VkeyChannel { get; set; } = new();

        [Browsable(false)]
        public ChannelSettings ClickClackChannel { get; set; } = new();

        [Browsable(false)]
        public bool LogMidi { get; set; } = false;
        #endregion
    }

    [Serializable]
    public class ChannelSettings
    {
        #region Persisted Non-editable Properties
        /// <summary>Actual 1-based midi channel number.</summary>
        [Browsable(true)]
        [Editor(typeof(ChannelSelectorTypeEditor), typeof(UITypeEditor))]
        public int ChannelNumber
        {
            get { return _channelNumber; }
            set { _channelNumber = MathUtils.Constrain(value, 1, MidiDefs.NUM_CHANNELS); }
        }
        int _channelNumber = 1;


        /// <summary>Current volume.</summary>
        [Browsable(false)]
        public double Volume
        {
            get { return _volume; }
            set { _volume = MathUtils.Constrain(value, 0.0, MidiLibDefs.MAX_VOLUME); }
        }
        double _volume = MidiLibDefs.DEFAULT_VOLUME;





//        public static Dictionary<int, string> DoOne(string fn)



        /// <summary>All possible instrument/patch.</summary>
        [Browsable(false)]
        [JsonIgnore]
        public Dictionary<int, string>? Patches { get; set; }// = new string[MidiDefs.MAX_MIDI + 1];
        // public string[] Patches { get; set; }// = new string[MidiDefs.MAX_MIDI + 1];


        /// <summary>Override default instrument presets.</summary>
        [Browsable(true)]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string PresetFile
        {
            get { return _presetFile; }
            //set { _presetFile = value; }
            
            set
            {
                if (value == "") // use defaults
                {
                    Patches = null;
                    //MidiDefs.Load(MidiDefs.DefType.Patch, @"C:\Dev\Apps\MidiGenerator\gm_instruments.ini");
                }
                else // load override
                {
                    if (!File.Exists(value)) throw new FileNotFoundException();
                    Patches = MidiDefs.DoOne(value);
                }
                _presetFile = value;
            }

            //TODO1 >>> MidiDefs.Load(MidiDefs.DefType.Patch, @"C:\Dev\Apps\MidiGenerator\exp_instruments.txt");
            // or reset to original
        }
        string _presetFile = "";


        /// <summary>Edit current instrument/patch number.</summary>
        [Browsable(true)]
        [Editor(typeof(PatchTypeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(PatchConverter))]
        public int Patch { get; set; } = 0;







        #endregion


        //[Browsable(false)]
        //public Presets Presets { get; set; } = new();
    }
}
