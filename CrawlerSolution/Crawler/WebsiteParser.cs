using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Crawler
{
    //This class parses all of the files found and logs all of them
    class WebsiteParser : CrawlerPlugin, PluginInterface
    {
        public WebsiteParser(Website website, DatabaseAccessor db, int crawlID, Log l)
            : base(website, db, crawlID, l)
        {
            
        }

        public List<String> analyzeSite()
        {
            log.writeInfo("Beginning website directory parsing");
            var files = from file in Directory.EnumerateFiles(website.dirpath) select file;

            foreach (var file in files)
            {
                String filePath = file.ToString().Substring(file.LastIndexOf("\\") + 1);
                log.writeDebug("Found file " + filePath);
                website.addFile(filePath);
            }
            log.writeDebug(files.Count<string>().ToString() + " files found");
            return new List<String>();
        }
    }
}
