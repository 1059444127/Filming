using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface ILattice : IAppend
    {
        int Count { get; set; }
        event EventHandler CountChanged;
    }
}