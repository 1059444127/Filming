using System.Collections.Generic;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Adapters
{
    public class ImageCellFactory
    {
        public static IList<ImageCell> CreateCells(int cellCount)
        {
            var cells = new List<ImageCell>();
            for (var i = 0; i < cellCount; i++)
            {
                cells.Add(new NullImageCell());
            }
            return cells;
        }

        public static ImageCell CreateCell()
        {
            return new NullImageCell();
        }

        public static ImageCell CreateCell(string sopInstanceUid)
        {
            return new FilmingImageCell(sopInstanceUid);
        }
    }
}