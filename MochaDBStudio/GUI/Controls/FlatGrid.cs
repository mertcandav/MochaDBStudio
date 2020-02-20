using System.Drawing;
using System.Windows.Forms;

namespace MochaDBStudio.GUI.Controls {
    /// <summary>
    /// Flat DataGridView.
    /// </summary>
    public partial class FlatGrid:DataGridView {
        #region Constructors

        /// <summary>
        /// Create new FlatGrid.
        /// </summary>
        public FlatGrid() {
            //Init.
            Init();
        }

        #endregion
    }

    //Designer.
    public partial class FlatGrid {
        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            Tag = "";
            BorderStyle = BorderStyle.None;
            EnableHeadersVisualStyles = false;
            AllowUserToResizeRows = false;
            SelectionMode = DataGridViewSelectionMode.CellSelect;
            BackgroundColor = Color.White;
            BackColor = BackgroundColor;
            GridColor = Color.DodgerBlue;
            ReadOnly=true;
            AllowUserToAddRows=false;
            ForeColor = Color.Black;
            Font = new Font("Arial",11f,FontStyle.Regular,GraphicsUnit.Pixel);
            
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            AllowUserToResizeRows = false;

            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            ColumnHeadersDefaultCellStyle.BackColor = Color.DodgerBlue;
            ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            ColumnHeadersDefaultCellStyle.Font = new Font("Arial",9f,FontStyle.Bold);

            RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            RowHeadersDefaultCellStyle.BackColor = Color.DodgerBlue;
            RowHeadersDefaultCellStyle.ForeColor = Color.White;
            
            DefaultCellStyle.BackColor = BackgroundColor;
            CellBorderStyle = DataGridViewCellBorderStyle.Single;

            DefaultCellStyle.SelectionBackColor = Color.RoyalBlue;
            DefaultCellStyle.SelectionForeColor = Color.White;
            
            RowHeadersDefaultCellStyle.SelectionBackColor = Color.RoyalBlue;
            RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.RoyalBlue;
            ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        }
    }
}
