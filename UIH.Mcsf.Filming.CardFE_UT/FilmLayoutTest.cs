using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using UIH.Mcsf.Filming.Utility;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmLayoutTest and is intended
    ///to contain all FilmLayoutTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmLayoutTest
    {


        private TestContext _testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
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
        ///A test for FilmLayout Constructor
        ///</summary>
        //[TestMethod()]
        //public void FilmLayoutConstructorTest()
        //{
        //    string layoutXmlFileStream = "testXml";
        //    PrivateType type = new PrivateType(typeof(FilmLayout));
        //    PrivateObject privateObj = new PrivateObject(new FilmLayout(layoutXmlFileStream), type);
            
        //    var target = (FilmLayout)privateObj.Invoke("FilmLayout");
        //    Assert.IsNotNull(target);
        //}


        /// <key>\n 
        /// PRA:Yes \n
        /// Traced from: N/A \n
        /// Tests: DS_PRA_Filming_WYSWYG  \n
        /// Description: film layout constructor \n
        /// Short Description: FilmLayout \n
        /// Component: Filming \n
        /// </key> \n
        /// <summary>
        ///A test for FilmLayout Constructor
        ///</summary>
        [TestMethod()]
        public void FilmLayoutConstructorTest1()
        {
            int layoutRowsSize = 1; 
            int layoutColumnsSize = 2;

            FilmLayout target = new FilmLayout(layoutRowsSize, layoutColumnsSize);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for FilmLayout Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void FilmLayoutConstructorTest2()
        {
            FilmLayout target = new FilmLayout(3, 2, "3x2");
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for FilmLayout Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        public void FilmLayoutConstructorTest3()
        {
            string layoutXmlFileStream = "testXml";

            FilmLayout target = new FilmLayout(layoutXmlFileStream);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Clone
        ///</summary>
        [TestMethod()]
        public void CloneTest()
        {
            FilmLayout target = new FilmLayout(3, 2, "3x2");
            //Clone FilmLayout
            FilmLayout actual;
            actual = target.Clone();
            Assert.IsNotNull(actual);

            //Clone FilmLayout(layoutName, layoutXmlFileStream)
            string layoutXmlFileStream = "testXml";
            target = target = new FilmLayout(layoutXmlFileStream);
            actual = target.Clone();
            Assert.AreEqual(target.LayoutName, actual.LayoutName);
            Assert.AreEqual(target.LayoutXmlFileStream,actual.LayoutXmlFileStream);

            //Clone FilmLayout(layoutName, layoutRowsSize, layoutColumnsSize)
            int layoutRowsSize = 1;
            int layoutColumnsSize = 2;
            target = new FilmLayout(layoutRowsSize, layoutColumnsSize);
            actual = target.Clone();
            Assert.AreEqual(target.LayoutName, actual.LayoutName);
            Assert.AreEqual(target.LayoutRowsSize, actual.LayoutRowsSize);
            Assert.AreEqual(target.LayoutColumnsSize, actual.LayoutColumnsSize);
        }

        /// <summary>
        ///A test for LoadDefinedFilmLayout
        ///</summary>
        [TestMethod()]
        public void LoadDefinedFilmLayoutTest()
        {
            //FilmLayout_Accessor._definedFilmLayout =new List<string>();
            //List<string> actual;
            //actual = FilmLayout.LoadDefinedFilmLayout();
            //Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            FilmLayout target = new FilmLayout(3, 2, "test");
            string expected = "test";
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for LayoutColumnsSize
        ///</summary>
        //[TestMethod()]
        //public void LayoutColumnsSizeTest()
        //{
        //    //PrivateType type = new PrivateType(typeof(FilmLayout));                             //Class1为要测试的类。
        //    //PrivateObject param0 = new PrivateObject(new FilmLayout(3, 2, "3x2"), type);

        //    //FilmLayout_Accessor target = new FilmLayout_Accessor(param0); 
        //    //int expected = 5; 
        //    //int actual;
        //    //target.LayoutColumnsSize = expected;
        //    //actual = target.LayoutColumnsSize;
        //    //Assert.AreEqual(expected, actual);

        //    PrivateType type = new PrivateType(typeof(FilmLayout));                             //Class1为要测试的类。
        //    PrivateObject privateObj = new PrivateObject(new FilmLayout(3, 2, "3x2"), type);

        //    int expected = 5;
        //    int actual;
        //    object[] myArgs = new object[] { expected };
        //    privateObj.Invoke("LayoutColumnsSize", myArgs);
        //    actual = (int)privateObj.Invoke("LayoutColumnsSize");
        //    Assert.AreEqual(expected, actual);
        //}

        /// <summary>
        ///A test for LayoutName
        ///</summary>
        //[TestMethod()]
        //public void LayoutNameTest()
        //{
        //    //PrivateType type = new PrivateType(typeof(FilmLayout));                             //Class1为要测试的类。
        //    //PrivateObject param0 = new PrivateObject(new FilmLayout(3, 2, "3x2"), type);

        //    //FilmLayout_Accessor target = new FilmLayout_Accessor(param0); 
        //    //string expected = "test"; 
        //    //string actual;
        //    //target.LayoutName = expected;
        //    //actual = target.LayoutName;
        //    //Assert.AreEqual(expected, actual);
        //    PrivateType type = new PrivateType(typeof(FilmLayout));                             //Class1为要测试的类。
        //    PrivateObject privateObj = new PrivateObject(new FilmLayout(3, 2, "3x2"), type);

        //    string expected = "test";
        //    string actual;
        //    object[] myArgs = new object[] { expected };
        //    privateObj.Invoke("LayoutName", myArgs);
        //    actual = (string)privateObj.Invoke("LayoutName");
        //    Assert.AreEqual(expected, actual);
        //}

        /// <summary>
        ///A test for LayoutRowsSize
        ///</summary>
        //[TestMethod()]
        //public void LayoutRowsSizeTest()
        //{
        //    //PrivateType type = new PrivateType(typeof(FilmLayout));                             //Class1为要测试的类。
        //    //PrivateObject param0 = new PrivateObject(new FilmLayout(3, 2, "3x2"), type);

        //    //FilmLayout_Accessor target = new FilmLayout_Accessor(param0); 
        //    //int expected = 5; 
        //    //int actual;
        //    //target.LayoutRowsSize = expected;
        //    //actual = target.LayoutRowsSize;
        //    //Assert.AreEqual(expected, actual);
        //    PrivateType type = new PrivateType(typeof(FilmLayout));                             //Class1为要测试的类。
        //    PrivateObject privateObj = new PrivateObject(new FilmLayout(3, 2, "3x2"), type);

        //    int expected = 5;
        //    int actual;
        //    object[] myArgs = new object[] { expected };
        //    privateObj.Invoke("LayoutRowsSize", myArgs);
        //    actual = (int)privateObj.Invoke("LayoutRowsSize");
        //    Assert.AreEqual(expected, actual);
        //}

        /// <summary>
        ///A test for LayoutType
        ///</summary>
        [TestMethod()]
        public void LayoutTypeTest()
        {
            //PrivateType type = new PrivateType(typeof(FilmLayout));                             //Class1为要测试的类。
            //PrivateObject param0 = new PrivateObject(new FilmLayout(3, 2, "3x2"), type);

            //FilmLayout_Accessor target = new FilmLayout_Accessor(param0); 
            //LayoutTypeEnum expected = new LayoutTypeEnum(); 
            //LayoutTypeEnum actual;
            //target.LayoutType = expected;
            //actual = target.LayoutType;
            //Assert.AreEqual(expected, actual);
            FilmLayout target = new FilmLayout(3, 2, "3x2");
            LayoutTypeEnum expected = new LayoutTypeEnum();
            LayoutTypeEnum actual;
            target.LayoutType = expected;
            actual = target.LayoutType;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for LayoutXmlFileStream
        ///</summary>
        [TestMethod()]
        public void LayoutXmlFileStreamTest()
        {
            //PrivateType type = new PrivateType(typeof(FilmLayout));                             //Class1为要测试的类。
            //PrivateObject param0 = new PrivateObject(new FilmLayout(3, 2, "3x2"), type);

            //FilmLayout_Accessor target = new FilmLayout_Accessor(param0); 
            //string expected = "test"; 
            //string actual;
            //target.LayoutXmlFileStream = expected;
            //actual = target.LayoutXmlFileStream;
            //Assert.AreEqual(expected, actual);
            FilmLayout target = new FilmLayout(3, 2, "3x2");
            string expected = "test";
            string actual;
            target.LayoutXmlFileStream = expected;
            actual = target.LayoutXmlFileStream;
            Assert.AreEqual(expected, actual);
        }
    }
}
