//
// MIT License
//
// Copyright (c) 2020 Mertcan Davulcu
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
// OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MochaDB;
using MochaDBStudio.dialogs;
using MochaDBStudio.engine;
using MochaDBStudio.gui;
using MochaDBStudio.Properties;

namespace MochaDBStudio {
    /// <summary>
    /// Main window of application.
    /// </summary>
    public sealed partial class Studio:sform {
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

            for(int dex = 0; dex < Program.Arguments.Length; dex++) {
                var arg = Program.Arguments[dex];
                var finfo = fs.GetFileInfo(arg);

                if(!finfo.Exists)
                    continue;

                if(finfo.Extension == ".mhdb") {
                    ConnectDB_Dialog dialog = new ConnectDB_Dialog();
                    dialog.CNCList = connectionMenu;
                    dialog.pathTB.Text = arg;
                    dialog.ShowDialog();
                    dialog.Dispose();
                } else if(finfo.Extension == ".mhsc") {
                    OpenScript_Dialog dialog = new OpenScript_Dialog();
                    dialog.CNCList = connectionMenu;
                    dialog.pathTB.Text = arg;
                    dialog.OpenButton_Click(null,null);
                    dialog.Dispose();
                }
            }

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

        #region titlePanel

        private void TitlePanel_MouseDoubleClick(object sender,MouseEventArgs e) {
            if(e.Button == MouseButtons.Left) {
                FsButton_Click(null,null);
            }
        }

        #endregion

        #region fsButton

        private void FsButton_Click(object sender,System.EventArgs e) {
            WindowState =
                WindowState == FormWindowState.Normal ?
                    FormWindowState.Maximized :
                    FormWindowState.Normal;
           fsButton.Text =
                WindowState == FormWindowState.Normal ?
                    "⬜" :
                    "❐";
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

        #region connectionCM

        private void ConnectionCM_ItemClicked(object sender,ToolStripItemClickedEventArgs e) {
            connectionCM.Close();

            if(e.ClickedItem.Text == "Disconnect") {
                connectionMenu.close();
            }
        }

        #endregion

        #region generalCM

        private void GeneralCM_ItemClicked(object sender,ToolStripItemClickedEventArgs e) {
            generalCM.Close();
            if(e.ClickedItem.Text == "Connect MochaDB") {
                var dialog = new ConnectDB_Dialog();
                dialog.CNCList = connectionMenu;
                dialog.ShowDialog();
                dialog.Dispose();
            } else if(e.ClickedItem.Text == "Open MochaScript") {
                var dialog = new OpenScript_Dialog();
                dialog.CNCList = connectionMenu;
                dialog.ShowDialog();
                dialog.Dispose();
            } else if(e.ClickedItem.Text == "Create MochaDB") {
                var dialog = new CreateDB_Dialog();
                dialog.CNCList = connectionMenu;
                dialog.ShowDialog();
                dialog.Dispose();
            } else if(e.ClickedItem.Text == "Create MochaScript") {
                var dialog = new CreateScript_Dialog();
                dialog.CNCList = connectionMenu;
                dialog.ShowDialog();
                dialog.Dispose();
            }
        }

        #endregion

        #region scriptCM

        private void ScriptCM_ItemClicked(object sender,ToolStripItemClickedEventArgs e) {
            scriptCM.Close();

            if(e.ClickedItem.Text == "Debug & Run") {
                ((scriptpanel)(connectionMenu.CurrentItem.Tag)).DebugAsync();
            } else if(e.ClickedItem.Text == "Save") {
                ((scriptpanel)(connectionMenu.CurrentItem.Tag)).Save();
            } else if(e.ClickedItem.Text == "Exit") {
                connectionMenu.close();
            }
        }

        #endregion

        #region helpCM

        private void HelpCM_ItemClicked(object sender,ToolStripItemClickedEventArgs e) {
            helpCM.Close();

            if(e.ClickedItem.Text == "MochaDB Documentation") {
                Process.Start("https://github.com/mertcandav/MochaDB/wiki");
            } else if(e.ClickedItem.Text == "About MochaDB Studio") {
                var dialog = new About_Dialog();
                dialog.ShowDialog();
            }
        }

        #endregion

        #region connectionMenu

        private void ConnectionMenu_CurrentItemChanged(object sender,EventArgs e) {
            if(sender != null) {
                if(connectionMenu.CurrentItem.Tag2 == "Database") {
                    connectionButton.Location = new Point(
                        generalButton.Location.X+generalButton.Width,0);
                    helpButton.Location = new Point(
                        connectionButton.Location.X+connectionButton.Width,0);
                    connectionButton.BringToFront();
                } else {
                    scriptButton.Location = new Point(
                        generalButton.Location.X+generalButton.Width,0);
                    helpButton.Location = new Point(
                        scriptButton.Location.X+scriptButton.Width,0);
                    scriptButton.BringToFront();
                }
            } else {
                generalButton.BringToFront();
                connectionButton.Location = generalButton.Location;
                scriptButton.Location = generalButton.Location;
                helpButton.Location = new Point(
                    generalButton.Location.X+generalButton.Width,0);
            }
        }

        #endregion

        #region Location override

        protected override void OnLocationChanged(EventArgs e) {
            if(fsButton.Text != "⬜")
                fsButton.Text = "⬜";

            base.OnLocationChanged(e);
        }

        #endregion
    }

