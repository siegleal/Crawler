using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Crawler
{
    public class Crawler
    {
        public Crawler(String path, int depth)
        {
            //TODO logging, we don't have the logger :(
            String outputPath = path.Substring(path.IndexOf('.') + 1,path.LastIndexOf('.') - path.IndexOf('.') -1) + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Year.ToString() + DateTime.Now.ToFileTime().ToString();
         
            String arguments = path + " -g -r" + depth.ToString() + " -O " + outputPath;
            //DEBUG - Console.WriteLine("Running httrack with arguments: " + arguments);
            Process p = Process.Start(Directory.GetCurrentDirectory() + "/httrack/httrack.exe", arguments);
            p.WaitForExit();

            outputPath =  Directory.GetCurrentDirectory() + "\\"  +  outputPath;
            
            Website site = new Website(path,outputPath);
            Log log = new Log(outputPath + "\\log.txt");


            WebsiteParser siteparser = new WebsiteParser(site, null, 0, log);
            siteparser.analyzeSite(site, null, 0, log);

            log.destroy();
        }
    }
}
