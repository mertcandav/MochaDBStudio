using System.Drawing;
using System.Windows.Forms;

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

        protected override void OnMouseDoubleClick(MouseEventArgs e) {
            base.OnMouseDoubleClick(e);

            ((Form)Tag).WindowState =
                ((Form)Tag).WindowState == FormWindowState.Maximized ?
                FormWindowState.Normal :
                FormWindowState.Maximized;
        }

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
            if(my < 0 | mx < 0)
                return;
            var form = (Form)Tag;
            form.SetDesktopLocation(Cursor.Position.X - mx,Cursor.Position.Y - my);
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
