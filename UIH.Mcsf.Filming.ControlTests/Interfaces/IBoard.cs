using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoard
    {
        int Count { get; set; }
        event EventHandler CountChanged;

        void Append();
        object this[int i] { get; }
    }
}
