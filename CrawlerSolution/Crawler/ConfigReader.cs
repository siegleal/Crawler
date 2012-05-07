using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Crawler
{


    public class ConfigReader
    {
        const string Filename = "config.ini";


        public static string ReadDatabaseAccessorString()
        {
            var parseFor = new Regex(@"database =(.+)");

            string result = MatchRegex(parseFor);

            if(result == null)
            {
                throw new Exception("Cannot find database string");
            }

            return result;
        }

        public static string ReadEmailAddress()
        {
            var parseFor = new Regex(@"email =(.+)");

            string result = MatchRegex(parseFor);
            
            if(result == null)
            {
                throw new Exception("Cannot find email string");
            }

            return result;
        }

        private static string MatchRegex(Regex parseFor)
        {
            using (var file = new StreamReader(Filename))
            {
                while (!file.EndOfStream)
                {
                    var current = file.ReadLine();
                    if (parseFor.IsMatch(current))
                    {
                        var matches = parseFor.Match(current);
                        return matches.Groups[1].ToString();
                    }
                }
            }
            return null;
        }

    }
}
