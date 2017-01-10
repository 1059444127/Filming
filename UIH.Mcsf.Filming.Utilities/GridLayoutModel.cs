using System;

namespace UIH.Mcsf.Filming.Utilities
{
    public class GridLayoutModel
    {
        private readonly int _cols;
        private readonly int _cellCount;

        public GridLayoutModel(int rows, int cols)
        {
            _cols = cols;
            _cellCount = rows*_cols;
        }

        public GridPosition GetGridPositionBy(int childrenIndex)
        {
            var indexInGrid = childrenIndex%_cellCount;
            var row = indexInGrid/_cols;
            var col = indexInGrid%_cols;
            return  new GridPosition(row, col);
        }
    }
}
