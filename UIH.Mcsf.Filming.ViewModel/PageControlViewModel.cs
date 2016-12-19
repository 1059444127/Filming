using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.ViewModel
{
    public class PageControlViewModel : ViewModelBase
    {
        public PageControlViewModel(PageModel pageModel)
        {
            //TODO-working-on: PageModel
            Layout = pageModel.Layout;
            ImageCells = pageModel.ImageCells;
        }

        #region [--Layout--]

        private Layout _layout;

        public Layout Layout
        {
            get { return _layout; }
            set
            {
                //TODO-later: Layout.Equal
                if (_layout == value) return;
                _layout = value;
                RaisePropertyChanged(() => Layout);
            }
        }

        #endregion [--Layout--]

        #region [--ImageCells--]

        private IList<ImageCell> _imageCells;

        public IList<ImageCell> ImageCells
        {
            get { return _imageCells; }
            set
            {
                if (_imageCells == value) return;
                _imageCells = value;
                RefreshTitle();
                RaisePropertyChanged(() => ImageCells);
            }
        }


        #endregion [--ImageCells--]

        #region [--PageNO--]

        private int _pageNO;

        public int PageNO
        {
            get { return _pageNO; }
            set
            {
                if (_pageNO == value) return;
                _pageNO = value;
                RaisePropertyChanged(() => PageNO);
            }
        }

        #endregion [--PageNO--]


        #region [--TitleBarVisibility--]

        private Visibility _titleBarVisibility = Visibility.Visible;

        public Visibility TitleBarVisibility
        {
            get { return _titleBarVisibility; }
            set
            {
                if (_titleBarVisibility == value) return;
                _titleBarVisibility = value;
                RaisePropertyChanged(() => TitleBarVisibility);
            }
        }

        #endregion [--TitleBarVisibility--]

        #region [--TitleBarPosition--]

        private Dock _titleBarPosition = Dock.Top;

        public Dock TitleBarPosition
        {
            get { return _titleBarPosition; }
            set
            {
                if (_titleBarPosition == value) return;
                _titleBarPosition = value;
                RaisePropertyChanged(() => TitleBarPosition);
            }
        }

        #endregion [--TitleBarPosition--]


        #region [--Patient Info--]

        #region [--PatientName--]

        private string _patientName;

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

        #region [--PatientID--]

        private string _patientID;

        public string PatientID
        {
            get { return _patientID; }
            set
            {
                if (_patientID == value) return;
                _patientID = value;
                RaisePropertyChanged(() => PatientID);
            }
        }

        #endregion [--PatientID--]

        #region [--PatientAge--]

        private string _patientAge;

        public string PatientAge
        {
            get { return _patientAge; }
            set
            {
                if (_patientAge == value) return;
                _patientAge = value;
                RaisePropertyChanged(() => PatientAge);
            }
        }

        #endregion [--PatientAge--]

        #region [--PatientSex--]

        private string _patientSex;

        public string PatientSex
        {
            get { return _patientSex; }
            set
            {
                if (_patientSex == value) return;
                _patientSex = value;
                RaisePropertyChanged(() => PatientSex);
            }
        }

        #endregion [--PatientSex--]


        #endregion [--Patient Info--]

        #region [--Study Info--]

        #region [--StudyInstanceUID--]

        private string _studyInstanceUID;

        public string StudyInstanceUID
        {
            get { return _studyInstanceUID; }
            set
            {
                if (_studyInstanceUID == value) return;
                _studyInstanceUID = value;
                RaisePropertyChanged(() => StudyInstanceUID);
            }
        }

        #endregion [--StudyInstanceUID--]

        #region [--StudyDate--]

        private string _studyDate;

        public string StudyDate
        {
            get { return _studyDate; }
            set
            {
                if (_studyDate == value) return;
                _studyDate = value;
                RaisePropertyChanged(() => StudyDate);
            }
        }

        #endregion [--StudyDate--]

        #region [--AccessionNumber--]

        private string _accessionNumber;

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set
            {
                if (_accessionNumber == value) return;
                _accessionNumber = value;
                RaisePropertyChanged(() => AccessionNumber);
            }
        }

        #endregion [--AccessionNumber--]


        #endregion [--Study Info--]

        //TODO: PageControlViewModel.RefreshTitle()
        private void RefreshTitle()
        {
            var sampleCell = ImageCells.FirstOrDefault();
            AccessionNumber = sampleCell.AccessionNumber;
        }
    }
}
