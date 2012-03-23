using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Crawler
{
    class HtmlHeaderPlugin : CrawlerPlugin
    {
        private Website website;
        private DatabaseAccessor db;
        private int crawlID;
        private Log logger;

        public HtmlHeaderPlugin(Website website, DatabaseAccessor db, int crawlID, Log l) : base(website, db, crawlID, l)
        {
            this.website = website;
            this.db = db;
            this.crawlID = crawlID;
            this.logger = l;
        }

        public override List<string> analyzeSite()
        {
            logger.writeDebug("Starting header analysis...");
            var result = new List<string>();

            var request = (HttpWebRequest)WebRequest.Create("http://" + website.url);
            request.AllowAutoRedirect = true;
            request.Method = "GET";

            
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var serverType = response.Headers.Get("Server");
                logger.writeInfo("Server type is: " + serverType);

                var warningField = response.Headers.Get("Warning");
                logger.writeInfo("Warning field contains: " + warningField);

                logger.writeInfo("Checking Non-standard headers for technology information...");
                
                var poweredBy = response.Headers.Get("X-Powered-By");
                logger.writeInfo("X-Powered-By field is: " + poweredBy);

                var xversion = response.Headers.Get("X-Version");
                logger.writeInfo("X-Version field is: " + xversion);

                var xruntime = response.Headers.Get("X-Runtime");
                logger.writeInfo("X-Runtime field is: " + xruntime);

                var xasp = response.Headers.Get("X-AspNet-Version");
                logger.writeInfo("X-AspNet-Version field is: " + xasp);

                var xssprot = response.Headers.Get("X-XSS-Protection");
                logger.writeInfo("X-XSS-Protection field is: " + xssprot);

            }
            catch(Exception e)
            {
                logger.writeError("Could not get response from server.  Cancelling header analysis");
            }
         

            return result;
        }
    }
}
