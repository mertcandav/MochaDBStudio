using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
    /// <summary>
    /// ToolTip for MochaDB Studio.
    /// </summary>
    public class stooltip:ToolTip {
        #region Fields

        private Font _Font { get; set; } = new Font("Arial",12,FontStyle.Regular,GraphicsUnit.Pixel);

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public stooltip() {
            OwnerDraw = true;
            Draw += FlatToolTip_Draw;
            Popup += FlatToolTip_Popup;
            BackColor = Color.FromArgb(30,30,30);
            ForeColor = Color.White;
        }

        #endregion

        #region Drawing

        private void FlatToolTip_Popup(object sender,PopupEventArgs e) {
            Size TipSize = TextRenderer.MeasureText(GetToolTip(e.AssociatedControl),Font);
            Size TitleSize = TextRenderer.MeasureText(ToolTipTitle,TitleFont);
            if(TitleSize.Width < TipSize.Width)
                e.ToolTipSize = new Size(TipSize.Width + 7,TipSize.Height + TitleSize.Height + 7);
            else
                e.ToolTipSize = new Size(TitleSize.Width + 9,TipSize.Height + TitleSize.Height + 7);
        }

        private void FlatToolTip_Draw(object sender,DrawToolTipEventArgs e) {
            //Graphics Quality.
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
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
                        Rectangle DrawRect = new Rectangle(e.Bounds.X,e.Bounds.Y + 3,e.Bounds.Width,e.Bounds.Height - 6);

                        //Background.
                        e.Graphics.FillRectangle(BackBrush,-1,-1,e.Bounds.Width + 1,e.Bounds.Height + 1);

                        //Title.
                        e.Graphics.DrawString(ToolTipTitle,TitleFont,ForeBrush,DrawRect,SFormat);

                        SFormat.LineAlignment = StringAlignment.Far;

                        //Tip Text.
                        e.Graphics.DrawString(e.ToolTipText,Font,ForeBrush,DrawRect,SFormat);
                    }
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Font(Pixel).
        /// </summary>
        public Font Font {
            get => _Font;
            set {
                if(value == _Font)
                    return;

                _Font = new Font(value.FontFamily,value.Size,FontStyle.Regular,GraphicsUnit.Pixel);
                TitleFont = new Font(_Font,FontStyle.Bold);
            }
        }

        /// <summary>
        /// TitleFont(Pixel).
        /// </summary>
        public Font TitleFont { get; private set; } = new Font("Arial",12,FontStyle.Bold,GraphicsUnit.Pixel);

        #endregion
    }
}
