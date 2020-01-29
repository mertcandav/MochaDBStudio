using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using MochaDB;
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

            ForeColor = Color.Black;
            tabPages = new List<Page>();
            ItemSize = new Size(10,30);
            selectedTab = null;
            selectedIndex = -1;
            closeable = true;

            #region stripButton

            stripButton = new FlatButton();
            stripButton.Text = "▼";
            stripButton.Size = new Size(20,ItemSize.Height -2);
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

            stripButton.Height=ItemSize.Height;

            if(TabCount==0) {
                stripButton.Hide();
                return;
            }

            stripButton.Show();

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
            using(SolidBrush BackBrush = new SolidBrush(Color.LightGray)) {
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
            }

            if(TabCount >0)
                e.Graphics.FillRectangle(Brushes.LightGray,0,ItemSize.Height - 2,Width,2);
        }

        #endregion

        #region stripButton

        private void StripButton_Click(object sender,System.EventArgs e) {
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

        #endregion

        #region Methods

        /// <summary>
        /// Add page.
        /// </summary>
        /// <param name="page">Page object to add.</param>
        public void Add(Page page) {
            page.Size = new Size(Width,Height - ItemSize.Height);
            page.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            page.Location = new Point(0,ItemSize.Height);
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
                TextRenderer.MeasureText(TabPages[index].Text,Font).Width + 35,ItemSize.Height);
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
        /// Page title size.
        /// </summary>
        public Size ItemSize { get; set; }

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

        #endregion
    }

    /// <summary>
    /// Page for PageView.
    /// </summary>
    public abstract class Page:Panel {
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

        private TreeNode sectorsNode;
        private TreeNode tablesNode;
        private TreeNode terminalNode;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new ConnectionPage.
        /// </summary>
        /// <param name="db">MochaDatabase.</param>
        public ConnectionPage(MochaDatabase db) {
            DB = db;
            DB.ChangeContent+=DB_ChangeContent;
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

            #endregion

            #region explorerTree

            explorerTree = new TreeView();
            explorerTree.Dock = DockStyle.Left;
            explorerTree.Width=150;
            explorerTree.BorderStyle = BorderStyle.None;
            explorerTree.ImageList =imageList;
            explorerTree.BackColor = Color.WhiteSmoke;
            explorerTree.NodeMouseDoubleClick+=ExplorerTree_NodeMouseDoubleClick;

            #endregion

            #region sectorsNode

            sectorsNode = new TreeNode();
            sectorsNode.Text="Sectors";
            sectorsNode.Tag="Sectors";
            sectorsNode.ImageIndex =0;
            sectorsNode.SelectedImageIndex=sectorsNode.ImageIndex;

            explorerTree.Nodes.Add(sectorsNode);

            #endregion

            #region tablesNode

            tablesNode = new TreeNode();
            tablesNode.Text="Tables";
            tablesNode.Tag="Tables";
            tablesNode.ImageIndex =0;
            tablesNode.SelectedImageIndex=tablesNode.ImageIndex;

            explorerTree.Nodes.Add(tablesNode);

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
            tab.BackColor = Color.White;
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

        #region explorerTree

        private void ExplorerTree_NodeMouseDoubleClick(object sender,TreeNodeMouseClickEventArgs e) {
            if(e.Node.Level == 0 && e.Node.Text =="Terminal") {
                if(!ControlAndSelectPage("Terminal")) {
                    Terminal terminal = new Terminal();
                    terminal.Name="Terminal";
                    terminal.DB=DB;
                    terminal.InputProcessing+=Terminal_InputProcessing;
                    tab.Add(terminal);
                }
            } else {

            }
        }

        #endregion

        #region terminal

        private void Terminal_InputProcessing(object sender,TerminalInputProcessEventArgs e) {
            if(e.Input.StartsWith("cnc")) {
                e.Cancel=true;
                Terminal terminal = sender as Terminal;
                terminal.TerminalErrorEcho("Can't use the connection commands in database terminal!");
            }
        }

        #endregion

        #region Methods

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

            TreeNode
                columnsNode,
                cacheNode,
                columnNode;

            IList<MochaSector> sectors = DB.GetSectors();
            for(int index = 0; index < sectors.Count; index++) {
                cacheNode = new TreeNode();
                cacheNode.Text =sectors[index].Name;
                cacheNode.Tag="Sector";
                cacheNode.ImageIndex=4;
                cacheNode.SelectedImageIndex=cacheNode.ImageIndex;
                explorerTree.Nodes[0].Nodes.Add(cacheNode);
            }

            IList<MochaColumn> columns;
            IList<MochaTable> tables = DB.GetTables();
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

                explorerTree.Nodes[1].Nodes.Add(cacheNode);
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
}