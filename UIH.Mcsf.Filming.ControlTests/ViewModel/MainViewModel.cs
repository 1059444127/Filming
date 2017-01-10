using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            CardControlViewModel = new CardControlViewModel();
        }

        public CardControlViewModel CardControlViewModel { get; private set; }

        #region [--UpdateViewModelCommand--]

        private ICommand _updateViewModelCommand;

        public ICommand UpdateViewModelCommand
        {
            get { return _updateViewModelCommand = _updateViewModelCommand ?? new RelayCommand(UpdateViewModel); }
        }

        private void UpdateViewModel()
        {
            CardControlViewModel.DisplayMode = 6;
        }

        #endregion [--UpdateViewModelCommand--]    

   

    }
}