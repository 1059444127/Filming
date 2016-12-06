using System;

namespace UIH.Mcsf.Filming.UserControlTests
{
    public class ViewModelLocator
    {
        // TODO-working-on: ViewModelLocater

        public static Object MainWindowViewModelStatic = CreateViewModel();

        private static object CreateViewModel()
        {
            return _001_Create_ViewerControlAdapter_ViewModel();
        }

        #region [--UserControl-ViewModel-Created--]

        private static object _001_Create_ViewerControlAdapter_ViewModel()
        {
            return new ViewControlAdapterViewerModel();
        }

        #endregion
    }
}
