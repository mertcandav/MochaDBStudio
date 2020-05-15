using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
    /// <summary>
    /// ComboBox for MochaDB Studio.
    /// </summary>
    public class scombobox:ComboBox {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public scombobox() {
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawMode = DrawMode.OwnerDrawFixed;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer,true);
        }

        #endregion

        #region Drawing

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using(SolidBrush bgBrush = new SolidBrush(BackColor))
                e.Graphics.FillRectangle(bgBrush,0,0,Width - 1,Height - 1);

            //Border.
            using(Pen pen = new Pen(BorderColor,1))
                e.Graphics.DrawRectangle(pen,0,0,Width - 1,Height - 1);

            using(StringFormat centerFormat = new StringFormat() {
                FormatFlags = StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center
            }) {

                //SelectedItem.
                if(SelectedIndex != -1)
                    using(SolidBrush foreBrush = new SolidBrush(ForeColor))
                        e.Graphics.DrawString(Items[SelectedIndex].ToString(),
                            Font,foreBrush,ClientRectangle,centerFormat);

                //DropDownArrow.
                using(SolidBrush borderBrush = new SolidBrush(BorderColor)) {
                    centerFormat.Alignment = StringAlignment.Far;
                    e.Graphics.DrawString(DroppedDown ? "▲" : "▼",
                        Font,borderBrush,ClientRectangle,centerFormat);
                }
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e) {
            if(e.Index == -1)
                return;
            e.DrawBackground();
            e.DrawFocusRectangle();
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using(StringFormat centerFormat = new StringFormat() {
                FormatFlags = StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center
            })
            using(SolidBrush foreBrush = new SolidBrush(ForeColor))
            using(SolidBrush backBrush = new SolidBrush(
                BackColor)) {
                //ItemText.
                e.Graphics.DrawString(Items[e.Index].ToString(),
                    Font,foreBrush,e.Bounds,centerFormat);
            }
        }

        #endregion

        #region Properties

        private Color borderColor = Color.DodgerBlue;
        /// <summary>
        /// Color or border. Set transparent for disable.
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

        #endregion
    }
}
