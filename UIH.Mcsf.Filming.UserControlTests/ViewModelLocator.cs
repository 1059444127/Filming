using System;
using UIH.Mcsf.Filming.ViewModel;

namespace UIH.Mcsf.Filming.UserControlTests
{
    public class ViewModelLocator
    {
        private static object CreateViewModel()
        {
            return _001_Create_ViewerControlAdapter_ViewModel();
        }

        #region [--UserControl-ViewModel-Created--]

        private static object _001_Create_ViewerControlAdapter_ViewModel()
        {
            return new ViewerControlAdapterViewModel();
        }

        #endregion

        // TODO: Package-Uninstall MvvmLibs From UserControlTests Project
        public static Object MainWindowViewModelStatic = CreateViewModel();
    }
}