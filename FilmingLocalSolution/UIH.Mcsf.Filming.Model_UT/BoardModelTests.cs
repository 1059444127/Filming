using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIH.Mcsf.Filming.Model;

namespace UIH.Mcsf.Filming.Model_UT
{
    [TestClass]
    public class BoardModelTests
    {
        // TODO-User-intent-working-on: New_Page_Is_Always_Displayed_at_the_end
        [TestMethod]
        public void New_Page_Is_Always_Displayed_at_the_end()
        {
            // Arrange
            var boardModel = new BoardModel();
            boardModel.CellCount = 2;

            // Act
            boardModel.NewPage();
            boardModel.NewPage();
            boardModel.NewPage();

            // Assert
            Assert.AreEqual(1, boardModel.BoardNO);
            Assert.AreEqual(2, boardModel.BoardCount);
        }
    }
}