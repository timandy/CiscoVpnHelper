using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CiscoVpnHelper
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool Run;
            string Name = "4035a3d4-bdaa-4e18-b4ce-9ea2eef0b308";
            Mutex Mutex = new Mutex(true, Name, out Run);
            if (Run)
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmMain());

                Mutex.ReleaseMutex(); //添加此行,防止生成Release版本时Mutex无效.
            }
        }
    }
}
