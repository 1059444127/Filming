using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Auto
{
    public class ElectronicFilmInfo
    {
        private int _row = 1;
        private int _column = 1;
        private readonly List<string> _filePathList;


        public ElectronicFilmInfo(int row, int column, List<string> filePathList)
        {
            _row = row;
            _column = column;

            _filePathList = filePathList;
        }

        public int Rows
        {
            get { return _row; }
        }

        public int Columns
        {
            get { return _column; }
        }

        public List<string> FilePathList
        {
            get { return _filePathList; }
        }
    }
}
