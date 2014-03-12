using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace DemoHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(@"..\..\..\SenderConsoleApplication\bin\Debug\SenderConsoleApplication.exe");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start(@"..\..\..\RecieverConsoleApplication\bin\Debug\RecieverConsoleApplication.exe");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start(@"..\..\..\RecieverConsoleApplication\bin\Debug\RecieverConsoleApplication2.exe");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start(@"..\..\..\SubscriptionControlBridgeConsoleApplication\bin\Debug\SubscriptionControlBridgeConsoleApplication.exe");
        }

        private void button5_Click(object sender, EventArgs e)
        {

            Process.Start(@"..\..\..\SubscriptionControlConsole\bin\Debug\SubscriptionControlConsole.exe");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Process.Start("Architecture.docx");
        }
    }
}
