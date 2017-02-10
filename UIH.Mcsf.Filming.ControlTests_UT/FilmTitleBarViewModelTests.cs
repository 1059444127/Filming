using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.ViewModel;

namespace UIH.Mcsf.Filming.ControlTests_UT
{
    [TestClass]
    public class FilmTitleBarViewModelTests
    {
        private const string ConstString = "ConstString";
        private FilmTitleBarViewModel _filmTitleBar;
        private Mock<IFilmTitleSubject> _titleMock;
        
        [TestInitialize]
        public void SetUp()
        {
            _filmTitleBar = new FilmTitleBarViewModel();
            _titleMock = new Mock<IFilmTitleSubject>();
        }

        [TestMethod]
        public void When_SetTitle_Then_PatientName_of_FilmTitle_Is_Set()
        {
            // Arrange
            const string expectedPatientName = ConstString;
            _titleMock.Setup(tm => tm.PatientName).Returns(expectedPatientName);

            // Act
            _filmTitleBar.FilmTitle = _titleMock.Object;

            // Assert
            Assert.AreEqual(expectedPatientName, _filmTitleBar.PatientName);
        }

        [TestMethod]
        public void When_Set_PatientName_of_Title_Then_PatientName_of_FilmTilte_Is_Changed()
        {
            // Arrange
            const string expectedPatientName = ConstString;
            _titleMock.SetupProperty(tm => tm.PatientName);
            var _title = _titleMock.Object;
            _filmTitleBar.FilmTitle = _title;

            // Act
            _title.PatientName = expectedPatientName;
            _titleMock.Raise(tm=>tm.PatientNameChanged += null, new EventArgs());

            // Assert
            Assert.AreEqual(expectedPatientName, _filmTitleBar.PatientName);
        }
    }
}
