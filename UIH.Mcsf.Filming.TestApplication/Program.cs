using System;
using System.Windows;

namespace UIH.Mcsf.Filming.TestApplication
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var win = new MainWindow();
            win.Show();

            var app = new Application();
            app.Run();
        }
    }
}