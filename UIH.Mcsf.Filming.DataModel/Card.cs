using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Filming.Interface;
using UIH.Mcsf.Filming.Wrapper;
using Environment = UIH.Mcsf.Filming.Configure.Environment;

namespace UIH.Mcsf.Filming.DataModel
{
    public interface IBoardProvider
    {
        IBoardChange BoardChange { get; }
    }

    public interface IBoardPosition
    {
        int BoardNo { set; }
        event EventHandler<IntEventArgs> BoardCountChanged;
        event EventHandler<IntEventArgs> BoardNoChanged;
    }

    public interface IBoardProperty
    {
        int DisplayMode { set; }
        LayoutBase Layout { set; }
        event EventHandler<IntEventArgs> DisplayModeChanged;
        event EventHandler<LayoutEventArgs> LayoutChanged;
    }

    public interface IImageLoader
    {
        void LoadSeries(string seriesUid, int index = 0);
        void NewPage();
        void PreLoad(string sopInstanceUid);
    }

    public interface ICard : IBoardPosition, IBoardProperty, IImageLoader
    {
        void InitializeFromConfigure();
    }

    public class Card : ObjectWithAspects, IBoardProvider, ICard
    {
        public Card()
        {
            _board = new Board(_displayMode);
        }

        public int BoardCount
        {
            get { return _boardCount; }
            private set
            {
                if (_boardCount == value) return;
                _boardCount = value;
                BoardCountChanged(this, new IntEventArgs {Int = _boardCount});
            }
        }

        #region [--Implement From IBoardProvider--]

        public IBoardChange BoardChange
        {
            get { return _board; }
        }

        #endregion [--Implement From IBoardProvider--]

        #region [--Implement From ICard--]

        public void InitializeFromConfigure()
        {
            // Todo-later: Violate SRP, Extract to private method
            var filmingConfigure = Environment.Instance.GetFilmingConfigure();
            filmingConfigure.ParseConfigures();
            DisplayMode = filmingConfigure.ViewMode;

            var defaultLayoutConfigure = Environment.Instance.GetDefaultLayoutConfigure();
            //defaultLayoutConfigure.ParseConfigures();
            Layout = defaultLayoutConfigure.Layout;
        }

        #endregion [--Implement From ICard--]

        //private void SelectableCellListOnChanged(object sender, EventArgs eventArgs)
        //{
        //    //viewModel changes

        //    _pages = new SelectablePageList(_cells.BuildPages());
        //    BoardCount = (int) Math.Ceiling((0.0 + _pages.Count())/_displayMode);
        //    if (_boardNo < BoardCount)
        //    {
        //        NotifyBoardChanged();
        //        Task.Factory.StartNew(PrepareNextBoard);
        //    }
        //    else
        //    {
        //        BoardNo = BoardCount - 1;
        //    }
        //}

        private void NotifyBoardChanged()
        {
            //_taskCancellationTokenSource.Cancel();


            var pageStartIndex = _boardNo*_displayMode;
            var pageCount = Math.Min(_displayMode, _pages.Count - pageStartIndex);
            //BoardChanged(this, new BoardEventArgs(_pages.GetRange(pageStartIndex, pageCount)));
            _board.Pages = _pages.GetRange(pageStartIndex, pageCount);

            //_taskCancellationTokenSource = new CancellationTokenSource();
            //_task = Task.Factory.StartNew(PrepareNextBoard, _taskCancellationTokenSource.Token);
            //Console.WriteLine("Task Cancelled ? {0} ", _task != null && _task.IsCanceled);
        }

        private void PrepareNextBoard()
        {
            var pageNo = (_boardNo + 1)*_displayMode;

            for (var i = 0; i < _displayMode && pageNo < _pages.Count; i++)
            {
                _pages[pageNo].Cells.ForEach(cell => cell.Prepare());
                //Console.WriteLine("Prepare For Page {0}", pageNo);
            }
        }

        #region [--Field--]

        private int _boardCount = 1;
        //private List<ImageCell> _cells; 
        private int _boardNo;
        private int _displayMode = 1;
        private LayoutBase _layout;
        //private SelectablePageList _pages;
        private readonly Board _board;
        //private IBasicLoader _dataLoader;
        //private readonly SelectableCellList _cells;

        #endregion  [--Field--]

        #region [--Implement From IBoardPosition--]

        public int BoardNo
        {
            set
            {
                if (_boardNo == value) return;
                var delta = value - _boardNo;
                _boardNo = value;
                BoardNoChanged(this, new IntEventArgs {Int = _boardNo + 1});
                NotifyBoardChanged();
                if (delta > 0)
                    Task.Factory.StartNew(PrepareNextBoard);
            }
        }

        public event EventHandler<IntEventArgs> BoardCountChanged = delegate { };
        public event EventHandler<IntEventArgs> BoardNoChanged = delegate { };

