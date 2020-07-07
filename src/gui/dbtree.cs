using System.Windows.Forms;
using MochaDBStudio.Properties;

namespace MochaDBStudio.gui {
    /// <summary>
    /// TreeView for MochaDB Studio.
    /// </summary>
    public sealed partial class dbtree:TreeView {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public dbtree() {
            Init();
        }

        #endregion

        #region Overrides

        protected override void OnAfterExpand(TreeViewEventArgs e) {
            if(e.Node.ImageIndex == 0) {
                e.Node.ImageIndex = 1;
                e.Node.SelectedImageIndex = 1;
            }
        }

        protected override void OnAfterCollapse(TreeViewEventArgs e) {
            if(e.Node.ImageIndex == 1) {
                e.Node.ImageIndex = 0;
                e.Node.SelectedImageIndex = 0;
            }
        }

        #endregion
    }

    // Designer.
    public sealed partial class dbtree {
        #region Components

        private ImageList
            images;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            LabelEdit = false;
            Dock = DockStyle.Fill;
            BorderStyle = BorderStyle.None;
            PathSeparator="/";

            #endregion

            #region images

            images = new ImageList();
            images.ColorDepth=ColorDepth.Depth32Bit;
            images.Images.Add("FolderClose",Resources.FolderClose);
            images.Images.Add("FolderOpen",Resources.FolderOpen);
            images.Images.Add("Table",Resources.Table);
            images.Images.Add("Stack",Resources.Stack);
            images.Images.Add("Sector",Resources.Sector);
            images.Images.Add("Key",Resources.Key);
            ImageList = images;

            #endregion
        }
    }
}
