using System;
using UIH.Mcsf.Filming.Adapters;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Model
{
    public class PageRepository : SelectableList<PageModel>
    {
        private int _focusIndex;
        // TODO-working-on: PageRepository focused pageIndex
        public int FocusIndex
        {
            get { return _focusIndex; }
            private set
            {
                if(_focusIndex == value) return;
                _focusIndex = value;
                FocusChanged(this, new EventArgs());
            }
        }

        public override PageModel this[int pageNO]
        {
            get
            {
                if (pageNO < 0 || pageNO >= Count) return PageModelFactory.CreatePageModel();
                return base[pageNO];
            }
        }

        public virtual void AppendPage()
        {
            MakeLastPageBreak();

            // TODO-Later: Layout of New Page is the same with LastPage
            // TODO-Later£º PageRepository use LayoutFactory.CreateDefaultLayout(), Depends on File system, not good to UT
            // TODO-Later: Make Project Model not Dependent on Project Adapters 
            Add(PageModelFactory.CreatePageModel(LayoutFactory.CreateDefaultLayout()));

            var lastPageNO = Count - 1;
            FocusIndex = lastPageNO;
            PageChange(lastPageNO);
        }

        private void MakeLastPageBreak()
        {
            this[Count - 1].IsBreak = true;
        }

        #region [--For UT--]

        protected void PageChange(int pageNO)
        {
            PageChanged(this, new IntEventArgs(pageNO));
        }

        protected void FocusChange(int pageNO)
        {
            FocusIndex = pageNO;
        }

        #endregion

        #region [--Events--]

        public event EventHandler<IntEventArgs> PageChanged = delegate { };
        // TODO: PageControl.IsFocused(TitleBar.Border=Yellow & IsSelected(TitleBar.Fill=Aqua)
        public event EventHandler FocusChanged = delegate { };
        // TODO-working-on: pageCountChanged event
        // TODO-UT: PageRepository.PageCountChanged
        public event EventHandler PageCountChanged = delegate { };

        #endregion

        #region Overrides of SelectableList<PageModel>

        public override void Add(PageModel item)
        {
            base.Add(item);
            PageCountChanged(this, new EventArgs());
        }

        public override void Clear()
        {
            base.Clear();
            PageCountChanged(this, new EventArgs());
        }

        public override bool Remove(PageModel item)
        {
            var remove = base.Remove(item);
            PageCountChanged(this, new EventArgs());
            return remove;
        }

        public override void Insert(int index, PageModel item)
        {
            base.Insert(index, item);
            PageCountChanged(this, new EventArgs());
        }

        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);
            PageCountChanged(this, new EventArgs());
        }

        #endregion
    }
}