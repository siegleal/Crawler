using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
namespace Crawler
{
    /// <summary>
    /// Class: HTMLParseingModule
    /// Description:
    /// Changes in progress...
    /// </summary>
    class HTMLParsingModule: CrawlerPlugin
    {
        private List<String> searchStringList;
        private List<String> descriptionList;
        public HTMLParsingModule(Website w, DatabaseAccessor db, int id, Log log)
            : base(w, db, id, log)
        {
            searchStringList = new List<String>();
            descriptionList = new List<String>();
        }

        public void loadDefinitionsFromFile()
        {
            System.IO.StreamReader file = new System.IO.StreamReader("Vuln.txt");
            String line;
            int i = 0;
            while ((line = file.ReadLine()) != null)
            {
                //int s = line.IndexOf("\t");
                //searchStringList.Add(line.Substring(0, s));
                //descriptionList.Add(line.Substring(s+1, line.Length-s-1));
                //i++;
            }
        }

        public override List<String> analyzeSite()
        {
            loadDefinitionsFromFile();
            List<String> files = website.getFilesFound();
            List<String> report = new List<string>();
            var matchStrategy = new StringMatchStrategy(searchStringList, descriptionList, files);
            report.AddRange(matchStrategy.Parse());

            foreach(var line in report)
            {
                log.writeUser(line, "VULN");
            }
            return report;
        }
    }
}
