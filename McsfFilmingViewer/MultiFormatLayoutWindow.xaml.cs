using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UIH.Mcsf.Controls;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Utility;

//using UIH.Mcsf.Utility;

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Interaction logic for MultiFormatLayoutWindow.xaml
    /// </summary>
    public partial class MultiFormatLayoutWindow : UserControl
    {
        #region  [---Fields---]
        private List<FilmLayout> _multiFormatLayoutList = new List<FilmLayout>();
        private Dictionary<int, RadioButton> _defaultRadioButtonChecked = new Dictionary<int, RadioButton>();
        #endregion


        #region  [---Constructors---]
        public MultiFormatLayoutWindow()
        {
            InitializeComponent();

            if (FilmingViewerContainee.FilmingResourceDict != null)
            {
                Resources.MergedDictionaries.Add(FilmingViewerContainee.FilmingResourceDict);
            }

            InitMultiFormatLayout();


            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(MultiFormatLayoutWindow_IsVisibleChanged);
        }

        private void MultiFormatLayoutWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var window = sender as MultiFormatLayoutWindow;
            if (null == window) return;
            if (window.Visibility == Visibility.Visible)
            {
                switch (ImageCount)
                {
                    case 0:
                    _defaultRadioButtonChecked[2].IsChecked = false;
                    _defaultRadioButtonChecked[4].IsChecked = false;
                    _defaultRadioButtonChecked[6].IsChecked = false;
                    _defaultRadioButtonChecked[9].IsChecked = false;
                        break;
                    case 1:
                    case 2:
                        _defaultRadioButtonChecked[2].IsChecked = true;
                        break;
                    case 3:
                    case 4:
                        _defaultRadioButtonChecked[4].IsChecked = true;
                        break;
                    case 5:
                    case 6:
                        _defaultRadioButtonChecked[6].IsChecked = true;
                        break;
                    case 7:
                    case 8:
                    case 9:
                        _defaultRadioButtonChecked[9].IsChecked = true;
                        break;
                    default:
                        _defaultRadioButtonChecked[9].IsChecked = true;
                        break;
                }
            }
        }
        #endregion


        #region  [---Properties---]
        public FilmLayout SelectedMultiFormatLayout { private set; get; }
        public int ImageCount { set; get; }
        #endregion


        #region  [---Private Methods---]
        private void InitMultiFormatLayout()
        {
            _multiFormatLayoutList.Add(new FilmLayout(1, 2));
            _defaultRadioButtonChecked.Add(1, radio1);

            _multiFormatLayoutList.Add(new FilmLayout(2, 1));
            _defaultRadioButtonChecked.Add(2, radio2);

            _multiFormatLayoutList.Add(new FilmLayout(2, 2));
            _defaultRadioButtonChecked.Add(4, radio3);

            _multiFormatLayoutList.Add(new FilmLayout(3, 2));
            _defaultRadioButtonChecked.Add(6, radio4);

            _multiFormatLayoutList.Add(new FilmLayout(3, 3));
            _defaultRadioButtonChecked.Add(9, radio6);
        }

        private void OnOKButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)radio1.IsChecked)
                {
                    SelectedMultiFormatLayout = _multiFormatLayoutList[0];
                }

                if ((bool)radio2.IsChecked)
                {
                    SelectedMultiFormatLayout = _multiFormatLayoutList[1];
                }

                if ((bool)radio3.IsChecked)
                {
                    SelectedMultiFormatLayout = _multiFormatLayoutList[2];
                }

                if ((bool)radio4.IsChecked)
                {
                    SelectedMultiFormatLayout = _multiFormatLayoutList[3];
                }

                if ((bool)radio6.IsChecked)
                {
                    SelectedMultiFormatLayout = _multiFormatLayoutList[4];
                }

                //旧的实现，不要删除
                //bool IsMultiFormatCellSuccess = (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).MultiFormatCells();
                bool IsMultiFormatCellSuccess = (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).contextMenu.DivideCell();
                if (IsMultiFormatCellSuccess)
                {
                    _defaultRadioButtonChecked[1].IsChecked = false;
                    _defaultRadioButtonChecked[2].IsChecked = false;
                    _defaultRadioButtonChecked[4].IsChecked = false;
                    _defaultRadioButtonChecked[6].IsChecked = false;
                    _defaultRadioButtonChecked[9].IsChecked = false;
                    ImageCount = 0;
                    SelectedMultiFormatLayout = null;
                    //WindowHostManagerWrapper.CloseSecondaryWindow();
                   // WindowHostManager.Close();// DivideCell中调用了Repack函数,将hostAdoner设置为0了
                    this.CloseParentDialog();
                }

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        public void OnCancelButtonClick(object sender, EventArgs e)
        {
            try
            {
                _defaultRadioButtonChecked[1].IsChecked = false;
                _defaultRadioButtonChecked[2].IsChecked = false;
                _defaultRadioButtonChecked[4].IsChecked = false;
                _defaultRadioButtonChecked[6].IsChecked = false;
                _defaultRadioButtonChecked[9].IsChecked = false;
                ImageCount = 0;
                SelectedMultiFormatLayout = null;
              //  WindowHostManagerWrapper.CloseSecondaryWindow();
                this.CloseParentDialog();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }


        #endregion

    }
}
