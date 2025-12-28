using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Text.Json.Serialization;
using System.Windows.Forms.Design;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;
using Ephemera.MidiLib;


namespace MidiGenerator
{
    [Serializable]
    public sealed class ChannelConfig
    {
        [DisplayName("Channel Number")]
        [Description("Actual 1-based midi channel number.")]
        [Browsable(true)]
        [Editor(typeof(MidiValueTypeEditor), typeof(UITypeEditor))]
        public int ChannelNumber { get; set; } = 1;

        [DisplayName("Channel Patch")]
        [Description("Current instrument/patch number.")]
        [Browsable(true)]
        [Range(0, MidiDefs.MAX_MIDI)]
        [Editor(typeof(GenericListTypeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(GenericConverter))]
        public int Patch { get; set; } = 0;
    }

    [Serializable]
    public sealed class UserSettings : SettingsCore
    {
        [DisplayName("Output Device")]
        [Description("Valid output device.")]
        [Browsable(true)]
        [Editor(typeof(GenericListTypeEditor), typeof(UITypeEditor))]
        public string OutputDevice { get; set; } = "???";

        [DisplayName("Virtual Keyboard")]
        [Description("Config.")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ChannelConfig VkeyChannel { get; set; } = new();

        [DisplayName("Click Clack")]
        [Description("Config.")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ChannelConfig ClClChannel { get; set; } = new();

        [DisplayName("Draw Color")]
        [Description("The color used for active control surfaces.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonColorConverter))]
        public Color DrawColor { get; set; } = Color.Red;

        [DisplayName("Selected Color")]
        [Description("The color used for when control is selected.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonColorConverter))]
        public Color SelectedColor { get; set; } = Color.Blue;

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

        [Browsable(false)]
        public bool LogMidi { get; set; } = false;
    }
}
