using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    internal class BoardContent : IBoardContent
    {
        #region Implementation of IBoardContent

        public IFilm this[int i]
        {
            get { return new NullFilm(); }
        }

        public void AppendContent()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}