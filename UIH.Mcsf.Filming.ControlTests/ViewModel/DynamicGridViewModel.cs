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
