using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Crawler
{
    public class CrawlerController
    {
        public CrawlerController(String path, int depth)
        {
            Assembly assem = Assembly.GetEntryAssembly();
            AssemblyName aName = assem.GetName();
            Version ver = aName.Version;
            Console.Out.WriteLine("Application {0}, Version {1}", aName.Name, ver.ToString());
            
            String outputPath = path.Substring(path.IndexOf('.') + 1,path.LastIndexOf('.') - path.IndexOf('.') -1) + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0') +  "-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() ;
         
            String arguments = path + " -g -r" + depth.ToString() + " -O " + outputPath;
            //DEBUG - Console.WriteLine("Running httrack with arguments: " + arguments);
            Process p = Process.Start(Directory.GetCurrentDirectory() + "/httrack/httrack.exe", arguments);
            p.WaitForExit();
            //Directory.CreateDirectory(output);

            String foldername = outputPath;
            outputPath =  Directory.GetCurrentDirectory() + "\\"  +  outputPath;

            //TESTING
            Bot b = new Bot("www.google.com/", 0, null, null, null);
            b.CrawlSite();
            
            //initialize the website

            Website site = new Website(path,outputPath);

            //initialize the log
            Log log = new Log(outputPath + "\\log.txt");
            log.writeInfo("Log created correctly");
            log.writeInfo("Website: " + path + " == CrawlLevel: " + depth.ToString());
            log.writeInfo("Running version: " + aName.Version.ToString());

            //initalize the database accessor class
            log.writeDebug("Creating database object");
            DatabaseAccessor dbAccess = null;//new DatabaseAccessor(log);
            //dbAccess.addWebsite(path, null, null, null);
            int crawlID = 0;// dbAccess.newCrawl(path, "example@gmail.com");

            //Parse website
            WebsiteParser siteparser = new WebsiteParser(site, dbAccess, crawlID, log);
            siteparser.analyzeSite();
            
            //Try to analyse an SSL certificate, if there is one
            log.writeDebug("Creating SSL object");
            CrawlerPlugin ssl = new SSLConfirmationPlugin(site, dbAccess, crawlID, log);
            ssl.analyzeSite();

            //Get headers
            CrawlerPlugin headers = new HtmlHeaderPlugin(site, dbAccess, crawlID, log);
            headers.analyzeSite();
            
            
            //HTML Parser
            log.writeDebug("Creating HTML parsing object");
            HTMLParsingModule HTMLParser = new HTMLParsingModule(site, dbAccess, crawlID, log);
            HTMLParser.analyzeSite();
            log.writeDebug("Done parsing HTML");

            //notify
            log.writeDebug("Preparing to send message");
            //NotifyClient.sendMessage();
            log.writeDebug("Done sending notification");
            
            log.writeDebug("Destroying log....program exiting");
            log.destroy();
        }
    }
}
