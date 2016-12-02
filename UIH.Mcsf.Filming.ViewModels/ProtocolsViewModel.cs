using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using UIH.Mcsf.Filming.Interface;
using UIH.Mcsf.Filming.Models;
using UIH.Mcsf.Filming.Widgets;

namespace UIH.Mcsf.Filming.ViewModels
{

    public class ProtocolsViewModel : Notifier
    {
        //public event PropertyChangedEventHandler PropertyChanged;

      
        
        #region [--Bindings--]
        public ProtocolsViewModel()
        {
            _model = new ProtocolsModel();
            Protocols = _model.GetProtocols();
	        var protocols = Protocols.ToList();
            protocols.ForEach(p=>p.PropertyChanged += ProtocolChanged);
        }

	    private void ProtocolChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
	    {
		    IsProtocolChanged = true;
	    }

	    private bool _isProtocolChanged = false;

	    public bool IsProtocolChanged
	    {
		    get { return _isProtocolChanged; }
		    set
		    {
			    if (_isProtocolChanged == value) return;
			    _isProtocolChanged = value;
			    NotifyPropertyChanged(() => IsProtocolChanged);
		    }
	    }

	    public ObservableCollection<Protocol> Protocols
	    {
		    get { return _protocols; }
		    set
		    {
			    if (_protocols == value) return;
			    _protocols = value;
			    NotifyPropertyChanged(()=>Protocols);
		    }
	    }

        public ObservableCollection<Protocol> SelectedProtocols
	    {
            get { return _selectedProtocols; }
		    set
		    {
			    if(value == _selectedProtocols) return;
			    _selectedProtocols = value;

		        foreach (var selectedProtocol in _selectedProtocols)
		        {
		            selectedProtocol.Layout = _setLayout;
		        }
		    }
	    }

        #endregion [--Bindings--]
        #region [--Commands--]
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand
                       ?? (_searchCommand
                           = new RelayCommand(SearchProtocol));
            }
        }

        public ICommand ResetProtocolLayoutCommand
        {
            get
            {
                return _resetProtocolLayoutCommand
                    ?? (_resetProtocolLayoutCommand
                    = new RelayCommand(
                        param => _selectedProtocols.ToList().ForEach(p=>p.Layout=DefaultLayout.Instance), 
                        param => _selectedProtocols!=null && _selectedProtocols.Count!=0));
            }
        }

	    public ICommand ConfirmCommand
        {
            get
            {
                return _confirmCommand
                       ?? (_confirmCommand
                           = new RelayCommand(SaveProtocolLayouts, 
                               param => IsProtocolChanged));
            }
        }

        //public ICommand CancelCommand
        //{
        //    get
        //    {
        //        return _cancelCommand
        //               ?? (_cancelCommand
        //                   = new RelayCommand(param=>));
        //    }
        //}

        public ICommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand
                       ?? (_confirmCommand
                           = new RelayCommand(SelectionChanged));
            }
        }

        #endregion [--Commands--]
        #region [--Private Methods--]
        private void SaveProtocolLayouts(object obj)
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ProtocolsViewModel.SaveProtocolLayouts]" );
	        _model.SaveProtocols();
	        IsProtocolChanged = false;
        }

        public void SaveProtocolLayoutsHint()
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ProtocolsViewModel.SaveProtocolLayouts]");
            _model.SaveProtocols();
            IsProtocolChanged = false;
        }


        private void SearchProtocol(object key)
        {
            string keyword = key == null ? string.Empty : key.ToString();
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ProtocolsViewModel.SearchProtocol]" + "[key]" +　keyword);
	        Protocols = _model.GetProtocols(keyword);
        }

        private void SelectionChanged(object obj)
        {

            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ProtocolsViewModel.SelectionChanged]" + "[obj]" );

            var objects = obj as IEnumerable<object>;
            if (objects == null)
            {
                Logger.Instance.LogDevWarning("[Binding][SelectedItems]No item selected or type cast mistake");
                return;
            }
            
            _selectedProtocols = new ObservableCollection<Protocol>();

            foreach (var o in objects)
            {
                var protocol = o as Protocol;
                if (protocol == null)
                {
                    Logger.Instance.LogDevWarning("Selected Item is not a protocol");
                    continue;
                }
                _selectedProtocols.Add(protocol);
                if (_setLayout == null || _setLayout is DefaultLayout) continue;
                protocol.Layout = _setLayout;
            }
        }

        #endregion [--Private Methods--]


        #region [--Fields--]
        private ICommand _searchCommand;
        private ICommand _resetProtocolLayoutCommand;
        private ICommand _confirmCommand;
        //private ICommand _cancelCommand;
        private ProtocolsModel _model;
        private ObservableCollection<Protocol> _protocols;

        private ObservableCollection<Protocol> _selectedProtocols = new ObservableCollection<Protocol>();
#pragma warning disable 649
        private ICommand _selectionChangedCommand;
#pragma warning restore 649
        private LayoutBase _setLayout = DefaultLayout.Instance;

        #endregion [--Fields--]

        public LayoutBase SetLayout
        {
            set
            {
                _setLayout = value;
                if (_setLayout == null || _setLayout is DefaultLayout) return;
                foreach (var selectedProtocol in _selectedProtocols)
                {
                    selectedProtocol.Layout = _setLayout;
                }
            }
        }
    }

}
