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

        private IFilmTitleSubject _filmTitle;

        public IFilmTitleSubject FilmTitle
        {
            set
            {
                if(_filmTitle == value) return;
                UnRegisterFilmTitleEvent();
                _filmTitle = value;
                RefreshProperties();
                RegisterFilmTitleEvent();
            }
        }

        private void RefreshProperties()
        {
            if(_filmTitle == null) return;

            Title = _filmTitle;
            PatientName = _filmTitle.PatientName;
        }

        private void RegisterFilmTitleEvent()
        {
            if(_filmTitle == null) return;
            _filmTitle.PatientNameChanged += FilmTitleOnPatientNameChanged;
        }

        private void UnRegisterFilmTitleEvent()
        {
            if(_filmTitle == null) return;
            _filmTitle.PatientNameChanged -= FilmTitleOnPatientNameChanged;
        }

        private void FilmTitleOnPatientNameChanged(object sender, EventArgs eventArgs)
        {
            PatientName = _filmTitle.PatientName;
        }

        #endregion
    }
}