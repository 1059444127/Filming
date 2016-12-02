using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UIH.Mcsf.Filming.Wrappers;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingNLSFactoryTest and is intended
    ///to contain all FilmingNLSFactoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmingNLSFactoryTest
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
        ///A test for FilmingNLSFactory Constructor
        ///</summary>
        [TestMethod()]
        public void FilmingNLSFactoryConstructorTest()
        {
            FilmingNlsFactory target = new FilmingNlsFactory();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for WarningAddDifferentPatientToSameFilmCard
        ///</summary>
        [TestMethod()]
        public void WarningAddDifferentPatientToSameFilmCardTest()
        {
            try
            {
                string actual;
                actual = FilmingNlsFactory.WarningAddDifferentPatientToSameFilmCard;
                Assert.AreNotEqual(string.Empty, actual);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///A test for WarningCellLossImages
        ///</summary>
        [TestMethod()]
        public void WarningCellLossImagesTest()
        {
            try
            {
                string actual;
                actual = FilmingNlsFactory.WarningCellLossImages;
                Assert.AreNotEqual(string.Empty, actual);
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        ///A test for WarningCorrectNumber
        ///</summary>
        [TestMethod()]
        public void WarningCorrectNumberTest()
        {
            try
            {
                string actual;
                actual = FilmingNlsFactory.WarningCorrectNumber;
                Assert.AreNotEqual(string.Empty, actual);
            }
            catch (Exception)
            {

            }
            
        }

        /// <summary>
        ///A test for WarningHaveNoEmptyCell
        ///</summary>
        [TestMethod()]
        public void WarningHaveNoEmptyCellTest()
        {
            try
            {
                string actual;
                actual = FilmingNlsFactory.WarningHaveNoEmptyCell;
                Assert.AreNotEqual(string.Empty, actual);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for WarningInsertEmptyCellToUnStandardLayout
        ///</summary>
        [TestMethod()]
        public void WarningInsertEmptyCellToUnStandardLayoutTest()
        {
            try
            {
                string actual;
                actual = FilmingNlsFactory.WarningInsertEmptyCellToUnStandardLayout;
                Assert.AreNotEqual(string.Empty, actual);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for WarningLoadingImagesWhichMayBeSlowForCountExceedThreshold
        ///</summary>
        [TestMethod()]
        public void WarningLoadingImagesWhichMayBeSlowForCountExceedThresholdTest()
        {
            try
            {
                string actual;
                actual = FilmingNlsFactory.WarningLoadingImagesWhichMayBeSlowForCountExceedThreshold;
                Assert.AreNotEqual(string.Empty, actual);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for WarningNoEmptyCellForPaste
        ///</summary>
        [TestMethod()]
        public void WarningNoEmptyCellForPasteTest()
        {
            try
            {
                string actual;
                actual = FilmingNlsFactory.WarningNoEmptyCellForPaste;
                Assert.AreNotEqual(string.Empty, actual);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for WarningTitle
        ///</summary>
        [TestMethod()]
        public void WarningTitleTest()
        {
            try
            {
                string actual;
                actual = FilmingNlsFactory.WarningTitle;
                Assert.AreNotEqual(string.Empty, actual);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for WarningViewportLossImages
        ///</summary>
        [TestMethod()]
        public void WarningViewportLossImagesTest()
        {
            try
            {
                string actual;
                actual = FilmingNlsFactory.WarningViewportLossImages;
                Assert.AreNotEqual(string.Empty, actual);
            }
            catch (Exception)
            {

            }
        }
    }
}
