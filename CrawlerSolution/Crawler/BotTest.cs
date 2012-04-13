using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Crawler
{
    [TestFixture]
    class BotTest
    {
        private string path;
        private Bot b;

        [SetUp]
        public void Init()
        {
        }

        [Test]
        public void AlwaysPass()
        {
            Assert.AreEqual(2.0,2.0);
        }

        [Test]
        public void TestDirectory()
        {
            Bot b = new Bot("www.test.com",0,null,null,null);
            b.CrawlSite();
            //Assert.IsTrue(Directory.Exists("www.test.com"));
            path = "www.test.com_" + DateTime.Now.ToString("hh-mm_MM-dd-yyyy");
            Assert.IsTrue(Directory.Exists(path));
        }

        [Test]
        public void TestDownloadFile()
        {
            Bot b = new Bot("www.test.com", 0, null, null, null);
            b.CrawlSite();
            Assert.IsTrue(File.Exists(path+"/testfile.html"));
            
        }

        [Test]
        public void Test404ReturnCode()
        {
            Bot b = new Bot("www.google.com/doesntexist", 0, null, null, null);
            CrawlResult cr = b.CrawlSite();
            Assert.AreEqual(cr.ReturnCode,404);
            Assert.AreEqual("NotFound",cr.ReturnStatus);
        } 

        [Test]
        public void Test200ReturnCode()
        {
            Bot b = new Bot("www.google.com", 0, null, null, null);
            CrawlResult cr = b.CrawlSite();
            Assert.AreEqual(cr.ReturnCode,200);
            Assert.AreEqual(cr.ReturnStatus,"OK");
        }

//        [Test]
//        public void TestChristmasHTML()
//        {
//            Bot b = new Bot("www.isitchristmasyet.com", 0, null, null, null);
//            CrawlResult cr = b.CrawlSite();
//            
//       }
    }
}
