using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Text.Json.Serialization;
using System.Windows.Forms.Design;
using Ephemera.NBagOfTricks;


using MidiLibNew;


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




    // [Serializable]
    // public class DeviceSpec
    // {
    //     [DisplayName("Device Id")]
    //     [Description("User supplied id for use in client.")]
    //     [Browsable(true)]
    //     public string DeviceId { get; set; } = "";

    //     [DisplayName("Device Name")]
    //     [Description("System device name.")]
    //     [Browsable(true)]
    //     public string DeviceName { get; set; } = "";
    // }

        // [DisplayName("Input Devices")]
        // [Description("Valid devices if handling input.")]
        // [Browsable(true)]
        // [Editor(typeof(DevicesTypeEditor), typeof(UITypeEditor))]
        // public List<DeviceSpec> InputDevices { get; set; } = new();

        // [DisplayName("Output Devices")]
        // [Description("Valid devices if sending output.")]
        // [Browsable(true)]
        // [Editor(typeof(DevicesTypeEditor), typeof(UITypeEditor))]
        // public List<DeviceSpec> OutputDevices { get; set; } = new();






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
        [Browsable(true)]
        [Editor(typeof(ChannelSelectorTypeEditor), typeof(UITypeEditor))]
        public int ChannelNumber { get; set; } = 1;

        /// <summary>Current midi presets file.</summary>
        [Browsable(true)]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string PresetFile { get; set; } = "";

        /// <summary>Current patch.</summary>
        [Browsable(true)]
        [Editor(typeof(PatchSelectorTypeEditor), typeof(UITypeEditor))]
        public int Patch { get; set; } = 0;

        /// <summary>Current volume.</summary>
        [Browsable(false)]
        public double Volume { get; set; } = Defs.DEFAULT_VOLUME;
        #endregion
    }
}
