using System.Diagnostics;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class DisplayDataFactory
    {
        #region [--Singleton--]

        public static DisplayDataFactory Instance = new DisplayDataFactory();
        private DisplayData _emptyDisplayData;

        private DisplayDataFactory()
        {
            _emptyDisplayData = new DisplayData();
        }

        #endregion [--Singleton--]

        public DisplayData CreateDisplayData()
        {
            return _emptyDisplayData;
        }

        public DisplayData CreateDisplayData(string sopInstanceUid)
        {
            // TODO: make dataloader(sync) static in ImageCell or in Factory
            var dataLoader = DataLoaderFactory.Instance().CreateSyncSopLoader(DBWrapperHelper.DBWrapper);
            // TODO: use ConcurrentDictionary manage sop
            var sop = dataLoader.LoadSopByUid(sopInstanceUid);
            // TODO: make dataAccessor static in ImageCell or in Factory
            // TODO: create IViewerConfigure for dataAccessor
            var dataAccessor = new DataAccessor();
            var imageSop = sop as ImageSop;
            Debug.Assert(imageSop != null);
            var displayData = dataAccessor.CreateImageData(sop.DicomSource, imageSop.GetNormalizedPixelData(), imageSop.PresentationState);
            return displayData ?? DisplayDataFactory.Instance.CreateDisplayData();
        }
    }
}
