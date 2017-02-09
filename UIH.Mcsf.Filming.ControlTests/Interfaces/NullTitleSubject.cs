using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public class NullTitleSubject : ITitleSubject
    {
        #region Implementation of ITitle

        public int NO { get; set; }

        public int Count { get; set; }

        #endregion

        #region Implementation of ITitleSubject

        public event EventHandler NOChanged;
        public event EventHandler CountChanged;

        #endregion
    }
}