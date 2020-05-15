using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MochaDBStudio.gui.codesense {
    [ToolboxItem(false)]
    internal class codesenseHost:ToolStripDropDown {
        private IAutocompleteListView listView;
        public ToolStripControlHost Host { get; set; }
        public readonly codesense Menu;
        public codesenseListView AlistView = new codesenseListView();

        public IAutocompleteListView ListView {
            get { return listView; }
            set {

                if(listView != null)
                    (listView as Control).LostFocus -= new EventHandler(ListView_LostFocus);

                if(value == null)
                    listView = new codesenseListView();
                else {
                    if(!(value is Control))
                        throw new Exception("ListView must be derived from Control class");

                    listView = value;
                }

                Host = new ToolStripControlHost(ListView as Control);
                Host.Margin = new Padding(2,2,2,2);
                Host.Padding = Padding.Empty;
                Host.AutoSize = false;
                Host.AutoToolTip = false;

                (ListView as Control).MaximumSize = Menu.MaximumSize;
                (ListView as Control).Size = Menu.MaximumSize;
                (ListView as Control).LostFocus += new EventHandler(ListView_LostFocus);

                CalcSize();
                base.Items.Clear();
                base.Items.Add(Host);
                (ListView as Control).Parent = this;
            }
        }

        public codesenseHost(codesense menu) {
            AutoClose = false;
            AutoSize = false;
            Margin = Padding.Empty;
            Padding = Padding.Empty;

            Menu = menu;
            ListView = AlistView;
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            using(var brush = new SolidBrush(listView.Colors.BackColor))
                e.Graphics.FillRectangle(brush,e.ClipRectangle);
        }

        internal void CalcSize() {
            Host.Size = (ListView as Control).Size;
            Size = new System.Drawing.Size((ListView as Control).Size.Width + 4,(ListView as Control).Size.Height + 4);
        }

        public override RightToLeft RightToLeft {
            get {
                return base.RightToLeft;
            }
            set {
                base.RightToLeft = value;
                (ListView as Control).RightToLeft = value;
            }
        }

        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            if(!(ListView as Control).Focused)
                Close();
        }

        void ListView_LostFocus(object sender,EventArgs e) {
            if(!Focused)
                Close();
        }
    }
}
