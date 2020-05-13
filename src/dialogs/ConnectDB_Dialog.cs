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
    public sealed partial class ConnectDB_Dialog:Form {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConnectDB_Dialog() {
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

        #region passwordTB

        private void PasswordTB_TextChanged(object sender,EventArgs e) {
            passwordTB.BorderColor = Color.DodgerBlue;
        }

        #endregion

        #region connectButton

        private void ConnectButton_Click(object sender,EventArgs e) {
            if(string.IsNullOrEmpty(pathTB.Text)) {
                pathTB.BorderColor = Color.Red;
                return;
            }

            try {
                MochaDatabase db = new MochaDatabase($@"
                    AutoConnect=True; Path={pathTB.Text};
                    Password={passwordTB.Text};
                    Logs={logToggle.Checked}");

                var connectionPanel = new cncpanel(db);
                CNCList.AddItem(new sbutton() { Text = fs.GetFileNameFromPath(pathTB.Text),Tag = connectionPanel });
                Close();
            } catch(MochaException excep) {
                if(excep.Message == "MochaDB database password does not match the password specified!" |
                   excep.Message == "The MochaDB database is password protected!")
                    passwordTB.BorderColor = Color.Red;
                else
                    MessageBox.Show(excep.Message,"MochaDB Studio",MessageBoxButtons.OK,MessageBoxIcon.Error);
            } catch(Exception excep) {
                MessageBox.Show(excep.Message,"MochaDB Studio",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        #endregion

        #region pathButton

        private void PathButton_Click(object sender,EventArgs e) {
            using(OpenFileDialog opd = new OpenFileDialog()) {
                opd.Title = "Select MochaDB database file.";
                opd.Multiselect = false;
                opd.Filter ="All MochaDB database files.|*.mochadb";

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
    public sealed partial class ConnectDB_Dialog {
        #region Components

        private stextbox
            pathTB,
            passwordTB;

        private sbutton
            cancelButton,
            connectButton,
            pathButton;

        private toggle
            logToggle;

        private passwordeye
            passwordTBeye;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            Text = "Connect to MochaDB Database";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            Icon = Resources.MochaDB_Logo;
            BackColor = Color.FromArgb(50,50,50);
            Opacity = 0;
            Size = new Size(400,400);

            #endregion

            #region pathTB

            pathTB = new stextbox();
            pathTB.Placeholder = "Database path";
            pathTB.TextChanged+=PathTB_TextChanged;
            pathTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pathTB.BackColor = BackColor;
            pathTB.ForeColor = Color.White;
            pathTB.Location = new Point(20,60);
            pathTB.Size = new Size(Width - (pathTB.Location.X * 2),20);
            pathTB.Readonly = true;
            Controls.Add(pathTB);

            #endregion

            #region passwordTB

            passwordTB = new stextbox();
            passwordTB.Placeholder = "Database password";
            passwordTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            passwordTB.BackColor = BackColor;
            passwordTB.ForeColor = Color.White;
            passwordTB.Location = new Point(20,pathTB.Location.Y + pathTB.Height + 60);
            passwordTB.Size = new Size(Width - (passwordTB.Location.X * 2) - 40,20);
            passwordTB.PasswordChar = '●';
            passwordTB.TextChanged+=PasswordTB_TextChanged;
            Controls.Add(passwordTB);

            #endregion

            #region passwordTBeye

            passwordTBeye = new passwordeye(passwordTB);
            passwordTBeye.Size = new Size(30,passwordTB.Height);
            passwordTBeye.Location = new Point(
                passwordTB.Location.X+passwordTB.Width + 5,passwordTB.Location.Y);
            Controls.Add(passwordTBeye);

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

            #region connectButton

            connectButton = new sbutton();
            connectButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            connectButton.Text = "Connect";
            connectButton.Size = new Size(60,30);
            connectButton.Click+=ConnectButton_Click;
            connectButton.BackColor = Color.LightGreen;
            connectButton.MouseEnterColor = Color.LimeGreen;
            connectButton.MouseDownColor = Color.Green;
            connectButton.Location = new Point(
                cancelButton.Location.X - connectButton.Width - 10,
                Height - connectButton.Height - connectButton.Location.Y - 10);
            Controls.Add(connectButton);

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

            #region logToggle

            logToggle = new toggle();
            logToggle.Text = "Keep log";
            logToggle.ForeColor = Color.White;
            logToggle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            logToggle.BackColor = BackColor;
            logToggle.Location = new Point(20,passwordTB.Location.Y + passwordTB.Height + 100);
            logToggle.Size = new Size(130,20);
            Controls.Add(logToggle);

            #endregion
        }
    }
}
