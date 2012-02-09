using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
    public class Website
    {
        public string url;
        public string dirpath;
        private List<string> filesFound;

        public Website(string url, String dirpath)
        {
            this.url = url;
            this.dirpath = dirpath;
            this.filesFound = new List<String>();
        }

        public void addFile(String s)
        {
            try
            {
                filesFound.Add(s);
            }
            finally { }
        }

        public List<String> getFilesFound()
        {
            return filesFound;
        }
    }
}
