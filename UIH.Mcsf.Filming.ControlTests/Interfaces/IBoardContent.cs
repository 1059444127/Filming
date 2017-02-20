using System;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoardContent 
    {
        // Common
        int Count { get; set; }
        event EventHandler CountChanged;
        
        // Board need
        IFilm this[int i] { get; }
        event EventHandler<IntEventArgs> CellChanged;

        // CardControlViewModel need
        int NO { get; set; }
        int MaxNO { get;}
        event EventHandler NOChanged;
        event EventHandler MaxNOChanged;
    }
}