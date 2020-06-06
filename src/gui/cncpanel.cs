using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MochaDB;
using MochaDB.FileSystem;
using MochaDB.Logging;
using MochaDB.Mhql;
using MochaDB.Querying;
using MochaDB.Streams;
using MochaDBStudio.dialogs;
using MochaDBStudio.gui.codesense;
using MochaDBStudio.gui.editor;
using MochaDBStudio.Properties;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Connection Panel.
    /// </summary>
    public sealed partial class cncpanel:Panel {
        #region Fields

        private Task
            mhqlRefreshTask;

        private bool
            reshExplorer = false,
            reshMHQL = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public cncpanel() {
            Init();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="database">Target database.</param>
        public cncpanel(MochaDatabase database)
            : this() {
            Database = database;
            Database.Changed+=Database_Changed;

            term.Database = Database;
            refreshDashboard();
            refreshExplorer();
        }

        #endregion

        #region tab

        private void Tab_SelectedIndexChanged(object sender,EventArgs e) {
            var form = (Studio)Parent.Parent;
            if(form.titlePanel.Controls.ContainsKey("mhqlButton")) {
                var mhqlButton =
                    form.titlePanel.Controls[form.titlePanel.Controls.IndexOfKey("mhqlButton")]
                    as sbutton;
                mhqlButton.ContextMenu.Dispose();
                mhqlButton.Dispose();
                form.helpButton.Location = new Point(
                    form.connectionButton.Location.X+form.connectionButton.Width,0);
            }

            /*if(tab.SelectedTab == dashboardPage) {
                refreshDashboard();
            } else */if(tab.SelectedTab == explorerPage) {
                if(reshExplorer)
                    refreshExplorer();
            } else if(tab.SelectedTab == terminalPage) {
                term.Select();
            } else if(tab.SelectedTab == mhqlPage) {
                #region mhqlCM

                var mhqlCM = new sContextMenu();
                mhqlCM.ForeColor = Color.White;
                mhqlCM.BackColor = Color.FromArgb(24,24,24);
                mhqlCM.Items.Add(new sContextMenuItem("Run",
                    mhqlCM.BackColor,Color.Gray) {
                    Image = Resources.Play,
                    ShortcutKeyDisplayString = "F5"
                });
                mhqlCM.ItemClicked += (object mhqlCM_Sender,ToolStripItemClickedEventArgs mhqlCM_e) => {
                    if(mhqlCM_e.ClickedItem.Text == "Run")
                        ExecuteMHQL();
                };

                #endregion

                #region mhqlButton

                var mhqlButton = new sbutton();
                mhqlButton.Name = "mhqlButton";
                mhqlButton.Font = new Font("Microsoft Sans Serif",9);
                mhqlButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top;
                mhqlButton.Text = "MHQL";
                mhqlButton.ForeColor = Color.White;
                mhqlButton.BackColor = mhqlCM.BackColor;
                mhqlButton.MouseEnterColor = Color.Gray;
                mhqlButton.MouseDownColor = Color.DodgerBlue;
                mhqlButton.Location = new Point(form.connectionButton.Location.X+form.connectionButton.Width,0);
                mhqlButton.Size = new Size(70,30);
                mhqlButton.ContextMenu = mhqlCM;
                mhqlButton.DisableClick = true;

                #endregion

                form.helpButton.Location = new Point(mhqlButton.Location.X+mhqlButton.Width,0);
                form.titlePanel.Controls.Add(mhqlButton);

                if(reshMHQL)
                    refreshMHQL();
            } else if(tab.SelectedTab == settingsPage) {
                refreshSettings();
            }
        }

        #endregion

        #region passwordTB

        private void PasswordTB_TextChanged(object sender,EventArgs e) {
            Database.SetPassword(passwordTB.Text);
        }

        #endregion

        #region descriptionTB

        private void DescriptionTB_TextChanged(object sender,EventArgs e) {
            Database.SetDescription(descriptionTB.Text);
        }

        #endregion

        #region explorerTree

        private void ExplorerTree_BeforeLabelEdit(object sender,NodeLabelEditEventArgs e) {
            string tag = e.Node.Tag as string;
            e.CancelEdit =
                tag=="Tables" ? true :
                tag=="Columns" ? true :
                tag=="Sectors" ? true :
                tag=="Stacks" ? true :
                tag=="Attributes" ? true : false;
        }

        private void ExplorerTree_AfterLabelEdit(object sender,NodeLabelEditEventArgs e) {
            if(string.IsNullOrWhiteSpace(e.Label)) {
                e.CancelEdit = true;
                return;
            }

            try {
                if(explorerTree.SelectedNode.Tag=="Table") {
                    Database.RenameTable(e.Node.Text,e.Label.Trim());
                } else if(explorerTree.SelectedNode.Tag=="Column") {
                    Database.RenameColumn(e.Node.Parent.Parent.Text,e.Node.Text,e.Label.Trim());
                } else if(explorerTree.SelectedNode.Tag=="Sector") {
                    Database.RenameSector(e.Node.Text,e.Label.Trim());
                } else if(explorerTree.SelectedNode.Tag=="Stack") {
                    Database.RenameStack(e.Node.Text,e.Label.Trim());
                } else if(explorerTree.SelectedNode.Tag=="StackItem") {
                    Database.RenameStackItem(GetStackItemStackName(explorerTree.SelectedNode),e.Label.Trim(),
                        GetStackItemPath(explorerTree.SelectedNode));
                }
            } catch(Exception excep) {
                e.CancelEdit=true;
                errorbox.Show(excep.Message);
            }
        }

        private void ExplorerTree_KeyDown(object sender,KeyEventArgs e) {
            if(e.KeyCode == Keys.Delete) {
                if(explorerTree.SelectedNode.Tag!="Table" &
                   explorerTree.SelectedNode.Tag!="Column" &
                   explorerTree.SelectedNode.Tag!="Stack" &
                   explorerTree.SelectedNode.Tag!="Sector" &
                   explorerTree.SelectedNode.Tag!="StackItem")/* &
                   explorerTree.SelectedNode.Tag.ToString().EndsWith("Attribute") == false)*/
                    return;

                if(explorerTree.SelectedNode.Tag=="Table")
                    Database.RemoveTable(explorerTree.SelectedNode.Text);
                else if(explorerTree.SelectedNode.Tag=="Column")
                    Database.RemoveColumn(explorerTree.SelectedNode.Parent.Parent.Text,explorerTree.SelectedNode.Text);
                else if(explorerTree.SelectedNode.Tag=="Sector")
                    Database.RemoveSector(explorerTree.SelectedNode.Text);
                else if(explorerTree.SelectedNode.Tag=="Stack")
                    Database.RemoveStack(explorerTree.SelectedNode.Text);
                else if(explorerTree.SelectedNode.Tag=="StackItem")
                    Database.RemoveStackItem(GetStackItemStackName(explorerTree.SelectedNode),
                        GetStackItemPath(explorerTree.SelectedNode));
                /*else if(explorerTree.SelectedNode.Tag=="TableAttribute")
                    Database.RemoveTableAttribute(explorerTree.SelectedNode.Parent.Parent.Text,
                        explorerTree.SelectedNode.Text);
                else if(explorerTree.SelectedNode.Tag=="SectorAttribute")
                    Database.RemoveSectorAttribute(explorerTree.SelectedNode.Parent.Parent.Text,
                        explorerTree.SelectedNode.Text);
                else if(explorerTree.SelectedNode.Tag=="StackAttribute")
                    Database.RemoveStackAttribute(explorerTree.SelectedNode.Parent.Parent.Text,
                        explorerTree.SelectedNode.Text);
                else if(explorerTree.SelectedNode.Tag=="StackItemAttribute")
                    Database.RemoveStackItemAttribute(GetStackItemStackName(explorerTree.SelectedNode),
                        GetStackItemPath(explorerTree.SelectedNode.Parent.Parent),explorerTree.SelectedNode.Text);*/

                explorerTree.SelectedNode.Remove();
            } else if(e.KeyCode==Keys.F2) {
                if(explorerTree.SelectedNode!=null)
                    explorerTree.SelectedNode.BeginEdit();
            } else if(e.KeyCode==Keys.Escape) {
                explorerTree.SelectedNode.EndEdit(true);
            }
        }

        private void ExplorerTree_NodeMouseDoubleClick(object sender,TreeNodeMouseClickEventArgs e) {
            if(e.Node.Tag == "Table") {
                var dialog = new TableEdit_Dialog(Database,e.Node.Text);
                dialog.ShowDialog();
            } else if(e.Node.Tag == "Column") {
                var dialog = new ColumnEdit_Dialog(Database,e.Node.Parent.Parent.Text,e.Node.Text);
                dialog.ShowDialog();
            } else if(e.Node.Tag == "Sector") {
                var dialog = new SectorEdit_Dialog(Database,e.Node.Text);
                dialog.ShowDialog();
            } else if(e.Node.Tag == "Stack") {
                var dialog = new StackEdit_Dialog(Database,e.Node.Text);
                dialog.ShowDialog();
            } else if(e.Node.Tag == "StackItem") {
                var parts = e.Node.FullPath.Split(new[] { '/' },3);
                var dialog = new StackItemEdit_Dialog(Database,
                    parts[1],e.Node.FullPath.Substring(e.Node.FullPath.IndexOf('/',e.Node.FullPath.IndexOf('/')+1)+1));
                dialog.ShowDialog();
            }
        }

        #endregion

        #region mhqlEditor

        private void MhqlEditor_KeyDown(object sender,KeyEventArgs e) {
            if(e.KeyCode != Keys.F5)
                return;

            ExecuteMHQL();
        }

        #endregion

        #region mhqlTestButton

        private void MhqlTestButton_Click(object sender,EventArgs e) {
            new Task(() => {
                mhqlTestButton.Enabled = false;
                mhqlTestRB.Text = "Testing...";
                mhqlTestRB.State = 0;
                var sw = new Stopwatch();
                long total = 0;
                for(short counter = 1; counter <= 5; counter++) {
                    sw.Start();
                    Database.ExecuteScalar(
$@"
SELECT {(double.Parse(MochaDatabase.Version) < double.Parse("3.4.6") ? "\"[A-z]\"" : "([A-z])")}
RETURN
");
                    sw.Stop();
                    total += sw.ElapsedMilliseconds;
                }
                total = total / 5;
                mhqlTestRB.Text = total + "ms";
                mhqlTestRB.State =
                    total <= 200 ?
                    1 :
                    total <= 1500 ?
                        2 :
                        total <= 3000 ?
                            3 :
                            total <= 5000 ?
                            4 :
                            5;
                mhqlTestButton.Enabled = true;
            }).Start();
        }

        #endregion

        #region mhqlHardTestButton

        private void MhqlHardTestButton_Click(object sender,EventArgs e) {
            new Task(() => {
                mhqlHardTestButton.Enabled = false;
                mhqlHardTestRB.Text = "Testing...";
                mhqlHardTestRB.State = 0;
                var sw = new Stopwatch();
                long total = 0;
                for(short counter = 1; counter <= 5; counter++) {
                    sw.Start();
                    Database.ExecuteScalar(
$@"
@TABLES
@STACKS
@SECTORS
SELECT {(double.Parse(MochaDatabase.Version) < double.Parse("3.4.6") ? "\"[A-z]\"" : "([A-z])")}
                RETURN
");
                    sw.Stop();
                    total += sw.ElapsedMilliseconds;
                }
                total = total / 5;
                mhqlHardTestRB.Text = total + "ms";
                mhqlHardTestRB.State =
                    total <= 200 ?
                    1 :
                    total <= 1500 ?
                        2 :
                        total <= 3000 ?
                            3 :
                            total <= 5000 ?
                            4 :
                            5;
                mhqlHardTestButton.Enabled = true;
            }).Start();
        }

        #endregion

        #region directFetchTestButton

        private void DirectFetchTestButton_Click(object sender,EventArgs e) {
            new Task(() => {
                directFetchTestButton.Enabled = false;
                directFetchTestRB.Text = "Testing...";
                directFetchTestRB.State = 0;
                var sw = new Stopwatch();
                long total = 0;
                for(short counter = 1; counter <= 5; counter++) {
                    sw.Start();
                    Database.GetTables();
                    sw.Stop();
                    total += sw.ElapsedMilliseconds;
                }
                total = total / 5;
                directFetchTestRB.Text = total + "ms";
                directFetchTestRB.State =
                    total <= 200 ?
                    1 :
                    total <= 1500 ?
                        2 :
                        total <= 3000 ?
                            3 :
                            total <= 5000 ?
                            4 :
                            5;
                directFetchTestButton.Enabled = true;
            }).Start();
        }

        #endregion

        #region getMochaScriptButton

        private void GetMochaScriptButton_Click(object sender,EventArgs e) {
            MochaScript_Dialog dialog = new MochaScript_Dialog();
            dialog.codeBox.Text = Database.GetMochaScript();
            dialog.ShowDialog();
        }

        #endregion

        #region Database

        private void Database_Changed(object sender,EventArgs e) {
            reshExplorer = true;
            reshMHQL = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refresh all datas.
        /// </summary>
        public void refreshDatas() {
            refreshDashboard();
            refreshExplorer();
            refreshSettings();
            refreshMHQL();
        }

        /// <summary>
        /// Refresh Dashboard tab.
        /// </summary>
        public void refreshDashboard() {
            nameTB.Text = Database.Name;
            pathTB.Text = Database.Provider.Path;
        }

        /// <summary>
        /// Refresh Explorer tab.
        /// </summary>
        public void refreshExplorer() {
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

            explorerTree.Nodes[0].Nodes.Clear();
            explorerTree.Nodes[1].Nodes.Clear();
            explorerTree.Nodes[2].Nodes.Clear();

            TreeNode
                columnsNode,
                attributesNode,
                cacheNode,
                columnNode,
                attributeNode;

            // 
            // Tables
            // 

            MochaCollectionResult<MochaColumn> columns;
            MochaCollectionResult<IMochaAttribute> attributes;
            var tables = Database.GetTables();
            for(int index = 0; index < tables.Count; index++) {
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
                cacheNode.Text =tables[index].Name;
                cacheNode.Tag="Table";
                cacheNode.ImageIndex = 2;
                cacheNode.SelectedImageIndex=cacheNode.ImageIndex;
                cacheNode.Nodes.Add(attributesNode);
                cacheNode.Nodes.Add(columnsNode);

                attributes = Database.GetTableAttributes(cacheNode.Text);
                for(int attrDex = 0; attrDex < attributes.Count; attrDex++) {
                    attributeNode = new TreeNode();
                    attributeNode.Text = attributes[attrDex].Name;
                    attributeNode.ImageIndex = 5;
                    attributeNode.SelectedImageIndex=attributeNode.ImageIndex;
                    attributeNode.Tag="Attribute";
                    attributesNode.Nodes.Add(attributeNode);
                }

                columns = Database.GetColumns(cacheNode.Text);
                for(int columnIndex = 0; columnIndex < columns.Count; columnIndex++) {
                    columnNode = new TreeNode();
                    columnNode.Text =columns[columnIndex].Name;
                    columnNode.ImageIndex = 4;
                    columnNode.SelectedImageIndex=columnNode.ImageIndex;
                    columnNode.Tag="Column";
                    columnsNode.Nodes.Add(columnNode);
                }

                explorerTree.Nodes[0].Nodes.Add(cacheNode);
            }

            // 
            // Stacks
            // 

            var stacks = Database.GetStacks();
            for(int index = 0; index < stacks.Count; index++) {
                cacheNode = new TreeNode();
                cacheNode.Text =stacks[index].Name;
                cacheNode.Tag="Stack";
                cacheNode.ImageIndex=3;
                cacheNode.SelectedImageIndex=cacheNode.ImageIndex;

                attributesNode = new TreeNode();
                attributesNode.Text ="Attributes";
                attributesNode.Tag="Attributes";
                attributesNode.ImageIndex = 0;
                attributesNode.SelectedImageIndex=attributesNode.ImageIndex;
                cacheNode.Nodes.Add(attributesNode);

                attributes = Database.GetStackAttributes(cacheNode.Text);
                for(int attrDex = 0; attrDex < attributes.Count; attrDex++) {
                    attributeNode = new TreeNode();
                    attributeNode.Text = attributes[attrDex].Name;
                    attributeNode.ImageIndex = 5;
                    attributeNode.SelectedImageIndex=attributeNode.ImageIndex;
                    attributeNode.Tag="Attribute";
                    attributesNode.Nodes.Add(attributeNode);
                }

                var stack = stacks[index];
                if(stacks[index].Items.Count >0)
                    for(int itemIndex = 0; itemIndex < stacks[index].Items.Count; itemIndex++) {
                        var curitem = stack.Items[itemIndex];
                        cacheNode.Nodes.Add(GetMochaStackItemNODE(curitem));
                    }

                explorerTree.Nodes[1].Nodes.Add(cacheNode);
            }

            // 
            // Sectors
            // 

            var sectors = Database.GetSectors();
            for(int index = 0; index < sectors.Count; index++) {
                cacheNode = new TreeNode();
                cacheNode.Text =sectors[index].Name;
                cacheNode.Tag="Sector";
                cacheNode.ImageIndex=4;
                cacheNode.SelectedImageIndex=cacheNode.ImageIndex;

                attributesNode = new TreeNode();
                attributesNode.Text ="Attributes";
                attributesNode.Tag="Attributes";
                attributesNode.ImageIndex = 0;
                attributesNode.SelectedImageIndex=attributesNode.ImageIndex;
                cacheNode.Nodes.Add(attributesNode);

                attributes = Database.GetSectorAttributes(cacheNode.Text);
                for(int attrDex = 0; attrDex < attributes.Count; attrDex++) {
                    attributeNode = new TreeNode();
                    attributeNode.Text = attributes[attrDex].Name;
                    attributeNode.ImageIndex = 5;
                    attributeNode.SelectedImageIndex=attributeNode.ImageIndex;
                    attributeNode.Tag="Attribute";
                    attributesNode.Nodes.Add(attributeNode);
                }

                explorerTree.Nodes[2].Nodes.Add(cacheNode);
            }

            reshExplorer = false;
        }

        /// <summary>
        /// Refresh 
        /// </summary>
        public void refreshMHQL() {
            if(mhqlRefreshTask != null) {
                mhqlRefreshTask.Wait();
                mhqlRefreshTask.Dispose();
            }

            mhqlRefreshTask = new Task(() => {
                mhqlCodeSense.Items = new string[0];

                mhqlCodeSense.AddItem(
                    new Item("SELECT",0,"SELECT","SELECT - Keyword",
                    "Select structures with regex."));
                mhqlCodeSense.AddItem(
                    new Item("USE",0,"USE","USE - Keyword",
                    "Use the x struct(s)."));
                mhqlCodeSense.AddItem(
                    new Item("USE *\nRETURN",2,"USE","USE - Snippet",
                    "Use the x struct(s)."));
                mhqlCodeSense.AddItem(new Item("MUST",0,"MUST","MUST - Keyword",
                    "Define a conditions."));
                mhqlCodeSense.AddItem(new Item("MUST 0() END",2,"MUST","MUST - Snippet",
                    "Define a conditions snippet."));
                mhqlCodeSense.AddItem(new Item("REMOVE",0,"REMOVE","REMOVE - Keyword",
                    "Remove selected with SELECT keyword."));
                mhqlCodeSense.AddItem(new Item("RETURN",0,"RETURN","RETURN - Keyword",
                    "Return result(s)."));
                mhqlCodeSense.AddItem(new Item("ASC",0,"ASC","ASC - Keyword",
                    "Ascending define for ORDERBY keyword."));
                mhqlCodeSense.AddItem(new Item("DESC",0,"DESC","DESC - Keyword",
                    "Descending define for ORDERBY keyword."));
                mhqlCodeSense.AddItem(new Item("ORDERBY",0,"ORDERBY","ORDERBY - Keyword",
                    "Sort items."));
                mhqlCodeSense.AddItem(new Item("AND",0,"AND","AND - Keyword",
                    "Define other conditions for MUST keywords."));
                mhqlCodeSense.AddItem(new Item("END",0,"END","END - Keyword",
                    "Reports that MUST is over."));
                mhqlCodeSense.AddItem(new Item("GROUPBY",0,"GROUPBY","GROUPBY - Keyword",
                    "Group by values."));
                mhqlCodeSense.AddItem(new Item("FROM",0,"FROM","FROM - Keyword",
                    "Define table for USE keyword."));
                mhqlCodeSense.AddItem(new Item("AS",0,"AS","AS - Keyword",
                    "Rename item."));
                mhqlCodeSense.AddItem(new Item("EQUAL",1,"EQUAL","EQUAL - Function",
                    "Returns a specified numerical equal to condition."));
                mhqlCodeSense.AddItem(new Item("ENDW",1,"ENDW","ENDW - Function",
                    "Does it end with...? Returns the condition."));
                mhqlCodeSense.AddItem(new Item("STARTW",1,"STARTW","STARTW - Function",
                    "Does it start with ...? Returns the condition."));
                mhqlCodeSense.AddItem(new Item("BETWEEN",1,"BETWEEN","BETWEEN - Function",
                    "Returns a specified numerical range condition."));
                mhqlCodeSense.AddItem(new Item("LOWER",1,"LOWER","LOWER - Function",
                    "Returns a specified numerical bigger and equal condition."));
                mhqlCodeSense.AddItem(new Item("BIGGER",1,"BIGGER","BIGGER - Function",
                    "Returns a specified numerical lower and equal condition."));
                mhqlCodeSense.AddItem(new Item("USE *\nRETURN",2,"BODY","BODY - Snippet",
                    "USE body snippet."));

                var tables = Database.GetTables();
                for(int index = 0; index < tables.Count; index++) {
                    var table = tables[index];
                    mhqlCodeSense.AddItem(
                        new Item(table.Name,3,table.Name,
                        $"{table.Name} - Table","Table"));

                    for(int cindex = 0; cindex < table.Columns.Count; cindex++) {
                        var column = table.Columns[cindex];
                        mhqlCodeSense.AddItem(
                            new Item($"{table.Name}.{column.Name}",5,$"{table.Name}.{column.Name}",
                            $"{column.Name} - Column",$"Column of {table.Name} table."));
                    }
                }

                var stacks = Database.GetStacks();
                for(int index = 0; index < stacks.Count; index++) {
                    var stack = stacks[index];
                    mhqlCodeSense.AddItem(
                        new Item(stack.Name,4,stack.Name,
                        $"{stack.Name} - Stack","Stack"));
                }

                var sectors = Database.GetSectors();
                for(int index = 0; index < sectors.Count; index++) {
                    var sector = sectors[index];
                    mhqlCodeSense.AddItem(
                        new Item(sector.Name,5,sector.Name,
                        $"{sector.Name} - Sector","Sector"));
                }

                mhqlCodeSense.SortItems();
                mhqlCodeSense.SetCodeSense(mhqlEditor,mhqlCodeSense);
                reshMHQL = false;
            });
            mhqlRefreshTask.Start();
        }

        /// <summary>
        /// Refresh Settings tab.
        /// </summary>
        public void refreshSettings() {
            passwordTB.Text = Database.GetPassword();
            descriptionTB.Text = Database.GetDescription();
        }

        /// <summary>
        /// Return stack item path.
        /// </summary>
        /// <param name="node">StackItem node.</param>
        public string GetStackItemPath(TreeNode node) {
            string cachepath = node.FullPath.Remove(0,node.FullPath.IndexOf("Stacks/")+7);
            return cachepath.Remove(0,cachepath.IndexOf("/")+1);
        }

        /// <summary>
        /// Return stack name of stack item.
        /// </summary>
        /// <param name="node">StackItem node.</param>
        public string GetStackItemStackName(TreeNode node) {
            string cachepath = node.FullPath.Remove(0,node.FullPath.IndexOf("Stacks/")+7);
            return cachepath.Substring(0,cachepath.IndexOf("/"));
        }

        /// <summary>
        /// Execute current MHQL code.
        /// </summary>
        public void ExecuteMHQL() {
            try {
                var command = new MhqlCommand(mhqlEditor.Text);
                if(command.IsExecuteCompatible()) {
                    Database.ExecuteCommand(mhqlEditor.Text);
                    mhqlRPanel.ShowResults(new MochaReader<object>());
                    refreshMHQL();
                } else {
                    MochaReader<object> results;
                    results = Database.ExecuteReader(mhqlEditor.Text);
                    mhqlRPanel.ShowResults(results);
                }
            }
            catch(Exception excep) { errorbox.Show(excep.ToString()); return; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Target database.
        /// </summary>
        public MochaDatabase Database { get; set; }

        #endregion
    }

    // Designer.
    public sealed partial class cncpanel {
        #region Components

        private sbutton
            mhqlTestButton,
            mhqlHardTestButton,
            directFetchTestButton,
            getMochaScriptButton;

        private stabcontrol
            tab;

        private TabPage
            dashboardPage,
            explorerPage,
            terminalPage,
            mhqlPage,
            settingsPage;

        private stextbox
            passwordTB,
            descriptionTB;

        private passwordeye
            passwordTBeye;

        private Label
            nameLabel,
            pathLabel,
            mhqlTestLabel,
            mhqlHardTestLabel,
            directFetchTestLabel;

        private TextBox
            nameTB,
            pathTB;

        private areapanel
            testsPanel;

        private rangebar
            mhqlTestRB,
            mhqlHardTestRB,
            directFetchTestRB;

        private dbtree
            explorerTree;

        private ImageList
            mhqlIL;

        private terminal
            term;

        private editor.editor
            mhqlEditor;

        private codesense.codesense
            mhqlCodeSense;

        private resultPanel
            mhqlRPanel;

        private SplitContainer
            mhqlContainer;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(60,60,60);
            ForeColor = Color.White;

            #endregion

            #region tab

            tab = new stabcontrol();
            tab.Dock = DockStyle.Fill;
            tab.SelectedIndexChanged +=Tab_SelectedIndexChanged;
            Controls.Add(tab);

            #endregion

            // 
            // Dashboard
            // 

            #region dashboardPage

            dashboardPage = new TabPage();
            dashboardPage.Text = "Dashboard";
            dashboardPage.BackColor = BackColor;
            tab.TabPages.Add(dashboardPage);

            #endregion

            #region nameLabel

            nameLabel = new Label();
            nameLabel.AutoSize = true;
            nameLabel.Text = "Name";
            nameLabel.Font = new Font("Arial",10);
            nameLabel.Location = new Point(20,30);
            dashboardPage.Controls.Add(nameLabel);

            #endregion

            #region nameTB

            nameTB = new TextBox();
            nameTB.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            nameTB.ReadOnly = true;
            nameTB.BorderStyle = BorderStyle.FixedSingle;
            nameTB.BackColor = BackColor;
            nameTB.ForeColor = Color.White;
            nameTB.Location = new Point(nameLabel.Location.X,
                nameLabel.Location.Y+nameLabel.Height+5);
            dashboardPage.Controls.Add(nameTB);

            #endregion

            #region pathLabel

            pathLabel = new Label();
            pathLabel.AutoSize = true;
            pathLabel.Text = "Path";
            pathLabel.Font = nameLabel.Font;
            pathLabel.Location = new Point(nameLabel.Location.X,
                nameTB.Location.Y+nameTB.Height+20);
            dashboardPage.Controls.Add(pathLabel);

            #endregion

            #region pathTB

            pathTB = new TextBox();
            pathTB.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            pathTB.ReadOnly = true;
            pathTB.BorderStyle = BorderStyle.FixedSingle;
            pathTB.BackColor = BackColor;
            pathTB.ForeColor = Color.White;
            pathTB.Location = new Point(pathLabel.Location.X,
                pathLabel.Location.Y+pathLabel.Height+5);
            dashboardPage.Controls.Add(pathTB);

            #endregion

            #region testsPanel

            testsPanel = new areapanel("Tests");
            testsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            testsPanel.BackColor = dashboardPage.BackColor;
            testsPanel.Size = new Size(nameTB.Width,300);
            testsPanel.Location = new Point(20,pathTB.Location.Y+pathTB.Height+30);
            dashboardPage.Controls.Add(testsPanel);

            #endregion

            #region mhqlTestLabel

            mhqlTestLabel = new Label();
            mhqlTestLabel.AutoSize = true;
            mhqlTestLabel.Text = "MHQL querying test (5 Test / AVG Time)";
            mhqlTestLabel.Font = new Font("Arial",10);
            mhqlTestLabel.Location = new Point(0,60);
            testsPanel.Controls.Add(mhqlTestLabel);

            #endregion

            #region mhqlTestRB

            mhqlTestRB = new rangebar();
            mhqlTestRB.BackColor = BackColor;
            mhqlTestRB.State = 0;
            mhqlTestRB.Text = "Not tested";
            mhqlTestRB.Location = new Point(mhqlTestLabel.Width + 100,mhqlTestLabel.Location.Y-15);
            testsPanel.Controls.Add(mhqlTestRB);

            #endregion

            #region mhqlTestButton

            mhqlTestButton = new sbutton();
            mhqlTestButton.Text = "Make test";
            mhqlTestButton.Size = new Size(60,25);
            mhqlTestButton.BackColor = Color.Gray;
            mhqlTestButton.MouseEnterColor = Color.DimGray;
            mhqlTestButton.MouseDownColor = Color.DodgerBlue;
            mhqlTestButton.Click+=MhqlTestButton_Click;
            mhqlTestButton.Location = new Point(
                (mhqlTestRB.Location.X + mhqlTestRB.Width) + 30,
                mhqlTestRB.Location.Y + 10);
            testsPanel.Controls.Add(mhqlTestButton);

            #endregion

            #region mhqlHardTestLabel

            mhqlHardTestLabel = new Label();
            mhqlHardTestLabel.AutoSize = true;
            mhqlHardTestLabel.Text = "MHQL hard querying test (5 Test / AVG Time)";
            mhqlHardTestLabel.Font = new Font("Arial",10);
            mhqlHardTestLabel.Location = new Point(0,mhqlTestLabel.Location.Y+mhqlTestLabel.Height + 20);
            testsPanel.Controls.Add(mhqlHardTestLabel);

            #endregion

            #region mhqlHardTestRB

            mhqlHardTestRB = new rangebar();
            mhqlHardTestRB.BackColor = BackColor;
            mhqlHardTestRB.State = 0;
            mhqlHardTestRB.Text = "Not Tested";
            mhqlHardTestRB.Location = new Point(mhqlTestRB.Location.X,mhqlHardTestLabel.Location.Y-15);
            testsPanel.Controls.Add(mhqlHardTestRB);

            #endregion

            #region mhqlHardTestButton

            mhqlHardTestButton = new sbutton();
            mhqlHardTestButton.Text = "Make test";
            mhqlHardTestButton.Size = new Size(60,25);
            mhqlHardTestButton.BackColor = Color.Gray;
            mhqlHardTestButton.MouseEnterColor = Color.DimGray;
            mhqlHardTestButton.MouseDownColor = Color.DodgerBlue;
            mhqlHardTestButton.Click+=MhqlHardTestButton_Click;
            mhqlHardTestButton.Location = new Point(
                (mhqlHardTestRB.Location.X + mhqlHardTestRB.Width) + 30,
                mhqlHardTestRB.Location.Y + 10);
            testsPanel.Controls.Add(mhqlHardTestButton);

            #endregion

            #region directFetchTestLabel

            directFetchTestLabel = new Label();
            directFetchTestLabel.AutoSize = true;
            directFetchTestLabel.Text = "Direct fetch test (5 Test / AVG Time)";
            directFetchTestLabel.Font = new Font("Arial",10);
            directFetchTestLabel.Location = new Point(0,mhqlHardTestLabel.Location.Y+mhqlHardTestLabel.Height + 20);
            testsPanel.Controls.Add(directFetchTestLabel);

            #endregion

            #region directFetchTestRB

            directFetchTestRB = new rangebar();
            directFetchTestRB.BackColor = BackColor;
            directFetchTestRB.State = 0;
            directFetchTestRB.Text = "Not tested";
            directFetchTestRB.Location = new Point(mhqlHardTestRB.Location.X,directFetchTestLabel.Location.Y-15);
            testsPanel.Controls.Add(directFetchTestRB);

            #endregion

            #region directFetchTestButton

            directFetchTestButton = new sbutton();
            directFetchTestButton.Text = "Make test";
            directFetchTestButton.Size = new Size(60,25);
            directFetchTestButton.BackColor = Color.Gray;
            directFetchTestButton.MouseEnterColor = Color.DimGray;
            directFetchTestButton.MouseDownColor = Color.DodgerBlue;
            directFetchTestButton.Click+=DirectFetchTestButton_Click;
            directFetchTestButton.Location = new Point(
                (directFetchTestRB.Location.X + directFetchTestRB.Width) + 30,
                directFetchTestRB.Location.Y + 10);
            testsPanel.Controls.Add(directFetchTestButton);

            #endregion

            // 
            // Explorer
            // 

            #region explorerPage

            explorerPage = new TabPage();
            explorerPage.Text = "Explorer";
            explorerPage.BackColor = BackColor;
            tab.TabPages.Add(explorerPage);

            #endregion

            #region explorerTree

            explorerTree = new dbtree();
            explorerTree.ForeColor = Color.White;
            explorerTree.BackColor = BackColor;
            explorerTree.LabelEdit=true;
            explorerPage.Controls.Add(explorerTree);
            explorerTree.NodeMouseDoubleClick+=ExplorerTree_NodeMouseDoubleClick;
            explorerTree.KeyDown+=ExplorerTree_KeyDown;
            explorerTree.BeforeLabelEdit+=ExplorerTree_BeforeLabelEdit;
            explorerTree.AfterLabelEdit+=ExplorerTree_AfterLabelEdit;

            #endregion

            // 
            // Terminal
            // 

            #region terminalPage

            terminalPage = new TabPage();
            terminalPage.Text = "Terminal";
            terminalPage.BackColor = BackColor;
            tab.TabPages.Add(terminalPage);

            #endregion

            #region term

            term = new terminal();
            term.BannedCommandNamespaces = new[] { "cnc" };
            terminalPage.Controls.Add(term);

            #endregion

            // 
            // MHQL
            // 

            #region mhqlPage

            mhqlPage = new TabPage();
            mhqlPage.Text = "MHQL";
            mhqlPage.BackColor = BackColor;
            tab.TabPages.Add(mhqlPage);

            #endregion

            #region mhqlContainer

            mhqlContainer = new SplitContainer();
            mhqlContainer.Dock = DockStyle.Fill;
            mhqlContainer.BackColor = BackColor;
            mhqlContainer.Orientation = Orientation.Horizontal;
            mhqlContainer.BorderStyle = BorderStyle.FixedSingle;
            mhqlPage.Controls.Add(mhqlContainer);

            #endregion

            #region mhqlEditor

            mhqlEditor = new editor.editor();
            mhqlEditor.Dock = DockStyle.Fill;
            mhqlEditor.Language = Language.Mhql;
            mhqlEditor.BackColor = BackColor;
            mhqlEditor.ForeColor = ForeColor;
            mhqlEditor.LineNumberColor = Color.Khaki;
            mhqlEditor.CurrentLineColor = Color.Black;
            mhqlEditor.ServiceLinesColor = Color.Transparent;
            mhqlEditor.CaretColor = Color.White;
            mhqlEditor.WordWrap = false;
            mhqlEditor.KeyDown+=MhqlEditor_KeyDown;
            mhqlContainer.Panel1.Controls.Add(mhqlEditor);

            #endregion

            #region mhqlIL

            mhqlIL = new ImageList();
            mhqlIL.ColorDepth=ColorDepth.Depth32Bit;
            mhqlIL.Images.Add("Keyword",Resources.Key);
            mhqlIL.Images.Add("Function",Resources.Cube);
            mhqlIL.Images.Add("Snippet",Resources.Brackets);
            mhqlIL.Images.Add("Table",Resources.Table);
            mhqlIL.Images.Add("Stack",Resources.Stack);
            mhqlIL.Images.Add("Sector",Resources.Sector);

            #endregion

            #region mhqlCodeSense

            mhqlCodeSense = new codesense.codesense();
            mhqlCodeSense.ImageList = mhqlIL;
            mhqlCodeSense.AllowsTabKey = true;
            mhqlCodeSense.Colors.BackColor = Color.FromArgb(50,50,50);
            mhqlCodeSense.Colors.SelectedBackColor = Color.Black;
            mhqlCodeSense.Colors.SelectedBackColor2 = Color.Black;
            mhqlCodeSense.Colors.HighlightingColor = Color.DodgerBlue;
            mhqlCodeSense.Colors.ForeColor = Color.White;
            mhqlCodeSense.Colors.SelectedForeColor = Color.Khaki;
            mhqlCodeSense.AppearInterval = 100;
            mhqlCodeSense.Font = new Font("Consolas",12,FontStyle.Regular,GraphicsUnit.Pixel);

            #endregion

            #region mhqlRPanel

            mhqlRPanel = new resultPanel(Database);
            mhqlContainer.Panel2.Controls.Add(mhqlRPanel);

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

            #region passwordTB

            passwordTB = new stextbox();
            passwordTB.Placeholder = "Database password";
            passwordTB.BorderColor = Color.LightGray;
            passwordTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            passwordTB.BackColor = BackColor;
            passwordTB.ForeColor = Color.White;
            passwordTB.Location = new Point(40,30);
            passwordTB.Size = new Size(Width - (passwordTB.Location.X * 2)-40,20);
            passwordTB.PasswordChar = '●';
            passwordTB.TextChanged +=PasswordTB_TextChanged;
            settingsPage.Controls.Add(passwordTB);

            #endregion

            #region passwordTBeye

            passwordTBeye = new passwordeye(passwordTB);
            passwordTBeye.Size = new Size(30,passwordTB.Height);
            passwordTBeye.Location = new Point(
                passwordTB.Location.X+passwordTB.Width + 5,passwordTB.Location.Y);
            settingsPage.Controls.Add(passwordTBeye);

            #endregion

            #region descriptionTB

            descriptionTB = new stextbox();
            descriptionTB.Placeholder = "Database description";
            descriptionTB.BorderColor = Color.LightGray;
            descriptionTB.Multiline = true;
            descriptionTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            descriptionTB.BackColor = BackColor;
            descriptionTB.ForeColor = Color.White;
            descriptionTB.Location = new Point(40,passwordTB.Location.Y + passwordTB.Height + 30);
            descriptionTB.Size = new Size(passwordTB.Width,20);
            descriptionTB.InputSize = new Size(descriptionTB.Width,200);
            descriptionTB.TextChanged +=DescriptionTB_TextChanged;
            settingsPage.Controls.Add(descriptionTB);

            #endregion

            #region getMochaScriptButton

            getMochaScriptButton = new sbutton();
            getMochaScriptButton.Text = "Get MochaScript";
            getMochaScriptButton.Size = new Size(100,25);
            getMochaScriptButton.BackColor = Color.Gray;
            getMochaScriptButton.MouseEnterColor = Color.DimGray;
            getMochaScriptButton.MouseDownColor = Color.DodgerBlue;
            getMochaScriptButton.Location = new Point(descriptionTB.Location.X,
                descriptionTB.Location.Y+descriptionTB.Height+20);
            getMochaScriptButton.Click+=GetMochaScriptButton_Click;
            settingsPage.Controls.Add(getMochaScriptButton);

            #endregion
        }
    }
}
