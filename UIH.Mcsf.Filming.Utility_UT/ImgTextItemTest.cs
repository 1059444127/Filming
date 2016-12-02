using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UIH.Mcsf.Filming.Utility_UT
{
    
    
    /// <summary>
    ///This is a test class for ImgTextItemTest and is intended
    ///to contain all ImgTextItemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ImgTextItemTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ImgTextItem Constructor
        ///</summary>
        [TestMethod()]
        public void ImgTextItemConstructorTest()
        {
            ImgTextItem target = new ImgTextItem();
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Clone
        ///</summary>
        [TestMethod()]
        public void CloneTest()
        {
            ImgTextItem target = new ImgTextItem(); // 
            ImgTextItem expected = target; // 
            ImgTextItem actual;
            actual = target.Clone();
            Assert.AreEqual(expected.Format, actual.Format);
            Assert.AreEqual(expected.Show, actual.Show);
            Assert.AreEqual(expected.Tag, actual.Tag);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Format
        ///</summary>
        [TestMethod()]
        public void FormatTest()
        {
            ImgTextItem target = new ImgTextItem(); // 
            string expected = string.Empty; // 
            string actual;
            target.Format = expected;
            actual = target.Format;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Show
        ///</summary>
        [TestMethod()]
        public void ShowTest()
        {
            ImgTextItem target = new ImgTextItem(); // 
            bool expected = false; // 
            bool actual;
            target.Show = expected;
            actual = target.Show;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Tag
        ///</summary>
        [TestMethod()]
        public void TagTest()
        {
            ImgTextItem target = new ImgTextItem(); // 
            string expected = string.Empty; // 
            string actual;
            target.Tag = expected;
            actual = target.Tag;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
