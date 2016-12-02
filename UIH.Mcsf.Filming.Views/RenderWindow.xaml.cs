using System;
using System.Linq;
using System.Windows;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.Models;
using UIH.Mcsf.Viewer;


namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for RenderWindow.xaml
    /// </summary>
    public partial class RenderWindow
    {
        private FilmPageControl _filmPageControl;

        private string _modalities = string.Empty;
        private string Modality
        {
            get
            {
                if (String.IsNullOrEmpty(_modalities))
                {
                    mcsf_clr_systemenvironment_config.GetModalityName(out _modalities);
                }
                return _modalities;
            }
        }
        public RenderWindow()
        {
            InitializeComponent();

            this._filmPageControl = new FilmPageControl();
            MainGrid.Children.Add(_filmPageControl);
            this.SetRenderPosition(new Size(FilmingUtility.DisplayedFilmPageWidth, FilmingUtility.DisplayedFilmPageHeight));
        }

        public void AddFilmCard(CardModel cardModelModel)
        {
            try
            {
                Logger.LogFuncUp();

                //设置胶片尺寸, 与FilmingCard保持一致
                bool ifPageNo = cardModelModel.PageTitleConfigInfoModel.PageNo == "1" ? true : false;
                if (Modality=="MG"||Modality == "DBT")
                    this.SetFilmSizeForMG(cardModelModel.AllFilmSize,cardModelModel.DisplayedFilmPageWidth,cardModelModel.DisplayedFilmPageViewerHeight);
                else
                {
                    this.SetFilmSize(cardModelModel.AllFilmSize,
                    cardModelModel.FilmPageModel.PageTitleInfoModel.DisplayPosition,
                    cardModelModel.DisplayedFilmPageWidth, cardModelModel.DisplayedFilmPageHeight);
                }
                _filmPageControl.FilmingViewerControl.ForceRender();
                this.SetRenderPosition(new Size(this._filmPageControl.Width, this._filmPageControl.Height));
               
                //设置FilmPageTitle的配置信息
                
                this._filmPageControl.FilmTitleViewModel.SetPageTitleConfigInfo(cardModelModel.PageTitleConfigInfoModel);
                


                this._filmPageControl.SetPageTitleElementPosition();

                this._filmPageControl.AddFilmPage(cardModelModel.FilmPageModel, ifPageNo);
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void SetFilmSize(Size size, string filmTitleBarPosition, double width, double height)
        {
            var scale = size.Height / size.Width;
            _filmPageControl.Width = width;
            _filmPageControl.filmingViewerControlGrid.Width = width;
            _filmPageControl.Height = height;
            _filmPageControl.filmingViewerControlGrid.Height = filmTitleBarPosition != "0"
                                          ? _filmPageControl.Width * scale *
                                            FilmingUtility.VIEWERCONTROL_PERCENTAGE_OF_FILMPAGE
                                          : _filmPageControl.Width * scale;
        }

        private void SetFilmSizeForMG(Size size, double width,double viewerHeight)
        {
            var scale = size.Height / size.Width;
            _filmPageControl.Width = width;
            _filmPageControl.filmingViewerControlGrid.Width = width;
            _filmPageControl.Height = width * scale;
            _filmPageControl.FilmingViewerControl.Height = viewerHeight;
            //if (ifPageNo)
            //{
            //    _filmPageControl.filmingViewerControlGrid.Height = filmTitleBarPosition != "0"
            //        ? _filmPageControl.Width * scale * FilmingUtility.VIEWERCONTROL_PERCENTAGE_OF_FILMPAGE
            //        : _filmPageControl.Width * scale * FilmingUtility.HEADER_PERCENTAGE_OF_FILMPAGE_SIMPLE;
            //}
            //else
            //{
            //    _filmPageControl.filmingViewerControlGrid.Height = filmTitleBarPosition != "0"
            //        ? _filmPageControl.Width * scale *
            //          FilmingUtility.VIEWERCONTROL_PERCENTAGE_OF_FILMPAGE
            //        : _filmPageControl.Width * scale;
            //}
        }

        public void SetRenderPosition(Size filmSize)
        {

            var debugConfigure = Configure.Environment.Instance.GetDebugConfigure();
            if (debugConfigure.StandAlone) return;


            {

                this.ShowInTaskbar = false;
                this.WindowStartupLocation = WindowStartupLocation.Manual;

                this.Top = -3 * filmSize.Height;
                this.Left = -3 * filmSize.Width;
            }

        }

        public void UpdateFilmingImageText()
        {
            _filmPageControl.UpdateFilmingImageText();
        }

        public void UpdateFilmingImageProperty()
        {
            _filmPageControl.UpdateFilmingImageProperty();
        }

        public void UpdateFilmingPrinterConfig()
        {
            _filmPageControl.UpdateFilmingPrintConfig();
        }
    }
}
