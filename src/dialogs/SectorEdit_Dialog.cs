using System;
using System.Drawing;
using System.Windows.Forms;
using MochaDB;
using MochaDBStudio.gui;

namespace MochaDBStudio.dialogs {
    /// <summary>
    /// Sector edit dialog for MochaDB Studio.
    /// </summary>
    public sealed partial class SectorEdit_Dialog:sform {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="db">Database.</param>
        /// <param name="sectorName">Name of sector.</param>
        public SectorEdit_Dialog(MochaDatabase db,string sectorName) {
            Database = db;
            SectorName = sectorName;
            Init();
        }

        #endregion

        #region closeButton

        private void CloseButton_Click(object sender,EventArgs e) {
            Close();
        }

        #endregion

        #region tab

        private void Tab_SelectedIndexChanged(object sender,EventArgs e) {
            
        }

        #endregion

        #region valueTB

        private void ValueTB_TextChanged(object sender,EventArgs e) {
            Database.SetSectorData(SectorName,valueTB.Text);
        }

        #endregion

        #region descriptionTB

        private void DescriptionTB_TextChanged(object sender,EventArgs e) {
            Database.SetSectorDescription(SectorName,descriptionTB.Text);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Targetted database.
        /// </summary>
        public MochaDatabase Database { get; private set; }

        /// <summary>
        /// Name of targetted sector.
        /// </summary>
        public string SectorName { get; set; }

        #endregion
    }

    // Designer.
    public sealed partial class SectorEdit_Dialog {
        #region Components

        private Label
            titleLabel;

        private stabcontrol
            tab;

        private TabPage
            contentPage,
            settingsPage;

        private spanel
            titlePanel;

        private sbutton
            closeButton;

        private stextbox
            valueTB,
            descriptionTB;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            Text = "Editing sector";
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
            titleLabel.Width = 120;
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
            tab.SelectedIndexChanged+=Tab_SelectedIndexChanged;
            Controls.Add(tab);

            #endregion

            // 
            // Content
            // 

            #region contentPage

            contentPage = new TabPage();
            contentPage.Text = "Content";
            contentPage.BackColor = BackColor;
            tab.TabPages.Add(contentPage);

            #endregion

            #region valueTB

            valueTB = new stextbox();
            valueTB.Placeholder = "Sector value";
            valueTB.Text = Database.GetSectorData(SectorName);
            valueTB.BorderColor = Color.LightGray;
            valueTB.Multiline = true;
            valueTB.BackColor = BackColor;
            valueTB.ForeColor = Color.White;
            valueTB.Location = new Point(40,30);
            valueTB.Size = new Size(Width - (valueTB.Location.X * 2)-40,20);
            valueTB.InputSize = new Size(valueTB.Width,200);
            valueTB.TextChanged+=ValueTB_TextChanged;
            contentPage.Controls.Add(valueTB);

            #endregion

            // 
            // Settings
            // 

            #region settingsPage

            settingsPage = new TabPage();
            settingsPage.Text = "Settings";
            settingsPage.BackColor = BackColor;
            tab.TabPages.Add(settingsPage);

            #endregion

            #region descriptionTB

            descriptionTB = new stextbox();
            descriptionTB.Placeholder = "Sector description";
            descriptionTB.Text = Database.GetSectorDescription(SectorName);
            descriptionTB.BorderColor = Color.LightGray;
            descriptionTB.Multiline = true;
            descriptionTB.BackColor = BackColor;
            descriptionTB.ForeColor = Color.White;
            descriptionTB.Location = new Point(40,30);
            descriptionTB.Size = new Size(Width - (descriptionTB.Location.X * 2)-40,20);
            descriptionTB.InputSize = new Size(descriptionTB.Width,200);
            descriptionTB.TextChanged+=DescriptionTB_TextChanged;
            settingsPage.Controls.Add(descriptionTB);

            #endregion
        }
    }
}
