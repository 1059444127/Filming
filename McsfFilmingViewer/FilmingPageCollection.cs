using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming
{
    public class FilmingPageCollection:List<FilmingPageControl>
    {
        private readonly IFimingCountChangeNotifier _countChangeNotifier;

        public FilmingPageCollection(IFimingCountChangeNotifier countChangeNotifier)
        {
            _countChangeNotifier = countChangeNotifier;
        }

        public void ClearTextAnnotationEditableStatus()
        {
            try
            {
                Logger.LogFuncUp();
                int iPageNum = this.Count;
                for (int i = iPageNum - 1; i >= 0; i--)
                {
                    FilmingPageControl page = this[i];
                    page.filmingViewerControl.Focus();
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        public void UnSelectAllCells()
        {
            try
            {
                Logger.LogFuncUp();
                int iPageNum = this.Count;
                for (int i = iPageNum-1; i >= 0 ; i--)
                {
                    FilmingPageControl page = this[i];
                    page.IsSelected = false;
                    page.SelectViewports(false);
                    page.SelectedAll(false);
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }
        public void UnselectOtherFilmingPages(FilmingPageControl page)
        {
            Logger.LogFuncUp();
            foreach (var filmPage in this)
            {
                if (page.FilmPageIndex != filmPage.FilmPageIndex)
                {
                    filmPage.IsSelected = false;
                    filmPage.SelectedAll(false);
                }
            }
            Logger.LogFuncDown();  
        }
        public FilmingPageControl First
        {
            get { return this.FirstOrDefault(); }
        }
        public void AddPage(FilmingPageControl page)
        {
            if(!Contains(page))
                Add(page);
        }
        public void RemovePage(FilmingPageControl page)
        {
            if (Contains(page))
                Remove(page);
        }

       
        public void SetPagesSelectedStatusNotIn(int startPageIndex,int endPageIndex,bool selectedStatus)
        {
            if (startPageIndex > endPageIndex)
                return;
            var list = this.Where(
                   filmingPage =>
                   filmingPage.FilmPageIndex < startPageIndex || filmingPage.FilmPageIndex > endPageIndex).ToList();
            foreach (var filmingPage in list)
            {
                FilmPageUtil.SelectAllOfFilmingPage(filmingPage, selectedStatus);
            }
        }
        public void SetPagesSelectedStatusBetween(int startPageIndex, int endPageIndex, bool selectedStatus)
        {
            if (startPageIndex > endPageIndex)
                return;
            var list = this.Where(
                   filmingPage =>
                   filmingPage.FilmPageIndex >= startPageIndex && filmingPage.FilmPageIndex <= endPageIndex).ToList();
            foreach (var filmingPage in list)
            {
                FilmPageUtil.SelectAllOfFilmingPage(filmingPage, selectedStatus);
            }
        }
        public void ClearPagesBetween(int startPageIndex,int endPageIndex)
        {
            for(int i=startPageIndex;i<endPageIndex;i++)
            {
                this[i].Clear();
            }
        }
        public void ClearCellsBetweenPages(int startPageIndex, int endPageIndex)
        {
            for(int i=startPageIndex;i<endPageIndex;i++)
            {
                this[i].filmingViewerControl.RemoveAll();
            }
        }

        public void SetFilmLayoutBetween(FilmLayout filmLayout, int startPageIndex, int endPageIndex)
        {
            if (startPageIndex > endPageIndex)
                return;
            for (int i = startPageIndex; i < endPageIndex; i++)
            {
                if(this[i].ViewportLayout != filmLayout)
                    this[i].ViewportLayout = filmLayout;
            }
        }
        public void UnselectAllFilmingPages()
        {
            foreach (var fpc in this)
            {
                fpc.IsSelected = false;
            }
        }
        public FilmingPageControl GetFilmPageControlByViewerControl(MedViewerControl viewerControl)
        {
            if (viewerControl == null)
                return null;
            foreach (var filmPage in this)
            {
                if (filmPage.filmingViewerControl.Equals(viewerControl))
                {
                    return filmPage;
                }
            }
            return null;
        }
        public void RemoveEmptyPages()
        {
            RemoveAll(film => film.IsEmpty());
            UpdatPageIndex();
            UpdatePageLabel();
        }

        private void UpdatPageIndex()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i].FilmPageIndex = i;
            }
        }

        public void UpdatePageLabel()
        {
            foreach (var filmingPage in this)
            {
                int filmNumber = filmingPage.FilmPageIndex + 1;

                filmingPage.FilmPageTitileNumber = filmNumber + "/" + this.Count;
            }

            if (_countChangeNotifier != null)
            {
                _countChangeNotifier.UpdateFilmingCount(Count);
                if (Count >= 1)
                {
                    _countChangeNotifier.UpdateImageCountRemain(this[this.Count - 1].NonEmptyImageCount);
                }
            }

            UpdateBreakPageFlag();
        }

        public void UpdateBreakPageFlag()
        {
            int pageCount = this.Count;

            if (pageCount == 0) return;

            var previousPage = this[0];
            previousPage.RefereshPageTitle();  //一直未更新pageIndex为0的胶片title
            for (int i = 1; i < pageCount; i++)
            {
                var nextPage = this[i];
                previousPage.SetPageBreakLabel(nextPage.FilmPageType == FilmPageType.BreakFilmPage);
                previousPage = nextPage;
                nextPage.RefereshPageTitle();
            }
           
            previousPage.SetPageBreakLabel(false);
        }
        public void UpdatePageTitleDisplay()
        {
            foreach (FilmingPageControl filmingPageControl in this)
            {
                filmingPageControl.UpdatePageTitleDisplay();
                if (filmingPageControl.GetFilmingCard().IsCellModalitySC)
                {
                    filmingPageControl.filmPageBarGrid.Visibility = Visibility.Collapsed;
                    filmingPageControl.filmPageBarGridSimple.Visibility = Visibility.Collapsed;
                }
            }
        }
        public void UpdateAllPageTitle()
        {
            foreach (FilmingPageControl fpc in this)
            {
                fpc.RefereshPageTitle();
            }
        }
    }
}
