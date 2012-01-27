using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
    interface PluginInterface
    {
        List<String> analyzeSite(Website web, DatabaseAccessor db, int crawlid, Log log);
    }
}
