//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Collections.Generic;

//namespace UIH.Mcsf.Filming.CardFE_UT
//{
    
    
//    /// <summary>
//    ///This is a test class for StudyDataTest and is intended
//    ///to contain all StudyDataTest Unit Tests
//    ///</summary>
//    [TestClass]
//    public class StudyDataTest
//    {


//        private TestContext testContextInstance;

//        /// <summary>
//        ///Gets or sets the test context which provides
//        ///information about and functionality for the current test run.
//        ///</summary>
//        public TestContext TestContext
//        {
//            get
//            {
//                return testContextInstance;
//            }
//            set
//            {
//                testContextInstance = value;
//            }
//        }

//        #region Additional test attributes
//        // 
//        //You can use the following additional attributes as you write your tests:
//        //
//        //Use ClassInitialize to run code before running the first test in the class
//        //[ClassInitialize()]
//        //public static void MyClassInitialize(TestContext testContext)
//        //{
//        //}
//        //
//        //Use ClassCleanup to run code after all tests in a class have run
//        //[ClassCleanup()]
//        //public static void MyClassCleanup()
//        //{
//        //}
//        //
//        //Use TestInitialize to run code before running each test
//        //[TestInitialize()]
//        //public void MyTestInitialize()
//        //{
//        //}
//        //
//        //Use TestCleanup to run code after each test has run
//        //[TestCleanup()]
//        //public void MyTestCleanup()
//        //{
//        //}
//        //
//        #endregion


//        [TestMethod]
//        public void StudyDataConstructorTest()
//        {
//            string studyUid = string.Empty; 
//            string studyDesc = string.Empty; 
//            var target = new StudyData(studyUid, studyDesc);
//            Assert.IsNotNull(target);
//        }

//        /// <summary>
//        ///A test for StudyData Constructor
//        ///</summary>
//        [TestMethod]
//        public void StudyDataConstructorTest1()
//        {
//            string studyUid = string.Empty; 
//            var target = new StudyData(studyUid);
//            Assert.IsNotNull(target);
//        }

//        /// <summary>
//        ///A test for StudyData Constructor
//        ///</summary>
//        [TestMethod]
//        public void StudyDataConstructorTest2()
//        {
//            var target = new StudyData();
//            Assert.IsNotNull(target);
//        }

//        /// <summary>
//        ///A test for ToString
//        ///</summary>
//        [TestMethod]
//        public void ToStringTest()
//        {
//            var target = new StudyData {PatientID = "1", PatientName = "test", AccessionNumber = "2"};
//            string expected = "PatientID=1;PatientName=test;AccessionNumber=2";
//            string actual;
//            actual = target.ToString();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for AccessionNumber
//        ///</summary>
//        [TestMethod]
//        public void AccessionNumberTest()
//        {
//            var target = new StudyData(); 
//            string expected = "test";
//            string actual;
//            target.AccessionNumber = expected;
//            actual = target.AccessionNumber;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for Description
//        ///</summary>
//        [TestMethod]
//        public void DescriptionTest()
//        {
//            var target = new StudyData();
//            string expected = "test"; 
//            string actual;
//            target.Description = expected;
//            actual = target.Description;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for Modality
//        ///</summary>
//        [TestMethod]
//        public void ModalityTest()
//        {
//            var target = new StudyData(); 
//            string expected = "CT"; 
//            string actual;
//            target.Modality = expected;
//            actual = target.Modality;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for PatientBirthday
//        ///</summary>
//        [TestMethod]
//        public void PatientBirthdayTest()
//        {
//            var target = new StudyData(); 
//            string expected = "19661011"; 
//            string actual;
//            target.PatientBirthday = expected;
//            actual = target.PatientBirthday;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for PatientID
//        ///</summary>
//        [TestMethod]
//        public void PatientIDTest()
//        {
//            StudyData target = new StudyData(); 
//            string expected = "123"; 
//            string actual;
//            target.PatientID = expected;
//            actual = target.PatientID;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for PatientName
//        ///</summary>
//        [TestMethod]
//        public void PatientNameTest()
//        {
//            StudyData target = new StudyData(); 
//            string expected = "test"; 
//            string actual;
//            target.PatientName = expected;
//            actual = target.PatientName;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for PatientSex
//        ///</summary>
//        [TestMethod]
//        public void PatientSexTest()
//        {
//            StudyData target = new StudyData(); 
//            string expected = "M"; 
//            string actual;   
//            target.PatientSex = expected;
//            actual = target.PatientSex;
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        ///A test for SeriesItems
//        ///</summary>
//        [TestMethod]
//        public void SeriesItemsTest()
//        {
//            StudyData target = new StudyData(); 
//            List<SeriesData> actual;
//            actual = target.SeriesItems;
//        }

//        /// <summary>
//        ///A test for StudyUid
//        ///</summary>
//        [TestMethod]
//        public void StudyUidTest()
//        {
//            StudyData target = new StudyData(); 
//            string expected = "111";
//            string actual;
//            target.StudyUid = expected;
//            actual = target.StudyUid;
//            Assert.AreEqual(expected, actual);
//        }
//    }
//}
