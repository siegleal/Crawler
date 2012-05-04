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
        void WriteStringToNewFile(string input, string filepath);
    }

    public class FileSystemInteractor : IFileSystemInteractor
    {
        private string _basePath = "";

        public void MakeDirectory(string path)
        {
            _basePath = path;
            Directory.CreateDirectory(path);
        }

        public void WriteStringToNewFile(string input,string filepath)
        {
           //create file structure
            string directoryToCreate = filepath.Substring(0, filepath.LastIndexOf("/"));

            if (!Directory.Exists(_basePath + directoryToCreate + "/"))
                Directory.CreateDirectory(_basePath + directoryToCreate + "/");
            StreamWriter fs = new StreamWriter(_basePath + "/" + filepath);
            fs.Write(input);
            fs.Close();
        }
    }

    public class WebInteractor : IWebInteractor
    {
        public string BaseUrl { get; set; }
//        private string _basePath;

        public CrawlResult GetPage(string url)
        {
            CrawlResult cr = new CrawlResult();

            int indexOfSlash = url.IndexOf('/');
            string urlExtension = "";
            if (indexOfSlash > -1)
                urlExtension = url.Substring(indexOfSlash);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://" + url);
            request.Method = "GET";
            request.AllowAutoRedirect = false;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var html = reader.ReadToEnd();

                
                cr.ReturnCode = (int) response.StatusCode;
                cr.ReturnStatus = response.StatusDescription;
                cr.Html = html;
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
        public string BasePath;
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
            //string dirPath = string.Format("{0}_{1}", _baseurl, DateTime.Now.ToString("hh-mm_MM-dd-yyyy"));
            //BasePath = dirPath;
            string dirPath = _website.DirPath;
            _fsinteractor.MakeDirectory(dirPath);
            return dirPath;
        }

        
        public string RemoveStrings(string input,string[] toRemove)
        {
            return toRemove.Aggregate(input, (current, s) => current.Replace(s, ""));
        }


        public List<CrawlResult> CrawlSite(int level)
        {
            string baseUrl = _website.url;
            List<CrawlResult> returnList = new List<CrawlResult>();
            List<string> alreadyParsed = new List<string>();
            alreadyParsed.Add("/");

            CreateRootDirectory();

            CrawlResult result = _webinteractor.GetPage(baseUrl);
            _fsinteractor.WriteStringToNewFile(result.Html,"/index.html");
            returnList.Add(result);

           
            //find patterns that match href="/xxxxxx.xxx"
            //String pattern = "href=\"/(\\w+[\\w/]*\\.\\w+)*\"";
            String pattern = "href=\"[/\\w.]*\""; //find all href's
            var relativeMatches = new List<DepthResult>();
            foreach (String str in GetMatches(pattern,result.Html))
            {
                //find internal links
                //remove href
                string trimmedString = str.Replace("href=\"", "");
                trimmedString = trimmedString.TrimEnd("\"".ToCharArray());
                
                //if it starts with http:// it might be an external link, it might not too
                bool externalFlag = trimmedString.IndexOf("http://", System.StringComparison.Ordinal) > -1;


                //trim the string from href="/xxx.xxx" => /xxx.xxx
//                string trimmedString = str.Substring(str.IndexOf('/'));
//                trimmedString = trimmedString.Substring(0, trimmedString.Length - 1);

                if (!alreadyParsed.Contains(trimmedString) && !externalFlag)
                {
                    relativeMatches.Add(new DepthResult(trimmedString.TrimEnd('/'), 1));
                    alreadyParsed.Add(trimmedString);
                }
            }

            int i = 0;
            while (i < relativeMatches.Count)
            {
                DepthResult match = relativeMatches[i];
                CrawlResult newResult = _webinteractor.GetPage(baseUrl + match.RelPath);
                _fsinteractor.WriteStringToNewFile(newResult.Html,match.RelPath);

                //look for more pages to crawl
                if (match.Level < level)
                {
                    foreach (String str in GetMatches(pattern, newResult.Html))
                    {

                        //trim the string from href="/xxx.xxx\" => /xxx.xxx
                        string trimmedString = str.Substring(str.IndexOf('/'));
                        trimmedString = trimmedString.Substring(0, trimmedString.Length - 1);

                        if (!alreadyParsed.Contains(trimmedString))
                        {
                            relativeMatches.Add(new DepthResult(trimmedString.TrimEnd('/'), match.Level + 1));
                            alreadyParsed.Add(trimmedString);
                        }
                    }
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

        private class DepthResult
        {
            public string RelPath { get; set; }

            public int Level { get; private set; }

            public DepthResult(string path,int l)
            {
                RelPath = path;
                Level = l;
            }
        
        }


        
    }
}
