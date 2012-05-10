using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;

    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Version {0}", Assembly.GetEntryAssembly().GetName().Version.ToString());
            Console.Write("Enter the site to crawl>>");
            String path = Console.ReadLine();
            Console.Write("Enter the crawl level>>");
            String level = Console.ReadLine();
            Console.Write("Enter e-mail address to respond to>>");
            String email = Console.ReadLine();

            Crawler.CrawlerController crawler = new Crawler.CrawlerController(path, Int32.Parse(level),email);

            Console.WriteLine("Complete. Press any key to continue...");
            Console.Read();
        }
    }
