using System;
using System.Drawing;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Context menu item for MochaDB Studio.
    /// </summary>
    public class sContextMenuItem:ToolStripMenuItem {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public sContextMenuItem() {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">Text.</param>
        public sContextMenuItem(string text) {
            Text = text;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">Text.</param>
        public sContextMenuItem(string text,Color backColor,Color mouseEnterColor) {
            Text = text;
            BackColor = backColor;
            MouseEnterColor = mouseEnterColor;
        }

        #endregion

        #region Drawing

        protected override void OnPaint(PaintEventArgs e) {
            using(var bgBrush = new SolidBrush(
                MouseEnter ? MouseEnterColor : BackColor))
                e.Graphics.FillRectangle(bgBrush,0,0,Width,Height);

            using(var centerFormat = new StringFormat() {
                LineAlignment = StringAlignment.Center
            })
            using(var foreBrush = new SolidBrush(ForeColor)) {
                if(Image != null) {
                    e.Graphics.DrawImage(Image,10,5,15,Height-10);
                    e.Graphics.DrawString(Text,Font,foreBrush,new
                        Rectangle(new Point(ContentRectangle.X + 30,0),ContentRectangle.Size),
                        centerFormat);
                } else {
                    e.Graphics.DrawString(Text,Font,foreBrush,ContentRectangle);
                }
            }
        }

        #endregion

        #region MouseOverride

        protected override void OnMouseEnter(EventArgs e) {
            MouseEnter = true;
        }

        protected override void OnMouseLeave(EventArgs e) {
            MouseEnter = false;
        }

        #endregion

        #region Properties

        private bool
            mouseEnter = false;
        /// <summary>
        /// Mouse hover state of control.
        /// </summary>
        public bool MouseEnter {
            get {
                return mouseEnter;
            }
            protected set {
                if(value == mouseEnter)
                    return;

                mouseEnter = value;
                Invalidate();
            }
        }

        private Color
            mouseEnterColor = Color.Gray;
        /// <summary>
        /// Color of MouseEnter.
        /// </summary>
        public Color MouseEnterColor {
            get {
                return mouseEnterColor;
            }
            set {
                if(value == mouseEnterColor)
                    return;

                mouseEnterColor = value;
                if(MouseEnter)
                    Invalidate();
            }
        }

        #endregion
    }
}
