using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Filming.Model;

namespace UIH.Mcsf.Filming.Model_UT
{
    [TestClass]
    public class SelectableListTest
    {
        #region [--Fields--]

        private const int ListSize = 8;
        private readonly Random _random = new Random();
        private readonly SelectableList<SelectableStub> _selectableList = new SelectableList<SelectableStub>();
        private readonly Mock<IClickStatus> _clickStatusMock = new Mock<IClickStatus>();
        private int _originalSelectStatus;
        private int _originalFocusStatus;
        private int _clickIndex;
        private SelectableStub _clickElement;

        #endregion

        [TestInitialize]
        public void Setup()
        {
            CreateARandomSelectableList();
            GetSelectableListStatus();
            SetClickElement();
        }

        [TestMethod]
        public void When_Left_click_on_an_element_Then_focus_on_it_And_only_select_it() // Method_Scenario_Result
        {
            // Arrange
            var expectedSelectStatus = 1 << _clickIndex;
            var expectedFocusStatus = 1 << _clickIndex;
            var clickStatus = Click(left:true, right:null, ctrl:false, shift:false);

            // Act
            _clickElement.Click(clickStatus);

            // Assert
            var actualSelectStatus = GetSelectStatus();
            var actualFocusStatus = GetFocusStatus();

            Console.WriteLine("[Before Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}[Clicked Index]{2}", _originalSelectStatus, _originalFocusStatus, _clickIndex);
            Console.WriteLine("[After Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}", actualSelectStatus, actualFocusStatus);
            Assert.AreEqual(expectedSelectStatus, actualSelectStatus);
            Assert.AreEqual(expectedFocusStatus, actualFocusStatus);
        }

        [TestMethod]
        public void When_Left_click_on_an_element_With_Ctrl_pressed_Then_focus_on_it_And_change_its_select_state() // Method_Scenario_Result
        {
            // Arrange
            var expectedSelectStatus = _originalSelectStatus ^ (1<<_clickIndex);
            var expectedFocusStatus = 1 << _clickIndex;
            var clickStatus = Click(left:true, right:null, ctrl:true, shift:false);

            // Act
            _clickElement.Click(clickStatus);

            // Assert
            var actualSelectStatus = GetSelectStatus();
            var actualFocusStatus = GetFocusStatus();

            Console.WriteLine("[Before Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}[Clicked Index]{2}", _originalSelectStatus, _originalFocusStatus, _clickIndex);
            Console.WriteLine("[After Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}", actualSelectStatus, actualFocusStatus);
            Assert.AreEqual(expectedSelectStatus, actualSelectStatus);
            Assert.AreEqual(expectedFocusStatus, actualFocusStatus);
        }

        [TestMethod]
        public void When_Left_click_on_an_element_With_Shift_pressed_Then_Focus_not_changed_And_Select_only_from_focus_to_operation_element() // Method_Scenario_Result
        {
            // Arrange
            var expectedSelectStatus = AllOneFromFocusTo(_clickIndex);
            var expectedFocusStatus = _originalFocusStatus;
            var clickStatus = Click(left:true, right:null, ctrl:false, shift:true);

            // Act
            _clickElement.Click(clickStatus);

            // Assert
            var actualSelectStatus = GetSelectStatus();
            var actualFocusStatus = GetFocusStatus();

            Console.WriteLine("[Before Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}[Clicked Index]{2}", _originalSelectStatus, _originalFocusStatus, _clickIndex);
            Console.WriteLine("[After Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}", actualSelectStatus, actualFocusStatus);
            Assert.AreEqual(expectedSelectStatus, actualSelectStatus);
            Assert.AreEqual(expectedFocusStatus, actualFocusStatus);
        }

