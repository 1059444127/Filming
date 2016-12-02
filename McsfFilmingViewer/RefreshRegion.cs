using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming
{
    internal class RefreshRegion
    {
        #region [--Fields--]

        private IList<FilmingPageControl> _pages;
        private FilmingLayoutCell _startLayoutCell;
        private MedViewerControlCell _startControlCell;
        private IList<MedViewerControlCell> _replaceControlCells;

        private List<DisplayData> _displayDataToBeMoveForward;
        private List<FilmingLayoutCellModel> _layoutCellsToBeMoveForward;
        private List<FilmingLayoutCellModel> _backupedLayoutCellModels;
        private IList<FilmingLayoutCell> _multiFormatCells;
        private List<FilmingLayoutCell> _backupLayoutCells;

        #endregion //[--Fields--]

        

        public RefreshRegion(IList<FilmingPageControl> pages, FilmingLayoutCell startLayoutCell, MedViewerControlCell startControlCell, IList<MedViewerControlCell> replaceControlCells)
        {
            _pages = pages;
            _startLayoutCell = startLayoutCell;
            _startControlCell = startControlCell;
            _replaceControlCells = replaceControlCells;
        }

        //todo: refer to Repack Region
        public void Refresh()
        {
            //1. backup layoutCells ������Щcell? ����ñ������ݽṹ FilmingLayoutCellModel, �ο�һ��backupCells������, ���ݴ�startLayoutCell��ʼ��LayoutCell(����startLayoutCell)
            BackupCells(); //�õ�BackupLayoutCellModels֮�󣬾Ϳ���һ��һ���滻��

            ////2. replace control cells ( maybe new layout cells) (��������ͬ��multiformat cells��
            //InsertCells();

            //3. refresh backuped LayoutCells
            RefreshBackupCells();
        }

        private void RefreshBackupCells()
        {
            //Refresh Old layoutCells
            RefreshLayoutCellsByModels(_backupLayoutCells, _backupedLayoutCellModels);

            //New page and LayoutCells
            var loadingTargetPage = _pages.Last();
            var rootCell = loadingTargetPage.RootCell;
            var layoutCellCountInPage = rootCell.Children.Count();
            int pageCount = _backupedLayoutCellModels.Count/layoutCellCountInPage;
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            var newPages = new List<FilmingPageControl>();
            for (int i = 0; i < pageCount; i++)
            {
                filmingCard.Dispatcher.Invoke(new Action(() => newPages.Add(filmingCard.InsertFilmPage(loadingTargetPage.FilmPageIndex + 1+ i))));
            }
            var layoutCells = newPages.SelectMany(p => p.RootCell.Children.OfType<FilmingLayoutCell>());

            RefreshLayoutCellsByModels(layoutCells, _backupedLayoutCellModels);


        }

        private void RefreshLayoutCellsByModels(IEnumerable<FilmingLayoutCell> layoutCells, IList<FilmingLayoutCellModel> layoutCellModels)
        {
            foreach (var layoutCell in layoutCells)
            {
                if (layoutCellModels.Count == 0) break;
                var layoutCellModel = layoutCellModels.FirstOrDefault();
                layoutCell.ReplaceBy(layoutCellModel);
                layoutCellModels.Remove(layoutCellModel);
            }
        }

        //private void InsertCells()
        //{
        //    //1.�滻���еĶ�ָ��е�cell
            

        //    //2.�½���ָ�֮���滻��ָ��е�cell

        //    //3.ѡ��ճ����cell
        //}

        private void BackupCells()
        {

            //1.backup all displayData
            _backupLayoutCells =
                _pages.SelectMany(p => p.RootCell.Children.OfType<FilmingLayoutCell>()).Skip(
                    _startLayoutCell.CellIndexUnderParentCell).ToList();
            var layoutCellCollectionModel = new LayoutCellCollectionModel(_backupLayoutCells);
            layoutCellCollectionModel.InsertCells(_replaceControlCells, _startControlCell.CellIndexUnderParentCell);
            _backupedLayoutCellModels = layoutCellCollectionModel.LayoutCellModels;
        }

        ///// <summary>
        ///// BackupCells and New CellImpl to contain new cell
        ///// </summary>
        //private void BackupCells1()
        //{
        //    _displayDataToBeMoveForward = new List<DisplayData>();
        //    _layoutCellsToBeMoveForward = new List<FilmingLayoutCellModel>();

        //    FilmingPageControl endPageOfMultiFormatCells;
        //    FilmingLayoutCell endLayoutCellAfterMultiFormatCells;
        //    _multiFormatCells = GetLinkedMultiFormatCells(out endPageOfMultiFormatCells, out endLayoutCellAfterMultiFormatCells);
        //    //var controlCells = new List<FilmingControlCell>();
        //    //foreach (var filmingLayoutCell in multiFormatCells)
        //    //{
        //    //    controlCells.AddRange(filmingLayoutCell.Children.OfType<FilmingControlCell>());
        //    //}
        //    int indexOfDropCell = _startControlCell.CellIndexUnderParentCell;
        //    var controlCells =
        //        _multiFormatCells.SelectMany(layoutCell => layoutCell.Children.OfType<FilmingControlCell>()).Skip(indexOfDropCell).ToList();
        //    int replaceCellCount = _replaceControlCells.Count;
        //    int firstNonEmptyCellIndex = controlCells.FindIndex(c => c != null && !c.IsEmpty);
        //    int lastNonEmptyCellIndex = controlCells.FindLastIndex(c => c != null && !c.IsEmpty);

        //    int headEmptyCellCount = firstNonEmptyCellIndex == -1 ? controlCells.Count : firstNonEmptyCellIndex;

        //    //���һ�� ��ͷ�Ŀո��㹻, ����Ҫ�ƶ��κ�cell
        //    if (headEmptyCellCount >= replaceCellCount) return;

        //    var controlCellsToBeMoveForward = controlCells.GetRange(firstNonEmptyCellIndex,
        //                                                         lastNonEmptyCellIndex - firstNonEmptyCellIndex + 1);
        //    _displayDataToBeMoveForward = controlCellsToBeMoveForward.ConvertAll(c=>c.Image.CurrentPage);

        //    int tailEmptyCellCount = controlCells.Count - 1 - lastNonEmptyCellIndex;

        //    //������� ��ͷ�ͽ�β�Ŀո��㹻����Ҫ�������Ķ�ָ�cell���ƶ�cell
        //    int emptyControlCellCount = headEmptyCellCount + tailEmptyCellCount;
        //    if (emptyControlCellCount >= replaceCellCount) return;

        //        //������Ҫ�½����ٸ���ָ�
        //    int newMultiFormatCellCount = (replaceCellCount - emptyControlCellCount)/_startLayoutCell.DisplayCapacity;

        //    //todo: getlinkedLayoutCell to backup

        //    //������� �ո񲻹�����Ҫ�½���ָ������µĽ�Ƭ������ ���ο�BackupCells��
        //    //���ʱ����Ҫ��ͬ���ķ�ʽ������LayoutCell��ͬ��������������
            
        //    var layoutCellsAfterMultiFormatCells = GetlinkedLayoutCells(endPageOfMultiFormatCells,
        //                                                                endLayoutCellAfterMultiFormatCells);

        //    int firstNonEmptyLayoutCellIndex = layoutCellsAfterMultiFormatCells.FindIndex(c => !c.IsEmpty());
        //    int headEmptyLayoutCellCount = firstNonEmptyCellIndex == -1
        //                                       ? layoutCellsAfterMultiFormatCells.Count
        //                                       : firstNonEmptyCellIndex;

        //    //���1�� ��ͷ�Ŀո��㹻������Ҫ�ƶ��κ�cell
        //    if (headEmptyLayoutCellCount >= newMultiFormatCellCount) return;
        //    //���2�� ��ͷ�ͽ�β�Ŀո��㹻����Ҫ��������LayoutCell���ƶ�
        //    //���3�� �ո񲻹�����Ҫ�½���Ƭ

        //    int lastNonEmptyLayoutCellIndex = layoutCellsAfterMultiFormatCells.FindLastIndex(c => !c.IsEmpty());
        //    var backupLayoutCells = layoutCellsAfterMultiFormatCells.GetRange(firstNonEmptyLayoutCellIndex,
        //                                                                            lastNonEmptyLayoutCellIndex -
        //                                                                            firstNonEmptyCellIndex + 1);
        //    _layoutCellsToBeMoveForward = backupLayoutCells.ConvertAll(c=>new FilmingLayoutCellModel(c));
        //}

        //private DisplayData GetDisplayDataOf(FilmingControlCell cell)
        //{
        //    if (cell == null || cell.Image == null) return null;
        //    return cell.Image.CurrentPage;
        //}

        //private FilmingLayoutCellModel CreateFilmingLayoutCellModel(FilmingLayoutCell cell)
        //{
        //    return new FilmingLayoutCellModel(cell);
        //}

        private List<FilmingLayoutCell> GetlinkedLayoutCells(FilmingPageControl startPage, FilmingLayoutCell startLayoutCell)
        {
            var linkedPage = _pages.Skip(_pages.IndexOf(startPage)).ToList();
            var linkedLayoutCells = linkedPage.SelectMany(p=>p.RootCell.Children.OfType<FilmingLayoutCell>()).Skip(startLayoutCell.CellIndexUnderParentCell).ToList();
            return linkedLayoutCells;
        }

        private IList<FilmingLayoutCell> GetLinkedMultiFormatCells(out FilmingPageControl endPage, out FilmingLayoutCell endLayoutCell)
        {
            var page = _pages.FirstOrDefault();
            var layoutCells = page.RootCell.Children;
            var layoutCellCount = layoutCells.Count();
            FilmingLayoutCell layoutCell = null;

            var multiFormatCells = new List<FilmingLayoutCell>();
            multiFormatCells.Add(_startLayoutCell);

            bool endFlag = false;
            for (int i = _startLayoutCell.CellIndexUnderParentCell; i < layoutCellCount; i++)
            {
                layoutCell = layoutCells.ElementAt(i) as FilmingLayoutCell;
                if (!layoutCell.HasSameLayoutWith(_startLayoutCell))
                {
                    endFlag = true; 
                    break; 
                }
                multiFormatCells.Add(layoutCell);
            }

            if (!endFlag)
            {
                var pageCount = _pages.Count;
                for (int j = 1; j < pageCount; j++)
                {
                    page = _pages.ElementAt(j);
                    layoutCells = page.RootCell.Children;
                    layoutCellCount = layoutCells.Count();
                    for (int i = 0; i < layoutCellCount; i++)
                    {
                        layoutCell = layoutCells.ElementAt(i) as FilmingLayoutCell;
                        if (!layoutCell.HasSameLayoutWith(_startLayoutCell))
                        {
                            endFlag = true;
                            break;
                        }
                        multiFormatCells.Add(layoutCell);
                    }
                    if(endFlag) break;
                }
            }

            endPage = page;
            endLayoutCell = layoutCell;

            return multiFormatCells;
        }
    }

    internal class LayoutCellCollectionModel
    {
        public LayoutCellCollectionModel(List<FilmingLayoutCell> layoutCells)
        {
            LayoutCellModels = layoutCells.ConvertAll(c => new FilmingLayoutCellModel(c));
        }

        public List<FilmingLayoutCellModel> LayoutCellModels { get; private set; }


        public void InsertCells(IList<MedViewerControlCell> controlCells, int cellIndex)
        {
            //1.����λ�ã���ָ�λ��Index��Layout���� 
            Debug.Assert(LayoutCellModels.Count>0);
            var firstLayoutCellModel = LayoutCellModels.First();

            var firstDifferentLayoutIndex = LayoutCellModels.FindIndex(m=>m.Layout)
            var multiFormatCellModelOfSameLayout = LayoutCellModels.GetRange()

            //2.ƽ�����е�displayData���õ�����

            //3.�滻controlCells

            //4.����Layout���ϣ���ȡlayoutCellModels
        }
    }

    public class FilmingLayoutCellModel
    {
        public FilmingLayoutCellModel(FilmingLayoutCell cell)
        {
            DisplayDatas = cell.Children.OfType<FilmingControlCell>().ToList().ConvertAll(c => c.Image.CurrentPage);
            Layout = new FilmLayout(cell.Rows, cell.Columns);
        }

        public FilmLayout Layout { get; private set; }

        public List<DisplayData> DisplayDatas { get; private set; }
    }
}