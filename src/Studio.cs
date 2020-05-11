using System.Drawing;
using System.Windows.Forms;

namespace MochaDBStudio {
    /// <summary>
    /// Main window of application.
    /// </summary>
    public sealed partial class Studio: Form {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public Studio() {
            Init();
        }

        #endregion
    }

    // Designer.
    public partial class Studio {
        #region Components

        private Panel
            titlePanel;

        #endregion

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Init() {
            #region Base

            Text = "MochaDB Studio";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(810,470);

            #endregion

            #region titlePanel

            titlePanel = new Panel();
            titlePanel.Dock = DockStyle.Top;
            titlePanel.Height = 30;
            titlePanel.BackColor = Color.FromArgb(60,60,60);
            Controls.Add(titlePanel);

            #endregion
        }
    }
}
