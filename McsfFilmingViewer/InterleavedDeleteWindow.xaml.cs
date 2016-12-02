using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.MHC;
using UIH.Mcsf.Utility;
using System.ComponentModel;

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Interaction logic for InterleavedDeleteWindow.xaml
    /// </summary>
    public partial class InterleavedDeleteWindow : UserControl
    {
        private TextBox FactTextBox = null;
        private FilmingCard filmingCard = null;
        
        public InterleavedDeleteWindow()
        {
            InitializeComponent();

            IsQuit = true;

            if (FilmingViewerContainee.FilmingResourceDict != null)
            {
                Resources.MergedDictionaries.Add(FilmingViewerContainee.FilmingResourceDict);
            }
            this.Loaded += new RoutedEventHandler(InterleavedDeleteWindow_Loaded);
        }

        private void InterleavedDeleteWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FactTextBox = FindChild<TextBox>(EveryTextBox, "TextBox");
            if (FactTextBox != null)
            {               
                FactTextBox.Focus();
                FactTextBox.TextChanged += FactTextBox_ValueChanged;
            }
            
            filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;

        }

        private void FactTextBox_ValueChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;         
            ViewModel.Every = uint.Parse(textBox.Text);            
        }


        public bool? IsQuit { get; set; }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                IsQuit = null;

                if (EveryTextBox.Text == string.Empty)
                {
                    MessageBoxHandler.Instance.ShowError("UID_Filming_Warning_Form_Not_Complete");
                    return;
                }


                if (filmingCard.IfZoomWindowShowState)
                {
                    filmingCard.ZoomViewer.CloseDialog();
                }

                int imageCount = int.Parse(FactTextBox.Text);
                //var count = filmingCard.ActiveFilmingPageList.Aggregate<FilmingPageControl, uint>(0,
                //   (current, page) => current + (uint)page.SelectedCells().Count(c => c != null && !c.IsEmpty));

                if (imageCount > ViewModel.TotalImages)
                {
                    MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Warning_InterleavedExceedTotalCount", new MsgResponseHander(DoInterleavedDeleteThresholdHandler));
                }
                else
                {
                    DoInterleavedDeleteThresholdHandler();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }

        }

        private void DoInterleavedDeleteThresholdHandler(MessageBoxResponse response = MessageBoxResponse.YES)
        {
            try
            {
                Logger.LogFuncUp();
                if (response != MessageBoxResponse.YES) return;
               
                IsQuit = false;

                this.CloseParentDialog();

                DoInterleavedDelete();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private void DoInterleavedDelete()
        {
            try
            {
                Logger.LogFuncUp();
               
                foreach (var filmPage in filmingCard.ActiveFilmingPageList.Skip(1))  //不用处理第一张为BreakFilmPage的情况
                {
                    if (filmPage.FilmPageType == FilmPageType.BreakFilmPage)
                    {
                        filmPage.FilmPageType = FilmPageType.NormalFilmPage;
                        if (filmPage.FilmPageIndex - 1 >= 0 && filmPage.FilmPageIndex < filmingCard.EntityFilmingPageList.Count)
                        {
                            filmingCard.EntityFilmingPageList[filmPage.FilmPageIndex - 1].SetPageBreakLabel(false);
                        }
                    }
                }

                var selectedCells = filmingCard.CollectSelectedCells();
                var every = ViewModel.Every;
                for (int i = 0; i < selectedCells.Count; i++)
                {
                    if (i % every != 0)
                    {
                        selectedCells[i].IsSelected = false;
                        selectedCells[i].Image.Clear();
                        selectedCells[i].Refresh();
                    }
                }

                filmingCard.UpdateImageCount();

                if (filmingCard.IsEnableRepack)
                    filmingCard.contextMenu.Repack(RepackMode.RepackMenu);
                else
                    filmingCard.EntityFilmingPageList.UpdatePageLabel();
                filmingCard.UpdateUIStatus();

                //todo: performance optimization begin pageTitle
                filmingCard.EntityFilmingPageList.ForEach((film) => film.RefereshPageTitle());
                //todo: performance optimization end

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message);
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.CloseParentDialog();
            IsQuit = true;
        }

        private void EverySpinButton_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            EveryTextBox.Focus();
        }
        private void firstImageSpinButton_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsNumberic(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void firstImageSpinButton_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void firstImageSpinButton_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumberic(e.Text))
            {
                e.Handled = true;
            }
            else
            {
                var currentText = FactTextBox.Text + e.Text;                
                if (currentText.Length > 6)
                {
                    if (FactTextBox.SelectionLength == 6)
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    e.Handled = false;
                }
            }
        }

        public static bool IsNumberic(string _string)
        {
            string str = @"^\d+$";
            Regex regex = new Regex(str);
            if (regex.IsMatch(_string))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CheckNullParameter()
        {
            if (EveryTextBox.Value == null) EveryTextBox.Value = ViewModel.Every;
        }

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

    }
}
