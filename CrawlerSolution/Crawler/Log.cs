using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Crawler
{
    public class Log
    {
        private System.IO.StreamWriter file;
        string format;

        public Log(string filePath)
        {
            this.file = File.CreateText(filePath);// = new System.IO.StreamWriter(filePath);
        }

        public void writeWarning(string text)
        {
            file.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss")+"   WARNING: "+text);
        }

        public void writeError(string text)
        {
            file.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + "   ERROR: " + text);
        }

        public void writeFatal(string text)
        {
            file.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + "   FATAL: " + text);
        }

        public void writeInfo(string text)
        {
            file.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + "   INFO: " + text);
        }

        public void writeDebug(string text)
        {
            file.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + "   DEBUG: " + text);
        }

        public void writeUser(string text, string type)
        {
            file.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + "   "+type+": " + text);
        }

        public void destroy()
        {
            file.Close();
        }
    }
}
