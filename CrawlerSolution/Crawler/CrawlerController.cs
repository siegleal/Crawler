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
            
            //String outputPath = path.Substring(path.IndexOf('.') + 1,path.LastIndexOf('.') - path.IndexOf('.') -1) + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0') +  "-" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() ;
            string outputPath = string.Format("{0}_{1}", path, DateTime.Now.ToString("hh-mm_MM-dd-yyyy"));
            
         
            String arguments = path + " -g -r" + depth.ToString() + " -O " + outputPath;
            //DEBUG - Console.WriteLine("Running httrack with arguments: " + arguments);
            //Process p = Process.Start(Directory.GetCurrentDirectory() + "/httrack/httrack.exe", arguments);
            //p.WaitForExit();
            //Directory.CreateDirectory(output);

            String foldername = outputPath;
            outputPath =  Directory.GetCurrentDirectory() + "\\"  +  outputPath;
            Directory.CreateDirectory(outputPath);

            //initialize the website
            Website site = new Website(path,outputPath);

            //initialize the log
            Log log = new Log(outputPath + "\\log.txt");
            log.writeInfo("Log created correctly");
            log.writeInfo("Website: " + path + " == CrawlLevel: " + depth.ToString());
            log.writeInfo("Running version: " + aName.Version.ToString());

            //initalize the database accessor class
            log.writeDebug("Creating database object");
            DatabaseAccessor dbAccess = null;
            try
            {
                dbAccess = new DatabaseAccessor(log, ConfigReader.ReadDatabaseAccessorString());
            }
            catch(Exception e)
            {
                Console.Out.WriteLine("Error creating database connection: " + e.Message);
                Console.Out.WriteLine("Reverting to default CrawlID");
                log.writeError("Error creating database connection: " + e.Message);
                log.writeError("Reverting to default CrawlID");
            }
            if (dbAccess != null) dbAccess.addWebsite(path, null, null, null);

            int crawlID;
            if(dbAccess != null)
                crawlID = dbAccess.newCrawl(path, "example@gmail.com");
            else
            {
                crawlID = 0;
            }

            var fsi = new FileSystemInteractor();
            Bot b = new Bot(site, log, null, new WebInteractor(log), fsi);
            b.CrawlSite(depth);

            //Parse website
            WebsiteParser siteparser = new WebsiteParser(site, dbAccess, crawlID, log,fsi);
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
            try
            {
                //NotifyClient.sendMessage(ConfigReader.ReadEmailAddress());
            }
            catch(Exception e)
            {
                Console.Out.WriteLine("Error in Notify client: " + e.Message);
                log.writeError("Error in Notify client: " + e.Message);
            }
            log.writeDebug("Done sending notification");
            
            log.writeDebug("Destroying log....program exiting");
            log.destroy();
        }
    }
}
