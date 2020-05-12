using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MochaDBStudio.gui;
using MochaDBStudio.Properties;

namespace MochaDBStudio {
    /// <summary>
    /// Main window of application.
    /// </summary>
    public sealed partial class Studio:Form {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public Studio() {
            Init();
        }

        #endregion

        #region Form Overrides

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            Focus();
        }

        protected override void OnShown(EventArgs e) {
            Animator.FormFadeShow(this,25);

            base.OnShown(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            Animator.FormFadeHide(this,25);

            base.OnFormClosing(e);
        }

        #endregion

        #region closeButton

        private void CloseButton_Click(object sender,System.EventArgs e) {
            Application.Exit();
        }

        #endregion

        #region fsButton

        private void FsButton_Click(object sender,System.EventArgs e) {
            WindowState =
                WindowState == FormWindowState.Maximized ?
                    FormWindowState.Normal : FormWindowState.Maximized;
        }

        #endregion

        #region minimizeButton

        private void MinimizeButton_Click(object sender,System.EventArgs e) {
            WindowState = FormWindowState.Minimized;
        }

        #endregion

        #region iconButton

        private void iconButton_Click(object sender,EventArgs e) {
            connectionMenu.Toggle();
        }

        #endregion
    }

    // Designer.
    public partial class Studio {
        #region Components

        private spanel
            titlePanel,
            gridPanel;

        private sbutton
            closeButton,
            fsButton,
            minimizeButton,
            connectionButton,
            helpButton,
            iconButton;

        private slidemenu
            connectionMenu;

        #endregion

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Init() {
            #region Base

            Text = "MochaDB Studio";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(810,470);
            Icon = Resources.MochaDB_Logo;
            Opacity = 0;

            #endregion

            #region titlePanel

            titlePanel = new spanel();
            titlePanel.Dock = DockStyle.Top;
            titlePanel.Height = 30;
            titlePanel.BackColor = Color.FromArgb(24,24,24);
            titlePanel.Moveable = true;
            titlePanel.Tag = this;
            Controls.Add(titlePanel);

            #endregion

            #region iconButton

            iconButton = new sbutton();
            iconButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left;
            iconButton.Location = new Point(0,0);
            iconButton.Size = new Size(30,titlePanel.Height);
            iconButton.BackColor = titlePanel.BackColor;
            iconButton.MouseEnterColor = Color.Gray;
            iconButton.MouseDownColor = Color.DodgerBlue;
            iconButton.Image = Resources.MochaDB_Logo.ToBitmap();
            iconButton.Click +=iconButton_Click;
            titlePanel.Controls.Add(iconButton);

            #endregion

            #region closeButton

            closeButton = new sbutton();
            closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
            closeButton.Text = "X";
            closeButton.ForeColor = Color.White;
            closeButton.BackColor = titlePanel.BackColor;
            closeButton.MouseEnterColor = Color.Coral;
            closeButton.MouseDownColor = Color.Red;
            closeButton.Size = new Size(30,titlePanel.Height);
            closeButton.Location = new Point(titlePanel.Width - closeButton.Width,0);
            closeButton.Click += CloseButton_Click;
            closeButton.TabStop = false;
            titlePanel.Controls.Add(closeButton);

            #endregion

            #region fsButton

            fsButton = new sbutton();
            fsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
            fsButton.Text = "□";
            fsButton.Font = new Font(fsButton.Font.Name,13);
            fsButton.ForeColor = Color.White;
            fsButton.BackColor = titlePanel.BackColor;
            fsButton.MouseEnterColor = Color.Gray;
            fsButton.MouseDownColor = Color.DodgerBlue;
            fsButton.Size = new Size(30,titlePanel.Height);
            fsButton.Location = new Point(closeButton.Location.X - fsButton.Width,0);
            fsButton.TabStop = false;
            fsButton.Click +=FsButton_Click;
            titlePanel.Controls.Add(fsButton);

            #endregion

            #region minimizeButton

            minimizeButton = new sbutton();
            minimizeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
            minimizeButton.Text = "̶";
            minimizeButton.ForeColor = Color.White;
            minimizeButton.BackColor = titlePanel.BackColor;
            minimizeButton.MouseEnterColor = Color.Gray;
            minimizeButton.MouseDownColor = Color.DodgerBlue;
            minimizeButton.Size = new Size(30,titlePanel.Height);
            minimizeButton.Location = new Point(fsButton.Location.X - closeButton.Width,0);
            minimizeButton.TabStop = false;
            minimizeButton.Click +=MinimizeButton_Click;
            titlePanel.Controls.Add(minimizeButton);

            #endregion

            #region connectionButton

            connectionButton = new sbutton();
            connectionButton.Font = new Font("Microsoft Sans Serif",9);
            connectionButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top;
            connectionButton.Text = "Connection";
            connectionButton.ForeColor = Color.White;
            connectionButton.BackColor = titlePanel.BackColor;
            connectionButton.MouseEnterColor = Color.Gray;
            connectionButton.MouseDownColor = Color.DodgerBlue;
            connectionButton.Size = new Size(70,titlePanel.Height);
            connectionButton.Location = new Point(iconButton.Width + 5,0);
            titlePanel.Controls.Add(connectionButton);

            #endregion

            #region helpButton

            helpButton = new sbutton();
            helpButton.Font = new Font("Microsoft Sans Serif",9);
            helpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top;
            helpButton.Text = "Help";
            helpButton.ForeColor = Color.White;
            helpButton.BackColor = titlePanel.BackColor;
            helpButton.MouseEnterColor = Color.Gray;
            helpButton.MouseDownColor = Color.DodgerBlue;
            helpButton.Size = new Size(70,titlePanel.Height);
            helpButton.Location = new Point(connectionButton.Location.X + connectionButton.Width,0);
            titlePanel.Controls.Add(helpButton);

            #endregion

            #region gridPanel

            gridPanel = new spanel();
            gridPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            gridPanel.Location = new Point(0,titlePanel.Height);
            gridPanel.Size = new Size(Width,Height-gridPanel.Location.Y);
            gridPanel.BackColor = Color.FromArgb(60,60,60);
            Controls.Add(gridPanel);

            #endregion

            #region connectionMenu

            connectionMenu = new slidemenu();
            connectionMenu.Location = Point.Empty;
            connectionMenu.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top;
            connectionMenu.Height = ClientSize.Height;
            connectionMenu.Width = 200;
            Controls.Add(connectionMenu);
            connectionMenu.BringToFront();
            connectionMenu.Close();

            #endregion
        }
    }
}
