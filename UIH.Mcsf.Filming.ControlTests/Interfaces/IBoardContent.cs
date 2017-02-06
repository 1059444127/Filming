namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoardContent
    {
        IPage this[int i] { get; }
        void AppendContent();
    }
}