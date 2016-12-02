

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.AppControls.Viewer.Primitives;
using UIH.Mcsf.Core;
using UIH.Mcsf.Database;
using UIH.Mcsf.Filming.Command;
//using UIH.Mcsf.AppControls.Viewer.MiniPA;
using UIH.Mcsf.Controls;
using UIH.Mcsf.AppControls.Viewer;
using UIH.Mcsf.Filming.ImageManager;
using UIH.Mcsf.Filming.Wrappers;
using UIH.Mcsf.MHC;
using UIH.Mcsf.MainFrame;

[assembly: XmlnsDefinition("http://UIH.Mcsf.Filming", "UIH.Mcsf.Filming")]

namespace UIH.Mcsf.Filming
{
    //public enum StudyListDisplayMode
    //{
    //    ListMode,
    //    IconMode,
    //}

    /// <summary>
    /// Interaction logic for StudySelectionCtrl.xaml
    /// </summary>
    public partial class StudySelectionCtrl
    {


        #region [--Image Loading Performance Optimization--]

        public class StudyListEventArgs : EventArgs
        {
            public StudyListEventArgs(IEnumerable<string> studyList)
            {
                StudyList = studyList;
            }
            public IEnumerable<string> StudyList { get; private set; }
        }

       // public event EventHandler<StudyListEventArgs> NewStudyAdded = delegate { };

        #endregion //[--Image Loading Performance Optimization--]



        public delegate void StudyListSelectionChanged();

        public event StudyListSelectionChanged StudyListSelectionChangedHandler;

        private void HandleSelectionChanged()
        {
            StudyListSelectionChanged handler = StudyListSelectionChangedHandler;
            if (handler != null)
            {
                handler();
            }
        }

        //Point _dragStartPoint;
        //Point _offsetPoint;        Point _adornerPostion;


        //public ReviewController Controller { get; set; }

        //internal SeriesDragDropAdorner Adorner { get; set; }

        private IList<SeriesTreeViewItemModel> _selectedSeriesItems = new List<SeriesTreeViewItemModel>();

        public IList<SeriesTreeViewItemModel> SelectedSeriesItems
        {
            get { return _selectedSeriesItems; }
            set { _selectedSeriesItems = value; }
        }

        //public Point OffsetPoint
        //{
        //    get { return _offsetPoint; }
        //    set { _offsetPoint = value; }
        //}

        //public Point AdornerPostion
        //{
        //    get { return _adornerPostion; }
        //    set { _adornerPostion = value; }
        //}

        //private FilmingMiniPA _filmingMiniPaWindow;
        //public FilmingMiniPA FilmingMiniPaWindow
        //{
        //    get
        //    {
        //        if (_filmingMiniPaWindow == null)
        //        {
        //            _filmingMiniPaWindow = new FilmingMiniPA();

        //        }
        //        return _filmingMiniPaWindow;
        //    }
        //}

        private StudyListViewModel _studyListViewModel;

        public StudyListViewModel StudyListViewModel
        {
            get { return _studyListViewModel; }
            set { _studyListViewModel = value; }
        }

        private string _originalStudyUid;

        private string Studystatus
        {
            get
            {
                var db = FilmingDbOperation.Instance.FilmingDbWrapper;
                var studyByStudyInstanceUid = db.GetStudyByStudyInstanceUID(_originalStudyUid ?? string.Empty);
                return studyByStudyInstanceUid == null ? string.Empty : studyByStudyInstanceUid.StudyFlag;
            }
        }
        /// <summary>
        /// Note: you must call Initialize() after new entity.!
        /// </summary>
        public StudySelectionCtrl()
        {
            InitializeComponent();

            _studyListViewModel = new StudyListViewModel(SeriesDescriptionDisplayMode.ShowSeriesDescription)
                                      {AllowSelectionCrossStudies = true, StudyListContextMenuVM = null};

            _studyListViewModel.SeriesSelectionChangedEvent += OnSeriesSelectionChange;

            //Action<SeriesItemChangedType, SeriesTreeViewItemModel> StudyListViewModel_SeriesItemChanged;
            _studyListViewModel.SeriesItemChanged += OnStudyListViewModel_SeriesItemChanged;

            filmingStudyTree.DataContext = _studyListViewModel;

            filmingStudyTree.SupportMultiSelection = true;
            var dict = AppNLSHelper.AppCommonNLSResource;
            if (dict != null)
            {
                filmingStudyTree.Resources.MergedDictionaries.Add(dict);
            }
           // System.Diagnostics.Debugger.Launch();
            filmingStudyTree.CanUpdateScroll = true;
            filmingStudyTree.SeriesDoubleClick += FilmingStudyTreeOnSeriesDoubleClick;
            //filmingStudyTree.AllowDrop = true;
         //   filmingStudyTree.PreviewDrop +=new DragEventHandler(filmingStudyTree_PreviewDrop);
            filmingStudyTree.AddHandler(TreeViewMultipleDragDropHelper.OnDropAcceptedEvent,
                                        new SelectedSeriesListEventHandler(HandleStudyListDropEvent));

            HandleSelectionChanged();

            //adjust content location of Button
            //var children = GetChildObjects<TextBlock>(btnLoad, typeof (TextBlock));
            //if (children.Count == 0) return;

            //var textBlock = children.FirstOrDefault();
            //if(textBlock != null)
            //    textBlock.Margin = new Thickness(0, 0, 0, 1);
//#if DEBUG
//            InitCompareBtn();
//#endif
        }

