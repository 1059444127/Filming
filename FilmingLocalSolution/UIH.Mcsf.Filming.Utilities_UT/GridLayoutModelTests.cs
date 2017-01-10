using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.Utilities_UT
{
    [TestClass]
    public class GridLayoutModelTests
    {
        [TestMethod]
        public void Position_Of_Child_in_Grid_1_1_is_0_0()
        {
            // Arrange
            var gridLayoutModel = new GridLayoutModel(1, 1);

            // Act 
            var position = gridLayoutModel.GetGridPositionBy(3);

            // Assert
            Assert.AreEqual(0, position.Row);
            Assert.AreEqual(0, position.Col);
        }

        [TestMethod]
        public void Position_Of_Child_7__in_Grid_2_3_is_0_1()
        {
            // Arrange 
            var gridLayoutModel = new GridLayoutModel(2, 3);

            // Act
            var position = gridLayoutModel.GetGridPositionBy(7);

            // Assert
            Assert.AreEqual(0, position.Row);
            Assert.AreEqual(1, position.Col);
        }
    }
}
