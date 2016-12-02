//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using UIH.Mcsf.Viewer;

//namespace UIH.Mcsf.Filming.CardFE_UT
//{
    
    
//    /// <summary>
//    ///This is a test class for McsfFilmRegionTest and is intended
//    ///to contain all McsfFilmRegionTest Unit Tests
//    ///</summary>
//    [TestClass]
//    public class McsfFilmRegionTest
//    {


//        private TestContext _testContextInstance;

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
//        //public static void MyClassInitialize(TestContext testContext)
//        //{
//        //}
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
//        ///A test for McsfFilmRegion Constructor
//        ///</summary>
//        [TestMethod]
//        public void McsfFilmRegionConstructorTest()
//        {
//            var regionLayoutCell = new MedViewerLayoutCell();
//            var target = new McsfFilmRegion(regionLayoutCell);
//            Assert.IsNotNull(target);
//        }

//        /// <summary>
//        ///A test for GetChildNodeCount
//        ///</summary>
//        [TestMethod]
//        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
//        public void GetChildNodeCountTest()
//        {
//            var regionLayoutCell = new MedViewerLayoutCell();
//            var target = new McsfFilmRegion_Accessor(regionLayoutCell); 
//            MedViewerCellBase cellBase = new MedViewerLayoutCell(); 
//            int actual = target.GetChildNodeCount(cellBase);
//            Assert.IsNotNull(actual);
//        }

//        /// <summary>
//        ///A test for CellLayout
//        ///</summary>
//        [TestMethod]
//        public void CellLayoutTest()
//        {
//            var regionLayoutCell = new MedViewerLayoutCell();
//            var target = new McsfFilmRegion(regionLayoutCell);
//            FilmLayout actual = target.CellLayout;
//            Assert.IsNotNull(actual);
//        }

//        /// <summary>
//        ///A test for Index
//        ///</summary>
//        [TestMethod]
//        public void IndexTest()
//        {
//            var regionLayoutCell = new MedViewerLayoutCell(); 
//            var target = new McsfFilmRegion(regionLayoutCell); 
//            const int expected = 5; 
//            target.Index = expected;
//            int actual = target.Index;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for IsSelected
//        ///</summary>
//        [TestMethod]
//        public void IsSelectedTest()
//        {
//            var regionLayoutCell = new MedViewerLayoutCell();
//            var target = new McsfFilmRegion(regionLayoutCell); 
//            bool expected = false;
//            target.IsSelected = expected;
//            bool actual = target.IsSelected;
//            Assert.AreEqual(expected, actual);

//            expected = true;
//            target.IsSelected = expected;
//            actual = target.IsSelected;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for MaxImagesCount
//        ///</summary>
//        [TestMethod]
//        public void MaxImagesCountTest()
//        {
//            var regionLayoutCell = new MedViewerLayoutCell();
//            var target = new McsfFilmRegion(regionLayoutCell)
//                             {CellLayout = {LayoutType = LayoutTypeEnum.StandardLayout}};
//            int actual = target.MaxImagesCount;
//            Assert.IsNotNull(actual);

//            target.CellLayout.LayoutType = LayoutTypeEnum.DefinedLayout;
//            actual = target.MaxImagesCount;
//            Assert.IsNotNull(actual);
//        }

//        /// <summary>
//        ///A test for RegionLayoutCell
//        ///</summary>
//        [TestMethod]
//        public void RegionLayoutCellTest()
//        {
//            var regionLayoutCell = new MedViewerLayoutCell(); 
//            var target = new McsfFilmRegion(regionLayoutCell);
//            MedViewerLayoutCell actual = target.RegionLayoutCell;
//            Assert.IsNotNull(actual);
//        }
//    }
//}
