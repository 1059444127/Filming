using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IDegree
    {
        int NO { get; set; }
        event EventHandler NOChanged;
        int MaxNO { get; }
        event EventHandler MaxNOChanged;
    }
}