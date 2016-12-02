using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using UIH.Mcsf.Filming.DataModel;
using UIH.Mcsf.Filming.Interface;
using UIH.Mcsf.Filming.Widgets;

namespace UIH.Mcsf.Filming.DemoViewModel
{
    public class ControlPanelViewModel : Notifier
    {
        private readonly IBoardProperty _boardProperty;

        public ControlPanelViewModel(IBoardProperty boardProperty)
        {
            _boardProperty = boardProperty;
            RegisterEventHandler();
        }

        private void RegisterEventHandler()
        {
            _boardProperty.DisplayModeChanged += BoardPropertyOnDisplayModeChanged;
            _boardProperty.LayoutChanged += BoardPropertyOnLayoutChanged;
        }

        #region [--EventHandler--]

        private void BoardPropertyOnLayoutChanged(object sender, LayoutEventArgs layoutEventArgs)
        {
            var layout = layoutEventArgs.Layout;
            Row = layout.Rows;
            Col = layout.Columns;
        }

        private void BoardPropertyOnDisplayModeChanged(object sender, IntEventArgs intEventArgs)
        {
            DisplayMode = intEventArgs.Int;
        }

        #endregion [--EventHandler--]

        #region [--CandidateLayout--]

        private readonly ObservableCollection<int> _rows =
            new ObservableCollection<int>(Enumerable.Range(1, LayoutBase.MaxRows));

        private readonly ObservableCollection<int> _cols =
            new ObservableCollection<int>(Enumerable.Range(1, LayoutBase.MaxColumns));

        public ObservableCollection<int> Rows
        {
            get { return _rows; }
        }

        public ObservableCollection<int> Cols
        {
            get { return _cols; }
        }

        #endregion [--CandidateLayout--]

        #region Col

        private int _col;

        public int Col
        {
            get { return _col; }
            set
            {
                if (_col == value) return;
                _col = value;
                NotifyPropertyChanged(() => Col);
            }
        }

        #endregion //Col

        #region Row

        private int _row;

        public int Row
        {
            get { return _row; }
            set
            {
                if (_row == value) return;
                _row = value;
                NotifyPropertyChanged(() => Row);
            }
        }

        #endregion //Row

        #region DisplayMode

        private int _displayMode = 1;

        public int DisplayMode
        {
            get { return _displayMode; }
            set
            {
                if (_displayMode == value) return;
                _displayMode = value;
                NotifyPropertyChanged(() => DisplayMode);
                _boardProperty.DisplayMode = value;
            }
        }

        #endregion //DisplayMode

        #region SetLayoutCommand

        private ICommand _setLayoutCommand;

        public ICommand SetLayoutCommand
        {
            get
            {
                return
                    _setLayoutCommand =
                        _setLayoutCommand ??
                        new RelayCommand(p => _boardProperty.Layout = LayoutFactory.Instance.CreateLayout(Col, Row));
            }
        }

        #endregion SetLayoutCommand
    }
}