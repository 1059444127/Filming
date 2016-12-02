//using UIH.Mcsf.Database;
//using UIH.Mcsf.Filming;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using UIH.Mcsf.Viewer;
//using System.Windows.Media;

//namespace UIH.Mcsf.Filming.CardFE_UT
//{
    
    
//    /// <summary>
//    ///This is a test class for ImageDataTest and is intended
//    ///to contain all ImageDataTest Unit Tests
//    ///</summary>
//    [TestClass()]
//    public class ImageDataTest
//    {
//        private TestContext _testContextInstance;
//        private static string _seriesUID = string.Empty;
//        private static StudyData _studyData;
//        private static string _imageUID;

//        /// <summary>
//        ///Gets or sets the test context which provides
//        ///information about and functionality for the current test run.
//        ///</summary>
//        public TestContext TestContext
//        {
//            get
//            {
//                return _testContextInstance;
//            }
//            set
//            {
//                _testContextInstance = value;
//            }
//        }

//        #region Additional test attributes
//        // 
//        //You can use the following additional attributes as you write your tests:
//        //
//        //Use ClassInitialize to run code before running the first test in the class
//        //[ClassInitialize()]
//        public static void MyClassInitialize(TestContext testContext)
//        {
//            DBWrapper dbWrapper = new DBWrapper();
//            if (!dbWrapper.Initialize())
//                return;
//            var studyList = dbWrapper.GetAllStudyList();
//            string studyInstanceUID = string.Empty;
//            if (studyList.Count > 0)
//                studyInstanceUID = studyList[0].StudyInstanceUID;
//            else
//            {
//                return;
//            }

//            _seriesUID = string.Empty;
//            var seriesList = dbWrapper.GetSeriesListByStudyInstanceUID(studyInstanceUID);
//            if (seriesList.Count > 0)
//                _seriesUID = seriesList[0].SeriesInstanceUID;
//            else
//            {
//                return;
//            }

//            var imageList = dbWrapper.GetImageListBySeriesInstanceUID(_seriesUID);
//            if (imageList.Count > 0)
//                _imageUID = imageList[0].SOPInstanceUID;

//            _studyData = new StudyData(studyInstanceUID);
//        }
//        //
//        //Use ClassCleanup to run code after all tests in a class have run
//        //[ClassCleanup()]
//        //public static void MyClassCleanup()
//        //{
//        //}
//        //
//        //Use TestInitialize to run code before running each test
//        //[TestInitialize()]
//        //public void MyTestInitialize()
//        //{
//        //}
//        //
//        //Use TestCleanup to run code after each test has run
//        //[TestCleanup()]
//        //public void MyTestCleanup()
//        //{
//        //}
//        //
//        #endregion

        
        
//        /// <summary>
//        ///A test for ImageData Constructor
//        ///</summary>
//        [TestMethod()]
//        public void ImageDataConstructorTest()
//        {
//            var target = new ImageData(_imageUID);
//            Assert.IsNotNull(target);
//        }

//        /// <summary>
//        ///A test for CreateDisplayData
//        ///</summary>
//        [TestMethod()]
//        public void CreateDisplayDataTest()
//        {
//            var target = new ImageData(_imageUID);
//            string sharedMemName = "test";
//            target.CreateDisplayData(sharedMemName);
//        }

//        /// <summary>
//        ///A test for Description
//        ///</summary>
//        [TestMethod()]
//        public void DescriptionTest()
//        {
//            ImageData target = new ImageData(_imageUID);
//            string expected = "test";
//            string actual;
//            target.Description = expected;
//            actual = target.Description;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for DisplayData
//        ///</summary>
//        [TestMethod()]
//        public void DisplayDataTest()
//        {
//            ImageData target = new ImageData(_imageUID);
//            DisplayData expected = null;
//            DisplayData actual;
//            target.DisplayData = expected;
//            actual = target.DisplayData;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for FilePath
//        ///</summary>
//        [TestMethod()]
//        public void FilePathTest()
//        {
//            ImageData target = new ImageData(_imageUID);
//            string expected = "test";
//            string actual;
//            target.FilePath = expected;
//            actual = target.FilePath;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for GSPSFilePath
//        ///</summary>
//        [TestMethod()]
//        public void GSPSFilePathTest()
//        {
//            ImageData target = new ImageData(_imageUID);
//            string expected = "test"; 
//            string actual;
//            target.GSPSFilePath = expected;
//            actual = target.GSPSFilePath;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for ImageSource
//        ///</summary>
//        [TestMethod()]
//        public void ImageSourceTest()
//        {
//            ImageData target = new ImageData(_imageUID);
//            ImageSource actual;
//            actual = target.ImageSource;
//        }

//        /// <summary>
//        ///A test for IsDisplayDataCreated
//        ///</summary>
//        [TestMethod()]
//        public void IsDisplayDataCreatedTest()
//        {
//            ImageData target = new ImageData(_imageUID);
//            bool expected = false; 
//            bool actual;
//            target.IsDisplayDataCreated = expected;
//            actual = target.IsDisplayDataCreated;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for SOPInstanceUID
//        ///</summary>
//        [TestMethod()]
//        public void SOPInstanceUIDTest()
//        {
//            ImageData target = new ImageData(_imageUID);
//            string expected = "test"; 
//            string actual;
//            target.SOPInstanceUID = expected;
//            actual = target.SOPInstanceUID;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for SharedMemName
//        ///</summary>
//        [TestMethod()]
//        public void SharedMemNameTest()
//        {
//            ImageData target = new ImageData(_imageUID);
//            string expected = "test"; 
//            string actual;
//            target.SharedMemName = expected;
//            actual = target.SharedMemName;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for Status
//        ///</summary>
//        [TestMethod()]
//        public void StatusTest()
//        {
//            ImageData target = new ImageData(_imageUID);
//            ReviewDataStatus expected = new ReviewDataStatus(); 
//            ReviewDataStatus actual;
//            target.Status = expected;
//            actual = target.Status;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for ThumbnailFilePath
//        ///</summary>
//        [TestMethod()]
//        public void ThumbnailFilePathTest()
//        {
//            ImageData target = new ImageData(_imageUID);
//            string expected = "test";
//            string actual;
//            target.ThumbnailFilePath = expected;
//            actual = target.ThumbnailFilePath;
//            Assert.AreEqual(expected, actual);
//        }
//    }
//}
