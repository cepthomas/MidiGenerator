using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ephemera.NBagOfTricks;


namespace MidiGenerator
{
    public class ClickClack : ChannelControl
    {
        #region Fields
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

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        public ClickClack()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Name = nameof(ClickClack);
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
            _bmp?.Dispose();
            _pen.Dispose();
            base.Dispose(disposing);
        }
        #endregion

        #region Event handlers
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
                g.DrawImage(_bmp.ClientBitmap, DrawRect);
            }

            // Draw grid. X is octaves. Y is volume.
            foreach (var gl in new int[] { 36, 48, 60, 72, 84 })
            {
                if (gl >= MIN_X && gl <= MAX_X) // sanity - throw?
                {
                    int x = MathUtils.Map(gl, MIN_X, MAX_X, 0, Width);
                    g.DrawLine(_pen, x, 0, x, Height);
                }
            }

            foreach (var gl in new int[] { 20, 40, 60, 80, 100, 120 })
            {
                if (gl >= MIN_Y && gl <= MAX_Y)
                {
                    int y = MathUtils.Map(gl, MIN_Y, MAX_Y, Height, 0);
                    g.DrawLine(_pen, 0, y, Width, y);
                }
            }

            base.OnPaint(pe);
        }

        /// <summary>
        /// Show the pixel info.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            var (ux, uy) = MouseToUser();
            NoteEventArgs args = new() { Note = ux, Velocity = uy };
            _toolTip.SetToolTip(this, args.ToString());

            // Also gen click?
            if (e.Button == MouseButtons.Left)
            {
                // Dragging. Did it change?
                if (_lastNote != ux)
                {
                    if (_lastNote != -1)
                    {
                        // Turn off last note.
                        OnNoteSend(new() { Note = _lastNote, Velocity = 0 });
                    }

                    // Start the new note.
                    _lastNote = ux;
                    OnNoteSend(new() { Note = ux, Velocity = uy });
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
            var (ux, uy) = MouseToUser();
            _lastNote = ux;
            OnNoteSend(new() { Note = ux, Velocity = uy });

            base.OnMouseDown(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_lastNote != -1)
            {
                OnNoteSend(new() { Note = _lastNote, Velocity = 0 });
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
                OnNoteSend(new() { Note = _lastNote, Velocity = 0 });
            }

            // Reset and tell client.
            _lastNote = -1;

            _toolTip.SetToolTip(this, "");

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
            var w = DrawRect.Width;
            var h = DrawRect.Height;
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
        /// Get mouse x and y mapped to user coordinates.
        /// </summary>
        /// <returns>Tuple of x and y.</returns>
        (int ux, int uy) MouseToUser()
        {
            var mp = PointToClient(MousePosition);

            // Map and check.
            int x = MathUtils.Map(mp.X, 0, Width, MIN_X, MAX_X);
            int ux = x >= 0 && x < Width ? x : -1;
            int y = MathUtils.Map(mp.Y, Height, DrawRect.Top, MIN_Y, MAX_Y);
            int uy = y >= 0 && y < Height ? y : -1;

            return (ux, uy);
        }
        #endregion
    }
}
