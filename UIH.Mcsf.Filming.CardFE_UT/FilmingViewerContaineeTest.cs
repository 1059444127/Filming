using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using System.Windows.Controls;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingViewerContaineeTest and is intended
    ///to contain all FilmingViewerContaineeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmingViewerContaineeTest
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
        ///A test for FilmingViewerContainee Constructor
        ///</summary>
        [TestMethod()]
        public void FilmingViewerContaineeConstructorTest()
        {
            //FilmingViewerContainee target = new FilmingViewerContainee();
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for DoWork
        ///</summary>
        [TestMethod()]
        public void DoWorkTest()
        {
            try
            {
                //FilmingViewerContainee target = new FilmingViewerContainee(); // 
                //target.DoWork();
                //Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for InitFilmingViewerApp
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void InitFilmingViewerAppTest()
        {
            try
            {
                //PrivateType type = new PrivateType(typeof(FilmingViewerContainee));
                //PrivateObject privateObj = new PrivateObject(new FilmingViewerContainee(), type);

                //FilmingViewerContainee_Accessor target = new FilmingViewerContainee_Accessor(param0); // 
                //target.InitFilmingViewerApp();
                //privateObj.Invoke("InitFilmingViewerApp");
                //Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for SendCommand
        ///</summary>
        [TestMethod()]
        public void SendCommandTest()
        {
            object[] param = new object[7];
            param[0] = (int)CommandID.SAVE_EFILMS_COMMAND;
            param[1] = CommandType.AsyncCommand;
            //param[2] = "FilmingFE";
            param[4] = null;
            param[5] = null;
            param[6] = "";

            param[3] = "";
            //object expected = null; // 
           // object actual;
            //actual = FilmingViewerContainee.SendCommand(param);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Shutdown
        ///</summary>
        [TestMethod()]
        public void ShutdownTest()
        {
            try
            {
                //FilmingViewerContainee target = new FilmingViewerContainee(); // 
                //bool expected = false; // 
                //bool actual;
                //actual = target.Shutdown(false);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Startup
        ///</summary>
        [TestMethod()]
        public void StartupTest()
        {
            try
            {
                //FilmingViewerContainee target = new FilmingViewerContainee(); // 
                //target.Startup();
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CommunicationProxyName
        ///</summary>
        [TestMethod()]
        public void CommunicationProxyNameTest()
        {
            try
            {
               // string actual;
                //actual = FilmingViewerContainee.CommunicationProxyName;
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FilmingResourceDict
        ///</summary>
        [TestMethod()]
        public void FilmingResourceDictTest()
        {
            try
            {
               // ResourceDictionary actual;
                //actual = FilmingViewerContainee.FilmingResourceDict;
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FilmingViewerWindow
        ///</summary>
        [TestMethod()]
        public void FilmingViewerWindowTest()
        {
            try
            {
                //UserControl expected = null; // 
                //UserControl actual;
                //FilmingViewerContainee.FilmingViewerWindow = expected;
                //actual = FilmingViewerContainee.FilmingViewerWindow;
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Main
        ///</summary>
        [TestMethod()]
        public void MainTest()
        {
            try
            {
                //FilmingViewerContainee expected = null; // 
                //FilmingViewerContainee actual;
                //FilmingViewerContainee.Main = expected;
                //actual = FilmingViewerContainee.Main;
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ShowStatusWarning
        ///</summary>
        [TestMethod()]
        public void ShowStatusWarningTest()
        {
            //string key = string.Empty; // 
            //object[] args = null; // 
            //FilmingViewerContainee.ShowStatusWarning(key, args);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ShowStatusInfo
        ///</summary>
        [TestMethod()]
        public void ShowStatusInfoTest()
        {
            //string key = string.Empty; // 
            //object[] args = null; // 
            //FilmingViewerContainee.Main.ShowStatusInfo(key, args);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
