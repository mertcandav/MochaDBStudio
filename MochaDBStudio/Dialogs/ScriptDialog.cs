using System;
using System.Drawing;
using System.Windows.Forms;
using MochaDBStudio.Engine;
using MochaDBStudio.GUI.Controls;
using MochaDBStudio.Properties;

namespace MochaDBStudio.Dialogs {
    /// <summary>
    /// Dialog for script tasks.
    /// </summary>
    public sealed class ScriptDialog:Form {
        #region Fields

        private PageView pageView;

        private MenuSelectionItem
            openItem,
            createItem;

        private FlatButton
            cancelButton,
            createSelectPathButton,
            createCreateButton,
            createCancelButton;

        private Panel
            createPanel;

        private TextInput
            createNameInput,
            createPathInput;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new ScriptDialog.
        /// </summary>
        public ScriptDialog() {
            //Build.
            openItem = new MenuSelectionItem(
                "Open","Open exists MochaScript file.",Resources.ConnectDatabase);
            openItem.Location = new Point(20,20);
            openItem.Click+=OpenItem_Click;
            Controls.Add(openItem);

            createItem = new MenuSelectionItem(
                "Create","Create new MochaScript file.",Resources.AddDatabase);
            createItem.Location = new Point(20,openItem.Location.X + openItem.Height + 20);
            createItem.Click+=CreateItem_Click;
            Controls.Add(createItem);

            Text = "MochaScript";
            BackColor = Color.White;
            ForeColor = Color.Black;
            Size = new Size(openItem.Location.X + openItem.Width + 20,createItem.Location.Y + createItem.Height + 40);
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

            #region createPanel

            createPanel = new Panel();
            createPanel.Dock = DockStyle.Fill;
            createPanel.Visible = false;

            createNameInput = new TextInput();
            createNameInput.Placeholder = "Name of MochaScript file.";
            createNameInput.Width =createPanel.Width - 50;
            createNameInput.Location=new Point(25,20);
            createNameInput.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            createNameInput.TextChanged+=CreateNameInput_TextChanged;
            createPanel.Controls.Add(createNameInput);

            createPathInput = new TextInput();
            createPathInput.Placeholder = "Path of MochaScript file to create.";
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

        private void OpenItem_Click(object sender,EventArgs e) {
            using(OpenFileDialog ofd = new OpenFileDialog()) {
                ofd.Title="Select MochaScript file.";
                ofd.Multiselect=false;
                ofd.Filter="MochaScript files.|*.mochascript";
                if(ofd.ShowDialog()==DialogResult.OK) {
                    ScriptPage scriptPage = new ScriptPage(ofd.FileName);
                    pageView.Add(scriptPage);
                    CancelButton_Click(null,null);
                }
            }
        }

        private void CreateItem_Click(object sender,EventArgs e) {
            createPanel.BringToFront();
            createPanel.Show();
        }

        private void CancelButton_Click(object sender,EventArgs e) {
            Close();
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

            string path = FileSystem.Combine(createPathInput.Text,createNameInput.Text + ".mochascript");
            if(FileSystem.ExistsFile(path)) {
                MessageBox.Show("A MochaScript file with this name already exists on this path.","MochaDB Studio",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            FileSystem.WriteTextFile(path,"//Created with MochaDB Studio.\n\nProvider \n\nBegin\n\nfunc Main()\n{\n}\n\nFinal");
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
            createPanel.Hide();
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Show dialog and dispose finally.
        /// </summary>
        /// <param name="pageView">PageView object to use.</param>
        public static void ShowDialog(PageView pageView) {
            ScriptDialog dbDialog = new ScriptDialog();
            dbDialog.pageView = pageView;
            dbDialog.ShowDialog();
            dbDialog.Dispose();
        }

        #endregion
    }
}