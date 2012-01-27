using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the site to crawl>>");
            String path = Console.ReadLine();
            Console.Write("Enter the crawl level>>");
            String level = Console.ReadLine();

            CrawlingEngine.Crawler crawler = new CrawlingEngine.Crawler(path, Int32.Parse(level));

            Console.WriteLine("Complete. Press any key to continue...");
            Console.Read();
        }
    }
