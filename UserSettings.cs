using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Text.Json;
using System.Text.Json.Serialization;
using NAudio.Midi;
using NBagOfTricks;
using NBagOfUis;
using MidiLib;


namespace MidiGenerator
{
    [Serializable]
    public class UserSettings : Settings
    {
        #region Persisted editable properties
        [DisplayName("Control Color")]
        [Description("Pick what you like.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonColorConverter))]
        public Color ControlColor { get; set; } = Color.MediumOrchid;

        [DisplayName("Midi Settings")]
        [Description("Edit midi settings.")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MidiSettings MidiSettings { get; set; } = new();
        #endregion

        #region Persisted Non-editable Properties
        [Browsable(false)]
        public ChannelSettings VkeyChannel { get; set; } = new();

        [Browsable(false)]
        public ChannelSettings BingBongChannel { get; set; } = new();

        [Browsable(false)]
        public bool LogMidi { get; set; } = false;
        #endregion

        #region Non-persisted Properties
        //[Browsable(false)]
        //public bool Valid { get; set; } = false;
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
        public double Volume { get; set; } = VolumeDefs.DEFAULT;
        #endregion
    }
}
