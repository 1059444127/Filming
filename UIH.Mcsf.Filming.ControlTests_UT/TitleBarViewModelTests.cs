using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.ViewModel;

namespace UIH.Mcsf.Filming.ControlTests_UT
{
    [TestClass]
    public class TitleBarViewModelTests
    {
        private const int ConstNumber = 3;
        private TitleBarViewModel _titleBarViewModel;
        private Mock<ITitleSubject> _titleSubjectMock;

        [TestInitialize]
        public void SetUp()
        {
            _titleBarViewModel = new TitleBarViewModel();
            _titleSubjectMock = new Mock<ITitleSubject>();
        }

        [TestMethod]
        public void When_SetTitle_Then_PageNO_of_TitleBar_Is_Set()
        {
            // Arrange 
            const int expectedPageNO = ConstNumber;
            _titleSubjectMock.Setup(pm => pm.PageNO).Returns(expectedPageNO);

            // Act
            _titleBarViewModel.Title = _titleSubjectMock.Object;

            // Assert
            Assert.AreEqual(_titleBarViewModel.PageNO, expectedPageNO);
        }

        [TestMethod]
        public void When_Set_PageNO_of_Title_Then_PageNO_of_TitleBar_Is_Changed()
        {
            // Arrange
            const int expectedPageNO = ConstNumber;
            _titleSubjectMock.SetupProperty(pm => pm.PageNO);
            var titleSubject = _titleSubjectMock.Object;
            _titleBarViewModel.Title = titleSubject;

            // Act
            titleSubject.PageNO = expectedPageNO;
            _titleSubjectMock.Raise(pm => pm.PageNOChanged += null, new EventArgs());

            // Assert
            Assert.AreEqual(_titleBarViewModel.PageNO, expectedPageNO);
        }

        [TestMethod]
        public void When_SetTitle_Then_PageCount_of_TitleBar_Is_Set()
        {
            // Arrange 
            const int expectedPageCount = ConstNumber;
            _titleSubjectMock.Setup(pm => pm.PageCount).Returns(expectedPageCount);

            // Act
            _titleBarViewModel.Title = _titleSubjectMock.Object;

            // Assert
            Assert.AreEqual(_titleBarViewModel.PageCount, expectedPageCount);
        }

        [TestMethod]
        public void When_Set_PageCount_of_Title_Then_PageCount_of_TitleBar_Is_Changed()
        {
            // Arrange
            const int expectedPageCount = ConstNumber;
            _titleSubjectMock.SetupProperty(pm => pm.PageCount);
            var titleSubject = _titleSubjectMock.Object;
            _titleBarViewModel.Title = titleSubject;

            // Act
            titleSubject.PageCount = expectedPageCount;
            _titleSubjectMock.Raise(pm => pm.PageCountChanged += null, new EventArgs());

            // Assert
            Assert.AreEqual(_titleBarViewModel.PageCount, expectedPageCount);
        }
    }
}