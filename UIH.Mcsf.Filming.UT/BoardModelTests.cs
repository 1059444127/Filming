using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.UT
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

            // Act
            boardModel.NewPage();

            // Assert
            var pageModel = boardModel.PageDisplayModels.FirstOrDefault();
            Assert.IsTrue(pageModel!= null && pageModel.IsVisible);
        }
    }
}
