using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations;
using Ephemera.NBagOfTricks;
using Ephemera.MusicLib;
using Ephemera.MidiLib;


namespace MidiGenerator
{
    //public class Defs
    //{
    //    /// <summary>Default value.</summary>
    //    public const double DEFAULT_VOLUME = 0.8;

    //    /// <summary>Allow UI controls some more headroom.</summary>
    //    public const double MAX_VOLUME = 2.0;
    //}

    #region Events
    /// <summary>Notify host of user clicks.</summary>
    public class NoteEventArgs : EventArgs
    {
        /// <summary>The note number to play.</summary>
        [Required]
        public int Note { get; set; }

        /// <summary>0 to 127.</summary>
        [Required]
        public int Velocity { get; set; }

        /// <summary>Read me.</summary>
        public override string ToString()
        {
            return $"Note:{MusicDefs.Instance.NoteNumberToName(Note)}({Note}):{Velocity}";
        }
    }

    /// <summary>Notify host of user clicks.</summary>
    public class ControllerEventArgs : EventArgs
    {
        /// <summary>Specific controller id.</summary>
        [Required]
        public int ControllerId { get; set; }

        /// <summary>Payload.</summary>
        [Required]
        public int Value { get; set; }

        /// <summary>Read me.</summary>
        public override string ToString()
        {
            return $"ControllerId:{MidiDefs.Instance.GetControllerName(ControllerId)}({ControllerId}):{Value}";
        }
    }

    /// <summary>Notify host of user clicks.</summary>
    public class ChannelEventArgs : EventArgs
    {
        public bool PatchChange { get; set; } = false;
        public bool ChannelNumberChange { get; set; } = false;
        public bool PresetFileChange { get; set; } = false;
    }
    #endregion

    //public class Utils
    //{
    //    // Load a standard midi def file.
    //    public static Dictionary<int, string> LoadDefs(string fn)
    //    {
    //        Dictionary<int, string> res = [];

    //        var ir = new IniReader(fn);

    //        var defs = ir.Contents["midi_defs"];

    //        defs.Values.ForEach(kv =>
    //        {
    //            int index = int.Parse(kv.Key); // can throw
    //            if (index < 0 || index > MidiDefs.MAX_MIDI) { throw new InvalidOperationException($"Invalid def file {fn}"); }
    //            res[index] = kv.Value.Length > 0 ? kv.Value : "";
    //        });

    //        return res;
    //    }
    //}
}
