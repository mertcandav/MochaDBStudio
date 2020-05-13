using System.Drawing;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
    /// <summary>
    /// DataGridView for MochaDB Studio.
    /// </summary>
    public partial class sgrid:DataGridView {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public sgrid() {
            //Init.
            Init();
        }

        #endregion
    }

    //Designer.
    public partial class sgrid {
        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            Tag = "";
            BorderStyle = BorderStyle.None;
            EnableHeadersVisualStyles = false;
            AllowUserToResizeRows = false;
            AllowUserToAddRows=false;
            AllowUserToDeleteRows=false;
            SelectionMode = DataGridViewSelectionMode.CellSelect;
            BackgroundColor = Color.FromArgb(60,60,60);
            BackColor = BackgroundColor;
            GridColor = Color.Gray;
            ForeColor = Color.White;
            Font = new Font("Arial",11f,FontStyle.Regular,GraphicsUnit.Pixel);

            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            ColumnHeadersDefaultCellStyle.ForeColor = Color.Khaki;
            ColumnHeadersDefaultCellStyle.Font = new Font("Arial",9f,FontStyle.Bold);

            RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            RowHeadersDefaultCellStyle.BackColor = Color.Gray;
            RowHeadersDefaultCellStyle.ForeColor = Color.White;

            DefaultCellStyle.BackColor = BackgroundColor;
            CellBorderStyle = DataGridViewCellBorderStyle.Single;

            DefaultCellStyle.SelectionBackColor = Color.Black;
            DefaultCellStyle.SelectionForeColor = Color.White;

            RowHeadersDefaultCellStyle.SelectionBackColor = Color.Black;
            RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.Black;
            ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        }
    }
}
