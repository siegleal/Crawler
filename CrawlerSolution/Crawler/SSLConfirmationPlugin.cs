using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;

namespace Crawler
{
    class SSLConfirmationPlugin : CrawlerPlugin , PluginInterface
    {
        static List<String> result;
        static private Log logger;
        static private DateTime lastTSLRevision; 
        static private String preNextRevision = "1/8/2011";
        static private String NextRevision = "1/7/2012";
        static private String[] CertificateAuthorities;
        private Website web;
        private int crawlID;

        public SSLConfirmationPlugin(Website website, DatabaseAccessor db, int crawlID, Log l)
           : base(website, db, crawlID, l)
        {
            result = new List<String>();
            logger = l;
            web = website;
            this.crawlID = crawlID;

            /* Ensure we're not doing the effective date stuff wrong */
            if (DateTime.Now < DateTime.Parse(NextRevision))
            {
                lastTSLRevision = DateTime.Parse(preNextRevision);
            }
            else
            {
                lastTSLRevision = DateTime.Parse(NextRevision);
            }
        }

        public List<String> analyzeSite()
        {
            List<String> result = new List<String>();
            
            log.writeInfo("Beginning SSL Confirmation");

            /* Jank.  REAL jank.  Need to ensure that we have a secure page to test rather than
             * just sort of hoping there's one via adding https to the url, but... this SHOULD
             * work */
            makeRequest("https://" + web.url);

            log.writeInfo("Ending SSL Confirmation");
            return result;
        }

        /* Make the request that allows us to examine the SSL Certificate */
        private static void makeRequest(string url)
        {    
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = true;
            request.Method = "GET";

            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            finally
            {
                if (response == null)
                {
                    logger.writeInfo("There seems to be no SSL certificate");
                }
                else
                {
                    response.Close();
                }
            }
        }


        /* Add checks to the certificate here
         * This function will ALWAYS accept the certificate */
        private static bool ValidateRemoteCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors policyErrors)
        {
            bool retVal = true;

            /* Check the effective date to ensure it's after the latest TLS protocol update */
            DateTime effDate = DateTime.Parse(certificate.GetEffectiveDateString());
            logger.writeInfo("Certificate is effective on " + certificate.GetEffectiveDateString());

            if (effDate < lastTSLRevision)
            {
                result.Add("Effective Date of SSL Certificate before last TLS revision");
                logger.writeInfo("Certificate effective before most recent TLS revision");
            }

            /* Check Expiration Date */
            DateTime expDate = DateTime.Parse(certificate.GetExpirationDateString());

            if (expDate < DateTime.Now)
            {
                result.Add("SSL Certificate Expired");
                logger.writeInfo("Certificate expired");
            }
            
            /* TODO:  Check that the CA is valid */

            logger.writeInfo("Certificate Authority is " + certificate.Issuer);

            return retVal;
        }
    }
}
