namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoard : ILattice
    {
        object this[int i] { get; }
    }
}
