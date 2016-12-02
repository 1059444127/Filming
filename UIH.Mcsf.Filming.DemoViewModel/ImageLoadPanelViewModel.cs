using System.Windows.Input;
using UIH.Mcsf.Filming.DataModel;
using UIH.Mcsf.Filming.Widgets;

namespace UIH.Mcsf.Filming.DemoViewModel
{
    public class ImageLoadPanelViewModel
    {
        private readonly IImageLoader _imageLoader;

        public ImageLoadPanelViewModel(IImageLoader imageLoader)
        {
            _imageLoader = imageLoader;
            _imageLoader.PreLoad(SopInstanceUid);
        }

        #region LoadSeriesCommand

        private ICommand _loadSeriesCommand;

        public ICommand LoadSeriesCommand
        {
            get { return _loadSeriesCommand = _loadSeriesCommand ?? new RelayCommand(p => _imageLoader.LoadSeries(SopInstanceUid)); }
        }

        #endregion LoadSeriesCommand

        #region NewPageCommand

        private ICommand _newPageCommand;

        public ICommand NewPageCommand
        {
            get { return _newPageCommand = _newPageCommand ?? new RelayCommand(p => _imageLoader.NewPage()); }
        }

        #endregion NewPageCommand

        private const string SopInstanceUid = @"1.2.156.112605.161340985965.20140523063942.3.8880.2"; //24 images

        //private const string SopInstanceUid = @"2.16.840.1.113669.632.21.3562420283.83959825.9877042702579478.12"; //2814 images
        //2814 images
        //const string sopInstanceUid = @"1.2.156.112605.0.0.20121225125357974609.0.3.0.0";
    }
}