using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
    public abstract class crawlerPlugin
    {
        private DatabaseAccessor db;
        private Website website;
        private int crawlID;
        private Log log;

        /* Class methods */
        /*public abstract crawlerPlugin(Website website, DatabaseAccessor db, int crawlID, Log l)
        {
            this.db = db;
            this.website = website;
            this.crawlID = crawlID;
            this.log = l;
        }

        protected void addToDatabase(List<String> results)
        {
            foreach(String result in results)
            {
                db.addVulnerability(this.crawlID, result);
            }
        }

        /* Abstract methods
         * TODO:  Add MustOverride*/
        //public abstract List<String> analyseSite();

        

        

    }
}
