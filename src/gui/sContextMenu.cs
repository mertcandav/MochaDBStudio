using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace MochaDBStudio.gui {
  /// <summary>
  /// Context menu for MochaDB Studio.
  /// </summary>
  public class sContextMenu:ContextMenuStrip {
    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public sContextMenu() {
      ImageScalingSize = new Size(17,17);
      ShowCheckMargin = true;
    }

    #endregion

    #region Drawing

    protected override void OnPaintBackground(PaintEventArgs e) {
      using(var bgbrush = new SolidBrush(BackColor))
        e.Graphics.FillRectangle(bgbrush,ClientRectangle);
    }

    #endregion
  }
}