        [TestMethod]
        public void When_Left_Click_on_an_element_With_CtrlShift_pressed_Then_Focus_not_changed_And_Select_or_UnSelect_from_focus_to_operation_element_Based_on_its_select_status() // Method_Scenario_Result
        {
            // Arrange
            var mask = AllOneFromFocusTo(_clickIndex);
            var expectedSelectStatus = (_originalFocusStatus & _originalSelectStatus)==0 
                ? _originalSelectStatus & (~mask)
                : _originalSelectStatus | mask;
            var expectedFocusStatus = _originalFocusStatus;
            var clickStatus = Click(left:true, right:null, ctrl:true, shift:true);

            // Act
            _clickElement.Click(clickStatus);

            // Assert
            var actualSelectStatus = GetSelectStatus();
            var actualFocusStatus = GetFocusStatus();

            Console.WriteLine("[Before Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}[Clicked Index]{2}", _originalSelectStatus, _originalFocusStatus, _clickIndex);
            Console.WriteLine("[After Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}", actualSelectStatus, actualFocusStatus);
            Assert.AreEqual(expectedSelectStatus, actualSelectStatus);
            Assert.AreEqual(expectedFocusStatus, actualFocusStatus);
        }

        [TestMethod]
        public void When_Right_Click_on_an_element_Then_Select_it_But_Not_change_Focus() // Method_Scenario_Result
        {
            // Arrange
            var expectedSelectStatus = _clickElement.IsSelected ? _originalSelectStatus : 1 << _clickIndex;
            var expectedFocusStatus = _clickElement.IsSelected ? _originalFocusStatus : 1 << _clickIndex;
            var clickStatus = Click(left:false, right:true, ctrl:false, shift:false);

            // Act
            _clickElement.Click(clickStatus);

            // Assert
            var actualSelectStatus = GetSelectStatus();
            var actualFocusStatus = GetFocusStatus();

            Console.WriteLine("[Before Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}[Clicked Index]{2}", _originalSelectStatus, _originalFocusStatus, _clickIndex);
            Console.WriteLine("[After Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}", actualSelectStatus, actualFocusStatus);
            Assert.AreEqual(expectedSelectStatus, actualSelectStatus);
            Assert.AreEqual(expectedFocusStatus, actualFocusStatus);
        }

        [TestMethod]
        public void When_Right_Click_on_an_element_With_Only_Ctrl_pressed_Then_nothing_changed() // Method_Scenario_Result
        {
            // Arrange
            var expectedSelectStatus = _originalSelectStatus;
            var expectedFocusStatus = _originalFocusStatus;
            var clickStatus = Click(left:false, right:true, ctrl:true, shift:false);

            // Act
            _clickElement.Click(clickStatus);

            // Assert
            var actualSelectStatus = GetSelectStatus();
            var actualFocusStatus = GetFocusStatus();

            Console.WriteLine("[Before Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}[Clicked Index]{2}", _originalSelectStatus, _originalFocusStatus, _clickIndex);
            Console.WriteLine("[After Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}", actualSelectStatus, actualFocusStatus);
            Assert.AreEqual(expectedSelectStatus, actualSelectStatus);
            Assert.AreEqual(expectedFocusStatus, actualFocusStatus);
        }

        [TestMethod]
        public void When_Right_Click_on_an_element_With_Shift_pressed_Then_Select_it_But_Not_change_Focus() // Method_Scenario_Result
        {
            // Arrange
            var expectedSelectStatus = _clickElement.IsSelected ? _originalSelectStatus : 1 << _clickIndex;
            var expectedFocusStatus = _clickElement.IsSelected ? _originalFocusStatus : 1 << _clickIndex;
            var clickStatus = Click(left:false, right:true, ctrl:false, shift:true);

            // Act
            _clickElement.Click(clickStatus);

            // Assert
            var actualSelectStatus = GetSelectStatus();
            var actualFocusStatus = GetFocusStatus();

            Console.WriteLine("[Before Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}[Clicked Index]{2}", _originalSelectStatus, _originalFocusStatus, _clickIndex);
            Console.WriteLine("[After Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}", actualSelectStatus, actualFocusStatus);
            Assert.AreEqual(expectedSelectStatus, actualSelectStatus);
            Assert.AreEqual(expectedFocusStatus, actualFocusStatus);
        }

