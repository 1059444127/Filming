using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    // IRepository Like IList
    public interface IRepository
    {
        void Append();

        int Focus { get; set; }
        event EventHandler FocusChanged;
    }

}