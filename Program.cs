using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Windows.Forms;
using iServer;


namespace Yvonne2
{
    class Program
    {
        public static MyServer s;
        public static int PORT;
        public static string URL;
        public static Yvonne LIST;
        public static string TEMP_FILE = "template.htm";


        [STAThread]
        static void Main(string[] args)
        {
            // Set port and url
            Random r = new Random();
            PORT = r.Next(8855, 65456);
            URL = String.Format("http://localhost:{0}/", PORT.ToString());

            // Create server and start it
            s = new MyServer(PORT);
            s.startServer();

            // Add Exit handler
            Application.ApplicationExit += new EventHandler(OnAppExit);

            // Load the template
            Program.TEMP_FILE = String.Format("{0}\\{1}", Directory.GetCurrentDirectory(), Program.TEMP_FILE);
            if (File.Exists(Program.TEMP_FILE) == false)
            {
                File.WriteAllText(Program.TEMP_FILE, loadTemplate());
            }

            if (File.Exists(Program.TEMP_FILE) == true)
            {
                // Start GUI
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                // Exit if there's no template file
                MessageBox.Show("Could not deploy template.");
            }
        }

        static void OnAppExit(object Sender, EventArgs e)
        {
            s.stopServer();
        }

        static string loadTemplate()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            StreamReader sr = new StreamReader(a.GetManifestResourceStream("Yvonne2.template.htm"));
            return sr.ReadToEnd();
        }
    }
}
