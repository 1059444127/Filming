using McsfCommonSave;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIH.Mcsf.Core;
using System.Collections.Generic;
using UIH.Mcsf.Filming;
using System;

namespace UIH.Mcsf.Filming.Utility_UT
{
    
    
    /// <summary>
    ///This is a test class for JobCreatorTest and is intended
    ///to contain all JobCreatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class JobCreatorTest
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
        ///A test for JobCreator Constructor
        ///</summary>
        [TestMethod()]
        public void JobCreatorConstructorTest()
        {
            JobCreator target = new JobCreator();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for CreateFilmingJobInstance
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.Utility.dll")]
        //public void CreateFilmingJobInstanceTest()
        //{
        //    PrivateType type = new PrivateType(typeof(JobCreator));                             //Class1为要测试的类。
        //    JobCreator target = new JobCreator();
        //    PrivateObject privateObj = new PrivateObject(target, type);

        //    //JobCreator_Accessor target = new JobCreator_Accessor(param0); 
        //    byte[] expected = null; 
        //    byte[] actual;
        //    //actual = target.CreateFilmingJobInstance();
        //    actual = (byte[])privateObj.Invoke("CreateFilmingJobInstance");

        //    Assert.AreEqual(expected, actual);

        //    target.Peer.PeerAE = "test";
        //    target.Peer.PeerIP = "1.2.3";
        //    target.Peer.PeerPort = 5;
        //    //actual = target.CreateFilmingJobInstance();
        //    actual = (byte[])privateObj.Invoke("CreateFilmingJobInstance");
        //    Assert.IsNotNull(actual);
        //}

        /// <summary>
        ///A test for SendFilmingJobCommand
        ///</summary>


        /// <key>\n 
        /// PRA:Yes \n
        /// Traced from: N/A \n
        /// Tests: DS_PRA_Filming_FilmingCommand \n
        /// Description: Test for Sending Filming Job Info to Backend Printer Serives \n
        /// Short Description:FilmingCommandTest \n
        /// Component:Filming \n
        /// </key> \n

        //[TestMethod()]
        //public void SendFilmingJobCommandTest()
        //{
        //    JobCreator target = new JobCreator();
        //    var filmBox = new FilmingPrintJob.Types.FilmBox.Builder();
        //    target.FilmBoxList.Add(filmBox);
        //    ICommunicationProxy proxy = null;
        //    target.SendFilmingJobCommand(proxy);

        //    target.Peer.PeerAE = "test";
        //    target.Peer.PeerIP = "1.2.3";
        //    target.Peer.PeerPort = 5;
        //    target.SendFilmingJobCommand(proxy);

        //}

        /// <summary>
        ///A test for FilmBoxList
        ///</summary>
        [TestMethod()]
        public void FilmBoxListTest()
        {
            JobCreator target = new JobCreator(); 
            IList<FilmingPrintJob.Types.FilmBox.Builder> expected = null; 
            IList<FilmingPrintJob.Types.FilmBox.Builder> actual;
            target.FilmBoxList = expected;
            actual = target.FilmBoxList;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Patient
        ///</summary>
        [TestMethod()]
        public void PatientTest()
        {
            JobCreator target = new JobCreator(); 
            Patient expected = null;
            Patient actual;
            target.Patient = expected;
            actual = target.Patient;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Peer
        ///</summary>
        [TestMethod()]
        public void PeerTest()
        {
            JobCreator target = new JobCreator(); 
            PeerNode expected = null; 
            PeerNode actual;
            target.Peer = expected;
            actual = target.Peer;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PrintSetting
        ///</summary>
        [TestMethod()]
        public void PrintSettingTest()
        {
            JobCreator target = new JobCreator(); 
            PrintSetting expected = null; 
            PrintSetting actual;
            target.PrintSetting = expected;
            actual = target.PrintSetting;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ArchivedSeriesInstanceUid
        ///</summary>
        [TestMethod()]
        public void ArchivedSeriesInstanceUidTest()
        {
            JobCreator target = new JobCreator(); 
            string expected = string.Empty; 
            string actual;
            target.ArchivedSeriesInstanceUid = expected;
            actual = target.ArchivedSeriesInstanceUid;
            Assert.AreEqual(expected, actual);
        }
    }
}
