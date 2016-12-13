using System;
using System.Windows.Input;

namespace UIH.Mcsf.Filming.Interfaces
{
    public interface IClickStatus
    {
        bool IsLeftMouseButtonClicked { get; }
        bool IsRightMouseButtonClicked { get; }
        bool IsCtrlPressed { get; }
        bool IsShiftPressed { get; }
    }

    public class ClickStatus : IClickStatus
    {
        public ClickStatus(bool isLeftMouseButtonClicked, bool isRightMouseButtonClicked)
        {
            IsLeftMouseButtonClicked = isLeftMouseButtonClicked;
            IsRightMouseButtonClicked = isRightMouseButtonClicked;
            IsCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            IsShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        public bool IsLeftMouseButtonClicked { get; private set; }
        public bool IsRightMouseButtonClicked { get; private set; }
        public bool IsCtrlPressed { get; private set; }
        public bool IsShiftPressed { get; private set; }
    }

    public class ClickStatusEventArgs : EventArgs
    {
        public ClickStatusEventArgs(IClickStatus clickStatus)
        {
            ClickStatus = clickStatus;
        }

        public IClickStatus ClickStatus { get; private set; }
    }
}
