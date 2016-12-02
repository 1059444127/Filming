namespace UIH.Mcsf.Filming.Utility
{
    public class FilmImageObject
    {
        public string ImageFilePath { get; set; }

        public string GspsFilePath { get; set; }

        public string ImageSopInstanceUid { get; set; }

        private int _viewPortIndex = -1;
        public int ViewPortIndex
        {
            get { return _viewPortIndex; }
            set { _viewPortIndex = value; }
        }

        private int _cellIndex = -1;
        public int CellIndex
        {
            get { return _cellIndex; }
            set { _cellIndex = value; }
        }
    }
}
