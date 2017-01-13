using System.Windows;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    class DynamicSubContentControlViewModel : TestViewModelBase
    {
        private object _subContentViewModel;

        public DynamicSubContentControlViewModel()
        {
            SubContentViewModel = new FooControlViewModel();
        }

        public object SubContentViewModel
        {
            get { return _subContentViewModel; }
            set
            {
                _subContentViewModel = value;
                RaisePropertyChanged(() => SubContentViewModel);
            }
        }

        #region Overrides of TestViewModelBase

        protected override void UpdateViewModel()
        {
            MessageBox.Show("Middle Button clicked in DynamicSubContentControl.");

            MessageBox.Show("Content will be changed");

            SubContentViewModel = new FooControl2ViewModel();

        }

        #endregion
    }
}
