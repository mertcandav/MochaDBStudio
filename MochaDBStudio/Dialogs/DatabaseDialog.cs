using System;
using System.Drawing;
using System.Windows.Forms;
using MochaDB;
using MochaDBStudio.Engine;
using MochaDBStudio.GUI.Controls;
using MochaDBStudio.Properties;

namespace MochaDBStudio.Dialogs {
    /// <summary>
    /// Dialog for database tasks.
    /// </summary>
    public sealed class DatabaseDialog:Form {
        #region Fields

        private PageView pageView;

        private MenuSelectionItem 
            connectItem,
            createItem;

        private FlatButton 
            cancelButton,
            connectCancelButton,
            connectConnectButton,
            connectSelectPathButton,
            createSelectPathButton,
            createCreateButton,
            createCancelButton;

        private Panel 
            connectPanel,
            createPanel;

        private TextInput
            connectPathInput,
            connectPasswordInput,
            createNameInput,
            createPathInput,
            createDescriptionInput,
            createPasswordInput;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new DatabaseDialog.
        /// </summary>
        public DatabaseDialog() {
            //Build.
            connectItem = new MenuSelectionItem(
                "Connect","Connect exists MochaDB database.",Resources.ConnectDatabase);
            connectItem.Location = new Point(20,20);
            connectItem.Click+=ConnectItem_Click;
            Controls.Add(connectItem);

            createItem = new MenuSelectionItem(
                "Create","Create new MochaDB database.",Resources.AddDatabase);
            createItem.Location = new Point(20,connectItem.Location.X + connectItem.Height + 20);
            createItem.Click+=CreateItem_Click;
            Controls.Add(createItem);

            Text = "Database";
            BackColor = Color.White;
            ForeColor = Color.Black;
            Size = new Size(connectItem.Location.X + connectItem.Width + 20,createItem.Location.Y + createItem.Height + 40);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;

            cancelButton = new FlatButton();
            cancelButton.BackColor = Color.OrangeRed;
            cancelButton.ForeColor = Color.White;
            cancelButton.BorderColor = Color.Red;
            cancelButton.EntryColor = Color.Red;
            cancelButton.Text = "Cancel";
            cancelButton.Size = new Size(50,20);
            cancelButton.Location = new Point(createItem.Location.X + createItem.Width - cancelButton.Width,
                createItem.Location.Y + createItem.Height + 10);
            cancelButton.Click +=CancelButton_Click;
            Controls.Add(cancelButton);

            #region connectPanel

            connectPanel = new Panel();
            connectPanel.Dock = DockStyle.Fill;
            connectPanel.Visible = false;

            connectPathInput = new TextInput();
            connectPathInput.Placeholder = "Path of database to connect.";
            connectPathInput.Width =connectPanel.Width - 50;
            connectPathInput.Location=new Point(25,20);
            connectPathInput.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            connectPathInput.ReadOnly = true;
            connectPathInput.TextChanged+=ConnectPathInput_TextChanged;
            connectPanel.Controls.Add(connectPathInput);

            connectSelectPathButton = new FlatButton();
            connectSelectPathButton.BackColor = Color.Gainsboro;
            connectSelectPathButton.ForeColor = Color.Black;
            connectSelectPathButton.BorderColor = Color.DimGray;
            connectSelectPathButton.EntryColor = Color.Gray;
            connectSelectPathButton.Text = "...";
            connectSelectPathButton.Size = cancelButton.Size;
            connectSelectPathButton.Location =new Point(450,
                connectPathInput.Location.Y+connectPathInput.Height + 5);
            connectSelectPathButton.Click +=ConnectSelectPathButton_Click;
            connectPanel.Controls.Add(connectSelectPathButton);

            connectPasswordInput = new TextInput();
            connectPasswordInput.Placeholder = "Password of database.";
            connectPasswordInput.Width =connectPanel.Width - 50;
            connectPasswordInput.PasswordChar = '●';
            connectPasswordInput.Location=new Point(25,connectSelectPathButton.Location.Y + connectSelectPathButton.Height+50);
            connectPasswordInput.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            connectPasswordInput.TextChanged +=ConnectPasswordInput_TextChanged;
            connectPanel.Controls.Add(connectPasswordInput);

            connectConnectButton = new FlatButton();
            connectConnectButton.BackColor = Color.YellowGreen;
            connectConnectButton.ForeColor =Color.White;
            connectConnectButton.BorderColor = Color.LimeGreen;
            connectConnectButton.EntryColor = Color.LimeGreen;
            connectConnectButton.Text = "Connect";
            connectConnectButton.Size = cancelButton.Size;
            connectConnectButton.Location =new Point(cancelButton.Location.X -cancelButton.Width- 10,cancelButton.Location.Y);
            connectConnectButton.Click +=ConnectConnectButton_Click;
            connectPanel.Controls.Add(connectConnectButton);

            connectCancelButton = new FlatButton();
            connectCancelButton.BackColor = cancelButton.BackColor;
            connectCancelButton.ForeColor = cancelButton.ForeColor;
            connectCancelButton.BorderColor = cancelButton.BorderColor;
            connectCancelButton.EntryColor = cancelButton.EntryColor;
            connectCancelButton.Text = cancelButton.Text;
            connectCancelButton.Size = cancelButton.Size;
            connectCancelButton.Location =cancelButton.Location;
            connectCancelButton.Click +=ConnectCancelButton_Click;
            connectPanel.Controls.Add(connectCancelButton);

            Controls.Add(connectPanel);

            #endregion

            #region createPanel

            createPanel = new Panel();
            createPanel.Dock = DockStyle.Fill;
            createPanel.Visible = false;

            createNameInput = new TextInput();
            createNameInput.Placeholder = "Name of database";
            createNameInput.Width =createPanel.Width - 50;
            createNameInput.Location=new Point(25,20);
            createNameInput.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            createNameInput.TextChanged+=CreateNameInput_TextChanged;
            createPanel.Controls.Add(createNameInput);

            createPathInput = new TextInput();
            createPathInput.Placeholder = "Path of database to create.";
            createPathInput.Width =createPanel.Width - 50;
            createPathInput.Location=new Point(25,createNameInput.Location.Y + createNameInput.Height+20);
            createPathInput.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            createPathInput.ReadOnly = true;
            createPathInput.TextChanged+=CreatePathInput_TextChanged;
            createPanel.Controls.Add(createPathInput);

            createSelectPathButton = new FlatButton();
            createSelectPathButton.BackColor = Color.Gainsboro;
            createSelectPathButton.ForeColor = Color.Black;
            createSelectPathButton.BorderColor = Color.DimGray;
            createSelectPathButton.EntryColor = Color.Gray;
            createSelectPathButton.Text = "...";
            createSelectPathButton.Size = cancelButton.Size;
            createSelectPathButton.Location =new Point(450,
                createPathInput.Location.Y+createPathInput.Height + 5);
            createSelectPathButton.Click +=CreateSelectPathButton_Click;
            createPanel.Controls.Add(createSelectPathButton);

            createDescriptionInput = new TextInput();
            createDescriptionInput.Placeholder = "Description of database.";
            createDescriptionInput.Width =createPanel.Width - 50;
            createDescriptionInput.Location=new Point(25,createSelectPathButton.Location.Y + createSelectPathButton.Height+50);
            createDescriptionInput.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            createPanel.Controls.Add(createDescriptionInput);

            createPasswordInput = new TextInput();
            createPasswordInput.Placeholder = "Password of database.";
            createPasswordInput.Width =createPanel.Width - 50;
            createPasswordInput.PasswordChar = '●';
            createPasswordInput.Location=new Point(25,createDescriptionInput.Location.Y + createDescriptionInput.Height+20);
            createPasswordInput.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            createPanel.Controls.Add(createPasswordInput);

            createCreateButton = new FlatButton();
            createCreateButton.BackColor = Color.YellowGreen;
            createCreateButton.ForeColor =Color.White;
            createCreateButton.BorderColor = Color.LimeGreen;
            createCreateButton.EntryColor = Color.LimeGreen;
            createCreateButton.Text = "Create";
            createCreateButton.Size = cancelButton.Size;
            createCreateButton.Location =new Point(cancelButton.Location.X -cancelButton.Width- 10,cancelButton.Location.Y);
            createCreateButton.Click +=CreateCreateButton_Click;
            createPanel.Controls.Add(createCreateButton);

            createCancelButton = new FlatButton();
            createCancelButton.BackColor = cancelButton.BackColor;
            createCancelButton.ForeColor = cancelButton.ForeColor;
            createCancelButton.BorderColor = cancelButton.BorderColor;
            createCancelButton.EntryColor = cancelButton.EntryColor;
            createCancelButton.Text = cancelButton.Text;
            createCancelButton.Size = cancelButton.Size;
            createCancelButton.Location =cancelButton.Location;
            createCancelButton.Click +=CreateCancelButton_Click;
            createPanel.Controls.Add(createCancelButton);

            Controls.Add(createPanel);

            #endregion

            cancelButton.Select();
        }

