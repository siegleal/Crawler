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
                try
                {
                    var serverType = response.Headers.Get("Server");
                    logger.writeInfo("Server type is: " + serverType);
                }
                catch (Exception e)
                {
                    logger.writeError("HTTP Response did not have 'Server' field");
                } 
            }
            catch(Exception e)
            {
                logger.writeError("Could not get response from server.  Cancelling header analysis");
            }
         

            return result;
        }
    }
}
