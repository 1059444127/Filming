namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    public class FilmControlViewModel : PageControlViewModel
    {
        private FilmTitleBarViewModel _filmTitleBarViewModel;

        public FilmControlViewModel()
        {
            _filmTitleBarViewModel = new FilmTitleBarViewModel();
        }

        public FilmTitleBarViewModel FilmTitleBarViewModel
        {
            set
            {
                _filmTitleBarViewModel = value;
                TitleBarViewModel = value;
            }
        }
    }
}