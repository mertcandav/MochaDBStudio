using System;
using System.Windows.Forms;

namespace MochaDBStudio {
    /// <summary>
    /// Base class for application.
    /// </summary>
    public static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main() {
            //Run application.
            RunApplication();
        }

        /// <summary>
        /// Run application.
        /// </summary>
        private static void RunApplication() {
            Control.CheckForIllegalCrossThreadCalls = false;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Studio());
        }
    }
}
