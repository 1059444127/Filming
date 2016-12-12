using System.Diagnostics;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class ImageCell
    {
        public ImageCell()
        {
            DisplayData = GlobalDefinitions.EmptyDisplayData;
        }
        // TODO: ImageCell.Create An Image DisplayData  for Displaying An Image in ViewerControlAdapter
        public DisplayData DisplayData { get; private set; }
        // TODO: try 异步加载方式

        public ImageCell(string sopInstanceUid)
        {
            var dataLoader = DataLoaderFactory.Instance().CreateSyncSopLoader(DBWrapperHelper.DBWrapper);
            var sop = dataLoader.LoadSopByUid(sopInstanceUid);
            var dataAccessor = new DataAccessor();
            var imageSop = sop as ImageSop;
            Debug.Assert(imageSop != null);
            var pixelData = imageSop.GetNormalizedPixelData();
            var ps = imageSop.PresentationState;
            var displayData = dataAccessor.CreateImageData(sop.DicomSource, pixelData, ps);
            DisplayData = displayData ?? GlobalDefinitions.EmptyDisplayData;
        }

    }
}
