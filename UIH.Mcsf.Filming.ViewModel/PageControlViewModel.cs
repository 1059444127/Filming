using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Filming.Model;

namespace UIH.Mcsf.Filming.ViewModel
{
    public class PageControlViewModel : ViewModelBase
    {
        private IBoardCell _boardCell;
        private PageModel _pageModel = PageModelFactory.CreatePageModel();
        public PageControlViewModel(IBoardCell boardCell)
        {
            _boardCell = boardCell;

            RegisterBoardCellEvent();

        }

        private void RegisterBoardCellEvent()
        {
            _boardCell.PageModelChanged += BoardCellOnPageModelChanged;
            _boardCell.RowChanged += BoardCellOnRowChanged;
            _boardCell.ColChanged += BoardCellOnColChanged;
        }

        private void BoardCellOnColChanged(object sender, EventArgs eventArgs)
        {
            Row = _boardCell.Row;
        }

        private void BoardCellOnRowChanged(object sender, EventArgs eventArgs)
        {
            Col = _boardCell.Col;
        }

        private PageModel PageModel
        {
            set
            {
                if (_pageModel == value) return;

                UnRegisterPageModelEvent();
                _pageModel = value;
                RegisterPageModelEvent();

                RefreshPage();
            }
        }

        private void BoardCellOnPageModelChanged(object sender, EventArgs args)
        {
            PageModel = _boardCell.PageModel;
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

        private void PageModelOnIsBreakChanged(object sender, EventArgs args)
        {
            BreakVisibility = BoolToVisibility(_pageModel.IsBreak);
        }

        private Visibility BoolToVisibility(bool b)
        {
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PageModelOnVisibleChanged(object sender, EventArgs args)
        {
            Visibility = BoolToVisibility(_pageModel.IsVisible);
        }

        //TODO-Later: PageControlViewModel.RefreshTitle()
        private void RefreshTitle()
        {
            //var sampleCell = ImageCells.FirstOrDefault();
            //AccessionNumber = sampleCell.AccessionNumber;
        }

        #region [--Row--]

        private int _row;

        public int Row
        {
            get { return _row; }
            set
            {
                if (_row == value) return;
                _row = value;
                RaisePropertyChanged(() => Row);
            }
        }

        #endregion [--Row--]

        #region [--Col--]

        private int _col;

        public int Col
        {
            get { return _col; }
            set
            {
                if (_col == value) return;
                _col = value;
                RaisePropertyChanged(() => Col);
            }
        }

        #endregion [--Col--]


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
        
        // TODO-working-on: 更新PageControlViewModel.PageCount
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
    }
}