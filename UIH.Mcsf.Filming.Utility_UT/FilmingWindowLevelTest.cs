using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UIH.Mcsf.Filming.Utility_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingWindowLevelTest and is intended
    ///to contain all FilmingWindowLevelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmingWindowLevelTest
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
        ///A test for FilmingWindowLevel Constructor
        ///</summary>
        [TestMethod()]
        public void FilmingWindowLevelConstructorTest()
        {
            FilmingWindowLevel target = new FilmingWindowLevel();
            //Assert.Inconclusive("TODO: Implement code to verify target");
            Assert.AreEqual(string.Empty, target.Name);
            Assert.AreEqual(0, target.Center);
            Assert.AreEqual(0, target.Width);
        }

        /// <summary>
        ///A test for Center
        ///</summary>
        [TestMethod()]
        public void CenterTest()
        {
            FilmingWindowLevel target = new FilmingWindowLevel(); // 
            double expected = 0F; // 
            double actual;
            target.Center = expected;
            actual = target.Center;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Width
        ///</summary>
        [TestMethod()]
        public void WidthTest()
        {
            FilmingWindowLevel target = new FilmingWindowLevel(); // 
            double expected = 0F; // 
            double actual;
            target.Width = expected;
            actual = target.Width;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Name
        ///</summary>
        [TestMethod()]
        public void NameTest()
        {
            FilmingWindowLevel target = new FilmingWindowLevel(); // 
            string expected = string.Empty; // 
            string actual;
            target.Name = expected;
            actual = target.Name;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            FilmingWindowLevel target = new FilmingWindowLevel(); // 
            string expected = string.Empty; // 
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
