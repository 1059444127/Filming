using System;
using System.Windows;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Core;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Card.View
{
    /// <summary>
    /// Interaction logic for Film.xaml
    /// </summary>
    public partial class Film
    {

        public Film()
        {
            InitializeComponent();

            //var studyTree = new StudyTree();
            //_dataLoader = DataLoaderFactory.Instance().CreateLoader(studyTree, DBWrapperHelper.DBWrapper);
            //_dataLoader.SopLoadedHandler += OnImageDataLoaded;


            ////Read configuration
            //var entryPath = mcsf_clr_systemenvironment_config
            //    .GetApplicationPath("FilmingConfigPath");
            //_viewerControl.InitializeWithoutCommProxy(entryPath);
        }


//         private void OnButtonClick(object sender, RoutedEventArgs e)
//         {
//             //_viewerControl.Controller.
//             _dataLoader.LoadSopByUid("1.2.120.2012.1234567890.1.0");   
//         }
// 
//         private void OnImageDataLoaded(object sender, DataLoaderArgs e)
//         {
//             try
//             {
//                 Logger.LogFuncUp();
// 
//                 var imgSop = e.Target as ImageSop;
//                 if (imgSop == null)
//                 {
//                     throw new Exception("Image Data Loading mistake");
//                 }
//                 Logger.LogInfo(imgSop.ToString());
// 
//                 //new a cell
//                 var cell = new MedViewerControlCell();
// 
// 
//                 //add display data to cell
//                 var accessor = new DataAccessor();  //IViewerConfiguration config = null?
//                 var displayData = accessor.CreateImageData(imgSop.DicomSource, imgSop.GetNormalizedPixelData);
//                 cell.Image.AddPage(displayData);
// 
//                 //add cell to viewcontrol
//                 _viewerControl.LayoutManager.AddControlCell(cell);
// 
//                 //displayData.DeserializePSInfo();
//                 //_viewerControl.OnRaiseImageLoading(displayData);
//                 _viewerControl.Dispatcher.Invoke(new Action(() =>
//                 {
//                     try
//                     {
//                         if (_viewerControl.CellCount > 0)
//                         {
//                             _viewerControl.LayoutManager.Refresh();
//                         }
//                         else
//                         {
//                             cell.Refresh();
//                         }
//                     }
//                     catch (Exception exp)
//                     {
//                         Logger.LogError(exp.Message);
//                     }
//                 }));
// 
//                 //cell.DeserializeGraphics();
//                 //_viewerControl.OnRaiseImageLoaded(displayData);
// 
//                 Logger.LogFuncDown();
//             }
//             catch (Exception ex)
//             {
//                 Logger.LogFuncException(ex.Message+ex.StackTrace);
//             }
//         }
// 
// 
//         private IDataLoader _dataLoader;
    }
}
