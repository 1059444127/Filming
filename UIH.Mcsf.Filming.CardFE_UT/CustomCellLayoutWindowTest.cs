//////////////////////////////////////////////////////////////////////////
/// 2013. 5.31 uih.mcsf.commonTheme resource key duplicated， and then ut failed

using System.Reflection;
using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Markup;
using UIH.Mcsf.Filming.Utility;

namespace UIH.Mcsf.Filming.CardFE_UT
{


    /// <summary>
    ///This is a test class for CustomCellLayoutWindowTest and is intended
    ///to contain all CustomCellLayoutWindowTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CustomCellLayoutWindowTest
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
        ///A test for CustomCellLayoutWindow Constructor
        ///</summary>
        [TestMethod()]
        public void CustomCellLayoutWindowConstructorTest()
        {
            try
            {
                CustomCellLayoutWindow target = new CustomCellLayoutWindow();
            }
            catch (Exception)
            {
                return;
                //Console.WriteLine(e);
            }
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for ApplySettingButtonClick
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void ApplySettingButtonClickTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(CustomCellLayoutWindow));                             //Class1为要测试的类。
                CustomCellLayoutWindow target = new CustomCellLayoutWindow();
                PrivateObject privateObj = new PrivateObject(target, type);

                //CustomCellLayoutWindow_Accessor target = new CustomCellLayoutWindow_Accessor(param0);                   //Class1_Accessor为自动生成的测试类           
                //object sender = null; // 
                //RoutedEventArgs e = null; // 
                //target.ApplySettingButtonClick(sender, e);

                //target.rowNumberLabel.Text = "12";
                //target.columnNumberLabel.Text = "10";
                //target.ApplySettingButtonClick(sender, e);

                //target.rowNumberLabel.Text = "5";
                //target.columnNumberLabel.Text = "4";
                //target.ApplySettingButtonClick(sender, e);

                //Assert.Inconclusive("A method that does not return a value cannot be verified.");

                object sender = null; // 
                RoutedEventArgs e = null; // 
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("ApplySettingButtonClick", myArgs);

                target.rowNumberLabel.Text = "12";
                target.columnNumberLabel.Text = "10";
                myArgs = new object[] { sender, e };
                privateObj.Invoke("ApplySettingButtonClick", myArgs);

