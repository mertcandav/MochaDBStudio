using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MochaDBStudio.Properties;

namespace MochaDBStudio.GUI.Controls {
    /// <summary>
    /// Slide menu for menu window.
    /// </summary>
    public sealed partial class SlideMenu:ScrollableControl {
        #region Constructors

        /// <summary>
        /// Create new SlideMenu.
        /// </summary>
        public SlideMenu() {
            //Initialize.
            Init();

            IsOpen = true;
        }

        #endregion

        #region togglePicture

        private void TogglePicture_Click(object sender,EventArgs e) {
            ToggleAsync();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Open menu now with slide animation.
        /// </summary>
        public void Open() {
            IsOpen = true;
            Animator.ShowHide(this,AnimationEffect.Roll,100,360);
            Visible = true;
            Enabled = true;
        }

        /// <summary>
        /// Close menu now with slide animation.
        /// </summary>
        public void Close() {
            IsOpen = false;
            Animator.ShowHide(this,AnimationEffect.Roll,100,360);
            Visible = false;
            Enabled = false;
        }

        /// <summary>
        /// Toggle state.
        /// </summary>
        public void Toggle() {
            if(IsOpen)
                Close();
            else
                Open();
        }

        /// <summary>
        /// Open menu now with slide animation and asynchrone task.
        /// </summary>
        public void OpenAsync() {
            Thread openThread = new Thread(Open);
            openThread.Start();
        }

        /// <summary>
        /// Close menu now with slide animation and asynchrone task.
        /// </summary>
        public void CloseAsync() {
            Thread closeThread = new Thread(Close);
            closeThread.Start();
        }

        /// <summary>
        /// Toggle state with asynchrone task.
        /// </summary>
        public void ToggleAsync() {
            if(IsOpen)
                CloseAsync();
            else
                OpenAsync();
        }

        #endregion

        #region Properties

        /// <summary>
        /// MenuItemListView.
        /// </summary>
        public MenuItemListView ItemListener =>
            itemListener;

        /// <summary>
        /// Open state.
        /// </summary>
        public bool IsOpen { get; private set; }

        #endregion
    }

    //Designer for SlideMenu.
    partial class SlideMenu {
        #region Components

        private MenuItemListView itemListener;
        private PictureBox togglePicture;

        #endregion

        /// <summary>
        /// Initialize components and settings.
        /// </summary>
        public void Init() {
            //Drawing optimization.
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer,true);

            #region Base

            BackColor = Color.FromArgb(50,50,50);
            ForeColor = Color.White;
            Location = new Point(0,0);
            Size = new Size(150,150);
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top;
            Width = 150;

            #endregion

            #region togglePicture

            togglePicture = new PictureBox();
            togglePicture.SizeMode = PictureBoxSizeMode.Zoom;
            togglePicture.Size = new Size(20,20);
            togglePicture.Location = new Point(2,2);
            togglePicture.Image = Resources.Menu;
            togglePicture.Click +=TogglePicture_Click;
            Controls.Add(togglePicture);

            #endregion

            #region itemListener

            itemListener = new MenuItemListView();
            itemListener.Location = new Point(0,30);
            itemListener.ListStyle = ListViewListStyle.List;
            itemListener.Size = new Size(ClientSize.Width,ClientSize.Height - itemListener.Location.Y);
            itemListener.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            Controls.Add(itemListener);

            #endregion
        }
    }
}