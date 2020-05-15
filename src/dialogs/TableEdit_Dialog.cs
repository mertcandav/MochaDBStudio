using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MochaDB;
using MochaDB.Mhql;
using MochaDB.Querying;
using MochaDBStudio.gui;

namespace MochaDBStudio.dialogs {
    /// <summary>
    /// Table edit form for MochaDB Studio.
    /// </summary>
    public sealed partial class TableEdit_Dialog:sform {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="db">Database.</param>
        /// <param name="tableName">Name of table.</param>
        public TableEdit_Dialog(MochaDatabase db,string tableName) {
            Database = db;
            TableName = tableName;
            Init();
            refreshView();
        }

        #endregion

        #region closeButton

        private void CloseButton_Click(object sender,EventArgs e) {
            Close();
        }

        #endregion

        #region descriptionTB

        private void DescriptionTB_TextChanged(object sender,EventArgs e) {
            Database.SetTableDescription(TableName,descriptionTB.Text);
        }

        #endregion

        #region tab

        private void Tab_SelectedIndexChanged(object sender,EventArgs e) {
            if(tab.SelectedTab == viewPage) {
                refreshView();
            } else if(tab.SelectedTab == settingsPage) {
                descriptionTB.Text = Database.GetTableDescription(TableName);
            }
        }

        #endregion

        #region tableGrid

        private void TableGrid_CellBeginEdit(object sender,DataGridViewCellCancelEventArgs e) {
            if(tableGrid.Updating)
                return;

            tableGrid.Updating = true;
#if DEBUG
            var stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            MochaDataType DataType = Database.GetColumnDataType(TableName,
                tableGrid.Columns[e.ColumnIndex].HeaderText);

            if(DataType == MochaDataType.AutoInt) {
                errorbox.Show("Columns of AutoInt type cannot be edit!");
                e.Cancel = true;
                return;
            }
#if DEBUG
            stopwatch.Stop();
            Console.WriteLine("Table_CellBeginEdit: " + stopwatch.ElapsedMilliseconds);
#endif
            tableGrid.Updating = false;
        }

