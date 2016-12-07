using System;

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


        public static Object MainWindowViewModelStatic = CreateViewModel();
    }
}