    // Designer.
    public partial class Studio {
        #region Components

        public spanel
            titlePanel,
            gridPanel;

        public sbutton
            closeButton,
            fsButton,
            minimizeButton,
            generalButton,
            scriptButton,
            connectionButton,
            helpButton,
            iconButton;

        private slidemenu
            connectionMenu;

        public sContextMenu
            generalCM,
            scriptCM,
            connectionCM,
            helpCM;

        #endregion

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Init() {
            #region Base

            Text = "MochaDB Studio";
            Size = new Size(810,470);
            MinimumSize = Size;

            #endregion

            #region titlePanel

            titlePanel = new spanel();
            titlePanel.Dock = DockStyle.Top;
            titlePanel.Height = 30;
            titlePanel.BackColor = Color.FromArgb(24,24,24);
            titlePanel.MouseDoubleClick+=TitlePanel_MouseDoubleClick;
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
            fsButton.Text = "⬜";
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

            #region generalCM

            generalCM = new sContextMenu();
            generalCM.ForeColor = Color.White;
            generalCM.BackColor = titlePanel.BackColor;
            generalCM.Items.Add(new sContextMenuItem("Connect MochaDB",
                generalCM.BackColor,Color.Gray) {
                Image = Resources.Connect
            });
            generalCM.Items.Add(new sContextMenuItem("Open MochaScript",
                generalCM.BackColor,Color.Gray) {
                Image = Resources.Script
            });
            generalCM.Items.Add(new sContextMenuItem("Create MochaDB",
                generalCM.BackColor,Color.Gray) {
                Image = Resources.Create
            });
            generalCM.Items.Add(new sContextMenuItem("Create MochaScript",
                generalCM.BackColor,Color.Gray) {
                Image = Resources.Create
            });
            generalCM.ItemClicked+=GeneralCM_ItemClicked;

            #endregion

            #region generalButton

            generalButton = new sbutton();
            generalButton.Font = new Font("Microsoft Sans Serif",9);
            generalButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top;
            generalButton.Text = "General";
            generalButton.ForeColor = Color.White;
            generalButton.BackColor = titlePanel.BackColor;
            generalButton.MouseEnterColor = Color.Gray;
            generalButton.MouseDownColor = Color.DodgerBlue;
            generalButton.Size = new Size(70,titlePanel.Height);
            generalButton.Location = new Point(iconButton.Width + 5,0);
            generalButton.ContextMenu = generalCM;
            generalButton.DisableClick = true;
            titlePanel.Controls.Add(generalButton);

            #endregion

            #region connectionCM

            connectionCM = new sContextMenu();
            connectionCM.ForeColor = Color.White;
            connectionCM.BackColor = titlePanel.BackColor;
            connectionCM.Items.Add(new sContextMenuItem("Disconnect",
                connectionCM.BackColor,Color.Gray) {
                Image = Resources.Disconnect
            });
            connectionCM.ItemClicked+=ConnectionCM_ItemClicked;

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
            connectionButton.ContextMenu = connectionCM;
            connectionButton.DisableClick = true;
            titlePanel.Controls.Add(connectionButton);

            #endregion

            #region scriptCM

            scriptCM = new sContextMenu();
            scriptCM.ForeColor = Color.White;
            scriptCM.BackColor = titlePanel.BackColor;
            scriptCM.Items.Add(new sContextMenuItem("Debug & Run",
                connectionCM.BackColor,Color.Gray) {
                Image = Resources.Play,
                ShortcutKeyDisplayString = "F5"
            });
            scriptCM.Items.Add(new sContextMenuItem("Save",
                connectionCM.BackColor,Color.Gray) {
                Image = Resources.Save,
                ShortcutKeyDisplayString = "Ctrl+S"
            });
            scriptCM.Items.Add(new sContextMenuItem("Exit",
                scriptCM.BackColor,Color.Gray) {
                Image = Resources.Disconnect
            });
            scriptCM.ItemClicked+=ScriptCM_ItemClicked;

            #endregion

            #region scriptButton

            scriptButton = new sbutton();
            scriptButton.Font = new Font("Microsoft Sans Serif",9);
            scriptButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top;
            scriptButton.Text = "Script";
            scriptButton.ForeColor = Color.White;
            scriptButton.BackColor = titlePanel.BackColor;
            scriptButton.MouseEnterColor = Color.Gray;
            scriptButton.MouseDownColor = Color.DodgerBlue;
            scriptButton.Size = new Size(70,titlePanel.Height);
            scriptButton.Location = new Point(iconButton.Width + 5,0);
            scriptButton.ContextMenu = scriptCM;
            scriptButton.DisableClick = true;
            titlePanel.Controls.Add(scriptButton);

            #endregion

            #region helpCM

            helpCM = new sContextMenu();
            helpCM.ForeColor = Color.White;
            helpCM.BackColor = titlePanel.BackColor;
            helpCM.Items.Add(new sContextMenuItem("MochaDB Documentation",
                helpCM.BackColor,Color.Gray) {
                Image = Resources.Documentation
            });
            helpCM.Items.Add(new sContextMenuItem("About MochaDB Studio",
                helpCM.BackColor,Color.Gray) {
                Image = Resources.Information
            });
            helpCM.ItemClicked+=HelpCM_ItemClicked;

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
            helpButton.ContextMenu = helpCM;
            helpButton.DisableClick = true;
            titlePanel.Controls.Add(helpButton);

            #endregion

            #region gridPanel

            gridPanel = new spanel();
            gridPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            gridPanel.Location = new Point(0,titlePanel.Height);
            gridPanel.Size = new Size(Width,Height-gridPanel.Location.Y);
            gridPanel.BackColor = Color.FromArgb(60,60,60);
            gridPanel.BackgroundImage = Resources.MochaDB_LogoGray;
            Controls.Add(gridPanel);

            #endregion

            #region connectionMenu

            connectionMenu = new slidemenu();
            connectionMenu.Location = Point.Empty;
            connectionMenu.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top;
            connectionMenu.Height = ClientSize.Height;
            connectionMenu.Width = 200;
            connectionMenu.Grid = gridPanel;
            connectionMenu.CurrentItemChanged+=ConnectionMenu_CurrentItemChanged;
            Controls.Add(connectionMenu);
            connectionMenu.BringToFront();
            connectionMenu.Close();

            #endregion

            generalButton.BringToFront();
        }
    }
}
