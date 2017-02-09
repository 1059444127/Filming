using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IFilmTitleSubject : IFilmMetaData, ITitleSubject
    {
        event EventHandler PatientNameChanged;
    }
}