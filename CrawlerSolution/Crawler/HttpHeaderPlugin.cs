using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Crawler
{
    class HttpHeaderPlugin : CrawlerPlugin
    {
        private Website website;
        private DatabaseAccessor db;
        private int crawlID;
        private Log logger;

        public HttpHeaderPlugin(Website website, DatabaseAccessor db, int crawlID, Log l) : base(website, db, crawlID, l)
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
                result.Add("Server type is: " + serverType);
                logger.writeInfo("Server type is: " + serverType);

                var warningField = response.Headers.Get("Warning");
                result.Add("Warning field contains: " + warningField);
                logger.writeInfo("Warning field contains: " + warningField);

                logger.writeInfo("Checking Non-standard headers for technology information...");
                
                var poweredBy = response.Headers.Get("X-Powered-By");
                result.Add("X-Powered-By field is: " + poweredBy);
                logger.writeInfo("X-Powered-By field is: " + poweredBy);

                var xversion = response.Headers.Get("X-Version");
                result.Add("X-Version field is: " + xversion);
                logger.writeInfo("X-Version field is: " + xversion);

                var xruntime = response.Headers.Get("X-Runtime");
                result.Add("X-Runtime field is: " + xruntime);
                logger.writeInfo("X-Runtime field is: " + xruntime);

                var xasp = response.Headers.Get("X-AspNet-Version");
                result.Add("X-AspNet-Version field is: " + xasp);
                logger.writeInfo("X-AspNet-Version field is: " + xasp);

                var xssprot = response.Headers.Get("X-XSS-Protection");
                result.Add("X-XSS-Protection field is: " + xssprot);
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
