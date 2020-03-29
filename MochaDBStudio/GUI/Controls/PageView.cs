﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MochaDB;
using MochaDB.FileSystem;
using MochaDB.Logging;
using MochaDB.Mhql;
using MochaDB.MochaScript;
using MochaDB.Querying;
using MochaDBStudio.Engine;
using MochaDBStudio.Editor;
using MochaDBStudio.GUI.Components;
using MochaDBStudio.Properties;

namespace MochaDBStudio.GUI.Controls {
    /// <summary>
    /// PageView for multi window.
    /// </summary>
    public sealed class PageView:Control {
        #region Fields

        private List<Page> tabPages;
        private Page selectedTab;
        private int selectedIndex;
        private bool closeable;
        private Color entryColor;
        private bool hideExplorerButton;
        private int itemHeight;

        private FlatToolTip tip;
        private FlatButton stripButton;
        private ContextMenuStrip itemStrip;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new PageView.
        /// </summary>
        public PageView() {
            //Drawing optimization.
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserMouse,true);

            tip = new FlatToolTip();
            itemHeight=30;
            entryColor=Color.LightGray;
            hideExplorerButton=false;
            ForeColor = Color.Black;
            tabPages = new List<Page>();
            selectedTab = null;
            selectedIndex = -1;
            closeable = true;

            #region stripButton

            stripButton = new FlatButton();
            stripButton.Text = "V";
            stripButton.Font=new Font("Consolas",10,FontStyle.Bold);
            stripButton.Size = new Size(20,ItemHeight -2);
            stripButton.Location = new Point(ClientSize.Width-stripButton.Width,0);
            stripButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            stripButton.Click+=StripButton_Click;

            Controls.Add(stripButton);

            #endregion

            #region itemStrip

            itemStrip = new ContextMenuStrip();
            itemStrip.ItemClicked+=İtemStrip_ItemClicked;

            #endregion
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            //Background.
            using(SolidBrush BackBrush = new SolidBrush(BackColor)) {
                e.Graphics.FillRectangle(BackBrush,ClientRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if(!hideExplorerButton) {
                if(TabCount==0) {
                    stripButton.Hide();
                    return;
                }

                stripButton.Show();
            }

            if(DrawIndex >TabCount-1)
                DrawIndex=0;

            int intvalue;
            Rectangle rect = GetRect(selectedIndex);
            intvalue=rect.X+rect.Width;
            DrawIndex = intvalue > ClientSize.Width ? selectedIndex : DrawIndex;
            Rectangle drect = GetRect(DrawIndex);

            intvalue = drect.X+drect.Width;
            if(selectedIndex < DrawIndex && intvalue <= ClientSize.Width) {
                DrawIndex = 0;
            }

            //Items.
            using(SolidBrush BackBrush = new SolidBrush(entryColor)) {
                for(intvalue = DrawIndex; intvalue < TabCount; intvalue++) {
                    rect = GetRect(intvalue);

                    //Background.
                    if(intvalue == SelectedIndex)
                        e.Graphics.FillRectangle(BackBrush,rect);

                    if(Closeable) {
                        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                        e.Graphics.FillPie(Brushes.Red,GetClosePieRect(rect),0,360);
                        e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
                    }

                    //Image.
                    if(this[intvalue].Image != null)
                        e.Graphics.DrawImage(this[intvalue].Image,rect.X + 2,7,16,rect.Height - 14);

                    //Text.
                    using(StringFormat SFormat = new StringFormat(StringFormatFlags.NoWrap) {
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Center
                    })
                    using(SolidBrush ForeBrush = new SolidBrush(ForeColor))
                        e.Graphics.DrawString(TabPages[intvalue].Text,Font,ForeBrush,rect,SFormat);
                }

                if(TabCount >0)
                    e.Graphics.FillRectangle(BackBrush,0,ItemHeight - 2,Width,2);
            }
        }

        #endregion

        #region stripButton

        private void StripButton_Click(object sender,EventArgs e) {
            itemStrip.Show(this,stripButton.Location.X,stripButton.Height);
        }

        #endregion

        #region itemStrip

        private void İtemStrip_ItemClicked(object sender,ToolStripItemClickedEventArgs e) {
            SelectedTab = e.ClickedItem.Tag as Page;
        }

        #endregion

        #region Mouse

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            Page page = GetHoveringPage();
            if(page != null) {
                if(Closeable) {
                    Rectangle closeRect = GetClosePieRect(GetRect(IndexOf(page)));
                    if(closeRect.Contains(e.Location)) {
                        Remove(page);
                        return;
                    }
                }

                SelectedTab = page;
            }
        }

