using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.CustomizeLayout;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.MHC;
using UIH.Mcsf.Utility;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Interaction logic for CustomViewportLayoutWindow.xaml
    /// </summary>
    public partial class CustomViewportLayoutWindow : UserControl
    {
        private CustomViewportViewModel _customViewportViewModel = null;
        public CustomViewportViewModel CustomViewportViewModel
        {
            get
            {
                if (_customViewportViewModel == null)
                {
                    _customViewportViewModel = new CustomViewportViewModel();
                }
                return _customViewportViewModel;
            }
        }

        public CustomViewportLayoutWindow()
        {
            InitializeComponent(); 
            if (FilmingViewerContainee.FilmingResourceDict != null)
            {
                Resources.MergedDictionaries.Add(FilmingViewerContainee.FilmingResourceDict);
            }
            this.DataContext = CustomViewportViewModel;
            Keyboard.Focus(tabControl);
            InitialContextMenu();
        }

        private FilmLayout _activeFilmLayout;
        public FilmLayout ActiveFilmLayout
        {
            get { return _activeFilmLayout; }
            set
            {
                _activeFilmLayout = value.Clone();
            }
        }

        private FilmLayout _capturedFilmLayout;
        public FilmLayout CapturedFilmLayout
        {
            get {return _capturedFilmLayout;}
            set
            {
                _capturedFilmLayout = value.Clone();
            }
        }

        //private ContextMenu ItemContextMenu;
        private MenuItem _savetoPresetMenuItem;
        private void InitialContextMenu()
        {
            var contextMenu = new ContextMenu();
            contextMenu.Style = TryFindResource("Style_ContextMenu_CSW_CC_Default") as Style;

            //删除
            var deleteMenuItem = new MenuItem();
            deleteMenuItem.Header = Resources["UID_Filming_Custom_Delete"] as string;
            deleteMenuItem.Focusable = false;
            var deleteRelayCommand = new RelayCommand(param => OnDeleteCustomViewportLayoutButtonClick(null, null), param =>CustomViewportViewModel.IsEnableDelete);
            deleteMenuItem.Command = deleteRelayCommand;
            contextMenu.Items.Add(deleteMenuItem);

            //重命名
            var renameMenuItem = new MenuItem();
            renameMenuItem.Header = Resources["UID_Filming_Custom_Rename"] as string;
            renameMenuItem.Focusable = false;
            var renameRelayCommand = new RelayCommand(param => OnRenameButtonClick(), param => CustomViewportViewModel.IsEnableDelete);
            renameMenuItem.Command = renameRelayCommand;
            contextMenu.Items.Add(renameMenuItem);

            //保存到预设
            _savetoPresetMenuItem = new MenuItem
            {
                Header = Resources["UID_Filming_Custom_Save_to_Preset"] as string,
                Focusable = false
            };
            var emptyRelayCommand = new RelayCommand(param =>EmptyMethod(), param => !CustomViewportViewModel.IsEnableDelete);
            _savetoPresetMenuItem.Command = emptyRelayCommand;
            for(int i=1;i<7;i++)
            {
                var presetMenuItem = new MenuItem();
                var index = i;
                presetMenuItem.Header = (Resources["UID_Filming_Custom_Preset_Layout"] as string)+i;
                presetMenuItem.Focusable = false;
                var relayCommand = new RelayCommand(param => SavetoPresetLayout(index), param => !CustomViewportViewModel.IsEnableDelete);
                presetMenuItem.Command = relayCommand;
                presetMenuItem.IsCheckable = true;
                _savetoPresetMenuItem.Items.Add(presetMenuItem);
            }
            contextMenu.Items.Add(_savetoPresetMenuItem);

            //绑定布局
            var bindingMenuItem = new MenuItem();
            bindingMenuItem.Header = Resources["UID_Filming_Custom_Binding_Layout"] as string;
            bindingMenuItem.Focusable = false;
            var bindingRelayCommand = new RelayCommand(param => OnBindingButtonClick());
            bindingMenuItem.Command = bindingRelayCommand;
            //contextMenu.Items.Add(bindingMenuItem);

            this.ContextMenuOpening += (ContextMenu_ContextMenuOpening);
            this.ContextMenu = contextMenu;
            this.ContextMenu.Focusable = false;
        }

        private void SetBtnCheck(int i)
        {
            for (int j = 0; j < _savetoPresetMenuItem.Items.Count; j++)
            {
                var menuItem = _savetoPresetMenuItem.Items[j] as MenuItem;
                if (menuItem != null) menuItem.IsChecked = j+1 == i;
            }
        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (CustomViewportViewModel.SelectedViewportItem == null)
            {
                e.Handled = true;
                return;
            }
            SetBtnCheck(CustomViewportViewModel.SelectedViewportItem.IndexLabelContent);
        }
        
        private void EmptyMethod()
        {
            
        }

        private void SavetoPresetLayout(int index)
        {
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmingCard != null && ActiveFilmLayout != null)
            {
                filmingCard.layoutCtrl.ChangeBtnViewportLayout(index, ActiveFilmLayout);
            }
        }

        private void OnRenameButtonClick()
        {
            if (this.CustomViewportViewModel.SelectedViewportItem != null)
            {
                var item = this.CustomViewportViewModel.SelectedViewportItem;
                var index = this.CustomViewportViewModel.CustomViewportItemCollectionUser.IndexOf(item);
                if (index < 0) return;
                item.NameIsReadOnly = false;
                var myListBoxItem =(ListBoxItem)customViewportListBoxUser.ItemContainerGenerator.ContainerFromItem(
                            customViewportListBoxUser.SelectedItem);
                var txtBox = FindFirstVisualChild<TextBox>(myListBoxItem);
                if (txtBox != null)
                {
                    txtBox.Focus();
                    txtBox.SelectionLength = txtBox.Text.Length;
                }
            }
        }

        public T FindFirstVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindFirstVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        } 

        private void OnBindingButtonClick()
        {
            
        }

        private void OnApplyCustomViewportLayoutButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //Logger.LogFuncUp();
               
                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard != null && ActiveFilmLayout != null)
                {
                  
                    filmingCard.DisableUIForElecFilmOperation();

                    filmingCard.DisableUI();
                    this.CloseParentDialog();
                    //filmingCard.CellLayoutActiveFilmingPages(ActiveFilmLayout);
                    filmingCard.layoutCtrl.ViewportLayoutActiveFilmingPages(ActiveFilmLayout);

                    filmingCard.EnableUI();
                    filmingCard.DisableUIForElecFilmOperation(false);
                    filmingCard.UpdateUIStatus();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        private void OnDeleteCustomViewportLayoutButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                var selectedItem = CustomViewportViewModel.SelectedViewportItem;
                if (selectedItem != null 
                    && this.tabItemUser.Visibility ==Visibility.Visible
                    && CustomViewportViewModel.CustomViewportItemCollectionUser.Contains(selectedItem))
                {
                    if (!selectedItem.Destroy())
                    {
                        MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_CannotDeleteConfigure",
                            response => FilmingViewerContainee.Main.OnExitSecondaryUI());
                        customViewportListBoxOrigin.SelectedItem = null;
                        return;
                    }
                    var selectIndex = CustomViewportViewModel.CustomViewportItemCollectionUser.IndexOf(selectedItem);
                    CustomViewportViewModel.CustomViewportItemCollectionUser.Remove(selectedItem);
                    CustomViewportViewModel.SelectedViewportItemUser =
                        CustomViewportViewModel.CustomViewportItemCollectionUser.ElementAtOrDefault(selectIndex);
                    CustomViewportViewModel.SelectedViewportItem = CustomViewportViewModel.SelectedViewportItemUser;
                    CustomViewportViewModel.Refresh();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        //private void OnWindowClosing(object sender, CancelEventArgs e)
        //{
        //    e.Cancel = true;
        //    Hide();
        //}

        private void OnQuitButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                this.CloseParentDialog();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        private void customViewportListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.CustomViewportViewModel.SelectedViewportItem == null) return;
                using (var sr = new StreamReader(this.CustomViewportViewModel.SelectedViewportItem.CustomViewportLayoutXmlPath))
                {
                    var layout = new FilmLayout(sr.ReadToEnd(), this.CustomViewportViewModel.SelectedViewportItem.CustomViewportName);
                    layout.LayoutType = this.CustomViewportViewModel.SelectedViewportItem.LayoutType;
                    ActiveFilmLayout = layout;
                }
                e.Handled = true;
            }
            catch (Exception exp)
            {
                Logger.LogError("Exception in customViewportListBox_SelectionChanged: " + exp.Message);
                //throw;
            }
        }

        private void textBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.IsReadOnly = false;
            }
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null) 
            {
                textBox.IsReadOnly = true;
            }
        }


        private void TabItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabItem = ((sender as TabControl).SelectedItem) as TabItem;
            if (null == tabItem)
            {
                return;
            }
            if (tabItem.Name == "tabItemOrigin")
            {
                this.tabItemOrigin.IsSelected = true;
                this.CustomViewportViewModel.SelectedViewportItem =
                    this.CustomViewportViewModel.SelectedViewportItemOrigin;
            }
            else if (tabItem.Name == "tabItemUser")
            {
                this.tabItemUser.IsSelected = true;
                this.CustomViewportViewModel.SelectedViewportItem =
                    this.CustomViewportViewModel.SelectedViewportItemUser;
            }
            if (this.CustomViewportViewModel.SelectedViewportItem == null) return;
            using (var sr = new StreamReader(this.CustomViewportViewModel.SelectedViewportItem.CustomViewportLayoutXmlPath))
            {
                var layout = new FilmLayout(sr.ReadToEnd(), this.CustomViewportViewModel.SelectedViewportItem.CustomViewportName);
                layout.LayoutType = this.CustomViewportViewModel.SelectedViewportItem.LayoutType;
                ActiveFilmLayout = layout;
            }
        }

        private void TabControl_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (this.CustomViewportViewModel.SelectedViewportItem != null)
            {
                var item = this.CustomViewportViewModel.SelectedViewportItem;
                var index = this.CustomViewportViewModel.CustomViewportItemCollectionUser.IndexOf(item);
                if (index < 0) return;
                item.NameIsReadOnly = true;
            }
        }
    }
}
