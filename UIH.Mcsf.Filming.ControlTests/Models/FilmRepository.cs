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

        public void AppendFilm()
        {
            Add(new Film());
            Focus = Count - 1;
        }

        #region [--Focus--]

        private int _focus;

        public int Focus
        {
            get { return _focus; }
            private set
            {
                if (_focus == value) return;
                _focus = value;
                FocusChanged(this, new EventArgs());
            }
        }

        #endregion

        public event EventHandler FocusChanged = delegate { };

        #endregion
    }
}