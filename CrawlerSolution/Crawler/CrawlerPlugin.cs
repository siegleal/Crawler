using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
    public abstract class CrawlerPlugin
    {
        protected DatabaseAccessor db;
        protected Website website;
        protected int crawlID;
        protected Log log;

        /* Class methods */
        public CrawlerPlugin(Website website, DatabaseAccessor db, int crawlID, Log l)
        {
            this.db = db;
            this.website = website;
            this.crawlID = crawlID;
            this.log = l;
        }

        protected void addToDatabase(List<String> results)
        {
            db.AddVulnerabilities(this.crawlID, results);
        }

        /* Abstract methods
         * TODO:  Add MustOverride*/
        public abstract List<String> analyzeSite();

              

    }
}
