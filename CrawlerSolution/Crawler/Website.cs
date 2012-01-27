using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
    class Website
    {
        public string url;
        public string dirpath;

        public Website(string url, String dirpath)
        {
            this.url = url;
            this.dirpath = dirpath;
        }
    }
}
