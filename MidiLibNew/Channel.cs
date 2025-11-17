using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Windows.Forms.Design;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using NAudio.Midi;
using Ephemera.NBagOfTricks;


namespace Ephemera.MidiLib
{
    /// <summary>Describes one midi output channel. Some properties are optional.</summary>
    [Serializable]
    public class Channel
    {
        #region Fields
#if _FULL

        ///<summary>The collection of playable events for this channel and pattern. Key is the internal sub/time.</summary>
        readonly Dictionary<int, List<MidiEvent>> _events = [];

        /// <summary>Things that are executed once and disappear: NoteOffs, script send now. Key is the internal sub/time.</summary>
        readonly Dictionary<int, List<MidiEvent>> _transients = [];

        ///<summary>Current volume.</summary>
        double _volume = MidiLibDefs.DEFAULT_VOLUME;
#endif
        #endregion


////////////////// ADDED ///////////////////////////////
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
            set { _volume = MathUtils.Constrain(value, 0.0, MidiLibDefs.MAX_VOLUME); }
        }
        double _volume = MidiLibDefs.DEFAULT_VOLUME;

        /// <summary>All possible instrument/patch.</summary>
        //[Browsable(false)]
        //[JsonIgnore]
        //public Dictionary<int, string> Instruments { get; set; } = [];

        Dictionary<int, string> _instruments = MidiDefs.TheDefs.GetDefaultInstrumentDefs();



        /// <summary>Override default instrument presets.</summary>
        [Browsable(true)]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string PresetFile
        {
            get { return _presetFile; }
            set
            {
                //if (value == "") // use defaults
                //{
                //    Instruments = MidiDefs.GetDefaultInstrumentDefs();
                //}
                //else // load override
                //{
                //    if (!File.Exists(value)) throw new FileNotFoundException(x);
                //    Instruments = LibUtils.LoadDefs(value);
                //}
                _presetFile = value;
            }
        }
        string _presetFile = "";

