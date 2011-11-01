using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SHDocVw;

namespace CSharpCrawler
{
    public static class Crawler
    {
        #region Private Fields

        private static List<Page> _pages = new List<Page>();
        private static List<string> _externalUrls = new List<string>();
        private static List<string> _otherUrls = new List<string>();
        private static List<string> _failedUrls = new List<string>();
        private static List<string> _exceptions = new List<string>();
        private static List<string> _classes = new List<string>();
        private static StringBuilder _logBuffer = new StringBuilder();
        private static int depth = 0;
        private static int maxDepth = 10;

        #endregion



        /// <summary>
        /// Crawls a site.
        /// </summary>
        public static void CrawlSite()
        {
            Console.WriteLine("Beginning crawl.");

            CrawlPage(ConfigurationManager.AppSettings["url"]);

            StringBuilder sb = CreateReport();

            WriteReportToDisk(sb.ToString());

            OpenReportInIE();

            Console.WriteLine("Finished crawl.");

        }




        /// <summary>
        /// Crawls a page.
        /// </summary>
        /// <param name="url">The url to crawl.</param>
        private static void CrawlPage(string url)
        {
            if (depth == maxDepth)
            {
                return;
            }
            if (!PageHasBeenCrawled(url))
            {
                string htmlText = GetWebText(url);

                Page page = new Page();
                page.Text = htmlText;
                page.Url = url;
                page.CalculateViewstateSize();
                
                _pages.Add(page);

                LinkParser linkParser = new LinkParser();
                linkParser.ParseLinks(page, url);

                CSSClassParser classParser = new CSSClassParser();
                classParser.ParseForCssClasses(page);


                //Add data to main data lists
                AddRangeButNoDuplicates(_externalUrls, linkParser.ExternalUrls);
                AddRangeButNoDuplicates(_otherUrls, linkParser.OtherUrls);
                AddRangeButNoDuplicates(_failedUrls, linkParser.BadUrls);
                AddRangeButNoDuplicates(_classes, classParser.Classes);

                foreach (string exception in linkParser.Exceptions)
                    _exceptions.Add(exception);


                //Crawl all the links found on the page.
                foreach (string link in linkParser.GoodUrls)
                {
                    string formattedLink = link;
                    try
                    {

                        formattedLink = FixPath(url, formattedLink);

                        if (formattedLink != String.Empty)
                        {
                            depth++;
                            CrawlPage(formattedLink);
                            depth--;
                        }
                    }
                    catch (Exception exc)
                    {
                        _failedUrls.Add(formattedLink + " (on page at url " + url + ") - " + exc.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Fixes a path. Makes sure it is a fully functional absolute url.
        /// </summary>
        /// <param name="originatingUrl">The url that the link was found in.</param>
        /// <param name="link">The link to be fixed up.</param>
        /// <returns>A fixed url that is fit to be fetched.</returns>
        public static string FixPath(string originatingUrl, string link)
        {
            string formattedLink = String.Empty;

            if (link.IndexOf("../") > -1)
            {
                formattedLink = ResolveRelativePaths(link, originatingUrl);
            }
           /* else if (originatingUrl.IndexOf(ConfigurationManager.AppSettings["url"].ToString()) > -1
                && link.IndexOf(ConfigurationManager.AppSettings["url"].ToString()) == -1)
            {
                formattedLink = originatingUrl.Substring(0, originatingUrl.LastIndexOf("/") + 1) + link;
            }
            * */
            else if (link.IndexOf(ConfigurationManager.AppSettings["url"].ToString()) == -1)
            {
                formattedLink = ConfigurationManager.AppSettings["url"].ToString() + link;
            }

            return formattedLink;
        }

        /// <summary>
        /// Needed a method to turn a relative path into an absolute path. And this seems to work.
        /// </summary>
        /// <param name="relativeUrl">The relative url.</param>
        /// <param name="originatingUrl">The url that contained the relative url.</param>
        /// <returns>A url that was relative but is now absolute.</returns>
        private static string ResolveRelativePaths(string relativeUrl, string originatingUrl)
        {
            string resolvedUrl = String.Empty;

            string[] relativeUrlArray = relativeUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string[] originatingUrlElements = originatingUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            int indexOfFirstNonRelativePathElement = 0;
            for(int i = 0; i <= relativeUrlArray.Length - 1; i++)
            {
                if (relativeUrlArray[i] != "..")
                {
                    indexOfFirstNonRelativePathElement = i;
                    break;
                }
            }

            int countOfOriginatingUrlElementsToUse = originatingUrlElements.Length - indexOfFirstNonRelativePathElement - 1;
            for (int i = 0; i <= countOfOriginatingUrlElementsToUse - 1; i++)
            {
                if (originatingUrlElements[i] == "http:" || originatingUrlElements[i] == "https:")
                    resolvedUrl += originatingUrlElements[i] + "//";
                else
                    resolvedUrl += originatingUrlElements[i] + "/";
            }

            for (int i = 0; i <= relativeUrlArray.Length - 1; i++)
            {
                if (i >= indexOfFirstNonRelativePathElement)
                {
                    resolvedUrl += relativeUrlArray[i];

                    if (i < relativeUrlArray.Length - 1)
                        resolvedUrl += "/";
                }
            }

            return resolvedUrl;
        }

        /// <summary>
        /// Checks to see if the page has been crawled.
        /// </summary>
        /// <param name="url">The url that has potentially been crawled.</param>
        /// <returns>Boolean indicating whether or not the page has been crawled.</returns>
        private static bool PageHasBeenCrawled(string url)
        {
            foreach (Page page in _pages)
            {
                if (page.Url == url)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Merges a two lists of strings.
        /// </summary>
        /// <param name="targetList">The list into which to merge.</param>
        /// <param name="sourceList">The list whose values need to be merged.</param>
        private static void AddRangeButNoDuplicates(List<string> targetList, List<string> sourceList)
        {
            foreach (string str in sourceList)
            {
                if (!targetList.Contains(str))
                    targetList.Add(str);
            }
        }

        /// <summary>
        /// Gets the response text for a given url.
        /// </summary>
        /// <param name="url">The url whose text needs to be fetched.</param>
        /// <returns>The text of the response.</returns>
        private static string GetWebText(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.UserAgent = "A .NET Web Crawler";

            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();

            StreamReader reader = new StreamReader(stream);
            string htmlText = reader.ReadToEnd();
            return htmlText;
        }



        #region Logging and Reporting

        private static void WriteReportToDisk(string contents)
        {
            string fileName = ConfigurationManager.AppSettings["logTextFileName"].ToString();
            FileStream fStream = null;
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
                fStream = File.Create(fileName);
            }
            else
            {
                fStream = File.OpenWrite(fileName);
            }

            using (TextWriter writer = new StreamWriter(fStream))
            {
                writer.WriteLine(contents);
                writer.Flush();
            }

            fStream.Dispose();
        }

        /// <summary>
        /// Creates a report out of the data gathered.
        /// </summary>
        /// <returns></returns>
        private static StringBuilder CreateReport()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<html><head><title>Crawl Report</title><style>");
            sb.Append("table { border: solid 3px black; border-collapse: collapse; }");
            sb.Append("table tr th { font-weight: bold; padding: 3px; padding-left: 10px; padding-right: 10px; }");
            sb.Append("table tr td { border: solid 1px black; padding: 3px;}");
            sb.Append("h1, h2, p { font-family: Rockwell; }");
            sb.Append("p { font-family: Rockwell; font-size: smaller; }");
            sb.Append("h2 { margin-top: 45px; }");
            sb.Append("</style></head><body>");
            sb.Append("<h1>Crawl Report</h1>");

            sb.Append("<h2>Internal Urls - In Order Crawled</h2>");
            sb.Append("<p>These are the pages found within the site. The size is calculated by getting value of the Length of the text of the response text. This is the order in which they were crawled.</p>");

            sb.Append("<table><tr><th>Page Size</th><th>Viewstate Size</th><th>Url</th></tr>");

            foreach (Page page in _pages)
            {
                sb.Append("<tr><td>");
                sb.Append(page.Size.ToString());
                sb.Append("</td><td>");
                sb.Append(page.ViewstateSize.ToString());
                sb.Append("</td><td>");
                sb.Append(page.Url);
                sb.Append("</td></tr>");
            }

            sb.Append("</table>");


            sb.Append("<h2>Internal Urls - In Order of Size</h2>");
            sb.Append("<p>These are the pages found within the site. The size is calculated by getting value of the Length of the text of the response text. This is the order in terms of total page size.</p>");

            sb.Append("<table><tr><th>Page Size</th><th>Viewstate Size</th><th>Url</th></tr>");

            List<Page> sortedList = new List<Page>();
            foreach (Page page in _pages)
            {
                if (sortedList.Count == 0)
                {
                    sortedList.Add(page);
                }
                else
                {
                    for (int i = 0; i <= sortedList.Count - 1; i++)
                    {
                        Page sortedPage = sortedList[i];

                        if (sortedPage.Size > page.Size)
                        {
                            sortedList.Insert(i, page);
                            break;
                        }
                        else if (i == sortedList.Count - 1)
                        {
                            sortedList.Add(page);
                            break;
                        }
                    }
                }
            }

            for (int i = sortedList.Count - 1; i >= 0; i--)
            {
                Page page = sortedList[i];

                sb.Append("<tr><td>");
                sb.Append(page.Size.ToString());
                sb.Append("</td><td>");
                sb.Append(page.ViewstateSize.ToString());
                sb.Append("</td><td>");
                sb.Append(page.Url);
                sb.Append("</td></tr>");
            }
            sb.Append("</table>");


            sb.Append("<h2>External Urls</h2>");
            sb.Append("<p>These are the links to the pages outside the site.</p>");

            sb.Append("<table><tr><th>Url</th></tr>");

            foreach (string str in _externalUrls)
            {
                sb.Append("<tr><td>");
                sb.Append(str);
                sb.Append("</td></tr>");
            }

            sb.Append("</table>");

            sb.Append("<h2>Other Urls</h2>");
            sb.Append("<p>These are the links to things on the site that are not html files (html, aspx, etc.), like images and css files.</p>");

            sb.Append("<table><tr><th>Url</th></tr>");

            foreach (string str in _otherUrls)
            {
                sb.Append("<tr><td>");
                sb.Append(str);
                sb.Append("</td></tr>");
            }

            sb.Append("</table>");

            sb.Append("<h2>Bad Urls</h2>");
            sb.Append("<p>Any bad urls will be listed here.</p>");

            sb.Append("<table><tr><th>Url</th></tr>");

            if (_failedUrls.Count > 0)
            {
                foreach (string str in _failedUrls)
                {
                    sb.Append("<tr><td>");
                    sb.Append(str);
                    sb.Append("</td></tr>");
                }
            }
            else
            {
                sb.Append("<tr><td>No bad urls.</td></tr>");
            }

            sb.Append("</table>");


            sb.Append("<h2>Exceptions</h2>");
            sb.Append("<p>Any exceptions that were thrown would be shown here.</p>");

            sb.Append("<table><tr><th>Exception</th></tr>");

            if (_exceptions.Count > 0)
            {
                foreach (string str in _exceptions)
                {
                    sb.Append("<tr><td>");
                    sb.Append(str);
                    sb.Append("</td></tr>");
                }
            }
            else
            {
                sb.Append("<tr><td>No exceptions thrown.</td></tr>");
            }

            sb.Append("</table>");

            sb.Append("<h2>Css Classes</h2>");
            sb.Append("<p>These are the css classes used on the site, just in case you want to know which ones you're using and compare that with your css...</p>");

            sb.Append("<table><tr><th>Class</th></tr>");

            if (_classes.Count > 0)
            {
                foreach (string str in _classes)
                {
                    sb.Append("<tr><td>");
                    sb.Append(str);
                    sb.Append("</td></tr>");
                }
            }
            else
            {
                sb.Append("<tr><td>No classes found.</td></tr>");
            }

            sb.Append("</table>");

            sb.Append("</body></html>");
            return sb;
        }

        private static void OpenReportInIE()
        {
            object o = new object();
            InternetExplorer ie = new InternetExplorerClass();
            IWebBrowserApp wb = (IWebBrowserApp)ie;
            wb.Visible = true;

            wb.Navigate(ConfigurationManager.AppSettings["logTextFileName"].ToString(), ref o, ref o, ref o, ref o);
        }

        #endregion
    }
}
