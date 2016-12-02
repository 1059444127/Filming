using System;
namespace UIH.Mcsf.Filming.Events
{
    public class FilmingCardSizeArgs : EventArgs
    {
        public FilmingCardSizeArgs(double left, double top, double width, double height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public double Left { get; private set; }
        public double Top { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
    }
}
