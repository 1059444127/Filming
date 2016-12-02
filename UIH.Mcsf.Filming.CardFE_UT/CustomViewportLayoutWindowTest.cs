//////////////////////////////////////////////////////////////////////////
/// 2013. 5.31 uih.mcsf.commonTheme resource key duplicated， and then ut failed
using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Controls;
using UIH.Mcsf.Filming.CustomizeLayout;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.MHC;

namespace UIH.Mcsf.Filming.CardFE_UT
{


    /// <summary>
    ///This is a test class for CustomViewportLayoutWindowTest and is intended
    ///to contain all CustomViewportLayoutWindowTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CustomViewportLayoutWindowTest
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
        ///A test for CustomViewportLayoutWindow Constructor
        ///</summary>
        [TestMethod()]
        public void CustomViewportLayoutWindowConstructorTest()
        {
            try
            {
                CustomViewportLayoutWindow target = new CustomViewportLayoutWindow();
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for InitializeComponent
        ///</summary>
        [TestMethod()]
        public void InitializeComponentTest()
        {
            try
            {
                CustomViewportLayoutWindow target = new CustomViewportLayoutWindow(); // 
                target.InitializeComponent();
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnApplyCustomViewportLayoutButtonClick
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnApplyCustomViewportLayoutButtonClickTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(CustomViewportLayoutWindow));
                PrivateObject privateObj = new PrivateObject(new CustomViewportLayoutWindow(), type);

                //CustomViewportLayoutWindow_Accessor target = new CustomViewportLayoutWindow_Accessor(param0); // 
                object sender = null; // 
                RoutedEventArgs e = null; // 
                //target.OnApplyCustomViewportLayoutButtonClick(sender, e);
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("OnApplyCustomViewportLayoutButtonClick", myArgs);
                //Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for OnDeleteCustomViewportLayoutButtonClick
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnDeleteCustomViewportLayoutButtonClickTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(CustomViewportLayoutWindow));
                PrivateObject privateObj = new PrivateObject(new CustomViewportLayoutWindow(), type);

                //CustomViewportLayoutWindow_Accessor target = new CustomViewportLayoutWindow_Accessor(param0); // 
                object sender = null; // 
                RoutedEventArgs e = null; // 
                //target.OnDeleteCustomViewportLayoutButtonClick(sender, e);
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("OnDeleteCustomViewportLayoutButtonClick", myArgs);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnQuitButtonClick
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnQuitButtonClickTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(CustomViewportLayoutWindow));
                PrivateObject privateObj = new PrivateObject(new CustomViewportLayoutWindow(), type);

                //CustomViewportLayoutWindow_Accessor target = new CustomViewportLayoutWindow_Accessor(param0); // 
                object sender = null; // 
                RoutedEventArgs e = null; // 
                //target.OnQuitButtonClick(sender, e);
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("OnQuitButtonClick", myArgs);
                //Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for OnSaveCustomViewportLayoutButtonClick
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnSaveCustomViewportLayoutButtonClickTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(CustomViewportLayoutWindow));
                PrivateObject privateObj = new PrivateObject(new CustomViewportLayoutWindow(), type);

                //CustomViewportLayoutWindow_Accessor target = new CustomViewportLayoutWindow_Accessor(param0); // 
                object sender = null; // 
                RoutedEventArgs e = null; // 
                //target.OnSaveCustomViewportLayoutButtonClick(sender, e);
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("OnSaveCustomViewportLayoutButtonClick", myArgs);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnWindowClosing
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void OnWindowClosingTest()
        //{
        //    CustomViewportLayoutWindow_Accessor target = new CustomViewportLayoutWindow_Accessor(); // 
        //    object sender = null; // 
        //    CancelEventArgs e = new CancelEventArgs(); // 
        //    target.OnWindowClosing(sender, e);
        //    //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for System.Windows.Markup.IComponentConnector.Connect
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void ConnectTest()
        {
            try
            {
                IComponentConnector target = new CustomViewportLayoutWindow(); // 
                int connectionId = 0; // 
                object target1 = null; // 
                target.Connect(connectionId, target1);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for customViewportListBox_SelectionChanged
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void customViewportListBox_SelectionChangedTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(CustomViewportLayoutWindow));
                PrivateObject privateObj = new PrivateObject(new CustomViewportLayoutWindow(), type);

                //CustomViewportLayoutWindow_Accessor target = new CustomViewportLayoutWindow_Accessor(param0); // 
                object sender = null; // 
                SelectionChangedEventArgs e = null; // 
                //target.customViewportListBox_SelectionChanged(sender, e);
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("customViewportListBox_SelectionChanged", myArgs);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ActiveFilmLayout
        ///</summary>
        [TestMethod()]
        public void ActiveFilmLayoutTest()
        {
            try
            {
                CustomViewportLayoutWindow target = new CustomViewportLayoutWindow(); // 
                FilmLayout expected = null; // 
                FilmLayout actual;
                target.ActiveFilmLayout = expected;
                actual = target.ActiveFilmLayout;
                Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for CapturedFilmLayout
        ///</summary>
        [TestMethod()]
        public void CapturedFilmLayoutTest()
        {
            try
            {
                CustomViewportLayoutWindow target = new CustomViewportLayoutWindow(); // 
                FilmLayout expected = null; // 
                FilmLayout actual;
                target.CapturedFilmLayout = expected;
                actual = target.CapturedFilmLayout;
                Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for CustomViewportViewModel
        ///</summary>
        [TestMethod()]
        public void CustomViewportViewModelTest()
        {
            try
            {
                CustomViewportLayoutWindow target = new CustomViewportLayoutWindow(); // 
                CustomViewportViewModel actual;
                actual = target.CustomViewportViewModel;
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SaveLayoutHandler
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void SaveLayoutHandlerTest()
        {
            try
            {
                //PrivateType type = new PrivateType(typeof(CustomViewportLayoutWindow));
                //PrivateObject privateObj = new PrivateObject(new CustomViewportLayoutWindow(), type);
                ////CustomViewportLayoutWindow_Accessor target = new CustomViewportLayoutWindow_Accessor(param0);

                //MessageBoxResponse res = MessageBoxResponse.YES;
                ////target.SaveLayoutHandler(res);
                //object[] myArgs = new object[] { res };
                //privateObj.Invoke("SaveLayoutHandler", myArgs);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }

        }
    }
}
