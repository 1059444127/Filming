using System;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.ViewModel;

namespace UIH.Mcsf.Filming.ControlTests_UT
{
    [TestClass]
    public class PageControlViewModelTests
    {
        private PageControlViewModel _pageControlViewModel;
        private Mock<IPage> _pageMock;

        [TestInitialize]
        public void SetUp()
        {
            _pageControlViewModel = new PageControlViewModel();
            _pageMock = new Mock<IPage>();
        }

        [TestMethod]
        public void When_SetPage_with_A_Page_IsVisible_Then_PageControlViewModel_IsVisible()
        {
            // Arrange
            _pageMock.Setup(pm => pm.IsVisible).Returns(true);

            // Act
            _pageControlViewModel.Page = _pageMock.Object;

            // Assert
           Assert.IsTrue(_pageControlViewModel.IsVisible);
        }

        [TestMethod]
        public void When_Set_Page_IsVisibile_Then_PageControlViewModel_IsVisible()
        {
            // Arrange
            _pageMock.SetupProperty(mp => mp.IsVisible);
            var page = _pageMock.Object;
            _pageControlViewModel.Page = page;

            // Act
            page.IsVisible = true;
            _pageMock.Raise(mp => mp.VisibleChanged += null, new EventArgs());

            // Assert
            Assert.IsTrue(_pageControlViewModel.IsVisible);
        }

    }
}