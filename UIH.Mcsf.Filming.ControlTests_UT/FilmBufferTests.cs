using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.Models;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests_UT
{
    [TestClass]
    public class FilmBufferTests
    {
        private FilmBuffer _filmBuffer;
        private Mock<IFilmRepository> _filmRepositoryMock;

        [TestInitialize]
        public void SetUp()
        {
            _filmRepositoryMock = new Mock<IFilmRepository>();
            _filmBuffer = new FilmBuffer(_filmRepositoryMock.Object);
        }

        [TestMethod]
        public void When_FilmRepository_Count_is_0_Then_FilmBuffer_MaxNO_is_0()
        {
            // Prepare

            // Act
            _filmRepositoryMock.Setup(frm => frm.Count).Returns(0);
            _filmRepositoryMock.Raise(frm=>frm.CountChanged += null, new EventArgs());

            // Assert
            Assert.AreEqual(0, _filmBuffer.MaxNO);
        }

        [TestMethod]
        public void When_FilmRepository_Count_is_4_Then_FilmBuffer_MaxNO_is_0_If_Board_Count_is_4()
        {
            // Prepare
            _filmBuffer.VisibleSize = 4;

            // Act
            _filmRepositoryMock.Setup(frm => frm.Count).Returns(4);
            _filmRepositoryMock.Raise(frm=>frm.CountChanged += null, new EventArgs());

            // Assert
            Assert.AreEqual(0, _filmBuffer.MaxNO);
        }

        [TestMethod]
        public void When_FilmRepository_Count_is_5_Then_FilmBuffer_MaxNO_is_1_If_Board_Count_is_4()
        {
            // Prepare
            _filmBuffer.VisibleSize = 4;

            // Act
            _filmRepositoryMock.Setup(frm => frm.Count).Returns(5);
            _filmRepositoryMock.Raise(frm=>frm.CountChanged += null, new EventArgs());

            // Assert
            Assert.AreEqual(1, _filmBuffer.MaxNO);
        }

        [TestMethod]
        public void When_FilmRepository_Count_is_8_Then_FilmBuffer_MaxNO_is_2_If_Board_Count_is_4()
        {
            // Prepare
            _filmBuffer.VisibleSize = 4;

            // Act
            _filmRepositoryMock.Setup(frm => frm.Count).Returns(8);
            _filmRepositoryMock.Raise(frm => frm.CountChanged += null, new EventArgs());

            // Assert
            Assert.AreEqual(1, _filmBuffer.MaxNO);
        }

        //[TestMethod]
        //TODO-User-Intent: public void Only_First_Film_In_Board_Is_Visible()
        //{

        //    // Assert
        //    Assert.IsTrue(_filmBuffer[0].IsVisible, "First Film in Board is InVisible");
        //    for (int i = 1; i < GlobalDefinitions.MaxDisplayMode; i++)
        //    {
        //        Assert.IsFalse(_filmBuffer[i].IsVisible, string.Format("Film {0} in Board is Visible", i));
        //    }
        //}

        //[TestMethod]
        //TODO-User-Intent: public void New_Film_In_Board_Is_Visible()
        //{
        //    // Arrange
        //    _filmBuffer.Append();

        //    // Assert
        //    var filmRepository = _filmRepositoryMock.Object;
        //    Assert.IsTrue(filmRepository.);
        //}
    }
}
