using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Yvonne2
{
    public partial class Form1 : Form
    {
        private BackgroundWorker bw;
        private int seconds;
        private int delay;

        public Form1()
        {
            InitializeComponent();

            // Delay time
            seconds = Properties.Settings.Default.PauseTime;
        }

        private void SetupWorker()
        {
            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerReportsProgress = false;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerAsync();
        }

        private void navigate(Pic p)
        {
            string url = String.Format("{0}?f={1}&t={2}",
                                       Program.URL,
                                       System.Uri.EscapeDataString(p.Full),
                                       System.Uri.EscapeDataString(p.Thumb));
            webBrowser1.Navigate(url);
        }

        public void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            // BOTH THESE NEED TO WORK
            try
            {
                // Load the list
                Program.LIST = new Yvonne(Properties.Settings.Default.MainUrl.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\n\n{1}", ex.Message, ex.StackTrace));
                this.Close();
            }

            // Gets a random image
            foreach (Pic p in Program.LIST)
            {
                navigate(p);

                delay = seconds;

                while (delay > 0)
                {
                    // Pause one second
                    Thread.Sleep(1000);
                    // Decrease the delay counter
                    delay -= 1;
                }
                
            }
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            delay = -1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // splash screen
            webBrowser1.Navigate(Program.URL);

            // Start the refresh process
            SetupWorker();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "Notepad.exe";
            p.StartInfo.Arguments = Program.TEMP_FILE;
            p.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            p.StartInfo.ErrorDialog = true;
            p.Start();
        }
    }
}
