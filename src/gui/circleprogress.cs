using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Circle progress bar for MochaDB Studio.
    /// </summary>
    public class circleprogress:Control {
        #region Fields

        private int
            drawValue = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public circleprogress() {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer,true);
        }

        #endregion

        #region Drawing

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            try {
                //FullValueCircle.
                using(SolidBrush backBrush = new SolidBrush(Color.Gray))
                    e.Graphics.FillPie(backBrush,5,5,Width - 10,Height - 10,0,360);

                //Value.
                using(SolidBrush backBrush = new SolidBrush(ValueColor))
                    e.Graphics.FillPie(backBrush,0,1,Width - 1,Height - 2,270,360 * drawValue / MaxmimumValue);

                //Center.
                using(SolidBrush backBrush = new SolidBrush(BackColor))
                    e.Graphics.FillPie(backBrush,10,10,Width - 20,Height - 20,0,360);

                //ValueText.
                using(SolidBrush foreBrush = new SolidBrush(ForeColor))
                using(StringFormat stringFormat = new StringFormat() {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                    e.Graphics.DrawString("" + Value,Font,foreBrush,ClientRectangle,stringFormat);
            } catch { }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Maxmimum value.
        /// </summary>
        public int MaxmimumValue { get; set; }

        /// <summary>
        /// Color of value.
        /// </summary>
        public Color ValueColor { get; set; }

        private int value = 0;
        /// <summary>
        /// Value.
        /// </summary>
        public int Value {
            get => value;
            set {
                this.value = value;
                Invalidate();
            }
        }

        #endregion
    }
}
