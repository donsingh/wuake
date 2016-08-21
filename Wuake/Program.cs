using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wuake.Forms;

namespace Wuake
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            /// Ok, so today we'll try out some P/Invoke features along with WinForms to create a globally hotkeyable WinForms application
            /// I'm not really sure where this will take us yet, so let's just jump right in.

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());


        }
    }
}
