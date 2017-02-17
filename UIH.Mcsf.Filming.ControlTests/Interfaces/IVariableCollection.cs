using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IVariableCollection : IAppend
    {
        int Count { get; set; }
        event EventHandler CountChanged;
    }
}