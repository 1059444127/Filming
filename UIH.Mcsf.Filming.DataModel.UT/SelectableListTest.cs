using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UIH.Mcsf.Filming.DataModel.UT
{
    [TestClass]
    public class SelectableListTest
    {
        private const int ElementCount = 10;
        private readonly SelectableList<SelectableStub> _selectableList;

        public SelectableListTest()
        {
            var stubList = new List<SelectableStub>();
            for (var i = 0; i < ElementCount; i++)
            {
                stubList.Add(new SelectableStub());
            }
            _selectableList = new SelectableList<SelectableStub>(stubList);
        }

        [TestInitialize]
        public void SetUp()
        {
        }

        [TestMethod]
        public void When_left_clicked_Then_only_an_Element_is_selected_and_focused()
        {
            //Arrange
            var clickStatus = new ClickStatusStub {IsLeftMouseButtonClicked = true};

            //Act
            var element = _selectableList.ElementAt(0);
            element.Click(clickStatus);

            //Assert
            Assert.IsTrue(element.IsSelected);
            Assert.IsTrue(element.IsFocused);
        }

        [TestMethod]
        public void When_right_clicked_Then_nothing_changed()
        {
            //Arrange
            var clickStatus = new ClickStatusStub {IsRightMouseButtonClicked = true};

            //Act
            var element = _selectableList.ElementAt(0);
            const bool expectedSelectStatus = true;
            var expectedFocusStatus = !element.IsSelected || element.IsFocused;
            element.Click(clickStatus);

            //Assert
            Assert.AreEqual(expectedSelectStatus, element.IsSelected);
            Assert.AreEqual(expectedFocusStatus, element.IsFocused);
        }
    }

    internal class ClickStatusStub : IClickStatus
    {
        #region Implementation of IClickStatus

        public bool IsLeftMouseButtonClicked { get; set; }

        public bool IsRightMouseButtonClicked { get; set; }

        public bool IsCtrlPressed { get; set; }

        public bool IsShiftPressed { get; set; }

        #endregion
    }

    internal class SelectableStub : ISelect
    {
        public bool IsSelected { get; set; }
        public bool IsFocused { get; set; }
        public event EventHandler<ClickStatusEventArgs> Clicked = delegate { };

        public void Click(IClickStatus clickStatus)
        {
            Clicked(this, new ClickStatusEventArgs(clickStatus));
        }
    }
}