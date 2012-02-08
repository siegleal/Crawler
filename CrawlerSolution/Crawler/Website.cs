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
        }

        public void addFile(String s)
        {
            filesFound.Add(s);
        }

        public List<String> getFilesFound()
        {
            return filesFound;
        }
    }
}
