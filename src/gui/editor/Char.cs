using System;

namespace MochaDBStudio.gui.editor {
    /// <summary>
    /// Char and style
    /// </summary>
    public struct Char {
        #region Tanımlar

        public char c;

        /// <summary>
        /// Style bit mask
        /// </summary>
        /// <remarks>Bit 1 in position n means that this char will rendering by FusionTextBox.Styles[n]</remarks>
        public StyleIndex style;

        #endregion

        #region Ana Yükleme

        public Char(char c) {
            this.c = c;
            style = StyleIndex.None;
        }

        #endregion
    }
}
