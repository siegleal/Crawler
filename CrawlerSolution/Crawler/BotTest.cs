using NUnit.Framework;
using Rhino.Mocks;



namespace Crawler
{
    [TestFixture]
    class BotTest
    {
        private string path;
        private Bot b;
        private MockRepository mock;
        
        

        [SetUp]
        public void Init()
        {
            mock = new MockRepository();
        }

        [Test]
        public void AlwaysPass()
        {
            Assert.AreEqual(2.0,2.0);
        }


        [Test]
        public void TestCreateDirectory()
        {
            
        }

        [Test]//Mikey
        public void TestStatus200()
        {
            
        }

        [Test]//Mikey
        public void TestStatus404()
        {
            
        }

        [Test]
        public void TestOneLink()
        {
            CrawlResult testResult = new CrawlResult();
            testResult.ReturnCode = 200;
            testResult.ReturnStatus = "OK";
            testResult.Html = "href=\"/csse/index.html\"";

        }

        [Test]
        public void TestTwoLink()
        {

        }

//        [Test]
//        public void TestDirectory()
//        {
//            Bot mockBot = MockRepository.GenerateStub<Bot>("www.test.com", 0, null, null, null);
//            using (mock.Record())
//            {
//                mockBot.C
//                
//            }
//            Bot b = new Bot("www.test.com",0,null,null,null);
//            b.CrawlSite();
//            //Assert.IsTrue(Directory.Exists("www.test.com"));
//            path = "www.test.com_" + DateTime.Now.ToString("hh-mm_MM-dd-yyyy");
//            Assert.IsTrue(Directory.Exists(path));
//        }
//
//        [Test]
//        public void TestDownloadFile()
//        {
//            Bot b = new Bot("www.test.com", 0, null, null, null);
//            b.CrawlSite();
//            Assert.IsTrue(File.Exists(path+"/testfile.html"));
//            
//        }
//        [Test]
//        public void Test404ReturnCode()
//        {
//            Bot b = new Bot("www.google.com/doesntexist", 0, null, null, null);
//            CrawlResult cr = b.CrawlSite();
//            Assert.AreEqual(cr.ReturnCode,404);
//            Assert.AreEqual("NotFound",cr.ReturnStatus);
//        } 
//
//        [Test]
//        public void Test200ReturnCode()
//        {
//            Bot b = new Bot("www.google.com", 0, null, null, null);
//            CrawlResult cr = b.CrawlSite();
//            Assert.AreEqual(cr.ReturnCode,200);
//            Assert.AreEqual(cr.ReturnStatus,"OK");
//        }

//        [Test]
//        public void TestChristmasHTML()
//        {
//            Bot b = new Bot("www.isitchristmasyet.com", 0, null, null, null);
//            CrawlResult cr = b.CrawlSite();
//            
//       }
    }
}
