﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
//Nothing too sophisicated yet. This class will retrun the frequency of the given strings in another given string or text or a file.
namespace Crawler
{
    class ParsingModule: crawlerPlugin
    {
        private List<String> searchStringList;


        public void passInStrings(List<String> stringList)
        {
            searchStringList = stringList;
        }

        public List<String> analyseSite()
        {
            /* TODO:  This.  Needs a list of vulnerabilities to hunt for, then should pass its
             * stuff into */
            return new List<String>();
        }
        
        public ArrayList getFrequencyFromString(string text)
        {
            int n = searchStringList.Count;
            ArrayList rList = new ArrayList(); //I can't make an array list have a certain type of elements, I will look into this
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

        public ArrayList getFrequencyFromFile(string file)
        {
            StreamReader fileStream = new StreamReader(file);
            //Simply turn the file into a string
            string text = fileStream.ReadToEnd();
            ArrayList r = getFrequencyFromString(text);
            fileStream.Close();
            return r;
        }

    }
}