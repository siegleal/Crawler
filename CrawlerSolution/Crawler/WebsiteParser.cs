using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Crawler
{
    //This class parses all of the files found and logs all of them
    class WebsiteParser : CrawlerPlugin
    {
        public WebsiteParser(Website website, DatabaseAccessor db, int crawlID, Log l)
            : base(website, db, crawlID, l)
        {
            
        }

       public override List<String> analyzeSite()
        {
            log.writeInfo("Beginning website directory parsing");
            var files = from file in Directory.EnumerateFiles(website.DirPath) select file;
            log.writeInfo("DONE");
            log.writeDebug("Beginning to list files");

            foreach (var file in files)
            {
                String filePath = file.ToString().Substring(file.LastIndexOf("\\") + 1);
                if (!filePath.Equals("log.txt"))
                {
                    log.writeInfo("Found file " + filePath);
                    website.addFile(file.ToString());
                }
            }
            log.writeDebug("DONE");
            log.writeInfo((files.Count<string>() - 1).ToString() + " files found");
            log.writeDebug("Done finding files");
            return new List<String>();
        }
    }
}
