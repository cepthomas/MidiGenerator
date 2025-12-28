using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ephemera.MidiLib;
using Ephemera.NBagOfTricks;


namespace MidiGenerator
{
    public class ClickClack : UserControl
    {
        #region Fields
        /// <summary>Required designer variable.</summary>
        readonly Container components = new();

        /// <summary>Background image data.</summary>
        PixelBitmap? _bmp;

        /// <summary>Last key down position in client coordinates.</summary>
        int _lastNote = -1;

        /// <summary>The grid pen.</summary>
        readonly Pen _pen = new(Color.WhiteSmoke, 1);

        /// <summary>Tool tip.</summary>
        readonly ToolTip _toolTip = new();

        /// <summary>Ranges.</summary>
        const int MIN_X = 24; // C0
        const int MAX_X = 96; // C6
        const int MIN_Y = 0;
        const int MAX_Y = 128;
        #endregion

        #region Properties
        /// <summary>Cosmetics.</summary>
        public Color DrawColor { get; set; } = Color.Red;
        #endregion

        #region Events
        /// <summary>UI midi send. Client must fill in the channel number.</summary>
        public event EventHandler<BaseMidi>? SendMidi;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        public ClickClack()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Name = nameof(ClickClack);
            _toolTip = new(components);
            Size = new(300, 190);
        }

        /// <summary>
        /// Init after properties set.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            DrawBitmap();
            base.OnLoad(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components.Dispose();
            }

            _bmp?.Dispose();
            _pen.Dispose();

            base.Dispose(disposing);
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Paint the surface.
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;

            // Background?
            if (_bmp is not null)
            {
                g.DrawImage(_bmp.ClientBitmap, ClientRectangle);
            }

            // Draw grid. X is octaves. Y is volume.
            foreach (var gl in new int[] { 36, 48, 60, 72, 84 })
            {
                if (gl >= MIN_X && gl <= MAX_X) // sanity - throw?
                {
                    int x = MathUtils.Map(gl, MIN_X, MAX_X, 0, Width);
                    g.DrawLine(_pen, x, 0, x, ClientRectangle.Height);
                }
            }

            foreach (var gl in new int[] { 20, 40, 60, 80, 100, 120 })
            {
                if (gl >= MIN_Y && gl <= MAX_Y)
                {
                    int y = MathUtils.Map(gl, MIN_Y, MAX_Y, Height, 0);
                    g.DrawLine(_pen, 0, y, ClientRectangle.Width, y);
                }
            }

            base.OnPaint(pe);
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Show the pixel info.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            var res = MouseToUser();

            if (res is not null)
            {
                _toolTip.SetToolTip(this, $"X:{res.Value.ux} Y:{res.Value.uy}");

                // Also gen click?
                if (e.Button == MouseButtons.Left)
                {
                    // Dragging. Did it change?
                    if (_lastNote != res.Value.ux)
                    {
                        if (_lastNote != -1)
                        {
                            // Turn off last note.
                            Send(new NoteOff(0, _lastNote));
                        }

                        // Start the new note.
                        _lastNote = res.Value.ux;
                        Send(new NoteOn(0, res.Value.ux, res.Value.uy));
                    }
                }
            }

           base.OnMouseMove(e);
        }

        /// <summary>
        /// Send info to client.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            var res = MouseToUser();
            if (res is not null)
            {
                _lastNote = res.Value.ux;
                Send(new NoteOn(0, res.Value.ux, res.Value.uy));
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Send info to client.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_lastNote != -1)
            {
                Send(new NoteOff(0, _lastNote));
                _lastNote = -1;
            }

            base.OnMouseUp(e);
        }

        /// <summary>
        /// Disable control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            // Turn off last click.
            if (_lastNote != -1)
            {
                Send(new NoteOff(0, _lastNote));
            }

            // Reset and tell client.
            _lastNote = -1;

            base.OnMouseLeave(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            DrawBitmap();
            Invalidate();

            base.OnResize(e);
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Render.
        /// </summary>
        void DrawBitmap()
        {
            // Clean up old.
            _bmp?.Dispose();

            // Draw background.
            var w = ClientRectangle.Width;
            var h = ClientRectangle.Height;
            _bmp = new(w, h);
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    _bmp!.SetPixel(x, y, Color.FromArgb(255, x * 256 / w, y * 256 / h, 150));
                }
            }
        }

        /// <summary>
        /// Get mouse x and y mapped to useful coordinates.
        /// </summary>
        /// <returns>Tuple of x and y.</returns>
        (int ux, int uy)? MouseToUser()
        {
            // Map and check.
            var mp = PointToClient(MousePosition);
            int x = MathUtils.Map(mp.X, ClientRectangle.Left, ClientRectangle.Right, 0, MidiDefs.MAX_MIDI);
            int y = MathUtils.Map(mp.Y, ClientRectangle.Bottom, ClientRectangle.Top, 0, MidiDefs.MAX_MIDI);
            return (x, y);
        }

        /// <summary>
        /// Helper.
        /// </summary>
        /// <param name="e"></param>
        void Send(BaseMidi e)
        {
            // Send from parent ChannelControl!
            SendMidi?.Invoke(Parent, e);
        }
        #endregion
    }
}