        #endregion [--Implement From IBoardPosition--]

        #region [--Implement From IBoardProperty--]

        public LayoutBase Layout
        {
            private get { return _layout; }
            set
            {
                if (_layout != null && _layout.Equals(value)) return;
                _layout = value;
                //_cells.Layout = value;
                LayoutChanged(this, new LayoutEventArgs(value));
                var defaultLayoutConfigure = Environment.Instance.GetDefaultLayoutConfigure();
                defaultLayoutConfigure.Layout = value;
            }
        }

        public int DisplayMode
        {
            set
            {
                if (_displayMode == value) return;
                BoardNo = _displayMode*_boardNo/value;
                _displayMode = value;
                BoardCount = (int) Math.Ceiling((0.0 + _pages.Count())/_displayMode);
                DisplayModeChanged(this, new IntEventArgs {Int = value});
                _board.DisplayMode = value;
                NotifyBoardChanged();
                Environment.Instance.GetFilmingConfigure().ViewMode = value;
            }
        }

        public event EventHandler<IntEventArgs> DisplayModeChanged = delegate { };
        public event EventHandler<LayoutEventArgs> LayoutChanged = delegate { };

        #endregion [--Implement From IBoardProperty--]

        #region [--Implement From IImageLoader--]

        public void LoadSeries(string seriesUid, int index = 0)
        {
            //var db = DBWrapperHelper.DBWrapper;
            //CellLink如何变更
            //var newCells = db.GetImageListBySeriesInstanceUID(seriesInstanceUid).Select(image=>CellFactory.Instance.CreateCell(image.SOPInstanceUID));
            //_cells.AddSeries(seriesUid, index);
            //var emptyRangeEnd = _cells.FindIndex(index, c => c.IsEmpty);
            //if (emptyRangeEnd != -1) _cells.RemoveRange(index, emptyRangeEnd - index + 1);

            //_cells.AddRange(DBWrapperHelper.DBWrapper.GetImageListBySeriesInstanceUID(seriesUid).Select(image => new ImageCell() { RawData = image }));
        }

        public void PreLoad(string sopInstanceUid)
        {
            //find board count
            //preload count of images;
            var imageCountInOneBoard = Layout.Capacity*_displayMode;
            Task.Factory.StartNew(() =>
            {
                var dbWrapper = DBWrapperHelper.DBWrapper;
                var images = dbWrapper.GetImageListBySeriesInstanceUID(sopInstanceUid);
                var imagesToBeLoad = images.Take(imageCountInOneBoard).ToList();
                foreach (var image in imagesToBeLoad)
                {
                    ImageCell.DataLoader.LoadSopByUid(image.SOPInstanceUID);
                    //Console.WriteLine("PreLoad Image : {0}", image.SOPInstanceUID);
                }
            }
                );
        }


        private readonly ObservableCollection<Segment> _segments = new ObservableCollection<Segment>();
        private SelectableList<Page> _pages;
        private SelectableList<ImageCell> _cells;

        public void NewPage()
        {
            AppendSegment();
            BuildPageLink();
        }

        private void BuildPageLink()
        {
            var pages = new List<Page>();
            foreach (var segment in _segments)
            {
                segment.Pages.ToList().ForEach(p => p.IsBreak = false);
                segment.Pages.Last().IsBreak = true;
                pages.AddRange(segment.Pages);
            }
            pages.Last().IsBreak = true;

            _pages = new SelectableList<Page>(pages);

            BuildCellLink();
        }

        private void BuildCellLink()
        {
            _cells = new SelectableList<ImageCell>(_pages.SelectMany(p => p.Cells));

            // TODO-Later: Update Card.NotifyBoardChanged()
            NotifyBoardChanged();
        }

        private void AppendSegment()
        {
            var segment = _segments.LastOrDefault();
            var layout = segment == null ? DataModel.Layout.CreateDefaultLayout() : segment.Pages.LastOrDefault().Layout;
            _segments.Add(new Segment(layout, new List<ImageCell>()));
        }

        #endregion [--Implement From IImageLoader--]
    }

    #region EventArgs

    public class BoardEventArgs : EventArgs
    {
        public BoardEventArgs(IEnumerable<Page> pages)
        {
            Pages = pages;
        }

        public IEnumerable<Page> Pages { get; private set; }
    }

    public class IntEventArgs : EventArgs
    {
        public int Int { get; set; }
    }

    public class BoolEventArgs : EventArgs
    {
        public bool Bool { get; set; }
    }

    public class PageEventArgs : EventArgs
    {
        public PageEventArgs(int pageNo, IList<ImageCell> cells)
        {
            PageNo = pageNo;
            Cells = cells;
        }

        public int PageNo { get; private set; }
        public IList<ImageCell> Cells { get; private set; }
    }

    #endregion //EventArgs
}