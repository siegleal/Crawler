using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
    public interface IWebInteractor
    {
        List<CrawlResult> CrawlSite(string url, int level);

        CrawlResult GetPage(string url);
    }

}
