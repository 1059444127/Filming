using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Filming.ViewModel;

namespace UIH.Mcsf.Filming.ViewModel_UT
{
    [TestClass]
    public class PageControlViewModelTests
    {
        private const string ExpectedString = "ExpectedString";
        private readonly PageControlViewModel _pageControlViewModel = new PageControlViewModel();
        private Mock<ImageCell> _imageCell1Mock;
        private Mock<ImageCell> _imageCell2Mock;
        private Mock<ImageCell> _imageCell3Mock;
        private Mock<ImageCell> _imageCell4Mock;

        [TestInitialize]
        public void Setup()
        {
            _imageCell1Mock = new Mock<ImageCell>();
            _imageCell2Mock = new Mock<ImageCell>();
            _imageCell3Mock = new Mock<ImageCell>();
            _imageCell4Mock = new Mock<ImageCell>();
        }

            // TODO-User-Intent: When StudyInstanceUID is mixed Then AccessionNumber is StarString
        // TODO-User-Intent: When Cells have mixed PatientID Then PatientID is StarString
        // TODO-User-Intent: When Cells except Empty Cells have same PatientID Then PatientID is same with sampleCell
        // TODO-User-Intent: When Cells have mixed PatientName Then PatientName is Mixed
        // TODO-User-Intent: When Cells except Empty Cells have same PatientName Then PatientName is same with sampleCell
        // TODO-User-Intent: When Cells have mixed PatientAge Then PatientAge is StarString
        // TODO-User-Intent: When Cells except Empty Cells have same PatientAge Then PatientAge is same with sampleCell
        // TODO-User-Intent: When Cells have mixed PatientSex Then PatientSex is StarString
        // TODO-User-Intent: When Cells except Empty Cells have same PatientSex Then PatientSex is same with sampleCell
        // TODO-User-Intent: When Cells have mixed StudyInstanceUID Then StudyInstanceUID is StarString
        // TODO-User-Intent: When Cells except Empty Cells have same StudyInstanceUID Then StudyInstanceUID is same with sampleCell
        // TODO-User-Intent: When Cells have mixed StudyDate Then StudyDate is StarString
        // TODO-User-Intent: When Cells except Empty Cells have same StudyDate Then StudyDate is same with sampleCell
        // TODO-User-Intent: When Cells have mixed AccessionNumber Then AccessionNumber is StarString
        // TODO-User-Intent-working-on: When Cells except Empty Cells have same AccessionNumber Then AccessionNumber is same with sampleCell
        [TestMethod]
        public void When_Cells_except_Empty_Cells_have_same_AccessionNumber_Then_AccessionNumber_is_same_with_sampleCell
            ()
        {
            // Arrange
            _imageCell1Mock.Setup(ic => ic.AccessionNumber).Returns(ExpectedString);
            _imageCell2Mock.Setup(ic => ic.AccessionNumber).Returns((string)(null));
            _imageCell3Mock.Setup(ic => ic.AccessionNumber).Returns(string.Empty);
            _imageCell4Mock.Setup(ic => ic.AccessionNumber).Returns(ExpectedString);
            var cells = new[]
            {
                _imageCell1Mock.Object, _imageCell2Mock.Object, _imageCell3Mock.Object, _imageCell4Mock.Object
            };

            // Act
            _pageControlViewModel.ImageCells = cells;
            var actual = _pageControlViewModel.AccessionNumber;

            // Assert
            Assert.AreEqual(ExpectedString, actual);
        }
    }
}