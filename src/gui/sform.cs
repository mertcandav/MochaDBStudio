using System;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Form for MochaDB Studio.
    /// </summary>
    public class sform:Form {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public sform() {

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
}
