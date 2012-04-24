using NUnit.Framework;

using System.Collections.Generic;
using Rhino.Mocks;


namespace Crawler
{
    [TestFixture]
    class BotTest
    {
        private string path;
        private Bot b;
        private MockRepository _mock;
        
        

        [SetUp]
        public void Init()
        {
            _mock = new MockRepository();
        }

        [Test]
        public void AlwaysPass()
        {
            Assert.AreEqual(2.0,2.0);
        }


        [Test]
        public void TestCreateDirectory()
        {
            MockRepository mocks = new MockRepository();

            Website web = new Website("www.test.com", "");
            Log l = MockRepository.GenerateStub<Log>("samplepath");
            WebInteractor wi = MockRepository.GenerateStub<WebInteractor>();
            var fsi = mocks.StrictMock<IFileSystemInteractor>();

            Bot b = new Bot(web, l, null, wi, fsi);

            using (mocks.Record())
            {
                fsi.MakeDirectory("samplepath");
            }
        }

        [Test]
        public void TestBotCallsCrawlSiteOnWebInteractor()
        {
            MockRepository mocks = new MockRepository();
            var web = MockRepository.GenerateStub<IWebInteractor>();
            var site = new Website("www.whocares.com", "whatever");
            var mockFSI = MockRepository.GenerateStub<FileSystemInteractor>();

            var useableBot = new Bot(site, null, null, web, mockFSI);
            useableBot.CrawlSite(1);
            web.AssertWasCalled(s => s.GetPage("www.whocares.com"));
        }

        [Test]
        public void TestStatus200()
        {
            MockRepository mocks = new MockRepository();
            var web = mocks.StrictMock<IWebInteractor>();
            var site = new Website("www.whocares.com", "whatever");
            var mockFSI = MockRepository.GenerateStub<FileSystemInteractor>();

            var retVal = new CrawlResult();
            retVal.ReturnCode = 200;
            retVal.Html = @"";
            var retList = new List<CrawlResult>();
            retList.Add(retVal);

            Expect.On(web).Call(web.GetPage("www.whocares.com")).Return(retVal);
            mocks.ReplayAll();


            var useableBot = new Bot(site, null, null, web, mockFSI);
            var check = useableBot.CrawlSite(1);

            Assert.IsTrue(check[0].ReturnCode == 200);

            mocks.VerifyAll();

        }

        [Test]
        public void TestStatus404()
        {
            MockRepository mocks = new MockRepository();
            var web = mocks.StrictMock<IWebInteractor>();
            var site = new Website("www.whocares.com", "whatever");
            var mockFSI = MockRepository.GenerateStub<FileSystemInteractor>();

            var retVal = new CrawlResult();
            retVal.ReturnCode = 404;
            retVal.Html = @"";

            Expect.On(web).Call(web.GetPage("www.whocares.com")).Return(retVal);
            mocks.ReplayAll();


            var useableBot = new Bot(site, null, null, web, mockFSI);
            var check = useableBot.CrawlSite(1);

            Assert.IsTrue(check[0].ReturnCode == 404);

            mocks.VerifyAll();
        }

        [Test]
        public void TestChainOneLink()
        {
            MockRepository mocks = new MockRepository();
            var web = mocks.StrictMock<IWebInteractor>();
            var site = new Website("www.whocares.com", "whatever");
            var mockFSI = MockRepository.GenerateStub<IFileSystemInteractor>();

            CrawlResult testResult = new CrawlResult();
            testResult.ReturnCode = 200;
            testResult.ReturnStatus = "OK";
            testResult.Html = "href=\"/csse.html\"";

            var resultTwo = new CrawlResult();
            resultTwo.ReturnCode = 200;
            resultTwo.ReturnStatus = "OK";
            resultTwo.Html = "";

            Expect.On(web).Call(web.GetPage("www.test.com")).Return(testResult);
            Expect.On(web).Call(web.GetPage("www.test.com/csse.html")).Return(resultTwo);

            mocks.ReplayAll();

            Bot b = new Bot(new Website("www.test.com","simplepath"),null,null,web,mockFSI);
            List<CrawlResult> results = b.CrawlSite(2);

            mocks.VerifyAll();

            Assert.AreEqual(2,results.Count);


        }

