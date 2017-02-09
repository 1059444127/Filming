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
                new PageControlViewModel {TitleBarViewModel = new TitleBarViewModel {NO = 1, Count = 8}},
                new PageControlViewModel {TitleBarViewModel = new TitleBarViewModel {NO = 2, Count = 8}},
                new PageControlViewModel {TitleBarViewModel = new TitleBarViewModel {NO = 3, Count = 8}},
                new PageControlViewModel {TitleBarViewModel = new TitleBarViewModel {NO = 4, Count = 8}},
                new PageControlViewModel {TitleBarViewModel = new TitleBarViewModel {NO = 5, Count = 8}},
                new PageControlViewModel {TitleBarViewModel = new TitleBarViewModel {NO = 6, Count = 8}},
                new PageControlViewModel {TitleBarViewModel = new TitleBarViewModel {NO = 7, Count = 8}},
                new PageControlViewModel {TitleBarViewModel = new TitleBarViewModel {NO = 8, Count = 8}}
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
        }

        #endregion
    }

    public interface IGridCell
    {
        Visibility Visibility { get; set; }
    }
}
