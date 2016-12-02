//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Windows.Media;
//using System.Collections.Generic;
//using UIH.Mcsf.Database;

//namespace UIH.Mcsf.Filming.CardFE_UT
//{
    
    
//    /// <summary>
//    ///This is a test class for SeriesDataTest and is intended
//    ///to contain all SeriesDataTest Unit Tests
//    ///</summary>
//    [TestClass]
//    public class SeriesDataTest
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
//        [ClassInitialize()]
//        public static void MyClassInitialize(TestContext testContext)
//        {
//            DBWrapper dbWrapper = new DBWrapper();
//            if (!dbWrapper.Initialize())
//                return;
//            var studyList = dbWrapper.GetAllStudyList();
//            string studyInstanceUID = string.Empty;
//            if (studyList != null && studyList.Count > 0)
//                studyInstanceUID = studyList[0].StudyInstanceUID;
//            else
//            {
//                return;
//            }

//            _seriesUID = string.Empty;
//            var seriesList = dbWrapper.GetSeriesListByStudyInstanceUID(studyInstanceUID);
//            if (seriesList != null && seriesList.Count > 0)
//                _seriesUID = seriesList[0].SeriesInstanceUID;
//            else
//            {
//                return;
//            }

//            var imageList = dbWrapper.GetImageListBySeriesInstanceUID(_seriesUID);
//            if (imageList != null && imageList.Count > 0)
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


//        [TestMethod]
//        public void SeriesDataConstructorTest()
//        {
//            string seriesUid = _seriesUID; 
//            string seriesDesc = "test"; 
//            ImageSource source = null;
//            var target = new SeriesData(seriesUid, seriesDesc, source);
//            Assert.IsNotNull(target);
//        }

//        /// <summary>
//        ///A test for SeriesData Constructor
//        ///</summary>
//        [TestMethod]
//        public void SeriesDataConstructorTest1()
//        {
//            var target = new SeriesData(_seriesUID); 
//            Assert.IsNotNull(target);
//        }

//        /// <summary>
//        ///A test for SetViewportSelected
//        ///</summary>
//        [TestMethod]
//        public void SetViewportSelectedTest()
//        {
//            var target = new SeriesData(_seriesUID); 
//            int index = 1; 
//            target.SetViewportSelected(index, false);
            
//            target.SetViewportSelected(index,true);
//        }

//        /// <summary>
//        ///A test for ToString
//        ///</summary>
//        [TestMethod]
//        public void ToStringTest()
//        {
//            var target = new SeriesData(_seriesUID); 
//            target.SeriesNumber = "222";
//            target.SeriesInstanceUID = "111";
//            string expected = "SeriesUID=111;SeriesNumber222";
//            string actual;
//            actual = target.ToString();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for CanSupport3D
//        ///</summary>
//        [TestMethod]
//        public void CanSupport3DTest()
//        {
//            var target = new SeriesData(_seriesUID); 
//            bool expected = false; 
//            bool actual;
//            target.CanSupport3D = expected;
//            actual = target.CanSupport3D;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for ClientNum
//        ///</summary>
//        [TestMethod]
//        public void ClientNumTest()
//        {
//            var target = new SeriesData(_seriesUID);  
//            int expected = 2; 
//            int actual;
//            target.ClientNum = expected;
//            actual = target.ClientNum;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for Description
//        ///</summary>
//        [TestMethod]
//        public void DescriptionTest()
//        {
//            var target = new SeriesData(_seriesUID); 
//            string expected = "Test"; 
//            string actual;
//            target.SeriesDescription = expected;
//            actual = target.SeriesDescription;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for Images
//        ///</summary>
//        [TestMethod]
//        public void ImagesTest()
//        {
//            var target = new SeriesData(_seriesUID); 
//            List<ImageData> expected = null; 
//            List<ImageData> actual;
//            target.Images = expected;
//            actual = target.Images;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for IsSelected
//        ///</summary>
//        [TestMethod]
//        public void IsSelectedTest()
//        {
//            var target = new SeriesData(_seriesUID); 
//            bool expected = false; 
//            bool actual;
//            target.IsSelected = expected;
//            actual = target.IsSelected;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for SeriesImage
//        ///</summary>
//        [TestMethod]
//        public void SeriesImageTest()
//        {
//            var target = new SeriesData(_seriesUID); 
//            ImageSource expected = null; 
//            ImageSource actual;
//            target.SeriesImage = expected;
//            actual = target.SeriesImage;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for SeriesInstanceUID
//        ///</summary>
//        [TestMethod]
//        public void SeriesInstanceUIDTest()
//        {
//            var target = new SeriesData(_seriesUID); 
//            string expected = "Test"; 
//            string actual;
//            target.SeriesInstanceUID = expected;
//            actual = target.SeriesInstanceUID;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for SeriesNumber
//        ///</summary>
//        [TestMethod]
//        public void SeriesNumberTest()
//        {
//            var target = new SeriesData(_seriesUID); 
//            string expected = "Test"; 
//            string actual;
//            target.SeriesNumber = expected;
//            actual = target.SeriesNumber;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for Status
//        ///</summary>
//        [TestMethod]
//        public void StatusTest()
//        {
//            var target = new SeriesData(_seriesUID); 
//            ReviewDataStatus expected = ReviewDataStatus.Initializing; 
//            ReviewDataStatus actual;
//            target.Status = expected;
//            actual = target.Status;
//            Assert.AreEqual(expected, actual);
//        }
//    }
//}
