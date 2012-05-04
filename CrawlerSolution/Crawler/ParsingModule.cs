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
            var file = new System.IO.StreamReader("Vuln.txt");
            String line;
            while ((line = file.ReadLine()) != null)
            {
                string[] splitLine = line.Split(new char[] {'\t'});
                splitLine = splitLine.Where(x => x != "").ToArray();
                if (splitLine.Length >= 2)
                {
                    searchStringList.Add(splitLine[0]);
                    descriptionList.Add(splitLine[1]);
                }
                else
                {
                    log.writeError("Unknown vulnerability definition: " + line);
                }
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
