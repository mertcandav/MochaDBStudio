using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MochaDB;
using MochaDB.Mhql;
using MochaDB.Querying;
using MochaDBStudio.gui;
using MochaDBStudio.gui.editor;

namespace MochaDBStudio.dialogs {
    /// <summary>
    /// MochaScript dialog for MochaDB Studio.
    /// </summary>
    public sealed partial class MochaScript_Dialog:sform {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public MochaScript_Dialog() {
            Init();
        }

        #endregion

        #region closeButton

        private void CloseButton_Click(object sender,EventArgs e) {
            Close();
        }

        #endregion
    }

    // Designer.
    public sealed partial class MochaScript_Dialog {
        #region Components

        private Label
            titleLabel;

        private spanel
            titlePanel;

        private sbutton
            closeButton;

        public editor
            codeBox;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            Text = "MochaScript";
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

            #region codeBox

            codeBox = new editor();
            codeBox.Dock = DockStyle.Fill;
            codeBox.Language = Language.MochaScript;
            codeBox.BackColor = BackColor;
            codeBox.ForeColor = Color.White;
            codeBox.LineNumberColor = Color.Khaki;
            codeBox.CurrentLineColor = Color.Black;
            codeBox.ServiceLinesColor = Color.Transparent;
            codeBox.CaretColor = Color.White;
            codeBox.WordWrap = false;
            codeBox.ReadOnly = true;
            Controls.Add(codeBox);

            #endregion
        }
    }
}
