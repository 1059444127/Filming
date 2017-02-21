﻿using System;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoardContent : ICountSubject
    {
        // ICountSubject
        new int Count { get; set; }
        
        // Board need
        IFilm this[int i] { get; }
        event EventHandler<IntEventArgs> CellChanged;
        event EventHandler Changed;

        // CardControlViewModel need
        int NO { get; set; }
        int MaxNO { get;}
        event EventHandler NOChanged;
        event EventHandler MaxNOChanged;
    }
}