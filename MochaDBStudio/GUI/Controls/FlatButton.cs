using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace MochaDBStudio.GUI.Controls {
    /// <summary>
    /// Style button for gui.
    /// </summary>
    public sealed class FlatButton:Control {
        #region Fields

        private bool isMouseDown;
        private Color mouseDownColor;
        private Color borderColor;
        private bool isFocus;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new FlatButton.
        /// </summary>
        public FlatButton() {
            //Drawing optimization.
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw,true);

            //-----
            isMouseDown = false;
            mouseDownColor = BackColor;
            borderColor = Color.Transparent;
            isFocus = false;
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            using(SolidBrush BackBrush = new SolidBrush(IsMouseDown ? EntryColor : BackColor)) {
                e.Graphics.FillRectangle(BackBrush,ClientRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            //Text.
            if(!string.IsNullOrEmpty(Text)) {
                using(StringFormat SFotmat = new StringFormat() {
                    FormatFlags = StringFormatFlags.NoWrap,
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                using(SolidBrush ForeBrush = new SolidBrush(ForeColor)) {
                    e.Graphics.DrawString(Text,Font,ForeBrush,ClientRectangle,SFotmat);
                }
            }

            //Border.
            if(BorderColor != Color.Transparent && IsFocus) {
                using(Pen BorderPen = new Pen(BorderColor,4)) {
                    e.Graphics.DrawRectangle(BorderPen,0,0,Width,Height);
                }
            }

            base.OnPaint(e);
        }

        #endregion

        #region Mouse

        protected override void OnMouseDown(MouseEventArgs e) {
            IsMouseDown = true;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            IsMouseDown = false;

            base.OnMouseUp(e);
        }

        #endregion

        #region Keyboard

        protected override void OnKeyDown(KeyEventArgs e) {
            if(e.KeyCode == Keys.Enter) {
                IsMouseDown = true;
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            if(e.KeyCode == Keys.Enter) {
                IsMouseDown = false;
                OnClick(new EventArgs());
            }

            base.OnKeyUp(e);
        }

        #endregion

        #region Others

        protected override void OnGotFocus(EventArgs e) {
            IsFocus = true;

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e) {
            IsFocus = false;

            base.OnLostFocus(e);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Mouse any key is pressed.
        /// </summary>
        public bool IsMouseDown {
            get =>
                isMouseDown;
            set {
                if(value == isMouseDown)
                    return;

                isMouseDown = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Mouse down color. Use if mouse any key is downed.
        /// </summary>
        public Color EntryColor {
            get =>
                mouseDownColor;
            set {
                if(value == mouseDownColor)
                    return;

                mouseDownColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Border color. Set transparent for disable.
        /// </summary>
        public Color BorderColor {
            get =>
                borderColor;
            set {
                if(value == borderColor)
                    return;

                borderColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Focused this.
        /// </summary>
        public bool IsFocus {
            get =>
                isFocus;
            set {
                if(value == isFocus)
                    return;

                isFocus = value;
                Invalidate();
            }
        }

        #endregion
    }
}