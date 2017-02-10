using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIH.Mcsf.Filming.ControlTests.Models;
using UIH.Mcsf.Filming.Utilities;

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
        public void Only_First_Film_In_Board_Is_Visible()
        {

            // Assert
            Assert.IsTrue(_boardContent[0].IsVisible, "First Film in Board is InVisible");
            for (int i = 1; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                Assert.IsFalse(_boardContent[i].IsVisible, string.Format("Film {0} in Board is Visible", i));
            }
        }
    }
}
