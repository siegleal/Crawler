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

        /* Class methods */
        public crawlerPlugin(Website website, DatabaseAccessor db, int crawlID)
        {
            this.db = db;
            this.website = website;
            this.crawlID = crawlID;
        }

        public void addToDatabase(List<String> results)
        {
            foreach(String result in results)
            {
                db.addVulnerability(this.crawlID, result);
            }
        }

        /* Abstract methods
         * TODO:  Add MustOverride*/
        public List<String> analyseSite();



        

    }
}
