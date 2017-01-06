using GalaSoft.MvvmLight;

namespace UIH.Mcsf.Filming.UserControlTests
{
    public class GridTestViewModel : ViewModelBase
    {
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


    }
}