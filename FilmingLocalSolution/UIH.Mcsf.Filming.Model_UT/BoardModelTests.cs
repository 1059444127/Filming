using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIH.Mcsf.Filming.Adapters;
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
            var boardModel = new BoardModel(new DataModelStub());
            boardModel.CellCount = 4;

            // Act
            boardModel.NewPage();
            boardModel.NewPage();
            boardModel.NewPage();
            boardModel.NewPage();
            boardModel.NewPage();
            boardModel.CellCount = 6;
            boardModel.NewPage();

            // Assert
            Assert.AreEqual(0, boardModel.BoardNO);
            Assert.AreEqual(1, boardModel.BoardCount);
        }
    }

    class DataModelStub : DataModel
    {
        #region Overrides of DataModel

        public override void AppendPage()
        {
            Add(PageModelFactory.CreatePageModel(LayoutFactory.CreateLayout(3, 3)));

            var lastPageNO = Count - 1;
            PageChange(lastPageNO);
            FocusChange(lastPageNO);
        }

        #endregion
    }
}