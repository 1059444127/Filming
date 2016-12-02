using UIH.Mcsf.Filming.Card.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UIH.Mcsf.Filming.FE.Test_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingViewerCmdHandlerTest and is intended
    ///to contain all FilmingViewerCmdHandlerTest Unit Tests
    ///</summary>
    [TestClass]
    public class FilmingViewerCmdHandlerTest
    {


        #region [--unused--]

        /// <summary>
        ///A test for LoadStudyByInterationInfo
        ///</summary>
        [TestMethod]
        [DeploymentItem("UIH.Mcsf.Filming.FE.Test.dll")]
        public void LoadStudyByInterationInfoTest()
        {

        }

        /// <summary>
        ///A test for LoadStudies
        ///</summary>
        [TestMethod]
        [DeploymentItem("UIH.Mcsf.Filming.FE.Test.dll")]
        public void LoadStudiesTest()
        {

        }

        /// <summary>
        ///A test for HandleCommand
        ///</summary>
        [TestMethod]
        public void HandleCommandTest()
        {
            var target = new CommandHandler(); 
            //CommandContext pContext = null; 
            //ISyncResult pSyncResult = null; 
            int expected = -1; 
            int actual;
            actual = target.HandleCommand(null, null);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for AddEFilmSeriesToFilmPage
        ///</summary>
        [TestMethod]
        [DeploymentItem("UIH.Mcsf.Filming.FE.Test.dll")]
        public void AddEFilmSeriesToFilmPageTest()
        {

        }

        #endregion [--unused--]

    }
}
