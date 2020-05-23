using System;
using System.Drawing;
using System.Windows.Forms;
using MochaDB;
using MochaDBStudio.engine;
using MochaDBStudio.gui;
using MochaDBStudio.Properties;

namespace MochaDBStudio.dialogs {
    /// <summary>
    /// Dialog for create database.
    /// </summary>
    public sealed partial class CreateScript_Dialog:sform {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateScript_Dialog() {
            Init();
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);

            e.Graphics.DrawRectangle(Pens.DodgerBlue,0,0,Width-1,Height-1);

            using(var font = new Font("Arial",12,FontStyle.Bold))
            using(var centerFormat = new StringFormat() {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
                e.Graphics.DrawString(Text,font,Brushes.Gray,
                    new Rectangle(0,0,Width,50),centerFormat);
        }

        #endregion

        #region cancelButton

        private void CancelButton_Click(object sender,EventArgs e) {
            Close();
        }

        #endregion

        #region nameTB

        private void NameTB_TextChanged(object sender,EventArgs e) {
            nameTB.BorderColor = Color.DodgerBlue;
        }

        #endregion

        #region pathTB

        private void PathTB_TextChanged(object sender,EventArgs e) {
            pathTB.BorderColor = Color.DodgerBlue;
        }

        #endregion

        #region createButton

        private void CreateButton_Click(object sender,EventArgs e) {
            bool ok = true;
            string name = nameTB.Text.TrimStart();
            if(string.IsNullOrEmpty(name)) {
                nameTB.BorderColor = Color.Red;
                ok = false;
            }
            if(string.IsNullOrEmpty(pathTB.Text)) {
                pathTB.BorderColor = Color.Red;
                ok = false;
            }

            if(!ok)
                return;

            string path = fs.Combine(pathTB.Text,name) + ".mhsc";
            if(fs.ExistsFile(path)) {
                errorbox.Show(
$@"[MochaDB-Studio]
A script with this name already exists on this path.");
                return;
            }

            fs.WriteTextFile(path,
$@"// Created with MochaDB Studio {Resources.Version}

Provider 

Begin

func Main()
{{
    
}}

Final");
            var scriptPanel = new scriptpanel(path);
            CNCList.AddScriptItem(new sbutton() { Text = name,Tag = scriptPanel });
            Close();
        }

        #endregion

        #region pathButton

        private void PathButton_Click(object sender,EventArgs e) {
            using(FolderBrowserDialog fbd = new FolderBrowserDialog()) {
                fbd.ShowNewFolderButton = true;
                if(fbd.ShowDialog()==DialogResult.OK)
                    pathTB.Text=fbd.SelectedPath;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Connection list.
        /// </summary>
        public slidemenu CNCList { get; set; }

        #endregion
    }

    // Designer.
    public sealed partial class CreateScript_Dialog {
        #region Components

        private stextbox
            nameTB,
            pathTB;

        private sbutton
            cancelButton,
            createButton,
            pathButton;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            Text = "Create new MochaScript";
            ShowInTaskbar = false;
            BackColor = Color.FromArgb(50,50,50);
            Size = new Size(400,400);

            #endregion

            #region nameTB

            nameTB = new stextbox();
            nameTB.Placeholder = "MochaScript file name";
            nameTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nameTB.BackColor = BackColor;
            nameTB.ForeColor = Color.White;
            nameTB.Location = new Point(20,60);
            nameTB.Size = new Size(Width - (nameTB.Location.X * 2),20);
            nameTB.TextChanged+=NameTB_TextChanged;
            Controls.Add(nameTB);

            #endregion

            #region pathTB

            pathTB = new stextbox();
            pathTB.Placeholder = "File path";
            pathTB.TextChanged+=PathTB_TextChanged;
            pathTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pathTB.BackColor = BackColor;
            pathTB.ForeColor = Color.White;
            pathTB.Location = new Point(20,nameTB.Location.Y + nameTB.Height + 30);
            pathTB.Size = new Size(Width - (pathTB.Location.X * 2),20);
            pathTB.Readonly = true;
            Controls.Add(pathTB);

            #endregion

            #region cancelButton

            cancelButton = new sbutton();
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Text = "Cancel";
            cancelButton.Click+=CancelButton_Click;
            cancelButton.Size = new Size(60,30);
            cancelButton.BackColor = Color.LightCoral;
            cancelButton.MouseEnterColor = Color.Coral;
            cancelButton.MouseDownColor = Color.Red;
            cancelButton.Location = new Point(Width-cancelButton.Width - 10,
                Height - cancelButton.Height - cancelButton.Location.Y - 10);
            Controls.Add(cancelButton);

            #endregion

            #region createButton

            createButton = new sbutton();
            createButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            createButton.Text = "Create";
            createButton.Size = new Size(60,30);
            createButton.Click+=CreateButton_Click;
            createButton.BackColor = Color.LightGreen;
            createButton.MouseEnterColor = Color.LimeGreen;
            createButton.MouseDownColor = Color.Green;
            createButton.Location = new Point(
                cancelButton.Location.X - createButton.Width - 10,
                Height - createButton.Height - createButton.Location.Y - 10);
            Controls.Add(createButton);

            #endregion

            #region pathButton

            pathButton = new sbutton();
            pathButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            pathButton.Text = "Select";
            pathButton.Size = new Size(60,20);
            pathButton.BackColor = Color.LightGray;
            pathButton.MouseEnterColor = Color.Gray;
            pathButton.MouseDownColor = Color.DodgerBlue;
            pathButton.Click+=PathButton_Click;
            pathButton.Location = new Point(
                (pathTB.Location.X + pathTB.Width) - pathButton.Width,
                (pathTB.Location.Y + pathTB.Height) + 10);
            Controls.Add(pathButton);

            #endregion
        }
    }
}
