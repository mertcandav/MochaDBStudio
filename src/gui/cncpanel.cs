using System;
using System.Drawing;
using System.Windows.Forms;
using MochaDB;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Connection Panel.
    /// </summary>
    public sealed partial class cncpanel:Panel {
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
            refreshDatas();
        }

        #endregion

        #region tab

        private void Tab_SelectedIndexChanged(object sender,EventArgs e) {
            if(tab.SelectedIndex == 0) /* Overview */ {
                refreshOverview();
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
        /// Refresh "Overview" tab.
        /// </summary>
        public void refreshOverview() {

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
            overviewPage,
            contentPage;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            BackColor = Color.FromArgb(60,60,60);
            ForeColor = Color.White;

            #endregion

            #region tab

            tab = new stabcontrol();
            tab.Dock = DockStyle.Fill;
            tab.SelectedIndexChanged +=Tab_SelectedIndexChanged;
            Controls.Add(tab);

            #endregion

            #region overviewPage

            overviewPage = new TabPage();
            overviewPage.Text = "Overview";
            overviewPage.BackColor = BackColor;
            tab.Controls.Add(overviewPage);

            #endregion

            #region contentPage

            contentPage = new TabPage();
            contentPage.Text = "Content";
            contentPage.BackColor = BackColor;
            tab.Controls.Add(contentPage);

            #endregion
        }
    }
}