        [TestMethod]
        public void When_Right_Click_on_an_element_With_CtrlShift_Pressed_Then_Select_it_But_Not_change_Focus() // Method_Scenario_Result
        {
            // Arrange
            var expectedSelectStatus = _clickElement.IsSelected ? _originalSelectStatus : 1 << _clickIndex;
            var expectedFocusStatus = _clickElement.IsSelected ? _originalFocusStatus : 1 << _clickIndex;
            var clickStatus = Click(left:false, right:true, ctrl:true, shift:true);

            // Act
            _clickElement.Click(clickStatus);

            // Assert
            var actualSelectStatus = GetSelectStatus();
            var actualFocusStatus = GetFocusStatus();

            Console.WriteLine("[Before Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}[Clicked Index]{2}", _originalSelectStatus, _originalFocusStatus, _clickIndex);
            Console.WriteLine("[After Clicked][SelectStatus]{0:x2}[FocusStatus]{1:x2}", actualSelectStatus, actualFocusStatus);
            Assert.AreEqual(expectedSelectStatus, actualSelectStatus);
            Assert.AreEqual(expectedFocusStatus, actualFocusStatus);
        }

        #region [--Private Methods--]

        private int AllOneFromFocusTo(int clickIndex)
        {
            var focus = _selectableList.FirstOrDefault(e => e.IsFocused);
            if (focus == null) return 0;
            var focusIndex = _selectableList.IndexOf(focus);

            var first = Math.Min(clickIndex, focusIndex);
            var last = Math.Max(clickIndex, focusIndex);

            var result = 0;
            for (int i = first; i <= last; i++)
            {
                result ^= 1 << i;
            }
            return result;
        }

        private void SetClickElement()
        {
            _clickIndex = RandomClickIndex();
            _clickElement = _selectableList[_clickIndex];
        }

        private void GetSelectableListStatus()
        {
            _originalSelectStatus = GetSelectStatus();
            _originalFocusStatus = GetFocusStatus();
        }

        private void CreateARandomSelectableList()
        {
            _selectableList.Clear();
            for (var i = 0; i < ListSize; i++)
            {
                _selectableList.Add(new SelectableStub {IsSelected = _random.Next(2) == 1});
            }
            var focusIndex = _random.Next(ListSize + 1) - 1;
            if (focusIndex >= 0) _selectableList[focusIndex].IsFocused = true;
        }

        private IClickStatus Click(bool left, bool? right, bool ctrl, bool shift)
        {
            _clickStatusMock.Setup(cs => cs.IsLeftMouseButtonClicked).Returns(left);
            _clickStatusMock.Setup(cs => cs.IsRightMouseButtonClicked).Returns(right ?? _random.Next(1) == 0);
            _clickStatusMock.Setup(cs => cs.IsCtrlPressed).Returns(ctrl);
            _clickStatusMock.Setup(cs => cs.IsShiftPressed).Returns(shift);

            return _clickStatusMock.Object;
        }

        private int RandomClickIndex()
        {
            return _random.Next(ListSize);
        }

        private int GetSelectStatus()
        {
            var result = 0;
            for (var i = _selectableList.Count - 1; i >= 0; i--)
            {
                var e = _selectableList[i];
                result <<= 1;
                result ^= e.IsSelected ? 1 : 0;
            }
            return result;
        }

        private int GetFocusStatus()
        {
            var result = 0;
            for (var i = _selectableList.Count - 1; i >= 0; i--)
            {
                var e = _selectableList[i];
                result <<= 1;
                result ^= e.IsFocused ? 1 : 0;
            }
            return result;
        }

        #endregion
    }

    internal class SelectableStub : ISelect
    {
        public void Click(IClickStatus clickStatus)
        {
            Clicked(this, new ClickStatusEventArgs(clickStatus));
        }

        #region Implementation of ISelect

        public bool IsSelected { get; set; }

        public bool IsFocused { get; set; }

        public event EventHandler<ClickStatusEventArgs> Clicked = delegate { };

        #endregion
    }
}