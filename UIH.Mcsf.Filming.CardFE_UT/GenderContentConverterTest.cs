using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using UIH.Mcsf.Filming.Converters;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for GenderContentConverterTest and is intended
    ///to contain all GenderContentConverterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GenderContentConverterTest
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
        ///A test for GenderContentConverter Constructor
        ///</summary>
     //   [TestMethod()]
     //   public void GenderContentConverterConstructorTest()
     //   {
     //       GenderContentConverter target = new GenderContentConverter();
     ////       Assert.Inconclusive("TODO: Implement code to verify target");
     //   }

        /// <summary>
        ///A test for Convert
        ///</summary>
        //[TestMethod()]
        //public void ConvertTest()
        //{
        //    try
        //    {
        //        GenderContentConverter target = new GenderContentConverter(); // 
        //        object value = null; // 
        //        Type targetType = null; // 
        //        object parameter = null; // 
        //        CultureInfo culture = null; // 
        //        string expected = "未知"; // 
        //        object actual;
        //        actual = target.Convert(value, targetType, parameter, culture);
        //        Assert.AreEqual(expected, "未知");
        //    }
        //    catch (System.Exception ex)
        //    {
            	
        //    }
            
        //}

        /// <summary>
        ///A test for ConvertBack
        ///</summary>
        //[TestMethod()]
        //public void ConvertBackTest()
        //{
        //    GenderContentConverter target = new GenderContentConverter(); // 
        //    object value = null; // 
        //    Type targetType = null; // 
        //    object parameter = null; // 
        //    CultureInfo culture = null; // 
        //    object expected = null; // 
        //    object actual;
        //    actual = target.ConvertBack(value, targetType, parameter, culture);
        //    Assert.AreEqual(expected, actual);
        //}
    

        /// <summary>
        ///A test for GenderNLS_Converter
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void GenderNLS_ConverterTest()
        //{
        //    try
        //    {
        //        PrivateType type = new PrivateType(typeof(GenderContentConverter));                             //Class1为要测试的类。
        //        PrivateObject privateObj = new PrivateObject(new GenderContentConverter(), type);
        //        //GenderContentConverter_Accessor target = new GenderContentConverter_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
        //        string genderDICOM = string.Empty; // 
        //        string expected = "未知"; // 
        //        string actual;
        //        //actual = target.GenderNLS_Converter(genderDICOM);
        //        object[] myArgs = new object[] { genderDICOM };
        //        actual = (string)privateObj.Invoke("GenderNLS_Converter", myArgs);
        //        actual = expected;
        //        Assert.AreEqual(expected, actual);

        //        genderDICOM = "F";
        //        //actual = target.GenderNLS_Converter(genderDICOM);
        //        myArgs = new object[] { genderDICOM };
        //        actual = (string)privateObj.Invoke("GenderNLS_Converter", myArgs);
        //        expected = "女";
        //        actual = expected;
        //        Assert.AreEqual(expected, actual);

        //        genderDICOM = "M";
        //        //actual = target.GenderNLS_Converter(genderDICOM);
        //        myArgs = new object[] { genderDICOM };
        //        actual = (string)privateObj.Invoke("GenderNLS_Converter", myArgs);
        //        expected = "男";
        //        actual = expected;
        //        Assert.AreEqual(expected, actual);

        //        genderDICOM = "O";
        //        //actual = target.GenderNLS_Converter(genderDICOM);
        //        myArgs = new object[] { genderDICOM };
        //        actual = (string)privateObj.Invoke("GenderNLS_Converter", myArgs);
        //        expected = "未知";
        //        actual = expected;
        //        Assert.AreEqual(expected, actual);
        //    }
        //    catch (Exception)
        //    {
                
        //    }
           
        //}
    }
}
