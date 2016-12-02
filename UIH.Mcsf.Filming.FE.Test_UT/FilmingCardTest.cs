using UIH.Mcsf.Filming.Card.View;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UIH.Mcsf.Filming.FE.Test_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingCardTest and is intended
    ///to contain all FilmingCardTest Unit Tests
    ///</summary>
    [TestClass]
    public class FilmingCardTest
    {
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


        #region [--unused--]

        ///// <summary>
        /////A test for Card Constructor
        /////</summary>
        //[TestMethod()]
        //public void FilmingCardConstructorTest()
        //{
        //    var target = new Card();
        //}

        /// <summary>
        ///A test for InitializeComponent
        ///</summary>
        [TestMethod]
        public void InitializeComponentTest()
        {
            var target = new Card.View.Card();
            target.InitializeComponent();
        }

        #endregion [--unused--]

    }
}
