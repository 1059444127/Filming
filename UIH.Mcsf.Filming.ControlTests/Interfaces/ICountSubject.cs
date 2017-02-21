using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface ICountSubject
    {
        int Count { get; }
        event EventHandler CountChanged;
    }
}