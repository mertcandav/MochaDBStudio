using System;
using System.Drawing;
using System.Windows.Forms;
using MochaDB;
using MochaDBStudio.engine;
using MochaDBStudio.gui;
using MochaDBStudio.Properties;

namespace MochaDBStudio.dialogs {
    /// <summary>
    /// Dialog for connect to database.
    /// </summary>
    public sealed partial class OpenScript_Dialog:sform {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public OpenScript_Dialog() {
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

        #region pathTB

        private void PathTB_TextChanged(object sender,EventArgs e) {
            pathTB.BorderColor = Color.DodgerBlue;
        }

        #endregion

        #region openButton

        public void OpenButton_Click(object sender,EventArgs e) {
            if(string.IsNullOrEmpty(pathTB.Text)) {
                pathTB.BorderColor = Color.Red;
                return;
            }

            try {
                var scriptPanel = new scriptpanel(pathTB.Text);
                CNCList.AddScriptItem(new sbutton() { Text = fs.GetFileNameFromPath(pathTB.Text),Tag = scriptPanel });
                Close();
            } catch(Exception excep) {
                errorbox.Show("[Exception]\n" + excep.Message + excep);
            }
        }

        #endregion

        #region pathButton

        private void PathButton_Click(object sender,EventArgs e) {
            using(OpenFileDialog opd = new OpenFileDialog()) {
                opd.Title = "Select MochaScript file.";
                opd.Multiselect = false;
                opd.Filter ="All MochaScript files.|*.mhsc";

                if(opd.ShowDialog()==DialogResult.OK) {
                    pathTB.Text=opd.FileName;
                }
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
    public sealed partial class OpenScript_Dialog {
        #region Components

        public stextbox
            pathTB;

        private sbutton
            cancelButton,
            openButton,
            pathButton;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            Text = "Open MochaScript";
            ShowInTaskbar = false;
            BackColor = Color.FromArgb(50,50,50);
            Size = new Size(400,400);

            #endregion

            #region pathTB

            pathTB = new stextbox();
            pathTB.Placeholder = "Script path";
            pathTB.TextChanged+=PathTB_TextChanged;
            pathTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pathTB.BackColor = BackColor;
            pathTB.ForeColor = Color.White;
            pathTB.Location = new Point(20,60);
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

            #region openButton

            openButton = new sbutton();
            openButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            openButton.Text = "Connect";
            openButton.Size = new Size(60,30);
            openButton.Click+=OpenButton_Click;
            openButton.BackColor = Color.LightGreen;
            openButton.MouseEnterColor = Color.LimeGreen;
            openButton.MouseDownColor = Color.Green;
            openButton.Location = new Point(
                cancelButton.Location.X - openButton.Width - 10,
                Height - openButton.Height - openButton.Location.Y - 10);
            Controls.Add(openButton);

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
