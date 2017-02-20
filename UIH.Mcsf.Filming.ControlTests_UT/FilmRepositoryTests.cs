using Microsoft.VisualStudio.TestTools.UnitTesting;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.Models;

namespace UIH.Mcsf.Filming.ControlTests_UT
{
    [TestClass]
    public class FilmRepositoryTests
    {
        private IFilmRepository _filmRepository;

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
    }
}
