using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Crawler
{
    public class CrawlerController
    {
        private List<String> result;
        private String outputPath;

        public CrawlerController(String path, int depth)
        {
           
            String outputPath = path.Substring(path.IndexOf('.') + 1,path.LastIndexOf('.') - path.IndexOf('.') -1) + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Year.ToString() + DateTime.Now.ToFileTime().ToString();
            this.outputPath = outputPath;

            String arguments = path + " -g -r" + depth.ToString() + " -O " + outputPath;
            //DEBUG - Console.WriteLine("Running httrack with arguments: " + arguments);
            Process p = Process.Start(Directory.GetCurrentDirectory() + "/httrack/httrack.exe", arguments);
            p.WaitForExit();

            outputPath =  Directory.GetCurrentDirectory() + "\\"  +  outputPath;

            this.result = new List<String>();
            //initialize the website
            Website site = new Website(path,outputPath);

            //initialize the log
            Log log = new Log(outputPath + "\\log.txt");

            //initalize the database accessor class
            DatabaseAccessor dbAccess = null;
            int crawlID = 0;

            //Parse website
            WebsiteParser siteparser = new WebsiteParser(site, dbAccess, crawlID, log);
            result.AddRange(siteparser.analyzeSite());
            
            //Try to analyse an SSL certificate, if there is one
            CrawlerPlugin ssl = new SSLConfirmationPlugin(site, dbAccess, crawlID, log);
            result.AddRange(ssl.analyzeSite());
            
            /*
            //HTML Parser
            ParsingModule HTMLParser = new ParsingModule(site, dbAccess, crawlID, log);
            HTMLParser.analyzeSite();
            */
            log.destroy();
            if (result.Count == 0)
            {
                result.Add("No vulnerabilities detected!");
            }
        }

        public void printResults(String path)
        {
            String filepath = this.outputPath + "\\results.txt";
            System.IO.StreamWriter outfile = File.CreateText(filepath);

            outfile.WriteLine("Crawl results for " + path);

            foreach (String item in this.result)
            {
                outfile.WriteLine(item);
            }

            outfile.Close();
        }
    }
}
