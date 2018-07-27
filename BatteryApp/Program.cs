using System;
using System.Windows.Forms;

namespace BatteryApp
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Run(new NotifyTrayIcon());
        }
    }
}
