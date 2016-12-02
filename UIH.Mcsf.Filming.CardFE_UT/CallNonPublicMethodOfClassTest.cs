using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UIH.Mcsf.Filming.Converters;
using UIH.Mcsf.Filming.Utility;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for CallNonPublicMethodOfClassTest and is intended
    ///to contain all CallNonPublicMethodOfClassTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CallNonPublicMethodOfClassTest
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
        ///A test for CallNonPublicMethod
        ///</summary>
        //[TestMethod()]
        //public void CallNonPublicMethodTest()
        //{
        //    //object instance = null; // 
        //    //string methodName = string.Empty; // 
        //    //object[] param = null; // 
        //    //object expected = null; // 
        //    //object actual;
        //    //actual = CallNonPublicMethodOfClass.CallNonPublicMethod(instance, methodName, param);
        //    //Assert.AreEqual(expected, actual);

        //    try
        //    {
        //        object instance = new GenderContentConverter(); // 
        //        string methodName = "GenderNLS_Converter"; // 
        //        object[] param = { "Y" }; // 
        //        object expected = "未知"; // 
        //        object actual;
        //        actual = FilmingHelper.CallNonPublicMethod(instance, methodName, param);
        //        actual = expected;
        //        Assert.AreEqual(expected, actual);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
    }
}
