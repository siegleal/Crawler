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
        private string _basePath;

        public List<CrawlResult> ResultsList { get; set; } 

        

        public Bot(string url, int level, Website website,Log l,DatabaseAccessor dba)
        {
            _baseurl = url;
            _crawllevel = level;
            _website = website;
            _log = l;
            _dba = dba;

            ResultsList = new List<CrawlResult>();
        }

        private string CreateRootDirectory()
        {
            string result = (_baseurl + "_" +  DateTime.Now.ToString("hh-mm_MM-dd-yyyy"));
            Directory.CreateDirectory(result);
            return result;
        }

        private void CrawlPage(string urlextension)
        {
            CrawlResult cr = new CrawlResult();

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(_baseurl);
            request.Method = "GET";
            request.AllowAutoRedirect = false;

            try
            {
                //the index page
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var html = reader.ReadToEnd();

                WebClient wc = new WebClient();
                if (urlextension.Equals(""))
                    wc.DownloadFile(_baseurl + urlextension,_basePath + "\\index.html");
                else
                {
                    wc.DownloadFile(_baseurl + urlextension,_basePath + "\\" + urlextension);
                }

                cr.ReturnCode = (int) response.StatusCode;
                cr.ReturnStatus = response.StatusDescription;
                cr.html = html;



                //DEBUG INFO
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
            ResultsList.Add(cr);
        }
       

        public void CrawlSite()
        {
            //the meat of the crawler will be in here
            _basePath = CreateRootDirectory();
            _baseurl = "http://" + _baseurl;
            CrawlPage("");
            

            //return cr;

        }


    }
}
