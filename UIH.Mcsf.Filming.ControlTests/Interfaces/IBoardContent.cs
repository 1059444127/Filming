namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoardContent
    {
        IFilm this[int i] { get; }
        void AppendContent();
    }
}