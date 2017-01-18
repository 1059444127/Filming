namespace UIH.Mcsf.Filming.Abstracts
{
    public interface IClickStatus
    {
        bool IsLeftMouseButtonClicked { get; }
        bool IsRightMouseButtonClicked { get; }
        bool IsCtrlPressed { get; }
        bool IsShiftPressed { get; }
    }



}
