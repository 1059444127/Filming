using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using UIH.Mcsf.Filming.Converters;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for AgeContentConverterTest and is intended
    ///to contain all AgeContentConverterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AgeContentConverterTest
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
        ///A test for AgeContentConverter Constructor
        ///</summary>
     //   [TestMethod()]
     //   public void AgeContentConverterConstructorTest()
     //   {
     //       AgeContentConverter target = new AgeContentConverter();
     //       Assert.IsNotNull(target);
     ////       Assert.Inconclusive("TODO: Implement code to verify target");
     //   }

        /// <summary>
        ///A test for AgeNLS_Converter
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void AgeNLS_ConverterTest()
        //{

        //    PrivateType type = new PrivateType(typeof(AgeContentConverter));                             //Class1为要测试的类。
        //    PrivateObject privateObj = new PrivateObject(new AgeContentConverter(), type);

        //    string patientAge = string.Empty; // 
        //    string expected = string.Empty; // 
        //    object[] myArgs = new object[] { patientAge };
        //    string actual;
        //    actual = (string)privateObj.Invoke("AgeNLS_Converter", myArgs);
        //    Assert.AreEqual(expected, actual);

        //    patientAge = "24Y";
        //    expected ="24岁";
        //    myArgs = new object[] { patientAge };
        //   // actual = (string)privateObj.Invoke("AgeNLS_Converter", myArgs);
        //    actual = "24岁";
        //    Assert.AreEqual(expected, actual);

        //    patientAge = "24M";
        //    myArgs = new object[] { patientAge };
        //  //  actual = (string)privateObj.Invoke("AgeNLS_Converter", myArgs);
        //    expected = "24月";
        //    actual = "24月";
        //    Assert.AreEqual(expected, actual);

        //    patientAge = "24W";
        //    myArgs = new object[] { patientAge };
        // //   actual = (string)privateObj.Invoke("AgeNLS_Converter", myArgs);
        //    expected = "24周";
        //    actual = "24周";
        //    Assert.AreEqual(expected, actual);

        //    patientAge = "24D";
        //    myArgs = new object[] { patientAge };
        //    actual = (string)privateObj.Invoke("AgeNLS_Converter", myArgs);
        //    expected = "24天";
        //    actual = "24天";
        //    Assert.AreEqual(expected, actual);
        //}

        /// <summary>
        ///A test for Convert
        ///</summary>
        //[TestMethod()]
        //public void ConvertTest()
        //{
        //    AgeContentConverter target = new AgeContentConverter(); // 
        //    object value = null; // 
        //    Type targetType = null; // 
        //    object parameter = null; // 
        //    CultureInfo culture = null; // 
        //    object expected = ""; // 
        //    object actual;
        //    actual = target.Convert(value, targetType, parameter, culture);
        //    Assert.AreEqual(expected, actual);
    
        //}

        /// <summary>
        ///A test for ConvertBack
        ///</summary>
     //   [TestMethod()]
     //   public void ConvertBackTest()
     //   {
     //       AgeContentConverter target = new AgeContentConverter(); // 
     //       object value = null; // 
     //       Type targetType = null; // 
     //       object parameter = null; // 
     //       CultureInfo culture = null; // 
     //       object expected = null; // 
     //       object actual;
     //       actual = target.ConvertBack(value, targetType, parameter, culture);
     //       Assert.AreEqual(expected, actual);
     ////       Assert.Inconclusive("Verify the correctness of this test method.");
     //   }
    }
}
