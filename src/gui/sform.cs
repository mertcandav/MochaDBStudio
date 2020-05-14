using System;
using System.Windows.Forms;
using MochaDBStudio.Properties;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Form for MochaDB Studio.
    /// </summary>
    public partial class sform:Form {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public sform() {
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
    }

    // Designer.
    public partial class sform {
        /// <summary>
        /// Initialize component.
        /// </summary>
        private void Init() {
            #region Base

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            Icon = Resources.MochaDB_Logo;
            Opacity = 0;

            #endregion
        }
    }
}