        [Test]
        public void TestChainTwoLinks()
        {
            MockRepository mocks = new MockRepository();
            var web = mocks.StrictMock<IWebInteractor>();
            var mockFSI = MockRepository.GenerateStub<IFileSystemInteractor>();

            CrawlResult testResult = new CrawlResult();
            testResult.ReturnCode = 200;
            testResult.ReturnStatus = "OK";
            testResult.Html = "href=\"/csse.html\"";

            var resultTwo = new CrawlResult();
            resultTwo.ReturnCode = 200;
            resultTwo.ReturnStatus = "OK";
            resultTwo.Html = "href=\"/abbe.html\"";

            var resultThree = new CrawlResult();
            resultThree.ReturnCode = 200;
            resultThree.ReturnStatus = "OK";
            resultThree.Html = "";


            Expect.On(web).Call(web.GetPage("www.test.com")).Return(testResult);
            Expect.On(web).Call(web.GetPage("www.test.com/csse.html")).Return(resultTwo);
            Expect.On(web).Call(web.GetPage("www.test.com/abbe.html")).Return(resultThree);

            mocks.ReplayAll();

            Bot b = new Bot(new Website("www.test.com","simplepath"),null,null,web,mockFSI);
            List<CrawlResult> results = b.CrawlSite(2);

            mocks.VerifyAll();

            Assert.AreEqual(3,results.Count);


        }


        [Test]
        public void TestDepthOne()
        {
            MockRepository mocks = new MockRepository();
            var web = mocks.StrictMock<IWebInteractor>();
            var mockFSI = MockRepository.GenerateStub<IFileSystemInteractor>();

            CrawlResult testResult = new CrawlResult();
            testResult.ReturnCode = 200;
            testResult.ReturnStatus = "OK";
            testResult.Html = "href=\"/csse.html\"";

            var resultTwo = new CrawlResult();
            resultTwo.ReturnCode = 200;
            resultTwo.ReturnStatus = "OK";
            resultTwo.Html = "href=\"/abbe.html\"";

            var resultThree = new CrawlResult();
            resultThree.ReturnCode = 200;
            resultThree.ReturnStatus = "OK";
            resultThree.Html = "";


            Expect.On(web).Call(web.GetPage("www.test.com")).Return(testResult);
            Expect.On(web).Call(web.GetPage("www.test.com/csse.html")).Return(resultTwo);
            //Expect.On(web).Call(web.GetPage("www.test.com/abbe.html")).Return(resultThree);

            mocks.ReplayAll();

            Bot b = new Bot(new Website("www.test.com","simplepath"),null,null,web,mockFSI);
            List<CrawlResult> results = b.CrawlSite(1);

            mocks.VerifyAll();

            Assert.AreEqual(2,results.Count);


        }

        //TODO test link loops
        [Test]
        public void TestLoopToIndex()
        {
            MockRepository mocks = new MockRepository();
            var web = mocks.StrictMock<IWebInteractor>();
            var mockFSI = MockRepository.GenerateStub<IFileSystemInteractor>();

            CrawlResult testResult = new CrawlResult();
            testResult.ReturnCode = 200;
            testResult.ReturnStatus = "OK";
            testResult.Html = "href=\"/csse.html\"";

            var resultTwo = new CrawlResult();
            resultTwo.ReturnCode = 200;
            resultTwo.ReturnStatus = "OK";
            resultTwo.Html = "href=\"/\"";

            var resultThree = new CrawlResult();
            resultThree.ReturnCode = 200;
            resultThree.ReturnStatus = "OK";
            resultThree.Html = "";


            Expect.On(web).Call(web.GetPage("www.test.com")).Return(testResult);
            Expect.On(web).Call(web.GetPage("www.test.com/csse.html")).Return(resultTwo);
            //Expect.On(web).Call(web.GetPage("www.test.com/abbe.html")).Return(resultThree);

            mocks.ReplayAll();

            Bot b = new Bot(new Website("www.test.com","simplepath"),null,null,web,mockFSI);
            List<CrawlResult> results = b.CrawlSite(2);

            mocks.VerifyAll();

            Assert.AreEqual(2,results.Count);


        }

        [Test]
        public void TestLoopToPage()
        {
            MockRepository mocks = new MockRepository();
            var web = mocks.StrictMock<IWebInteractor>();
            var mockFSI = MockRepository.GenerateStub<IFileSystemInteractor>();

            CrawlResult testResult = new CrawlResult();
            testResult.ReturnCode = 200;
            testResult.ReturnStatus = "OK";
            testResult.Html = "href=\"/csse.html\"";

            var resultTwo = new CrawlResult();
            resultTwo.ReturnCode = 200;
            resultTwo.ReturnStatus = "OK";
            resultTwo.Html = "href=\"/abbe.html\"";

            var resultThree = new CrawlResult();
            resultThree.ReturnCode = 200;
            resultThree.ReturnStatus = "OK";
            resultThree.Html = "href=\"/csse.html\"";


            Expect.On(web).Call(web.GetPage("www.test.com")).Return(testResult);
            Expect.On(web).Call(web.GetPage("www.test.com/csse.html")).Return(resultTwo);
            Expect.On(web).Call(web.GetPage("www.test.com/abbe.html")).Return(resultThree);

            mocks.ReplayAll();

            Bot b = new Bot(new Website("www.test.com","simplepath"),null,null,web,mockFSI);
            List<CrawlResult> results = b.CrawlSite(4);

            mocks.VerifyAll();

            Assert.AreEqual(3,results.Count);


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
