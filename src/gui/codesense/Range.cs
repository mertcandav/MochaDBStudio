using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MochaDBStudio.gui.codesense {
    public class Range {
        #region Constructors

        public Range(ITextBoxWrapper targetWrapper) {
            this.TargetWrapper = targetWrapper;
        }

        #endregion

        #region Properties

        public ITextBoxWrapper TargetWrapper { get; private set; }
        public int Start { get; set; }
        public int End { get; set; }

        public string Text {
            get {
                var text = TargetWrapper.Text;

                if(string.IsNullOrEmpty(text))
                    return "";
                if(Start >= text.Length)
                    return "";
                if(End > text.Length)
                    return "";

                return TargetWrapper.Text.Substring(Start,End - Start);
            }

            set {
                TargetWrapper.SelectionStart = Start;
                TargetWrapper.SelectionLength = End - Start;
                TargetWrapper.SelectedText = value;
            }
        }

        #endregion
    }
}
