using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Text.Json.Serialization;
using System.Windows.Forms.Design;
using System.IO;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;


namespace MidiGenerator
{
    [Serializable]
    public sealed class UserSettings : SettingsCore
    {
        #region Persisted editable properties
        [DisplayName("Draw Color")]
        [Description("The color used for active control surfaces.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonColorConverter))]
        public Color DrawColor { get; set; } = Color.Red;

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
        [Editor(typeof(GenericListTypeEditor), typeof(UITypeEditor))]
        public string OutputDevice { get; set; } = "???";
        #endregion

        #region Persisted non-editable properties
        // [Browsable(false)]
        // public Channel VkeyChannel { get; set; } = new();

        // [Browsable(false)]
        // public Channel ClClChannel { get; set; } = new();

        [Browsable(false)]
        public bool LogMidi { get; set; } = false;
        #endregion
    }
}
