using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Utility_UT
{
    
    
    /// <summary>
    ///This is a test class for PeerNodeTest and is intended
    ///to contain all PeerNodeTest Unit Tests
    ///</summary>
    [TestClass]
    public class PeerNodeTest
    {



        /// <summary>
        ///A test for PeerNode Constructor
        ///</summary>
        [TestMethod]
        public void PeerNodeConstructorTest()
        {
            var target = new PeerNode();
            Assert.IsNotNull(target); 
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod]
        public void ToStringTest()
        {
            var target = new PeerNode();
            target.PrinterDiscription = "aa";
            target.PeerAE = "bb";
            target.PeerIP = "cc";
            target.PeerPort = 2;           
            string expected = "aa's AE:bb;IP:cc;Port:2"; 
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PeerAE
        ///</summary>
        [TestMethod]
        public void PeerAETest()
        {
            PeerNode target = new PeerNode(); // 
            string expected = "aa"; // 
            string actual;
            target.PeerAE = expected;
            actual = target.PeerAE;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PeerIP
        ///</summary>
        [TestMethod]
        public void PeerIPTest()
        {
            PeerNode target = new PeerNode(); // 
            string expected = "aa"; // 
            string actual;
            target.PeerIP = expected;
            actual = target.PeerIP;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PeerPort
        ///</summary>
        [TestMethod]
        public void PeerPortTest()
        {
            PeerNode target = new PeerNode(); // 
            ushort expected = 2; // 
            ushort actual;
            target.PeerPort = expected;
            actual = target.PeerPort;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PeerPort
        ///</summary>
        [TestMethod]
        public void AllowAutoFilmingTest()
        {
            var target = new PeerNode(); //    
            bool actual;
            target.AllowAutoFilming = true;
            actual = target.AllowAutoFilming;
            Assert.AreEqual(true, actual);              
          
        }


        /// <summary>
        ///A test for PrinterDiscription
        ///</summary>
        [TestMethod]
        public void PrinterDiscriptionTest()
        {
            PeerNode target = new PeerNode(); // 
            string expected = "aa"; // 
            string actual;
            target.PrinterDiscription = expected;
            actual = target.PrinterDiscription;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SupportFilmSizeList
        ///</summary>
        [TestMethod]
        public void SupportFilmSizeListTest()
        {
            PeerNode target = new PeerNode(); // 
            List<object> expected = new List<object>();
            expected.Add("aa");
            expected.Add("bb");// 
            List<object> actual;
            target.SupportFilmSizeList = expected;
            actual = target.SupportFilmSizeList;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SupportLayoutList
        ///</summary>
        [TestMethod]
        public void SupportLayoutListTest()
        {
            PeerNode target = new PeerNode(); // 
            List<string> expected = new List<string>();
            expected.Add("aa");
            expected.Add("bb");// 
            List<string> actual;
            target.SupportLayoutList = expected;
            actual = target.SupportLayoutList;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DefaultFilmSize
        ///</summary>
        [TestMethod]
        public void DefaultFilmSizeTest()
        {
            PeerNodeType type = new PeerNodeType(); 
            PeerNode target = new PeerNode(type); 
            string expected = string.Empty; 
            string actual;
            target.DefaultFilmSize = expected;
            actual = target.DefaultFilmSize;
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for DefaultOrientation
        ///</summary>
        [TestMethod]
        public void DefaultOrientationTest()
        {
            PeerNodeType type = new PeerNodeType(); 
            PeerNode target = new PeerNode(type); 
            int expected = 0; 
            int actual;
            target.DefaultOrientation = expected;
            actual = target.DefaultOrientation;
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for MaxDensity
        ///</summary>
        [TestMethod]
        public void MaxDensityTest()
        {
            PeerNodeType type = new PeerNodeType(); 
            PeerNode target = new PeerNode(type); 
            int expected = 0; 
            int actual;
            target.MaxDensity = expected;
            actual = target.MaxDensity;
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for NodeType
        ///</summary>
        [TestMethod]
        public void NodeTypeTest()
        {
            PeerNodeType type = new PeerNodeType(); 
            PeerNode target = new PeerNode(type); 
            PeerNodeType expected = new PeerNodeType(); 
            PeerNodeType actual;
            target.NodeType = expected;
            actual = target.NodeType;
            Assert.AreEqual(expected, actual);

        }

    }
}
