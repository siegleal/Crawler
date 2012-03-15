using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
    interface IParsingStrategy
    {
        List<String> Parse();
    }
}
