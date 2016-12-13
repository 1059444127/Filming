﻿using System;

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

    public class BoolEventArgs : EventArgs
    {
        public BoolEventArgs(bool b)
        {
            Bool = b;
        }

        public bool Bool { get; private set; }
    }
}
