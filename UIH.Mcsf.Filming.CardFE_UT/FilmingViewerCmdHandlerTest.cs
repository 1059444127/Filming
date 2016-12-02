using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.ImageManager;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingViewerCmdHandlerTest and is intended
    ///to contain all FilmingViewerCmdHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmingViewerCmdHandlerTest
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
        ///A test for FilmingViewerCmdHandler Constructor
        ///</summary>
        [TestMethod()]
        public void FilmingViewerCmdHandlerConstructorTest()
        {
            try
            {
                //FilmingViewerCmdHandler target = new FilmingViewerCmdHandler();
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        ///// <summary>
        /////A test for HandleCommand
        /////</summary>
        //[TestMethod()]
        //public void HandleCommandTest()
        //{
        //    FilmingViewerCmdHandler target = new FilmingViewerCmdHandler(); // 
        //    CommandContext pContext = null; // 
        //    ISyncResult pSyncResult = null; // 
        //    //int expected = 0; // 
        //    int actual;
        //    actual = target.HandleCommand(pContext, pSyncResult);
        //    //Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");

        //    pContext = new CommandContext();
        //    pContext.iCommandId = (int)CommandID.SWITCH_TO_APPLICATION;
        //    target.HandleCommand(pContext, pSyncResult);
        //}

        /// <summary>
        ///A test for LoadStudy
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void LoadStudiesTest()
        {
            PrivateType type = new PrivateType(typeof(FilmingViewerCmdHandler));
            PrivateObject privateObj = new PrivateObject(new FilmingViewerCmdHandler(), type);

            //FilmingViewerCmdHandler_Accessor target = new FilmingViewerCmdHandler_Accessor(param0);
            IList<string> studyInstanceUIDList=null;
            //target.LoadStudies(studyInstanceUIDList);
            object[] myArgs = new object[] { studyInstanceUIDList };
            privateObj.Invoke("LoadStudies", myArgs);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for LoadStudyByInterationInfo
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void LoadStudyByInterationInfoTest()
        {
            //PrivateType type = new PrivateType(typeof(SwitchToFilmingCommandHandler));
            //PrivateObject privateObj = new PrivateObject(new SwitchToFilmingCommandHandler(null), type);

            ////FilmingViewerCmdHandler_Accessor target = new FilmingViewerCmdHandler_Accessor(param0); // 
            //byte[] interationStudyInfo = null; // 
            ////target.LoadStudyByInterationInfo(interationStudyInfo);
            //object[] myArgs = new object[] { interationStudyInfo };
            //privateObj.Invoke("LoadStudyByInterationInfo", myArgs);
            ////Assert.Inconclusive("A method that does not return a value cannot be verified.");
            var handler = new SwitchToFilmingCommandHandler(null);
            handler.HandleCommand();
        }
    }
}
