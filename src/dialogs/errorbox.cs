using System;
using System.Drawing;
using System.Windows.Forms;
using MochaDBStudio.gui;
using MochaDBStudio.Properties;

namespace MochaDBStudio.dialogs {
    /// <summary>
    /// ErrorBox for MochaDB Studio.
    /// </summary>
    public sealed partial class errorbox:sform {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public errorbox() {
            Init();
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);

            using(var pen = new Pen(Color.Red,1))
                e.Graphics.DrawRectangle(pen,0,0,Width-1,Height-1);

            using(var font = new Font("Arial",12,FontStyle.Bold))
            using(var centerFormat = new StringFormat() {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            }) {
                e.Graphics.DrawString("Ops!",font,Brushes.Gray,
                    new Rectangle(0,0,Width,50),centerFormat);

                e.Graphics.DrawString("A problem occurred while running MochaDB Studio.",
                    Font,Brushes.Red,new Rectangle(0,40,Width,30),centerFormat);
            }
        }

        #endregion

        #region okButton

        private void OkButton_Click(object sender,EventArgs e) {
            Close();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Show dialog.
        /// </summary>
        /// <param name="message">Message.</param>
        public static void Show(string message) {
            var dialog = new errorbox();
            dialog.msgTB.Text = message;
            dialog.okButton.Select();
            dialog.ShowDialog();
        }

        #endregion
    }

    // Designer.
    public sealed partial class errorbox {
        #region Components

        private sbutton
            okButton;

        private TextBox
            msgTB;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            Icon = Resources.MochaDB_Logo;
            BackColor = Color.FromArgb(50,50,50);
            Opacity = 0;
            Font = new Font("Arial",9);
            Size = new Size(400,250);

            #endregion

            #region msgTB

            msgTB = new TextBox();
            msgTB.ReadOnly = true;
            msgTB.Multiline = true;
            msgTB.ForeColor = Color.White;
            msgTB.BackColor = BackColor;
            msgTB.BorderStyle = BorderStyle.FixedSingle;
            msgTB.Location = new Point(20,80);
            msgTB.Size = new Size(Width-40,100);
            Controls.Add(msgTB);

            #endregion

            #region okButton

            okButton = new sbutton();
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.Text = "OK";
            okButton.Size = new Size(60,30);
            okButton.Click+=OkButton_Click;
            okButton.BackColor = Color.Gray;
            okButton.MouseEnterColor = Color.DimGray;
            okButton.MouseDownColor = Color.DodgerBlue;
            okButton.Location = new Point(Width-okButton.Width - 10,
                Height - okButton.Height - okButton.Location.Y - 10);
            Controls.Add(okButton);

            #endregion
        }
    }
}
