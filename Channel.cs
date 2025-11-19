using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using NAudio.Midi;
using Ephemera.NBagOfTricks;
using System.Text.Json.Serialization;


namespace MidiGenerator
{
    /// <summary>Describes one midi output channel. Some properties are optional.</summary>
    [Serializable]
    public class Channel
    {
        #region Fields
        /// <summary>All possible instrument/patch.</summary>
        Dictionary<int, string> _instruments = MidiDefs.TheDefs.GetDefaultInstrumentDefs();
        #endregion

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
            set { _volume = MathUtils.Constrain(value, 0.0, MiscDefs.MAX_VOLUME); }
        }
        double _volume = MiscDefs.DEFAULT_VOLUME;

        /// <summary>Override default instrument presets.</summary>
        [Browsable(true)]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string PresetFile
        {
            get { return _presetFile; }
            set
            {
                _presetFile = value;
            }
        }
        string _presetFile = "";

        /// <summary>Edit current instrument/patch number.</summary>
        [Browsable(true)]
        [Editor(typeof(PatchTypeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(PatchConverter))]
        public int Patch
        {
            get { return _patch; }
            set { _patch = MathUtils.Constrain(value, 0, MidiDefs.MAX_MIDI); }
        }
        int _patch = 0;
        #endregion


        /// <summary>Convenience property.</summary>
        [Browsable(false)]
        [JsonIgnore]
        public Dictionary<int, string> Instruments { get { return _instruments; } }

        #region Misc functions
        /// <summary>Use default or custom presets.</summary>
        /// <exception cref="FileNotFoundException"></exception>
        public void UpdatePresets()
        {
            if (PresetFile != "")
            {
                if (!File.Exists(PresetFile))
                {
                    throw new FileNotFoundException(PresetFile);
                }
                _instruments = Utils.LoadDefs(PresetFile);
            }
            else // use defaults
            {
                _instruments = MidiDefs.TheDefs.GetDefaultInstrumentDefs();
            }
        }
        #endregion
    }
}
