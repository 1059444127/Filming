using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    public class FilmTitleBarViewModel : TitleBarViewModel, IFilmMetaData
    {
        #region Implementation of IFilmMetaData

        #region [--PatientName--]

        private string _patientName = "Nobody";

        public string PatientName
        {
            get { return _patientName; }
            set
            {
                if (_patientName == value) return;
                _patientName = value;
                RaisePropertyChanged(() => PatientName);
            }
        }

        #endregion [--PatientName--]

        #endregion

        #region [--FilmTitle--]

        private IFilmTitleSubject _filmTitle = new NullFilmTitleSubject();

        public IFilmTitleSubject FilmTitle
        {
            set
            {
                if(_filmTitle == null) return;
                UnRegisterFilmTitleEvent();
                _filmTitle = value;
                RefreshProperties();
                RegisterFilmTitleEvent();
            }
        }

        private void RefreshProperties()
        {
            Title = _filmTitle;
            PatientName = _filmTitle.PatientName;
        }

        private void RegisterFilmTitleEvent()
        {
            _filmTitle.PatientNameChanged += FilmTitleOnPatientNameChanged;
        }

        private void UnRegisterFilmTitleEvent()
        {
            _filmTitle.PatientNameChanged -= FilmTitleOnPatientNameChanged;
        }

        private void FilmTitleOnPatientNameChanged(object sender, EventArgs eventArgs)
        {
            PatientName = _filmTitle.PatientName;
        }

        #endregion
    }
}