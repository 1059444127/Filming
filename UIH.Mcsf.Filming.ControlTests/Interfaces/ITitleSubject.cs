using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface ITitleSubject : ITitle
    {
        event EventHandler NOChanged;
        event EventHandler CountChanged;
    }
}