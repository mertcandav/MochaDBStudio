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
                attributesNode,
                cacheNode,
                columnNode,
                attributeNode;

            while(results.Read()) {
                if(results.Value.GetType() == typeof(MochaTable)) {
                    var table = results.Value as MochaTable;
                    columnsNode = new TreeNode();
                    columnsNode.Text ="Columns";
                    columnsNode.Tag="Columns";
                    columnsNode.ImageIndex = 0;
                    columnsNode.SelectedImageIndex=columnsNode.ImageIndex;

                    attributesNode = new TreeNode();
                    attributesNode.Text ="Attributes";
                    attributesNode.Tag="Attributes";
                    attributesNode.ImageIndex = 0;
                    attributesNode.SelectedImageIndex=attributesNode.ImageIndex;

                    cacheNode = new TreeNode();
                    cacheNode.Text =table.Name;
                    cacheNode.Tag="Table";
                    cacheNode.ImageIndex = 2;
                    cacheNode.SelectedImageIndex=cacheNode.ImageIndex;
                    cacheNode.Nodes.Add(attributesNode);
                    cacheNode.Nodes.Add(columnsNode);

                    for(int attrDex = 0; attrDex < table.Attributes.Count; attrDex++) {
                        attributeNode = new TreeNode();
                        attributeNode.Text = table.Attributes[attrDex].Name;
                        attributeNode.ImageIndex = 5;
                        attributeNode.SelectedImageIndex=attributeNode.ImageIndex;
                        attributeNode.Tag="Attribute";
                        attributesNode.Nodes.Add(attributeNode);
                    }

                    for(int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++) {
                        columnNode = new TreeNode();
                        columnNode.Text =table.Columns[columnIndex].Name;
                        columnNode.ImageIndex = 4;
                        columnNode.SelectedImageIndex=columnNode.ImageIndex;
                        columnNode.Tag="Column";
                        columnsNode.Nodes.Add(columnNode);
                    }

                    explorerTree.Nodes[0].Nodes.Add(cacheNode);
                } else if(results.Value.GetType() == typeof(MochaStack)) {
                    TreeNode GetMochaStackItemNODE(MochaStackItem item) {
                        TreeNode
                            node = new TreeNode(item.Name),
                            attrNode,
                            attrsNode;

                        node.Tag="StackItem";
                        node.ImageIndex = 4;
                        node.SelectedImageIndex=node.ImageIndex;

                        attrsNode = new TreeNode();
                        attrsNode.Text ="Attributes";
                        attrsNode.Tag="Attributes";
                        attrsNode.ImageIndex = 0;
                        attrsNode.SelectedImageIndex=attrsNode.ImageIndex;
                        node.Nodes.Add(attrsNode);

                        for(int attrDex = 0; attrDex < item.Attributes.Count; attrDex++) {
                            attrNode = new TreeNode();
                            attrNode.Text = item.Attributes[attrDex].Name;
                            attrNode.ImageIndex = 5;
                            attrNode.SelectedImageIndex=attrNode.ImageIndex;
                            attrNode.Tag="Attribute";
                            attrsNode.Nodes.Add(attrNode);
                        }

                        if(item.Items.Count>0)
                            for(int index = 0; index < item.Items.Count; index++) {
                                var curitem = item.Items[index];
                                node.Nodes.Add(GetMochaStackItemNODE(curitem));
                            }

                        return node;
                    }

                    var stack = results.Value as MochaStack;

                    cacheNode = new TreeNode();
                    cacheNode.Text =stack.Name;
                    cacheNode.Tag="Stack";
                    cacheNode.ImageIndex=3;
                    cacheNode.SelectedImageIndex=cacheNode.ImageIndex;

                    attributesNode = new TreeNode();
                    attributesNode.Text ="Attributes";
                    attributesNode.Tag="Attributes";
                    attributesNode.ImageIndex = 0;
                    attributesNode.SelectedImageIndex=attributesNode.ImageIndex;
                    cacheNode.Nodes.Add(attributesNode);

                    for(int attrDex = 0; attrDex < stack.Attributes.Count; attrDex++) {
                        attributeNode = new TreeNode();
                        attributeNode.Text = stack.Attributes[attrDex].Name;
                        attributeNode.ImageIndex = 5;
                        attributeNode.SelectedImageIndex=attributeNode.ImageIndex;
                        attributeNode.Tag="Attribute";
                        attributesNode.Nodes.Add(attributeNode);
                    }

                    if(stack.Items.Count >0)
                        for(int itemIndex = 0; itemIndex < stack.Items.Count; itemIndex++) {
                            var curitem = stack.Items[itemIndex];
                            cacheNode.Nodes.Add(GetMochaStackItemNODE(curitem));
                        }

                    explorerTree.Nodes[1].Nodes.Add(cacheNode);
                } else {
                    var sector = results.Value as MochaSector;

                    cacheNode = new TreeNode();
                    cacheNode.Text =sector.Name;
                    cacheNode.Tag="Sector";
                    cacheNode.ImageIndex=4;
                    cacheNode.SelectedImageIndex=cacheNode.ImageIndex;

                    attributesNode = new TreeNode();
                    attributesNode.Text ="Attributes";
                    attributesNode.Tag="Attributes";
                    attributesNode.ImageIndex = 0;
                    attributesNode.SelectedImageIndex=attributesNode.ImageIndex;
                    cacheNode.Nodes.Add(attributesNode);

                    for(int attrDex = 0; attrDex < sector.Attributes.Count; attrDex++) {
                        attributeNode = new TreeNode();
                        attributeNode.Text = sector.Attributes[attrDex].Name;
                        attributeNode.ImageIndex = 5;
                        attributeNode.SelectedImageIndex=attributeNode.ImageIndex;
                        attributeNode.Tag="Attribute";
                        attributesNode.Nodes.Add(attributeNode);
                    }

                    explorerTree.Nodes[2].Nodes.Add(cacheNode);
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
