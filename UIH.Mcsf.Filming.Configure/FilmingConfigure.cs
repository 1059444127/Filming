using System.Globalization;

namespace UIH.Mcsf.Filming.Configure
{
    public class FilmingConfigure : AbstractFlatConfigure
    {
        public FilmingConfigure(string filePath) : base(filePath) 
        {
        }

        private int _viewMode;
        public int ViewMode
        {
            get { return _viewMode; }  
            set
            {
                _viewMode = value;
                WriteBack(value.ToString(CultureInfo.InvariantCulture), () => ViewMode);
            }
        }
    }
}
