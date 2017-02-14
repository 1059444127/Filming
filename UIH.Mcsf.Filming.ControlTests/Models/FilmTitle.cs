using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class FilmTitle : IFilmTitleSubject
    {
        #region Implementation of IFilmMetaData

        #region [--PatientName--]

        private string _patientName;
        public string PatientName
        {
            get { return _patientName; }
            set
            {
                if(_patientName == value) return;
                _patientName = value;
                PatientNameChanged(this, new EventArgs());
            }
        }

        #endregion

        #endregion

        #region Implementation of ITitle

        #region [--NO--]

        private int _no;

        public int NO
        {
            get { return _no; }
            set
            {
                if (_no == value+1) return;
                _no = value+1;
                NOChanged(this, new EventArgs());
            }
        }

        #endregion

        #region [--Count--]

        private int _count;
        public int Count
        {
            get { return _count; }
            set
            {
                if (_count == value) return;
                _count = value;
                CountChanged(this, new EventArgs());
            }
        }

        #endregion

        #endregion

        #region Implementation of ITitleSubject

        public event EventHandler NOChanged = delegate { };
        public event EventHandler CountChanged = delegate { };

        #endregion

        #region Implementation of IFilmTitleSubject

        public event EventHandler PatientNameChanged = delegate { };

        #endregion
    }
}