using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MochaDBStudio.gui.codesense {
  /// <summary>
  /// Control for displaying menu items, hosted in IntelliSense.
  /// </summary>
  public interface IAutocompleteListView {
    #region Fields

    event EventHandler ItemSelected;
    event EventHandler<HoveredEventArgs> ItemHovered;

    #endregion

    #region Methods

    /// <summary>
    /// ToolTipleri göster.
    /// </summary>
    /// <param name="autocompleteItem"></param>
    /// <param name="control"></param>
    void ShowToolTip(Item autocompleteItem,Control control = null);

    /// <summary>
    /// Item Rectangle.
    /// </summary>
    Rectangle GetItemRectangle(int itemIndex);

    #endregion

    #region Properties

    ImageList ImageList { get; set; }
    int SelectedItemIndex { get; set; }
    int HighlightedItemIndex { get; set; }
    IList<Item> VisibleItems { get; set; }
    int ToolTipDuration { get; set; }
    Colors Colors { get; set; }

    #endregion
  }
}
