using UIH.Mcsf.Filming.Card.View;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Markup;

namespace UIH.Mcsf.Filming.FE.Test_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmPageTest and is intended
    ///to contain all FilmPageTest Unit Tests
    ///</summary>
    [TestClass]
    public class FilmPageTest
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


        #region     [--Unused--]

        ///// <summary>
        /////A test for Film Constructor
        /////</summary>
        //[TestMethod]
        //public void FilmPageConstructorTest()
        //{
        //    new Film();
        //}

        /// <summary>
        ///A test for InitializeComponent
        ///</summary>
        [TestMethod]
        public void InitializeComponentTest()
        {
            var target = new Film();
            target.InitializeComponent();
        }

        /// <summary>
        ///A test for System.Windows.Markup.IComponentConnector.Connect
        ///</summary>
        [TestMethod]
        [DeploymentItem("UIH.Mcsf.Filming.FE.Test.dll")]
        public void ConnectTest()
        {
            IComponentConnector target = new Film();
            const int connectionId = 0;
            target.Connect(connectionId, null);
        }

        #endregion  [--Unused--]


    }
}
