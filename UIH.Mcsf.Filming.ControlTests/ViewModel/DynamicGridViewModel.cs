using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    public class DynamicGridViewModel : ViewModelBase
    {
        public DynamicGridViewModel()
        {
            GridCellViewModels = new List<IGridCell>
            {
                new PageControlViewModel {Row = 0, Col = 0, TitleBarViewModel = new TitleBarViewModel {PageNO = 1, PageCount = 8}},
                new PageControlViewModel {Row = 0, Col = 1, TitleBarViewModel = new TitleBarViewModel {PageNO = 2, PageCount = 8}},
                new PageControlViewModel {Row = 0, Col = 2, TitleBarViewModel = new TitleBarViewModel {PageNO = 3, PageCount = 8}},
                new PageControlViewModel {Row = 0, Col = 3, TitleBarViewModel = new TitleBarViewModel {PageNO = 4, PageCount = 8}},
                new PageControlViewModel {Row = 1, Col = 0, TitleBarViewModel = new TitleBarViewModel {PageNO = 5, PageCount = 8}},
                new PageControlViewModel {Row = 1, Col = 1, TitleBarViewModel = new TitleBarViewModel {PageNO = 6, PageCount = 8}},
                new PageControlViewModel {Row = 1, Col = 2, TitleBarViewModel = new TitleBarViewModel {PageNO = 7, PageCount = 8}},
                new PageControlViewModel {Row = 1, Col = 3, TitleBarViewModel = new TitleBarViewModel {PageNO = 8, PageCount = 8}}
            };
        }


        public IList<IGridCell> GridCellViewModels { get; private set; }

        #region [--CellCount--]

        private int _cellCount = 1;

        public int CellCount
        {
            get { return _cellCount; }
            set
            {
                if (_cellCount == value) return;
                _cellCount = value;
                RaisePropertyChanged(() => CellCount);
            }
        }

        #endregion [--CellCount--]

    }

    public interface IGridCell
    {
        Visibility Visibility { get; set; }
        int Row { get; set; }
        int Col { get; set; }
    }
}
