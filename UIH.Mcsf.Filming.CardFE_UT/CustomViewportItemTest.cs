using UIH.Mcsf.Filming.CustomizeLayout;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for CustomViewportItemTest and is intended
    ///to contain all CustomViewportItemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CustomViewportItemTest
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
        ///A test for CustomViewportItem Constructor
        ///</summary>
        //[TestMethod()]
        //public void CustomViewportItemConstructorTest()
        //{
        //    string name = string.Empty; // 
        //    string thumbnailFullPath = string.Empty; // 
        //    string layoutStreamFullPath = string.Empty; // 
        //    CustomViewportItem target = new CustomViewportItem(name, thumbnailFullPath, layoutStreamFullPath);
        //    //Assert.Inconclusive("TODO: Implement code to verify target");
        //}

        /// <summary>
        ///A test for Destroy
        ///</summary>
        [TestMethod()]
        public void DestroyTest()
        {
            try
            {
                string name = string.Empty; // 
                string thumbnailFullPath = string.Empty; // 
                string layoutStreamFullPath = string.Empty; // 
                CustomViewportItem target = new CustomViewportItem(name, thumbnailFullPath, layoutStreamFullPath); // 
                //bool expected = false; // 
                bool actual;
                actual = target.Destroy();
                //Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        ///A test for OnPropertyChanged
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnPropertyChangedTest()
        {
            try
            {
                string name = string.Empty; // 
                string thumbnailFullPath = string.Empty; // 
                string layoutStreamFullPath = string.Empty; // 
                PrivateType type = new PrivateType(typeof(CustomViewportItem));
                PrivateObject privateObj = new PrivateObject(new CustomViewportItem(name, thumbnailFullPath, layoutStreamFullPath), type);
                //PrivateObject param0 = null; // 
                //CustomViewportItem_Accessor target = new CustomViewportItem_Accessor(param0); // 
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(string.Empty); // 
                //target.OnPropertyChanged(e);
                object[] myArgs = new object[] { e };
                privateObj.Invoke("OnPropertyChanged", myArgs);
               
                //Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for CustomViewportLayoutXmlPath
        ///</summary>
        [TestMethod()]
        public void CustomViewportLayoutXmlPathTest()
        {
            try
            {
                string name = string.Empty; // 
                string thumbnailFullPath = string.Empty; // 
                string layoutStreamFullPath = string.Empty; // 
                CustomViewportItem target = new CustomViewportItem(name, thumbnailFullPath, layoutStreamFullPath); // 
                string expected = string.Empty; // 
                string actual;
                target.CustomViewportLayoutXmlPath = expected;
                actual = target.CustomViewportLayoutXmlPath;
                //Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///A test for CustomViewportName
        ///</summary>
        [TestMethod()]
        public void CustomViewportNameTest()
        {
            try
            {
                string name = string.Empty; // 
                string thumbnailFullPath = string.Empty; // 
                string layoutStreamFullPath = string.Empty; // 
                CustomViewportItem target = new CustomViewportItem(name, thumbnailFullPath, layoutStreamFullPath); // 
                string expected = string.Empty; // 
                string actual;
                target.CustomViewportName = expected;
                actual = target.CustomViewportName;
                //Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///A test for CustomViewportThumbnailBitmapSource
        ///</summary>
        //[TestMethod()]
        //public void CustomViewportThumbnailBitmapSourceTest()
        //{
        //    try
        //    {
        //        string name = string.Empty; // 
        //        string thumbnailFullPath = string.Empty; // 
        //        string layoutStreamFullPath = string.Empty; // 
        //        CustomViewportItem target = new CustomViewportItem(name, thumbnailFullPath, layoutStreamFullPath); // 
        //        ImageSource actual;
        //        actual = target.CustomViewportThumbnailBitmapSource;

        //        target.CustomViewportThumbnailImagePath = "test";
        //        actual = target.CustomViewportThumbnailBitmapSource;


        //        target.CustomViewportThumbnailImagePath = @"\\10.1.2.12\Public\images\LiuFang_SW\test.bmp";
        //        actual = target.CustomViewportThumbnailBitmapSource;

        //        target.CustomViewportThumbnailImagePath = @"\\10.1.2.12\Public\images\LiuFang_SW\zoom.png";
        //        actual = target.CustomViewportThumbnailBitmapSource;

        //        target.CustomViewportThumbnailImagePath = @"\\10.1.2.12\Public\images\LiuFang_SW\text.txt";
        //        actual = target.CustomViewportThumbnailBitmapSource;

        //        //Assert.Inconclusive("Verify the correctness of this test method.");
        //    }
        //    catch (System.Exception ex)
        //    {
        //    }
        //}

        /// <summary>
        ///A test for CustomViewportThumbnailImagePath
        ///</summary>
        [TestMethod()]
        public void CustomViewportThumbnailImagePathTest()
        {
            try
            {
                string name = string.Empty; // 
                string thumbnailFullPath = string.Empty; // 
                string layoutStreamFullPath = string.Empty; // 
                CustomViewportItem target = new CustomViewportItem(name, thumbnailFullPath, layoutStreamFullPath); // 
                string expected = string.Empty; // 
                string actual;
                target.CustomViewportThumbnailImagePath = expected;
                actual = target.CustomViewportThumbnailImagePath;
                //Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {
            }
        }
    }
}
