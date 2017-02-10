using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class BoardContent : IBoardContent
    {
        #region Implementation of IBoardContent

        public IFilm this[int i]
        {
            get { return new Film(){IsVisible = true}; }
        }

        public void AppendContent()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}