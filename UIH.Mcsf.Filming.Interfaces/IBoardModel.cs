using System;

namespace UIH.Mcsf.Filming.Model
{
    public interface IBoardModel : IBoardComponet
    {
        int BoardNO { get; set; }
        int BoardCount { get; }
        event EventHandler BoardNOChanged;
        event EventHandler BoardCountChanged;
        void NewPage();
    }
}