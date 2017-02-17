namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoardContent : IAppend
    {
        IFilm this[int i] { get; }
    }
}