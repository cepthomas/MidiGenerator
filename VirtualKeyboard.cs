using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.ComponentModel;
using Ephemera.NBagOfTricks;
using System.DirectoryServices.ActiveDirectory;


namespace MidiGenerator
{
    /// <summary>Virtual keyboard control borrowed from Leslie Sanford with extras.</summary>
    public class VirtualKeyboard : UserControl
    {
        #region Properties
        /// <summary>Draw the names on the keys.</summary>
        public bool ShowNoteNames { get; set; } = false;

        /// <summary>Determines the overall size.</summary>
        public int KeySize { get; set; } = 14;

        public Color ControlColor { get; set; } = Color.Red;
        #endregion

        #region Events
        /// <summary>Click info.</summary>
        public event EventHandler<UserClickNoteEventArgs>? UserClickNote;
        #endregion

        #region Constants
        //https://studiocode.dev/resources/midi-middle-c/

        /// <summary>Standard 88 keyboard - lowest note A0.</summary>
        const int LOW_NOTE = 21; // A0

        /// <summary>Standard 88 keyboard - highest note C8.</summary>
        const int HIGH_NOTE = 108; // C88

        /// <summary>Standard 88 keyboard - reference C4.</summary>
        const int MIDDLE_C = 60;
        #endregion

        #region Fields
        /// <summary>All the created piano keys.</summary>
        readonly List<VirtualKey> _keys = [];

        /// <summary>Map from Keys value to the index in _keys.</summary>
        readonly Dictionary<Keys, int> _keyMap = [];

        /// <summary>Known bug?</summary>
        bool _keyDown = false;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        public VirtualKeyboard()
        {
            // Intercept all keyboard events.
            // KeyPreview = true;

            Name = "VirtualKeyboard";
            Text = "Virtual Keyboard";

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 140);

