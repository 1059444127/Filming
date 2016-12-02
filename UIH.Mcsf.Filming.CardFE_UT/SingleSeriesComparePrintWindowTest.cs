using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Controls;

using UIH.Mcsf.AppControls.Viewer;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for SingleSeriesComparePrintWindowTest and is intended
    ///to contain all SingleSeriesComparePrintWindowTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SingleSeriesComparePrintWindowTest
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
        ///A test for levelTextBox2_TextChanged
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void levelTextBox2_TextChangedTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(SingleSeriesComparePrintWindow));
                PrivateObject privateObj = new PrivateObject(new SingleSeriesComparePrintWindow(null), type);
                
                object sender = null; // 
                TextChangedEventArgs e = null; // 
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("levelTextBox2_TextChanged", myArgs);
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        ///A test for levelTextBox1_TextChanged
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void levelTextBox1_TextChangedTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(SingleSeriesComparePrintWindow));
                PrivateObject privateObj = new PrivateObject(new SingleSeriesComparePrintWindow(null), type);
                
                object sender = null; // 
                TextChangedEventArgs e = null; // 

                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("levelTextBox1_TextChanged", myArgs);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for widthTextBox1_TextChanged
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void widthTextBox1_TextChangedTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(SingleSeriesComparePrintWindow));
                PrivateObject privateObj = new PrivateObject(new SingleSeriesComparePrintWindow(null), type);

                
                object sender = null; // 
                TextChangedEventArgs e = null; // 
                
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("widthTextBox1_TextChanged", myArgs);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for widthTextBox2_TextChanged
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void widthTextBox2_TextChangedTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(SingleSeriesComparePrintWindow));
                PrivateObject privateObj = new PrivateObject(new SingleSeriesComparePrintWindow(null), type);

                
                object sender = null; // 
                TextChangedEventArgs e = null; // 
                
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("widthTextBox2_TextChanged", myArgs);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for IsEnableApply
        ///</summary>
        [TestMethod()]
        public void IsEnableApplyTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(SingleSeriesComparePrintWindow));
                PrivateObject privateObj = new PrivateObject(new SingleSeriesComparePrintWindow(null), type);
                
                bool actual;
                actual = (bool)privateObj.Invoke("IsEnableApply");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for IsValidFloat
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void IsValidFloatTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(SingleSeriesComparePrintWindow));
                PrivateObject privateObj = new PrivateObject(new SingleSeriesComparePrintWindow(null), type);
                
                string text = string.Empty; // 
                bool expected = false; // 
                bool actual;
                object[] sArgs = new object[] { text };
                actual = (bool)privateObj.Invoke("IsValidFloat", sArgs);
                Assert.AreEqual(expected, actual);

                sArgs = new object[] { "." };
                actual = (bool)privateObj.Invoke("IsValidFloat", sArgs);
                Assert.AreEqual(expected, actual);
                
                sArgs = new object[] { "3" };
                actual = (bool)privateObj.Invoke("IsValidFloat", sArgs);
                Assert.AreEqual(true, actual);

                sArgs = new object[] { "-3" };
                actual = (bool)privateObj.Invoke("IsValidFloat", sArgs);
                Assert.AreEqual(true, actual);

                sArgs = new object[] { ".3" };
                actual = (bool)privateObj.Invoke("IsValidFloat", sArgs);
                Assert.AreEqual(true, actual);

                sArgs = new object[] { "-.3" };
                actual = (bool)privateObj.Invoke("IsValidFloat", sArgs);
                Assert.AreEqual(true, actual);

                sArgs = new object[] { "3." };
                actual = (bool)privateObj.Invoke("IsValidFloat", sArgs);
                Assert.AreEqual(true, actual);

                sArgs = new object[] { "-3." };
                actual = (bool)privateObj.Invoke("IsValidFloat", sArgs);
                Assert.AreEqual(true, actual);
            }
            catch (Exception)
            {

            }
        }
    }
}
