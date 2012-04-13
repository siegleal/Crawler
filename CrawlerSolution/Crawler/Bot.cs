using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Net;

namespace Crawler
{
    class CrawlResult
    {
        public int ReturnCode { get; set; }
        public string ReturnStatus { get; set; }
        public string html { get; set; }

    }
    //The bot class will be given a site and a crawl level
    //It will begin by looking at the index.html file and finding all links
    //in that file and repeat up to crawl level
    //
    class Bot
    {
        private String _baseurl;
        private int _crawllevel;
        private Website _website;
        private Log _log;
        private DatabaseAccessor _dba;

        

        public Bot(string url, int level, Website website,Log l,DatabaseAccessor dba)
        {
            _baseurl = url;
            _crawllevel = level;
            _website = website;
            _log = l;
            _dba = dba;
        }

        private string CreateRootDirectory()
        {
            string result = (_baseurl + "_" + DateTime.Now.ToString("hh-mm_MM-dd-yyyy"));
            Directory.CreateDirectory(result);
            return result;
        }

       

        public CrawlResult CrawlSite()
        {
            //the meat of the crawler will be in here
            string basePath = CreateRootDirectory();
            CrawlResult cr = new CrawlResult();

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://" + _baseurl);
            request.Method = "GET";
            request.AllowAutoRedirect = false;

            try
            {

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var html = reader.ReadToEnd();

                WebClient wc = new WebClient();
                wc.DownloadFile("http://" + _baseurl,basePath +"/testfile.html");

                cr.ReturnCode = (int) response.StatusCode;
                cr.ReturnStatus = response.StatusDescription;
                cr.html = html;




                Console.Out.WriteLine(@"HTTP Version: {0}", response.ProtocolVersion);
                Console.Out.WriteLine(@"Status code: {0}", (int)response.StatusCode);
                Console.Out.WriteLine(@"Status description: {0}", response.StatusDescription);
                Console.Out.WriteLine(@"ResponseUri: {0}", response.ResponseUri);

                //Console.Out.WriteLine(@"html: \n{0}", html);
            }
            catch (WebException we)
            {
                var code = ((HttpWebResponse)we.Response).StatusCode;
                cr.ReturnCode = (int) code;
                cr.ReturnStatus = code.ToString();
                Console.Out.WriteLine(@"Error: {0} ({1}) {2}",((HttpWebResponse)we.Response).ResponseUri, (int)code, code);

            }

            return cr;

        }


    }
}