        protected override void OnMouseHover(EventArgs e) {
            tip.Hide(this);
            Page page = GetHoveringPage();
            if(page != null) {
                Rectangle pageRect = GetHoveringRect();
                tip.Show(page.Tip,this,pageRect.X,pageRect.Y+pageRect.Height);
            }

            base.OnMouseHover(e);
        }

        protected override void OnMouseLeave(EventArgs e) {
            tip.Hide(this);
            base.OnMouseLeave(e);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add page.
        /// </summary>
        /// <param name="page">Page object to add.</param>
        public void Add(Page page) {
            page.Size = new Size(Width,Height - ItemHeight);
            page.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            page.Location = new Point(0,ItemHeight);
            tabPages.Add(page);
            Controls.Add(page);
            SelectedTab = page;

            ToolStripMenuItem item = new ToolStripMenuItem();
            item.ToolTipText=page.Tip;
            item.Name=page.Name;
            item.Text=page.Text;
            item.Image=page.Image;
            item.Tag=page;
            itemStrip.Items.Add(item);

            Invalidate();
        }

        /// <summary>
        /// Remove page.
        /// </summary>
        /// <param name="index">Index of page to remove.</param>
        public void Remove(int index) {
            Page page = tabPages[index];
            itemStrip.Items.RemoveAt(index);
            Controls.Remove(page);
            tabPages.RemoveAt(index);
            page.Dispose();

            if(SelectedIndex == index) {
                if(TabCount == 1)
                    SelectedIndex = 0;
                else if(TabCount == 0)
                    SelectedIndex = -1;
                else if(SelectedIndex == TabCount && TabCount > 1)
                    SelectedIndex--;
                else if(SelectedIndex == TabCount && TabCount > 1)
                    SelectedIndex--;
                else if(SelectedIndex == 0 && TabCount > 0)
                    SelectedIndex++;
                else if(SelectedIndex > 0 && SelectedIndex < TabCount)
                    SelectedIndex--;
            } else {
                if(SelectedIndex > index)
                    SelectedIndex--;
            }

            Invalidate();
        }

        /// <summary>
        /// Remove page.
        /// </summary>
        /// <param name="page">Page object to remove.</param>
        public void Remove(Page page) {
            int Index = IndexOf(page);
            Remove(Index);
        }

        /// <summary>
        /// Return index if find is successfully but return -1 if find is not successfully.
        /// </summary>
        /// <param name="page">Page object to find index.</param>
        public int IndexOf(Page page) =>
            tabPages.IndexOf(page);

        /// <summary>
        /// Return index if find is successfully but return -1 if find is not successfully.
        /// </summary>
        /// <param name="key">Name of page.</param>
        public int IndexOf(string key) {
            for(int index = 0; index < TabCount; index++) {
                if(this[index].Name == key)
                    return index;
            }
            return -1;
        }

        /// <summary>
        /// Return page rectangle.
        /// </summary>
        /// <param name="index">Index of page.</param>
        public Rectangle GetRect(int index) {
            int x = 0;

            for(int pageIndex = DrawIndex; pageIndex < index; pageIndex++) {
                x += TextRenderer.MeasureText(TabPages[pageIndex].Text,Font).Width + 35;
            }

            Rectangle rect = new Rectangle(x,0,
                TextRenderer.MeasureText(TabPages[index].Text,Font).Width + 35,ItemHeight);
            return rect;
        }

        /// <summary>
        /// Return close pie rectangle from rectangle.
        /// </summary>
        /// <param name="Rect">Base rectangle.</param>
        public Rectangle GetClosePieRect(Rectangle Rect) =>
            new Rectangle(Rect.X + Rect.Width - 13,Rect.Height / 2 - 5,8,8);

        /// <summary>
        /// Return hovering page.
        /// </summary>
        public Page GetHoveringPage() {
            for(int index = DrawIndex; index < TabCount; index++)
                if(GetRect(index).Contains(PointToClient(Cursor.Position)))
                    return this[index];
            return null;
        }

        /// <summary>
        /// Return rectangle of hovering page.
        /// </summary>
        public Rectangle GetHoveringRect() {
            for(int index = DrawIndex; index < TabCount; index++) {
                Rectangle rect = GetRect(index);
                if(rect.Contains(PointToClient(Cursor.Position)))
                    return rect;
            }
            return new Rectangle();
        }

        /// <summary>
        /// Set explorer button visible.
        /// </summary>
        /// <param name="state">Visible state to set.</param>
        public void SetExplorerButtonVisible(bool state) {
            if(state)
                stripButton.Show();
            else
                stripButton.Hide();

            hideExplorerButton=!state;
        }

        #endregion

        #region Properties

        /// <summary>
        /// All pages.
        /// </summary>
        public IList<Page> TabPages =>
            tabPages;

        /// <summary>
        /// Count of page.
        /// </summary>
        public int TabCount =>
            tabPages.Count;

        /// <summary>
        /// Index of start page title drawing.
        /// </summary>
        public int DrawIndex { get; private set; }

        /// <summary>
        /// Return page by index.
        /// </summary>
        /// <param name="Index">Index.</param>
        public Page this[int Index] =>
            tabPages[Index];

        /// <summary>
        /// Current page.
        /// </summary>
        public Page SelectedTab {
            get => selectedTab;
            set {
                if(value == selectedTab)
                    return;

                selectedTab = value;
                if(SelectedTab != null) {
                    SelectedIndex = IndexOf(value);
                    selectedTab.BringToFront();
                } else {
                    SelectedIndex = -1;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Index of current page.
        /// </summary>
        public int SelectedIndex {
            get => selectedIndex;
            set {
                if(value == selectedIndex)
                    return;

                selectedIndex = value;
                if(selectedIndex != -1)
                    SelectedTab = tabPages[selectedIndex];
                else
                    SelectedTab = null;
            }
        }

        /// <summary>
        /// Page title height.
        /// </summary>
        public int ItemHeight {
            get =>
                itemHeight;
            set {
                if(value==itemHeight)
                    return;

                itemHeight=value;
                stripButton.Height=value-2;
            }
        }

        /// <summary>
        /// Draw and use close pies.
        /// </summary>
        public bool Closeable {
            get =>
                closeable;
            set {
                if(value == closeable)
                    return;

                closeable = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Active color.
        /// </summary>
        public Color EntryColor {
            get =>
                entryColor;
            set {
                if(value == entryColor)
                    return;

                entryColor = value;
                Invalidate();
            }
        }

        #endregion
    }

    /// <summary>
    /// Page for PageView.
    /// </summary>
    public class Page:Panel {
        #region Constructors

        /// <summary>
        /// Create new Page.
        /// </summary>
        public Page() {
            //Drawing optimization.
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer,true);

            Image = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Text.
        /// </summary>
        public override string Text {
            get => base.Text;
            set {
                base.Text = value;

                if(Parent != null)
                    Parent.Invalidate();
            }
        }

        /// <summary>
        /// Tip.
        /// </summary>
        public string Tip { get; set; }

        /// <summary>
        /// Page image.
        /// </summary>
        public virtual Bitmap Image { get; set; }

        #endregion
    }

    /// <summary>
    /// Page for database pages.
    /// </summary>
    public sealed class ConnectionPage:Page {
        #region Fields

        private PageView tab;
        private TreeView explorerTree;
        private ImageList imageList;

        private TreeNode
            sectorsNode,
            stacksNode,
            tablesNode,
            fileSystemNode,
            logsNode,
            terminalNode;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new ConnectionPage.
        /// </summary>
        /// <param name="db">MochaDatabase.</param>
        public ConnectionPage(MochaDatabase db) {
            DB = db;
            DB.Changed+=DB_ChangeContent;
            Tag=DB;
            Text=DB.Name;
            Image = Resources.Database;

            #region imageList

            imageList = new ImageList();
            imageList.ColorDepth=ColorDepth.Depth32Bit;
            imageList.Images.Add("Folder",Resources.Folder);
            imageList.Images.Add("Table",Resources.Table);
            imageList.Images.Add("Terminal",Resources.Terminal);
            imageList.Images.Add("Dot",Resources.Dot);
            imageList.Images.Add("Sector",Resources.Sector);
            imageList.Images.Add("Disk",Resources.Disk);

            #endregion

            #region explorerTree

            explorerTree = new TreeView();
            explorerTree.Dock = DockStyle.Left;
            explorerTree.Width=170;
            explorerTree.BorderStyle = BorderStyle.None;
            explorerTree.ImageList =imageList;
            explorerTree.BackColor = Color.WhiteSmoke;
            explorerTree.LabelEdit=true;
            explorerTree.PathSeparator="/";
            explorerTree.NodeMouseDoubleClick+=ExplorerTree_NodeMouseDoubleClick;
            explorerTree.KeyDown+=ExplorerTree_KeyDown;
            explorerTree.BeforeLabelEdit+=ExplorerTree_BeforeLabelEdit;
            explorerTree.AfterLabelEdit+=ExplorerTree_AfterLabelEdit;

            #endregion

            #region sectorsNode

            sectorsNode = new TreeNode();
            sectorsNode.Text="Sectors";
            sectorsNode.Tag="Sectors";
            sectorsNode.ImageIndex =0;
            sectorsNode.SelectedImageIndex=sectorsNode.ImageIndex;

            explorerTree.Nodes.Add(sectorsNode);

            #endregion

            #region stacksNode

            stacksNode = new TreeNode();
            stacksNode.Text="Stacks";
            stacksNode.Tag="Stacks";
            stacksNode.ImageIndex =0;
            stacksNode.SelectedImageIndex=stacksNode.ImageIndex;

            explorerTree.Nodes.Add(stacksNode);

            #endregion

            #region tablesNode

            tablesNode = new TreeNode();
            tablesNode.Text="Tables";
            tablesNode.Tag="Tables";
            tablesNode.ImageIndex =0;
            tablesNode.SelectedImageIndex=tablesNode.ImageIndex;

            explorerTree.Nodes.Add(tablesNode);

            #endregion

            #region fileSystemNode

            fileSystemNode = new TreeNode();
            fileSystemNode.Text="FileSystem";
            fileSystemNode.Tag="FileSystem";
            fileSystemNode.ImageIndex =0;
            fileSystemNode.SelectedImageIndex=fileSystemNode.ImageIndex;

            explorerTree.Nodes.Add(fileSystemNode);

            #endregion

            #region logsNode

            logsNode = new TreeNode();
            logsNode.Text="Logs";
            logsNode.Tag="Logs";
            logsNode.ImageIndex =0;
            logsNode.SelectedImageIndex=logsNode.ImageIndex;

            explorerTree.Nodes.Add(logsNode);

            #endregion

            #region terminalNode

            terminalNode = new TreeNode();
            terminalNode.Text="Terminal";
            terminalNode.Tag="Terminal";
            terminalNode.ImageIndex =2;
            terminalNode.SelectedImageIndex=terminalNode.ImageIndex;

            explorerTree.Nodes.Add(terminalNode);

            #endregion

            #region tab

            tab = new PageView();
            tab.EntryColor = Color.Gray;
            tab.BackColor = Color.White;
            tab.ItemHeight = 26;
            tab.Location = new Point(explorerTree.Width,0);
            tab.Size = new Size(ClientSize.Width - tab.Location.X,ClientSize.Height);
            tab.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            Controls.Add(tab);

            #endregion

            Controls.Add(explorerTree);
            RefreshExplorer();
        }

        #endregion

        #region DB

        private void DB_ChangeContent(object sender,EventArgs e) {
            RefreshExplorer();
        }

        #endregion

        #region Terminal Events

        private void Terminal_InputProcessing(object sender,TerminalInputProcessEventArgs e) {
            if(e.Input == "disconnect") {
                PageView parent = Parent as PageView;
                parent.Remove(this);
                e.Cancel=true;
            }
        }

        #endregion

        #region explorerTree

        private void ExplorerTree_NodeMouseDoubleClick(object sender,TreeNodeMouseClickEventArgs e) {
            if(e.Node.Level == 0 && e.Node.Text =="Terminal") {
                if(ControlAndSelectPage("Terminal"))
                    return;

                Terminal terminal = new Terminal();
                terminal.Name="Terminal";
                terminal.DB=DB;
                terminal.BannedCommandNamespaces = new[] { "cnc" };
                terminal.InputProcessing+=Terminal_InputProcessing;
                tab.Add(terminal);
            } else if(e.Node.Tag=="Disk") {
                if(ControlAndSelectPage($"FileSystem_{e.Node.Text}"))
                    return;

                FileSystemPage fspage = new FileSystemPage();
                fspage.Tag=$"FileSystem_{e.Node.Text}";
                fspage.FileSystem=DB.FileSystem;
                fspage.Path=e.Node.Text;
                tab.Add(fspage);
            } else if(e.Node.Tag == "Table") {
                if(ControlAndSelectPage($"Table_{e.Node.Text}"))
                    return;

                Page page = new Page();
                page.Name="Table_" + e.Node.Text;
                page.Text=e.Node.Text;
                page.Image=Resources.Table;
                FlatGrid grid = new FlatGrid();
                grid.Dock = DockStyle.Fill;
                var table = DB.ExecuteScalar($@"
                    USE {e.Node.Text}
                    RETURN") as MochaTableResult;
                for(int columnIndex = 0; columnIndex < table.Columns.Length; columnIndex++)
                    grid.Columns.Add(table.Columns[columnIndex].Name,table.Columns[columnIndex].Name);
                for(int rowIndex = 0; rowIndex<table.Rows.Length; rowIndex++) {
                    string[] datas = new string[table.Columns.Length];
                    for(int dataIndex = 0; dataIndex < table.Rows[rowIndex].Datas.Count; dataIndex++)
                        datas[dataIndex] = table.Rows[rowIndex].Datas[dataIndex].ToString();
                    grid.Rows.Add(datas);
                }
                page.Controls.Add(grid);
                tab.Add(page);
            } else if(e.Node.Tag == "Sectors") {
                if(ControlAndSelectPage("Sectors"))
                    return;

                Page page = new Page();
                page.Name="Sectors";
                page.Text ="Sectors";
                page.Image=Resources.Sector;
                FlatGrid grid = new FlatGrid();
                grid.Dock = DockStyle.Fill;
                grid.Columns.Add("Name","Name");
                grid.Columns.Add("Description","Description");
                grid.Columns.Add("Data","Data");
                var sectors = DB.GetSectors();
                for(int sectorIndex = 0; sectorIndex < sectors.Count; sectorIndex++) {
                    var sector = sectors[sectorIndex];
                    grid.Rows.Add(sector.Name,sector.Description,sector.Data);
                }
                page.Controls.Add(grid);
                tab.Add(page);
            } else if(e.Node.Tag == "Log") {
                if(MessageBox.Show(
                    $"It will be restored to the log with the ID '{e.Node.Text}'.\n                                       Are you sure?",
                    "MochaDB Studio",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.Yes) {
                    DB.RestoreToLog(e.Node.Text);
                }
            }
        }

        private void ExplorerTree_KeyDown(object sender,KeyEventArgs e) {
            if(e.KeyCode == Keys.Delete) {
                if(explorerTree.SelectedNode.Tag=="Table")
                    DB.RemoveTable(explorerTree.SelectedNode.Text);
                else if(explorerTree.SelectedNode.Tag=="Column")
                    DB.RemoveColumn(explorerTree.SelectedNode.Parent.Parent.Text,explorerTree.SelectedNode.Text);
                else if(explorerTree.SelectedNode.Tag=="Sector")
                    DB.RemoveSector(explorerTree.SelectedNode.Text);
                else if(explorerTree.SelectedNode.Tag=="Stack")
                    DB.RemoveStack(explorerTree.SelectedNode.Text);
                else if(explorerTree.SelectedNode.Tag=="Disk")
                    DB.FileSystem.RemoveDisk(explorerTree.SelectedNode.Text);
                else if(explorerTree.SelectedNode.Tag=="StackItem")
                    DB.RemoveStackItem(GetStackItemStackName(explorerTree.SelectedNode),
                        GetStackItemPath(explorerTree.SelectedNode));
                else if(explorerTree.SelectedNode.Tag=="Logs")
                    DB.ClearLogs();
            } else if(e.KeyCode==Keys.F2) {
                if(explorerTree.SelectedNode!=null)
                    explorerTree.SelectedNode.BeginEdit();
            }
        }

        private void ExplorerTree_BeforeLabelEdit(object sender,NodeLabelEditEventArgs e) {
            string tag = e.Node.Tag as string;
            e.CancelEdit =
                tag=="Tables" ? true :
                tag=="Columns" ? true :
                tag=="Sectors" ? true :
                tag=="Stacks" ? true :
                tag=="FileSystem" ? true :
                tag=="Disk" ? true :
                tag=="Terminal" ? true :
                tag=="Logs" ? true :
                tag=="Log" ? true : false;
        }

        private void ExplorerTree_AfterLabelEdit(object sender,NodeLabelEditEventArgs e) {
            try {
                if(explorerTree.SelectedNode.Tag=="Table") {
                    DB.RenameTable(e.Node.Text,e.Label);
                } else if(explorerTree.SelectedNode.Tag=="Column") {
                    DB.RenameColumn(e.Node.Parent.Parent.Text,e.Node.Text,e.Label);
                } else if(explorerTree.SelectedNode.Tag=="Sector") {
                    DB.RenameSector(e.Node.Text,e.Label);
                } else if(explorerTree.SelectedNode.Tag=="Stack") {
                    DB.RenameStack(e.Node.Text,e.Label);
                } else if(explorerTree.SelectedNode.Tag=="StackItem") {
                    DB.RenameStackItem(GetStackItemStackName(explorerTree.SelectedNode),e.Label,
                        GetStackItemPath(explorerTree.SelectedNode));
                }
            } catch(Exception excep) {
                e.CancelEdit=true;
                MessageBox.Show(excep.Message,"MochaDB Studio",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Methods

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
        /// Return TreeNode from MochaStackItem.
        /// </summary>
        /// <param name="item">MochaStackItem object.</param>
        public TreeNode GetMochaStackItemNODE(MochaStackItem item) {
            TreeNode node = new TreeNode(item.Name);
            node.Tag="StackItem";

            if(item.Items.Count>0)
                for(int index = 0; index < item.Items.Count; index++) {
                    node.Nodes.Add(GetMochaStackItemNODE(item.Items[index]));
                }

            return node;
        }

        /// <summary>
        /// Check page exists and select if true. Return true if selected but can not selected return false.
        /// </summary>
        /// <param name="key">Name of page.</param>
        public bool ControlAndSelectPage(string key) {
            int dex = tab.IndexOf(key);
            if(dex != -1) {
                tab.SelectedIndex = dex;
                return true;
            } else
                return false;
        }

        /// <summary>
        /// Refresh explorer content.
        /// </summary>
        public void RefreshExplorer() {
            explorerTree.Nodes[0].Nodes.Clear();
            explorerTree.Nodes[1].Nodes.Clear();
            explorerTree.Nodes[2].Nodes.Clear();
            explorerTree.Nodes[3].Nodes.Clear();
            explorerTree.Nodes[4].Nodes.Clear();

            TreeNode
                columnsNode,
                cacheNode,
                columnNode;

            var sectors = DB.GetSectors();
            for(int index = 0; index < sectors.Count; index++) {
                cacheNode = new TreeNode();
                cacheNode.Text =sectors[index].Name;
                cacheNode.Tag="Sector";
                cacheNode.ImageIndex=4;
                cacheNode.SelectedImageIndex=cacheNode.ImageIndex;
                explorerTree.Nodes[0].Nodes.Add(cacheNode);
            }

            var stacks = DB.GetStacks();
            for(int index = 0; index < stacks.Count; index++) {
                cacheNode = new TreeNode();
                cacheNode.Text =stacks[index].Name;
                cacheNode.Tag="Stack";
                cacheNode.ImageIndex=4;
                cacheNode.SelectedImageIndex=cacheNode.ImageIndex;

                if(stacks[index].Items.Count >0)
                    for(int itemIndex = 0; itemIndex < stacks[index].Items.Count; itemIndex++)
                        cacheNode.Nodes.Add(GetMochaStackItemNODE(stacks[index].Items[itemIndex]));

                explorerTree.Nodes[1].Nodes.Add(cacheNode);
            }

            MochaCollectionResult<MochaColumn> columns;
            var tables = DB.GetTables();
            for(int index = 0; index < tables.Count; index++) {
                columnsNode = new TreeNode();
                columnsNode.Text ="Columns";
                columnsNode.Tag="Columns";
                columnsNode.ImageIndex =0;
                columnsNode.SelectedImageIndex=tablesNode.ImageIndex;

                cacheNode = new TreeNode();
                cacheNode.Text =tables[index].Name;
                cacheNode.Tag="Table";
                cacheNode.ImageIndex =1;
                cacheNode.SelectedImageIndex=cacheNode.ImageIndex;
                cacheNode.Nodes.Add(columnsNode);

                columns = DB.GetColumns(cacheNode.Text);
                for(int columnIndex = 0; columnIndex < columns.Count; columnIndex++) {
                    columnNode = new TreeNode();
                    columnNode.Text =columns[columnIndex].Name;
                    columnNode.Tag="Column";
                    columnNode.ImageIndex=3;
                    columnNode.SelectedImageIndex=columnNode.ImageIndex;
                    columnsNode.Nodes.Add(columnNode);
                }

                explorerTree.Nodes[2].Nodes.Add(cacheNode);
            }

            var disks = DB.FileSystem.GetDisks();
            for(int index = 0; index < disks.Count; index++) {
                IMochaDisk disk = disks[index];
                cacheNode=new TreeNode();
                cacheNode.Text=disk.Root;
                cacheNode.Tag="Disk";
                cacheNode.ImageIndex=5;
                cacheNode.SelectedImageIndex=cacheNode.ImageIndex;
                explorerTree.Nodes[3].Nodes.Add(cacheNode);
            }

            var logs = DB.GetLogs();
            for(int index = 0; index < logs.Count; index++) {
                MochaLog log = logs[index];
                cacheNode = new TreeNode();
                cacheNode.Text=log.ID;
                cacheNode.Tag="Log";
                cacheNode.ImageIndex=3;
                cacheNode.SelectedImageIndex=cacheNode.ImageIndex;
                explorerTree.Nodes[4].Nodes.Add(cacheNode);
            }
        }

        #endregion

        #region Overrides

        protected override void Dispose(bool disposing) {
            DB.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Properties

        /// <summary>
        /// MochaDatabase object.
        /// </summary>
        public MochaDatabase DB { get; private set; }

        #endregion
    }

    /// <summary>
    /// page for MochaDB FileSystems.
    /// </summary>
    public sealed class FileSystemPage:Page {
        #region Fields

        private Panel topPanel;
        private TextInput pathBox;
        private FlatGrid exploreGrid;
        private FlatButton backButton;
        private string path;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new FileSystemPage.
        /// </summary>
        public FileSystemPage() {
            Text="FileSystem";
            Image=Resources.Disk;

            #region topPanel

            topPanel=new Panel();
            topPanel.Dock=DockStyle.Top;
            topPanel.BackColor=Color.Gray;
            topPanel.Height=25;
            Controls.Add(topPanel);

            #endregion

            #region backButton

            backButton=new FlatButton();
            backButton.Location=Point.Empty;
            backButton.Size=new Size(30,topPanel.Height);
            backButton.Anchor=AnchorStyles.Bottom|AnchorStyles.Top|AnchorStyles.Left;
            backButton.Text="<";
            backButton.Font=new Font("Consolas",14,FontStyle.Bold);
            backButton.Click+=BackButton_Click;
            topPanel.Controls.Add(backButton);

            #endregion

            #region pathBox

            pathBox=new TextInput();
            pathBox.Readonly=true;
            pathBox.Location=new Point(backButton.Width,0);
            pathBox.Anchor=AnchorStyles.Top|AnchorStyles.Right|AnchorStyles.Left;
            pathBox.Size=new Size(topPanel.Width-backButton.Width,topPanel.Height);
            topPanel.Controls.Add(pathBox);

            #endregion

            #region exploreGrid

            exploreGrid=new FlatGrid();
            exploreGrid.CellBorderStyle=DataGridViewCellBorderStyle.None;
            exploreGrid.SelectionMode=DataGridViewSelectionMode.FullRowSelect;
            exploreGrid.Columns.Add(new DataGridViewImageColumn() {
                Name="Type",
                HeaderText=string.Empty,
                AutoSizeMode=DataGridViewAutoSizeColumnMode.NotSet,
                ImageLayout=DataGridViewImageCellLayout.Zoom
            });
            exploreGrid.Columns.Add("Name","Name");
            exploreGrid.Columns.Add("Description","Description");
            exploreGrid.Location=new Point(0,topPanel.Height);
            exploreGrid.Size=new Size(Width,Height-exploreGrid.Location.Y);
            exploreGrid.Anchor=AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            exploreGrid.CellDoubleClick+=ExploreGrid_CellDoubleClick;
            Controls.Add(exploreGrid);

            #endregion
        }

        #endregion

        #region backButton

        private void BackButton_Click(object sender,EventArgs e) {
            int dex = Path.LastIndexOf('/');
            Path=dex == -1 ? Path : Path.Substring(0,dex);
        }

        #endregion

        #region exploreGrid

        private void ExploreGrid_CellDoubleClick(object sender,DataGridViewCellEventArgs e) {
            if(exploreGrid.Rows[e.RowIndex].Cells[0].Tag!="Folder")
                return;

            if(e.ColumnIndex==-1 || e.RowIndex==-1)
                return;

            Path+=$"/{exploreGrid.Rows[e.RowIndex].Cells[1].Value}";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load path content.
        /// </summary>
        public void ReadPath() {
            exploreGrid.Rows.Clear();

            IMochaCollectionResult<MochaDirectory> directories = FileSystem.GetDirectories(Path);
            IMochaCollectionResult<MochaFile> files = FileSystem.GetFiles(Path);
            for(int index = 0; index < directories.Count; index++) {
                IMochaDirectory directory = directories[index];
                exploreGrid.Rows.Add(Resources.Folder,directory.Name,directory.Description);
                exploreGrid.Rows[exploreGrid.Rows.Count-1].Cells[0].Tag="Folder";
            }
            for(int index = 0; index < files.Count; index++) {
                IMochaFile file = files[index];
                exploreGrid.Rows.Add(Resources.File,file.FullName,file.Description);
                exploreGrid.Rows[exploreGrid.Rows.Count-1].Cells[0].Tag="File";
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current path.
        /// </summary>
        public string Path {
            get =>
                path;
            set {
                if(value==path)
                    return;

                path=value;
                pathBox.Text=value;
                ReadPath();
            }
        }

        /// <summary>
        /// FileSystem of database.
        /// </summary>
        public MochaFileSystem FileSystem { get; set; }

        #endregion
    }

    /// <summary>
    /// Page for MochaScript pages.
    /// </summary>
    public sealed class ScriptPage:Page {
        #region Fields

        private PageView tab;
        private Page editPage;
        private CodeEditor scriptBox;
        private Terminal terminal;

        private MochaScriptDebugger debugger;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new ScriptPage.
        /// </summary>
        /// <param name="path">Path of MochaScript file.</param>
        public ScriptPage(string path) {
            Debugger = new MochaScriptDebugger(path);
            Image=Resources.ScriptDocument;
            Text=FileSystem.GetFileInfo(path).Name;
            Text=Text.Substring(0,Text.Length-12);
            Tip=path;

            #region tab

            tab = new PageView();
            tab.Dock=DockStyle.Fill;
            tab.EntryColor = Color.Gray;
            tab.ItemHeight = 26;
            tab.SetExplorerButtonVisible(false);
            tab.Closeable=false;
            Controls.Add(tab);

            #endregion

            #region editPage

            editPage = new Page();
            editPage.Image=Resources.ScriptDocument;
            editPage.Text="Script";
            scriptBox = new CodeEditor();
            scriptBox.ForeColor = Color.White;
            scriptBox.BackColor = Color.FromArgb(24,24,24);
            scriptBox.Dock = DockStyle.Fill;
            scriptBox.BorderStyle = BorderStyle.None;
            scriptBox.Language = Language.MochaScript;
            //scriptBox.Font = new Font("Consolas",13,FontStyle.Regular,GraphicsUnit.Pixel);
            scriptBox.Text=Debugger.MochaScript;
            scriptBox.TextChanged+=ScriptBox_TextChanged;
            scriptBox.KeyDown+=ScriptBox_KeyDown;
            editPage.Controls.Add(scriptBox);

            tab.Add(editPage);

            #endregion

            #region outputTerminal

            terminal = new Terminal();
            terminal.SetBase(Text);
            terminal.InputProcessing+=Terminal_InputProcessing;
            terminal.BannedCommandNamespaces = new[] { "cnc" };
            tab.Add(terminal);

            #endregion
        }

        #endregion

        #region scriptBox

        private void ScriptBox_TextChanged(object sender,EventArgs e) {
            if(scriptBox.Text!=Debugger.MochaScript && editPage.Text[0]!='*') {
                editPage.Text=editPage.Text.Insert(0,"*");
            } else if(scriptBox.Text==Debugger.MochaScript && editPage.Text[0]=='*') {
                editPage.Text=editPage.Text.Remove(0,1);
            }
        }

        private void ScriptBox_KeyDown(object sender,KeyEventArgs e) {
            if(e.KeyCode==Keys.F5) {
                DebugAsync();
            } else if(e.Control && e.KeyCode ==Keys.S) {
                Save();
            }
        }

        #endregion

        #region terminal

        private void Terminal_InputProcessing(object sender,TerminalInputProcessEventArgs e) {
            if(e.Input == "save") {
                e.Cancel=true;
                Save();
            } else if(e.Input=="debug") {
                e.Cancel=true;
                DebugAsync();
            }
        }

        #endregion

        #region debugger

        private void Debugger_Echo(object sender,MochaScriptEchoEventArgs e) {
            terminal.AddInput(new TerminalInput(string.Empty,
                e.Message == null ? "{nil}" : e.Message.ToString(),null,terminal.Font),false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Debug MochaScript codes asynchronous.
        /// </summary>
        public void DebugAsync() {
            Save();
            tab.SelectedIndex=1;
            terminal.Focus();
            terminal.UseUserInput=false;
            terminal.AddInput(new TerminalInput(string.Empty,
                "Debugging MochaScript code and running...",null,terminal.Font),false);
            Task debugTask = new Task(() => {
                try {
                    Debugger.DebugRun();
                    terminal.AddInput(new TerminalInput(string.Empty,
                        "This MochaScript code debugged and runed sucessfully.\n",Color.LimeGreen,null,terminal.Font),false);
                } catch(Exception excep) {
                    terminal.AddInput(new TerminalInput(string.Empty,
                        excep.Message + "\n",Color.Red,null,terminal.Font),false);
                }
                terminal.UseUserInput=true;
            });
            debugTask.Start();
        }

        /// <summary>
        /// Save MochaScript from editor.
        /// </summary>
        public void Save() {
            string path = Debugger.ScriptPath;
            Debugger.Dispose();
            FileSystem.WriteTextFile(path,scriptBox.Text);
            Debugger = new MochaScriptDebugger(path);
            if(editPage.Text[0]=='*') {
                editPage.Text=editPage.Text.Remove(0,1);
            }
        }

        #endregion

        #region Overrides

        protected override void Dispose(bool disposing) {
            Debugger.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Properties

        /// <summary>
        /// MochaScript debugger.
        /// </summary>
        public MochaScriptDebugger Debugger {
            get =>
                debugger;
            set {
                debugger=value;
                debugger.Echo+=Debugger_Echo;
            }
        }

        #endregion
    }
}
