using System;
using System.Windows.Forms;

using MochaDBStudio.Properties;

namespace MochaDBStudio.gui {
  /// <summary>
  /// Password shower for password textboxes.
  /// </summary>
  public sealed class passwordeye:PictureBox {
    #region Fields

    private char
        passwordChar = '\b';

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="target">Target text box.</param>
    public passwordeye(stextbox target) {
      TargetTB = target;
      passwordChar = target.PasswordChar;
      Image = Resources.ShowEye;
      SizeMode = PictureBoxSizeMode.Zoom;
      Anchor = AnchorStyles.Top | AnchorStyles.Right;
    }

    #endregion

    #region Mouse override

    protected override void OnClick(EventArgs e) {
      if(TargetTB.PasswordChar != '\0') {
        TargetTB.PasswordChar = '\0';
        Image = Resources.HideEye;
      } else {
        TargetTB.PasswordChar = passwordChar;
        Image = Resources.ShowEye;
      }

      base.OnClick(e);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Targetted text box.
    /// </summary>
    public stextbox TargetTB { get; set; }

    #endregion
  }
}
