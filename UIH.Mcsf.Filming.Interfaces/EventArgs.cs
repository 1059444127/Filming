using System;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class ClickStatusEventArgs : EventArgs
    {
        public ClickStatusEventArgs(IClickStatus clickStatus)
        {
            ClickStatus = clickStatus;
        }

        public IClickStatus ClickStatus { get; private set; }
    }


    // TODO: Move IntEventArgs From Project Interfaces to Project Utilities
    public class IntEventArgs : EventArgs
    {
        public IntEventArgs(int i)
        {
            Int = i;
        }

        public int Int { get; private set; }
    }
    
}