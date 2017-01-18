using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    // IRepository Like IList
    public interface IRepository
    {
        void Append();

        int Count { get; }
        event EventHandler CountChanged;

        PageModel this[int i] { get; }
    }

}