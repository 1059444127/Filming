using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UIH.Mcsf.Filming.Utility;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmImageObjectTest and is intended
    ///to contain all FilmImageObjectTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmImageObjectTest
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
        ///A test for FilmImageObject Constructor
        ///</summary>
        [TestMethod()]
        public void FilmImageObjectConstructorTest()
        {
            var target = new FilmImageObject();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for CellIndex
        ///</summary>
        [TestMethod()]
        public void CellIndexTest()
        {
            var target = new FilmImageObject(); 
            int expected = -1; 
            int actual;
            target.CellIndex = expected;
            actual = target.CellIndex;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GSPSFilePath
        ///</summary>
        [TestMethod()]
        public void GSPSFilePathTest()
        {
            var target = new FilmImageObject(); 
            string expected = "test"; 
            string actual;
            target.GspsFilePath = expected;
            actual = target.GspsFilePath;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ImageFilePath
        ///</summary>
        [TestMethod()]
        public void ImageFilePathTest()
        {
            var target = new FilmImageObject(); 
            string expected = "test";
            string actual;
            target.ImageFilePath = expected;
            actual = target.ImageFilePath;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ImageSOPInstanceUID
        ///</summary>
        [TestMethod()]
        public void ImageSOPInstanceUIDTest()
        {
            var target = new FilmImageObject(); 
            string expected = "test";
            string actual;
            target.ImageSopInstanceUid = expected;
            actual = target.ImageSopInstanceUid;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ViewPortIndex
        ///</summary>
        [TestMethod()]
        public void ViewPortIndexTest()
        {
            var target = new FilmImageObject(); 
            int expected = -1; 
            int actual;
            target.ViewPortIndex = expected;
            actual = target.ViewPortIndex;
            Assert.AreEqual(expected, actual);
        }
    }
}
