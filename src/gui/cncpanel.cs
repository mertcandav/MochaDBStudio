using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MochaDB;
using MochaDB.FileSystem;
using MochaDB.Logging;
using MochaDB.Querying;
using MochaDBStudio.dialogs;
using MochaDBStudio.Properties;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Connection Panel.
    /// </summary>
    public sealed partial class cncpanel:Panel {
        #region Fields

        private Task
            mhqlTestTask,
            directFetchTestTask;

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

            var term = new terminal();
            term.DB=database;
            term.BannedCommandNamespaces = new[] { "cnc" };
            terminalPage.Controls.Add(term);

            refreshDashboard();
            refreshExplorer();
        }

        #endregion

        #region tab

        private void Tab_SelectedIndexChanged(object sender,EventArgs e) {
            if(tab.SelectedIndex == 0) /* Dashboard */ {
                refreshDashboard();
            } else if(tab.SelectedIndex == 3) /* Settings */ {
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

        private void ExplorerTree_AfterExpand(object sender,TreeViewEventArgs e) {
            if(e.Node.ImageIndex == 0) {
                e.Node.ImageIndex = 1;
                e.Node.SelectedImageIndex = 1;
            }
        }

        private void ExplorerTree_AfterCollapse(object sender,TreeViewEventArgs e) {
            if(e.Node.ImageIndex == 1) {
                e.Node.ImageIndex = 0;
                e.Node.SelectedImageIndex = 0;
            }
        }

        private void ExplorerTree_BeforeLabelEdit(object sender,NodeLabelEditEventArgs e) {
            string tag = e.Node.Tag as string;
            e.CancelEdit =
                tag=="Tables" ? true :
                tag=="Columns" ? true :
                tag=="Sectors" ? true :
                tag=="Stacks" ? true : false;
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
                   explorerTree.SelectedNode.Tag!="StackItem")
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
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refresh all datas.
        /// </summary>
        public void refreshDatas() {

        }

        /// <summary>
        /// Refresh "Dashboard" tab.
        /// </summary>
        public void refreshDashboard() {
            nameTB.Text = Database.Name;
            pathTB.Text = Database.Provider.Path;

            // 
            // Tests
            // 
            if(mhqlTestTask != null)
                mhqlTestTask.Wait();

            mhqlTestTask = new Task(() => {
                mhqlTestRB.State = 0;
                mhqlTestRB.Text = "Testing...";
                var sw = new Stopwatch();
                long total = 0;
                for(short counter = 1; counter <= 5; counter++) {
                    sw.Start();
                    Database.ExecuteScalar(
@"
@STACKS
@SECTORS
@TABLES
SELECT ""[A-z]""
RETURN
");
                    sw.Stop();
                    total += sw.ElapsedMilliseconds;
                }
                total = total / 5;
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
                mhqlTestRB.Text = total + "ms";
            });
            mhqlTestTask.Start();


            if(directFetchTestTask != null)
                directFetchTestTask.Wait();

            directFetchTestTask = new Task(() => {
                directFetchTestRB.State = 0;
                directFetchTestRB.Text = "Testing...";
                var sw = new Stopwatch();
                long total = 0;
                for(short counter = 1; counter <= 5; counter++) {
                    sw.Start();
                    Database.GetTables();
                    sw.Stop();
                    total += sw.ElapsedMilliseconds;
                }
                total = total / 5;
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
                directFetchTestRB.Text = total + "ms";
            });
            directFetchTestTask.Start();
        }

        /// <summary>
        /// Refresh "Explorer" tab.
        /// </summary>
        public void refreshExplorer() {
            TreeNode GetMochaStackItemNODE(MochaStackItem item) {
                TreeNode node = new TreeNode(item.Name);
                node.Tag="StackItem";

                if(item.Items.Count>0)
                    for(int index = 0; index < item.Items.Count; index++) {
                        node.Nodes.Add(GetMochaStackItemNODE(item.Items[index]));
                    }

                return node;
            }

            explorerTree.Nodes[0].Nodes.Clear();
            explorerTree.Nodes[1].Nodes.Clear();
            explorerTree.Nodes[2].Nodes.Clear();

            TreeNode
                columnsNode,
                cacheNode,
                columnNode;

            // 
            // Tables
            // 

            MochaCollectionResult<MochaColumn> columns;
            var tables = Database.GetTables();
            for(int index = 0; index < tables.Count; index++) {
                columnsNode = new TreeNode();
                columnsNode.Text ="Columns";
                columnsNode.Tag="Columns";
                columnsNode.ImageIndex =0;
                columnsNode.SelectedImageIndex=tablesNode.ImageIndex;

                cacheNode = new TreeNode();
                cacheNode.Text =tables[index].Name;
                cacheNode.Tag="Table";
                cacheNode.ImageIndex =2;
                cacheNode.SelectedImageIndex=cacheNode.ImageIndex;
                cacheNode.Nodes.Add(columnsNode);

                columns = Database.GetColumns(cacheNode.Text);
                for(int columnIndex = 0; columnIndex < columns.Count; columnIndex++) {
                    columnNode = new TreeNode();
                    columnNode.Text =columns[columnIndex].Name;
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

                if(stacks[index].Items.Count >0)
                    for(int itemIndex = 0; itemIndex < stacks[index].Items.Count; itemIndex++)
                        cacheNode.Nodes.Add(GetMochaStackItemNODE(stacks[index].Items[itemIndex]));

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
                explorerTree.Nodes[2].Nodes.Add(cacheNode);
            }
        }

        /// <summary>
        /// Refresh "Settings" tab.
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

        private stabcontrol
            tab;

        private TabPage
            dashboardPage,
            explorerPage,
            terminalPage,
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
            directFetchTestLabel;

        private TextBox
            nameTB,
            pathTB;

        private areapanel
            testsPanel;

        private rangebar
            mhqlTestRB,
            directFetchTestRB;

        private TreeView
            explorerTree;

        private ImageList
            explorerTreeIL;

        private TreeNode
            tablesNode,
            stacksNode,
            sectorsNode;

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
            mhqlTestRB.Text = "Testing...";
            mhqlTestRB.Location = new Point(mhqlTestLabel.Width + 100,mhqlTestLabel.Location.Y-15);
            testsPanel.Controls.Add(mhqlTestRB);

            #endregion

            #region directFetchTestLabel

            directFetchTestLabel = new Label();
            directFetchTestLabel.AutoSize = true;
            directFetchTestLabel.Text = "Direct fetch test (5 Test / AVG Time)";
            directFetchTestLabel.Font = new Font("Arial",10);
            directFetchTestLabel.Location = new Point(0,mhqlTestLabel.Location.Y+mhqlTestLabel.Height + 20);
            testsPanel.Controls.Add(directFetchTestLabel);

            #endregion

            #region directFetchTestRB

            directFetchTestRB = new rangebar();
            directFetchTestRB.BackColor = BackColor;
            directFetchTestRB.State = 0;
            directFetchTestRB.Text = "Testing...";
            directFetchTestRB.Location = new Point(mhqlTestRB.Location.X,directFetchTestLabel.Location.Y-15);
            testsPanel.Controls.Add(directFetchTestRB);

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

            #region explorerTreeIL

            explorerTreeIL = new ImageList();
            explorerTreeIL.ColorDepth=ColorDepth.Depth32Bit;
            explorerTreeIL.Images.Add("FolderClose",Resources.FolderClose);
            explorerTreeIL.Images.Add("FolderOpen",Resources.FolderOpen);
            explorerTreeIL.Images.Add("Table",Resources.Table);
            explorerTreeIL.Images.Add("Stack",Resources.Stack);
            explorerTreeIL.Images.Add("Sector",Resources.Sector);

            #endregion

            #region explorerTree

            explorerTree = new TreeView();
            explorerTree.ForeColor = Color.White;
            explorerTree.BackColor = BackColor;
            explorerTree.Dock = DockStyle.Fill;
            explorerTree.BorderStyle = BorderStyle.None;
            explorerTree.ImageList = explorerTreeIL;
            explorerTree.LabelEdit=true;
            explorerTree.PathSeparator="/";
            explorerPage.Controls.Add(explorerTree);
            explorerTree.AfterExpand +=ExplorerTree_AfterExpand;
            explorerTree.AfterCollapse+=ExplorerTree_AfterCollapse;
            explorerTree.NodeMouseDoubleClick+=ExplorerTree_NodeMouseDoubleClick;
            explorerTree.KeyDown+=ExplorerTree_KeyDown;
            explorerTree.BeforeLabelEdit+=ExplorerTree_BeforeLabelEdit;
            explorerTree.AfterLabelEdit+=ExplorerTree_AfterLabelEdit;

            #endregion

            #region tableNode

            tablesNode = new TreeNode();
            tablesNode.Text="Tables";
            tablesNode.Tag="Tables";
            tablesNode.ImageIndex = 0;
            tablesNode.SelectedImageIndex = 0;
            tablesNode.Nodes.Add("test");
            tablesNode.Nodes.Add("test");

            explorerTree.Nodes.Add(tablesNode);

            #endregion

            #region stacksNode

            stacksNode = new TreeNode();
            stacksNode.Text="Stacks";
            stacksNode.Tag="Stacks";
            stacksNode.ImageIndex = 0;
            stacksNode.SelectedImageIndex = 0;

            explorerTree.Nodes.Add(stacksNode);

            #endregion

            #region sectorsNode

            sectorsNode = new TreeNode();
            sectorsNode.Text="Sectors";
            sectorsNode.Tag="Sectors";
            stacksNode.ImageIndex = 0;
            stacksNode.SelectedImageIndex = 0;

            explorerTree.Nodes.Add(sectorsNode);

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
        }
    }
}
