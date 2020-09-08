using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Toggle for MochaDB Studio.
    /// </summary>
    public class toggle:Control {
        #region Fields

        private bool
            check = false,
            textingDrawing = true;

        private int
            index = 46;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public toggle() {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,true);
        }

        #endregion

        #region Overrides

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            Checked = !Checked;
        }

        #endregion

        #region Drawing

        [MTAThread]
        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            //BackColor.
            using(SolidBrush backBrush = new SolidBrush(BackColor))
                e.Graphics.FillRectangle(backBrush,DisplayRectangle);

            //Checked Area.
            using(SolidBrush backBrush = new SolidBrush(Color.DodgerBlue)) {
                if(!Checked) backBrush.Color = Color.Gray;

                if(TextingDraw) {
                    e.Graphics.FillPie(backBrush,Width - 50,1,20,Height-2,0,360);
                    e.Graphics.FillPie(backBrush,Width - 22,1,20,Height-2,0,360);
                    e.Graphics.FillRectangle(backBrush,Width - 38,1,27,Height-2);
                } else {
                    e.Graphics.FillPie(backBrush,0,0,20,Height - 1,0,360);
                    e.Graphics.FillPie(backBrush,Width - 20,0,20,Height - 1,0,360);
                    e.Graphics.FillRectangle(backBrush,10,0,Width - 20,Height - 1);
                }
            }

            //CheckState.
            using(SolidBrush backBrush = new SolidBrush(Color.Gainsboro)) {
                if(TextingDraw) {
                    e.Graphics.FillPie(backBrush,Width - index,4,12,12,0,360);
                } else {
                    e.Graphics.FillPie(backBrush,Width - index,4,12,12,0,360);
                }
            }

            if(TextingDraw)
                using(StringFormat format = new StringFormat() { LineAlignment = StringAlignment.Center })
                using(SolidBrush foreBrush = new SolidBrush(ForeColor))
                    e.Graphics.DrawString(Text,Font,foreBrush,DisplayRectangle,format);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Check state.
        /// </summary>
        public bool Checked {
            get => check;
            set {
                check = value;
                if(TextingDraw) {
                    /*var task = new Task(() => {
                        if(value) {
                            for(; index >= 18; index--) {
                                Invalidate();
                                Thread.Sleep(1);
                            }
                        } else {
                            for(; index <= 46; index++) {
                                Invalidate();
                                Thread.Sleep(1);
                            }
                        }
                    });
                    task.Start();*/
                    index = value ? 18 : 46;
                } else {
                    /*var task = new Task(() => {
                        if(value) {
                            for(; index <= 18; index++) {
                                Invalidate();
                                Thread.Sleep(1);
                            }
                        } else {
                            for(; index >= 46; index--) {
                                Invalidate();
                                Thread.Sleep(1);
                            }
                        }
                    });
                    task.Start();*/
                    index = value ? 18 : 46;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Draw with text.
        /// </summary>
        public bool TextingDraw {
            get => textingDrawing;
            set {
                textingDrawing = value;
                Invalidate();
            }
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor","System.Drawing.Design.UITypeEditor")]
        public override string Text { get => base.Text; set => base.Text = value; }

        #endregion
    }
}
