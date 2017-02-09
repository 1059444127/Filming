using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public class NullFilmTitleSubject : IFilmTitleSubject
    {
        #region Implementation of IFilmMetaData

        public string PatientName { get; set; }

        #endregion

        #region Implementation of ITitle

        public int NO { get; set; }

        public int Count { get; set; }

        #endregion

        #region Implementation of ITitleSubject

        public event EventHandler NOChanged;
        public event EventHandler CountChanged;

        #endregion

        #region Implementation of IFilmTitleSubject

        public event EventHandler PatientNameChanged;

        #endregion
    }
}