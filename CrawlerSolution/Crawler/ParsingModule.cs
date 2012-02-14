using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
namespace Crawler
{
    /// <summary>
    /// Class: HTMLParseingModule
    /// Description:
    /// This class is a crawler plugin reads vulnerability definitions from the file "Vuln.txt" to search for in HTML files. Definitions are in the format [String]\t[Description], 
    /// where [String] the string to look for in all source files, and [Description] is a brief description of what vulnerability [String] represents. To load definitions, call loadDefinitionsFromFile (loaded automatically with analyzeSite()).
    /// </summary>
    class HTMLParsingModule: CrawlerPlugin
    {
        //These will be defined when loadDefinitionsFromFile is called
        private List<String> searchStringList;
        private List<String> descriptionList;
        public HTMLParsingModule(Website w, DatabaseAccessor db, int id, Log log)
            : base(w, db, id, log)
        {
            searchStringList = new List<String>();
            descriptionList = new List<String>();
        }

        //Loads definitions. Call before doing anything else with this object.
        public void loadDefinitionsFromFile()
        {
            //Load from Vuln.txt
            System.IO.StreamReader file = new System.IO.StreamReader("Vuln.txt");
            String line;
            int i = 0;
            while ((line = file.ReadLine()) != null)
            {
                int s = line.IndexOf("\t");
                searchStringList.Add(line.Substring(0, s));
                descriptionList.Add(line.Substring(s+1, line.Length-s-1));
                i++;
            }
        }

        //Finds the vulnerabilites. Called by the factory.
        public override List<String> analyzeSite()
        {
            loadDefinitionsFromFile();
            List<String> files = website.getFilesFound();
            foreach (String file in files)
            {
                List<int> histogram = getFrequencyFromFile(file);
                for (int j = 0; j < histogram.Count; j++)
                {
                    int n = histogram[j];
                    if (n > 0)
                    {
                        //A string has matched -- we've found something.
                        log.writeUser("'" + descriptionList[j] + "' found in file " + file.Substring(file.LastIndexOf("\\")) + ". Frequency: " + n, "VULN");
                        //Insert any other actions to do after finding a vulnerability here.


                    }
                    j++;
                }
            }
            return new List<String>();
        }
        
        //Simply generates a histogram from the vulnerability string list.
        public List<int> getFrequencyFromString(string text)
        {
            int n = searchStringList.Count;
            List<int> rList = new List<int>();
            for (int i = 0; i < n; i++)
            {
                rList.Add(0);
                int j = 0;
                while ((j = text.IndexOf(searchStringList[i], j)) != -1)
                {
                    j += searchStringList[i].Length;
                    rList[i] = rList[i]+1;
                }
            }
            return rList;
        }

        //File verson of getFrequencyFromString()
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