        private void HandleStudyListDropEvent(object sender, SelectedSeriesListEventArgs e)
        {
            try
            {
                FilmingViewerContainee.IsBusy = true;
                e.ProcessDragEvent(new LoadSeriesInfo()
                                       {
                                           TargetIsStudyList = sender is StudyTreeControl,
                                           CommunicationProxy = ComProxyManager.GetCurrentProxy(),
                                           StudyListViewModel = StudyListViewModel,
                                           SupportAcceptMultiStudy = true,
                                           NeedAppDataChecking = false,
                                           MoreSettingsForInteractionInfo = (builder) =>
                                                                                {
                                                                                    builder.SetDestAppName("FilmingCard");
                                                                                },
                                           NotifyBELoadSeries = (info) =>
                                                                    {
                                                                        
                                                                       
                                                                    },
                                         NotifyStudyListAdded=(info)=>
                                                                  {
                                                                      var filmingCard =
                                                                            FilmingViewerContainee.FilmingViewerWindow
                                                                            as FilmingCard;
                                                                      filmingCard.studyTreeCtrl.seriesSelectionCtrl.
                                                                          LoadStudies(
                                                                              info.DraggedInfoWrapper.GetStudyList().
                                                                                  Select(i => i.UID));
                                                                  }

                                       });
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                FilmingViewerContainee.IsBusy = false;
            }

        } 

        private void FilmingStudyTreeOnSeriesDoubleClick(SeriesTreeViewItemModel seriesTreeViewItemModel)
        {
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if(filmingCard == null || filmingCard.studyTreeCtrl.seriesSelectionCtrl.SelectedSeriesItems.Count>1) return;
            filmingCard.studyTreeCtrl.OnDrop(null);
        }

        private void OnStudyListViewModel_SeriesItemChanged(SeriesItemChangedType eventType, SeriesTreeViewItemModel seriesTreeViewItemModel)
        {
           // string studyUID = seriesTreeViewItemModel.SeriesDBData.StudyInstanceUIDFk;
            switch (eventType)
            {
                case SeriesItemChangedType.Added:
                case SeriesItemChangedType.Updated:
                    // Review3D Bookmark Series should Disable in Filming
                    seriesTreeViewItemModel.IsEnabled = !DbWrapExtentsions.IsBookmarkSeries(seriesTreeViewItemModel.SeriesDBData);
                    break;
            }
        }
      

        public void AppendStudyToFilmingCard(string level,List<string> list,string fromSource)
        {
           
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmingCard == null) return;
            var db = FilmingDbOperation.Instance.FilmingDbWrapper;
            var _dropFilmingPage =filmingCard._dropFilmingPage ;
            switch (level)
            {
                case "study":
                    {
                       
                        var studyUidList = list;
                        var dbSeriesList = new List<SeriesBase>();
                        studyUidList.ForEach(studyId => dbSeriesList.AddRange((db.GetSeriesListByConditionWithOrder(
                            "StudyInstanceUIDFk in ('" + studyId + "')", "SeriesNumber"))));
                        this.SelectSeries(
                            dbSeriesList.Select(seriesBase => seriesBase.SeriesInstanceUID).ToList());
                        filmingCard.studyTreeCtrl.OnDrop(_dropFilmingPage);

                        break;
                    }
                case "series":
                    {
                        SelectSeries(list);
                        filmingCard.studyTreeCtrl.OnDrop(_dropFilmingPage);
                        break;
                    }
                case "image":
                    {
                        var imageUids = list;
                        filmingCard.studyTreeCtrl.OnDropMultiImages(imageUids);
                        break;
                    }
                default:
                    break;
            }
           
        }



