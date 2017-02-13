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
    }
}