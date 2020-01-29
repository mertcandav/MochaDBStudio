using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace MochaDBStudio.GUI.Controls {
    /// <summary>
    /// MenuItem listener.
    /// </summary>
    [Serializable]
    public sealed class MenuItemListView:ScrollableControl {
        #region Fields

        private List<MenuItem> items;
        private ListViewListStyle listStyle;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new MenuItemListView.
        /// </summary>
        public MenuItemListView() {
            AutoScroll = true;
            items = new List<MenuItem>();
            listStyle = ListViewListStyle.RelaxList;
        }

        #endregion

        #region Events

        /// <summary>
        /// This happens after any item clicked.
        /// </summary>
        public event EventHandler<ItemListViewItemEventArgs> ItemClicked;
        private void OnItemClicked(object sender,ItemListViewItemEventArgs e) {
            //Invoke.
            ItemClicked?.Invoke(sender,e);
        }

        #endregion

        #region Item Events

        private void Item_Click(object sender,EventArgs e) {
            OnItemClicked(this,new ItemListViewItemEventArgs(sender as MenuItem));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add menu item.
        /// </summary>
        /// <param name="item">MenuItem object to add.</param>
        /// <param name="adapdate">Adapdate controls finally if true.</param>
        public void AddItem(MenuItem item,bool adapdate) {
            if(ListStyle == ListViewListStyle.RelaxList)
                item.Location = new Point(20,(20 * ItemCount) + (ItemCount * item.Height) + 20);
            else {
                item.Location = new Point(0,ItemCount + (ItemCount * item.Height));
                item.Width = ClientSize.Width;
            }

            item.Click += Item_Click;
            items.Add(item);

            if(adapdate)
                AdapdateControls();
        }

        /// <summary>
        /// Adapt controls by menu items.
        /// </summary>
        internal void AdapdateControls() {
            if(ItemCount == 0)
                return;

            Controls.Clear();
            for(int Index = 0; Index < ItemCount; Index++) {
                Controls.Add(this[Index]);
            }

            if(ListStyle == ListViewListStyle.RelaxList)
                AutoScrollMinSize = new Size(0,(20 * ItemCount) + (ItemCount * this[0].Height) + 20);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Menu items.
        /// </summary>
        public IList<MenuItem> Items =>
            items;

        /// <summary>
        /// Count of menu items.
        /// </summary>
        public int ItemCount =>
            items.Count;

        /// <summary>
        /// Get menu item bt index.
        /// </summary>
        /// <param name="Index">Menu item's index.</param>
        public MenuItem this[int Index] =>
            items[Index];

        /// <summary>
        /// List style.
        /// </summary>
        public ListViewListStyle ListStyle {
            get =>
                listStyle;
            set {
                if(value == listStyle)
                    return;

                listStyle = value;
                if(ItemCount > 0) {
                    Controls.Clear();
                    MenuItem[] _items = items.ToArray();
                    items.Clear();
                    for(int index = 0; index < _items.Length; index++) {
                        AddItem(_items[index],false);
                    }
                    AdapdateControls();
                }
            }
        }

        #endregion
    }

    //

    /// <summary>
    /// Base class for ItemListView items.
    /// </summary>
    [Serializable]
    public abstract class MenuItem:Control {
        #region Fields

        protected string title;
        protected Image image;
        protected string description;
        protected Color entryColor;
        protected bool isHover;
        protected bool isMouseDown;
        protected bool isFocus;

        #endregion

        #region Mouse

        protected override void OnMouseEnter(EventArgs e) {
            IsHover = true;

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e) {
            IsHover = false;

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            IsMouseDown = true;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            IsMouseDown = false;

            base.OnMouseUp(e);
        }

        #endregion

        #region Keyboard

        protected override void OnKeyDown(KeyEventArgs e) {
            if(e.KeyCode == Keys.Enter) {
                IsMouseDown = true;
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            if(e.KeyCode == Keys.Enter) {
                IsMouseDown = false;
                OnClick(new EventArgs());
            }

            base.OnKeyUp(e);
        }

        #endregion

        #region Others

        protected override void OnGotFocus(EventArgs e) {
            IsFocus = true;

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e) {
            IsFocus = false;

            base.OnLostFocus(e);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Title of item.
        /// </summary>
        public string Title {
            get =>
                title;
            set {
                if(value == title)
                    return;

                title = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Description of item.
        /// </summary>
        public string Description {
            get =>
                description;
            set {
                if(value == description)
                    return;

                description = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Image of item.
        /// </summary>
        public Image Image {
            get =>
                image;
            set {
                if(value == image)
                    return;

                image = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Entry color of item.
        /// </summary>
        public Color EntryColor {
            get =>
                entryColor;
            set {
                if(value == entryColor)
                    return;

                entryColor = value;
            }
        }

        /// <summary>
        /// Mouse hovering state of item.
        /// </summary>
        public bool IsHover {
            get =>
                isHover;
            private set {
                if(value == isHover)
                    return;

                isHover = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Mouse any key is pressed.
        /// </summary>
        public bool IsMouseDown {
            get =>
                isMouseDown;
            set {
                if(value == isMouseDown)
                    return;

                isMouseDown = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Focused this.
        /// </summary>
        public bool IsFocus {
            get =>
                isFocus;
            set {
                if(value == isFocus)
                    return;

                isFocus = value;
                Invalidate();
            }
        }

        #endregion
    }

    /// <summary>
    /// Menu selection item.
    /// </summary>
    public sealed class MenuSelectionItem:MenuItem {
        #region Constructors

        /// <summary>
        /// Create new MenuSelectionItem.
        /// </summary>
        public MenuSelectionItem() {
            //Drawing optimization.
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,true);

            //----
            Size = new Size(500,100);
            BackColor = Color.FromArgb(230,230,230);
            title = string.Empty;
            image = null;
            description = string.Empty;
            entryColor = Color.DodgerBlue;
            isHover = false;
            isMouseDown = false;
        }

        /// <summary>
        /// Create new MenuSelectionItem.
        /// </summary>
        /// <param name="Title">Title of item.</param>
        /// <param name="Description">Description of item.</param>
        /// <param name="Image">Image of item.</param>
        public MenuSelectionItem(string Title,string Description,Image Image) : this() {
            title = Title;
            description = Description;
            image = Image;
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            using(SolidBrush BackBrush = new SolidBrush(IsMouseDown ? EntryColor : BackColor)) {
                e.Graphics.FillRectangle(BackBrush,ClientRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            //Focusing.
            if(IsHover || IsFocus) {
                using(SolidBrush LineBrush = new SolidBrush(EntryColor)) {
                    e.Graphics.FillRectangle(LineBrush,0,0,5,Height);
                }
            }

            //Border.
            if(EntryColor != Color.Transparent && (IsFocus || IsHover)) {
                using(Pen BorderPen = new Pen(EntryColor,1) {
                    DashStyle = DashStyle.DashDot
                }) {
                    e.Graphics.DrawRectangle(BorderPen,0,0,Width - 1,Height - 1);
                }
            }

            //Image.
            if(Image != null) {
                e.Graphics.DrawImage(Image,10,10,50,50);
            }

            //Title & Description.
            if(!string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(Description)) {
                using(SolidBrush ForeBrush = new SolidBrush(ForeColor)) {
                    //Title.
                    if(!string.IsNullOrEmpty(Title)) {
                        using(Font TitleFont = new Font(Font,FontStyle.Bold)) {
                            e.Graphics.DrawString(Title,TitleFont,ForeBrush,65,10);
                        }
                    }

                    //Description.
                    if(!string.IsNullOrEmpty(Description)) {
                        e.Graphics.DrawString(Description,Font,ForeBrush,65,30);
                    }
                }
            }

            base.OnPaint(e);
        }

        #endregion
    }

    /// <summary>
    /// Menu list item.
    /// </summary>
    public sealed class MenuListItem:MenuItem {
        #region Constructors

        /// <summary>
        /// Create new MenuListItem.
        /// </summary>
        public MenuListItem() {
            //Drawing optimization.
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,true);

            //----
            Size = new Size(200,30);
            BackColor = Color.FromArgb(230,230,230);
            Anchor = AnchorStyles.Left | AnchorStyles.Right |AnchorStyles.Top;
            Font = new Font(Font,FontStyle.Bold);
            image = null;
            description = string.Empty;
            entryColor = Color.DodgerBlue;
            isHover = false;
            isMouseDown = false;
        }

        /// <summary>
        /// Create new MenuListItem.
        /// </summary>
        /// <param name="Title">Title of item.</param>
        /// <param name="Description">Description of item.</param>
        /// <param name="Image">Image of item.</param>
        public MenuListItem(string Title,string Description,Image Image)
            : this() {
            title = Title;
            description = Description;
            image = Image;
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            using(SolidBrush BackBrush = new SolidBrush(IsMouseDown ? EntryColor : BackColor)) {
                e.Graphics.FillRectangle(BackBrush,ClientRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            //Click.
            if(EntryColor != Color.Transparent) {
                using(SolidBrush BackBrush = new SolidBrush(IsMouseDown ? EntryColor : IsHover ? Color.FromArgb(100,EntryColor) : BackColor))
                    e.Graphics.FillRectangle(BackBrush,ClientRectangle);
            }

            //Image.
            if(Image != null) {
                e.Graphics.DrawImage(Image,1,(Height / 2) - 11,22,22);
            }

            //Title.
            if(!string.IsNullOrEmpty(Title))
                using(StringFormat SFormat = new StringFormat() {
                    FormatFlags = StringFormatFlags.NoWrap,
                    LineAlignment = StringAlignment.Center
                })
                using(SolidBrush ForeBrush = new SolidBrush(ForeColor))
                    e.Graphics.DrawString(Title,Font,ForeBrush,new Rectangle(new Point(25,0),ClientSize),SFormat);

            base.OnPaint(e);
        }

        #endregion
    }
}
