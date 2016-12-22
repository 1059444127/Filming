namespace UIH.Mcsf.Filming.Interfaces
{
    public struct DisplayMode
    {
        public DisplayMode(int itemCount) : this()
        {
            Count = itemCount;

            Row = IsEven(itemCount) ? 2 : 1;
            Col = itemCount/Row;
        }

        public int Count { get; private set; }
        public int Row { get; private set; }
        public int Col { get; private set; }

        private bool IsEven(int i)
        {
            return i%2 == 0;
        }

        public bool Equals(DisplayMode displayMode)
        {
            return Count == displayMode.Count;
        }
    }
}