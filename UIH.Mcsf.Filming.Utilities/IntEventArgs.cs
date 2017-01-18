using System;

namespace UIH.Mcsf.Filming.Utilities
{
    public class IntEventArgs : EventArgs
    {
        public IntEventArgs(int i)
        {
            Int = i;
        }

        public int Int { get; private set; }
    }
}