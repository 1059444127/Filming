using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    ///// <summary>
    /////This is a test class for PrinterSettingDataViewModalTest and is intended
    /////to contain all PrinterSettingDataViewModalTest Unit Tests
    /////</summary>
    //[TestClass]
    //public class PrinterSettingDataViewModalTest
    //{


    //    private TestContext testContextInstance;

    //    /// <summary>
    //    ///Gets or sets the test context which provides
    //    ///information about and functionality for the current test run.
    //    ///</summary>
    //    public TestContext TestContext
    //    {
    //        get
    //        {
    //            return testContextInstance;
    //        }
    //        set
    //        {
    //            testContextInstance = value;
    //        }
    //    }

    //    #region Additional test attributes
    //    // 
    //    //You can use the following additional attributes as you write your tests:
    //    //
    //    //Use ClassInitialize to run code before running the first test in the class
    //    //[ClassInitialize()]
    //    //public static void MyClassInitialize(TestContext testContext)
    //    //{
    //    //}
    //    //
    //    //Use ClassCleanup to run code after all tests in a class have run
    //    //[ClassCleanup()]
    //    //public static void MyClassCleanup()
    //    //{
    //    //}
    //    //
    //    //Use TestInitialize to run code before running each test
    //    //[TestInitialize()]
    //    //public void MyTestInitialize()
    //    //{
    //    //}
    //    //
    //    //Use TestCleanup to run code after each test has run
    //    //[TestCleanup()]
    //    //public void MyTestCleanup()
    //    //{
    //    //}
    //    //
    //    #endregion


    //    /// <summary>
    //    ///A test for PrinterSettingDataViewModal Constructor
    //    ///</summary>
    //    [TestMethod]
    //    public void PrinterSettingDataViewModalConstructorTest()
    //    {
    //        var target = new PrinterSettingDataViewModal();
    //        Assert.IsNotNull(target);
    //    }


    //    private bool IsEqual<T>(IList<T> list1, IList<T> list2)
    //    {

    //        int count = 0;
    //        if ((count = list1.Count) != list2.Count)
    //            return false;
    //        for (int i = 0; i < count; i++)
    //            if (!list1[i].Equals(list2[i]))
    //            {
    //                return false;
    //            }
    //        return true;
    //    }

    //    /// <summary>
    //    ///A test for Clone
    //    ///</summary>
    //    [TestMethod]
    //    public void CloneTest()
    //    {
    //        //var target = new PrinterSettingDataViewModal();
    //        //PrinterSettingDataViewModal actual;
    //        //actual = target.Clone();
            
    //        //Assert.IsTrue(IsEqual(target.AnnotationTypeList, actual.AnnotationTypeList));
    //        //Assert.IsTrue(IsEqual(target.CurrentPrinterFilmSizeList, actual.CurrentPrinterFilmSizeList));
    //        //Assert.IsTrue(IsEqual(target.FilmCopyCountList, actual.FilmCopyCountList));
    //        //Assert.IsTrue(IsEqual(target.FilmOrientationList, actual.FilmOrientationList));
    //        ////Assert.IsTrue(IsEqual(target.FilmScaleModeList, actual.FilmScaleModeList));
    //        //Assert.IsTrue(IsEqual(target.RegisterPrinterAEList, actual.RegisterPrinterAEList));

    //        //Assert.AreEqual(target.CurrentAnnotationType, actual.CurrentAnnotationType);
    //        //Assert.AreEqual(target.CurrentCopyCount, actual.CurrentCopyCount);
    //        //Assert.AreEqual(target.CurrentFilmSize, actual.CurrentFilmSize);
    //        //Assert.AreEqual(target.CurrentFilmOrientation, actual.CurrentFilmOrientation);
    //        //Assert.AreEqual(target.CurrentPrinterAE, actual.CurrentPrinterAE);
    //        ////Assert.AreEqual(target.CurrentScaleMode, actual.CurrentScaleMode);
    //        //Assert.AreEqual(target.CurrentFilmSizeRatioOfPortrait, actual.CurrentFilmSizeRatioOfPortrait);
    //    }

    //    /// <summary>
    //    ///A test for InitFilmAnnotation
    //    ///</summary>
    //    [TestMethod]
    //    [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
    //    public void InitFilmAnnotationTest()
    //    {
    //        //var target = new PrinterSettingDataViewModal_Accessor();
    //        //target.InitFilmAnnotation();
    //    }

    //    /// <summary>
    //    ///A test for InitFilmCopyCount
    //    ///</summary>
    //    [TestMethod]
    //    [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
    //    public void InitFilmCopyCountTest()
    //    {
    //        //var target = new PrinterSettingDataViewModal_Accessor();
    //        //target.InitFilmCopyCount();
    //    }

    //    /// <summary>
    //    ///A test for InitFilmOrientation
    //    ///</summary>
    //    [TestMethod]
    //    [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
    //    public void InitFilmOrientationTest()
    //    {
    //        //var target = new PrinterSettingDataViewModal_Accessor();
    //        //target.InitFilmOrientation();
    //    }

    //    /// <summary>
    //    ///A test for InitFilmSizeInfoByAE
    //    ///</summary>
    //    [TestMethod]
    //    [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
    //    public void InitFilmSizeInfoByAETest()
    //    {
    //        //var target = new PrinterSettingDataViewModal_Accessor();
    //        //string printerAE = string.Empty;
    //        //target.InitFilmSizeInfoByAE(printerAE);
    //    }

    //    /// <summary>
    //    ///A test for InitPrinterAE
    //    ///</summary>
    //    [TestMethod]
    //    [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
    //    public void InitPrinterAETest()
    //    {
    //        //var target = new PrinterSettingDataViewModal_Accessor();
    //        //target.InitPrinterAE();
    //    }

    //    /// <summary>
    //    ///A test for InitScaleMode
    //    ///</summary>
    //    //[TestMethod]
    //    //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
    //    //public void InitScaleModeTest()
    //    //{
    //    //    PrinterSettingDataViewModal_Accessor target = new PrinterSettingDataViewModal_Accessor(); 
    //    //    target.InitScaleMode();
    //    //}

    //    /// <summary>
    //    ///A test for OnPropertyChanged
    //    ///</summary>
    //    [TestMethod]
    //    [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
    //    public void OnPropertyChangedTest()
    //    {
    //        //var target = new PrinterSettingDataViewModal_Accessor();
    //        //target.OnPropertyChanged(new PropertyChangedEventArgs("CurrentPrinterAE"));
    //    }

    //    /// <summary>
    //    ///A test for AnnotationTypeList
    //    ///</summary>
    //    //[TestMethod]
    //    //public void AnnotationTypeListTest()
    //    //{
    //    //    //var target = new PrinterSettingDataViewModal_Accessor(); 
    //    //    //ObservableCollection<ImgTxtDisplayState> expected = null; 
    //    //    //ObservableCollection<ImgTxtDisplayState> actual;
    //    //    //target.AnnotationTypeList = expected;
    //    //    //actual = target.AnnotationTypeList;
    //    //    //Assert.AreEqual(expected, actual);
    //    //}

    //    /// <summary>
    //    ///A test for CurrentAnnotationType
    //    ///</summary>
    //    [TestMethod]
    //    public void CurrentAnnotationTypeTest()
    //    {
    //        try
    //        {
    //            var target = new PrinterSettingDataViewModal();
    //            var expected = ImgTxtDisplayState.All;
    //            ImgTxtDisplayState actual;
    //            target.CurrentAnnotationType = expected;
    //            actual = target.CurrentAnnotationType;
    //            Assert.AreEqual(expected, actual);

    //            expected = ImgTxtDisplayState.Anonymity;
    //            target.CurrentAnnotationType = expected;
    //            actual = target.CurrentAnnotationType;
    //            Assert.AreEqual(expected, actual);

    //            expected = ImgTxtDisplayState.None;
    //            target.CurrentAnnotationType = expected;
    //            actual = target.CurrentAnnotationType;
    //            Assert.AreEqual(expected, actual);

    //            expected = ImgTxtDisplayState.Customization;
    //            target.CurrentAnnotationType = expected;
    //            actual = target.CurrentAnnotationType;
    //            Assert.AreEqual(expected, actual);
    //        }
    //        catch (Exception ex)
    //        {
    //            Logger.LogFuncException(ex.Message+ex.StackTrace);
    //        }
    //    }

    //    /// <summary>
    //    ///A test for CurrentCopyCount
    //    ///</summary>
    //    //[TestMethod]
    //    //public void CurrentCopyCountTest()
    //    //{
    //    //    //var target = new PrinterSettingDataViewModal();
    //    //    //uint expected = 1; 
    //    //    //uint actual;
    //    //    //target.CurrentCopyCount = expected;
    //    //    //actual = target.CurrentCopyCount;
    //    //    //Assert.AreEqual(expected, actual);
    //    //}

    //    /// <summary>
    //    ///A test for CurrentFilmSize
    //    ///</summary>
    //    [TestMethod]
    //    //public void CurrentFilmSizeTest()
    //    //{
    //    //    //var target = new PrinterSettingDataViewModal();
    //    //    //string expected = "test"; 
    //    //    //string actual;
    //    //    //target.CurrentFilmSize = expected;
    //    //    //actual = target.CurrentFilmSize;
    //    //    //Assert.AreEqual(expected, actual);
    //    //}

    //    /// <summary>
    //    ///A test for CurrentFilmOrientation
    //    ///</summary>
    //    //[TestMethod]
    //    //public void CurrentOrientationTest()
    //    //{
    //    //    //var target = new PrinterSettingDataViewModal(); 
    //    //    //var expected = new FilmOrientationEnum();
    //    //    //FilmOrientationEnum actual;
    //    //    //target.CurrentFilmOrientation = expected;
    //    //    //actual = target.CurrentFilmOrientation;
    //    //    //Assert.AreEqual(expected, actual);
    //    //}

    //    /// <summary>
    //    ///A test for CurrentPrinterAE
    //    ///</summary>
    //    //[TestMethod]
    //    //public void CurrentPrinterAETest()
    //    //{
    //    //    //var target = new PrinterSettingDataViewModal(); 
    //    //    //string expected = "test"; 
    //    //    //string actual;
    //    //    //target.CurrentPrinterAE = expected;
    //    //    //actual = target.CurrentPrinterAE;
    //    //    //Assert.AreEqual(expected, actual);
    //    //}

    //    /// <summary>
    //    ///A test for CurrentPrinterFilmSizeList
    //    ///</summary>
    //    //[TestMethod]
    //    //public void CurrentPrinterFilmSizeListTest()
    //    //{
    //    //    //var target = new PrinterSettingDataViewModal(); 
    //    //    //ObservableCollection<string> expected = null; 
    //    //    //ObservableCollection<string> actual;
    //    //    //target.CurrentPrinterFilmSizeList = expected;
    //    //    //actual = target.CurrentPrinterFilmSizeList;
    //    //    //Assert.AreEqual(expected, actual);
    //    //}

    //    /// <summary>
    //    ///A test for CurrentScaleMode
    //    ///</summary>
    //    //[TestMethod]
    //    //public void CurrentScaleModeTest()
    //    //{
    //    //    try
    //    //    {
    //    //        var target = new PrinterSettingDataViewModal();
    //    //        var expected = FilmScaleModeEnum.FitToFilm;
    //    //        FilmScaleModeEnum actual;
    //    //        target.CurrentScaleMode = expected;
    //    //        actual = target.CurrentScaleMode;
    //    //        Assert.AreEqual(expected, actual);

    //    //        expected = FilmScaleModeEnum.Scale_1X;
    //    //        target.CurrentScaleMode = expected;
    //    //        actual = target.CurrentScaleMode;
    //    //        Assert.AreEqual(expected, actual);

    //    //        expected = FilmScaleModeEnum.Scale_2X;
    //    //        target.CurrentScaleMode = expected;
    //    //        actual = target.CurrentScaleMode;
    //    //        Assert.AreEqual(expected, actual);
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        Logger.LogFuncException(ex.Message+ex.StackTrace);
    //    //    }

    //    //}

    //    /// <summary>
    //    ///A test for FilmCopyCountList
    //    ///</summary>
    //    //[TestMethod]
    //    //public void FilmCopyCountListTest()
    //    //{
    //    //    //var target = new PrinterSettingDataViewModal();
    //    //    //ObservableCollection<uint> expected = null;
    //    //    //ObservableCollection<uint> actual;
    //    //    //target.FilmCopyCountList = expected;
    //    //    //actual = target.FilmCopyCountList;
    //    //    //Assert.AreEqual(expected, actual);
    //    //}

    //    /// <summary>
    //    ///A test for FilmOrientationList
    //    ///</summary>
    //    //[TestMethod]
    //    //public void FilmOrientationListTest()
    //    //{
    //    //    //var target = new PrinterSettingDataViewModal();
    //    //    //ObservableCollection<FilmOrientationEnum> expected = null;
    //    //    //ObservableCollection<FilmOrientationEnum> actual;
    //    //    //target.FilmOrientationList = expected;
    //    //    //actual = target.FilmOrientationList;
    //    //    //Assert.AreEqual(expected, actual);
    //    //}

    //    /// <summary>
    //    ///A test for FilmScaleModeList
    //    ///</summary>
    //    //[TestMethod]
    //    //public void FilmScaleModeListTest()
    //    //{
    //    //    var target = new PrinterSettingDataViewModal(); 
    //    //    ObservableCollection<FilmScaleModeEnum> expected = null; 
    //    //    ObservableCollection<FilmScaleModeEnum> actual;
    //    //    target.FilmScaleModeList = expected;
    //    //    actual = target.FilmScaleModeList;
    //    //    Assert.AreEqual(expected, actual);
    //    //}

    //    /// <summary>
    //    ///A test for CurrentFilmSizeRatioOfPortrait
    //    ///</summary>
    //    //[TestMethod]
    //    //public void FilmSizeTest()
    //    //{
    //    //    //PrinterSettingDataViewModal target = new PrinterSettingDataViewModal();
    //    //    //string expected = "test";
    //    //    //string actual;
    //    //    //target.CurrentFilmSizeRatioOfPortrait = expected;
    //    //    //actual = target.CurrentFilmSizeRatioOfPortrait;
    //    //    //Assert.AreEqual(expected, actual);
    //    //}

    //    /// <summary>
    //    ///A test for RegisterPrinterAEList
    //    ///</summary>
    //    //[TestMethod]
    //    //public void RegisterPrinterAEListTest()
    //    //{
    //    //    //PrinterSettingDataViewModal target = new PrinterSettingDataViewModal(); 
    //    //    //ObservableCollection<string> expected = null; 
    //    //    //ObservableCollection<string> actual;
    //    //    //target.RegisterPrinterAEList = expected;
    //    //    //actual = target.RegisterPrinterAEList;
    //    //    //Assert.AreEqual(expected, actual);
    //    //}
    //}
}
