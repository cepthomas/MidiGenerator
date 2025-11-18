using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Ephemera.NBagOfTricks;
using Ephemera.MidiLib;


namespace MidiGenerator
{
    #region Events
    public class UserClickNoteEventArgs : EventArgs
    {
        /// <summary>The note number to play.</summary>
        public int Note { get; set; }

        /// <summary>0 to 127.</summary>
        public int Velocity { get; set; }

        /// <summary>Read me.</summary>
        public override string ToString()
        {
            return $"Note:{MusicDefinitions.NoteNumberToName(Note)}({Note}):{Velocity}";
        }
    }

    public class UserClickControllerEventArgs : EventArgs
    {
        /// <summary>Specific controller id.</summary>
        public int Controller { get; set; }

        /// <summary>Payload.</summary>
        public int Value { get; set; }

        /// <summary>Read me.</summary>
        public override string ToString()
        {
            return $"Controller:{MidiDefs.TheDefs.GetControllerName(Controller)}({Controller}):{Value}";
        }
    }
    #endregion





    public class Utils
    {
        // public static string[] CreateInitializedMidiArray(string id)
        // {
        //     var vals = new string[MidiDefs.MAX_MIDI + 1];

        //     for (int i = 0; i < vals.Length; i++)
        //     {
        //         vals[i] = $"{id}{i}"; // fake for now
        //     }
        //     return vals;
        // }


        ///// <summary>
        ///// Load a standard def file.
        ///// </summary>
        ///// <param name="fn"></param>
        ///// <returns></returns>
        //public static List<List<string>> LoadDefFile(string fn)
        //{
        //    List<List<string>> res = [];
        //    foreach (var inline in File.ReadAllLines(fn))
        //    {
        //        // Clean up line, strip comments.
        //        var cmt = inline.IndexOf(';');
        //        var line = cmt >= 0 ? inline[0..cmt] : inline;
        //        line = line.Trim();
        //        // Ignore empty lines.
        //        if (line.Length > 0)
        //        {
        //            List<string> resParts = [];
        //            var parts = line.SplitByToken(" ");
        //            parts.ForEach(p => resParts.Add(p.Trim()));
        //            res.Add(resParts);
        //        }
        //    }
        //    return res;
        //}

        // /// <summary>
        // /// Translate ascii char to Keys.
        // /// </summary>
        // /// <param name="ch"></param>
        // /// <returns></returns>
        // public static Keys TranslateKey(char ch)
        // {
        //     Keys xlat = Keys.None;

        //     switch (ch)
        //     {
        //         case ',':  xlat = Keys.Oemcomma; break;
        //         case '=':  xlat = Keys.Oemplus; break;
        //         case '-':  xlat = Keys.OemMinus; break;
        //         case '/':  xlat = Keys.OemQuestion; break;
        //         case '.':  xlat = Keys.OemPeriod; break;
        //         case '\'': xlat = Keys.OemQuotes; break;
        //         case '\\': xlat = Keys.OemPipe; break;
        //         case ']':  xlat = Keys.OemCloseBrackets; break;
        //         case '[':  xlat = Keys.OemOpenBrackets; break;
        //         case '`':  xlat = Keys.Oemtilde; break;
        //         case ';':  xlat = Keys.OemSemicolon; break;
        //         case (>= 'A' and <= 'Z') or (>= '0' and <= '9'): xlat = (Keys)ch; break;
        //     }

        //     return xlat;
        // }
    }



}
