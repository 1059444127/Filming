using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using UIH.Mcsf.MvvmLight;
using UIH.Mcsf.Viewer;
using ICommand = System.Windows.Input.ICommand;

namespace MEFViewer
{
    [Export(typeof (UserControl))]
    [ExportMetadata("ParentName", "ActionGrid")]
    public partial class ActionControl
    {
        public ActionControl()
        {
            InitializeComponent();

            DataContext = new ActionViewModel();
        }
    }

    public class ActionViewModel
    {

        #region [--Properties--]

        private void SetPanAction()
        {
            CurrentActionType = ActionType.Pan;
        }

        public ActionType CurrentActionType
        {
            set
            {
                if (_currentActionType == value) return;
                _currentActionType = value;
                Messenger.Default.Send<ActionType>(value);
            }
        }

        #endregion [--Properties--]

        #region [--PointerCommand--]

        private ICommand _pointerCommand;

        public ICommand PointerCommand
        {
            get { return _pointerCommand = _pointerCommand ?? new RelayCommand(SetPointerAction); }
        }

        private void SetPointerAction()
        {
            CurrentActionType = ActionType.Pointer;
        }

        #endregion [--PointerCommand--]

        #region [--PanCommand--] 

        private ICommand _panCommand;

        public ICommand PanCommand
        {
            get { return _panCommand = _panCommand ?? new RelayCommand(SetPanAction); }
        }

        private ActionType _currentActionType = ActionType.Pointer;

        #endregion [--PanCommand--]      
    }
}