        private IEnumerable<string> _toBeLoadedStudies = new List<string>();
        //private MiniPAControl _miniPA;

        public void LoadStudies(IEnumerable<string> toBeLoadedStudies)
        {

            try
            {
                Logger.LogFuncUp();

                var lockedStudies = FilmingDbOperation.Instance.LockedStudyUidList;

                _toBeLoadedStudies = toBeLoadedStudies;

                //check if can load studies
                bool hasNewToBeLoaded = toBeLoadedStudies.Any((uid) => !lockedStudies.Contains(uid));
                if (!hasNewToBeLoaded)
                {
                    return;// LoadStudiesHandler();   //now studylist not reload studies
                }
                else
                {
                    //if (lockedStudies.Count() != 0 || toBeLoadedStudies.Count() > 1)
                    //{
                    //    MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Warning_AddDifferentPatientToSameFilmCard", LoadStudiesHandler);
                    //}
                    //else // toBeLoadedCount == 1 && lockedCount == 0
                    //{
                        LoadStudiesHandler();
                    //}

                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {

                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }

        }

        private void LoadStudiesHandler(MessageBoxResponse response = MessageBoxResponse.YES)
        {
            if (response != MessageBoxResponse.YES)
            {
                return;
            }
            try
            {
                Logger.LogFuncUp();

                #region [--Image Loading Performance Optimization--]

              //  NewStudyAdded(this, new StudyListEventArgs(_toBeLoadedStudies));

                #endregion //[--Image Loading Performance Optimization--]

                

                var db = FilmingDbOperation.Instance.FilmingDbWrapper;

                foreach (var uid in _toBeLoadedStudies)
                {
                    _studyListViewModel.AddStudy(db.GetStudyByStudyInstanceUID(uid));
                    if (_originalStudyUid == null) _originalStudyUid = uid;
                    FilmingDbOperation.Instance.Lock(uid);

                    var serieses = db.GetSeriesListByStudyInstanceUID(uid).Where(s => s.ReconResult == 1);    //find series not recon completed
                    foreach (var seriesBase in serieses)
                    {
                        _studyListViewModel.RemoveSereis(seriesBase.SeriesInstanceUID);
                        Logger.LogWarning("Series" + seriesBase.SeriesInstanceUID + "No.:" + seriesBase.SeriesNumber + "ReconResult is 1 ,Recon does not complete!");
                    }
                    var loadedSeries = db.GetSeriesListByStudyInstanceUID(uid).Where(s => s.ReconResult != 1);
                    foreach (var series in loadedSeries)
                    {
                        var seriesResult = db.GetImageListBySeriesInstanceUID(series.SeriesInstanceUID);
                        if (seriesResult==null||seriesResult.Count == 0) continue;
                        var isHasSeries =seriesResult.All(image => (image!=null&&(image.Rows == 0 || image.Columns == 0)));
                        if (isHasSeries)
                        //禁掉全是非图像数据的系列
                        {
                            var seriesTreeViewItemModel = _studyListViewModel.GetSeriesItemByUID(series.SeriesInstanceUID);
                            seriesTreeViewItemModel.IsEnabled = false;
                            Logger.LogWarning("Series" + series.SeriesInstanceUID + "No.:" + series.SeriesNumber + "Only has dicoms those can not be print！");
                        }
                    }
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                //throw;
            }
        }

        private void OnSeriesSelectionChange(object sender, StudyTreeEventArgs e)
        {
            SelectedSeriesItems = e.SelectedSeries;
            //var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            //filmingCard.BtnEditCtrl.btnCompareSeries.IsEnabled = filmingCard.BtnEditCtrl.IsEnableCompareFilm;
            
            HandleSelectionChanged();
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmingCard != null) filmingCard.UpdateUIStatus();
        }

        public void SelectJust(string seriesUID)
        {
            try
            {
                Logger.LogFuncUp();
                _studyListViewModel.SetSeriesUnselected(SelectedSeriesItems.Select((series) => series.SeriesInstanceUID).ToList());
                var newSelectedSeries = new List<string>();
                newSelectedSeries.Add(seriesUID);
                _studyListViewModel.SetSeriesSelected(newSelectedSeries);
                SelectedSeriesItems = _studyListViewModel.SelectedSeries;
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        public void SelectSeries(List<string> seriesUiDs)
        {
            try
            {
                Logger.LogFuncUp();
                if (_studyListViewModel == null) return;
                var seriesList = seriesUiDs.Select(seriesUiD => _studyListViewModel.GetSeriesItemByUID(seriesUiD));
                bool isNotSupported = false;
                var seriesTreeViewItemModels = seriesList as SeriesTreeViewItemModel[] ?? seriesList.ToArray();
                if (seriesTreeViewItemModels.Any())
                {
                    isNotSupported = seriesTreeViewItemModels.Any(
                        seriesItem => (seriesItem != null && !seriesItem.IsEnabled));
                }

                if (isNotSupported)
                {
                    Logger.LogWarning("Maybe Request to load unsupported image");
                    Logger.Instance.LogSvcError(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcErrorImgTxtConfig,
                                                "[Fail to load an image for study]" +
                                                FilmingViewerContainee.Main.StudyInstanceUID);
                }

                _studyListViewModel.SetSeriesUnselected(SelectedSeriesItems.Select((series) => series.SeriesInstanceUID).ToList());
                _studyListViewModel.SetSeriesSelected(seriesUiDs);
                
                var disabledSeriesUids = _studyListViewModel.SelectedSeries.Where(s => !s.IsEnabled).Select(s => s.SeriesInstanceUID).ToList();

                var validateSeriesUids = seriesUiDs.Except(disabledSeriesUids).ToList();
                _studyListViewModel.SetSeriesUnselected(SelectedSeriesItems.Select((series) => series.SeriesInstanceUID).ToList());
                _studyListViewModel.SetSeriesSelected(validateSeriesUids);
                SelectedSeriesItems = _studyListViewModel.SelectedSeries;
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        ~StudySelectionCtrl()
        {
            try
            {
                Logger.LogInfo("On Filming Card unloaded study selection ctrl");
                Logger.Instance.LogTraceInfo("########Filming is unlocking When Filming Card unloaded study selection ctrl########");
                FilmingDbOperation.Instance.UnLock();
                Logger.Instance.LogTraceInfo("$$$$$$$$Filming has unlocked When Filming Card unloaded study selection ctrl$$$$$$$$");
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        /// <summary>
        /// Note: you must call Initialize() after new entity.!
        /// </summary>
        public void Initialize()
        {
            if (FilmingViewerContainee.FilmingResourceDict != null)
            {
                Resources.MergedDictionaries.Add(FilmingViewerContainee.FilmingResourceDict);
            }
        }


        public void ReplaceStudy(IEnumerable<string> toBeReplacedStudies)
        {
            try
            {
                Logger.LogFuncUp();

                //All loaded studies
                var lockedStudies = FilmingDbOperation.Instance.LockedStudyUidList;

                _toBeLoadedStudies = toBeReplacedStudies;
                //update studylistviewmodel
                if (_studyListViewModel.StudyCollection.Count == 1 && _studyListViewModel.StudyCollection[0].StudyInstanceUID == toBeReplacedStudies.ElementAt(0))
                {
                    return;
                }
                else
                {
                    var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                    filmingCard.commands.DeleteAllFilmPages();
                    _studyListViewModel.StudyCollection.Clear();
                }
                _originalStudyUid = toBeReplacedStudies.ElementAt(0);
                //check if can load studies
                bool hasNewToBeLoaded = toBeReplacedStudies.Any((uid) => !lockedStudies.Contains(uid));
                if (!hasNewToBeLoaded)
                {
                    var db = FilmingDbOperation.Instance.FilmingDbWrapper;
                    _studyListViewModel.AddStudy(db.GetStudyByStudyInstanceUID(toBeReplacedStudies.ElementAt(0)));
                    return;// LoadStudiesHandler();   //now studylist not reload studies
                }
                else
                {
                    LoadStudiesHandler();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {

                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

 

    }
    
}