        private void TableGrid_CellEndEdit(object sender,DataGridViewCellEventArgs e) {
            if(tableGrid.Updating)
                return;
            tableGrid.Updating = true;

#if DEBUG
            var stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            try {
                Database.UpdateData(TableName,tableGrid.Columns[e.ColumnIndex].HeaderText,
                    e.RowIndex,tableGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            } catch(Exception excep) {
                tableGrid.Updating = false;
                refreshView();
                errorbox.Show(excep.ToString());
            }
#if DEBUG
            stopwatch.Stop();
            Console.WriteLine("Table_CellEndEdit: " + stopwatch.ElapsedMilliseconds);
#endif
            tableGrid.Updating = false;
        }

        private void TableGrid_RowsAdded(object sender,DataGridViewRowsAddedEventArgs e) {
            if(tableGrid.Updating)
                return;

            tableGrid.Updating = true;
            MochaData[] datas = new MochaData[tableGrid.ColumnCount];

            for(int index = 0; index < tableGrid.ColumnCount; index++) {

                MochaDataType dataType = Database.GetColumnDataType(TableName,tableGrid.Columns[index].HeaderText);

                if(dataType == MochaDataType.Unique)
                    datas[index] = new MochaData(dataType,"");

                if(dataType == MochaDataType.AutoInt) {
                    int autoIntValue =
                        Database.GetColumnAutoIntState(TableName,tableGrid.Columns[index].HeaderText);

                    tableGrid.Rows[e.RowIndex - 1].Cells[index].Value = autoIntValue + 1;
                    datas[index] = new MochaData(dataType,0);

                    continue;
                }

                datas[index] = new MochaData(dataType,MochaData.TryGetData(dataType,
                    tableGrid.Rows[e.RowIndex - 1].Cells[index].Value));
            }
            Database.AddRow(TableName,new MochaRow(datas));
            Console.WriteLine("Added row");
            tableGrid.Updating = false;
        }

        private void TableGrid_RowsRemoved(object sender,DataGridViewRowsRemovedEventArgs e) {
            if(tableGrid.Updating)
                return;

            tableGrid.Updating = true;

#if DEBUG
            var stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            Database.RemoveRow(TableName,e.RowIndex);
#if DEBUG
            stopwatch.Stop();
            Console.WriteLine("Table_RowsRemoved: " + stopwatch.ElapsedMilliseconds);
#endif
            tableGrid.Updating = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refresh View page.
        /// </summary>
        public void refreshView() {
            if(tableGrid.Updating)
                return;

            tableGrid.Updating = true;
            tableGrid.Columns.Clear();
            var table = Database.ExecuteScalar($@"
                    USE {TableName}
                    RETURN") as MochaTableResult;
            for(int columnIndex = 0; columnIndex < table.Columns.Length; columnIndex++)
                tableGrid.Columns.Add(table.Columns[columnIndex].Name,table.Columns[columnIndex].Name);
            for(int rowIndex = 0; rowIndex<table.Rows.Length; rowIndex++) {
                tableGrid.Rows.Add(table.Rows[rowIndex].Datas.ToArray());
            }
            tableGrid.Updating = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Targetted database.
        /// </summary>
        public MochaDatabase Database { get; private set; }

        /// <summary>
        /// Name of targetted table.
        /// </summary>
        public string TableName { get; set; }

        #endregion
    }

    // Designer.
    public sealed partial class TableEdit_Dialog {
        #region Components

        private Label
            titleLabel;

        private stabcontrol
            tab;

        private TabPage
            viewPage,
            columnsPage,
            settingsPage;

        private spanel
            titlePanel;

        private sbutton
            closeButton;

        private stextbox
            descriptionTB;

        private sgrid
            tableGrid;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            Text = "Editing table";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(50,50,50);
            Size = new Size(790,400);

            #endregion

            #region titlePanel

            titlePanel = new spanel();
            titlePanel.Dock = DockStyle.Top;
            titlePanel.Height = 30;
            titlePanel.BackColor = Color.FromArgb(24,24,24);
            titlePanel.Tag = this;
            Controls.Add(titlePanel);

            #endregion

            #region titleLabel

            titleLabel = new Label();
            titleLabel.Dock = DockStyle.Left;
            titleLabel.Width = 100;
            titleLabel.Text = Text;
            titleLabel.ForeColor = Color.Gray;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Font = new Font("Arial",10,FontStyle.Bold);
            titlePanel.Controls.Add(titleLabel);

            #endregion

            #region closeButton

            closeButton = new sbutton();
            closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
            closeButton.Text = "X";
            closeButton.ForeColor = Color.White;
            closeButton.BackColor = titlePanel.BackColor;
            closeButton.MouseEnterColor = Color.Coral;
            closeButton.MouseDownColor = Color.Red;
            closeButton.Size = new Size(30,titlePanel.Height);
            closeButton.Location = new Point(titlePanel.Width - closeButton.Width,0);
            closeButton.Click +=CloseButton_Click;
            closeButton.TabStop = false;
            titlePanel.Controls.Add(closeButton);

            #endregion

            #region tab

            tab = new stabcontrol();
            tab.BackColor = BackColor;
            tab.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            tab.Location = new Point(0,titlePanel.Height);
            tab.Size = new Size(Width,Height-titlePanel.Height);
            tab.SelectedIndexChanged+=Tab_SelectedIndexChanged;
            Controls.Add(tab);

            #endregion

            // 
            // View
            // 

            #region viewPage

            viewPage = new TabPage();
            viewPage.Text = "View";
            viewPage.BackColor = BackColor;
            tab.TabPages.Add(viewPage);

            #endregion

            #region tableGrid

            tableGrid = new sgrid();
            tableGrid.Dock = DockStyle.Fill;
            tableGrid.BackgroundColor = BackColor;
            tableGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tableGrid.AllowUserToAddRows=true;
            tableGrid.AllowUserToDeleteRows=true;
            tableGrid.ReadOnly = false;
            tableGrid.RowsAdded+=TableGrid_RowsAdded;
            tableGrid.RowsRemoved+=TableGrid_RowsRemoved;
            tableGrid.CellBeginEdit+=TableGrid_CellBeginEdit;
            tableGrid.CellEndEdit+=TableGrid_CellEndEdit;
            viewPage.Controls.Add(tableGrid);

            #endregion

            // 
            // Columns
            // 

            #region columnsPage

            columnsPage = new TabPage();
            columnsPage.Text = "Columns";
            columnsPage.BackColor = BackColor;
            tab.TabPages.Add(columnsPage);

            #endregion

            // 
            // Settings
            // 

            #region settingsPage

            settingsPage = new TabPage();
            settingsPage.Text = "Settings";
            settingsPage.BackColor = BackColor;
            tab.TabPages.Add(settingsPage);

            #endregion

            #region descriptionTB

            descriptionTB = new stextbox();
            descriptionTB.Placeholder = "Database description";
            descriptionTB.BorderColor = Color.LightGray;
            descriptionTB.Multiline = true;
            descriptionTB.BackColor = BackColor;
            descriptionTB.ForeColor = Color.White;
            descriptionTB.Location = new Point(40,30);
            descriptionTB.Size = new Size(Width - (descriptionTB.Location.X * 2)-40,20);
            descriptionTB.InputSize = new Size(descriptionTB.Width,200);
            descriptionTB.TextChanged+=DescriptionTB_TextChanged;
            settingsPage.Controls.Add(descriptionTB);

            #endregion
        }
    }
}
