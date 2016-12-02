using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for InterleavedSelectionViewModelTest and is intended
    ///to contain all InterleavedSelectionViewModelTest Unit Tests
    ///</summary>
    [TestClass]
    public class InterleavedSelectionViewModelTest
    {


        private TestContext _testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
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
        ///A test for FirstImage
        ///</summary>
        [TestMethod]
        public void FirstImageTest()
        {
            InterleavedSelectionViewModel target = new InterleavedSelectionViewModel(); 
            uint expected = 5; 
            uint actual;
            target.FirstImage = expected;
            actual = target.FirstImage;
            Assert.AreEqual(expected, actual);
            
        }

        /// <summary>
        ///A test for Every
        ///</summary>
        [TestMethod]
        public void EveryTest()
        {
            InterleavedSelectionViewModel target = new InterleavedSelectionViewModel(); 
            uint expected = 5; 
            uint actual;
            target.Every = expected;
            actual = target.Every;
            Assert.AreEqual(expected, actual);
            
        }

        /// <summary>
        ///A test for ImageNumbers
        ///</summary>
        [TestMethod]
        public void ImageNumbersTest()
        {
            InterleavedSelectionViewModel target = new InterleavedSelectionViewModel(); 
            uint expected = 5; 
            uint actual;
            target.ImageNumbers = expected;
            actual = target.ImageNumbers;
            Assert.AreEqual(expected, actual);
            
        }

        /// <key>\n 
        /// PRA:Yes \n
        /// Traced from: N/A \n
        /// Tests: DS_PRA_Filming_AddImagesToFilmCard \n
        /// Description: study data constructor \n
        /// Short Description: StudyDataConstructor \n
        /// Component: Filming \n
        /// </key> \n
        /// <summary>
        ///A test for StudyData Constructor
        ///</summary>
        [TestMethod]
        public void LastImageTest()
        {
            InterleavedSelectionViewModel target = new InterleavedSelectionViewModel(); 
            uint expected = 5; 
            uint actual;
            target.LastImage = expected;
            actual = target.LastImage;
            Assert.AreEqual(expected, actual);
            
        }

        /// <summary>
        ///A test for MaxEvery
        ///</summary>
        [TestMethod]
        public void MaxEveryTest()
        {
            InterleavedSelectionViewModel target = new InterleavedSelectionViewModel(); 
            uint expected = 5; 
            uint actual;
            target.MaxEvery = expected;
            actual = target.MaxEvery;
            Assert.AreEqual(expected, actual);
            
        }

        /// <summary>
        ///A test for MaxFirstImageIndex
        ///</summary>
        [TestMethod]
        public void MaxFirstImageIndexTest()
        {
            InterleavedSelectionViewModel target = new InterleavedSelectionViewModel(); 
            uint expected = 5; 
            uint actual;
            target.MaxFirstImageIndex = expected;
            actual = target.MaxFirstImageIndex;
            Assert.AreEqual(expected, actual);
            
        }

        /// <summary>
        ///A test for MaxLastImageIndex
        ///</summary>
        [TestMethod]
        public void MaxLastImageIndexTest()
        {
            InterleavedSelectionViewModel target = new InterleavedSelectionViewModel(); 
            uint expected = 5; 
            uint actual;
            target.MaxLastImageIndex = expected;
            actual = target.MaxLastImageIndex;
            Assert.AreEqual(expected, actual);
            
        }

        /// <key>\n 
        /// PRA:Yes \n
        /// Traced from: N/A \n
        /// Tests: DS_PRA_Filming_AddImagesToFilmCard \n
        /// Description: study data constructor \n
        /// Short Description: StudyDataConstructor \n
        /// Component: Filming \n
        /// </key> \n
        /// <summary>
        ///A test for StudyData Constructor
        ///</summary>
        [TestMethod]
        public void TotalImagesTest()
        {
            InterleavedSelectionViewModel target = new InterleavedSelectionViewModel(); 
            uint expected = 5; 
            uint actual;
            target.TotalImages = expected;
            actual = target.TotalImages;
            Assert.AreEqual(expected, actual);
            
        }
    }
}
