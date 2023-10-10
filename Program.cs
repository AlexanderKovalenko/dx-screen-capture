using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXScreenCapture {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            // Enable DPI awareness for your application
            if (Environment.OSVersion.Version.Major >= 6)
                WinAPI.SetProcessDpiAwareness(WinAPI.ProcessDPIAwareness.ProcessPerMonitorDPIAware);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new Form1();

            Application.Run();
        }
    }
}