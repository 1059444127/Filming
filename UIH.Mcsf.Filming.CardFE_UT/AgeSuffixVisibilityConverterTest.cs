//using UIH.Mcsf.Filming;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Windows;
//using System.Globalization;

//namespace UIH.Mcsf.Filming.CardFE_UT
//{
    
    
//    /// <summary>
//    ///This is a test class for AgeSuffixVisibilityConverterTest and is intended
//    ///to contain all AgeSuffixVisibilityConverterTest Unit Tests
//    ///</summary>
//    [TestClass()]
//    public class AgeSuffixVisibilityConverterTest
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


//        /// <summary>
//        ///A test for AgeSuffixVisibilityConverter Constructor
//        ///</summary>
//        [TestMethod()]
//        public void AgeSuffixVisibilityConverterConstructorTest()
//        {
//            AgeSuffixVisibilityConverter target = new AgeSuffixVisibilityConverter();          
//        }

//        /// <summary>
//        ///A test for Convert
//        ///</summary>
//        [TestMethod()]
//        public void ConvertTest()
//        {
//            AgeSuffixVisibilityConverter target = new AgeSuffixVisibilityConverter(); // 
//            object value = null; // 
//            Type targetType = null; // 
//            object parameter = null; // 
//            CultureInfo culture = null; // 
//            object expected = Visibility.Collapsed; // 
//            object actual;
//            actual = target.Convert(value, targetType, parameter, culture);
//            Assert.AreEqual(expected, actual);

//            value = "24Y";
//            expected = Visibility.Visible;
//            actual = target.Convert(value, targetType, parameter, culture);
//            Assert.AreEqual(expected, actual);

//        }

//        /// <summary>
//        ///A test for ConvertBack
//        ///</summary>
//        [TestMethod()]
//        public void ConvertBackTest()
//        {
//            AgeSuffixVisibilityConverter target = new AgeSuffixVisibilityConverter(); // 
//            object value = null; // 
//            Type targetType = null; // 
//            object parameter = null; // 
//            CultureInfo culture = null; // 
//            object expected = null; // 
//            object actual;
//            actual = target.ConvertBack(value, targetType, parameter, culture);
//            Assert.AreEqual(expected, actual);
           
//        }
//    }
//}
