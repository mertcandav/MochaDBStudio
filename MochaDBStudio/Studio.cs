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
        /// <summary>
        /// Initialize.
        /// </summary>
        public void Init() {
            #region Base

            Text = "MochaDB Studio";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;

            #endregion
        }
    }
}
