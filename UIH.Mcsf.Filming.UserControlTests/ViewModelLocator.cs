using System;
using UIH.Mcsf.Filming.ViewModel;

namespace UIH.Mcsf.Filming.UserControlTests
{
    public class ViewModelLocator
    {
        // TODO: ViewModelLocator Add MainWindowViewModel to get an entry of program control
        private static object CreateViewModel()
        {
            return _002_Create_PageControl_ViewModel();
        }

        #region [--UserControl-ViewModel-Created--]

        private static object _002_Create_PageControl_ViewModel()
        {
            return new PageControlViewModel();
        }

        private static object _001_Create_ViewerControlAdapter_ViewModel()
        {
            return new ViewerControlAdapterViewModel();
        }

        #endregion

        public static Object MainWindowViewModelStatic = CreateViewModel();
    }
}