using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.Remoting.Contexts;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Button for MochaDB Studio.
    /// </summary>
    public class sbutton:Control {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public sbutton() {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,true);
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            using(var bgBrush = new SolidBrush(
                !Enabled ? Color.Gray : MouseDown ? MouseDownColor : MouseEnter ? MouseEnterColor : BackColor))
                e.Graphics.FillRectangle(bgBrush,ClientRectangle);
        }

        protected override void OnPaint(PaintEventArgs e) {
            if(string.IsNullOrWhiteSpace(Text)) {
                if(Image != null) {
                    e.Graphics.DrawImage(Image,ClientRectangle);
                }
                return;
            }

            using(var centerFormat = new StringFormat() {
                FormatFlags = StringFormatFlags.NoWrap,
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            using(var foreBrush = new SolidBrush(ForeColor)) {
                if(Image != null) {
                    e.Graphics.DrawImage(Image,0,5,25,Height-10);
                }
                e.Graphics.DrawString(Text,Font,foreBrush,ClientRectangle,centerFormat);
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

        protected override void OnMouseDown(MouseEventArgs e) {
            MouseDown = true;
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            MouseDown = false;
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            if(ContextMenu != null) {
                if(DisableClick || e.Button == MouseButtons.Right)
                    if(ContextMenu.Size.Height + Location.Y + Height <= Screen.FromControl(this).WorkingArea.Height)
                        ContextMenu.Show(this,new Point(0,Location.Y + Height));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// ContextMenu.
        /// </summary>
        public sContextMenu ContextMenu { get; set; }

        /// <summary>
        /// Disable mouse left click.
        /// </summary>
        public bool DisableClick { get; set; }

        /// <summary>
        /// Image.
        /// </summary>
        public Bitmap Image { get; set; }

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

        private bool
            mouseDown = false;
        /// <summary>
        /// Mouse down state of control.
        /// </summary>
        public bool MouseDown {
            get {
                return mouseDown;
            }
            protected set {
                if(value == mouseDown)
                    return;

                mouseDown = value;
                Invalidate();
            }
        }

        private Color
            mouseDownColor = Color.Gray;
        /// <summary>
        /// Color of MouseDown.
        /// </summary>
        public Color MouseDownColor {
            get {
                return mouseDownColor;
            }
            set {
                if(value == mouseDownColor)
                    return;

                mouseDownColor = value;
                if(MouseDown)
                    Invalidate();
            }
        }

        #endregion
    }
}
