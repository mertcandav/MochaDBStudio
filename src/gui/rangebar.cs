using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
    /// <summary>
    /// rangebar for MochaDB Studio.
    /// </summary>
    public sealed partial class rangebar:Control {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public rangebar() {
            Init();
        }

        #endregion

        #region Drawing

        protected override void OnPaint(PaintEventArgs e) {
            using(var format = new StringFormat() {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near
            })
                e.Graphics.DrawString(Text,Font,
                                        state > State ?
                                            Brushes.Gray :
                                            state <= 2 ?
                                                Brushes.LimeGreen :
                                                state == 3 ?
                                                    Brushes.Orange :
                                                    state == 4 ?
                                                    Brushes.OrangeRed :
                                                    Brushes.Red,
                                        ClientRectangle,format);

            for(short dex = 0; dex < 5; dex++) {
                var rect = new Rectangle((dex * 20) + 5,20,15,10);
                var state = dex + 1;
                var brush =
                    state > State ?
                        Brushes.Gray :
                        state <= 2 ?
                        Brushes.LimeGreen :
                            state == 3 ?
                            Brushes.Orange :
                                state == 4 ?
                                Brushes.OrangeRed :
                                Brushes.Red;
                e.Graphics.FillRectangle(brush,rect);
            }
        }

        #endregion

        #region Override

        protected override void OnTextChanged(EventArgs e) {
            base.OnTextChanged(e);
            Invalidate();
        }

        #endregion

        #region Properties

        private int state = 0;
        /// <summary>
        /// State.
        /// </summary>
        public int State {
            get => state;
            set {
                if(value == state)
                    return;

                new Task(() => {
                    for(short dex = 1; dex <= value; dex++) {
                        state = dex;
                        Invalidate();
                        Thread.Sleep(500);
                    }
                }).Start();
            }
        }

        #endregion
    }

    // Designer.
    public sealed partial class rangebar {
        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,true);
            Size = new Size(115,30);

            #endregion
        }
    }
}
