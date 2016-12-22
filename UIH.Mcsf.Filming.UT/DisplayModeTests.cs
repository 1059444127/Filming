using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.UT
{
    [TestClass]
    public class DisplayModeTests
    {
        [TestMethod]
        public void When_Element_Count_is_Even_Then_Row_is_2()
        {
            // Act
            var displayMode = new DisplayMode(8);

            // Assert
            Assert.AreEqual(2, displayMode.Row);
            Assert.AreEqual(4, displayMode.Col);
        }

        [TestMethod]
        public void When_Element_Count_is_Odd_Then_Row_is_1_And_Col_is_Count()
        {
            // Act
            var displayMode = new DisplayMode(3);

            // Assert
            Assert.AreEqual(1, displayMode.Row);
            Assert.AreEqual(3, displayMode.Col);
        }

        [TestMethod]
        public void When_Element_Count_is_equal_Then_DisplayMode_is_equal()
        {
            // Act
            var displayMode1 = new DisplayMode(8);
            var displayMode2 = new DisplayMode(8);

            // Assert
            Assert.IsTrue(displayMode1.Equals(displayMode2));
        }
    }
}