namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoardContent : IVariableCollection
    {
        IFilm this[int i] { get; }
    }
}