using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IVariableCollection
    {
        int Count { get; set; }
        event EventHandler CountChanged;
    }
}