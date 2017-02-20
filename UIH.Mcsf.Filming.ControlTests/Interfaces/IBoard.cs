using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoard
    {
        int Count { get; }
        event EventHandler CountChanged;
        object this[int i] { get; }
    }
}
