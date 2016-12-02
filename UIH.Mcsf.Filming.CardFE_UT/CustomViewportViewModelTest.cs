using UIH.Mcsf.Filming.CustomizeLayout;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for CustomViewportViewModelTest and is intended
    ///to contain all CustomViewportViewModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CustomViewportViewModelTest
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
        ///A test for CustomViewportViewModel Constructor
        ///</summary>
        [TestMethod()]
        public void CustomViewportViewModelConstructorTest()
        {
            CustomViewportViewModel target = new CustomViewportViewModel();
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Initialize
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void InitializeTest()
        {
            //PrivateType type = new PrivateType(typeof(CustomViewportViewModel));
            //PrivateObject param0 = new PrivateObject(new CustomViewportViewModel(), type);

            //CustomViewportViewModel_Accessor target = new CustomViewportViewModel_Accessor(param0); // 
            //target.Initialize();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");

            PrivateType type = new PrivateType(typeof(CustomViewportViewModel));
            PrivateObject privateObj = new PrivateObject(new CustomViewportViewModel(), type);
            privateObj.Invoke("Initialize");
        }

        /// <summary>
        ///A test for OnPropertyChanged
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnPropertyChangedTest()
        {
            //PrivateType type = new PrivateType(typeof(CustomViewportViewModel));
            //PrivateObject param0 = new PrivateObject(new CustomViewportViewModel(), type);

            //CustomViewportViewModel_Accessor target = new CustomViewportViewModel_Accessor(param0); // 
            //PropertyChangedEventArgs e = null; // 
            //target.OnPropertyChanged(e);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");

            PrivateType type = new PrivateType(typeof(CustomViewportViewModel));
            PrivateObject privateObj = new PrivateObject(new CustomViewportViewModel(), type);
            PropertyChangedEventArgs e = null; 
            object[] myArgs = new object[] { e };
            privateObj.Invoke("OnPropertyChanged", myArgs);
        }

        /// <summary>
        ///A test for Refresh
        ///</summary>
        [TestMethod()]
        public void RefreshTest()
        {
            CustomViewportViewModel target = new CustomViewportViewModel(); // 
            target.Refresh();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CustomViewportItemCollection
        ///</summary>
        [TestMethod()]
        public void CustomViewportItemCollectionTest()
        {
            CustomViewportViewModel target = new CustomViewportViewModel(); // 
            ObservableCollection<CustomViewportItem> expected = null; // 
            ObservableCollection<CustomViewportItem> actual;
            target.CustomViewportItemCollectionUser = expected;
            actual = target.CustomViewportItemCollectionUser;
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsEnableApply
        ///</summary>
        [TestMethod()]
        public void IsEnableApplyTest()
        {
            CustomViewportViewModel target = new CustomViewportViewModel(); // 
            bool actual;
            actual = target.IsEnableApply;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsEnableDelete
        ///</summary>
        [TestMethod()]
        public void IsEnableDeleteTest()
        {
            CustomViewportViewModel target = new CustomViewportViewModel(); // 
            bool actual;
            actual = target.IsEnableDelete;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsEnableSave
        ///</summary>
        [TestMethod()]
        public void IsEnableSaveTest()
        {
            CustomViewportViewModel target = new CustomViewportViewModel(); // 
            bool actual;
            actual = target.IsEnableSave;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SelectedViewportItem
        ///</summary>
        [TestMethod()]
        public void SelectedViewportItemTest()
        {
            CustomViewportViewModel target = new CustomViewportViewModel(); // 
            CustomViewportItem expected = null; // 
            CustomViewportItem actual;
            target.SelectedViewportItem = expected;
            actual = target.SelectedViewportItem;
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ExistViewportHasSameNameWithActiveViewport
        ///</summary>
        [TestMethod()]
        public void ExistViewportHasSameNameWithActiveViewportTest()
        {
            CustomViewportViewModel target = new CustomViewportViewModel(); // 
            bool expected = false; // 
            bool actual;
            actual = target.ExistViewportHasSameNameWithActiveViewport();
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CustomViewportName
        ///</summary>
        [TestMethod()]
        public void CustomViewportNameTest()
        {
            CustomViewportViewModel target = new CustomViewportViewModel(); // 
            string expected = string.Empty; // 
            string actual;
            target.CustomViewportName = expected;
            actual = target.CustomViewportName;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
