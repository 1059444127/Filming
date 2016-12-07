using System;
using UIH.Mcsf.Filming.DataModel;

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
            var layout = Layout.CreateDefaultLayout();
            return new ViewerControlAdapterViewModel(layout);
        }

        #endregion


        public static Object MainWindowViewModelStatic = CreateViewModel();
    }
}