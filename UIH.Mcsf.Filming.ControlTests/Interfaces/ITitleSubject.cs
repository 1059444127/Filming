using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface ITitleSubject : ITitle
    {
        event EventHandler PageNOChanged;
        event EventHandler PageCountChanged;
    }
}