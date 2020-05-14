using System;
using System.Drawing;
using System.Windows.Forms;
using MochaDB;
using MochaDBStudio.gui;

namespace MochaDBStudio.dialogs {
    /// <summary>
    /// Table edit form for MochaDB Studio.
    /// </summary>
    public sealed partial class TableEdit_Dialog:sform {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="db">Database.</param>
        /// <param name="tableName">Name of table.</param>
        public TableEdit_Dialog(MochaDatabase db,string tableName) {
            Database = db;
            TableName = tableName;
            Init();
        }

        #endregion

        #region closeButton

        private void CloseButton_Click(object sender,EventArgs e) {
            Close();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Targetted database.
        /// </summary>
        public MochaDatabase Database { get; private set; }

        /// <summary>
        /// Name of targetted table.
        /// </summary>
        public string TableName { get; set; }

        #endregion
    }

    // Designer.
    public sealed partial class TableEdit_Dialog {
        #region Components

        private Label
            titleLabel;

        private stabcontrol
            tab;

        private TabPage
            viewPage,
            columnsPage,
            settingsPage;

        private spanel
            titlePanel;

        private sbutton
            closeButton;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            Text = "Editing table";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(50,50,50);
            Size = new Size(790,400);

            #endregion

            #region titlePanel

            titlePanel = new spanel();
            titlePanel.Dock = DockStyle.Top;
            titlePanel.Height = 30;
            titlePanel.BackColor = Color.FromArgb(24,24,24);
            titlePanel.Tag = this;
            Controls.Add(titlePanel);

            #endregion

            #region titleLabel

            titleLabel = new Label();
            titleLabel.Dock = DockStyle.Left;
            titleLabel.Width = 100;
            titleLabel.Text = Text;
            titleLabel.ForeColor = Color.Gray;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Font = new Font("Arial",10,FontStyle.Bold);
            titlePanel.Controls.Add(titleLabel);

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
            closeButton.Click +=CloseButton_Click;
            closeButton.TabStop = false;
            titlePanel.Controls.Add(closeButton);

            #endregion

            #region tab

            tab = new stabcontrol();
            tab.BackColor = BackColor;
            tab.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            tab.Location = new Point(0,titlePanel.Height);
            tab.Size = new Size(Width,Height-titlePanel.Height);
            Controls.Add(tab);

            #endregion

            #region viewPage

            viewPage = new TabPage();
            viewPage.Text = "View";
            viewPage.BackColor = BackColor;
            tab.TabPages.Add(viewPage);

            #endregion

            #region columnsPage

            columnsPage = new TabPage();
            columnsPage.Text = "Columns";
            columnsPage.BackColor = BackColor;
            tab.TabPages.Add(columnsPage);

            #endregion

            #region settingsPage

            settingsPage = new TabPage();
            settingsPage.Text = "Settings";
            settingsPage.BackColor = BackColor;
            tab.TabPages.Add(settingsPage);

            #endregion
        }
    }
}
