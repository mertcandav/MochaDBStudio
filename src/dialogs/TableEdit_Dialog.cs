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
    /// Table edit dialog for MochaDB Studio.
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
            refresContent();
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
            if(tab.SelectedTab == contentPage) {
                refresContent();
            } else if(tab.SelectedTab == settingsPage) {
                //refreshSettings();
            }
        }

        #endregion

        #region tableGrid

        private void TableGrid_CellBeginEdit(object sender,DataGridViewCellCancelEventArgs e) {
            tableGrid.Tag = tableGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            /*if(tableGrid.Updating)
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
            tableGrid.Updating = false;*/
        }

        private void TableGrid_CellEndEdit(object sender,DataGridViewCellEventArgs e) {
            if(tableGrid.Updating)
                return;

            if(e.RowIndex >= tableGrid.RowCount-1)
                return;

            tableGrid.Updating = true;

#if DEBUG
            var stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            try {
                var value = tableGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if(value != tableGrid.Tag) {
                    value = value == null ? string.Empty : value.ToString();
                    Database.UpdateData(TableName,tableGrid.Columns[e.ColumnIndex].HeaderText,
                        e.RowIndex,value);
                }
            } catch(Exception excep) {
                tableGrid.Updating = false;
                MochaDataType dataType =
                    Database.GetColumnDataType(
                        TableName,
                        tableGrid.Columns[e.ColumnIndex].HeaderText);
                if(dataType == MochaDataType.AutoInt) {
                    if(tableGrid.Rows.Count == 1) {
                        tableGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 1;
                    } else {
                        var value = 1 + int.Parse(tableGrid.Rows[e.RowIndex-1].Cells[e.ColumnIndex].Value.ToString());
                        tableGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = value;
                    }
                } else {
                    tableGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = tableGrid.Tag;
                }
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
                if(dataType == MochaDataType.AutoInt) {
                    int autoIntValue =
                        Database.GetColumnAutoIntState(TableName,tableGrid.Columns[index].HeaderText);

                    tableGrid.Rows[e.RowIndex - 1].Cells[index].Value = autoIntValue + 1;
                    datas[index] = null;
                    continue;
                }

                var data = new MochaData(dataType,MochaData.TryGetData(dataType,""));
                datas[index] = data;
                tableGrid.Rows[e.RowIndex - 1].Cells[index].Value = data.Data;
            }
            tableGrid.Tag = datas[tableGrid.CurrentCell.ColumnIndex];

            Database.AddRow(TableName,new MochaRow(datas));
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
            if(!Database.RemoveRow(TableName,e.RowIndex))
                Database.RemoveRow(TableName,e.RowIndex-1);
#if DEBUG
            stopwatch.Stop();
            Console.WriteLine("Table_RowsRemoved: " + stopwatch.ElapsedMilliseconds);
#endif
            tableGrid.Updating = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refresh Content page.
        /// </summary>
        public void refresContent() {
            if(tableGrid.Updating)
                return;

            tableGrid.Updating = true;
            try { tableGrid.Columns.Clear(); } catch { return; }
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

        /// <summary>
        /// Refresh Settings page.
        /// </summary>
        public void refreshSettings() {
            descriptionTB.Text = Database.GetTableDescription(TableName);
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
            contentPage,
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
            // Content
            // 

            #region contentPage

            contentPage = new TabPage();
            contentPage.Text = "Content";
            contentPage.BackColor = BackColor;
            tab.TabPages.Add(contentPage);

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
            contentPage.Controls.Add(tableGrid);

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
            descriptionTB.Placeholder = "Table description";
            descriptionTB.Text = Database.GetTableDescription(TableName);
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
