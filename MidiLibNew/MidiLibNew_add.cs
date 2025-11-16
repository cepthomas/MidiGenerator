using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ephemera.NBagOfTricks;
using MidiGenerator;
using NAudio.Midi;


// Copy of stuff from original MidiLib to support minimal apps. Works as is unless noted.

namespace Ephemera.MidiLib
{
    public class MidiDefsX // ==> MidiDefs.cs
    {
        /// <summary>Midi caps.</summary>
        public const int MIN_MIDI = 0;

        /// <summary>Midi caps.</summary>
        public const int MAX_MIDI = 127;

        /// <summary>Midi caps.</summary>
        public const int NUM_CHANNELS = 16;

        #region New or modified TODO2
        /// <summary>
        /// Initialize some collections.
        /// </summary>
        static MidiDefsX()
        {
            bool valid = true;

            try
            {
                // _patches = DoOne(@"C:\Dev\Apps\MidiGenerator\gm_patches.ini");
                // _controllers = DoOne(@"C:\Dev\Apps\MidiGenerator\gm_controllers.ini");
                // _drums = DoOne(@"C:\Dev\Apps\MidiGenerator\gm_drums.ini");
                // _drumKits = DoOne(@"C:\Dev\Apps\MidiGenerator\gm_drumkits.ini");

                // reverses
                // _patchNumbers = _patchs.ToDictionary(x => x, x => _patchs.IndexOf(x));
                // _drumKitNumbers = _drumKits.ToDictionary(x => x.Value, x => x.Key);
                // _drumNumbers = _drums.ToDictionary(x => x.Value, x => x.Key);
                // _controllerNumbers = _controllers.ToDictionary(x => x.Value, x => x.Key);

            }
            catch (Exception)
            {
                // notify?
                valid = false;
            }
        }

        ///////////////////
        #endregion
    }

    public class MidiLibDefsX // ==> MidiCommon.cs
    {
        /// <summary>The normal drum channel.</summary>
        public const int DEFAULT_DRUM_CHANNEL = 10;

        /// <summary>Default value.</summary>
        public const double DEFAULT_VOLUME = 0.8;

        /// <summary>Allow UI controls some more headroom.</summary>
        public const double MAX_VOLUME = 2.0;
    }
}
