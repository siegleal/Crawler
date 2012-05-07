using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Crawler
{
    [TestFixture]
    class ConfigReaderTest
    {


        [Test]
        public void connectionStringTest()
        {
            Assert.AreEqual("Database=SecurityCrawlerDatabase;user id=SecurityCrawlerUser;password=LongerPasswordsAreMoreSecureThanShorterOnes;server=whale.cs.rose-hulman.edu;connection timeout=30;",ConfigReader.ReadDatabaseAccessorString());
        }

        [Test]
        public void emailReadTest()
        {
            Assert.AreEqual("asiegle@gmail.com", ConfigReader.ReadEmailAddress());
        }

    }
}
