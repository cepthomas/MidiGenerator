using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
}



    // /// <summary>Notify host of asynchronous changes from user.</summary>
    // public class ChannelChangeEventArgs : EventArgs
    // {
    // }

    // /// <summary>
    // /// Midi (real or sim) has received something. It's up to the client to make sense of it.
    // /// Property value of -1 indicates invalid or not pertinent e.g a controller event doesn't have velocity.
    // /// </summary>
    // public class InputReceiveEventArgs : EventArgs
    // {
    //     /// <summary>Channel number 1-based. Required.</summary>
    //     public int Channel { get; set; } = 0;

    //     /// <summary>The note number to play. NoteOn/Off only.</summary>
    //     public int Note { get; set; } = -1;

    //     /// <summary>Specific controller id.</summary>
    //     public int Controller { get; set; } = -1;

    //     /// <summary>For Note = velocity. For controller = payload.</summary>
    //     public int Value { get; set; } = -1;

    //     /// <summary>Something to tell the client.</summary>
    //     public string ErrorInfo { get; set; } = "";

    //     /// <summary>Special controller id to carry pitch info.</summary>
    //     public const int PITCH_CONTROL = 1000;
    // }
