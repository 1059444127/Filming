using System.Linq;
using UIH.Mcsf.Filming.DataModel;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.Filming.Wrapper;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.DemoViewModel
{
    [CallTrace(true)]
    public class PageViewModel : Notifier
    {
        private readonly Page _page;

        public PageViewModel(Page page)
        {
            _page = page;
            Refresh();
        }

        ~PageViewModel()
        {
            _page.SelectedChanged -= PageOnSelectedChanged;
        }

        private void PageOnSelectedChanged(object sender, BoolEventArgs boolEventArgs)
        {
            IsSelected = boolEventArgs.Bool;
        }

        private void Refresh()
        {
            _page.SelectedChanged -= PageOnSelectedChanged;
            _page.SelectedChanged += PageOnSelectedChanged;
            IsSelected = _page.IsSelected;
            PageNo = _page.No + 1;
            PageCount = _page.PageCount;
            RefreshPatientInfo();
        }

        private void RefreshPatientInfo()
        {
            var cells = _page.Cells;
            var sampleCell = cells.FirstOrDefault(cell => !cell.IsEmpty);
            if (sampleCell == null) return;
            var displayData = sampleCell.DisplayData;
            var imageHeader = displayData.ImageHeader;
            if (imageHeader == null) return;
            var dicomHeader = imageHeader.DicomHeader;
            if (dicomHeader == null) return;
            if (dicomHeader.ContainsKey(ServiceTagName.PatientName))
                PatientName = displayData.ImageHeader.DicomHeader[ServiceTagName.PatientName];
        }

        #region [--Properties--]

        #region PatientName

        private string _patientName;

        public string PatientName
        {
            get { return _patientName; }
            set
            {
                if (_patientName == value) return;
                _patientName = value;
                NotifyPropertyChanged(() => PatientName);
            }
        }

        #endregion //PatientName

        #region No

        private int _pageNo;

        public int PageNo
        {
            get { return _pageNo; }
            set
            {
                if (_pageNo == value) return;
                _pageNo = value;
                UpdatePageIndex();
            }
        }

        private void UpdatePageIndex()
        {
            PageIndex = string.Format("{0}/{1}", _pageNo, _pageCount);
        }

        #endregion //No

        #region PageCount

        private int _pageCount;

        public int PageCount
        {
            get { return _pageCount; }
            set
            {
                if (_pageCount == value) return;
                _pageCount = value;
                UpdatePageIndex();
            }
        }

        #endregion //PageCount

        #region PageIndex

        private string _pageIndex;

        public string PageIndex
        {
            get { return _pageIndex; }
            set
            {
                if (_pageIndex == value) return;
                _pageIndex = value;
                NotifyPropertyChanged(() => PageIndex);
            }
        }

        #endregion //PageIndex

        #region IsSelected

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                NotifyPropertyChanged(() => IsSelected);
            }
        }

        #endregion //IsSelected

        #endregion [--Properties--]
    }
}