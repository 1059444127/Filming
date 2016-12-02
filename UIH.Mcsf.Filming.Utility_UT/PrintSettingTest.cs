using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UIH.Mcsf.Filming.Utility_UT
{
    
    
    /// <summary>
    ///This is a test class for PrintSettingTest and is intended
    ///to contain all PrintSettingTest Unit Tests
    ///</summary>
    [TestClass]
    public class PrintSettingTest
    {
        /// <summary>
        ///A test for PrintSetting Constructor
        ///</summary>
        [TestMethod]
        public void PrintSettingConstructorTest()
        {
            var target = new PrintSetting();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Copies
        ///</summary>
        [TestMethod]
        public void CopiesTest()
        {
            var target = new PrintSetting(); 
            const uint expected = 5;
            target.Copies = expected;
            uint actual = target.Copies;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Priority
        ///</summary>
        [TestMethod]
        public void PriorityTest()
        {
            var target = new PrintSetting(); 
            const PRINT_PRIORITY expected = new PRINT_PRIORITY();
            target.Priority = expected;
            PRINT_PRIORITY actual = target.Priority;
            Assert.AreEqual(expected, actual);
        }

       //   <summary>
       // A test for FilmingDateTime
       // </summary>
        [TestMethod]
        public void FilmingDateTimeTest()
        {
            var target = new PrintSetting(); // 
            var expected = new DateTime(); // 
            target.FilmingDateTime = expected;
            DateTime actual = target.FilmingDateTime;
            Assert.AreEqual(expected, actual);
        }

        //   <summary>
        // A test for FilmingDateTime
        // </summary>
        [TestMethod]
        public void IfSaveElectronicFilmTest()
        {
            var target = new PrintSetting(); // 
            const bool expected = true; // 
            target.IfSaveElectronicFilm = expected;
            bool actual = target.IfSaveElectronicFilm;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for MediaType
        ///</summary>
        [TestMethod]
        public void MediaTypeTest()
        {
            var target = new PrintSetting(); 
            string expected = string.Empty;
            target.MediaType = expected;
            string actual = target.MediaType;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FilmDestination
        ///</summary>
        [TestMethod]
        public void FilmDestinationTest()
        {
            var target = new PrintSetting(); 
            string expected = string.Empty;
            target.FilmDestination = expected;
            string actual = target.FilmDestination;
            Assert.AreEqual(expected, actual);
        }
    }
}
