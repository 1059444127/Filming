namespace UIH.Mcsf.Filming.Utilities
{
    public class GridPosition
    {
        public GridPosition(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public int Row { get; private set; }
        public int Col { get; private set; }
    }
}
