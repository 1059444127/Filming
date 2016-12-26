using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.UT
{
    [TestClass]
    public class BoardModelTests
    {
        // TODO-User-intent: New_Page_When_Board_Control_Is_Full_Of_Page_Control_Then_New_Page_Is_Invisible
        // TODO-User-intent-working-on: New_Page_When_Board_Control_Is_Not_Fullfilled_Then_New_Page_Is_Displayed_at_the_end
        [TestMethod]
        public void New_Page_When_Board_Control_Is_Not_Fullfilled_Then_New_Page_Is_Displayed_at_the_end()
        {
            // Arrange
            var boardModel = new BoardModel();

            // Act
            boardModel.NewPage();

            // Assert
            var pageModel = boardModel.PageModels.FirstOrDefault();
            Assert.IsTrue(pageModel!= null && pageModel.IsVisible);
        }
    }
}
