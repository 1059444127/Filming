using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    public class FilmControlViewModel : PageControlViewModel
    {
        private FilmTitleBarViewModel _filmTitleBarViewModel;

        public FilmControlViewModel()
        {
            Film = new NullFilm();
            FilmTitleBarViewModel = new FilmTitleBarViewModel();
        }

        public FilmTitleBarViewModel FilmTitleBarViewModel
        {
            set
            {
                _filmTitleBarViewModel = value;
                TitleBarViewModel = value;
            }
        }

        #region [--Film--]

        private IFilm _film;

        public IFilm Film
        {
            set
            {
                if (_film == value) return;
                _film = value;
                RefreshProperties();

            }
        }

        private void RefreshProperties()
        {
        }

        #endregion [--Film--]

    }
}