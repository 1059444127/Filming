using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IPageRepository
    {
        void AppendPage();

        int Count { get; }
        event EventHandler CountChanged;

        PageModel this[int i] { get; }
    }

}