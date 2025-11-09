using System;
using System.Collections.Generic;
// using System.ComponentModel;
// using System.Data;
// using System.Drawing;
// using System.Drawing.Imaging;
using System.Linq;
// using System.Runtime.InteropServices;
using System.Text;
// using System.Threading.Tasks;
// using System.Windows.Forms;
// using Ephemera.NBagOfTricks;
// using Ephemera.NBagOfUis;
using Ephemera.MidiLib;


namespace MidiGenerator
{
    /// <summary>
    /// xxxx
    /// </summary>
    // public class Common
    // {
    //     /// <summary>Lowest note.</summary>
    //     public int MinNote { get; set; } = 24;

    //     /// <summary>Highest note.</summary>
    //     public int MaxNote { get; set; } = 95;

    //     /// <summary>Min control value. For velocity = off.</summary>
    //     public int MinControl { get; set; } = MidiDefs.MIN_MIDI;

    //     /// <summary>Max control value. For velocity = loudest.</summary>
    //     public int MaxControl { get; set; } = MidiDefs.MAX_MIDI;

    //     /// <summary>Special controller id to carry pitch info.</summary>
    //     public const int PITCH_CONTROL = 1000;
    // }


    #region Events
    public class NoteEventArgs : EventArgs
    {
        ///// <summary>The X value in user coordinates. null means invalid.</summary>
        //public int? X { get; set; } = null;

        ///// <summary>The Y value in user coordinates. -1 means unclicked. null means invalid.</summary>
        //public int? Y { get; set; } = null;

        // /// <summary>For tooltip provided by the owner.</summary>
        // public string Text { get; set; } = "";

        /// <summary>The note number to play. NoteOn/Off only.</summary>
        public int Note { get; set; }

        /// <summary>For Note = velocity.</summary>
        public double Volume { get; set; }

        /// <summary>Something to tell the client.</summary>
        public string ErrorInfo { get; set; } = "";


        /// <summary>Read me.</summary>
        public override string ToString()
        {
            return ErrorInfo != "" ? $"Error:{ErrorInfo}" : $"Note:{Note} Volume:{Volume}";
            // StringBuilder sb = new($"Channel:{Channel} ");
            // if (ErrorInfo != "")
            // {
            //     sb.Append($"Error:{ErrorInfo} ");
            // }
            // else
            // {
            //     sb.Append($"Channel:{Channel} Note:{Note} Controller:{Controller} Value:{Value}");
            // }
            // return sb.ToString();
        }
    }

    public class ControllerEventArgs : EventArgs
    {
        ///// <summary>The X value in user coordinates. null means invalid.</summary>
        //public int? X { get; set; } = null;

        ///// <summary>The Y value in user coordinates. -1 means unclicked. null means invalid.</summary>
        //public int? Y { get; set; } = null;

        // /// <summary>For tooltip provided by the owner.</summary>
        // public string Text { get; set; } = "";

        // /// <summary>The note number to play. NoteOn/Off only.</summary>
        // public int? Note { get; set; } = null;

        /// <summary>Specific controller id.</summary>
        public int Controller { get; set; }

        /// <summary>For controller = payload.</summary>
        public int Value { get; set; }

        /// <summary>Something to tell the client.</summary>
        public string ErrorInfo { get; set; } = "";

        /// <summary>Read me.</summary>
        public override string ToString()
        {
            return ErrorInfo != "" ? $"Error:{ErrorInfo}" : $"Controller:{Controller} Value:{Value}";
            
            // StringBuilder sb = new($"Channel:{Channel} ");
            // if (ErrorInfo != "")
            // {
            //     sb.Append($"Error:{ErrorInfo} ");
            // }
            // else
            // {
            //     sb.Append($"Channel:{Channel} Note:{Note} Controller:{Controller} Value:{Value}");
            // }
            // return sb.ToString();
        }
    }


    #endregion
}