        #endregion

        #region Main Events

        private void ConnectItem_Click(object sender,EventArgs e) {
            connectPanel.BringToFront();
            connectPanel.Show();
        }

        private void CreateItem_Click(object sender,EventArgs e) {
            createPanel.BringToFront();
            createPanel.Show();
        }

        private void CancelButton_Click(object sender,EventArgs e) {
            Close();
        }

        #endregion

        #region connectPanel

        private void ConnectPathInput_TextChanged(object sender,EventArgs e) {
            connectPathInput.BorderColor = Color.DodgerBlue;
        }

        private void ConnectPasswordInput_TextChanged(object sender,EventArgs e) {
            connectPasswordInput.BorderColor = Color.DodgerBlue;
        }

        private void ConnectConnectButton_Click(object sender,EventArgs e) {
            if(string.IsNullOrEmpty(connectPathInput.Text)) {
                connectPathInput.BorderColor = Color.Red;
                return;
            }

            try {
                MochaDatabase db = new MochaDatabase(connectPathInput.Text,connectPasswordInput.Text);
                db.Connect();
                ConnectionPage dbPage = new ConnectionPage(db);
                dbPage.Tip=connectPathInput.Text;
                pageView.Add(dbPage);
                ConnectCancelButton_Click(null,null);
                CancelButton_Click(null,null);
                MessageBox.Show("Connected database successfully.","MochaDB Studio",
                    MessageBoxButtons.OK,MessageBoxIcon.Information);
            } catch(Exception excep) {
                if(excep.Message == "MochaDB database password does not match the password specified!")
                    connectPasswordInput.BorderColor = Color.Red;
                else {
                    MessageBox.Show(excep.Message,"MochaDB Studio",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }

        private void ConnectSelectPathButton_Click(object sender,EventArgs e) {
            using(OpenFileDialog opd = new OpenFileDialog()) {
                opd.Title = "Select MochaDB database file.";
                opd.Multiselect = false;
                opd.Filter ="All MochaDB database files.|*.mochadb";

                if(opd.ShowDialog()==DialogResult.OK) {
                    connectPathInput.Text=opd.FileName;
                }
            }
        }

        private void ConnectCancelButton_Click(object sender,EventArgs e) {
            connectPathInput.Clear();
            connectPasswordInput.Clear();
            connectPanel.Hide();
        }

        #endregion

        #region createPanel

        private void CreatePathInput_TextChanged(object sender,EventArgs e) {
            createPathInput.BorderColor = Color.DodgerBlue;
        }

        private void CreateNameInput_TextChanged(object sender,EventArgs e) {
            createNameInput.BorderColor = Color.DodgerBlue;
        }

        private void CreateCreateButton_Click(object sender,EventArgs e) {
            bool ok = true;
            string name = createNameInput.Text.Trim();
            if(string.IsNullOrEmpty(name)) {
                createNameInput.BorderColor = Color.Red;
                ok = false;
            }
            if(string.IsNullOrEmpty(createPathInput.Text)) {
                createPathInput.BorderColor = Color.Red;
                ok = false;
            }

            if(!ok)
                return;

            string path = FileSystem.Combine(createPathInput.Text,createNameInput.Text);
            if(FileSystem.ExistsFile(path + ".mochadb")) {
                MessageBox.Show("A database with this name already exists on this path.","MochaDB Studio",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            MochaDatabase.CreateMochaDB(path,createDescriptionInput.Text.Trim(),createPasswordInput.Text);
            CreateCancelButton_Click(null,null);
            MessageBox.Show("Created successfully.","MochaDB Studio",
                MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void CreateSelectPathButton_Click(object sender,EventArgs e) {
            using(FolderBrowserDialog fbd = new FolderBrowserDialog()) {
                fbd.ShowNewFolderButton = true;
                if(fbd.ShowDialog()==DialogResult.OK)
                    createPathInput.Text=fbd.SelectedPath;
            }
        }

        private void CreateCancelButton_Click(object sender,EventArgs e) {
            createNameInput.Clear();
            createPathInput.Clear();
            createDescriptionInput.Clear();
            createPasswordInput.Clear();
            createPanel.Hide();
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Show dialog and dispose finally.
        /// </summary>
        /// <param name="pageView">PageView object to use.</param>
        public static void ShowDialog(PageView pageView) {
            DatabaseDialog dbDialog = new DatabaseDialog();
            dbDialog.pageView = pageView;
            dbDialog.ShowDialog();
            dbDialog.Dispose();
        }

        #endregion
    }
}