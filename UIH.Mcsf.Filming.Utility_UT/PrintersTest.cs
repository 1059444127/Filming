using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UIH.Mcsf.Filming;
using System;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;

namespace UIH.Mcsf.Filming.Utility_UT
{
    
    
    /// <summary>
    ///This is a test class for PrintersTest and is intended
    ///to contain all PrintersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PrintersTest
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
        ///A test for Printers Constructor
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.Utility.dll")]
        //public void PrintersConstructorTest()
        //{
        //    PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
        //    PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

        //    Printers_Accessor target = new Printers_Accessor(privateObj);
        //    Assert.IsNotNull(target);
        //}

        /// <summary>
        ///A test for GetPacsNodeParametersByAE
        ///</summary>
        [TestMethod()]
        public void GetPacsNodeParametersByAETest()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            Printers target = Printers.Instance;

            string sNodeAE = "LEAD_SERVER"; // 
            var peerNode = new PeerNode();
            var peerNodeExpected = new PeerNode { PeerAE = "LEAD_SERVER", PeerIP = "10.1.3.194", PeerPort = 10006};
            int expected = 0; 
            int actual;
            //actual = target.GetPacsNodeParametersByAE(sNodeAE, ref peerNode);
            //Assert.AreEqual(peerNodeExpected.PeerAE,peerNode.PeerAE);
            //Assert.AreEqual(peerNodeExpected.PeerIP,peerNode.PeerIP);
            //Assert.AreEqual(peerNodeExpected.PeerPort,peerNode.PeerPort);
            //Assert.AreEqual(expected, actual);

            sNodeAE = "aa";
            peerNode = new PeerNode();
            actual = target.GetPacsNodeParametersByAE(sNodeAE, ref peerNode);
            expected = -1;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ParseFilmingConfig
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.Utility.dll")]
        public void ParseFilmingConfigTest()
        {
            PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj); 

