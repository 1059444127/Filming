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
    }
}