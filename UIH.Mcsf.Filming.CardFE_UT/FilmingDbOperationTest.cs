using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UIH.Mcsf.Database;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingDbOperationTest and is intended
    ///to contain all FilmingDbOperationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmingDbOperationTest
    {


        private TestContext _testContextInstance;
        private static string _seriesUID = string.Empty;
        //private static StudyData _studyData;
        private static string _imageUID;
        private static string _studyInstanceUID = string.Empty;

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
            try
            {
                DBWrapper dbWrapper = new DBWrapper();
                if (!dbWrapper.Initialize())
                    return;
                var studyList = dbWrapper.GetAllStudyList();
                if (studyList.Count > 0)
                    _studyInstanceUID = studyList[0].StudyInstanceUID;
                else
                {
                    return;
                }

                _seriesUID = string.Empty;
                var seriesList = dbWrapper.GetSeriesListByStudyInstanceUID(_studyInstanceUID);
                if (seriesList.Count > 0)
                    _seriesUID = seriesList[0].SeriesInstanceUID;
                else
                {
                    return;
                }

                var imageList = dbWrapper.GetImageListBySeriesInstanceUID(_seriesUID);
                if (imageList.Count > 0)
                    _imageUID = imageList[0].SOPInstanceUID;

                //_studyData = new StudyData(_studyInstanceUID);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
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
        ///A test for FilmingDbOperation Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void FilmingDbOperationConstructorTest()
        {
            //PrivateType type = new PrivateType(typeof(FilmingDbOperation));                             //Class1为要测试的类。
            //PrivateObject param0 = new PrivateObject(FilmingDbOperation.Instance, type);

            //FilmingDbOperation_Accessor target = new FilmingDbOperation_Accessor(param0);
            //Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for LoadAllStudy
        ///</summary>
        //[TestMethod()]
        //public void LoadAllStudyTest()
        //{
        //    try
        //    {
        //        FilmingDbOperation target = FilmingDbOperation.Instance;
        //        target.LoadAllStudy();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //    }
        //}

        /// <summary>
        ///A test for LoadStudy
        ///</summary>
        [TestMethod()]
        public void LoadStudyTest()
        {
            //FilmingDbOperation target = FilmingDbOperation.Instance;
            //target.LoadStudy(_studyInstanceUID);
        }

        /// <summary>
        ///A test for FilmingDBWrapper
        ///</summary>
        [TestMethod()]
        public void FilmingDBWrapperTest()
        {
            //FilmingDbOperation target = FilmingDbOperation.Instance;
            //DBWrapper actual;
            //actual = target.FilmingDBWrapper;
            //Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for FilmingDataRepositry
        ///</summary>
        [TestMethod()]
        public void FilmingDataRepositryTest()
        {
            //FilmingDbOperation target = FilmingDbOperation.Instance;
            //DataRepositry expected = null;
            //DataRepositry actual;
            //target.FilmingDataRepositry = expected;
            //actual = target.FilmingDataRepositry;
            //Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Instance
        ///</summary>
        [TestMethod()]
        public void InstanceTest()
        {
            //FilmingDbOperation actual;
            //actual = FilmingDbOperation.Instance;
            //Assert.IsNotNull(actual);
        }
    }
}
