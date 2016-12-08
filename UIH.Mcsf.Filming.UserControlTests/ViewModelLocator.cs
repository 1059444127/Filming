using System;

namespace UIH.Mcsf.Filming.UserControlTests
{
    public class ViewModelLocator
    {
        private static object CreateViewModel()
        {
            return new MainWindowViewModel();
        }

        public static Object MainWindowViewModelStatic = CreateViewModel();
    }
}