using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Logger
{
    class Log
    {
        System.IO.StreamWriter file;
        string format;

        public Log(string filePath)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath);
        }

        void writeWarning(string text)
        {
            file.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss")+"   WARNING: "+text);
        }

        void writeError(string text)
        {
            file.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + "   ERROR: " + text);
        }

        void writeFatal(string text)
        {
            file.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + "   FATAL: " + text);
        }

        void writeInfo(string text)
        {
            file.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + "   INFO: " + text);
        }

        void writeDebug(string text)
        {
            file.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + "   DEBUG: " + text);
        }

        void writeUser(string text, string type)
        {
            file.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + "   "+type+": " + text);
        }
    }
}
