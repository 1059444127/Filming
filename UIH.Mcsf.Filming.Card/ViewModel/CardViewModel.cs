using System.Collections.ObjectModel;
using System.Diagnostics;
using UIH.Mcsf.Filming.Card.Model;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.Utility;

namespace UIH.Mcsf.Filming.Card.ViewModel
{
    public class CardViewModel : Notifier
    {
        public CardViewModel(CardModel cardModel)
        {
            _cardModel = cardModel;
            _cardModel.DisplayModeChanged += (sender, args) => SelectedDisplayMode = _cardModel.DisplayMode;
        }

        #region [--Handlers for Event From CardModel--]


        #endregion //[--Handlers for Event From CardModel--]

        

        #region [--Notified Properties--]

        public ObservableCollection<int> DisplayModes
        {
            get { return _displayModes; }
            set
            {
                if (_displayModes == value) return;
                _displayModes = value;
                NotifyPropertyChanged("DisplayModes");
            }
        }

        public int SelectedDisplayMode
        {
            get { return _selectedDisplayMode; }
            set
            {
                var log = string.Format("[ViewModule][SelectedDisplayMode][Changed] [OldValue]{0} [NewValue] {1}", _selectedDisplayMode, value);
                Logger.Instance.LogDevInfo(log);
                DebugHelper.Trace(TraceLevel.Info, log);

                if (_selectedDisplayMode == value) return;
                _selectedDisplayMode = value;
                NotifyPropertyChanged(() => SelectedDisplayMode);

                _cardModel.DisplayMode = _selectedDisplayMode;
            }
        }

        public int BoardCursor
        {
            get { return _boardCursor; }
            set
            {
                if (_boardCursor == value) return;
                _boardCursor = value;
                NotifyPropertyChanged(() => BoardCursor);
            }
        }


        public int BoardEnd
        {
            get { return _boardEnd; }
            set
            {
                if (_boardEnd == value) return;
                _boardEnd = value;
                NotifyPropertyChanged(() => BoardEnd);
            }
        }


        public int PageCursor
        {
            get { return _pageCursor; }
            set
            {
                if (_pageCursor == value) return;
                _pageCursor = value;
                NotifyPropertyChanged(() => PageCursor);
            }
        }

        #endregion //[--Notified Properties--]

        #region [--Fields--]

        private ObservableCollection<int> _displayModes = new ObservableCollection<int>(GlobalDefinition.DisplayModes);

        private int _selectedDisplayMode = GlobalDefinition.DefaultDisplayMode;

        private int _boardCursor = 0;

        private int _boardEnd = 0;

        private int _pageCursor = 0;
        private CardModel _cardModel;

        #endregion //[--Fields--]

        
    }
}
