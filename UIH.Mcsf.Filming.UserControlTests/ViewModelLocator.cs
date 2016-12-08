using System;
using UIH.Mcsf.Filming.ViewModel;

namespace UIH.Mcsf.Filming.UserControlTests
{
    public class ViewModelLocator
    {
        private static object CreateViewModel()
        {
            return _002_Create_PageControl_ViewModel();
        }

        #region [--UserControl-ViewModel-Created--]

        // TODO-working-on : Use PageControl To Test ViewerControlAdapter
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