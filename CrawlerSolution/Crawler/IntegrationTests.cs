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
        [SetUp]
        public void Init()
        {
            //            _path = "www.google.com_" + DateTime.Now.ToString("hh-mm_MM-dd-yyyy");
            //            _website = new Website("google.com", _path);
            //            IWebInteractor webInteractor = new WebInteractor();
            //            IFileSystemInteractor fileInteractor = new FileSystemInteractor();
            //            Log log = new Log(_path+"\\log.txt");
            //            _bot = new Bot(_website, log, null, webInteractor, fileInteractor);

        }

        [Test]
        public void TestDirectory()
        {
            string path = "http://www.google.com";
            string outputPath = string.Format("{0}_{1}", path, DateTime.Now.ToString("hh-mm_MM-dd-yyyy"));
            var cc = new CrawlerController(path, 0);
            Assert.IsTrue(Directory.Exists(Directory.GetCurrentDirectory() + "\\" + outputPath));
        }

        [Test]
        public void TestDownloadFile()
        {
            string path = "http://www.google.com";
            string outputPath = string.Format("{0}_{1}", path, DateTime.Now.ToString("hh-mm_MM-dd-yyyy"));
            var cc = new CrawlerController(path, 0);
            Assert.IsTrue(File.Exists(Directory.GetCurrentDirectory() + "\\" + outputPath + "index.html"));
        }

        [Test]
        public void TestLogTxtExists()
        {
            string path = "http://www.google.com";
            string outputPath = string.Format("{0}_{1}", path, DateTime.Now.ToString("hh-mm_MM-dd-yyyy"));
            var cc = new CrawlerController(path, 0);
            Assert.IsTrue(File.Exists(Directory.GetCurrentDirectory() + "\\" + outputPath + "log.txt"));
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
