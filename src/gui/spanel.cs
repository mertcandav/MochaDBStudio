using System.Drawing;
using System.Windows.Forms;
using MochaDBStudio.engine;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Panel for MochaDB Studio.
    /// </summary>
    public class spanel:Panel {
        #region Fields

        private bool
            mouseDown = false;

        private int
            mx = -1,
            my = -1;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public spanel() {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer,true);
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            using(var brush = new SolidBrush(BackColor))
                e.Graphics.FillRectangle(brush,0,0,Width,Height);

            if(BackgroundImage != null)
                e.Graphics.DrawImage(BackgroundImage,
                    (Width / 2) - 50,(Height/2)-50,100,100);
        }

        #endregion

        #region Mouse Override

        protected override void OnMouseDown(MouseEventArgs e) {
            mx = e.X;
            my = e.Y + 40;
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            mx = -1;
            my = -1;
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if(!Moveable)
                return;
            api.ReleaseCapture();
            api.SendMessage(((Control)Parent).Handle,api.WM_NCLBUTTONDOWN,api.HT_CAPTION,0);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Set desktop location with mouse move event if true.
        /// </summary>
        public bool Moveable { get; set; }

        #endregion
    }
}
