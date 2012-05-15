using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Crawler
{
    [TestFixture]
    internal class IntegrationTests
    {
        private List<string> _dirsCreated;
        private const string Path = "www.google.com";
        private static string _outputPath = string.Format("{0}_{1}", Path, DateTime.Now.ToString("hh-mm_MM-dd-yyyy"));
        private static CrawlerController _crawlerControler;

        [TestFixtureSetUp]
        public static void Init()
        {
            _crawlerControler = new CrawlerController(Path, 1, "asiegle@gmail.com");
        }

        [Test]
        public void TestDirectory()
        {
            Assert.IsTrue(Directory.Exists(Directory.GetCurrentDirectory() + "\\" + _outputPath));
        }

        [Test]
        public void TestDownloadFile()
        {
            Assert.IsTrue(File.Exists(Directory.GetCurrentDirectory() + "\\" + _outputPath + "\\index.html"));
        }

        [Test]
        public void TestLogTxtExists()
        {
            Assert.IsTrue(File.Exists(Directory.GetCurrentDirectory() + "\\" + _outputPath + "\\log.txt"));
        }

        //IMPORTANT: This test assumes that there are scripts on Google's homepage, which is most likely to be true.
        [Test]
        public void TestVulnerabilityInLog()
        {
            string logPath = Directory.GetCurrentDirectory() + "\\" + _outputPath + "\\log.txt";
            using(var sr = new StreamReader(logPath))
            {
                string contents = sr.ReadToEnd();
                Assert.IsTrue(contents.Contains("VULN: 'Script' found in file"));
            }
        }

        [Test]
        public void TestVersionInLog()
        {
            string logPath = Directory.GetCurrentDirectory() + "\\" + _outputPath + "\\log.txt";
            using (var sr = new StreamReader(logPath))
            {
                string contents = sr.ReadToEnd();
                Assert.IsTrue(contents.Contains("Running version:"));
            }
        }

        [Test]
        public void TestFilesFoundInLog()
        {
            string logPath = Directory.GetCurrentDirectory() + "\\" + _outputPath + "\\log.txt";
            using (var sr = new StreamReader(logPath))
            {
                string contents = sr.ReadToEnd();
                Assert.IsTrue(contents.Contains("Found file index.html"));
            }
        }

        [TestFixtureTearDown]
        public static void CleanUp()
        {
            Directory.Delete(Directory.GetCurrentDirectory() + "\\" + _outputPath, true);
        }
    }


//===== Below is just for reference. It is broken and very messy, do not use. =====

//        [Test]
//        public void Test404ReturnCode()
//        {
////            string path = "www.google.com_" + DateTime.Now.ToString("hh-mm_MM-dd-yyyy");
////            Website website = new Website("google.com/doesnotexist.html", path);
////            Bot bot = new Bot(website, null, null, null, null);
////            List<CrawlResult> cr = bot.CrawlSite(0);
////
//////            Assert.AreEqual(cr.ReturnCode,404);
//////            Assert.AreEqual("NotFound",cr.ReturnStatus);
//        } 
//        //        [Test]
//        //        public void TestDirectory()
//        //        {
//        //            Bot mockBot = MockRepository.GenerateStub<Bot>("www.test.com", 0, null, null, null);
//        //            using (mock.Record())
//        //            {
//        //                mockBot.C
//        //                
//        //            }
//        //            Bot b = new Bot("www.test.com",0,null,null,null);
//        //            b.CrawlSite();
//        //            //Assert.IsTrue(Directory.Exists("www.test.com"));
//        //            path = "www.test.com_" + DateTime.Now.ToString("hh-mm_MM-dd-yyyy");
//        //            Assert.IsTrue(Directory.Exists(path));
//        //        }
//        //
//        //        [Test]
//        //        public void TestDownloadFile()
//        //        {
//        //            Bot b = new Bot("www.test.com", 0, null, null, null);
//        //            b.CrawlSite();
//        //            Assert.IsTrue(File.Exists(path+"/testfile.html"));
//        //            
//        //        }
//        //        [Test]
//        //        public void Test404ReturnCode()
//        //        {
//        //            Bot b = new Bot("www.google.com/doesntexist", 0, null, null, null);
//        //            CrawlResult cr = b.CrawlSite();
//        //            Assert.AreEqual(cr.ReturnCode,404);
//        //            Assert.AreEqual("NotFound",cr.ReturnStatus);
//        //        } 
//        //
//        //        [Test]
//        //        public void Test200ReturnCode()
//        //        {
//        //            Bot b = new Bot("www.google.com", 0, null, null, null);
//        //            CrawlResult cr = b.CrawlSite();
//        //            Assert.AreEqual(cr.ReturnCode,200);
//        //            Assert.AreEqual(cr.ReturnStatus,"OK");
//        //        }
//
//        //        [Test]
//        //        public void TestChristmasHTML()
//        //        {
//        //            Bot b = new Bot("www.isitchristmasyet.com", 0, null, null, null);
//        //            CrawlResult cr = b.CrawlSite();
//        //            
//        //       }
//    }
}
