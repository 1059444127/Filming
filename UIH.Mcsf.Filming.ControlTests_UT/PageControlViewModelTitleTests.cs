using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.ViewModel;

namespace UIH.Mcsf.Filming.ControlTests_UT
{
    [TestClass]
    public class PageControlViewModelTitleTests
    {
        private PageControlViewModel _pageControlViewModel;
        private Mock<IPage> _pageMock;
        private TitleBarViewModel _title;

        private const int ConstNumber = 3; 

        [TestInitialize]
        public void SetUp()
        {
            _pageControlViewModel = new PageControlViewModel();
            _pageMock = new Mock<IPage>();
            _title = new TitleBarViewModel();
            _pageControlViewModel.TitleBarViewModel = _title;

        }
        [TestMethod]
        public void When_SetPage_Then_PageNO_of_PageControl_Is_Set()
        {
            // Arrange 
            const int expectedPageNO = ConstNumber;
            _pageMock.Setup(pm => pm.PageNO).Returns(expectedPageNO);

            // Act
            _pageControlViewModel.Page = _pageMock.Object;

            // Assert
            Assert.AreEqual(_title.PageNO, expectedPageNO);
        }

        [TestMethod]
        public void When_Set_PageNO_of_Page_Then_PageNO_of_PageControl_Is_Changed()
        {
            // Arrange
            const int expectedPageNO = ConstNumber;
            _pageMock.SetupProperty(pm => pm.PageNO);
            var page = _pageMock.Object;
            _pageControlViewModel.Page = page;

            // Act
            page.PageNO = expectedPageNO;
            _pageMock.Raise(pm => pm.PageNOChanged += null, new EventArgs());

            // Assert
            Assert.AreEqual(_title.PageNO, expectedPageNO);
        }

        [TestMethod]
        public void When_SetPage_Then_PageCount_of_PageControl_Is_Set()
        {
            // Arrange 
            const int expectedPageCount = ConstNumber;
            _pageMock.Setup(pm => pm.PageCount).Returns(expectedPageCount);

            // Act
            _pageControlViewModel.Page = _pageMock.Object;

            // Assert
            Assert.AreEqual(_title.PageCount, expectedPageCount);
        }

        [TestMethod]
        public void When_Set_PageCount_of_Page_Then_PageCount_of_PageControl_Is_Changed()
        {
            // Arrange
            const int expectedPageCount = ConstNumber;
            _pageMock.SetupProperty(pm => pm.PageCount);
            var page = _pageMock.Object;
            _pageControlViewModel.Page = page;

            // Act
            page.PageCount = expectedPageCount;
            _pageMock.Raise(pm => pm.PageCountChanged += null, new EventArgs());

            // Assert
            Assert.AreEqual(_title.PageCount, expectedPageCount);
        }


    }
}
