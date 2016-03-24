using System;
using System.Windows.Forms;

namespace TSP
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
       {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Test.run();
            Application.Run(MainForm = new mainform());
        }
        public static mainform MainForm;

    }
}