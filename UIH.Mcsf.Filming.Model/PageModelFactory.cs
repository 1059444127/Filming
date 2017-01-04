using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Model
{
    public class PageModelFactory
    {
        public static PageModel CreatePageModel(Layout layout)
        {
            return new FilmPageModel(layout);
        }

        public static PageModel CreatePageModel()
        {
            return new NullPageModel();
        }
    }
}