using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIH.Mcsf.Filming.ControlTests.Models;

namespace UIH.Mcsf.Filming.ControlTests_UT
{
    [TestClass]
    public class BoardContentTests
    {
        private BoardContent _boardContent;

        [TestInitialize]
        public void SetUp()
        {
            _boardContent = new BoardContent();
        }

        [TestMethod]
        public void First_Film_In_Board_Is_Visible()
        {

            // Assert
            Assert.IsTrue(_boardContent[0].IsVisible);
        }
    }
}
