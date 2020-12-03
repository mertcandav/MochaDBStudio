using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MochaDBStudio.gui.codesense {
  public class SelectingEventArgs:EventArgs {
    #region Properties

    public Item Item { get; internal set; }
    public bool Cancel { get; set; }
    public int SelectedIndex { get; set; }
    public bool Handled { get; set; }

    #endregion
  }

  public class SelectedEventArgs:EventArgs {
    #region Properties

    public Item Item { get; internal set; }
    public Control Control { get; set; }

    #endregion
  }

  public class HoveredEventArgs:EventArgs {
    #region Properties

    public Item Item { get; internal set; }

    #endregion
  }


  public class PaintItemEventArgs:PaintEventArgs {
    #region Constructors

    public PaintItemEventArgs(Graphics graphics,Rectangle clipRect) : base(graphics,clipRect) {
    }

    #endregion

    #region Properties

    public RectangleF TextRect { get; internal set; }
    public StringFormat StringFormat { get; internal set; }
    public Font Font { get; internal set; }
    public bool IsSelected { get; internal set; }
    public bool IsHovered { get; internal set; }
    public Colors Colors { get; internal set; }

    #endregion
  }

  public class WrapperNeededEventArgs:EventArgs {
    #region Constructors

    public WrapperNeededEventArgs(Control targetControl) => this.TargetControl = targetControl;

    #endregion

    #region Properties

    public Control TargetControl { get; private set; }
    public ITextBoxWrapper Wrapper { get; set; }

    #endregion
  }
}
