using UIH.Mcsf.Filming.ImageManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for ImageJobModelTest and is intended
    ///to contain all ImageJobModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ImageJobModelTest
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
        ///A test for ImageJobModel Constructor
        ///</summary>
        [TestMethod()]
        public void ImageJobModelConstructorTest()
        {
            ImageJobModel target = new ImageJobModel();
            Assert.AreNotEqual(null, target);
        }

        /// <summary>
        ///A test for ImageCell
        ///</summary>
        [TestMethod()]
        public void ImageCellTest()
        {
            ImageJobModel target = new ImageJobModel();
            MedViewerControlCell expected = new MedViewerControlCell();
            MedViewerControlCell actual;
            target.ImageCell = expected;
            actual = target.ImageCell;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Modality
        ///</summary>
        [TestMethod()]
        public void ModalityTest()
        {
            ImageJobModel target = new ImageJobModel();
            string expected = "CT";
            string actual;
            target.Modality = expected;
            actual = target.Modality;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SeriesInstanceUID
        ///</summary>
        [TestMethod()]
        public void SeriesInstanceUIDTest()
        {
            ImageJobModel target = new ImageJobModel();
            string expected = "1.2.156.111.2222.3333.4444";
            string actual;
            target.SeriesInstanceUid = expected;
            actual = target.SeriesInstanceUid;
            Assert.AreEqual(expected, actual);
        }
    }
}
