using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace CrawlingEngine
{
    public class Crawler
    {
        public Crawler(String path, int depth)
        {
            //TODO logging, we don't have the logger :(
            String outputPath = path.Substring(path.IndexOf('.') + 1,path.LastIndexOf('.') - path.IndexOf('.') -1) + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Year.ToString();
            String arguments = path + " -r" + depth.ToString() + " -O " + outputPath;
            //DEBUG - Console.WriteLine("Running httrack with arguments: " + arguments);
            Process.Start(Directory.GetCurrentDirectory() + "/httrack/httrack.exe", arguments);
        }
    }
}
