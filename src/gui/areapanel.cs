using System.Drawing;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Area panel for MochaDB Studio.
    /// </summary>
    public sealed partial class areapanel:Panel {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title">Titile.</param>
        public areapanel(string title) {
            Font = new Font("Arial",12);
            Title = title;
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);

            using(var centerFormat = new StringFormat() {
                LineAlignment = StringAlignment.Center
            })
            using(var forebrush = new SolidBrush(ForeColor))
                e.Graphics.DrawString(Title,Font,forebrush,
                    new Rectangle(0,0,Width,30),centerFormat);

            e.Graphics.DrawLine(Pens.LightSlateGray,0,30,Width,30);
        }

        #endregion

        #region Properties

        private string title = string.Empty;
        /// <summary>
        /// Title of area.
        /// </summary>
        public string Title {
            get => title;
            set {
                if(value == title)
                    return;

                title = value;
                Invalidate();
            }
        }

        #endregion
    }
}
