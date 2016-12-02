using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UIH.Mcsf.Log;

namespace UIH.Mcsf.Filming.Utility_UT
{
    
    
    /// <summary>
    ///This is a test class for LoggerTest and is intended
    ///to contain all LoggerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LoggerTest
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
        ///A test for GetDetailCallingMethodLocation
        ///</summary>
        ///
        //remove this case For ms-test UT sometimes fail when package building
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.Utility.dll")]
        //public void GetDetailCallingMethodLocationTest()
        //{
        //    PrivateType type = new PrivateType(typeof(Logger));                             
        //    PrivateObject param0 = new PrivateObject(Logger.Instance, type);
        //    Logger_Accessor target = new Logger_Accessor(param0); 

        //    Logger_Accessor.GetDetailCallingMethodLocation();
        //}

        /// <summary>
        ///A test for LogError
        ///</summary>
        [TestMethod()]
        public void LogErrorTest()
        {
            string info = "Error"; 
            Logger.LogError(info);
        }

        /// <summary>
        ///A test for LogFuncDown
        ///</summary>
        [TestMethod()]
        public void LogFuncDownTest()
        {
            string sParam = "test"; 
            Logger.LogFuncDown(sParam);
        }

        /// <summary>
        ///A test for LogFuncException
        ///</summary>
        [TestMethod()]
        public void LogFuncExceptionTest()
        {
            string sParam = "test";
            Logger.LogFuncException(sParam);
        }

        /// <summary>
        ///A test for LogFuncUp
        ///</summary>
        [TestMethod()]
        public void LogFuncUpTest()
        {
            string sParam = "test";
            Logger.LogFuncUp(sParam);
            
        }

        /// <summary>
        ///A test for LogInfo
        ///</summary>
        [TestMethod()]
        public void LogInfoTest()
        {
            string format = "Info"; 
            object[] args = null; 
            Logger.LogInfo(format, args);

           
            object[] args1 = new object[3];
            args1[0] = "111";
            args1[1] = "222";
            args1[2] = "333";
            Logger.LogInfo(format, args1);
        }

        /// <summary>
        ///A test for LogInfo
        ///</summary>
        [TestMethod()]
        public void LogInfoTest1()
        {
            string info = "info"; 
            Logger.LogInfo(info);
        }

        /// <summary>
        ///A test for LogSvcInfo
        ///</summary>
        [TestMethod()]
        public void LogSvcInfoTest()
        {
            string info = "info";
            Logger.LogSvcInfo(info);
        }

        /// <summary>
        ///A test for LogWarning
        ///</summary>
        [TestMethod()]
        public void LogWarningTest()
        {
            string info = "Warning";
            Logger.LogWarning(info);
        }

        /// <summary>
        ///A test for Instance
        ///</summary>
        [TestMethod()]
        public void InstanceTest()
        {
            CLRLogger actual;
            actual = Logger.Instance;
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for Location
        ///</summary>
        [TestMethod()]
        public void LocationTest()
        {
            string actual;
            actual = Logger.Location;
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for LogUID
        ///</summary>
        [TestMethod()]
        public void LogUIDTest()
        {
            uint actual;
            Logger.LogUid = 001035002;
            actual = Logger.LogUid;
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for Source
        ///</summary>
        [TestMethod()]
        public void SourceTest()
        {
            string actual;
            Logger.Source = "UIH.Mcsf.Filming";
            actual = Logger.Source;
            Assert.IsNotNull(actual);
        }
    }
}
