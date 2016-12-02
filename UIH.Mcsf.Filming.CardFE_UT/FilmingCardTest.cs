using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Windows;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingCardTest and is intended
    ///to contain all FilmingCardTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmingCardTest
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
        ///A detailed test for ConvertFilmSizeFrom
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void ConvertFilmSizeFromTest()
        //{
        //    PrivateType type = new PrivateType(typeof(FilmingCard));
        //    PrivateObject privateObj = new PrivateObject(new FilmingCard(), type);
        //    string filmSize = "14INX17IN";
        //    int DPI = 100;
        //    object[] dArgs = new object[] { filmSize, DPI};

            //filmSize = "a wrong film size string";
            //DPI = 100;
            //object[] sArgs = new object[] { filmSize, DPI };
            //Assert.AreEqual(privateObj.Invoke("ConvertFilmSizeFrom", dArgs), privateObj.Invoke("ConvertFilmSizeFrom", sArgs));

            //filmSize = "8INX10INX12IN";
            //DPI = 100;
            //sArgs = new object[] { filmSize, DPI };
            //Assert.AreEqual(privateObj.Invoke("ConvertFilmSizeFrom", dArgs), privateObj.Invoke("ConvertFilmSizeFrom", sArgs));

            //filmSize = "8INX10INX";
            //DPI = 100;
            //sArgs = new object[] { filmSize, DPI };
            //Assert.AreEqual(privateObj.Invoke("ConvertFilmSizeFrom", dArgs), privateObj.Invoke("ConvertFilmSizeFrom", sArgs));

            //filmSize = "X10INX12IN";
            //DPI = 100;
            //sArgs = new object[] { filmSize, DPI };
            //Assert.AreEqual(privateObj.Invoke("ConvertFilmSizeFrom", dArgs), privateObj.Invoke("ConvertFilmSizeFrom", sArgs));

            //filmSize = "8INX10CM";
            //DPI = 100;
            //sArgs = new object[] { filmSize, DPI };
            //Assert.AreEqual(privateObj.Invoke("ConvertFilmSizeFrom", dArgs), privateObj.Invoke("ConvertFilmSizeFrom", sArgs));

            //filmSize = "8CMX10IN";
            //DPI = 100;
            //sArgs = new object[] { filmSize, DPI };
            //Assert.AreEqual(privateObj.Invoke("ConvertFilmSizeFrom", dArgs), privateObj.Invoke("ConvertFilmSizeFrom", sArgs));


            //filmSize = "8INX10IN";
            //DPI = 300;
            //sArgs = new object[] { filmSize, DPI };
            //Assert.AreEqual(new Size(2400, 3000), (Size)privateObj.Invoke("ConvertFilmSizeFrom", sArgs));

        //    filmSize = "10CMX10CM";
        //    DPI = 100;
        //    sArgs = new object[] { filmSize, DPI };
        //    Assert.AreEqual(new Size(393.7, 393.7), (Size)privateObj.Invoke("ConvertFilmSizeFrom", sArgs));
        //}
    }
}
