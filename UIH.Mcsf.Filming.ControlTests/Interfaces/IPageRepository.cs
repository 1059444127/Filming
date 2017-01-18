using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    // IPageRepository Like IList
    public interface IPageRepository
    {
        void AppendPage();

        int Count { get; }
        event EventHandler CountChanged;

        PageModel this[int i] { get; }
    }

}