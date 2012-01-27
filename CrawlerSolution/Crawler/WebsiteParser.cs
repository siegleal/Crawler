using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Crawler
{
    class WebsiteParser : PluginInterface
    {
        public List<String> analyzeSite(Website web, DatabaseAccessor db, int crawlID, Log log)
        {
            log.writeInfo("Beginning website directory parsing");
            var files = from file in Directory.EnumerateFiles(web.dirpath + "\\" + web.url) select file;

            foreach (var file in files)
            {
                log.writeDebug("Found file " + file.ToString().Substring(file.LastIndexOf("\\") + 1));
            }
            log.writeDebug(files.Count<string>().ToString() + " files found");
            return new List<String>();
        }

    }
}
