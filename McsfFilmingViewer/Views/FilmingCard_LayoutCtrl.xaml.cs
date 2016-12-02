using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UIH.Mcsf.Database;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.CustomizeLayout;
using UIH.Mcsf.Filming.ImageManager;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Filming.Wrappers;
using UIH.Mcsf.MHC;
using UIH.Mcsf.Utility;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for FilmingCard_ButtonGrid.xaml
    /// </summary>
    public partial class LayoutCtrl : INotifyPropertyChanged
    {
        private FilmingCard Card { get; set; }
        //private CustomCellLayoutWindow _cellLayoutWindow;
        private string curModality;
        private FilmLayout CurrentCustomCellLayout = null;
        private Dictionary<string, ImageSource> SelectedCellLayoutIco = new Dictionary<string,ImageSource>();
        private Dictionary<string, ImageSource> SelectedViewPortLayoutIco = new Dictionary<string, ImageSource>();

        private ObservableCollection<int> _filmCardDisplayModeCollection =
          new ObservableCollection<int>();

        public ObservableCollection<int> FilmCardDisplayModeCollection
        {
            get { return _filmCardDisplayModeCollection; }
            set
            {
                _filmCardDisplayModeCollection = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FilmCardDisplayModeCollection"));
            }
        }

        private ObservableCollection<FilmLayout> _filmCellLayoutCollection = new ObservableCollection<FilmLayout>();

        public ObservableCollection<FilmLayout> FilmCellLayoutCollection
        {
            get { return _filmCellLayoutCollection; }
            set
            {
                _filmCellLayoutCollection = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FilmCellLayoutCollection"));
            }
        }

      
        private readonly ObservableCollection<FilmLayout> _viewportLayoutCollection =
           new ObservableCollection<FilmLayout>();

        public ObservableCollection<FilmLayout> ViewportLayoutCollection
        {
            get { return _viewportLayoutCollection; }
        }

        private readonly ObservableCollection<FilmLayout> _defaultViewportLayoutCollection =
            new ObservableCollection<FilmLayout>();

        public ObservableCollection<FilmLayout> DefaultViewportLayoutCollection
        {
            get { return _defaultViewportLayoutCollection; }
        }



        //public CustomCellLayoutWindow CustomCellLayoutWindow
        //{
        //    get { return _cellLayoutWindow ?? (_cellLayoutWindow = new CustomCellLayoutWindow()); }
        //}

        private readonly List<FilmLayout> DefaultButtonCellLayout = new List<FilmLayout>
                                                                        {
                                                                            new FilmLayout(3, 2),
                                                                            new FilmLayout(4, 3),
                                                                            new FilmLayout(5, 4)
                                                                        };


        public LayoutCtrl(FilmingCard _card, string modality)
        {
            InitializeComponent();
            Card = _card;
            curModality = modality.ToUpper();
           // InitBtnLayout();
            this.filmingbuttonGird1.DataContext = this;
            InitCustomCellLayout();
            InitCustomViewport();

            SavedFilmPageControl.Initialize();
            SavedFilmPageControl.FilmPageTitleVisibility = Visibility.Collapsed;
        }

        public void ChangeBtnViewportLayout(int index, FilmLayout layout)
        {
            if (DefaultViewportLayoutCollection.Any(l => l.LayoutName == layout.LayoutName))
            {
                var templayout = DefaultViewportLayoutCollection.FirstOrDefault(l => l.LayoutName == layout.LayoutName);
                var tempIndex = DefaultViewportLayoutCollection.IndexOf(templayout);
                var newLayout = DefaultViewportLayoutCollection[index];
                DefaultViewportLayoutCollection[tempIndex] = newLayout;
                UpdateViewportLayoutPanel(tempIndex, newLayout.LayoutName);
                if (Printers.Instance.PresetViewportLayouts.Count >= tempIndex)
                    Printers.Instance.PresetViewportLayouts[tempIndex - 1].SetViewPortLayout(tempIndex, newLayout.LayoutName, "Origin");
            }
            DefaultViewportLayoutCollection[index] = layout;
            UpdateViewportLayoutPanel(index, layout.LayoutName);
            if (Printers.Instance.PresetViewportLayouts.Count >= index)
                Printers.Instance.PresetViewportLayouts[index - 1].SetViewPortLayout(index, layout.LayoutName, "Origin");
        }

        private void UpdateViewportLayoutPanel(int index,string layoutName)
        {
            var findBtnName = "viewportLayoutButton" + index;
            var findBtn = new Button();
            bool isFindBtn = false;
            foreach (var item in viewportLayoutPanel.Children)
            {
                var itemBtn = item as Button;
                if (itemBtn != null)
                {
                    if (itemBtn.Name == findBtnName)
                    {
                        findBtn = itemBtn;
                        isFindBtn = true;
                        break;
                    }
                }
            }
            if (isFindBtn)
            {
                SetViewportLayoutButtonAttribute(findBtn, layoutName);
                SetViewportLayoutWindowAttribute(index, layoutName);
            }
        }

        private void SetViewportLayoutButtonAttribute(Button btn,string layoutName)
        {
            try
            {
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                string imgPath = string.Format(layoutName);
                img.Source = Card.FindResource(imgPath) as ImageSource;
                btn.Content = img;
                btn.ToolTip = layoutName;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void SetViewportLayoutWindowAttribute(int index, string layoutName)
        {
            var cancleItem = CustomViewportLayoutWindow.CustomViewportViewModel.CustomViewportItemCollectionOrigin.FirstOrDefault(
                        i => i.IndexLabelContent == index);
            if (cancleItem != null)
            {
                cancleItem.IndexLabelContent = -1;
                cancleItem.IndexLableVisibility = Visibility.Collapsed;
            }
            var settedItem = CustomViewportLayoutWindow.CustomViewportViewModel.CustomViewportItemCollectionOrigin.FirstOrDefault(
                i => i.CustomViewportName == layoutName);
            if (settedItem != null)
            {
                settedItem.IndexLabelContent = index;
                settedItem.IndexLableVisibility = Visibility.Visible;
            }
        }
        
        //private void InitBtnLayout()
        //{
           
        //    if (curModality == "MG")
        //    {
        //       // cellLayoutGrid.Visibility = Visibility.Collapsed;
        //       // filmViewportLayoutComboBox.Visibility = Visibility.Collapsed;
        //        //自定义视窗布局按钮
        //        filmCustomViewport.Visibility = Visibility.Collapsed;
        //      //  btnFilmingvLayout.Visibility = Visibility.Collapsed;
        //        filmCustomViewportLayout.Visibility = Visibility.Collapsed;
        //        //modify location
        //        displayGrid.Margin = new Thickness(0, 54, 0, 0);    //14-54 right
        //        viewportLayoutGrid.Margin = new Thickness(0, 51, 0, 0); //11-51
        //        //rename label
        //       // viewportLayoutLabel.Content = Card.Resources["UID_Filming_Layout"] as string;
        //        btnSaveFilmLayout.Visibility = Visibility.Collapsed;
        //    }
        //}

        private void InitCustomCellLayout()
        {
            CustomCellLayoutWindow = new CustomCellLayoutWindow();
            CustomCellLayoutWindow.RaiseCustomCellLayoutChanged += OnCustomCellLayoutChanged;
            CustomCellLayoutWindow.Initialize();
        }

        private void InitCustomViewport()
        {
            CustomViewportWindow = new CustomViewportWindow();
            CustomViewportWindow.RaiseCustomViewportChanged += OnCustomViewportChanged;
            CustomViewportWindow.Initialize();
        }

        public void InitLayout()
        {
            try
            {
                Logger.LogFuncUp();
                //init cell layout
                FilmCellLayoutCollection.Add(new FilmLayout(1, 1));
                FilmCellLayoutCollection.Add(new FilmLayout(2, 2));
                FilmCellLayoutCollection.Add(new FilmLayout(3, 3));
                FilmCellLayoutCollection.Add(new FilmLayout(4, 4));
                FilmCellLayoutCollection.Add(new FilmLayout(6, 4));
                FilmCellLayoutCollection.Add(new FilmLayout(6, 5));
              //  System.Diagnostics.Debugger.Launch();
                foreach (var filmLayout in FilmLayout.DefinedFilmRegionLayoutList)
                {
                    if (curModality == "MG" || curModality == "DBT") filmLayout.LayoutType = LayoutTypeEnum.RegularLayout;
                    ViewportLayoutCollection.Add(filmLayout);
                }
                //DefaultViewportLayoutCollection.Clear();
                if (curModality == "MG")
                {
                    FilmLayout layout = ViewportLayoutCollection.ElementAt(0);
                    layout = new FilmLayout(1, 1, layout.LayoutName);
                    layout.LayoutType = LayoutTypeEnum.RegularLayout;
                    DefaultViewportLayoutCollection.Add(layout);
                    layout = ViewportLayoutCollection.ElementAt(1);
                    DefaultViewportLayoutCollection.Add(layout);
                    layout = ViewportLayoutCollection.ElementAt(2);
                    DefaultViewportLayoutCollection.Add(layout);
                }
                else if (curModality == "DBT")
                {
                    FilmLayout layout = ViewportLayoutCollection.ElementAt(0);
                    layout = new FilmLayout(1, 1, layout.LayoutName);
                    layout.LayoutType = LayoutTypeEnum.RegularLayout;
                    DefaultViewportLayoutCollection.Add(layout);
                    layout = ViewportLayoutCollection.ElementAt(1);
                    DefaultViewportLayoutCollection.Add(layout);
                    layout = ViewportLayoutCollection.ElementAt(2);
                    DefaultViewportLayoutCollection.Add(layout);
                    layout = FilmLayout.CreateDefinedLayout(4, 4);
                    layout.LayoutType = LayoutTypeEnum.RegularLayout;
                    DefaultViewportLayoutCollection.Add(layout);
                    layout = FilmLayout.CreateDefinedLayout(5, 5);
                    layout.LayoutType = LayoutTypeEnum.RegularLayout;
                    DefaultViewportLayoutCollection.Add(layout);
                }
                else
                {
                    FilmLayout layout = ViewportLayoutCollection.ElementAt(0);
                    DefaultViewportLayoutCollection.Add(layout);
                    for (int i = 1; i < 7; i++)
                    {
                        if (i <=Printers.Instance.PresetViewportLayouts.Count)
                        {
                            var viewportLayout= Printers.Instance.PresetViewportLayouts[i-1];
                            layout = ViewportLayoutCollection.FirstOrDefault(v => v.LayoutName == viewportLayout.Name);
                        }
                        else
                        {
                            layout = ViewportLayoutCollection.ElementAt(i);
                        }
                        if (layout != null)
                            DefaultViewportLayoutCollection.Add(layout);
                    }

                    int index = 0;
                    foreach (var viewportLayout in DefaultViewportLayoutCollection)
                    {
                        UpdateViewportLayoutPanel(index, viewportLayout.LayoutName);
                        index++;
                    }
                }
                
                //init display mode
                FilmCardDisplayModeCollection.Add(1);
                FilmCardDisplayModeCollection.Add(2);
                FilmCardDisplayModeCollection.Add(3);
                FilmCardDisplayModeCollection.Add(4);
                FilmCardDisplayModeCollection.Add(6);
                FilmCardDisplayModeCollection.Add(8);

                Card.filmPageGrid.RowDefinitions.Add(new RowDefinition());
                Card.filmPageGrid.ColumnDefinitions.Add(new ColumnDefinition());

                Card.GotoFilmBoardWithIndex(0);

                this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(InitLayoutIcon));
                

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private void InitLayoutIcon()
        {
            
        }


        public void DisplayModeChanged(int mode)
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + mode);

                int row, column;
                switch (mode)
                {
                    case 1:
                        row = 1;
                        column = 1;
                        break;
                    case 2:
                        row = 1;
                        column = 2;
                        break;
                    case 3:
                        row = 1;
                        column = 3;
                        break;
                    case 4:
                        row = 2;
                        column = 2;
                        break;
                    case 6:
                        row = 2;
                        column = 3;
                        break;
                    case 8:
                        row = 2;
                        column = 4;
                        break;
                    default:
                        row = 1;
                        column = 1;
                        Logger.LogError("Unsupported display mode! " + mode.ToString(CultureInfo.InvariantCulture));
                        break;
                }

                SetDisplayModeLayout(row, column);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        //todo: performance optimization begin page flipping
        private void SetDisplayModeLayout(int row, int column)
        {
            try
            {
                Logger.LogFuncUp();

                Card._filmPlates.ForEach(p => p.Clear());

                var children = Card.filmPageGrid.Children;
                children.Clear();

                Card.filmPageGrid.RowDefinitions.Clear();
                Card.filmPageGrid.ColumnDefinitions.Clear();

                for (int i = 0; i < row; i++)
                {
                    Card.filmPageGrid.RowDefinitions.Add(new RowDefinition());
                }

                for (int j = 0; j < column; j++)
                {
                    Card.filmPageGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for (int i = 0, plateIndex = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        var plate = Card._filmPlates[plateIndex];
                        plateIndex++;
                        Grid.SetRow(plate, i);
                        Grid.SetColumn(plate, j);
                        children.Add(plate);
                    }
                }

                //re-show the film page control
                Card.ReOrderFilmPage();

                Card.Dispatcher.Invoke(new Action(()=>{Card.UpdateLayout();}));
                //for DIM wish:ID:115730
                var firstSelectedFilmPage = Card.ActiveFilmingPageList.FirstOrDefault();

                int boardIndex = firstSelectedFilmPage != null ? GetFilmingPageBoardIndex(firstSelectedFilmPage) : 0;
                Card.GotoFilmBoardWithIndex(boardIndex);

                //EntityFilmingPageList.UpdatePageLabel();
               
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }


        public int _selectedFilmCardDisplayMode;

        public int SelectedFilmCardDisplayMode
        {
            get { return _selectedFilmCardDisplayMode; }
            set
            {
                try
                {
                    Logger.LogFuncUp();
                    if (_selectedFilmCardDisplayMode != value && FilmCardDisplayModeCollection.Contains(value))
                    {
                        Logger.Instance.LogPerformanceRecord("[Begin][change to displayMode] " + _selectedFilmCardDisplayMode);
                        _selectedFilmCardDisplayMode = value;
                        DisplayModeChanged(value);
                        Card.UpdateFilmCardScrollBar();
                        OnPropertyChanged(new PropertyChangedEventArgs("SelectedFilmCardDisplayMode"));
                        Logger.Instance.LogPerformanceRecord("[End][change to displayMode]" + _selectedFilmCardDisplayMode);
                        Printers.Instance.DisplayMode = value;
                    }
                    Logger.LogFuncDown();
                }
                catch (Exception ex)
                {
                    Logger.LogFuncException(ex.Message + ex.StackTrace);
                }
            }
        }


        private int GetFilmingPageBoardIndex(FilmingPageControl filmingPage)
        {
            try
            {
                Logger.LogFuncUp();

                int currentFilmPageIndex = Card.EntityFilmingPageList.IndexOf(filmingPage);

                if (Card.FilmingCardRows == 0 || Card.FilmingCardColumns == 0)
                    return 0;

                int filmPageBoardIndex = (currentFilmPageIndex) / (Card.FilmingCardRows * Card.FilmingCardColumns);

                Logger.LogFuncDown();

                return filmPageBoardIndex;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }


        private void OnCustomCellLayoutChanged(object sender, CustomCelllLayoutChangeEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                CellLayoutActiveFilmingPages(e.CustomFilmLayout);

                Card.UpdateUIStatus();

                CurrentCustomCellLayout = e.CustomFilmLayout;
               // SetLayoutUIOnControlPanel(Card.LastSelectedFilmingPage.ViewportLayout, CurrentCustomCellLayout);
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnCustomViewportChanged(object sender, CustomViewportChangeEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    ViewportLayoutActiveFilmingPages(e.CustomFilmViewport);
                }), DispatcherPriority.Background);

                Card.UpdateUIStatus();

                //CurrentCustomCellLayout = e.CustomFilmViewport;
                //SetLayoutUIOnControlPanel(Card.LastSelectedFilmingPage.ViewportLayout, CurrentCustomCellLayout);
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
        
        private void OnCellLayout3X2Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                if (DefaultButtonCellLayout.Count > 0)
                {
                    CellLayoutActiveFilmingPages(DefaultButtonCellLayout[0]);

                    Card.UpdateUIStatus();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnCellLayout4X3Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                if (DefaultButtonCellLayout.Count > 1)
                {
                    CellLayoutActiveFilmingPages(DefaultButtonCellLayout[1]);

                    Card.UpdateUIStatus();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnCellLayout5X4Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                if (DefaultButtonCellLayout.Count > 2)
                {
                    CellLayoutActiveFilmingPages(DefaultButtonCellLayout[2]);

                    Card.UpdateUIStatus();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        //private void OnCurrentCellLayoutClick(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();
        //        FilmLayout layout = GetBoxSelectLayout(FilmCellLayoutCollection, cellLayoutCombox);
        //        if (null != layout)
        //        {
        //        //    if (layout.LayoutColumnsSize == -1 && layout.LayoutRowsSize == -1)
        //        //    {
        //                //CustomCellLayoutWindow.Initialize();
        //                //ShowCustomCellLayoutWindow();
        //            //}
        //            //else
        //            //{
        //            CellLayoutActiveFilmingPages(layout);
        //            Card.UpdateUIStatus();
        //            //}
        //        }
        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message + ex.StackTrace);
        //    }
        //}

        public CustomViewportWindow CustomViewportWindow;
        private void ShowCustomViewportWindow()
        {

            var messageWindow = new MessageWindow
            {
                WindowTitle = Card.Resources["UID_Filming_CustomViewport_Setting_Title"] as string,//Card.Resources["UID_Filming_CustomCellLayout_Setting_Title"] as string,
                WindowChild = CustomViewportWindow,
                WindowDisplayMode = WindowDisplayMode.Default
            };
            messageWindow.Closing += new CancelEventHandler(OnCustomViewportWindowClosing);
            Card.HostAdornerCount++;
            messageWindow.ShowModelDialog();
            Card.HostAdornerCount--;
        }
        public CustomCellLayoutWindow CustomCellLayoutWindow;
        private void ShowCustomCellLayoutWindow()
        {

            //WindowHostManagerWrapper.ShowSecondaryWindow(CustomCellLayoutWindow,
            //                                 Card.Resources["UID_Filming_CustomCellLayout_Setting_Title"] as string);

            var messageWindow = new MessageWindow
            {
                WindowTitle = Card.Resources["UID_Filming_CustomCellLayout_Setting_Title"] as string,//Card.Resources["UID_Filming_CustomCellLayout_Setting_Title"] as string,
                WindowChild = CustomCellLayoutWindow,
                WindowDisplayMode = WindowDisplayMode.Default
            };
            messageWindow.Closing += new CancelEventHandler(OnCustomCellLayoutWindowClosing);
            Card.HostAdornerCount++;
            messageWindow.ShowModelDialog();
            Card.HostAdornerCount--;
        }

        void OnCustomCellLayoutWindowClosing(object sender, CancelEventArgs e)
        {
            CustomCellLayoutWindow.CloseProtoclWindow(sender, e);
        }

        void OnCustomViewportWindowClosing(object sender, CancelEventArgs e)
        {
            CustomViewportWindow.CloseProtoclWindow(sender,e);
        }


        private void OnCustomCellLayoutClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                ShowCustomCellLayoutWindow();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

       

        public FilmingMIPresetCellLayoutPanel PresetLayoutPanel = null;
        public void SetMIPreSetCellLayoutButton()
        {
            try
            {
                Logger.LogFuncUp();
                preSetCellLayoutButtonGrid.Children.RemoveAt(0);

                PresetLayoutPanel = new FilmingMIPresetCellLayoutPanel();
                Grid.SetColumn(PresetLayoutPanel, 0);
                preSetCellLayoutButtonGrid.Children.Add(PresetLayoutPanel);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }
        public void UpdateMIPresetCellLayoutButton(int index, int row, int col)
        {
            if (PresetLayoutPanel != null)
            {
                PresetLayoutPanel.UpdateButtonStatus(index, row, col);
            }
        }

       


        //private void SetLayoutUIOnControlPanel(FilmLayout vpLayout, FilmLayout cellLayout)
        //{
        //    bool VPLayoutFound = false;
        //    bool CellLayoutFound = false;
        //    for (int i = 0; i < DefaultViewportLayoutCollection.Count; i++)
        //    {
        //        if (FilmPageUtil.FilmLayoutEquals(vpLayout, DefaultViewportLayoutCollection[i]))
        //        {
        //            VPLayoutFound = true;
        //            break;
        //        }
        //    }
        //    if (false == VPLayoutFound)
        //    {
        //        for (int i = 0; i < ViewportLayoutCollection.Count; i++)
        //        {
        //            if (FilmPageUtil.FilmLayoutEquals(vpLayout, ViewportLayoutCollection[i]))
        //            {
        //                filmViewportLayoutComboBox.SelectedIndex = i;
        //                break;
        //            }
        //        }
        //    }
        //    for (int i = 0; i < DefaultButtonCellLayout.Count; i++)
        //    {
        //        if (FilmPageUtil.FilmLayoutEquals(cellLayout, DefaultButtonCellLayout[i]))
        //        {
        //            CellLayoutFound = true;
        //            break;
        //        }
        //    }
        //    //find the item in FilmCellLayoutCollection
        //    if (false == CellLayoutFound)
        //    {
        //        for (int i = 0; i < FilmCellLayoutCollection.Count; i++)
        //        {
        //            if (FilmPageUtil.FilmLayoutEquals(cellLayout, FilmCellLayoutCollection[i]))
        //            {
        //                cellLayoutCombox.SelectedIndex = i;
        //                CellLayoutFound = true;

        //                break;
        //            }
        //        }
        //    }
        //    //if no matched item in FilmCellLayoutCollection, then select the custom cell layout in combobox
        //    //if (false == CellLayoutFound)
        //    //{
        //    //    for (int i = 0; i < cellLayoutCombox.Items.Count; i++)
        //    //    {
        //    //        FilmLayout layout = cellLayoutCombox.Items[i] as FilmLayout;
        //    //        if (-1 == layout.LayoutColumnsSize && -1 == layout.LayoutRowsSize)
        //    //        {
        //    //            cellLayoutCombox.SelectedIndex = i;
        //    //            break;
        //    //        }
        //    //    }
        //    //}
        //}

       // private bool _ignoreCellLayoutSelectionChangedEvent;

        private FilmLayout GetBoxSelectLayout(IEnumerable<FilmLayout> collection,ComboBox layoutCb)
        {
            var cellLayoutIco = layoutCb.SelectedItem as System.Windows.Controls.Image;
            FilmLayout cellLayout = null;
            if (cellLayoutIco != null)
            {
                var layoutname = cellLayoutIco.ToolTip.ToString();
                foreach (var item in collection)
                {
                    if (item.LayoutName == layoutname)
                    {
                        cellLayout = item;
                        break;
                    }
                }

            }
            return cellLayout;
        }

        //private void OnCellLayoutSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();

        //        if (_ignoreCellLayoutSelectionChangedEvent)
        //        {
        //            _ignoreCellLayoutSelectionChangedEvent = false;
        //            return;
        //        }

        //        var cellLayoutComboBox = sender as ComboBox;
        //        if (cellLayoutComboBox != null)
        //        {

        //            FilmLayout cellLayout = GetBoxSelectLayout(FilmCellLayoutCollection, cellLayoutComboBox);
                    
        //          //  var cellLayout = cellLayoutComboBox.SelectedItem as FilmLayout;
        //            if (cellLayout != null)
        //            {
        //                if (cellLayout.LayoutName.Equals(Card.Resources["UID_Filming_CustomCellLayout"] as string))
        //                {
        //                    //ModelDialogHandler.ShowModelWnd(CustomCellLayoutWindow);
        //                    ShowCustomCellLayoutWindow();
        //                }
        //                else
        //                {
        //                    Dispatcher.BeginInvoke(new Action(() =>
        //                    {
        //                        CellLayoutActiveFilmingPages(cellLayout);
        //                    }), DispatcherPriority.Background);
        //                }

        //                Card.UpdateUIStatus();
        //                string layoutIconKey = LayoutImageConverter.GetCellLayoutIconKey(cellLayout.LayoutName);
        //                imgCellLayout.Source =SelectedCellLayoutIco[layoutIconKey] ;
                        
        //                //string layoutString = cellLayout.ToString();
        //                //string layoutIconKey = LayoutImageConverter.GetCellLayoutIconKey(layoutString);
        //                //int idx = layoutIconKey.IndexOf("_white");
        //                //string resourceString = layoutIconKey.Substring(0, idx);
        //                //var filmmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
        //                //if (filmmingCard != null)
        //                //{
        //                //    try
        //                //    {
        //                //        var img = filmmingCard.FindResource(resourceString) as ImageSource;
        //                //        imgCellLayout.Source = img;
        //                //        //rdoFilmingcLayout.IsChecked = true;
        //                //    }
        //                //    catch (Exception ex)
        //                //    {
        //                //        Logger.LogFuncException(ex.Message + ex.StackTrace);
        //                //    }
        //                //}
        //            }
        //            else
        //            {
        //                imgCellLayout.Source = null;
        //            }
        //        }

        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message + ex.StackTrace);
        //    }
        //}

        //private void OnViewportLayoutComboboxToolTipOpening(object sender, ToolTipEventArgs e)
        //{
        //    ComboBox combobox = sender as ComboBox;
        //    if (null == combobox || null == combobox.SelectedItem)
        //    {
        //        return;
        //    }
        //    FilmLayout viewportLayout = GetBoxSelectLayout(ViewportLayoutCollection, combobox);
        //    combobox.ToolTip = viewportLayout.LayoutName;
        //}

        //private bool _ignoreViewportLayoutSelectionChangedEvent;

        //private void OnViewportLayoutSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();

        //        if (_ignoreViewportLayoutSelectionChangedEvent)
        //        {
        //            _ignoreViewportLayoutSelectionChangedEvent = false;
        //            return;
        //        }

        //        var viewportLayoutComboBox = sender as ComboBox;
        //        if (viewportLayoutComboBox != null)
        //        {
        //            //var viewportLayout = viewportLayoutComboBox.SelectedItem as FilmLayout;
        //            FilmLayout viewportLayout = GetBoxSelectLayout(ViewportLayoutCollection, viewportLayoutComboBox);
                    
        //            if (viewportLayout != null)
        //            {
        //                ViewportLayoutActiveFilmingPages(viewportLayout);
        //                Card.UpdateUIStatus();
        //                string layoutIconKey = LayoutImageConverter.GetViewportLayoutIconName(viewportLayout.LayoutName);
        //                imgVPLayout.Source = SelectedViewPortLayoutIco[layoutIconKey];
        //                //string layoutString = viewportLayout.ToString();
        //                //string layoutIconKey = LayoutImageConverter.GetViewportLayoutIconName(layoutString);
        //                ////int idx = layoutIconKey.IndexOf("_white");
        //                //string resourceString = layoutIconKey.Replace("white", "");
        //                //resourceString = resourceString.TrimEnd('_');
        //                //var filmmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
        //                //if (filmmingCard != null)
        //                //{
        //                //    try
        //                //    {
        //                //        var img = filmmingCard.FindResource(resourceString) as ImageSource;
        //                //        imgVPLayout.Source = img;
        //                //        //rdoFilmingvLayout.IsChecked = true;
        //                //    }
        //                //    catch (Exception ex)
        //                //    {
        //                //        Logger.LogFuncException(ex.Message + ex.StackTrace);
        //                //    }
        //                //}
        //            }
        //            else
        //            {
        //                imgVPLayout.Source = null;
        //            }
        //        }

        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message + ex.StackTrace);
        //    }
        //}

        private void OnViewportLayoutButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                int index;
                var button = sender as Button;
                if (button == null || !int.TryParse(button.Tag.ToString(), out index)) return;

                ViewportLayoutActiveFilmingPages(DefaultViewportLayoutCollection[index]);

                Card.UpdateUIStatus();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnAddFilmPage(object sender, RoutedEventArgs e)
        {
            if (Card.IfZoomWindowShowState)
            {
                Card.ZoomViewer.CloseDialog();
            }
            var page = Card.OnAddFilmPageInFilmingCard();
            if (page != null)
            {
                page.FilmPageType = FilmPageType.BreakFilmPage;
                Card.EntityFilmingPageList.UpdateBreakPageFlag();

                Card.GotoFilmBoardWithIndex(Card.FilmPageBoardCount - 1);

            }
            Card.EntityFilmingPageList.UpdatePageLabel();
            //UpdateUIStatus();
        }

        private CustomViewportLayoutWindow _customViewportLayoutWindow; // = new CustomViewportLayoutWindow();

        public CustomViewportLayoutWindow CustomViewportLayoutWindow
        {
            get
            {
                if (_customViewportLayoutWindow == null)
                {
                    _customViewportLayoutWindow = new CustomViewportLayoutWindow();
                }
                return _customViewportLayoutWindow;
            }
        }

        private void OnSaveCurViewportLayout(object sender, RoutedEventArgs e)
        {
            var filmingPageControl = Card.DisplayedFilmPage.FirstOrDefault();
            if (filmingPageControl == null)  return;
            var layout = filmingPageControl.ViewportLayout;
            #region [--save multiformat layout in layout--]
            if (filmingPageControl.ViewportLayout.LayoutType == LayoutTypeEnum.RegularLayout
                && filmingPageControl.filmingViewerControl.LayoutManager.RootCell.Children.Any(c => c.Count > 1))
            {
                var layoutString =
                    filmingPageControl.filmingViewerControl.LayoutManager.SaveFlimingLayoutToXML();
                layout = new FilmLayout(layoutString);
            }
            #endregion

            SavedFilmPageControl.ViewportLayout = layout;
            SavedFilmPageControl.filmingViewerControl.UpdateLayout();
            SaveCustomViewportLayout();
        }

        private void SaveCustomViewportLayout()
        {
            this.CustomViewportLayoutWindow.CustomViewportViewModel.CustomViewportName
                = FilmingHelper.GetDistinctCustomLayoutName(
                    Card.Resources["UID_Filming_CustomViewPortLayout_New"] as string);
            if (!this.CustomViewportLayoutWindow.CustomViewportViewModel.IsCustomViewportNameValid())
            {
                MessageBoxHandler.Instance.ShowWarning(Card.Resources["UID_Filming_CustomViewPortLayout_New_Warning_Invalid_Name"] as string);
                return;
            }
            if (this.CustomViewportLayoutWindow.CustomViewportViewModel.ExistViewportHasSameNameWithActiveViewport())
            {
                MessageBoxHandler.Instance.ShowQuestion(Card.Resources["UID_Filming_CustomViewPortLayout_New_Warning_With_Same_Name"] as string
                    + this.CustomViewportLayoutWindow.CustomViewportViewModel.CustomViewportName, new MsgResponseHander(SaveLayoutHandler));
            }
            else
            {
                SaveLayoutHandler(MessageBoxResponse.YES);
            }
        }


        private void SaveLayoutHandler(MessageBoxResponse res)
        {
            if (res != MessageBoxResponse.YES)
            {
                return;
            }
            try
            {
                Logger.LogFuncUp();
                SavedFilmPageControl.SaveCustomerLayout(this.CustomViewportLayoutWindow.CustomViewportViewModel.CustomViewportName);
                this.CustomViewportLayoutWindow.CustomViewportViewModel.Refresh();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnCustomerLayoutClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var messageWindow = new MessageWindow
                {
                    WindowTitle =Card.Resources["UID_Filming_CustomViewPortLayout_Setting_Title"] as string,
                    WindowChild = CustomViewportLayoutWindow,
                    WindowDisplayMode = WindowDisplayMode.Default
                };
                Card.HostAdornerCount++;
                messageWindow.ShowModelDialog();
                Card.HostAdornerCount = 0;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                Card.HostAdornerCount = 0;
            }
        }
        //规则viewport布局自定义
        private void OnCustomeViewportClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                ShowCustomViewportWindow();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void DoBatchFilm()
        {
            try
            {
                Logger.LogFuncUp();

                var items = Card.studyTreeCtrl.seriesSelectionCtrl.SelectedSeriesItems;
                FilmingUtility.AssertEnumerable(items);
                //Debug.Assert(items.Count == 1);
                //var item = items.FirstOrDefault();
                //Debug.Assert(item != null);

                var db = FilmingDbOperation.Instance.FilmingDbWrapper;

                var seriesUidAndCount = new List<Tuple<string, int>>();

                var images = new List<ImageBase>();
                foreach (var item in items)
                {
                    images.AddRange(db.GetImageListByConditionWithOrder(
                        "SeriesInstanceUIDFk='" + item.SeriesInstanceUID + "'", "InstanceNumber"));
                }
                FilmingUtility.AssertEnumerable(images);

                seriesUidAndCount.Add(new Tuple<string, int>(items[0].SeriesInstanceUID, (int)Card.InterSelection.ViewModel.ImageNumbers));

                if (!FilmingHelper.CheckMemoryForLoadingSeries(seriesUidAndCount, FilmingViewerContainee.Main.GetCommunicationProxy()))
                {
                    MessageBoxHandler.Instance.ShowError("UID_Filming_Warning_NoEnoughMemory");
                    return;
                }

                Card.ImagesLoadBeginning(Card.InterSelection.ViewModel.ImageNumbers);
                var loadingImages = new List<FilmImageObject>();
                loadingImages.AddRange(
                    images.Select(imageData => new FilmImageObject
                    {
                        ImageFilePath = imageData.FilePath,
                        ImageSopInstanceUid =
                            imageData.SOPInstanceUID,
                        ViewPortIndex = -1,
                        CellIndex = -1
                    }));

                Card._pagesToBeRefreshed.Clear();
                Card._cellsToBeMoveForward.Clear();

                FilmingPageControl beginPage = Card.EntityFilmingPageList.LastOrDefault();
                if (beginPage == null)
                    beginPage = Card.AddFilmPage();
                if (beginPage.HasEmptyCell())
                {
                    if (beginPage.ImageCount != beginPage.MaxImagesCount)
                    {
                        Card.ActiveFilmingPageList.UnSelectAllCells();
                        Card.SelectObject(beginPage, null, null);
                        Card._loadingTargetPage = beginPage;
                        Card._loadingTargetCellIndex = beginPage.ImageCount;
                    }
                    else
                    {
                        FilmingPageControl newPage = Card.GetLinkedPage(beginPage).Find(page => page.HasEmptyCell());
                        if (null == newPage)
                        {
                            newPage = beginPage;
                        }
                        var pageList = Card.GetLinkedPage(newPage);
                        newPage = Card.InsertFilmPage(newPage.FilmPageIndex + pageList.Count);
                        Card.ReOrderFilmPage();
                        newPage.FilmPageType = FilmPageType.BatchFilmPage;

                        Card.ActiveFilmingPageList.UnSelectAllCells();
                        Card.SelectObject(newPage, null, null);

                        Card._loadingTargetPage = newPage;
                        Card._loadingTargetCellIndex = 0;
                    }

                }
                else
                {
                    FilmingPageControl newPage = Card.GetLinkedPage(beginPage).Find(page => page.HasEmptyCell());
                    if (null == newPage)
                    {
                        newPage = beginPage;
                    }
                    //var pageList = GetLinkedPage(newPage);
                    //newPage = InsertFilmPage(newPage.FilmPageIndex + pageList.Count);
                    //ReOrderFilmPage();
                    //newPage.FilmPageType = FilmPageType.BatchFilmPage;

                    Card.ActiveFilmingPageList.UnSelectAllCells();
                    Card.SelectObject(newPage, null, null);

                    Card._loadingTargetPage = newPage;
                    Card._loadingTargetCellIndex = newPage.MaxImagesCount;
                }

                for (uint i = Card.InterSelection.ViewModel.FirstImage;
                     i <= Card.InterSelection.ViewModel.LastImage;
                     i = i + Card.InterSelection.ViewModel.Every)
                {
                    var k = (int)i;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        try
                        {
                            Card._dataLoader.LoadSopByUid(
                                loadingImages[k - 1].ImageSopInstanceUid);

                        }
                        catch (Exception ex)
                        {
                            Logger.LogFuncException(ex.Message + ex.StackTrace);
                        }
                    }), DispatcherPriority.Background);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                Card.UpdateUIStatus();
                Card.EnableUI();
                throw;
            }
            finally
            {
                //DisableUIForElecFilmOperation(false);
                //UpdateUIStatus();
                //EnableUI();
            }
        }

        public void CellLayoutActiveFilmingPages(FilmLayout filmLayout)
        {
            try
            {
                Logger.Instance.LogPerformanceRecord("[Begin][change layout]");
                bool winthCtrl = Keyboard.IsKeyDown(Key.LeftCtrl);
                Logger.LogFuncUp();
                if (filmLayout == null)
                {
                    return;
                }
                Card.DisableUI();

                if (Card.CurrentActionType == ActionType.RegionFreehand || Card.CurrentActionType == ActionType.RegionSpline ||
                    Card.CurrentActionType == ActionType.RegionPolygon)
                {
                    foreach (var filmPage in Card.ActiveFilmingPageList)
                    {
                        foreach (var cell in filmPage.SelectedCells())
                        {
                            if (cell.Image != null && cell.Image.CurrentPage != null)
                            {
                                cell.ForceEndAction();
                            }
                        }
                    }
                }
                if (Card.IfZoomWindowShowState)
                {
                    Card.ZoomViewer.CloseDialog();
                } 
                //irregular film change viewport's cell layout  
                //不规则布局变化其中一个viewport布局的逻辑
                //需满足条件1：选中一张胶片；2：选中一个viewport图片；3：设置的layout是一个standardLayout（即3X4未加工的)
                if (Card.ActiveFilmingPageList.Count == 1 
                    //&& Card.ActiveFilmingPageList.First().SelectedViewports().Count == 1
                    && filmLayout.LayoutType == LayoutTypeEnum.StandardLayout
                    && Card.ActiveFilmingPageList.Any(p =>p.ViewportLayout.LayoutType == LayoutTypeEnum.DefinedLayout))
                {
                    Card.LastSelectedFilmingPage = Card.ActiveFilmingPageList.First();
                    ChangeSelectedViewportsLayoutTo(Card.ActiveFilmingPageList.First().SelectedViewports(), filmLayout);
                    //ChangeSelectedViewportLayoutTo(filmLayout);
                    Card.EnableUI(Card.FilmingCardModality == FilmingUtility.EFilmModality);
                }
                else
                {
                    //不需要变换布局的情况
                    //1.规则布局，长宽一致，无多分格
                    if (filmLayout.LayoutType == LayoutTypeEnum.StandardLayout
                        &&Card.ActiveFilmingPageList.All(
                            film => film.filmingViewerControl.LayoutManager.RootCell.Rows == filmLayout.LayoutRowsSize
                            && film.filmingViewerControl.LayoutManager.RootCell.Columns == filmLayout.LayoutColumnsSize 
                            && film.SelectedCells().All(c=>             //multiformatCell
                                                            {
                                                                var filmingLayoutCell = c.ParentCell as FilmingLayoutCell;
                                                                return filmingLayoutCell != null && !filmingLayoutCell.IsMultiformatLayoutCell;
                                                            })))
                    
                    {
                        Card.EnableUI(Card.FilmingCardModality == FilmingUtility.EFilmModality);
                        return;
                    }

                    var startThread = new ThreadStart(() => Dispatcher.Invoke(
                        new Action(() =>  ChangeToStandardLayout(filmLayout,winthCtrl)),
                        DispatcherPriority.Background));
                    var thread = new Thread(startThread);
                    thread.Start();

                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                Card.EnableUI(Card.FilmingCardModality == FilmingUtility.EFilmModality);
            }
            finally
            {
                Logger.LogFuncDown();
                Logger.Instance.LogPerformanceRecord("[End][change layout]");
            }

        }

        private void ChangeSelectedViewportsLayoutTo(List<McsfFilmViewport> selectedViewports,FilmLayout filmLayout)
        {
            //不满足变换条件，返回
            if (selectedViewports == null || selectedViewports.Count == 0)    //未选中vp，返回
                return;
            if(selectedViewports.All(v => v.CellLayout.Compare(filmLayout)))  //选中vp都满足和所变化filmLayout一致，返回
                return;
            var pageSelected = Card.ActiveFilmingPageList.LastOrDefault();
            if(pageSelected == null) return;
            //备份参与变化的vp的cell
            var backupControlCells = new List<MedViewerControlCell>();
            foreach (var vp in selectedViewports)
            {
                var cellList = vp.RootLayoutCell.Children.OfType<MedViewerControlCell>().ToList();
                backupControlCells.AddRange(cellList.Select(cCell => cCell.Clone()));
            }

            //计算变化前vp中cell数和变化后vp中cell数
            int oldCellsCount = backupControlCells.Count;
            int newCellsCount = selectedViewports.Count * filmLayout.LayoutColumnsSize * filmLayout.LayoutRowsSize;

            //设置vp的celllayout
            if (!pageSelected.SetSelectedCellLayoutWithOutLimited(filmLayout)) return;
            
            //获取新cell集合
            var newCells = pageSelected.SelectedViewports().SelectMany(v => v.GetCells()).ToList();
            //SetSelectedCellLayout方法vplist被重新创建，故重新获取选中vp集合
            selectedViewports.Clear();
            selectedViewports = pageSelected.SelectedViewports();

            //处理变换布局引起的cell位置变化
            if (oldCellsCount > newCellsCount)           //新cell数不足，需要向后挤图片占用位置，走图片粘贴流程
            {
                //由于旧有cell比较多，便利新cell，将旧有cell的一部分替换进去
                
                for (int i = 0; i < newCellsCount; i++)
                {
                    var originalCell = newCells[i];
                    var replaceCell = backupControlCells[i];
                    if (originalCell == replaceCell) continue;
                    if (originalCell.IsEmpty && replaceCell.IsEmpty) continue;
                    if (!originalCell.IsEmpty) { originalCell.Image.Clear(); originalCell.Refresh(); }
                    if (!replaceCell.IsEmpty) originalCell.Image.AddPage(replaceCell.Image.CurrentPage);
                    FilmPageUtil.SetAllActionExceptLeftButton(originalCell);
                    originalCell.Refresh();
                }

                //获取旧有剩余的cell，调用粘贴流程将剩余cell插入
                    //1.获取backup的剩余cell
                var restCells = backupControlCells.GetRange(newCellsCount, oldCellsCount - newCellsCount).Where(c => !c.IsEmpty).ToList();
                
                //2.计算并设置插入位置
                if(restCells.Count != 0)
                {
                    var lastCellOfViewport = newCells.LastOrDefault();
                
                    if (lastCellOfViewport == null) return;

                    if (lastCellOfViewport.CellIndex < pageSelected.Cells.Count() - 1)
                    {
                        Card._dropFilmingPage = pageSelected;
                        Card._dropViewCell = pageSelected.Cells.ElementAt(lastCellOfViewport.CellIndex + 1);
                    }
                    else
                    {
                        Card._dropFilmingPage = Card.GetOrCreateNextFilmPage(pageSelected);
                        Card._dropViewCell = Card._dropFilmingPage.Cells.First();
                    }
                    Card.InsertCells(restCells);
                }
                Card.EntityFilmingPageList.UpdatePageLabel();
            }
            else                         //新cell数充足
            {
                //由于新cell比较多，遍历旧cell，将旧有cell的一部分替换进去 
                for (int i = 0; i < oldCellsCount; i++)
                {
                    var originalCell = newCells[i];
                    var replaceCell = backupControlCells[i];
                    if (originalCell == replaceCell) continue;
                    if (originalCell.IsEmpty && replaceCell.IsEmpty) continue;
                    if (!originalCell.IsEmpty)
                    {
                        FilmPageUtil.ClearAllActions(originalCell);
                        originalCell.Image.Clear(); 
                        originalCell.Refresh();
                    }
                    if (!replaceCell.IsEmpty)
                    {
                        originalCell.Image.AddPage(replaceCell.Image.CurrentPage);
                        FilmPageUtil.SetAllActionExceptLeftButton(originalCell);
                    }

                    originalCell.Refresh();
                }
                for (int j = oldCellsCount; j < newCellsCount; j++)
                {
                    FilmPageUtil.ClearAllActions(newCells[j]);
                    if(newCells[j].IsEmpty) continue;
                    newCells[j].Image.Clear();
                    newCells[j].Refresh();
                    
                }
                if(Card.IsEnableRepack)
                    Card.contextMenu.Repack();
                else
                {
                    Card.EntityFilmingPageList.UpdatePageLabel();
                }
            }
            //设置选中状态
            Card.EntityFilmingPageList.UnSelectAllCells();
            var lastCell = new MedViewerControlCell();
            pageSelected.IsSelected = true;
            foreach (var c in newCells)
            {
                if (!c.IsEmpty)
                {
                    c.IsSelected = true;
                    lastCell = c;
                }
            }
            foreach (var vp in selectedViewports)
            {
                vp.IsSelected = true;
            }
            Card.LastSelectedCell = lastCell;
            Card.LastSelectedViewport = selectedViewports.LastOrDefault();
            Card.LastSelectedFilmingPage = pageSelected;
            Card.UpdateUIStatus();
        }

        private void ChangeSelectedViewportLayoutTo(FilmLayout filmLayout)
        {
            try
            {

            FilmingPageControl pageSelected = Card.LastSelectedFilmingPage;
            McsfFilmViewport selectedViewport = pageSelected.SelectedViewports().FirstOrDefault();
            if (selectedViewport == null) 
                return;
            if (selectedViewport.CellLayout.Compare(filmLayout))
            {
                //EnableUI(FilmingCardModality == FilmingUtility.EFilmModality);
                return;
            }

            Card._loadingTargetPage = pageSelected;

            var viewport = pageSelected.SelectedViewports().FirstOrDefault();
            int backupViewportIndex = viewport.IndexInFilm;

            McsfFilmViewport backupViewport;

            if (viewport.CellLayout.MaxImagesCount >= filmLayout.MaxImagesCount) //布局由大改小
            {
                //1. 备份Cell
                //1.1 备份Viewport中的cell
                var controlCells = viewport.RootLayoutCell.Children.OfType<MedViewerControlCell>().ToList();
                int backupCellStartIndex = filmLayout.MaxImagesCount;
                int backupCellEndIndex = controlCells.FindLastIndex(c => c != null & !c.IsEmpty);
                int backupCellCount = backupCellEndIndex - backupCellStartIndex + 1;

                var backupCells = backupCellCount <= 0
                                      ? new List<MedViewerControlCell>()
                                      : controlCells.GetRange(backupCellStartIndex, backupCellCount);


                ////2.设置布局
                if (!pageSelected.SetSelectedCellLayout(filmLayout)) return;

                if (backupCells.Count == 0) return;

                ////3.填入备份的cell

                //采用粘贴图片的逻辑


                backupViewport = pageSelected.ViewportList.ElementAt(backupViewportIndex);
                var layoutCell = backupViewport.RootLayoutCell;
                var lastCellOfViewport = layoutCell.Children.Last() as MedViewerControlCell;

                if (lastCellOfViewport.CellIndex < pageSelected.Cells.Count() - 1)
                {
                    Card._dropFilmingPage = pageSelected;
                    Card._dropViewCell = pageSelected.Cells.ElementAt(lastCellOfViewport.CellIndex + 1);
                }
                else
                {
                    Card._dropFilmingPage = Card.GetOrCreateNextFilmPage(pageSelected);
                    Card._dropViewCell = Card._dropFilmingPage.Cells.First();
                }

                Card.InsertCells(backupCells);
                Card.EntityFilmingPageList.UpdatePageLabel();
            }
            else // 布局由小改大， 后续的viewport中的图片要前移
            {
                //1.备份cell
                var pages = Card.GetLinkedPage(pageSelected);
                var startCellIndex = viewport.GetCells().Last().CellIndex;
                var complementaryCellCount = filmLayout.MaxImagesCount - viewport.CellLayout.MaxImagesCount;
                var replaceCells = pages.SelectMany(p => p.Cells).Skip(startCellIndex + 1).ToList();
                replaceCells.AddRange(Enumerable.Repeat(new FilmingControlCell(), complementaryCellCount));

                //2.设置布局
                if (!pageSelected.SetSelectedCellLayout(filmLayout)) return;
                backupViewport = pageSelected.ViewportList.ElementAt(backupViewportIndex);

                ////为新增的cell注册事件
                //for (int i = startCellIndex + 1; i < filmLayout.MaxImagesCount; i++)
                //{
                //    var cell = pageSelected.Cells.ElementAt(i);
                //    pageSelected.OnNewCellAdded(cell, null);
                //    cell.Refresh();
                //}

                var originalCells = pages.SelectMany(p => p.Cells).Skip(startCellIndex + 1).ToList();
                for (int i = 0; i < originalCells.Count; i++)
                {
                    var originalCell = originalCells[i];
                    var replaceCell = replaceCells[i];
                    if (originalCell == replaceCell) continue;
                    if (originalCell.IsEmpty && replaceCell.IsEmpty) continue;
                    if (!originalCell.IsEmpty) { originalCell.Image.Clear(); originalCell.Refresh(); }
                    if (!replaceCell.IsEmpty) originalCell.Image.AddPage(replaceCell.Image.CurrentPage);
                    FilmPageUtil.SetAllActionExceptLeftButton(originalCell);
                    originalCell.Refresh();
                }

                //3. remove empty page
                pages = Card.GetLinkedPage(pageSelected);
                for (int i = pages.Count - 1; i >= 1; i--)
                {
                    var page = pages.ElementAt(i);
                    if (page.IsEmpty()) Card.commands.DeleteFilmPage(page);
                    else break;
                }
                Card.EntityFilmingPageList.UpdatePageLabel();
            }



            //4.设置选中状态

            FilmPageUtil.UnselectOtherViewports(pageSelected, backupViewport);
            FilmPageUtil.UnselectOtherFilmingPages(Card.EntityFilmingPageList, pageSelected);
            Card.SelectObject(pageSelected, backupViewport, null);
            backupViewport.SelectAllCells(true);
            //EnableUI(FilmingCardModality == FilmingUtility.EFilmModality);
            //return;

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                Card.actiontoolCtrl.RefreshAction();
            }
        }

        private void ChangeToStandardLayout(FilmLayout filmLayout,bool onCells=false)
        {
            if (onCells)
            {
                ChangeCellsToStandardLayout(filmLayout);
            }
            else
            {
                ChangePagesToStandardLayout(filmLayout);
            }
        }

        private void ChangePagesToStandardLayout(FilmLayout filmLayout)
        {
            try
            {
                Logger.Instance.LogPerformanceRecord("[Begin][change standard layout on Pages]");

                Logger.LogFuncUp();
                Card.CurrentFilmingState = FilmingRunState.ChangeLayout;
               //FilmingPageControl.visibleRefereshed = 0;
                //1. Backup displayData of selected cells  //similiar to cell reverse
                var backupDisplayData = new List<DisplayData>();
                var allCells = new List<MedViewerControlCell>();
                var sortedActivePageList =
                    Card.ActiveFilmingPageList.OrderBy(a => a.FilmPageIndex).ToList();
                if (sortedActivePageList.Count < 1)
                {
                    Logger.Instance.LogDevInfo("Empty Active Page in ChangePagesToStandardLayout");
                    return;
                }
                foreach (var filmingPage in sortedActivePageList)
                {
                    var celllist = filmingPage.Cells.OrderBy(a => a.CellIndex).ToList();
                    var num = celllist.Count;
                    for (int i = num - 1; i > -1; i--)
                    {
                        if(celllist[i].Image != null&&celllist[i].Image.CurrentPage!=null) break;
                        celllist.RemoveAt(i);
                    }
                    allCells.AddRange(celllist);
                }

                var LastSelectedCellIndexFlag = allCells.IndexOf(Card.LastSelectedCell);
                if (LastSelectedCellIndexFlag == -1) LastSelectedCellIndexFlag = 0;

                foreach (var cell in allCells)
                {
                    var displayData = !cell.IsEmpty ? cell.Image.CurrentPage : null;
                    backupDisplayData.Add(displayData);
                }
                 
                var headPageIndex = sortedActivePageList.FirstOrDefault().FilmPageIndex - 1;
                var headPage = headPageIndex < 0 ? null : Card.EntityFilmingPageList[headPageIndex];

                var tailPageIndex = sortedActivePageList.LastOrDefault().FilmPageIndex + 1;
                var tailPage = tailPageIndex >= Card.EntityFilmingPageList.Count
                    ? null
                    : Card.EntityFilmingPageList[tailPageIndex];

                foreach (var filmingPage in Card.ActiveFilmingPageList.ToList())
                {
                    Card.commands.DeleteFilmPage(filmingPage);
                }
                Card.ActiveFilmingPageList.Clear();

                ////2.3 new page for new body pages
                var headIndex = headPage == null ? -1 : headPage.FilmPageIndex;
                var bodyPage = Card.InsertFilmPage(headIndex + 1, filmLayout);
                bodyPage.IsSelected = true;
                
                Card._loadingTargetPage = bodyPage;
                Card._loadingTargetCellIndex = 0;
                Card._pagesToBeRefreshed.Clear();

                ////2.4 set page break
                if (headPage != null)
                    bodyPage.FilmPageType = FilmPageType.BreakFilmPage;
                if (tailPage != null)
                    tailPage.FilmPageType = FilmPageType.BreakFilmPage;

                //Card.EntityFilmingPageList.ForEach((page) =>
                //{
                //    page.IsSelected = false;
                //    page.SelectViewports(false);
                //});


                // Display First body page
                Card.DisplayFilmPage(bodyPage, true);

                var size = backupDisplayData.Count;
                for (int i = 0; i < size; i++)
                {
                    Card.ReplaceCellBy(backupDisplayData.ElementAt(i), "layout");
                    if (0 == LastSelectedCellIndexFlag)
                    {
                        Card.LastSelectedFilmingPage = Card._loadingTargetPage;
                        Card.LastSelectedCell = Card._loadingTargetPage.Cells.ElementAt(Card._loadingTargetCellIndex - 1);
                    }
                    LastSelectedCellIndexFlag--;
                }
                Card._dropFilmingPage = Card.LastSelectedFilmingPage;
                Card._dropViewCell = Card.LastSelectedCell;

                //7. Repack
                if (Card.IsEnableRepack)
                {
                    Card.contextMenu.Repack();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
              //  Card.HostAdornerCount = 0;
               // Card.UpdateUIStatus();
                Card.actiontoolCtrl.RefreshAction(); //reset cursor 
                Card.EnableUI(Card.FilmingCardModality ==FilmingUtility.EFilmModality);
                Card.CurrentFilmingState = FilmingRunState.Default;
                Card.EntityFilmingPageList.UpdatePageLabel();
                Card.UpdateFilmingCount(Card.EntityFilmingPageList.Count);
                Card.ReOrderCurrentFilmPageBoard();
                Card.UpdateFilmCardScrollBar();
                Card.UpdateLabelSelectedCount();
                Logger.Instance.LogPerformanceRecord("[End][change standard layout on Pages]");
            }
        }
        
        private void ChangeCellsToStandardLayout(FilmLayout filmLayout)
        {
            try
            {
                Logger.Instance.LogPerformanceRecord("[Begin][change standard layout on Cells]");

                Logger.LogFuncUp();
                Card.CurrentFilmingState = FilmingRunState.ChangeLayout;

                bool isHeadPageCleared = false;
                bool isTailPageCleared = false; 

                //FilmingPageControl.visibleRefereshed = 0;
                //1. Backup displayData of selected cells  //similiar to cell reverse
                var backupDisplayData = new List<DisplayData>();
                var selectedCells = new List<MedViewerControlCell>();
                var sortedActivePageList =
                    Card.ActiveFilmingPageList.OrderBy(a => a.FilmPageIndex).ToList();
                foreach (var filmingPage in sortedActivePageList)
                {
                    selectedCells.AddRange(filmingPage.SelectedCells().OrderBy(a => a.CellIndex).ToList());
                }

                var LastSelectedCellIndexFlag = selectedCells.IndexOf(Card.LastSelectedCell);
                if (LastSelectedCellIndexFlag == -1) LastSelectedCellIndexFlag = 0;

                foreach (var cell in selectedCells)
                {
                    var displayData = !cell.IsEmpty ? cell.Image.CurrentPage : null;
                    backupDisplayData.Add(displayData);

                    if(displayData == null) 
                    {
                        cell.IsSelected = false;
                        continue;
                    }

                    cell.Image.Clear();
                    cell.IsSelected = false;
                    cell.Refresh();
                }

                //todo: performance optimization begin change layout
                //清除空的displayData
                var layoutCellCount = filmLayout.MaxImagesCount;
                var firstNonEmptyDataIndex = backupDisplayData.FindIndex(d => d != null);
                if (firstNonEmptyDataIndex == -1) firstNonEmptyDataIndex = backupDisplayData.Count - 1;
                var lastNonEmptyDataIndex = backupDisplayData.FindLastIndex(d => d != null);
                if (lastNonEmptyDataIndex != -1)
                    backupDisplayData.RemoveRange(lastNonEmptyDataIndex + 1, (backupDisplayData.Count - 1 - lastNonEmptyDataIndex) / layoutCellCount * layoutCellCount);

                backupDisplayData.RemoveRange(0, firstNonEmptyDataIndex / layoutCellCount * layoutCellCount);
                //todo: performance optimization end


                //2. set page break,  new pages 

                //2.1 calulate "head" "body" "tail", delete empty body.

                var headPage = sortedActivePageList.FirstOrDefault();
                if (headPage != null)
                {
                    if (selectedCells.FirstOrDefault().CellIndex == 0)
                    {
                        var headPageIndex = headPage.FilmPageIndex - 1;
                        headPage = headPageIndex < 0 ? null : Card.EntityFilmingPageList[headPageIndex];
                    }
                    else
                    {
                        isHeadPageCleared = true;
                    }
                }
                if (headPage != null) headPage.IsSelected = false;

                var tailPage = sortedActivePageList.LastOrDefault();
                if (tailPage != null)
                {
                    if (selectedCells.LastOrDefault().CellIndex == tailPage.Cells.Count() - 1)
                    {
                        var tailPageIndex = tailPage.FilmPageIndex + 1;
                        tailPage = tailPageIndex >= Card.EntityFilmingPageList.Count
                                       ? null
                                       : Card.EntityFilmingPageList[tailPageIndex];
                    }
                    else
                    {
                        isTailPageCleared = true;
                    }
                }
                if (tailPage != null) tailPage.IsSelected = false;
                //Stopwatch sw = new Stopwatch();

                // sw.Restart();
                foreach (var filmingPage in Card.ActiveFilmingPageList.ToList())
                {
                    Card.commands.DeleteFilmPage(filmingPage);
                }
                Card.ActiveFilmingPageList.Clear();

                //2.2 new page for a head just in a page

                if (headPage != null && headPage == tailPage)   //选中的连续图片,只是在一张胶片的中间
                {
                    var oldHeadPage = headPage;
                    headPage = Card.InsertFilmPage(headPage.FilmPageIndex, headPage.ViewportLayout);
                    headPage.FilmPageType = oldHeadPage.FilmPageType;
                    isHeadPageCleared = true;
                    var headCellsCount = selectedCells.FirstOrDefault().CellIndex;
                    for (int i = 0; i < headCellsCount; i++)
                    {
                        var cell = headPage.GetCellByIndex(i);
                        var oldCell = oldHeadPage.GetCellByIndex(i);
                        if (!oldCell.IsEmpty)
                        {
                            //todo: performance optimization change layout
                            var displayData = oldCell.Image.CurrentPage;
                            cell.Image.AddPage(displayData);
                            FilmPageUtil.SetAllActions(cell, Card.CurrentActionType);
                            if (headPage.IsVisible)
                            {
                                cell.Refresh();
                                cell.Image.CurrentPage.IsDirty = false;
                            }
                            else
                            {
                                cell.Image.CurrentPage.IsDirty = true;
                            }
                            oldCell.Image.Clear();
                            FilmPageUtil.ClearAllActions(oldCell);
                            oldCell.Refresh();
                        }
                    }
                }
                //2.3 new page for new body pages
                var headIndex = headPage == null ? -1 : headPage.FilmPageIndex;
                var bodyPage = Card.InsertFilmPage(headIndex + 1, filmLayout);

                //sw.Restart();
                Card._loadingTargetPage = bodyPage;
                Card._loadingTargetCellIndex = 0;
                Card._pagesToBeRefreshed.Clear();

                //2.4 set page break
                if (headPage != null)
                    bodyPage.FilmPageType = FilmPageType.BreakFilmPage;
                if (tailPage != null)
                    tailPage.FilmPageType = FilmPageType.BreakFilmPage;

                Card.EntityFilmingPageList.ForEach((page) =>
                {
                    page.IsSelected = false;
                    page.SelectViewports(false);
                });


                // Display First body page
                Card.DisplayFilmPage(bodyPage, true);

                var size = backupDisplayData.Count;
                for (int i = 0; i < size; i++)
                {
                    Card.ReplaceCellBy(backupDisplayData.ElementAt(i),"layout");
                    if (0 == LastSelectedCellIndexFlag)
                    {
                        Card.LastSelectedFilmingPage = Card._loadingTargetPage;
                        Card.LastSelectedCell = Card._loadingTargetPage.Cells.ElementAt(Card._loadingTargetCellIndex - 1);

                    }

                    LastSelectedCellIndexFlag--;

                }
                Card._dropFilmingPage = Card.LastSelectedFilmingPage;
                Card._dropViewCell = Card.LastSelectedCell;

                //5 Remove empty head/tail/body pages
                int rBegin = Card.EntityFilmingPageList.Count - 1;
                if (tailPage != null) rBegin = tailPage.FilmPageIndex - 1;
                int rEnd = bodyPage.FilmPageIndex;
                for (int i = rBegin; i > rEnd; i--)
                {
                    var page = Card.EntityFilmingPageList.ElementAt(i);
                    if (!page.IsEmpty()) break;
                    Card.commands.DeleteFilmPage(page);
                }
                if (headPage != null && headPage.IsEmpty() && isHeadPageCleared)
                {
                    Card.commands.DeleteFilmPage(headPage);
                    Card.DisplayFilmPage(bodyPage);
                }

                if (tailPage != null && tailPage.IsEmpty() && isTailPageCleared)
                    Card.commands.DeleteFilmPage(tailPage);

                //7. Repack
                if (Card.IsEnableRepack)
                {
                    Card.contextMenu.Repack();
                }
               
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
              //  Card.HostAdornerCount = 0;
               // Card.UpdateUIStatus();
                Card.actiontoolCtrl.RefreshAction(); //reset cursor 
                Card.EnableUI(Card.FilmingCardModality ==FilmingUtility.EFilmModality);
                Card.CurrentFilmingState = FilmingRunState.Default;
                Card.EntityFilmingPageList.UpdatePageLabel();
                Card.UpdateFilmingCount(Card.EntityFilmingPageList.Count);
                Card.ReOrderCurrentFilmPageBoard();
                Card.UpdateFilmCardScrollBar();
                Logger.Instance.LogPerformanceRecord("[End][change standard layout]");
            }
        }


        public void ViewportLayoutActiveFilmingPages(FilmLayout filmLayout)
        {
            try
            {
                Logger.LogFuncUp();

                if (filmLayout == null)
                {
                    return;
                }

                if (Card.IsModalityForMammoImage())
                {
                    var layout = filmLayout.Clone();
                    layout.LayoutType = LayoutTypeEnum.RegularLayout;
                    CellLayoutActiveFilmingPages(layout);
                    return;
                }
                var layout1 = filmLayout.Clone();
                CellLayoutActiveFilmingPages(layout1);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
            finally
            {
                Card.actiontoolCtrl.RefreshAction(); //because action is reset by viewer control
                //DisableUIForElecFilmOperation(true);
                Card.UpdateButtonStatus();
            }
        }

        public void SetLayoutForCommonFilming(LayoutCommandInfo layoutInfo)
        {
            try
            {
                Logger.LogFuncUp();

                if (Card.FilmingCardModality == FilmingUtility.EFilmModality) return;

                //1.split layoutinfo

                var layoutString = layoutInfo.LayoutString;
                uint filmingNumbers = layoutInfo.ImageCount;
                var filmingMode = layoutInfo.Orientation;
                var setting = Card.PrintAndSave.PrinterSetting.DataViewModal;

                setting.CurrentFilmOrientation = filmingMode;
                Card.PrintAndSave.PrinterSetting.CloneViewModel();

                //ignore arg[3] which is FilmingIdentifier

                //2.set layout,
                var layout = new FilmLayout(layoutString);
                var lastFilm = Card.EntityFilmingPageList.Last();
                if (!lastFilm.IsEmpty())
                {
                    lastFilm = Card.AddFilmPage(layout);
                }
                else
                {
                    lastFilm.ViewportLayout = layout;
                }
                var lastFilmIndex = Card.EntityFilmingPageList.IndexOf(lastFilm);
                if (lastFilmIndex > 0) Card.EntityFilmingPageList.ElementAt(lastFilmIndex).FilmPageType = FilmPageType.BreakFilmPage;



                //3.update image loading progress bar
                //  filmingContextMenuRepack.IsChecked = false;
                Card.ImagesLoadBeginning(filmingNumbers);

                Card._isLayoutBatchJob = true;
                Card._isLayoutSetByOtherApplication = true;

                if (!lastFilm.IsVisible)
                {
                    Card.DisplayFilmPage(lastFilm);
                    Card.ReOrderCurrentFilmPageBoard();
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void UpdateBtnState()
        {
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableNewFilmPage"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableChangeCellLayout"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableChangeToSingleViewportLayout"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableChangeToMultiViewportLayout"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableViewportLayoutManager"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableViewportLayoutCustomer"));
          //  this.viewportLayoutButton1.IsEnabled = IsEnableChangeToSingleViewportLayout;

           

        }



        public bool IsEnableChangeCellLayout
        {
            get
            {
                // [2012-09-05 allow to change multiple film pages layout in same time]
                if (Card.IsCellModalitySC) return false;
                if (0 == Card.ActiveFilmingPageList.Count)
                {
                    return false;
                }
                if (Card.ActiveFilmingPageList.Any((page) => page.IsInSeriesCompareMode))
                {
                    return false;
                }
                if (Card.IsSuccessiveSelectedIgnoredEmptyCell())

                {
                    if(Card.ActiveFilmingPageList.Count != 1)
                        return true;
                    if(Card.ActiveFilmingPageList.FirstOrDefault().ViewportLayout.LayoutType != LayoutTypeEnum.DefinedLayout)
                        return true;
                }
                if (Card.IsEnableChangeViewportCellLayouts())
                    return true;
                return false;
            }
        }

       

       

        public bool IsEnableNewFilmPage
        {
            get
            {
                if (Card.IsCellModalitySC) return false;
                return true;
            }
        }


        public bool IsEnableChangeToSingleViewportLayout
        {
            get
            {
                try
                {
                    
                    if (Card.IsCellModalitySC) return false;
                    if (!PreConditionToChangeViewportLayout)
                        return false;

                    if (Card.IsModalityForMammoImage())
                    {
                        return true;
                    }

                    var film = Card.ActiveFilmingPageList.FirstOrDefault();
                    if (film == null) return false;
                    if (Card.ActiveFilmingPageList.Count > 1) return false;
                    if (film.IsEmpty())
                        return true;

                   

                    //if (film.ViewportList.Count == 1)
                    if (film.ViewportLayout.LayoutType != LayoutTypeEnum.DefinedLayout)
                        return false;

                    ////if (film.ImageCount > DefaultViewportLayoutCollection.FirstOrDefault().MaxImagesCount)
                    ////    //In Default 1x1 viewport layout, cell layout is 4x5
                    ////    return false;

                    //return true;
                    return false;
                }
                catch (Exception ex)
                {
                    Logger.LogFuncException(ex.Message + ex.StackTrace);
                    return false;
                }
            }
        }

        public bool PreConditionToChangeViewportLayout
        {
            get
            {
                try
                {
                    if (Card.ActiveFilmingPageList.Count == 1 || Card.ActiveFilmingPageList.Count != 0 && Card.IsModalityForMammoImage())
                    {
                        return true;
                    }

                    if (Card.ActiveFilmingPageList.Any((page) => page.IsInSeriesCompareMode))
                    {
                        return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogFuncDown(ex.StackTrace);
                    return false;
                }
            }

        }

        public bool IsEnableChangeToMultiViewportLayout
        {
            get
            {
                if (Card.IsCellModalitySC) return false;
                if (0 == Card.ActiveFilmingPageList.Count)
                {
                    return false;
                }
                if (Card.ActiveFilmingPageList.Any((page) => page.IsInSeriesCompareMode))
                {
                    return false;
                }
                if (!Card.IsSuccessiveSelectedIgnoredEmptyCell())
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsEnableViewportLayoutManager
        {
            get
            {
                if (Card.IsCellModalitySC) return false;
                return null != Card.ActiveFilmingPageList;// && Card.ActiveFilmingPageList.Count == 1 && Card.ActiveFilmingPageList.First().IsEmpty();
            }
        }

        public bool IsEnableViewportLayoutCustomer
        {
            get
            {
                if (Card.IsCellModalitySC) return false;
                return null != Card.ActiveFilmingPageList && Card.ActiveFilmingPageList.Count == 1 ;
            }
        }

        private void OnBtnRealSizeClick(object sender, RoutedEventArgs e)
        {
            Card.SetCellsRealSize();
        }
       
       

    }
}
