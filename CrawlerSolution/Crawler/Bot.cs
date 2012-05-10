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
        string MakeFilesystemGraph();
    }

    public class FileSystemInteractor : IFileSystemInteractor
    {
        private string _basePath = "";

        public string BasePath()
        {
            return _basePath;
        }

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

        public string MakeFilesystemGraph()
        {
            var sb = new StringBuilder();
            
            FilesystemPathHelper(_basePath,sb,"");

            return sb.ToString();
        }

        private void FilesystemPathHelper(string path, StringBuilder sb, string prefix)
        {
            const string INDENT = "-----";
            //list all of the files
            foreach (var file in Directory.GetFiles(path))
            {
                sb.Append(prefix + file.Replace(path,"") + "\r\n");
            }

            //recursively look into directories
            foreach (var directory in Directory.GetDirectories(path))
            {
                sb.Append(directory.Replace(path, "") + "\r\n");
                FilesystemPathHelper(directory, sb, prefix + INDENT);
            }
        }
    }

    public class WebInteractor : IWebInteractor
    {
        private Log _log;
        public WebInteractor(Log l):base()
        {
            _log = l;
        }
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
                _log.writeError(String.Format(@"Error: {0} ({1}) {2}",((HttpWebResponse)we.Response).ResponseUri, (int)code, code));

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

            String pattern = "href=\"(http://)*[/\\w.\\\\]*\""; //find all href's
            var relativeMatches = new List<DepthResult>();
            foreach (String str in GetMatches(pattern,result.Html))
            {
                FindMoreMatches(str,alreadyParsed,relativeMatches,0);
            }

            int i = 0;
            while (i < relativeMatches.Count)
            {
                DepthResult match = relativeMatches[i];
                CrawlResult newResult = _webinteractor.GetPage(baseUrl + match.RelPath);
                if (match.RelPath.IndexOf(".") == -1)
                {
                    if (match.RelPath[match.RelPath.Length - 1] == '/')
                        match.RelPath += "index.html";
                    else
                    {
                        match.RelPath += "index.html";
                    }
                }
                _fsinteractor.WriteStringToNewFile(newResult.Html,match.RelPath);

                //look for more pages to crawl
                if (match.Level < level)
                {
                    foreach (String str in GetMatches(pattern, newResult.Html))
                    {
                        FindMoreMatches(str,alreadyParsed,relativeMatches, match.Level);
                    }
                }
                returnList.Add(newResult);
                i++;
            }

            return returnList;

        }

        private void FindMoreMatches(string str,List<string> alreadyParsed,List<DepthResult> relativeMatches, int level  )
        {

            //find internal links
            //remove href
            string trimmedString = RemoveStrings(str, new string[] { "href=\"", "\"" });


            bool externalFlag = false;

            if (trimmedString.IndexOf(@"http://") > -1)
            {
                trimmedString = RemoveStrings(trimmedString, new string[] { "http://" });
                if (trimmedString.IndexOf(_baseurl) == -1) //baseUrl not found in link address
                {
                    if ((@"www." + trimmedString).IndexOf(_baseurl) == -1) //baseUrl not found if "www." added to the front of the string
                    {
                        externalFlag = true;
                    }
                    else
                    {
                        trimmedString = "www." + trimmedString;
                        trimmedString = RemoveStrings(trimmedString, new string[] { _baseurl });
                    }
                }
                else
                {
                    trimmedString = RemoveStrings(trimmedString, new string[] { _baseurl });
                }
            }


            //trim the string from href="/xxx.xxx" => /xxx.xxx

            if (!alreadyParsed.Contains(trimmedString) && !externalFlag)
            {
                relativeMatches.Add(new DepthResult(trimmedString, level+ 1));
                alreadyParsed.Add(trimmedString);
            }
        }

        private List<String> GetMatches(string pattern, string toSearch)
        {
            List<String> returnList = new List<string>();
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
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

            public DepthResult(string path, int l)
            {
                RelPath = path;
                Level = l;
            }

        }



    }
}
