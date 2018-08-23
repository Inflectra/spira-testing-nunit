using NUnit.Framework;
using Inflectra.SpiraTest.AddOns.NUnit;

namespace SampleTestSuite
{
    [TestFixture, SpiraConfiguration("localhost/Spira", "administrator", "", 1, -1, -1)]
    class SampleTest
    {
        int One, Two;
        [SetUp]
        protected void SetUp()
        {
            One = 1;
            Two = 2;
        }

        [Test]
        public void TestAdd()
        {
            int Result = One + Two;
            //will succeed
            Assert.AreEqual(Result, 3);
        }

        [Test]
        public void TestMultiply()
        {
            int Result = One * Two;
            //will fail
            Assert.AreEqual(Result, 3);
        }

        [Test]
        public void TestConcat()
        {
            string Result = string.Concat(One, Two);
            //will fail
            Assert.AreEqual(Result, "21");
        }


        
    }
}
