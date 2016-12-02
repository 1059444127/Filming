using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIH.Mcsf.Database;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for ViewModelBaseTest and is intended
    ///to contain all ViewModelBaseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ViewModelBaseTest
    {


        private TestContext _testContextInstance;
        private static string _seriesUID = string.Empty;
        //private static StudyData _studyData;
        private static string _imageUID;

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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            DBWrapper dbWrapper = new DBWrapper();
            if (!dbWrapper.Initialize())
                return;
            var studyList = dbWrapper.GetAllStudyList();
            string studyInstanceUID = string.Empty;
            if (studyList != null && studyList.Count > 0)
                studyInstanceUID = studyList[0].StudyInstanceUID;
            else
            {
                return;
            }

            _seriesUID = string.Empty;
            var seriesList = dbWrapper.GetSeriesListByStudyInstanceUID(studyInstanceUID);
            if (seriesList != null && seriesList.Count > 0)
                _seriesUID = seriesList[0].SeriesInstanceUID;
            else
            {
                return;
            }

            var imageList = dbWrapper.GetImageListBySeriesInstanceUID(_seriesUID);
            if (imageList != null && imageList.Count > 0)
                _imageUID = imageList[0].SOPInstanceUID;

            //_studyData = new StudyData(studyInstanceUID);
        }
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
        ///A test for ViewModelBase Constructor
        ///</summary>
        [TestMethod()]
        public void ViewModelBaseConstructorTest()
        {
            var target = new ViewModelBase();
            Assert.IsNotNull(target);
        }

        ///// <summary>
        /////A test for OnPropertyChanged
        /////</summary>
        //[TestMethod()]
        //public void OnPropertyChangedTest()
        //{
        //    try
        //    {
        //        var series = new SeriesData(_seriesUID);
        //        var parent = new StudyViewModel(_studyData, StudyListDisplayMode.ListMode);
        //        var temp = new SeriesViewModel(series, parent);
        //        var target = new ViewModelBase();
        //        string propName = "ImageSource";
        //        target.OnPropertyChanged(propName);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //    }
            
        //}

        /// <summary>
        ///A test for VerifyPropertyName
        ///</summary>
        [TestMethod()]
        public void VerifyPropertyNameTest()
        {
            try
            {
                ViewModelBase target = new ViewModelBase();
                string propertyName = "test";
                target.VerifyPropertyName(propertyName);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
            
        }
    }
}
