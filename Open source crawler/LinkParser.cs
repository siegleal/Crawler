using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;

namespace CSharpCrawler
{
    public class LinkParser
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LinkParser() { }

        #endregion
        #region Constants

        private const string _LINK_REGEX = "href=\"[a-zA-Z./:&\\d_-]+\"";

        #endregion
        #region Private Instance Fields

        private List<string> _goodUrls = new List<string>();
        private List<string> _badUrls = new List<string>();
        private List<string> _otherUrls = new List<string>();
        private List<string> _externalUrls = new List<string>();
        private List<string> _exceptions = new List<string>();

        #endregion
        #region Public Properties

        public List<string> GoodUrls
        {
            get { return _goodUrls; }
            set { _goodUrls = value; }
        }

        public List<string> BadUrls
        {
            get { return _badUrls; }
            set { _badUrls = value; }
        }

        public List<string> OtherUrls
        {
            get { return _otherUrls; }
            set { _otherUrls = value; }
        }

        public List<string> ExternalUrls
        {
            get { return _externalUrls; }
            set { _externalUrls = value; }
        }

        public List<string> Exceptions
        {
            get { return _exceptions; }
            set { _exceptions = value; }
        }

        #endregion

        /// <summary>
        /// Parses a page looking for links.
        /// </summary>
        /// <param name="page">The page whose text is to be parsed.</param>
        /// <param name="sourceUrl">The source url of the page.</param>
        public void ParseLinks(Page page, string sourceUrl)
        {
            MatchCollection matches = Regex.Matches(page.Text, _LINK_REGEX);

            for (int i = 0; i <= matches.Count - 1; i++)
            {
                Match anchorMatch = matches[i];

                if (anchorMatch.Value == String.Empty)
                {
                    BadUrls.Add("Blank url value on page " + sourceUrl);
                    continue;
                }

                string foundHref = null;
                try
                {
                    foundHref = anchorMatch.Value.Replace("href=\"", "");
                    foundHref = foundHref.Substring(0, foundHref.IndexOf("\""));
                }
                catch (Exception exc)
                {
                    Exceptions.Add("Error parsing matched href: " + exc.Message);
                }


                if (!GoodUrls.Contains(foundHref))
                {
                    if (IsExternalUrl(foundHref))
                    {
                        _externalUrls.Add(foundHref);
                    }
                    else if (!IsAWebPage(foundHref))
                    {
                        foundHref = Crawler.FixPath(sourceUrl, foundHref);
                        _otherUrls.Add(foundHref);
                    }
                    else
                    {
                        GoodUrls.Add(foundHref);
                    }
                }
            }
        }


        /// <summary>
        /// Is the url to an external site?
        /// </summary>
        /// <param name="url">The url whose externality of destination is in question.</param>
        /// <returns>Boolean indicating whether or not the url is to an external destination.</returns>
        private static bool IsExternalUrl(string url)
        {
            if (url.IndexOf(ConfigurationManager.AppSettings["authority"]) > -1)
            {
                return false;
            }
            else if (url.Equals("/"))
            {
                return true;
            }
            else if (url.Length < 7 && url.Substring(0, 3) != "www")
            {
                return true;
            }
            else if (url.Substring(0, 7) == "http://" || url.Substring(0, 3) == "www" || url.Substring(0, 7) == "https://")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Is the value of the href pointing to a web page?
        /// </summary>
        /// <param name="foundHref">The value of the href that needs to be interogated.</param>
        /// <returns>Boolen </returns>
        private static bool IsAWebPage(string foundHref)
        {
            if (foundHref.IndexOf("javascript:") == 0)
                return false;

            string extension = foundHref.Substring(foundHref.LastIndexOf(".") + 1, foundHref.Length - foundHref.LastIndexOf(".") - 1);
            switch (extension)
            {
                case "jpg":
                case "css":
                    return false;
                default:
                    return true;
            }

        }
    }
}
