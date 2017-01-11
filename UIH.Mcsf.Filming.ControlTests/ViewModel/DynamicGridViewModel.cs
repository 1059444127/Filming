using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    class DynamicGridViewModel : TestViewModelBase
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

        #region Overrides of TestViewModelBase

        protected override void UpdateViewModel()
        {
            MessageBox.Show("Middle Button Pressed at DynamicGrid");

            MessageBox.Show("Make Page 6 Invisible");
            GridCellViewModels[5].Visibility = Visibility.Collapsed;

            MessageBox.Show("Move Page 3 to Position 6");
            GridCellViewModels[2].Col = 1;
            GridCellViewModels[2].Row = 1;
        }

        #endregion
    }

    public interface IGridCell
    {
        Visibility Visibility { get; set; }
        int Row { get; set; }
        int Col { get; set; }
    }
}
