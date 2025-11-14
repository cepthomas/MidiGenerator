using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Ephemera.NBagOfTricks;


namespace MidiGenerator
{
    #region Events - prob just this app? TODO1
    public class NoteEventArgs : EventArgs
    {
        /// <summary>The note number to play.</summary>
        public int Note { get; set; }

        /// <summary>0 to 127.</summary>
        public int Velocity { get; set; }

        /// <summary>Read me.</summary>
        public override string ToString()
        {
            return $"Note:{Note} Velocity:{Velocity}";
        }
    }

    public class ControllerEventArgs : EventArgs
    {
        /// <summary>Specific controller id.</summary>
        public int Controller { get; set; }

        /// <summary>Payload.</summary>
        public int Value { get; set; }

        /// <summary>Read me.</summary>
        public override string ToString()
        {
            return $"Controller:{Controller} Value:{Value}";
         }
    }

    /// <summary>Notify host of UI changes.</summary>
    public class ChannelChangeEventArgs : EventArgs
    {
        public bool PatchChange { get; set; } = false;
      // public bool StateChange { get; set; } = false;
        public bool ChannelNumberChange { get; set; } = false;
    }

    #endregion


    public class Utils
    {
        public static List<List<string>> LoadDefFile(string fn)
        {
            List<List<string>> res = [];

            foreach (var inline in File.ReadAllLines(fn))
            {
                // Clean up line, strip comments.
                var cmt = inline.IndexOf(';');
                var line = cmt >= 0 ? inline[0..cmt] : inline;

                line = line.Trim();

                // Ignore empty lines.
                if (line.Length > 0)
                {
                    List<string> resParts = [];

                    var parts = line.SplitByToken(" ");
                    parts.ForEach(p => resParts.Add(p.Trim()));
                    res.Add(resParts);
                }
            }

            return res;
        }


        public static Keys Translate(char ch)
        {
            Keys xlat = Keys.None;

            switch (ch)
            {
                case ',':  xlat = Keys.Oemcomma; break;
                case '=':  xlat = Keys.Oemplus; break;
                case '-':  xlat = Keys.OemMinus; break;
                case '/':  xlat = Keys.OemQuestion; break;
                case '.':  xlat = Keys.OemPeriod; break;
                case '\'': xlat = Keys.OemQuotes; break;
                case '\\': xlat = Keys.OemPipe; break;
                case ']':  xlat = Keys.OemCloseBrackets; break;
                case '[':  xlat = Keys.OemOpenBrackets; break;
                case '`':  xlat = Keys.Oemtilde; break;
                case ';':  xlat = Keys.OemSemicolon; break;

                default:
                    if ((ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
                    {
                        xlat = (Keys)ch;
                    }
                    break;
            }

            return xlat;
        }
    }
}
