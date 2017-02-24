using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IVisibleCountSubject
    {
        int VisibleCount { get; }
        event EventHandler CountChanged;
    }
}