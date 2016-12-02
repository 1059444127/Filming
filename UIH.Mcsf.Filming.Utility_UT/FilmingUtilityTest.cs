using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Utility_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingUtilityTest and is intended
    ///to contain all FilmingUtilityTest Unit Tests
    ///</summary>
    [TestClass]
    public class FilmingUtilityTest
    {

        /// <summary>
        ///A test for AssertEnumerable
        ///</summary>
        public void AssertEnumerableTestHelper<T>()
        {
            FilmingUtility.AssertEnumerable<T>(null);
            
        }

        [TestMethod]
        public void AssertEnumerableTest()
        {
            AssertEnumerableTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for GenerateRowsAndColsForSeriesCompare
        ///</summary>
        [TestMethod]
        public void GenerateRowsAndColsForSeriesCompareTest()
        {
            int seriesCount = 10; 
            List<int> rows; 
            List<int> cols; 
            uint maxRow = 5; 
            uint maxCol = 5; 
            FilmingUtility.GenerateRowsAndColsForSeriesCompare(false, seriesCount, out rows, out cols, maxRow, maxCol);
            Assert.AreNotEqual(null, rows);
            Assert.AreNotEqual(null, cols);

            FilmingUtility.GenerateRowsAndColsForSeriesCompare(true, seriesCount, out rows, out cols, maxRow, maxCol);
            
        }
    }
}
