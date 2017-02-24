using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoard : IVisibleCountSubject
    {
        object this[int i] { get; }
    }
}
