using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UIH.Mcsf.Filming.Utility_UT
{
    
    
    /// <summary>
    ///This is a test class for PatientTest and is intended
    ///to contain all PatientTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PatientTest
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
        ///A test for Patient Constructor
        ///</summary>
        [TestMethod()]
        public void PatientConstructorTest()
        {
            Patient target = new Patient();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for AccessionNo
        ///</summary>
        [TestMethod()]
        public void AccessionNoTest()
        {
            Patient target = new Patient(); // 
            string expected = "aa"; // 
            string actual;
            target.AccessionNo = expected;
            actual = target.AccessionNo;
            Assert.AreEqual(expected, actual);

            expected = null;
            target.AccessionNo = expected;
            actual = string.Empty;
            Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///A test for OperatorName
        ///</summary>
        [TestMethod()]
        public void OperatorNameTest()
        {
            Patient target = new Patient(); // 
            string expected = "aa"; // 
            string actual;
            target.OperatorName = expected;
            actual = target.OperatorName;
            Assert.AreEqual(expected, actual);

            expected = null;
            target.OperatorName = expected;
            actual = string.Empty;
            Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///A test for PatientAge
        ///</summary>
        [TestMethod()]
        public void PatientAgeTest()
        {
            Patient target = new Patient(); // 
            string expected = "aa"; // 
            string actual;
            target.PatientAge = expected;
            actual = target.PatientAge;
            Assert.AreEqual(expected, actual);

            expected = null;
            target.PatientAge = expected;
            actual = string.Empty;
            Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///A test for PatientID
        ///</summary>
        [TestMethod()]
        public void PatientIDTest()
        {
            Patient target = new Patient(); // 
            string expected = "aa"; // 
            string actual;
            target.PatientID = expected;
            actual = target.PatientID;
            Assert.AreEqual(expected, actual);

            expected = null;
            target.PatientID = expected;
            actual = string.Empty;
            Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        ///A test for PatientName
        ///</summary>
        [TestMethod()]
        public void PatientNameTest()
        {
            Patient target = new Patient(); // 
            string expected = "aa"; // 
            string actual;
            target.PatientName = expected;
            actual = target.PatientName;
            Assert.AreEqual(expected, actual);

            expected = null;
            target.PatientName = expected;
            actual = string.Empty;
            Assert.AreNotEqual(expected, actual);
           

        }

        /// <summary>
        ///A test for PatientSex
        ///</summary>
        [TestMethod()]
        public void PatientSexTest()
        {
            Patient target = new Patient(); // 
            string expected = "aa"; // 
            string actual;
            target.PatientSex = expected;
            actual = target.PatientSex;
            Assert.AreEqual(expected, actual);

            expected = null;
            target.PatientSex = expected;
            actual = string.Empty;
            Assert.AreNotEqual(expected, actual);
            
        }
        //  <summary>
        //  A test for StudyID
        //  </summary>
        [TestMethod()]
        public void StudyIDTest()
        {
            Patient target = new Patient(); // 
            string expected = "123"; // 
            string actual;
            target.StudyID = expected;
            actual = target.StudyID;
            Assert.AreEqual(expected, actual);

            expected = null;
            target.StudyID = expected;
            actual = string.Empty;
            Assert.AreNotEqual(expected, actual);

        }
      
    }
}
