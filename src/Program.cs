using System;
using System.Windows.Forms;

namespace MochaDBStudio {
    /// <summary>
    /// Base class for application.
    /// </summary>
    internal static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Start arguments.</param>
        [STAThread]
        private static void Main(string[] args) {
            Arguments = args;

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

        #region Properties

        /// <summary>
        /// Start arguments.
        /// </summary>
        public static string[] Arguments { get; private set; }

        #endregion
    }
}
