using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Crawler
{
    class StringMatchStrategy : IParsingStrategy
    {
        private List<String> _searchStringList;
        private List<String> _descriptionList;
        private List<String> _files; 

        public StringMatchStrategy(List<String> searchStringList, List<String> descriptionList, List<String> files)
        {
            _searchStringList = searchStringList;
            _descriptionList = descriptionList;
            _files = files;
        }

        public List<String> Parse()
        {
            var report = new List<String>();
            foreach (String file in _files)
            {
                List<int> histogram = GetFrequencyFromFile(file);
                for (int j = 0; j < histogram.Count; j++)
                {
                    int n = histogram[j];
                    if (n > 0)
                    {
                        report.Add("'" + _descriptionList[j] + "' found in file " + file.Substring(file.LastIndexOf("\\")) + ". Frequency: " + n);
                    }
                    j++;
                }
            }
            return report;
        }

        public List<int> GetFrequencyFromFile(string file)
        {
            var fileStream = new StreamReader(file);
            string text = fileStream.ReadToEnd();
            List<int> r = GetFrequencyFromString(text);
            fileStream.Close();
            return r;
        }

        public List<int> GetFrequencyFromString(string text)
        {
            int n = _searchStringList.Count;
            var rList = new List<int>();
            for (int i = 0; i < n; i++)
            {
                rList.Add(0);
                int j = 0;
                while ((j = text.IndexOf(_searchStringList[i], j)) != -1)
                {
                    j += _searchStringList[i].Length;
                    rList[i] = rList[i] + 1;
                }
            }
            return rList;
        }
    }
}
