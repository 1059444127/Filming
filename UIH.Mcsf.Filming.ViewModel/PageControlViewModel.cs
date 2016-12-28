using System;
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
        private PageModel _pageModel = PageModel.CreatePageModel();
        //TODO: PageControlViewModel.RegisterEvent From BoardCell
        //TODO: Binding Page Changed
        //TODO: Binding Page.Layout Changed
        //TODO: Binding Page.ImageCells Changed
        public PageControlViewModel(BoardCell boardCell)
        {
            var pageModel = boardCell.PageModel;
            if (_pageModel == pageModel) return;

            UnRegisterPageModelEvent();
            _pageModel = pageModel;
            RegisterPageModelEvent();

            RefreshPage();
        }

        private void RegisterPageModelEvent()
        {
            UnRegisterPageModelEvent();
            _pageModel.VisibleChanged += PageModelOnVisibleChanged;
            _pageModel.IsBreakChanged += PageModelOnIsBreakChanged;
            
        }

        private void UnRegisterPageModelEvent()
        {
            _pageModel.IsBreakChanged -= PageModelOnIsBreakChanged;
            _pageModel.VisibleChanged -= PageModelOnVisibleChanged;
        }

        private void RefreshPage()
        {
            Layout = _pageModel.Layout;
            ImageCells = _pageModel.ImageCells;
            _visibility = BoolToVisibility(_pageModel.IsVisible);
            _breakVisibility = BoolToVisibility(_pageModel.IsBreak);
        }

        private void PageModelOnIsBreakChanged(object sender, BoolEventArgs boolEventArgs)
        {
            BreakVisibility = BoolToVisibility(boolEventArgs.Bool);
        }

        private Visibility BoolToVisibility(bool b)
        {
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PageModelOnVisibleChanged(object sender, BoolEventArgs boolEventArgs)
        {
            Visibility = BoolToVisibility(boolEventArgs.Bool);
        }

        #region [--BreakVisibility--]

        private Visibility _breakVisibility;

        public Visibility BreakVisibility
        {
            get { return _breakVisibility; }
            set
            {
                if (_breakVisibility == value) return;
                _breakVisibility = value;
                RaisePropertyChanged(() => BreakVisibility);
            }
        }

        #endregion [--BreakVisibility--]


        #region [--Visibility--]

        private Visibility _visibility;

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility == value) return;
                _visibility = value;
                RaisePropertyChanged(() => Visibility);
            }
        }

        #endregion [--Visibility--]


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

        #region [--PageCount--]

        private int _pageCount;

        public int PageCount
        {
            get { return _pageCount; }
            set
            {
                if (_pageCount == value) return;
                _pageCount = value;
                RaisePropertyChanged(() => PageCount);
            }
        }

        #endregion [--PageCount--]


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
            //var sampleCell = ImageCells.FirstOrDefault();
            //AccessionNumber = sampleCell.AccessionNumber;
        }
    }
}
