using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text.Json.Serialization;
using Ephemera.NBagOfTricks;


namespace MidiGenerator
{
    [Serializable]
    public sealed class UserSettings : SettingsCore
    {
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

        #region Non-persisted Properties
        #endregion
    }

    [Serializable]
    public class ChannelSettings
    {
        #region Persisted Non-editable Properties
        /// <summary>Actual 1-based midi channel number.</summary>
        [Browsable(false)]
        public int ChannelNumber { get; set; } = 1;

        /// <summary>Current patch.</summary>
        [Browsable(false)]
        public int Patch { get; set; } = 0;

        /// <summary>Current volume.</summary>
        [Browsable(false)]
        public double Volume { get; set; } = Defs.VOLUME_DEFAULT;
        #endregion
    }
}
