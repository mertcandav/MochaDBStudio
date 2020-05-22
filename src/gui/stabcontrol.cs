using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Tab control for MochaDB Studio.
    /// </summary>
    public sealed partial class stabcontrol:TabControl {
        #region Fields

        private volatile int
            lineX = 10;

        private Task
            oldTask;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public stabcontrol() {
            Init();
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            using(var bgbrush = new SolidBrush(BackColor))
                e.Graphics.FillRectangle(bgbrush,ClientRectangle);
        }

        protected override void OnPaint(PaintEventArgs e) {
            using(var centerFormat = new StringFormat() {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            })
            using(var forebrush = new SolidBrush(ForeColor))
                for(short index = 0; index < TabCount; index++) {
                    var tab = TabPages[index];
                    var rect = GetRect(index);
                    e.Graphics.DrawString(tab.Text,Font,forebrush,rect,centerFormat);
                }

            if(SelectedIndex != -1)
                using(var pen = new Pen(Color.DeepSkyBlue,2)) {
                    var rect = GetRect(SelectedIndex);
                    e.Graphics.DrawLine(pen,lineX,rect.Y+rect.Height,lineX+(rect.Width),rect.Y+rect.Height);
                }
        }

        #endregion

        #region Item override

        protected override void OnSelectedIndexChanged(EventArgs e) {
            base.OnSelectedIndexChanged(e);
            Refresh();
            if(SelectedIndex != -1) {
                if(oldTask != null) {
                    oldTask.Wait();
                    oldTask.Dispose();
                }

                oldTask = new Task(() => {
                    var rect = GetRect(SelectedIndex);
                    if(rect.X == lineX)
                        return;

                    if(rect.X <= lineX) {
                        for(; lineX >= rect.X; lineX-=4) {
                            Invalidate();
                            Thread.Sleep(1);
                        }
                    } else {
                        for(; lineX <= rect.X; lineX+=4) {
                            Invalidate();
                            Thread.Sleep(1);
                        }
                    }
                    lineX = rect.X;
                    Invalidate();
                });
                oldTask.Start();
            }
        }

        #endregion

        #region Mouse override

        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);

            var page = GetHoveringPage();
            if(page != null) {
                SelectedTab = page;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns tab rectangle by dex.
        /// </summary>
        /// <param name="index">Index of page.</param>
        public Rectangle GetRect(int index) {
            var rect = GetTabRect(index);
            rect.X += 10;
            return rect;
        }

        /// <summary>
        /// Return hovering page.
        /// </summary>
        public TabPage GetHoveringPage() {
            for(int index = 0; index < TabCount; index++)
                if(GetRect(index).Contains(PointToClient(Cursor.Position)))
                    return TabPages[index];
            return null;
        }

        /// <summary>
        /// Return rectangle of hovering page.
        /// </summary>
        public Rectangle GetHoveringRect() {
            for(int index = 0; index < TabCount; index++) {
                Rectangle rect = GetRect(index);
                if(rect.Contains(PointToClient(Cursor.Position)))
                    return rect;
            }
            return new Rectangle();
        }

        /// <summary>
        /// Add new tab page.
        /// </summary>
        /// <param name="text">Text of page.</param>
        public void AddPage(string text) {
            var page = new TabPage();
            page.BackColor = BackColor;
            TabPages.Add(page);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Background color.
        /// </summary>
        public new Color BackColor { get; set; }

        /// <summary>
        /// Foreground color.
        /// </summary>
        public new Color ForeColor { get; set; }

        #endregion
    }

    // Designer.
    public sealed partial class stabcontrol {
        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            DrawMode = TabDrawMode.OwnerDrawFixed;
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint | ControlStyles.UserMouse,true);
            BackColor = Color.FromArgb(60,60,60);
            ForeColor = Color.White;
            Font = new Font("Arial",9);

            #endregion
        }
    }
}
