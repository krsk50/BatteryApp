using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace BatteryApp
{
    public partial class PowerStatusBrowserForm : Form
    {
        private PowerStatusBrowserForm power;
        private void PowerStatusBrowserForm_Load(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Normal:
                    if (power == null)
                        power = new PowerStatusBrowserForm();
                    ShowInTaskbar = false;
                    this.Hide();
                    break;
                case FormWindowState.Minimized:
                    if (power == null)
                        power = new PowerStatusBrowserForm();
                    ShowInTaskbar = false;
                    this.Hide();
                    break;
                case FormWindowState.Maximized:
                    if (power == null)
                        power = new PowerStatusBrowserForm();
                    break;
                default:
                    break;
            }
        }

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox textBox1;
        private Timer timer = new Timer();
        public PowerStatusBrowserForm()
        {
            this.SuspendLayout();
            InitForm();
            timer.Tick += Timer_Tick;
            timer.Interval = 20000;

            //Add each property of the PowerStatus class to the list box.
            Type t = typeof(System.Windows.Forms.PowerStatus);
            PropertyInfo[] pi = t.GetProperties();
            for (int i = 0; i < pi.Length; i++)
                listBox1.Items.Add(pi[i].Name);
            textBox1.Text = "The PowerStatus class has " + pi.Length.ToString() + " properties.\r\n";

            // Configure the list item selected handler for the list box to invoke a 
            // method that displays the value of each property.           
            listBox1.SelectedIndexChanged += new EventHandler(listBox1_SelectedIndexChanged);
            this.ResumeLayout(false);
            //timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            PropertyInfo batteryProp = GetProperty("BatteryLifePercent");
            PropertyInfo batterychargingProp = GetProperty("PowerLineStatus");
            double batteryValue = Math.Round(Convert.ToDouble(batteryProp.GetValue(SystemInformation.PowerStatus, null)), 2);
            string chargeStatus = Convert.ToString(batterychargingProp.GetValue(SystemInformation.PowerStatus, null));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Return if no item is selected.
            if (listBox1.SelectedIndex == -1) return;
            // Get the property name from the list item
            string propname = listBox1.Text;

            // Display the value of the selected property of the PowerStatus type.
            PropertyInfo prop = GetProperty(propname);

            object propval = prop.GetValue(SystemInformation.PowerStatus, null);
            textBox1.Text += "\r\nThe value of the " + propname + " property is: " + propval.ToString();
        }

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

        private void InitForm()
        {
            // Initialize the form settings

            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.Location = new System.Drawing.Point(8, 16);
            this.listBox1.Size = new System.Drawing.Size(172, 496);
            this.listBox1.TabIndex = 0;
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(188, 16);
            this.textBox1.Multiline = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(420, 496);
            this.textBox1.TabIndex = 1;
            this.ClientSize = new System.Drawing.Size(616, 525);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listBox1);
            this.Text = "Select a PowerStatus property to get the value of";
            this.Load += PowerStatusBrowserForm_Load;
        }
    }
}