            //target.ParseFilmingConfig();
            privateObj.Invoke("ParseFilmingConfig");

           
        }

        /// <summary>
        ///A test for ParsePrinterConfig
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.Utility.dll")]
        public void ParsePrinterConfigTest()
        {
            PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj); 
            //target.ParsePrinterConfig();
            privateObj.Invoke("ParsePrinterConfig");

            XmlDocument doc = new XmlDocument();
          //  doc.Load(_entryPrinterConfigPath);

            XPathNavigator xNav = doc.CreateNavigator();

            XPathNodeIterator xPathIt = xNav.Select("//printer/printerID");
           
            //target.ParsePrinterConfig();
            privateObj.Invoke("ParsePrinterConfig");


        }

        /// <summary>
        ///A test for AutoFilmStrategy
        ///</summary>
        [TestMethod()]
        public void AutoFilmStrategyTest()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            var target = Printers.Instance;
            EnumAutoFilmStrategy actual;
            actual = target.AutoFilmStrategy;
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for CanAutoFilming
        ///</summary>
        [TestMethod()]
        public void CanAutoFilming_True_Setting_Test()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            var target = Printers.Instance;
            bool expected = true; 
            bool actual;
            target.CanAutoFilming = expected;
            actual = target.CanAutoFilming;
            //Assert.AreEqual(expected, actual);;
        }

        /// <summary>
        ///A test for CanAutoFilming
        ///</summary>
        [TestMethod()]
        public void CanAutoFilming_False_Setting_Test()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            var target = Printers.Instance;
            bool expected = false;
            bool actual;
            target.CanAutoFilming = expected;
            actual = target.CanAutoFilming;
            //Assert.AreEqual(expected, actual); ;
        }
        /// <summary>
        ///A test for DefaultAE
        ///</summary>
        [TestMethod()]
        public void DefaultAETest()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            var target = Printers.Instance;
            string expected = "LEAD_SERVER"; 
            string actual;
            target.DefaultAE = expected;
            actual = target.DefaultAE;
            //Assert.AreEqual(expected, actual);

            expected = "LEAD1_SERVER";
            target.DefaultAE = expected;
            actual = target.DefaultAE;
            //Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Instance
        ///</summary>
        [TestMethod()]
        public void InstanceTest()
        {
            Printers actual;
            actual = Printers.Instance;
            Assert.IsNotNull(actual);
        }

        /// <key>\n 
        /// PRA:Yes \n
        /// Traced from: N/A \n
        /// Tests: DS_PRA_Filming_LutDirectory \n
        /// Description: lut file directory property test \n
        /// Short Description: LutDirectoryTest \n
        /// Component: Filming \n
        /// </key> \n
        /// <summary>
        ///A test for LutFileDirectory
        ///</summary>
        [TestMethod()]
        public void LutFileDirectoryTest()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            var target = Printers.Instance;
            string actual;
            actual = target.LutFileDirectory;
            //Assert.IsNull(actual);
        }

        /// <key>\n 
        /// PRA:Yes \n
        /// Traced from: N/A \n
        /// Tests: DS_PRA_Filming_LutFiles \n
        /// Description: lut files  property test \n
        /// Short Description: LutFilesTest \n
        /// Component: Filming \n
        /// </key> \n
        /// <summary>
        ///A test for LutFiles
        ///</summary>
        [TestMethod()]
        public void LutFilesTest()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            var target = Printers.Instance;
            List<string> actual;
            actual = target.LutFiles;
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for OurAE
        ///</summary>
        [TestMethod()]
        public void OurAETest()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj); 
            var target = Printers.Instance;
            string expected = "test"; 
            string actual;
            target.OurAE = expected;
            actual = target.OurAE;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for OurPort
        ///</summary>
        [TestMethod()]
        public void OurPortTest()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj); 
            var target = Printers.Instance;
            ushort expected = 222;
            ushort actual;
            target.OurPort = expected;
            actual = target.OurPort;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PeerNodes
        ///</summary>
        [TestMethod()]
        public void PeerNodesTest()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            var target = Printers.Instance;
            List<PeerNode> expected = null; 
            List<PeerNode> actual;
            target.PeerNodes = expected;
            actual = target.PeerNodes;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PrintObjectStoragePath
        ///</summary>
        [TestMethod()]
        public void PrintObjectStoragePathTest()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            var target = Printers.Instance;
            string expected = "test"; 
            string actual;
            target.PrintObjectStoragePath = expected;
            actual = target.PrintObjectStoragePath;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TimeOut
        ///</summary>
        [TestMethod()]
        public void TimeOutTest()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj); 
            var target = Printers.Instance;
            uint expected = 5; 
            uint actual;
            target.TimeOut = expected;
            actual = target.TimeOut;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IfSaveEFilmWhenFilming
        ///</summary>
        ///
        [TestMethod()]
        public void IfSaveEFilmWhenFilming_True_Setting_Test()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            var target = Printers.Instance;
            bool expected = true;
            target.IfSaveEFilmWhenFilming = expected;
            bool actual = target.IfSaveEFilmWhenFilming;
            //Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IfSaveEFilmWhenFilming_False_Setting_Test()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj); // 
            var target = Printers.Instance;
            bool expected = false; 
            bool actual;
            target.IfSaveEFilmWhenFilming = expected;
            actual = target.IfSaveEFilmWhenFilming;
            //Assert.AreEqual(expected, actual);

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IfClearAfterAddFilmingJob
        ///</summary>
        [TestMethod()]
        public void IfClearAfterAddFilmingJob_False_Setting_Test()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj); // 
            var target = Printers.Instance;
            bool expected = false;
            bool actual;
            target.IfClearAfterAddFilmingJob = expected;
            actual = target.IfClearAfterAddFilmingJob;
            //Assert.AreEqual(expected, actual);

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void IfClearAfterAddFilmingJob_True_Setting_Test()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            var target = Printers.Instance;
            bool expected = true;
            target.IfClearAfterAddFilmingJob = expected;
            bool actual = target.IfClearAfterAddFilmingJob;
            //Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void CreateTextItemsTest()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            var target = Printers.Instance;
       
            //target.CreateTextItems(ImgTxtDisplayState.All);
            //target.CreateTextItems(ImgTxtDisplayState.Customization);
            //target.CreateTextItems(ImgTxtDisplayState.Anonymity);
            //target.CreateTextItems(ImgTxtDisplayState.None);           
       
        }



        /// <summary>
        ///A test for IfStandAlone
        ///</summary>
        [TestMethod()]
        public void IfStandAloneTest()
        {
            //PrivateType type = new PrivateType(typeof(Printers));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(Printers.Instance, type);

            //Printers_Accessor target = new Printers_Accessor(privateObj);
            var target = Printers.Instance;

            Debug.WriteLine("If stand alone is : {0}", target.IfStandAlone);

            bool expected = false; // 
            bool actual;
            target.IfStandAlone = expected;
            actual = target.IfStandAlone;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