            CreateKeys();
            if (CreateKeyMap())
            {
                DrawKeys();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            _keys.ForEach(key => { key.ControlColor = ControlColor ; });
            base.OnLoad(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            DrawKeys();
            Invalidate();

            base.OnResize(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Create the midi note/keyboard mapping.
        /// </summary>
        bool CreateKeyMap()
        {
            _keyMap.Clear();
            bool valid = true;

            try
            {
                var fn = @"defs\def_keymap.ini";
                var ir = new IniReader(fn);

                var defs = ir.Contents["keymap"];

                defs.Values.ForEach(kv => 
                {
                    // Z = C3
                    if (kv.Key.Length != 1) { throw new InvalidOperationException($"Invalid key {kv.Key} in {fn}"); }

                    Keys chkey = Keys.None;
                    switch (kv.Key[0])
                    {
                        case ',':  chkey = Keys.Oemcomma; break;
                        case '=':  chkey = Keys.Oemplus; break;
                        case '-':  chkey = Keys.OemMinus; break;
                        case '/':  chkey = Keys.OemQuestion; break;
                        case '.':  chkey = Keys.OemPeriod; break;
                        case '\'': chkey = Keys.OemQuotes; break;
                        case '\\': chkey = Keys.OemPipe; break;
                        case ']':  chkey = Keys.OemCloseBrackets; break;
                        case '[':  chkey = Keys.OemOpenBrackets; break;
                        case '`':  chkey = Keys.Oemtilde; break;
                        case ';':  chkey = Keys.OemSemicolon; break;
                        case (>= 'A' and <= 'Z') or (>= '0' and <= '9'): chkey = (Keys)kv.Key[0]; break;
                        default: throw new InvalidOperationException($"Invalid key {kv.Key} in {fn}");
                    }

                    var notes = MusicDefinitions.GetNotesFromString(kv.Value);
                    if (notes.Count != 1) { throw new InvalidOperationException($"Invalid key {kv.Key} in {fn}"); }

                    var note = notes[0];
                    _keyMap.Add(chkey, note);
                });
            }
            catch (Exception)
            {
                valid = false;
            }

            return valid;
        }
        #endregion

        #region User input handlers
        /// <summary>
        /// Use alpha keyboard to drive piano.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!_keyDown)
            {
                if (_keyMap.TryGetValue(e.KeyCode, out int value))
                {
                    VirtualKey pk = _keys[value];
                    if (!pk.IsPressed)
                    {
                        pk.PressVKey(100);
                        e.Handled = true;
                    }
                }

                _keyDown = true;
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Use alpha keyboard to drive piano.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            _keyDown = false;

            if (_keyMap.TryGetValue(e.KeyCode, out int value))
            {
                VirtualKey pk = _keys[value];
                pk.ReleaseVKey();
                e.Handled = true;
            }
            base.OnKeyUp(e);
        }

        /// <summary>
        /// Pass along an event from a virtual key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HandleKeyClick(object? sender, UserClickNoteEventArgs e)
        {
            UserClickNote?.Invoke(this, e);
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Create the key controls.
        /// </summary>
        void CreateKeys()
        {
            _keys.Clear();

            for (int i = 0; i < HIGH_NOTE - LOW_NOTE; i++)
            {
                int noteId = i + LOW_NOTE;
                VirtualKey pk = new(this, noteId);

                // Pass along an event from a virtual key.
                pk.KeyClickEvent += HandleKeyClick;

                _keys.Add(pk);
                Controls.Add(pk);

                if (!MusicDefinitions.IsNatural(noteId))
                {
                    pk.BringToFront();
                }
            }
        }

        /// <summary>
        /// Re/draw the keys.
        /// </summary>
        void DrawKeys()
        {
            if(_keys.Count > 0)
            {
                int whiteKeyWidth = _keys.Count * KeySize / _keys.Count(k => MusicDefinitions.IsNatural(k.NoteId));
                int blackKeyWidth = (int)(whiteKeyWidth * 0.6);
                int whiteKeyHeight = Height; // KeyHeight
                int blackKeyHeight = (int)(whiteKeyHeight * 0.65);
                int offset = whiteKeyWidth - blackKeyWidth / 2;

                int numWhiteKeys = 0;

                for (int i = 0; i < _keys.Count; i++)
                {
                    VirtualKey pk = _keys[i];

                    // Note that controls have to have integer width so resizing is a bit lumpy.
                    if (MusicDefinitions.IsNatural(pk.NoteId))
                    {
                        pk.Height = whiteKeyHeight;
                        pk.Width = whiteKeyWidth;
                        pk.Location = new Point(numWhiteKeys * whiteKeyWidth, 0);
                        numWhiteKeys++;
                    }
                    else
                    {
                        pk.Height = blackKeyHeight;
                        pk.Width = blackKeyWidth;
                        pk.Location = new Point(offset + (numWhiteKeys - 1) * whiteKeyWidth);
                        pk.BringToFront();
                    }
                }
            }
        }
        #endregion
    }

    /// <summary>One individual key.</summary>
    public class VirtualKey : Control
    {
        #region Fields
        /// <summary>Hook to owner.</summary>
        readonly VirtualKeyboard _owner;
        #endregion

        #region Properties
        /// <summary>Make user pick a good color.</summary>
        public Color ControlColor { get; set; } = Color.Red;

        /// <summary>Key status.</summary>
        public bool IsPressed { get; private set; } = false;

        /// <summary>Associated midi note.</summary>
        public int NoteId { get; private set; } = 0;
        #endregion

        #region Events
        /// <summary>Notify handlers of key change.</summary>
        public event EventHandler<UserClickNoteEventArgs>? KeyClickEvent;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="noteId"></param>
        public VirtualKey(VirtualKeyboard owner, int noteId)
        {
            _owner = owner;
            TabStop = false;
            NoteId = noteId;
            Font = new Font("Consolas", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }
        #endregion

        #region Public functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="velocity"></param>
        public void PressVKey(int velocity)
        {
            IsPressed = true;
            Invalidate();
            KeyClickEvent?.Invoke(this, new() { Note = NoteId, Velocity = velocity });
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReleaseVKey()
        {
            IsPressed = false;
            Invalidate();
            KeyClickEvent?.Invoke(this, new() { Note = NoteId, Velocity = 0 });
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Calc velocity from Y position.
        /// </summary>
        /// <returns></returns>
        int CalcVelocity()
        {
            var p = PointToClient(Cursor.Position);
            var vel = p.Y * 127 / Height;
            return vel;
        }
        #endregion

        #region Mouse handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (MouseButtons == MouseButtons.Left)
            {
                PressVKey(CalcVelocity());
            }
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsPressed)
            {
                ReleaseVKey();
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            PressVKey(CalcVelocity());

            if (!_owner.Focused)
            {
                _owner.Focus();
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            ReleaseVKey();
            base.OnMouseUp(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.X < 0 || e.X > Width || e.Y < 0 || e.Y > Height)
            {
                Capture = false;
            }
            base.OnMouseMove(e);
        }
        #endregion

        #region Draw the control
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (IsPressed)
            {
                e.Graphics.FillRectangle(new SolidBrush(ControlColor), 0, 0, Size.Width, Size.Height);
            }
            else
            {
                e.Graphics.FillRectangle(MusicDefinitions.IsNatural(NoteId) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black), 0, 0, Size.Width, Size.Height);
            }

            // Outline.
            e.Graphics.DrawRectangle(Pens.Black, 0, 0, Size.Width - 1, Size.Height - 1);

            // Note name. Just C to minimize clutter.
            if(_owner.ShowNoteNames)
            {
                int root = NoteId % 12;
                int octave = (NoteId / 12) - 1;

                if (root == 0)
                {
                    int x = MusicDefinitions.IsNatural(NoteId) ? 3 : 0;
                    e.Graphics.DrawString($"{MusicDefinitions.NoteNumberToName(root, false)}", Font, Brushes.Black, x, 3);
                    e.Graphics.DrawString($"{octave}", Font, Brushes.Black, x, 13);
                }
            }
        }
        #endregion
    }
}
