using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoard : ICountSubject
    {
        object this[int i] { get; }
    }
}
