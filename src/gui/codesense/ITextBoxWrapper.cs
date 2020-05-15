using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MochaDBStudio.gui.codesense {
    /// <summary>
    /// Wrapper over the control like TextBox.
    /// </summary>
    public interface ITextBoxWrapper {
        #region Fields

        event EventHandler LostFocus;
        event ScrollEventHandler Scroll;
        event KeyEventHandler KeyDown;
        event MouseEventHandler MouseDown;

        #endregion

        #region Methods

        Point GetPositionFromCharIndex(int pos);

        #endregion

        #region Properties

        Control TargetControl { get; }
        string Text { get; }
        string SelectedText { get; set; }
        int SelectionLength { get; set; }
        int SelectionStart { get; set; }
        bool Readonly { get; }

        #endregion
    }
}
