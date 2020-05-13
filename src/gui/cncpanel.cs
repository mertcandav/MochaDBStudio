using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MochaDB;
using MochaDB.Querying;

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
            refreshDashboard();
        }

        #endregion

        #region tab

        private void Tab_SelectedIndexChanged(object sender,EventArgs e) {
            if(tab.SelectedIndex == 0) /* Dashboard */ {
                refreshDashboard();
            } else if(tab.SelectedIndex == 2) /* Settings */ {
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
            new Task(() => {
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
            }).Start();

            new Task(() => {
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
            }).Start();
        }

        /// <summary>
        /// Refresh "Settings" tab.
        /// </summary>
        public void refreshSettings() {
            passwordTB.Text = Database.GetPassword();
            descriptionTB.Text = Database.GetDescription();
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
            settingsPage,
            dashboardPage,
            contentPage;

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
            // Content
            // 

            #region contentPage

            contentPage = new TabPage();
            contentPage.Text = "Content";
            contentPage.BackColor = BackColor;
            tab.TabPages.Add(contentPage);

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
            descriptionTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            descriptionTB.BackColor = BackColor;
            descriptionTB.ForeColor = Color.White;
            descriptionTB.Location = new Point(40,passwordTB.Location.Y + passwordTB.Height + 30);
            descriptionTB.Size = new Size(passwordTB.Width,20);
            descriptionTB.TextChanged +=DescriptionTB_TextChanged;
            settingsPage.Controls.Add(descriptionTB);

            #endregion
        }
    }
}
