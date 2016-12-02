using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UIH.Mcsf.Filming.DataModel.UT
{
    [TestClass]
    public class CellBaseTest
    {
        [TestMethod]
        public void Cell_Can_Set_Or_Get_Selected_Status() //Method_Scenario_ExpectedResult
        {
            var cellMock = new Mock<ImageCell>();
            var cell = cellMock.Object;

            Assert.IsFalse(cell.IsSelected);

            cell.IsSelected = false;

            Assert.IsFalse(cell.IsSelected);

            cell.IsSelected = true;

            Assert.IsTrue(cell.IsSelected);

            cell.IsSelected = true;

            Assert.IsTrue(cell.IsSelected);

            cell.IsSelected = false;

            Assert.IsFalse(cell.IsSelected);
        }
    }
}