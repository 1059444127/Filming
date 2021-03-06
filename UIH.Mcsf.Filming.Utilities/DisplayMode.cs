﻿namespace UIH.Mcsf.Filming.Utilities
{
    public struct DisplayMode
    {
        public DisplayMode(int itemCount) : this()
        {
            Count = itemCount;

            Row = itemCount > 2 && itemCount%2 == 0 ? 2 : 1;
            Col = itemCount/Row;
        }

        private int Count { get; set; }
        public int Row { get; private set; }
        public int Col { get; private set; }

        public bool Equals(DisplayMode displayMode)
        {
            return Count == displayMode.Count;
        }
    }
}