using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class SimpleLayout : Layout
    {
        private readonly int _row;
        private readonly int _col;

        public SimpleLayout(int row, int col)
        {
            _row = row;
            _col = col;
        }

        #region Overrides of Layout

        public override void Setup(LayoutManager layoutManager)
        {
            layoutManager.SetLayout(_row, _col);
        }

        public override int Capacity
        {
            get { return  _row * _col; }
        }

        #endregion
    }
}