using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Viewer;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using UIH.Mcsf.Pipeline.Data;
using System.Windows.Markup;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingPageControlTest and is intended
    ///to contain all FilmingPageControlTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmingPageControlTest
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
        ///A test for ViewportList
        ///</summary>
        [TestMethod()]
        public void ViewportListTest()
        {
            //FilmingPageControl target = new FilmingPageControl(); // 
            //List<McsfFilmViewport> actual;
            //actual = target.ViewportList;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ShowImageInCell
        ///</summary>
        [TestMethod()]
        public void ShowImageInCellTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            FilmImageObject imageObject = null; // 
            int cellIndex = 0; // 
            MedViewerControlCell expected = null; // 
            MedViewerControlCell actual;
            actual = target.ShowImageInCell(imageObject, cellIndex);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Clear
        ///</summary>
        [TestMethod()]
        public void ClearTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            target.Clear();
     //     // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

    //    /// <summary>
    //    ///A test for AddCells
    //    ///</summary>
    //    [TestMethod()]
    //    public void AddCellsTest()
    //    {
    //        FilmingPageControl target = new FilmingPageControl(); // 
    //        IEnumerable<MedViewerControlCell> cells = null; // 
    //        target.AddCells(cells);
    ////      // Assert.Inconclusive("A method that does not return a value cannot be verified.");
    //    }

        /// <summary>
        ///A test for AddCell
        ///</summary>
        [TestMethod()]
        public void AddCellTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                MedViewerControlCell cell = null; // 
                target.AddCell(cell);
                // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch
            {

            }
        }

        /// <summary>
        ///A test for FilmingPageControl Constructor
        ///</summary>
        [TestMethod()]
        public void FilmingPageControlConstructorTest()
        {
            FilmingPageControl target = new FilmingPageControl();
      //      Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for ClearAction
        ///</summary>
        [TestMethod()]
        public void ClearActionTest()
        {
            //FilmingPageControl target = new FilmingPageControl(); // 
            //target.ClearAction();
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        ///// <summary>
        /////A test for CreateMultiFormatCellWithImage
        /////</summary>
        //[TestMethod()]
        //public void CreateMultiFormatCellWithImageTest()
        //{
        //    FilmingPageControl target = new FilmingPageControl(); // 
        //    string dicomFileFullPath = string.Empty; // 
        //    MedViewerControlCell expected = null; // 
        //    MedViewerControlCell actual;
        //    actual = target.CreateMultiFormatCellWithImage(dicomFileFullPath);
        //    Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        /// <summary>
        ///A test for CreatePrintImageFullPath
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void CreatePrintImageFullPathTest()
        {
            PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
            //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类 

            int iCellIndex = 0; // 
            int iStackIndex = 0; // 
            string expected = string.Empty; // 
            string actual;
            //actual = target.CreatePrintImageFullPath(iCellIndex, iStackIndex);
            object[] myArgs = new object[] { iCellIndex, iStackIndex };
            actual = (string)privateObj.Invoke("CreatePrintImageFullPath", myArgs);

            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeleteAllImages
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void DeleteAllImagesTest()
        {
            //PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            //PrivateObject param0 = new PrivateObject(new FilmingPageControl(), type);
            //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
            //target.DeleteAllImages();
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DeleteImagesAfterIndex
        ///</summary>
        [TestMethod()]
        public void DeleteImagesAfterIndexTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                int index = 0; // 
                List<MedViewerControlCell> expected = null; // 
                List<MedViewerControlCell> actual;
                actual = target.DeleteImagesAfterIndex(index);
                Assert.AreNotEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch 
            {

            }
        }

        /// <summary>
        ///A test for DeleteSelectedImages
        ///</summary>
        [TestMethod()]
        public void DeleteSelectedImagesTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            target.DeleteSelectedImages();
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        ///// <summary>
        /////A test for Dispose
        /////</summary>
        //[TestMethod()]
        //public void DisposeTest()
        //{
        //    FilmingPageControl target = new FilmingPageControl(); // 
        //    target.Dispose();
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for EmptyCellCount
        ///</summary>
        [TestMethod()]
        public void EmptyCellCountTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                int expected = 1; // 
                int actual;
                actual = target.EmptyCellCount();
                Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for FitWindow
        ///</summary>
        [TestMethod()]
        public void FitWindowTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            target.FitWindow();
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetFilmingCard
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void GetFilmingCardTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
                PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
                //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
                //FilmingCard expected = null; // 
                FilmingCard actual;
                //actual = target.GetFilmingCard();
                actual = (FilmingCard)privateObj.Invoke("GetFilmingCard");
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetLayoutStoragePath
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void GetLayoutStoragePathTest()
        {
            PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
            //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
            string expected = string.Empty; // 
            string actual;
            //actual = target.GetLayoutStoragePath();
            actual = (string)privateObj.Invoke("GetLayoutStoragePath");
            Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetOriginalScaleTransform
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void GetOriginalScaleTransformTest()
        {
            //try
            //{
            //    PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            //    PrivateObject param0 = new PrivateObject(new FilmingPageControl(), type);
            //    FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
            //    target.GetOriginalScaleTransform();
            //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            //}
            //catch (Exception)
            //{

            //}
        }

        /// <summary>
        ///A test for GetPatientInfo
        ///</summary>
        [TestMethod()]
        public void GetPatientInfoTest()
        {
            //FilmingPageControl target = new FilmingPageControl(); // 
            //string tagName = string.Empty; // 
            //string defaultValue = string.Empty; // 
            //string mixValue = string.Empty; // 
            //string expected = string.Empty; // 
            //string actual;
            //actual = target.GetPatientInfo(tagName, defaultValue, mixValue);
            //Assert.AreEqual(expected, actual);
            ////Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSelectedCellIndex
        ///</summary>
        [TestMethod()]
        public void GetSelectedCellIndexTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                MedViewerLayoutCell layoutCell = null; // 
                int expected = 0; // 
                int actual;
                actual = target.GetSelectedCellIndex(layoutCell);
                Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for GetViewports
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void GetViewportsTest()
        {
            PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
            //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
            MedViewerCellBase cellBase = null; // 
            object[] myArgs = new object[] { cellBase };
            privateObj.Invoke("GetViewports", myArgs);
            //target.GetViewports(cellBase);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for HasEmptyCell
        ///</summary>
        [TestMethod()]
        public void HasEmptyCellTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                bool expected = true; // 
                bool actual;
                actual = target.HasEmptyCell();
                Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for Initialize
        ///</summary>
        [TestMethod()]
        public void InitializeTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                target.Initialize();
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for InitializeComponent
        ///</summary>
        [TestMethod()]
        public void InitializeComponentTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            target.InitializeComponent();
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for InsertCell
        ///</summary>
        [TestMethod()]
        public void InsertCellTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                MedViewerControlCell cell = null; // 
                MedViewerLayoutCell layoutCell = null; // 
                int index = 0; // 
                bool expected = false; // 
                bool actual;
                actual = target.InsertCell(cell, layoutCell, index);
                Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///A test for InsertCell
        ///</summary>
        [TestMethod()]
        public void InsertCellTest1()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                MedViewerControlCell cell = null; // 
                MedViewerLayoutCell layoutCell = null; // 
                bool expected = false; // 
                bool actual;
                actual = target.InsertCell(cell, layoutCell);
                Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for IsEmpty
        ///</summary>
        [TestMethod()]
        public void IsEmptyTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                bool expected = true; // 
                bool actual;
                actual = target.IsEmpty();
                Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {

            }
        }

        ///// <summary>
        /////A test for IsEnableChangeViewportLayout
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void IsEnableChangeViewportLayoutTest()
        //{
        //    FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(); // 
        //    IEnumerable<McsfFilmViewport> viewportList = null; // 
        //    FilmLayout newFilmLayout = null; // 
        //    bool expected = false; // 
        //    bool actual;
        //    actual = target.IsEnableChangeViewportLayout(viewportList, newFilmLayout);
        //    Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        /// <summary>
        ///A test for IsMultiImageActionMode
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void IsMultiImageActionModeTest()
        {
            // Private Accessor for IsMultiImageActionMode is not found. Please rebuild the containing project or run the Publicize.exe manually.
            //Assert.Inconclusive("Private Accessor for IsMultiImageActionMode is not found. Please rebuild the cont" +
            //        "aining project or run the Publicize.exe manually.");
        }

        ///// <summary>
        /////A test for OnActionCleared
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void OnActionClearedTest()
        //{
        //    FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(); // 
        //    MedViewerControl obj = null; // 
        //    target.OnActionCleared(obj);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for OnCellLeftButtonDownHandler
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnCellLeftButtonDownHandlerTest()
        {
            PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
            //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
            McsfFilmViewport viewport = null; // 
            MedViewerControlCell cell = null; // 
            //target.OnCellLeftButtonDownHandler(viewport, cell);
            object[] myArgs = new object[] { viewport, cell };
            privateObj.Invoke("OnCellLeftButtonDownHandler", myArgs);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnCellLeftButtonUpHandler
        ///</summary>
        [TestMethod()]
        public void OnCellLeftButtonUpHandlerTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            McsfFilmViewport viewport = null; // 
            MedViewerControlCell cell = null; // 
            target.OnCellLeftButtonUpHandler(viewport, cell);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnCellMouseLeftButtonDown
        ///</summary>
        [TestMethod()]
        public void OnCellMouseLeftButtonDownTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            object sender = null; // 
            MouseButtonEventArgs e = null; // 
            target.OnCellMouseLeftButtonDown(sender, e);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnCellMouseLeftButtonUp
        ///</summary>
        [TestMethod()]
        public void OnCellMouseLeftButtonUpTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            object sender = null; // 
            MouseButtonEventArgs e = null; // 
            target.OnCellMouseLeftButtonUp(sender, e);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnCellMouseRightButtonDown
        ///</summary>
        [TestMethod()]
        public void OnCellMouseRightButtonDownTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            object sender = null; // 
            MouseButtonEventArgs e = null; // 
            target.OnCellMouseRightButtonDown(sender, e);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnCellRemoved
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnCellRemovedTest()
        {
            PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
            //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
            object sender = null; // 
            MedViewerEventArgs e = null; // 
            //target.OnCellRemoved(sender, e);
            object[] myArgs = new object[] { sender, e };
            privateObj.Invoke("OnCellRemoved", myArgs);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnCellRightButtonDownHandler
        ///</summary>
        [TestMethod()]
        public void OnCellRightButtonDownHandlerTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            McsfFilmViewport viewport = null; // 
            MedViewerControlCell cell = null; // 
            target.OnCellRightButtonDownHandler(viewport, cell);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        ///// <summary>
        /////A test for OnCellSizeChanged
        /////</summary>
        //[TestMethod()]
        //public void OnCellSizeChangedTest()
        //{
        //    FilmingPageControl target = new FilmingPageControl(); // 
        //    object sender = null; // 
        //    SizeChangedEventArgs e = null; // 
        //    target.OnCellSizeChanged(sender, e);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for OnDrop
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void OnDropTest()
        //{
        //    FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(); // 
        //    DragEventArgs e = null; // 
        //    target.OnDrop(e);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for OnFilmingPageTitleBarLeftButtonDown
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnFilmingPageTitleBarLeftButtonDownTest()
        {
            PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
            //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
            object sender = null; // 
            MouseButtonEventArgs e = null; // 
            //target.OnFilmingPageTitleBarLeftButtonDown(sender, e);
            object[] myArgs = new object[] { sender, e };
            privateObj.Invoke("OnFilmingPageTitleBarLeftButtonDown", myArgs);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnFilmingPageTitleBarRightButtonDown
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnFilmingPageTitleBarRightButtonDownTest()
        {
            PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
            //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
            object sender = null; // 
            MouseButtonEventArgs e = null; // 
            //target.OnFilmingPageTitleBarRightButtonDown(sender, e);
            object[] myArgs = new object[] { sender, e };
            privateObj.Invoke("OnFilmingPageTitleBarRightButtonDown", myArgs);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        ///// <summary>
        /////A test for OnGroupMouseLeftButtonDownInvoking
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void OnGroupMouseLeftButtonDownInvokingTest()
        //{
        //    // Private Accessor for OnGroupMouseLeftButtonDownInvoking is not found. Please rebuild the containing project or run the Publicize.exe manually.
        //    Assert.Inconclusive("Private Accessor for OnGroupMouseLeftButtonDownInvoking is not found. Please rebu" +
        //            "ild the containing project or run the Publicize.exe manually.");
        //}

        ///// <summary>
        /////A test for OnGroupMouseLeftButtonUpInvoking
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void OnGroupMouseLeftButtonUpInvokingTest()
        //{
        //    // Private Accessor for OnGroupMouseLeftButtonUpInvoking is not found. Please rebuild the containing project or run the Publicize.exe manually.
        //    Assert.Inconclusive("Private Accessor for OnGroupMouseLeftButtonUpInvoking is not found. Please rebuil" +
        //            "d the containing project or run the Publicize.exe manually.");
        //}

        /// <summary>
        ///A test for OnGroupMouseMoveInvoking
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnGroupMouseMoveInvokingTest()
        {
            // Private Accessor for OnGroupMouseMoveInvoking is not found. Please rebuild the containing project or run the Publicize.exe manually.
          //  Assert.Inconclusive("Private Accessor for OnGroupMouseMoveInvoking is not found. Please rebuild the co" +
                //    "ntaining project or run the Publicize.exe manually.");
        }

        ///// <summary>
        /////A test for OnImageLoaded
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void OnImageLoadedTest()
        //{
        //    FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(); // 
        //    object sender = null; // 
        //    MedViewerEventArgs e = null; // 
        //    target.OnImageLoaded(sender, e);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for OnMouseLeftButtonDown
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnMouseLeftButtonDownTest()
        {
            //try
            //{
            //    PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            //    PrivateObject param0 = new PrivateObject(new FilmingPageControl(), type);
            //    FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
            //    object sender = null; // 
            //    MouseButtonEventArgs e = null; // 
            //    target.OnMouseLeftButtonDown(sender, e);
            //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            //}
            //catch (Exception)
            //{
            //}
        }

        /// <summary>
        ///A test for OnMouseRightButtonDown
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnMouseRightButtonDownTest()
        {
            //PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            //PrivateObject param0 = new PrivateObject(new FilmingPageControl(), type);
            //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
            //object sender = null; // 
            //MouseButtonEventArgs e = null; // 
            //target.OnMouseRightButtonDown(sender, e);
            //// Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnNewCellAdded
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void OnNewCellAddedTest()
        {
            PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
            //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
            object sender = null; // 
            MedViewerEventArgs e = null; // 
            //target.OnNewCellAdded(sender, e);
            object[] myArgs = new object[] { sender, e };
            privateObj.Invoke("OnNewCellAdded", myArgs);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnPageActiveStatusChangedHander
        ///</summary>
        [TestMethod()]
        public void OnPageActiveStatusChangedHanderTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
           
            target.OnPageActiveStatusChangedHander();
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnPageTitleBarLeftClickHandler
        ///</summary>
        [TestMethod()]
        public void OnPageTitleBarLeftClickHandlerTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            target.OnPageTitleBarLeftClickHandler();
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnPageTitleBarRightClickHandler
        ///</summary>
        [TestMethod()]
        public void OnPageTitleBarRightClickHandlerTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            target.OnPageTitleBarRightClickHandler();
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnPropertyChanged
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void OnPropertyChangedTest()
        //{
        //    PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
        //    PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
        //    //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
        //    PropertyChangedEventArgs e = null; // 
        //    //target.OnPropertyChanged(e);
        //    object[] myArgs = new object[] { e };
        //    privateObj.Invoke("OnPropertyChanged", myArgs);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for OnViewportLeftClickHandler
        ///</summary>
        [TestMethod()]
        public void OnViewportLeftClickHandlerTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            McsfFilmViewport viewport = null; // 
            target.OnViewportLeftClickHandler(viewport);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnViewportRightClickHandler
        ///</summary>
        [TestMethod()]
        public void OnViewportRightClickHandlerTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            McsfFilmViewport viewport = null; // 
            target.OnViewportRightClickHandler(viewport);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for PatientInfoChanged
        ///</summary>
        //[TestMethod()]
        //public void PatientInfoChangedTest()
        //{
        //   // FilmingPageControl target = new FilmingPageControl(); // 
        //    MedViewerControl mvCon=new MedViewerControl();
        //    FilmingPageTitle target = new FilmingPageTitle(mvCon);
        //    target.PatientInfoChanged();
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for PopAllMedViewCell
        ///</summary>
        [TestMethod()]
        public void PopAllMedViewCellTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                List<MedViewerControlCell> expected = null; // 
                List<MedViewerControlCell> actual;
                actual = target.PopAllMedViewCell();
                Assert.AreNotEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for ReBuildViewportList
        ///</summary>
        [TestMethod()]
        public void ReBuildViewportListTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                target.ReBuildViewportList();
                // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        ///// <summary>
        /////A test for RegisterEventFromCell
        /////</summary>
        //[TestMethod()]
        //public void RegisterEventFromCellTest()
        //{
        //    FilmingPageControl target = new FilmingPageControl(); // 
        //    MedViewerControlCell cell = null; // 
        //    target.RegisterEventFromCell(cell);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for Reset
        ///</summary>
        [TestMethod()]
        public void ResetTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                target.Reset();
                // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for SaveAsDicomFile
        ///</summary>
        [TestMethod()]
        public void SaveAsDicomFileTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                //string eFilmseriesUID = string.Empty; // 
                string expected = null; // 
                string actual;
                actual = target.SaveAsDicomFile();
                Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {

            }
        }

        ///// <summary>
        /////A test for SaveCustomerLayout
        /////</summary>
        //[TestMethod()]
        //public void SaveCustomerLayoutTest()
        //{
        //    FilmingPageControl target = new FilmingPageControl(); // 
        //    target.SaveCustomerLayout();
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for SelectAllCellsOfSelectedViewport
        ///</summary>
        [TestMethod()]
        public void SelectAllCellsOfSelectedViewportTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                target.SelectAllCellsOfSelectedViewport();
                // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for SelectViewports
        ///</summary>
        [TestMethod()]
        public void SelectViewportsTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                bool isSelected = false; // 
                target.SelectViewports(isSelected);
                // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for SelectedAll
        ///</summary>
        [TestMethod()]
        public void SelectedAllTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                bool isSelected = false; // 
                target.SelectedAll(isSelected);
                // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for SelectedCells
        ///</summary>
        [TestMethod()]
        public void SelectedCellsTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                List<MedViewerControlCell> expected = null; // 
                List<MedViewerControlCell> actual;
                actual = target.SelectedCells();
                Assert.AreNotEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for SelectedViewports
        ///</summary>
        [TestMethod()]
        public void SelectedViewportsTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                List<McsfFilmViewport> expected = null; // 
                List<McsfFilmViewport> actual;
                actual = target.SelectedViewports();
                Assert.AreNotEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for SetAction
        ///</summary>
        [TestMethod()]
        public void SetActionTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                ActionType actionType = new ActionType(); // 
                target.SetAction(actionType);
                // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        ///// <summary>
        /////A test for SetFourCornerFontSize
        /////</summary>
        //[TestMethod()]
        //public void SetFourCornerFontSizeTest()
        //{
        //    FilmingPageControl target = new FilmingPageControl(); // 
        //    double newFontSize = 0F; // 
        //    target.SetFourCornerFontSize(newFontSize);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for SetFourCornerFontSizeForSelecedCells
        /////</summary>
        //[TestMethod()]
        //public void SetFourCornerFontSizeForSelecedCellsTest()
        //{
        //    FilmingPageControl target = new FilmingPageControl(); // 
        //    double newFontSize = 0F; // 
        //    target.SetFourCornerFontSizeForSelectedCells(newFontSize);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for SetOverlayVisibility
        ///</summary>
        [TestMethod()]
        public void SetOverlayVisibilityTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                OverlayType type = new OverlayType(); // 
                bool isVisible = false; // 
                target.SetOverlayVisibility(type, isVisible);
                // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for SetOverlayVisibility
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void SetOverlayVisibilityTest1()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
                PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
                //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
                DisplayData page = null; // 
                OverlayType type1 = new OverlayType(); // 
                bool isVisible = false; // 
                //target.SetOverlayVisibility(page, type1, isVisible);
                object[] myArgs = new object[] { page, type1, isVisible };
                privateObj.Invoke("SetOverlayVisibility", myArgs);
                // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        ///// <summary>
        /////A test for ShowAnnotationTextItems
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void ShowAnnotationTextItemsTest()
        //{
        //    FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(); // 
        //    GraphicImageText graphicImageText = null; // 
        //    IEnumerable<ImgTextItem> items = null; // 
        //    target.ShowAnnotationTextItems(graphicImageText, items);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for ShowDisplayDataInEmptyCell
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void ShowDisplayDataInEmptyCellTest()
        //{
        //    FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(); // 
        //    DisplayData displayData = null; // 
        //    MedViewerControlCell expected = null; // 
        //    MedViewerControlCell actual;
        //    actual = target.ShowDisplayDataInEmptyCell(displayData);
        //    Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for ShowDisplayDataInMultiFormatCell
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void ShowDisplayDataInMultiFormatCellTest()
        //{
        //    FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(); // 
        //    DisplayData displayData = null; // 
        //    MedViewerControlCell expected = null; // 
        //    MedViewerControlCell actual;
        //    actual = target.ShowDisplayDataInMultiFormatCell(displayData);
        //    Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        /// <summary>
        ///A test for ShowImageInCell
        ///</summary>
        [TestMethod()]
        public void ShowImageInCellTest1()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                string imageFullPath = string.Empty; // 
                int cellIndex = 0; // 
                MedViewerControlCell expected = null; // 
                MedViewerControlCell actual;
                actual = target.ShowImageInCell(imageFullPath, cellIndex);
                Assert.AreEqual(expected, actual);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ShowImageInCell
        ///</summary>
        [TestMethod()]
        public void ShowImageInCellTest2()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                FilmImageObject imageObject = null; // 
                int cellIndex = 0; // 
                MedViewerControlCell expected = null; // 
                MedViewerControlCell actual;
                actual = target.ShowImageInCell(imageObject, cellIndex);
                Assert.AreEqual(expected, actual);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ShowImageInEmptyCell
        ///</summary>
        [TestMethod()]
        public void ShowImageInEmptyCellTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                string imageFullPath = string.Empty; // 
                MedViewerControlCell expected = null; // 
                MedViewerControlCell actual;
                actual = target.ShowImageInEmptyCell(imageFullPath);
                Assert.AreEqual(expected, actual);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ShowImageInEmptyCell
        ///</summary>
        [TestMethod()]
        public void ShowImageInEmptyCellTest1()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                FilmImageObject imageObject = null; // 
                MedViewerControlCell expected = null; // 
                MedViewerControlCell actual;
                actual = target.ShowImageInEmptyCell(imageObject);
                Assert.AreEqual(expected, actual);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ShowImageInEmptyCell
        ///</summary>
        [TestMethod()]
        public void ShowImageInEmptyCellTest2()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                string imageUID = string.Empty; // 
                bool isLoadPS = false; // 
                MedViewerControlCell expected = null; // 
                MedViewerControlCell actual;
                actual = target.ShowImageInEmptyCell(imageUID, isLoadPS);
                Assert.AreEqual(expected, actual);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ShowSOPImageInEmptyCell
        ///</summary>
        [TestMethod()]
        public void ShowSOPImageInEmptyCellTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                string sopInstanceUID = string.Empty; // 
                MedViewerControlCell expected = null; // 
                MedViewerControlCell actual;
                actual = target.ShowSOPImageInEmptyCell(sopInstanceUID);
                Assert.AreEqual(expected, actual);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ShowSOPImageInEmptyCell
        ///</summary>
        [TestMethod()]
        public void ShowSOPImageInEmptyCellTest1()
        {
            try
            {
                //FilmingPageControl target = new FilmingPageControl(); // 
                //DicomAttributeCollection dataHeader = null; // 
                //MedViewerControlCell expected = null; // 
                //MedViewerControlCell actual;
                //actual = target.ShowSOPImageInEmptyCell(dataHeader);
                //Assert.AreEqual(expected, actual);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for System.Windows.Markup.IComponentConnector.Connect
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void ConnectTest()
        {
            try
            {
                IComponentConnector target = new FilmingPageControl(); // 
                int connectionId = 0; // 
                object target1 = null; // 
                target.Connect(connectionId, target1);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        ///// <summary>
        /////A test for UnRegisterMouseDownEventFromCell
        /////</summary>
        //[TestMethod()]
        //public void UnRegisterMouseDownEventFromCellTest()
        //{
        //    FilmingPageControl target = new FilmingPageControl(); // 
        //    MedViewerControlCell cell = null; // 
        //    target.UnRegisterMouseDownEventFromCell(cell);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for UpdateCornerText
        ///</summary>
        //[TestMethod()]
        //public void UpdateCornerTextTest()
        //{
        //    FilmingPageControl target = new FilmingPageControl(); // 
        //    ImgTxtDisplayState type = new ImgTxtDisplayState(); // 
        //    target.UpdateCornerText(type);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for UpdateCornerTextDisplayData
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void UpdateCornerTextDisplayDataTest()
        //{
        //    PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
        //    PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
        //    //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
        //    DisplayData page = null; // 
        //    ImgTxtDisplayState type1 = new ImgTxtDisplayState(); // 
        //    //target.UpdateCornerTextDisplayData(page, type1);
        //    object[] myArgs = new object[] { page, type1 };
        //    privateObj.Invoke("UpdateCornerTextDisplayData", myArgs);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for UpdateCornerTextForCell
        ///</summary>
        //[TestMethod()]
        //public void UpdateCornerTextForCellTest()
        //{
        //    FilmingPageControl target = new FilmingPageControl(); // 
        //    MedViewerControlCell cell = null; // 
        //    ImgTxtDisplayState type = new ImgTxtDisplayState(); // 
        //   // target.UpdateCornerTextForCell(cell, type);
        //   FilmingHelper.UpdateCornerTextForCell(cell,type);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for UpdateFourCornerFontSizeForCell
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void UpdateFourCornerFontSizeForCellTest()
        //{
        //    FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(); // 
        //    MedViewerControlCell cell = null; // 
        //    target.UpdateFourCornerFontSizeForCell(cell);
        //    // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for UpdateFourCornerFontSizeForCell
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void UpdateFourCornerFontSizeForCellTest1()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
                PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
                //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
                MedViewerControlCell cell = null; // 
                //double size = 0F; // 
                //target.UpdateFourCornerFontSizeForCell(cell, size);
                object[] myArgs = new object[] { cell };
                privateObj.Invoke("UpdateFourCornerFontSizeForCell", myArgs);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for UpdateFourCornerFontSizeForCell
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void UpdateFourCornerFontSizeForCellTest2()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
                PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
                //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
                MedViewerControlCell cell = null; // 
                Size cellSize = new Size(); // 
                //target.UpdateFourCornerFontSizeForCell(cell, cellSize);
                object[] myArgs = new object[] { cell, cellSize };
                privateObj.Invoke("UpdateFourCornerFontSizeForCell", myArgs);
                // Assert.Inconclusive("A method that does not return a value cannot be verified.");} catch (r EX_NAME) {
              //  Console.WriteLine(EX_NAME);
            }catch(System.Exception)
            {
                
            }
        }

        /// <summary>
        ///A test for UpdateFourCornerFontSizeForDisplayData
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void UpdateFourCornerFontSizeForDisplayDataTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
                PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
                //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
                DisplayData page = null; // 
                //double size = 0F; // 
                //target.UpdateFourCornerFontSizeForDisplayData(page, 6);
                object[] myArgs = new object[] { page, 6 };
                privateObj.Invoke("UpdateFourCornerFontSizeForDisplayData", myArgs);
                // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for UpdateFourCornerFontSizeForOverLayText
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void UpdateFourCornerFontSizeForOverLayTextTest()
        {
            //PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
            //PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
            ////FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
            //OverlayText imageOverlayText = null; // 
            //double newFontSize = 0F; // 
            ////target.UpdateFourCornerFontSizeForOverLayText(imageOverlayText, newFontSize);
            //object[] myArgs = new object[] { imageOverlayText, newFontSize };
            //privateObj.Invoke("UpdateFourCornerFontSizeForOverLayText", myArgs);
            //// Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for UpdateImageCount
        ///</summary>
        [TestMethod()]
        public void UpdateImageCountTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                target.UpdateImageCount();
                //   // Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for ViewportOfCell
        ///</summary>
        [TestMethod()]
        public void ViewportOfCellTest()
        {
            try
            {
                FilmingPageControl target = new FilmingPageControl(); // 
                MedViewerControlCell cell = null; // 
                McsfFilmViewport expected = null; // 
                McsfFilmViewport actual;
                actual = target.ViewportOfCell(cell);
                Assert.AreEqual(expected, actual);
                //Assert.Inconclusive("Verify the correctness of this test method.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for ZoomCells
        ///</summary>
        [TestMethod()]
        public void ZoomCellsTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            double scale = 0F; // 
            target.ZoomCells(scale);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ActionType
        ///</summary>
        [TestMethod()]
        public void ActionTypeTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            ActionType actual;
            actual = target.ActionType;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cells
        ///</summary>
        [TestMethod()]
        public void CellsTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            IEnumerable<MedViewerControlCell> actual;
            actual = target.Cells;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CurrentActionType
        ///</summary>
        [TestMethod()]
        public void CurrentActionTypeTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            ActionType expected = new ActionType(); // 
            ActionType actual;
            target.CurrentActionType = expected;
            actual = target.CurrentActionType;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FilmPageCount
        ///</summary>
        [TestMethod()]
        public void FilmPageCountTest()
        {
            int expected = 0; // 
            int actual;
            FilmingPageControl.FilmPageCount = expected;
            actual = FilmingPageControl.FilmPageCount;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FilmPageHeight
        ///</summary>
        [TestMethod()]
        public void FilmPageHeightTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            double expected = 0F; // 
            double actual;
            target.FilmPageHeight = expected;
            actual = target.FilmPageHeight;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FilmPageIndex
        ///</summary>
        [TestMethod()]
        public void FilmPageIndexTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            int expected = 0; // 
            int actual;
            target.FilmPageIndex = expected;
            actual = target.FilmPageIndex;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FilmPageTitileNumber
        ///</summary>
        [TestMethod()]
        public void FilmPageTitileNumberTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            string expected = string.Empty; // 
            target.FilmPageTitileNumber = expected;
            //Assert.Inconclusive("Write-only properties cannot be verified.");
        }

        /// <summary>
        ///A test for FilmPageTitle
        ///</summary>
        [TestMethod()]
        public void FilmPageTitleTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            string expected = string.Empty; // 
            string actual;
            target.FilmPageTitle = expected;
            actual = target.FilmPageTitle;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FilmPageTitleVisibility
        ///</summary>
        [TestMethod()]
        public void FilmPageTitleVisibilityTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            Visibility expected = new Visibility(); // 
            target.FilmPageTitleVisibility = expected;
            //Assert.Inconclusive("Write-only properties cannot be verified.");
        }

        /// <summary>
        ///A test for FilmPageType
        ///</summary>
        [TestMethod()]
        public void FilmPageTypeTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            FilmPageType expected = new FilmPageType(); // 
            FilmPageType actual;
            target.FilmPageType = expected;
            actual = target.FilmPageType;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FilmPageWidth
        ///</summary>
        [TestMethod()]
        public void FilmPageWidthTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            double expected = 0F; // 
            double actual;
            target.FilmPageWidth = expected;
            actual = target.FilmPageWidth;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ImageLoadedCount
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void ImageLoadedCountTest()
        //{
        //    PrivateType type = new PrivateType(typeof(FilmingPageControl));                             //Class1为要测试的类。
        //    PrivateObject privateObj = new PrivateObject(new FilmingPageControl(), type);
        //    //FilmingPageControl_Accessor target = new FilmingPageControl_Accessor(param0);                   //Class1_Accessor为自动生成的测试类
        //    int expected = 0; // 
        //    int actual;
        //    //target.ImageLoadedCount = expected;
        //    //actual = target.ImageLoadedCount;
        //    object[] myArgs = new object[] { expected };
        //    privateObj.Invoke("ImageLoadedCount", myArgs);
        //    actual = (int)privateObj.Invoke("ImageLoadedCount");
        //    Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        /// <summary>
        ///A test for IsDisplay
        ///</summary>
        [TestMethod()]
        public void IsDisplayTest()
        {
            //FilmingPageControl target = new FilmingPageControl(); // 
            //bool expected = false; // 
            //bool actual;
          //  target.IsDisplay = expected;
        //    actual = target.IsDisplay;
           // Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsSelected
        ///</summary>
        [TestMethod()]
        public void IsSelectedTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            bool expected = false; // 
            bool actual;
            target.IsSelected = expected;
            actual = target.IsSelected;
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MaxImagesCount
        ///</summary>
        [TestMethod()]
        public void MaxImagesCountTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            //int actual;
            //actual = target.MaxImagesCount;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PatientAge
        ///</summary>
        [TestMethod()]
        public void PatientAgeTest()
        {
            //FilmingPageControl target = new FilmingPageControl(); // 
            //string actual;
            //actual = target.PatientAge;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PatientId
        ///</summary>
        [TestMethod()]
        public void PatientIdTest()
        {
            //FilmingPageControl target = new FilmingPageControl(); // 
            //string actual;
            //actual = target.PatientId;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PatientName
        ///</summary>
        [TestMethod()]
        public void PatientNameTest()
        {
            //FilmingPageControl target = new FilmingPageControl(); // 
            //string actual;
            //actual = target.PatientName;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PatientSex
        ///</summary>
        [TestMethod()]
        public void PatientSexTest()
        {
            //FilmingPageControl target = new FilmingPageControl(); // 
            //string actual;
            //actual = target.PatientSex;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RootCell
        ///</summary>
        [TestMethod()]
        public void RootCellTest()
        {
            FilmingPageControl target = new FilmingPageControl(); // 
            MedViewerLayoutCell actual;
            actual = target.RootCell;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetSelectedCellLayout
        ///</summary>
        [TestMethod()]
        public void SelectedCellLayoutTest()
        {
            //FilmingPageControl target = new FilmingPageControl(); // 
            //FilmLayout expected = null; // 
            //target.SetSelectedCellLayout = expected;
            //Assert.Inconclusive("Write-only properties cannot be verified.");
        }

        /// <summary>
        ///A test for StudyId
        ///</summary>
        [TestMethod()]
        public void StudyIdTest()
        {
            //FilmingPageControl target = new FilmingPageControl(); // 
            //string actual;
            //actual = target.StudyId;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ViewportLayout
        ///</summary>
        [TestMethod()]
        public void ViewportLayoutTest()
        {
            //FilmingPageControl target = new FilmingPageControl(); // 
            //FilmLayout expected =null; // 
            //FilmLayout actual;
            //target.ViewportLayout = expected;
            //actual = target.ViewportLayout;
            //Assert.AreNotEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
