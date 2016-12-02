using System.Collections.Generic;
using UIH.Mcsf.Database;
using UIH.Mcsf.Filming.Wrapper;

namespace UIH.Mcsf.Filming.DataModel
{
    public class CellFactory // : ObjectWithAspects
    {
        #region [--Singleton--]

        private static readonly CellFactory _instance = new CellFactory();
        

        public static CellFactory Instance { get { return _instance; } }

//        public ImageCell EmptyCell { get; private set; }


        private CellFactory()
        {
//            EmptyCell= new EmptyCell();
        }

        static CellFactory(){}

        #endregion [--Singleton--]

        #region [--Factory Method--]
        public ImageCell CreateCell(ImageBase imageBase) { return new Cell(imageBase); }
        public ImageCell CreateCell(byte[] bytes) { return new AppCell(bytes); }
        public ImageCell CreateCell() {return new EmptyCell();}
        public IEnumerable<ImageCell> CreateCells(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return CreateCell();
            }
        }
        #endregion [--Factory Method--]


    }
}
