using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;

namespace Crawler
{
    public interface IFileSystemInteractor
    {
        void MakeDirectory(string path);
    }

    public class FileSystemInteractor : IFileSystemInteractor
    {
        public void MakeDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
    }

    public class WebInteractor : IWebInteractor
    {
        private string _baseurl;
        private string _basePath;

        public CrawlResult GetPage(string url)
        {
            CrawlResult cr = new CrawlResult();
            string urlExtension = url.TrimStart(_baseurl.ToCharArray());

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
                if (urlExtension.Equals(""))
                    wc.DownloadFile(_baseurl + urlExtension,_basePath + "\\index.html");
                else
                {
                    wc.DownloadFile(_baseurl + urlExtension,_basePath + "\\" + urlExtension);
                }

                cr.ReturnCode = (int) response.StatusCode;
                cr.ReturnStatus = response.StatusDescription;
                cr.Html = html;



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

            return cr;
        }

       
    }
    public class CrawlResult
    {
        public int ReturnCode { get; set; }
        public string ReturnStatus { get; set; }
        public string Html { get; set; }

    }
    //The bot class will be given a site and a crawl level
    //It will begin by looking at the index.html file and finding all links
    //in that file and repeat up to crawl level
    //
    public class Bot
    {
        private String _baseurl;
        private Website _website;
        private Log _log;
        private DatabaseAccessor _dba;
        private string _basePath;
        private IWebInteractor _webinteractor;
        private IFileSystemInteractor _fsinteractor;
        public List<CrawlResult> ResultsList { get; set; } 

        


        public Bot(Website website, Log l,DatabaseAccessor dba, IWebInteractor wi, IFileSystemInteractor fsi)
        {
            _baseurl = website.url;
            _website = website;
            _log = l;
            _dba = dba;
            _webinteractor = wi;
            _fsinteractor = fsi;

            ResultsList = new List<CrawlResult>();
        }

        private string CreateRootDirectory()
        {
            string dirPath = string.Format("{0}_{1}", _baseurl, DateTime.Now.ToString("hh-mm_MM-dd-yyyy"));
            _fsinteractor.MakeDirectory(dirPath);
            return dirPath;
        }

        
       


        public List<CrawlResult> CrawlSite(int level)
        {
            string baseUrl = _website.url;
            List<CrawlResult> returnList = new List<CrawlResult>();

            CrawlResult result = _webinteractor.GetPage(baseUrl);
            returnList.Add(result);

           
            string pattern = @"/\w+[\w/]*\.\w+";
            List<String> relativeMatches = new List<string>();
            foreach (String str in GetMatches(pattern,result.Html))
            {
                relativeMatches.Add(str);
            }

            int i = 0;
            while (i < relativeMatches.Count)
            {
                string match = relativeMatches[i];
                CrawlResult newResult = _webinteractor.GetPage(baseUrl + match);

                foreach (String str in GetMatches(pattern, newResult.Html))
                {
                    relativeMatches.Add(str);
                }
                returnList.Add(newResult);
                i++;
            }



            return returnList;

        } 

        private List<String> GetMatches(string pattern, string toSearch)
        {
            List<String> returnList = new List<string>();
            Regex rgx = new Regex(pattern,RegexOptions.IgnoreCase);
            MatchCollection relativeMatches = rgx.Matches(toSearch);
            foreach (Match m in relativeMatches)
            {
                returnList.Add(m.Value);
            }

            return returnList;

        }


        
    }
}
