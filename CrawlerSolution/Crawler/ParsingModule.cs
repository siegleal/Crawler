using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
//Nothing too sophisicated yet. This class will retrun the frequency of the given strings in another given string or text or a file.
namespace Crawler
{
    class ParsingModule: CrawlerPlugin
    {
        public ParsingModule(Website w, DatabaseAccessor db, int id, Log log)
            : base(w, db, id, log)
        {

        }

        private List<String> searchStringList;
        private List<String> descriptionList;

        public void passInStrings(List<String> stringList)
        {
            searchStringList = stringList;
        }

        public void loadDefinitionsFromFile()
        {
            //Load from Vuln.txt
            System.IO.StreamReader file = new System.IO.StreamReader("Vuln.txt");
            String line;
            int i = 0;
            while ((line = file.ReadLine()) != null)
            {
                int s = line.IndexOf("\t");
                searchStringList[i] = line.Substring(0, s - 1);
                descriptionList[i] = line.Substring(s + 1, line.Length);
                i++;
            }
        }

        public override List<String> analyzeSite()
        {
            List<String> files = website.getFilesFound();
            foreach (String file in files)
            {
                List<int> histogram = getFrequencyFromFile(file);
                int j = 0;
                foreach (int n in histogram)
                {
                    if (n > 0)
                    {
                        //We've found something.
                        log.writeDebug("Vulnerability '" + descriptionList[j] + "' found in file " + file + ". Frequency: " + n);
                    }
                    j++;
                }
            }
            //Not sure what exactly to return yet.
            return new List<String>();
        }
        
        public List<int> getFrequencyFromString(string text)
        {
            int n = searchStringList.Count;
            List<int> rList = new List<int>(); //I can't make an array list have a certain type of elements, I will look into this
            for (int i = 0; i < n; i++)
            {
                rList[i] = 0;
                int j = 0;
                while ((i = text.IndexOf(searchStringList[i], j)) != -1)
                {
                    j += searchStringList[i].Length;
                    rList[i] = (int)rList[i]+1;
                }
            }
            return rList;
        }

        public List<int> getFrequencyFromFile(string file)
        {
            StreamReader fileStream = new StreamReader(file);
            //Simply turn the file into a string
            string text = fileStream.ReadToEnd();
            List<int> r = getFrequencyFromString(text);
            fileStream.Close();
            return r;
        }

    }
}
