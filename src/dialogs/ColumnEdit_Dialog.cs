using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MochaDB;
using MochaDBStudio.gui;

namespace MochaDBStudio.dialogs {
    /// <summary>
    /// Column edit dialog for MochaDB Studio.
    /// </summary>
    public sealed partial class ColumnEdit_Dialog:sform {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="db">Database.</param>
        /// <param name="tableName">Name of table.</param>
        /// <param name="columnName">Name of column.</param>
        public ColumnEdit_Dialog(MochaDatabase db,string tableName,string columnName) {
            Database = db;
            TableName = tableName;
            ColumnName = columnName;
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
            if(tab.SelectedTab == settingsPage) {
                //refreshSettings();
            }
        }

        #endregion

        #region descriptionTB

        private void DescriptionTB_TextChanged(object sender,EventArgs e) {
            Database.SetColumnDescription(TableName,ColumnName,descriptionTB.Text);
        }

        #endregion

        #region dataTypeCBox

        private void DataTypeCBox_SelectedIndexChanged(object sender,EventArgs e) {
            Database.SetColumnDataType(TableName,ColumnName,
                (MochaDataType)Enum.Parse(typeof(MochaDataType),
                dataTypeCBox.Items[dataTypeCBox.SelectedIndex].ToString()));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refresh Struct page.
        /// </summary>
        public void refreshStruct() {

        }

        /// <summary>
        /// Refresh Settings page.
        /// </summary>
        public void refreshSettings() {
            descriptionTB.Text = Database.GetColumnDescription(TableName,ColumnName);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Targetted database.
        /// </summary>
        public MochaDatabase Database { get; private set; }

        /// <summary>
        /// Name of table.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Name of targetted column.
        /// </summary>
        public string ColumnName { get; set; }

        #endregion
    }

    // Designer.
    public sealed partial class ColumnEdit_Dialog {
        #region Components

        private Label
            titleLabel,
            dataTypeLabel;

        private stabcontrol
            tab;

        private TabPage
            structPage,
            settingsPage;

        private spanel
            titlePanel;

        private sbutton
            closeButton;

        private stextbox
            descriptionTB;

        private scombobox
            dataTypeCBox;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            Text = "Editing column";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ForeColor = Color.White;
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
            // Struct
            // 

            #region structPage

            structPage = new TabPage();
            structPage.Text = "Struct";
            structPage.BackColor = BackColor;
            tab.TabPages.Add(structPage);

            #endregion

            #region dataTypeLabel

            dataTypeLabel = new Label();
            dataTypeLabel.AutoSize = true;
            dataTypeLabel.Text = "Data type";
            dataTypeLabel.Font = new Font("Arial",10);
            dataTypeLabel.Location = new Point(20,30);
            structPage.Controls.Add(dataTypeLabel);

            #endregion

            #region dataTypeCBox

            dataTypeCBox = new scombobox();
            dataTypeCBox.ForeColor = ForeColor;
            dataTypeCBox.BackColor = BackColor;
            dataTypeCBox.BorderColor = Color.LightGray;
            dataTypeCBox.Location = new Point(dataTypeLabel.Location.X+dataTypeLabel.Width+50,
                dataTypeLabel.Location.Y);
            dataTypeCBox.Font = dataTypeLabel.Font;
            dataTypeCBox.Size = new Size(200,30);

            var types = Enum.GetNames(typeof(MochaDataType)).ToList();
            types.Sort();
            dataTypeCBox.Items.AddRange(types.ToArray());
            dataTypeCBox.SelectedIndex =
                 dataTypeCBox.Items.IndexOf(Database.GetColumnDataType(TableName,ColumnName).ToString());
            dataTypeCBox.SelectedIndexChanged+=DataTypeCBox_SelectedIndexChanged;
            structPage.Controls.Add(dataTypeCBox);

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
            descriptionTB.Placeholder = "Column description";
            descriptionTB.Text = Database.GetColumnDescription(TableName,ColumnName);
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
