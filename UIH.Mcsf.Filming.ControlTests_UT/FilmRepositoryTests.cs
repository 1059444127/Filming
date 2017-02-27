using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIH.Mcsf.Filming.ControlTests.Models;

namespace UIH.Mcsf.Filming.ControlTests_UT
{
    [TestClass]
    public class FilmRepositoryTests
    {
        private FilmRepository _filmRepository;

        [TestInitialize]
        public void SetUp()
        {
            _filmRepository = new FilmRepository();
        }

        [TestMethod]
        public void First_Film_Is_Visible()
        {
            // Assert
            Assert.IsTrue(_filmRepository[0].IsVisible);
        }

        [TestMethod]
        public void When_FilmRepository_Count_is_0_Then_MaxNO_is_0()
        {
            // Prepare

            // Act
            _filmRepository.Clear();

            // Assert
            Assert.AreEqual(0, _filmRepository.MaxNO);
        }

        [TestMethod]
        public void When_FilmRepository_Count_is_4_Then_MaxNO_is_0_If_VisibleCount_is_4()
        {
            // Prepare
            _filmRepository.VisibleCount = 4;

            // Act
            for (int i = 0; i < 3; i++)
            {
                _filmRepository.Append();
            }

            // Assert
            Assert.AreEqual(0, _filmRepository.MaxNO);
        }

        [TestMethod]
        public void When_FilmRepository_Count_is_5_Then_MaxNO_is_1_If_VisibleCount_is_4()
        {
            // Prepare
            _filmRepository.VisibleCount = 4;

            // Act
            for (int i = 0; i < 4; i++)
            {
                _filmRepository.Append();
            }

            // Assert
            Assert.AreEqual(1, _filmRepository.MaxNO);
        }

        [TestMethod]
        public void When_FilmRepository_Count_is_8_Then_MaxNO_is_2_If_VisibleCount_is_4()
        {
            // Prepare
            _filmRepository.VisibleCount = 4;

            // Act
            for (int i = 0; i < 7; i++)
            {
                _filmRepository.Append();
            }

            // Assert
            Assert.AreEqual(1, _filmRepository.MaxNO);
        }

        [TestMethod]
        public void When_Append_A_Film_Then_Hide_Old_Film_And_Show_Last_Film()
        {
            // Prepare & Act
            _filmRepository.Append();

            // Assert
            Assert.IsFalse(_filmRepository[_filmRepository.Count-2].IsVisible);
            Assert.IsTrue(_filmRepository[_filmRepository.Count-1].IsVisible);
        }
    }
}
