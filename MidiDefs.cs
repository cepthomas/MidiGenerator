using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using NAudio.Midi;
using Ephemera.NBagOfTricks;


namespace MidiGenerator
{
    /// <summary>Stuff like readable versions of midi numbers.</summary>
    public class MidiDefs
    {
        /// <summary>The singleton.</summary>
        public static MidiDefs TheDefs { get; set; } = new();

        #region Constants - midi spec
        /// <summary>Midi caps.</summary>
        public const int MIN_MIDI = 0;

        /// <summary>Midi caps.</summary>
        public const int MAX_MIDI = 127;

        /// <summary>Midi caps.</summary>
        public const int NUM_CHANNELS = 16;

        /// <summary>The normal drum channel.</summary>
        public const int DEFAULT_DRUM_CHANNEL = 10;
        #endregion

        #region Fields
        /// <summary>All the GM instruments - default.</summary>
        readonly Dictionary<int, string> _instruments;
        
        /// <summary>All the GM CCs.</summary>
        readonly Dictionary<int, string> _controllers;
        
        /// <summary>All the GM drums.</summary>
        readonly Dictionary<int, string> _drums;
        
        /// <summary>All the GM drum kits.</summary>
        readonly Dictionary<int, string> _drumKits;
        #endregion

        /// <summary>
        /// Initialize some collections.
        /// </summary>
        public MidiDefs()
        {
            // TODO1 files from where?
            _instruments = Utils.LoadDefs(@"C:\Dev\Apps\MidiGenerator\gm_instruments.ini");
            _controllers = Utils.LoadDefs(@"C:\Dev\Apps\MidiGenerator\gm_controllers.ini");
            _drums = Utils.LoadDefs(@"C:\Dev\Apps\MidiGenerator\gm_drums.ini");
            _drumKits = Utils.LoadDefs(@"C:\Dev\Apps\MidiGenerator\gm_drumkits.ini");
        }

        #region API  TODO1 clean up debris
        /// <summary>Default instruments.</summary>
        /// <returns></returns>
        public Dictionary<int, string> GetDefaultInstrumentDefs()
        {
            return _instruments;
        }

        /// <summary>
        /// Make markdown content from the definitions.
        /// </summary>
        /// <returns>Markdown content.</returns>
        public List<string> FormatDoc()//TODO1
        {
            List<string> docs = new();
            docs.Add("# Midi GM Instruments");
            docs.Add("Instrument          | Number");
            docs.Add("----------          | ------");
            Enumerable.Range(0, _instruments.Count).ForEach(i => docs.Add($"{_instruments[i]}|{i}"));
            docs.Add("# Midi GM Drums");
            docs.Add("Drum                | Number");
            docs.Add("----                | ------");
            _drums.ForEach(kv => docs.Add($"{kv.Value}|{kv.Key}"));
            docs.Add("# Midi GM Controllers");
            docs.Add("- Undefined: 3, 9, 14-15, 20-31, 85-90, 102-119");
            docs.Add("- For most controllers marked on/off, on=127 and off=0");
            docs.Add("Controller          | Number");
            docs.Add("----------          | ------");
            _controllers.ForEach(kv => docs.Add($"{kv.Value}|{kv.Key}"));
            docs.Add("# Midi GM Drum Kits");
            docs.Add("Note that these will vary depending on your Soundfont file.");
            docs.Add("Kit        | Number");
            docs.Add("-----------| ------");
            _drumKits.ForEach(kv => docs.Add($"{kv.Value}|{kv.Key}"));

            return docs;
        }

        /// <summary>
        /// Get drum name.
        /// </summary>
        /// <param name="which"></param>
        /// <returns>The drum name or a fabricated one if unknown.</returns>
        public string GetDrumName(int which)
        {
            return _drums.ContainsKey(which) ? _drums[which] : $"DRUM_{which}";
        }

        // /// <summary>
        // /// Get drum number.
        // /// </summary>
        // /// <param name="which"></param>
        // /// <returns>The midi number or -1 if invalid.</returns>
        // public static int GetDrumNumber(string which)
        // {
        //     if (_drumNumbers.ContainsKey(which))
        //     {
        //         return _drumNumbers[which];
        //     }
        //     //throw new ArgumentException($"Invalid drum: {which}");
        //     return -1;
        // }

        /// <summary>
        /// Get controller name.
        /// </summary>
        /// <param name="which"></param>
        /// <returns>The controller name or a fabricated one if unknown.</returns>
        public string GetControllerName(int which)
        {
            return _controllers.ContainsKey(which) ? _controllers[which] : $"CTLR_{which}";
        }

        // /// <summary>
        // /// Get the controller number.
        // /// </summary>
        // /// <param name="which"></param>
        // /// <returns>The midi number or -1 if invalid.</returns>
        // public static int GetControllerNumber(string which)
        // {
        //     if (_controllerNumbers.ContainsKey(which))
        //     {
        //         return _controllerNumbers[which];
        //     }
        //     //throw new ArgumentException($"Invalid controller: {which}");
        //     return -1;
        // }

        /// <summary>
        /// Get GM drum kit name.
        /// </summary>
        /// <param name="which"></param>
        /// <returns>The drumkit name or a fabricated one if unknown.</returns>
        public string GetDrumKitName(int which)
        {
            return _drumKits.ContainsKey(which) ? _drumKits[which] : $"KIT_{which}";
        }

        // /// <summary>
        // /// Get GM drum kit number.
        // /// </summary>
        // /// <param name="which"></param>
        // /// <returns>The midi number or -1 if invalid.</returns>
        // public static int GetDrumKitNumber(string which)
        // {
        //     if(_drumKitNumbers.ContainsKey(which))
        //     {
        //         return _drumKitNumbers[which];
        //     }
        //     //throw new ArgumentException($"Invalid drum kit: {which}");
        //     return -1;
        // }

        // /// <summary>
        // /// Get the instrument/patch or drum number.
        // /// </summary>
        // /// <param name="which"></param>
        // /// <returns>The midi number or -1 if invalid.</returns>
        // public static int GetInstrumentOrDrumKitNumber(string which)
        // {
        //     if (_instrumentNumbers.ContainsKey(which))
        //     {
        //         return _instrumentNumbers[which];
        //     }
        //     else if (_drumKitNumbers.ContainsKey(which))
        //     {
        //         return _drumKitNumbers[which];
        //     }
        //     //throw new ArgumentException($"Invalid instrument or drum: {which}");
        //     return -1;
        // }
        #endregion
    }
}    