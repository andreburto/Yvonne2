using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TemplateEngine
{
    class HtmlRender
    {
        // PRIVATE VARIABLES
        private Hashtable _variables;
        private Templarse _templarse;

        // PUBLIC
        public void AddVariable(string key, string val)
        {
            if (key.Length == 0 || val.Length == 0) { return; }
            _variables.Add(key, val);
        }

        public void RemoveVariable(string key)
        {
            if (key.Length == 0 || _variables.Contains(key) == false) { return; }
            _variables.Remove(key);
        }

        public string Render(string template)
        {
            _templarse = new Templarse(template, _variables);
            return _templarse.ExecuteTemplate();
        }

        // CONSTRUCTOR
        public HtmlRender() { }

        public HtmlRender(Hashtable pre_var)
        {
            _variables = pre_var;
        }
    }

    class Templarse
    {
        // PROPERTIES
        private string _template = "";
        public string Template
        {
            get { return _template; }
        }

        private List<StackItem> _stack;
        public List<StackItem> Stack
        {
            get { return _stack; }
        }

        private Hashtable _commands;

        // PUBLIC
        public string ExecuteTemplate()
        {
            string retval = "";
            foreach (StackItem i in _stack)
            {
                if (i.Type == (int)StackItem.ItemType.HTML)
                {
                    retval += i.Item;
                }
                else if (i.Type == (int)StackItem.ItemType.COMMAND)
                {
                    retval += Commands(i.Item);
                }

            }
            return retval;
        }

        public string Commands(string c)
        {
            string[] parts = c.Split(' ');
            string cmd = parts[0].ToLower();

            // If the first command isn't a command return blank
            if (_commands.Contains(cmd) == false) { return ""; }

            // Copies the command table except for the current command
            Hashtable temp_cmds = new Hashtable();
            foreach (string k in _commands.Keys)
            {
                if (k != cmd) { temp_cmds[k] = _commands[k]; }
            }

            Templarse temp_tp = new Templarse(_commands[cmd].ToString(), temp_cmds);
            temp_cmds = null;

            // Return
            return temp_tp.ExecuteTemplate();
        }

        // PRIVATE
        private List<StackItem> ParseTemplate(string html)
        {
            List<StackItem> retval = new List<StackItem>();
            string temp_str = "";
            int temp_type = 0;
            int strlen = html.Length;
            int strcnt = 0;

            while (strcnt < strlen)
            {
                string temp_chr = html.Substring(strcnt, 1);
                int do_this = 0;

                // The start of a variable
                if (temp_chr == "<")
                {
                    if (html.Substring(strcnt, 2) == "<%") { do_this = 1; }
                }

                // The close of a variable
                if (temp_chr == ">")
                {
                    if (html.Substring(strcnt - 1, 2) == "%>") { do_this = 2; }
                }

                // The real stuff
                if (do_this == 1)
                {
                    retval.Add(new StackItem(temp_str, temp_type));
                    temp_str = temp_chr;
                    temp_type = (byte)StackItem.ItemType.COMMAND;
                }
                else if (do_this == 2)
                {
                    retval.Add(new StackItem(temp_str, temp_type));
                    temp_str = "";
                    temp_type = (byte)StackItem.ItemType.HTML;
                }
                else
                {
                    temp_str += temp_chr;
                }

                strcnt += 1;
            }

            retval.Add(new StackItem(temp_str, temp_type));

            return retval;
        }

        // CONSTRUCTOR
        public Templarse(string html, Hashtable cmds)
        {
            _template = html;
            _stack = ParseTemplate(html);
            _commands = cmds;
        }
    }

    class StackItem
    {
        // PROPERTIES
        private string _item;
        public string Item
        {
            get { return _item; }
        }

        private int _type;
        public int Type
        {
            get { return _type; }
        }

        // PUBLIC
        public enum ItemType { HTML = 0, COMMAND = 1 }

        // PRIVATE
        private string cleanCommand(string cmd)
        {
            // Take <% off the front
            cmd = cmd.Substring(2);

            // Take %> off the end
            cmd = cmd.Substring(0, cmd.Length - 2);

            // Trim extra spaces at the beinning or end
            while (cmd.Substring(0, 1) == " ") { cmd = cmd.Substring(1); }
            while (cmd.Substring(cmd.Length - 1, 1) == " ") { cmd = cmd.Substring(0, cmd.Length - 1); }

            // Return the cleaned up command
            return cmd;
        }

        // CONSTRUCTOR
        public StackItem(string item, int type)
        {
            // Clean up a command if it is one
            if (type == (byte)ItemType.COMMAND) { _item = cleanCommand(item); }
            else { _item = item; }
            // Assign type to this property
            _type = type;
        }
    }
}