        /// <summary>Edit current instrument/patch number.</summary>
        [Browsable(true)]
        [Editor(typeof(PatchTypeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(PatchConverter))]
        //public int Patch { get; set; } = 0;
        public int Patch
        {
            get { return _patch; }
            set { _patch = MathUtils.Constrain(value, 0, MidiDefs.MAX_MIDI); }
        }
        int _patch = 0;
        #endregion


        /// <summary>
        /// Get patch name.
        /// </summary>
        /// <param name="which"></param>
        /// <returns>The name.</returns>
        public string GetPatchName()
        {
            if (_instruments.Count == 0)
            {
                _instruments = MidiDefs.TheDefs.GetDefaultInstrumentDefs();

                //if (value == "") // use defaults
                //{
                //    Instruments = MidiDefs.GetDefaultInstrumentDefs();
                //}
                //else // load override
                //{
                //    if (!File.Exists(value)) throw new FileNotFoundException(x);
                //    Instruments = LibUtils.LoadDefs(value);
                //}


            }
            string ret = _instruments[Patch];



            //string ret = Patch switch
            //{
            //    -1 => "NoPatch",
            //    >= 0 and < MidiDefs.MAX_MIDI => Instruments[Patch],
            //    _ => throw new ArgumentOutOfRangeException(nameof(Patch)),
            //};
            return ret;
        }

        ////////////////// ADDED end ///////////////////////////////



        #region Properties
#if _FULL
        /// <summary>For muting/soloing.</summary>
        [JsonIgnore]
        public ChannelState State { get; set; } = ChannelState.Normal;

        /// <summary>Actual 1-based midi channel number - required.</summary>
        [JsonIgnore]
        public int ChannelNumber { get; set; } = -1;

        /// <summary>Current patch.</summary>
        [JsonIgnore]
        public int Patch { get; set; } = -1;

        /// <summary>Current volume constrained to legal values.</summary>
        [JsonIgnore]
        public double Volume
        {
            get { return _volume; }
            set { _volume = MathUtils.Constrain(value, 0.0, MidiLibDefs.MAX_VOLUME); }
        }

        /// <summary>Add a ghost note off for note on.</summary>
        [JsonIgnore]
        public bool AddNoteOff { get; set; } = false;

        /// <summary>Optional UI label/reference.</summary>
        [JsonIgnore]
        public string ChannelName { get; set; } = "";

        /// <summary>Drums may be handled differently.</summary>
        [JsonIgnore]
        public bool IsDrums { get; set; } = false;

        /// <summary>For UI user selection.</summary>
        [JsonIgnore]
        public bool Selected { get; set; } = false;

        ///<summary>The duration of the whole channel - calculated.</summary>
        [JsonIgnore]
        public int MaxSub { get; private set; } = 0;

        /// <summary>Get the number of events - calculated.</summary>
        [JsonIgnore]
        public int NumEvents { get { return _events.Count; } }

        /// <summary>The device used by this channel. Used to find and bind the device at runtime.</summary>
        public string DeviceId { get; set; } = "";
#endif

        /// <summary>Associated device.</summary>
        [Browsable(false)]
        [JsonIgnore]
        public IOutputDevice? Device { get; set; } = null;
        #endregion

        #region Functions

#if _FULL
        /// <summary>
        /// Set the time-ordered events for the channel.
        /// </summary>
        /// <param name="events"></param>
        public void SetEvents(IEnumerable<MidiEventDesc> events)
        {
            // Reset.
            _events.Clear();
            MaxSub = 0;

            // Bin by sub.
            foreach (var te in events)
            {
                // Add to our collection.
                if (!_events.TryGetValue(te.ScaledTime, out List<MidiEvent>? value))
                {
                    value = [];
                    _events.Add(te.ScaledTime, value);
                }

                value.Add(te.RawEvent);
                MaxSub = Math.Max(MaxSub, te.ScaledTime);
            }
        }
#endif

        /// <summary>
        /// Clean the events for the channel.
        /// </summary>
        public void Reset()
        {
#if _FULL
            // Reset.
            _events.Clear();
            _transients.Clear();
            MaxSub = 0;
            State = ChannelState.Normal;
            Selected = false;
            IsDrums = false;
#endif
            Patch = 0;
        }


#if _FULL
        /// <summary>
        /// Get the events for a specific sub.
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        public IEnumerable<MidiEvent> GetEvents(int sub)
        {
            return _events.TryGetValue(sub, out List<MidiEvent>? value) ? value : [];
        }

        /// <summary>
        /// Get all events.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<List<MidiEvent>> GetAllEvents()
        {
            return _events.Values;
        }

        /// <summary>
        /// Process any events for this time.
        /// </summary>
        /// <param name="sub"></param>
        public void DoStep(int sub)
        {
            // Main events.
            if(_events.TryGetValue(sub, out List<MidiEvent>? value))
            {
                foreach (var evt in value)
                {
                    switch (evt)
                    {
                        case FunctionMidiEvent fe:
                            fe.ScriptFunction?.Invoke();
                            break;

                        default:
                            SendEvent(evt);
                            break;
                    }
                }
            }

            // Transient events.
            if (_transients.TryGetValue(sub, out List<MidiEvent>? tvalue))
            {
                foreach (var evt in tvalue)
                {
                    SendEvent(evt);
                }
                _transients.Remove(sub);
            }
        }

        /// <summary>
        /// Execute any lingering transients and clear the collection.
        /// </summary>
        /// <param name="sub">After this time.</param>
        public void Flush(int sub)
        {
            _transients.Where(t => t.Key >= sub).ForEach(t => t.Value.ForEach(evt => SendEvent(evt)));
            _transients.Clear();
        }
#endif

        /// <summary>
        /// General patch sender.
        /// </summary>
        public void SendPatch()
        {
            if(Patch >= MidiDefs.MIN_MIDI && Patch <= MidiDefs.MAX_MIDI)
            {
                PatchChangeEvent evt = new(0, ChannelNumber, Patch);
                SendEvent(evt);
            }
        }

        /// <summary>
        /// Send a controller now.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="val"></param>
        public void SendController(MidiController controller, int val)
        {
            ControlChangeEvent evt = new(0, ChannelNumber, controller, val);
            SendEvent(evt);
        }

        /// <summary>
        /// Send midi all notes off.
        /// </summary>
        public void Kill()
        {
            SendController(MidiController.AllNotesOff, 0);
        }

        /// <summary>
        /// Generic event sender.
        /// </summary>
        /// <param name="evt"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SendEvent(MidiEvent evt)
        {
            if (Device is null)
            {
                throw new InvalidOperationException("Device not set");
            }
#if _FULL

            // If note on, add a transient note off for later.
            if(AddNoteOff && evt is NoteOnEvent)
            {
                var nevt = evt as NoteOnEvent;
                int offTime = (int)evt.AbsoluteTime + nevt!.NoteLength;
                if (!_transients.TryGetValue(offTime, out List<MidiEvent>? value))
                {
                    value = [];
                    _transients.Add(offTime, value);
                }

                value.Add(nevt.OffEvent);
            }
#endif
            // Now send it.
            Device.SendEvent(evt);
        }
#endregion
    }

#if _FULL

    /// <summary>Helper extension methods.</summary>
    public static class ChannelUtils
    {
        /// <summary>
        /// Any solo in collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channels"></param>
        /// <returns></returns>
        public static bool AnySolo<T>(this Dictionary<string, T> channels) where T : Channel
        {
            var solo = channels.Values.Where(c => c.State == ChannelState.Solo).Any();
            return solo;
        }

        /// <summary>
        /// Get subs for the collection, rounded to beat.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channels"></param>
        /// <returns></returns>
        public static int TotalSubs<T>(this Dictionary<string, T> channels) where T : Channel
        {
            var chmax = channels.Values.Max(ch => ch.MaxSub);
            // Round total up to next beat.
            BarTime bs = new();
            bs.SetRounded(chmax, SnapType.Beat, true);
            return bs.TotalSubs;
        }
    }
#endif

}
