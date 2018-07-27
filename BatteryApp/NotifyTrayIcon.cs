using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BatteryApp
{
    public partial class NotifyTrayIcon : ApplicationContext
    {
        #region Variable Declaration
        #region Public Variables
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
            string lpszWindow);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        #endregion

        #region Private Variables
        private PowerStatusBrowserForm _form = new PowerStatusBrowserForm();
        private System.Windows.Forms.NotifyIcon TrayNotify;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private Timer timer = new Timer();
        #endregion
        #endregion
        
        #region Constructor
        internal NotifyTrayIcon()
        {
            Application.ApplicationExit += Application_ApplicationExit;
            InitializeComponent();
            RefreshTrayArea();
            TrayNotify.Visible = true;
        }
        #endregion

        #region InitializeComponent
        private void InitializeComponent()
        {
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            timer.Tick += Timer_Tick;
            timer.Interval = 20000;

            // Initialize contextMenu1
            this.contextMenu1.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { new MenuItem("O&pen", menuItem0_Click), new MenuItem("C&lose", menuItem2_Click), new MenuItem("E&xit", menuItem1_Click) });

            // Create the NotifyIcon.
            this.TrayNotify = new System.Windows.Forms.NotifyIcon();

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            TrayNotify.Icon = new Icon(@"batteryicon_vNy_icon.ico");

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            TrayNotify.ContextMenu = this.contextMenu1;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            TrayNotify.Text = "Battery App";

            // Handle the DoubleClick event to activate the form.
            TrayNotify.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            timer.Start();
        } 
        #endregion

        #region Private Static Methods
        /// <summary>
        /// Get the Property from the Property Name
        /// </summary>
        /// <param name="propname"></param>
        /// <returns></returns>
        private static PropertyInfo GetProperty(string propname)
        {
            Type t = typeof(System.Windows.Forms.PowerStatus);
            PropertyInfo[] pi = t.GetProperties();
            PropertyInfo prop = null;
            for (int i = 0; i < pi.Length; i++)
                if (pi[i].Name == propname)
                {
                    prop = pi[i];
                    break;
                }

            return prop;
        }

        private static void RefreshTrayArea()
        {
            IntPtr systemTrayContainerHandle = FindWindow("Shell_TrayWnd", null);
            IntPtr systemTrayHandle = FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            IntPtr sysPagerHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
            IntPtr notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "Notification Area");
            if (notificationAreaHandle == IntPtr.Zero)
            {
                notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32",
                    "User Promoted Notification Area");
                IntPtr notifyIconOverflowWindowHandle = FindWindow("NotifyIconOverflowWindow", null);
                IntPtr overflowNotificationAreaHandle = FindWindowEx(notifyIconOverflowWindowHandle, IntPtr.Zero,
                    "ToolbarWindow32", "Overflow Notification Area");
                RefreshTrayArea(overflowNotificationAreaHandle);
            }
            RefreshTrayArea(notificationAreaHandle);
        }

        private static void RefreshTrayArea(IntPtr windowHandle)
        {
            const uint wmMousemove = 0x0200;
            RECT rect;
            GetClientRect(windowHandle, out rect);
            for (var x = 0; x < rect.right; x += 5)
                for (var y = 0; y < rect.bottom; y += 5)
                    SendMessage(windowHandle, wmMousemove, 0, (y << 16) + x);
        } 
        #endregion

        #region Private Events
        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            // Show the form when the user double clicks on the notify icon.

        }

        private void menuItem0_Click(object Sender, EventArgs e)
        {
            // Open the form, which closes the application.
            if (this._form.Visible == false)
            {
                this._form.WindowState = FormWindowState.Maximized;
                this._form.Show();
            }
        }

        private void menuItem2_Click(object Sender, EventArgs e)
        {
            // Open the form, which closes the application.
            if (this._form.Visible == true)
            {
                this._form.WindowState = FormWindowState.Minimized;
                _form.Hide();
            }
        }

        private void menuItem1_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            this._form.Dispose();
            Application.Exit();
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            TrayNotify.Visible = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            PropertyInfo batteryProp = GetProperty("BatteryLifePercent");
            PropertyInfo batterychargingProp = GetProperty("PowerLineStatus");
            double batteryValue = Math.Round(Convert.ToDouble(batteryProp.GetValue(SystemInformation.PowerStatus, null)), 2);
            string chargeStatus = Convert.ToString(batterychargingProp.GetValue(SystemInformation.PowerStatus, null));
            if (batteryValue == 1 && chargeStatus == "Online")
                DisplayNotification(ToolTipIcon.Warning, "Battery Status", "Battery Full");
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Display the Notification
        /// </summary>
        /// <param name="toolTipIcon"></param>
        /// <param name="Title"></param>
        /// <param name="Message"></param>
        private void DisplayNotification(ToolTipIcon toolTipIcon, string Title, string Message)
        {
            TrayNotify.BalloonTipTitle = Title;
            TrayNotify.BalloonTipText = Message;
            TrayNotify.Visible = true;
            TrayNotify.ShowBalloonTip(10);
        } 
        #endregion
    }
}