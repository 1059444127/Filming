using System.Windows.Input;
using UIH.Mcsf.Filming.Abstracts;

namespace UIH.Mcsf.Filming.Utilities
{
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
}