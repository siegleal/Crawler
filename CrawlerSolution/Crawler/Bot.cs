using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
    //The bot class will be given a site and a crawl level
    //It will begin by looking at the index.html file and finding all links
    //in that file and repeat up to crawl level
    //
    class Bot
    {
        private String _url;
        private int _crawllevel;
        private Website _website;
        private Log _log;
        private DatabaseAccessor _dba;

        public Bot(string url, int level, Website website,Log l,DatabaseAccessor dba)
        {
            _url = url;
            _crawllevel = level;
            _website = website;
            _log = l;
            _dba = dba;
        }

        private void CreateDirectory()
        {
            
        }

        private void CrawlSite()
        {
            //the meat of the crawler will be in here
            
        }


    }
}
