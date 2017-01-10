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

    // TODO: Remove BoolEventArgs
    public class BoolEventArgs : EventArgs
    {
        public BoolEventArgs(bool b)
        {
            Bool = b;
        }

        public bool Bool { get; private set; }
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
    
    // TODO: Remove PageModelEventArgs
    public class PageModelEventArgs : EventArgs
    {
        public PageModelEventArgs(PageModel pageModel)
        {
            PageModel = pageModel;
        }

        public PageModel PageModel { get; private set; }
    }
}