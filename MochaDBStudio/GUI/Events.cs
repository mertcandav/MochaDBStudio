using System;
using MochaDBStudio.GUI.Controls;

namespace MochaDBStudio.GUI {
    /// <summary>
    /// Event arguments for terminal input process.
    /// </summary>
    public sealed class TerminalInputProcessEventArgs:EventArgs {
        #region Constructors

        /// <summary>
        /// Create new TerminalInputProcessEventArgs.
        /// </summary>
        /// <param name="input">Input to add.</param>
        public TerminalInputProcessEventArgs(string input) {
            Input=input;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Input.
        /// </summary>
        public string Input { get; private set; }

        /// <summary>
        /// Cancel to adding.
        /// </summary>
        public bool Cancel { get; set; }

        #endregion
    }

    /// <summary>
    /// Event arguments for item events of ItemListView.
    /// </summary>
    public sealed class ItemListViewItemEventArgs:EventArgs {
        #region Constructors

        /// <summary>
        /// Create new ItemListViewItemEventArgs.
        /// </summary>
        /// <param name="item">MenuItem object.</param>
        public ItemListViewItemEventArgs(MenuItem item) {
            Item = item;
        }

        #endregion

        #region Properties

        /// <summary>
        /// MenuItem object.
        /// </summary>
        public MenuItem Item { get; private set; }

        #endregion
    }
}