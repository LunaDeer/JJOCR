using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Universe;

namespace Ocr
{
    static class Program
    {

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Array.BinarySearch(args, "-dev") != -1)
            {
                Global.AllocConsole();
            }
            bool isNotRunning;
            System.Threading.Mutex instance = new System.Threading.Mutex(true, "py0KpPi9dA", out isNotRunning);
            if (!isNotRunning)
            {
                System.Environment.Exit(1);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