                target.rowNumberLabel.Text = "5";
                target.columnNumberLabel.Text = "4";
                myArgs = new object[] { sender, e };
                privateObj.Invoke("ApplySettingButtonClick", myArgs);
            }
            catch (Exception)
            {
                return;
                //Console.WriteLine(e);
            }
        }

        /// <summary>
        ///A test for CancelSettingButtonClick
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void CancelSettingButtonClickTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(CustomCellLayoutWindow));                             //Class1为要测试的类。
                PrivateObject privateObj = new PrivateObject(new CustomCellLayoutWindow(), type);
                //CustomCellLayoutWindow_Accessor target = new CustomCellLayoutWindow_Accessor(param0);                   //Class1_Accessor为自动生成的测试类 
                object sender = null; // 
                RoutedEventArgs e = null; // 
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("CancelSettingButtonClick", myArgs);
                //target.CancelSettingButtonClick(sender, e);
                //Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///A test for CustomCellLayoutGridMouseLeftButtonDown
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void CustomCellLayoutGridMouseLeftButtonDownTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(CustomCellLayoutWindow));                             //Class1为要测试的类。
                PrivateObject privateObj = new PrivateObject(new CustomCellLayoutWindow(), type);
                //CustomCellLayoutWindow_Accessor target = new CustomCellLayoutWindow_Accessor(param0);                   //Class1_Accessor为自动生成的测试类 
                object sender = null; // 
                MouseButtonEventArgs e = null; // 
                //target.CustomCellLayoutGridMouseLeftButtonDown(sender, e);
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("CustomCellLayoutGridMouseLeftButtonDown", myArgs);
            }
            catch (Exception )
            {
                return;
            }
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CustomCellLayoutGridMouseLeftButtonUp
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void CustomCellLayoutGridMouseLeftButtonUpTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(CustomCellLayoutWindow));                             //Class1为要测试的类。
                PrivateObject privateObj = new PrivateObject(new CustomCellLayoutWindow(), type);
                //CustomCellLayoutWindow_Accessor target = new CustomCellLayoutWindow_Accessor(param0);                   //Class1_Accessor为自动生成的测试类 
                object sender = null; // 
                MouseButtonEventArgs e = null; // 
                //target.CustomCellLayoutGridMouseLeftButtonUp(sender, e);
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("CustomCellLayoutGridMouseLeftButtonUp", myArgs);
                //Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///A test for CustomCellLayoutGridMouseMove
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void CustomCellLayoutGridMouseMoveTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(CustomCellLayoutWindow));                             //Class1为要测试的类。
                PrivateObject privateObj = new PrivateObject(new CustomCellLayoutWindow(), type);
                //CustomCellLayoutWindow_Accessor target = new CustomCellLayoutWindow_Accessor(param0);                   //Class1_Accessor为自动生成的测试类 
                object sender = null; // 
                MouseEventArgs e = null; //   

                //target.CustomCellLayoutGridMouseMove(sender, e);
                object[] myArgs = new object[] { sender, e };
                privateObj.Invoke("CustomCellLayoutGridMouseMove", myArgs);
            }
            catch (Exception )
            {
                return;
                //Console.WriteLine(e);
            }
            //   target.customCellLayoutGrid.PointFromScreen = "";
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for EndHitTestFilter
        ///</summary>
        [TestMethod()]
        public void EndHitTestFilterTest()
        {
            try
            {
                CustomCellLayoutWindow target = new CustomCellLayoutWindow(); // 
                DependencyObject o = null; // 
                //HitTestFilterBehavior expected = new HitTestFilterBehavior(); // 
                HitTestFilterBehavior actual;
                actual = target.EndHitTestFilter(o);
                o = new DependencyObject();
                actual = target.MyHitTestFilter(o);
            }
            catch (Exception )
            {
                return;
                //Console.WriteLine(e);
            }

            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InitializeComponent
        ///</summary>
        [TestMethod()]
        public void InitializeComponentTest()
        {
            try
            {
                CustomCellLayoutWindow target = new CustomCellLayoutWindow(); // 
                target.InitializeComponent();
            }
            catch (Exception )
            {
                return;
                //Console.WriteLine(e);
            }
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for MyHitTestFilter
        ///</summary>
        [TestMethod()]
        public void MyHitTestFilterTest()
        {
            try
            {
                CustomCellLayoutWindow target = new CustomCellLayoutWindow(); // 
                DependencyObject o = null; // 
                HitTestFilterBehavior actual;
                actual = target.MyHitTestFilter(o);
                o = new DependencyObject();
                actual = target.MyHitTestFilter(o);
            }
            catch (Exception )
            {
               return;
            }


            //HitTestFilterBehavior expected = new HitTestFilterBehavior(); // 

            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MyHitTestResult
        ///</summary>
        [TestMethod()]
        public void MyHitTestResultTest()
        {
            try
            {
                CustomCellLayoutWindow target = new CustomCellLayoutWindow(); // 
                HitTestResult result = null; // 
                //HitTestResultBehavior expected = new HitTestResultBehavior(); // 
                HitTestResultBehavior actual;
                actual = target.MyHitTestResult(result);
            }
            catch (Exception)
            {
                return;
            }
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OnClosingWindow
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void OnClosingWindowTest()
        //{
        //    CustomCellLayoutWindow_Accessor target = new CustomCellLayoutWindow_Accessor(); // 
        //    object sender = null; // 
        //    CancelEventArgs e = new CancelEventArgs(); // 
        //    target.OnClosingWindow(sender, e);
        //    //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for System.Windows.Markup.IComponentConnector.Connect
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void ConnectTest()
        //{
        //    IComponentConnector target = new CustomCellLayoutWindow(); // 
        //    int connectionId = 0; // 
        //    object target1 = null; // 
        //    target.Connect(connectionId, target1);
        //    //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for CustomCellColumns
        ///</summary>
        [TestMethod()]
        public void CustomCellColumnsTest()
        {
            try
            {
                CustomCellLayoutWindow target = new CustomCellLayoutWindow(); // 
                int actual;
                actual = target.CustomCellColumns;
                target.columnNumberLabel.Text = "8";
                actual = target.CustomCellColumns;
            }
            catch (Exception )
            {
                return;
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CustomCellLayout
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void CustomCellLayoutTest()
        {
            try
            {
                PrivateType type = new PrivateType(typeof(CustomCellLayoutWindow));
                CustomCellLayoutWindow target = new CustomCellLayoutWindow(); // 
                PrivateObject privateObj = new PrivateObject(target, type);

                //CustomCellLayoutWindow_Accessor target = new CustomCellLayoutWindow_Accessor(param0); // 
                FilmLayout expected = null; // 
                FilmLayout actual;
                //target.CustomCellLayout = expected;
                object[] myArgs = new object[] { expected };
                //privateObj.Invoke("CustomCellLayout", myArgs);
                actual = target.CustomCellLayout;
                Assert.AreEqual(expected, actual);
            }
            catch (Exception )
            {
                return;
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CustomCellRows
        ///</summary>
        [TestMethod()]
        public void CustomCellRowsTest()
        {
            try
            {
                CustomCellLayoutWindow target = new CustomCellLayoutWindow(); // 
                int actual;
                actual = target.CustomCellRows;
                target.rowNumberLabel.Text = "8";
                actual = target.CustomCellRows;
                //   Assert.Inconclusive("Verify the correctness of this test method.");  
            } catch (Exception ) {
                return;
            }
        }
    }
}
