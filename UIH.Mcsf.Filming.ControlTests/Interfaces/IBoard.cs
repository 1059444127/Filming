namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoard : IVariableCollection
    {
        object this[int i] { get; }
    }
}
