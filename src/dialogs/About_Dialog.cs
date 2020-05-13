using System.Drawing;
using System.Windows.Forms;
using MochaDBStudio.gui;
using MochaDBStudio.Properties;

namespace MochaDBStudio.dialogs {
    /// <summary>
    /// About dialog for MochaDB Studio.
    /// </summary>
    public sealed partial class About_Dialog:sform {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public About_Dialog() {
            Init();
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);

            e.Graphics.DrawRectangle(Pens.Green,0,0,Width-1,Height-1);

            using(var font = new Font("Arial",12,FontStyle.Bold))
            using(var centerFormat = new StringFormat() {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
                e.Graphics.DrawString(Text,font,Brushes.Gray,
                    new Rectangle(0,0,Width,50),centerFormat);
        }

        #endregion

        #region okButton

        private void OkButton_Click(object sender,System.EventArgs e) {
            Close();
        }

        #endregion
    }

    // Designer.
    public sealed partial class About_Dialog {
        #region Components

        private sbutton
            okButton;

        private Label
            textLabel;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            Text = "About MochaDB Studio";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            Icon = Resources.MochaDB_Logo;
            BackColor = Color.FromArgb(50,50,50);
            Opacity = 0;
            Size = new Size(400,400);

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

            #region textLabel

            textLabel = new Label();
            textLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            textLabel.Size = new Size(Width-2,300);
            textLabel.Location = new Point(1,60);
            textLabel.ForeColor = Color.White;
            textLabel.Font = new Font("Arial",12,FontStyle.Bold);
            textLabel.TextAlign = ContentAlignment.MiddleCenter;
            textLabel.Text =
$@"Version [ { Resources.Version } ]
Distibution [ { Resources.Distribution } ]

Copyrights 2020 © Mertcan Davulcu";
            Controls.Add(textLabel);

            #endregion
        }
    }
}
