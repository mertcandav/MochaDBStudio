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
        if(results.Value.GetType() == typeof(MochaTableResult)) {
          var table = (MochaTableResult)results.Value;
          ShowTable(table);
          return;
        }
        results.GoBack();
      }

      var explorerTree = new dbtree();
      explorerTree.ForeColor = Color.White;
      explorerTree.BackColor = BackColor;

      TreeNode
          columnsNode,
          cacheNode,
          columnNode;

      while(results.Read()) {
        if(results.Value.GetType() == typeof(MochaTable)) {
          var table = results.Value as MochaTable;
          columnsNode = new TreeNode();
          columnsNode.Text ="Columns";
          columnsNode.Tag="Columns";
          columnsNode.ImageIndex = 0;
          columnsNode.SelectedImageIndex=columnsNode.ImageIndex;

          cacheNode = new TreeNode();
          cacheNode.Text =table.Name;
          cacheNode.Tag="Table";
          cacheNode.ImageIndex = 2;
          cacheNode.SelectedImageIndex=cacheNode.ImageIndex;
          cacheNode.Nodes.Add(columnsNode);

          for(int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
            columnNode = new TreeNode();
            columnNode.Text =table.Columns[columnIndex].Name;
            columnNode.ImageIndex = 4;
            columnNode.SelectedImageIndex=columnNode.ImageIndex;
            columnNode.Tag="Column";
            columnsNode.Nodes.Add(columnNode);
          }

          explorerTree.Nodes.Add(cacheNode);
        }
      }

      control = explorerTree;
      Controls.Add(explorerTree);
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
        tableGrid.Columns.Add(string.Empty,table.Columns[columnIndex].MHQLAsText);
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
