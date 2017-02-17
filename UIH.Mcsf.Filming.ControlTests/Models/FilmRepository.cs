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

        public void Append()
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

        #region Overrides of SelectableList<ISelectableFilm>

        public override void Add(ISelectableFilm item)
        {
            base.Add(item);
            Changed();
        }

        public override void Clear()
        {
            base.Clear();
            Changed();
        }

        public override bool Remove(ISelectableFilm item)
        {
            var removed = base.Remove(item);
            Changed();
            return removed;
        }

        public override void Insert(int index, ISelectableFilm item)
        {
            base.Insert(index, item);
            Changed();
        }

        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);
            Changed();
        }

        #endregion

        private void Changed()
        {
            for (int i = 0; i < Count; i++)
            {
                var film = this[i];
                var title = film.FilmTitle;
                title.NO = i;
                title.Count = Count;
            }

            CountChanged(this, new EventArgs());
        }

        #region Implementation of IVariableCollection

        public event EventHandler CountChanged = delegate { };

        #endregion
    }
}