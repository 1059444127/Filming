using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class FilmRepository : SelectableList<ISelectableFilm>, IFilmRepository
    {
        #region Overrides of SelectableList<ISelectableFilm>

        public override ISelectableFilm this[int index]
        {
            get
            {
                if (index < Count) return base[index];
                return new NullFilm();
            }
        }

        #endregion
    }
}