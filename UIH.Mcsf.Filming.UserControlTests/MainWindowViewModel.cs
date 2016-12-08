using UIH.Mcsf.Filming.ViewModel;

namespace UIH.Mcsf.Filming.UserControlTests
{
    public class MainWindowViewModel
    {
        private object _userControlViewModel;

        public object UserControlViewModel
        {
            get
            {
                return _userControlViewModel ?? (_userControlViewModel = CreateUserControlViewModel());
            }
        }

        private object CreateUserControlViewModel()
        {
            return _001CreateViewerControlAdapterViewModel();
        }

        private object _001CreateViewerControlAdapterViewModel()
        {
            return new ViewerControlAdapterViewModel();
        }
    }
}