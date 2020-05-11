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

        }

        #endregion

        #region mouseOverride

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
            ((Form)Tag).SetDesktopLocation(Cursor.Position.X - mx,Cursor.Position.Y - my);
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
