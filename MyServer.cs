using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using TemplateEngine;

namespace Yvonne2
{
    class MyServer : iServer.iServer
    {
        public override void Command(string path, string args, Hashtable headers, string type, Socket iSocket)
        {
            Hashtable vars = new Hashtable();
            Hashtable argList = parseArgs(args);

            // Load other variables
            if (argList.ContainsKey("f") && argList.ContainsKey("t"))
            {
                vars.Add("full", argList["f"].ToString());
                vars.Add("thumb", argList["t"].ToString());
                vars.Add("image", "<a href=\"<% full %>\"><img src=\"<% thumb %>\" /></a>");
            }
            else
            {
                vars.Add("image", "<h1>One moment.</h1>");
            }
            vars.Add("date", DateTime.Now.ToString("MM/dd/yy H:mm:ss"));
            vars.Add("today", "Today is <% date %>!");

            // Load template
            string templateHtml = File.ReadAllText(Program.TEMP_FILE);

            // Create rendering object
            HtmlRender hr = new HtmlRender(vars);

            // Display results
            string res = hr.Render(templateHtml);

            SendToBrowser(res, iSocket);
        }

        // Constructor
        public MyServer(int port) : base(port)
        {
        }
    }
}
