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
using System.Drawing;
using System.Windows.Forms;
using MochaDBStudio.Dialogs;
using MochaDBStudio.GUI;
using MochaDBStudio.GUI.Controls;
using MochaDBStudio.Properties;

namespace MochaDBStudio {
    /// <summary>
    /// Main window of application.
    /// </summary>
    public sealed partial class Studio:Form {
        #region Fields

        private Font strongBiggerFont;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new Studio.
        /// </summary>
        public Studio() {
            //Initialize.
            Init();

            strongBiggerFont = new Font("Consolas",14,FontStyle.Bold,GraphicsUnit.Pixel);
        }

        #endregion

        #region Overrides

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            Focus();
        }

        #endregion

        #region slideMenu

        private void ItemListener_ItemClicked(object sender,ItemListViewItemEventArgs e) {
            slideMenu.CloseAsync();

            if(e.Item == databaseItem) {
                DatabaseDialog.ShowDialog(tab);
            } else if(e.Item ==scriptItem) {
                ScriptDialog.ShowDialog(tab);
            } else if(e.Item == terminalItem) {
                Terminal terminal = new Terminal();
                tab.Add(terminal);
            }
        }

        #endregion

        #region togglePicture

        private void TogglePicture_Click(object sender,EventArgs e) {
            slideMenu.ToggleAsync();
        }

        #endregion

        #region Overrides

        protected override void OnShown(EventArgs e) {
            Animator.FormFadeShow(this,18);

            base.OnShown(e);
        }

        protected override void OnClosing(CancelEventArgs e) {
            Animator.FormFadeHide(this,18);

            base.OnClosing(e);
        }

        #endregion
    }

    //Designer of Studio.
    partial class Studio {
        #region Components

        private Panel topPanel;

        private SlideMenu slideMenu;
        private MenuListItem databaseItem;
        private MenuListItem scriptItem;
        private MenuListItem terminalItem;

        private PictureBox togglePicture;

        private PageView tab;

        #endregion

        /// <summary>
        /// Initialize components and settings.
        /// </summary>
        public void Init() {
            #region Base

            Text = "MochaDB Studio";
            Opacity = 0;
            BackColor = Color.White;
            ForeColor = Color.Black;
            Size = new Size(700,500);
            MinimumSize = Size;
            StartPosition = FormStartPosition.CenterScreen;
            Icon = Resources.Icon;

            #endregion

            #region slideMenu

            slideMenu = new SlideMenu();
            slideMenu.Location = Point.Empty;
            slideMenu.Height = ClientSize.Height;
            slideMenu.Width = 170;
            slideMenu.ItemListener.ItemClicked+=ItemListener_ItemClicked;

            #region databaseItem

            databaseItem = new MenuListItem();
            databaseItem.Title = "Database";
            databaseItem.Image = Resources.Database;
            databaseItem.BackColor = slideMenu.BackColor;

            slideMenu.ItemListener.AddItem(databaseItem,false);

            #endregion

            #region scriptItem

            scriptItem = new MenuListItem();
            scriptItem.Title = "MochaScript";
            scriptItem.Image = Resources.ScriptDocument;
            scriptItem.BackColor = slideMenu.BackColor;

            slideMenu.ItemListener.AddItem(scriptItem,false);

            #endregion

            #region terminalItem

            terminalItem = new MenuListItem();
            terminalItem.Title = "Terminal";
            terminalItem.Image = Resources.Terminal;
            terminalItem.BackColor = slideMenu.BackColor;

            slideMenu.ItemListener.AddItem(terminalItem,false);

            #endregion

            slideMenu.ItemListener.AdapdateControls();
            slideMenu.Close();
            Controls.Add(slideMenu);

            #endregion

            #region topPanel

            topPanel = new Panel();
            topPanel.BackColor = Color.FromArgb(17,17,17);
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 25;

            Controls.Add(topPanel);

            #endregion

            #region togglePicture

            togglePicture = new PictureBox();
            togglePicture.SizeMode = PictureBoxSizeMode.Zoom;
            togglePicture.Size = new Size(20,20);
            togglePicture.Location = new Point(2,2);
            togglePicture.Image = Resources.Menu;
            togglePicture.Click +=TogglePicture_Click;
            topPanel.Controls.Add(togglePicture);

            #endregion

            #region tab

            tab = new PageView();
            tab.BackColor = Color.White;
            tab.Location = new Point(0,topPanel.Height);
            tab.Size = new Size(ClientSize.Width,ClientSize.Height - tab.Location.Y);
            tab.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            Controls.Add(tab);

            #endregion
        }
    }
}