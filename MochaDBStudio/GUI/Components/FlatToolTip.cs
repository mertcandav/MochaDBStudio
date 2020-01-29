using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace MochaDBStudio.GUI.Components {
    /// <summary>
    /// Flat ToolTip for tips.
    /// </summary>
    public class FlatToolTip:ToolTip {
        #region Fields

        private Font font;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new FlatToolTip.
        /// </summary>
        public FlatToolTip() {
            OwnerDraw = true;
            Draw += FlatToolTip_Draw;
            Popup += FlatToolTip_Popup;
            BackColor = Color.FromArgb(100,100,100);
            ForeColor = Color.White;
            font = new Font("Arial",12,FontStyle.Regular,GraphicsUnit.Pixel);
            TitleFont = new Font("Arial",12,FontStyle.Bold,GraphicsUnit.Pixel);
            ReshowDelay = 0;
        }

        #endregion

        #region Drawing

        private void FlatToolTip_Popup(object sender,PopupEventArgs e) {
            Size tipSize = TextRenderer.MeasureText(GetToolTip(e.AssociatedControl),Font);
            Size titleSize = TextRenderer.MeasureText(ToolTipTitle,TitleFont);
            if(titleSize.Width < tipSize.Width)
                e.ToolTipSize = new Size(tipSize.Width + 7,tipSize.Height + titleSize.Height + 7);
            else
                e.ToolTipSize = new Size(titleSize.Width + 9,tipSize.Height + titleSize.Height + 7);
        }

        private void FlatToolTip_Draw(object sender,DrawToolTipEventArgs e) {
            //Graphics Quality.
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using(SolidBrush BackBrush = new SolidBrush(BackColor))
            using(SolidBrush ForeBrush = new SolidBrush(ForeColor)) {

                if(string.IsNullOrEmpty(ToolTipTitle))
                    using(StringFormat SFormat = new StringFormat() {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    }) {
                        //Background.
                        e.Graphics.FillRectangle(BackBrush,-1,-1,e.Bounds.Width + 1,e.Bounds.Height + 1);

                        //Tip Text.
                        e.Graphics.DrawString(e.ToolTipText,Font,ForeBrush,e.Bounds,SFormat);
                    }
                else {
                    using(StringFormat SFormat = new StringFormat(StringFormatFlags.NoWrap) {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Near
                    }) {
                        Rectangle drawRect = new Rectangle(e.Bounds.X,e.Bounds.Y + 3,e.Bounds.Width,e.Bounds.Height - 6);

                        //Background.
                        e.Graphics.FillRectangle(BackBrush,-1,-1,e.Bounds.Width + 1,e.Bounds.Height + 1);

                        //Title.
                        e.Graphics.DrawString(ToolTipTitle,TitleFont,ForeBrush,drawRect,SFormat);

                        SFormat.LineAlignment = StringAlignment.Far;

                        //Tip Text.
                        e.Graphics.DrawString(e.ToolTipText,Font,ForeBrush,drawRect,SFormat);
                    }
                }
            }
        }

        #endregion

        #region Get/Set

        /// <summary>
        /// Font(Pixel).
        /// </summary>
        public Font Font {
            get => font;
            set {
                if(value == font)
                    return;

                font = new Font(value.FontFamily,value.Size,FontStyle.Regular,GraphicsUnit.Pixel);
                TitleFont = new Font(font,FontStyle.Bold);
            }
        }

        /// <summary>
        /// TitleFont(Pixel).
        /// </summary>
        public Font TitleFont { get; private set; }

        #endregion
    }
}