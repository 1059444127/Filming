using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public sealed class FilmRepository : SelectableList<ISelectableFilm>, IFilmRepository
    {
        public FilmRepository()
        {
            Add(new Film{IsVisible = true});
        }

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

        //TODO-Later: When Base(SelectableList) Focus Changed, FilmRepository Focus Changed
        private int Focus
        {
            get { return _focus; }
            set
            {
                if (_focus == value) return;
                _focus = value;
                NO = _focus/VisibleCount;
            }
        }

        #endregion


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
            UpdateFilmIndex();

            MaxNO = Count==0?0
                : (int)Math.Ceiling((0.0 + Count) / VisibleCount) - 1; 
        }

        private void UpdateFilmIndex()
        {
            for (int i = 0; i < Count; i++)
            {
                var film = this[i];
                var title = film.FilmTitle;
                title.NO = i;
                title.Count = Count;
            }
        }

        #region Implementation of IVisibleCountSubject

        public event EventHandler VisibleCountChanged = delegate { };

        #endregion

        #region [--Film Buffer Function--]

        private int _visibleCount = 1;

        public int VisibleCount
        {
            get
            {
                return _visibleCount;
            }
            set
            {
                if(_visibleCount == value) return;
                _visibleCount = value;
                VisibleCountChanged(this, new EventArgs());
            }
        }



        #endregion

        #region [--Implemented from IDegree--]

        private int _no;
        public int NO
        {
            get { return _no; }
            set
            {
                if (_no == value) return;
                DropCurtain();
                _no = value;
                NOChanged(this, new EventArgs());
                RaiseCurtain();
            }
        }

        public event EventHandler NOChanged = delegate { };

        private int _maxNO;
        public int MaxNO
        {
            get { return _maxNO; }
            private set
            {
                if (_maxNO == value) return;
                _maxNO = value;
                MaxNOChanged(this, new EventArgs());
            }
        }

        public event EventHandler MaxNOChanged = delegate { };

        #endregion [--Implemented from IDegree--]

        #region [--Implemented from IFilmBuffer--]

        public event EventHandler<IntEventArgs> FilmChanged = delegate { };

        #endregion


        private void DropCurtain()
        {
            ShowBoard(false);
        }

        private void RaiseCurtain()
        {
            NotifyBoardChanged();
            ShowBoard(true);
        }

        private int _cursor;
        private void NotifyBoardChanged()
        {
            for (int i = 0; i < VisibleCount; i++)
            {
                FilmChanged(this, new IntEventArgs(_cursor+i));
            }
        }

        private void ShowBoard(bool isVisible)
        {
            var beginFilmIndex = _no*VisibleCount;
            for (int i = 0; i < VisibleCount; i++)
            {
                this[beginFilmIndex + i].IsVisible = isVisible;
            }
        }

    }
}