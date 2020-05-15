using System.Drawing;
using System.Windows.Forms;
using MochaDB;
using MochaDB.Mhql;
using MochaDB.Querying;
using MochaDB.Streams;

namespace MochaDBStudio.gui {
    /// <summary>
    /// MHQL query result panel for MochaDB Studio.
    /// </summary>
    public sealed partial class resultPanel:Panel {
        #region Fields

        private Control
            control;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="db">Database.</param>
        public resultPanel(MochaDatabase db) {
            Database = db;
            Init();
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);

            using(var centerFormat = new StringFormat() {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
                e.Graphics.DrawString("There are no results :/",Font,Brushes.LightSlateGray,
                    ClientRectangle,centerFormat);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Show results.
        /// </summary>
        /// <param name="results">Result(s).</param>
        public void ShowResults(MochaReader<object> results) {
            if(control != null)
                control.Dispose();

            if(results.Count == 0)
                return;

            if(results.Count == 1) {
                results.Read();
                var table = (MochaTableResult)results.Value;
                ShowTable(table);
            }
        }

        /// <summary>
        /// Show table result.
        /// </summary>
        /// <param name="table">Table.</param>
        public void ShowTable(MochaTableResult table) {
            var tableGrid = new sgrid();
            tableGrid.Dock = DockStyle.Fill;
            tableGrid.BackgroundColor = BackColor;
            tableGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            for(int columnIndex = 0; columnIndex < table.Columns.Length; columnIndex++)
                tableGrid.Columns.Add(table.Columns[columnIndex].Name,table.Columns[columnIndex].Name);
            for(int rowIndex = 0; rowIndex<table.Rows.Length; rowIndex++) {
                tableGrid.Rows.Add(table.Rows[rowIndex].Datas.ToArray());
            }

            control = tableGrid;
            Controls.Add(tableGrid);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Database.
        /// </summary>
        public MochaDatabase Database { get; set; }

        #endregion
    }

    // Designer.
    public sealed partial class resultPanel {
        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,true);
            Dock = DockStyle.Fill;
            Font = new Font("Arial",10);

            #endregion
        }
    }
}
