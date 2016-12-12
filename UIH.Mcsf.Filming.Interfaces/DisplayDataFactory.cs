using System.Diagnostics;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class DisplayDataFactory
    {
        #region [--Singleton--]

        public static readonly DisplayDataFactory Instance = new DisplayDataFactory();
        private readonly DisplayData _emptyDisplayData = new DisplayData();
        private readonly IBasicLoader _syncDataLoader = DataLoaderFactory.Instance().CreateSyncSopLoader(DBWrapperHelper.DBWrapper);
        private readonly DataAccessor _dataAccessor;

        private DisplayDataFactory()
        {
            // TODO-Later: create IViewerConfigure for dataAccessor
            _dataAccessor = new DataAccessor();
        }

        #endregion [--Singleton--]

        public DisplayData CreateDisplayData()
        {
            return _emptyDisplayData;
        }

        public DisplayData CreateDisplayData(string sopInstanceUid)
        {
            // TODO-later: use ConcurrentDictionary manage sop
            var sop = _syncDataLoader.LoadSopByUid(sopInstanceUid);
            var imageSop = sop as ImageSop;
            Debug.Assert(imageSop != null);
            var displayData = _dataAccessor.CreateImageData(sop.DicomSource, imageSop.GetNormalizedPixelData(), imageSop.PresentationState);
            return displayData ?? DisplayDataFactory.Instance.CreateDisplayData();
        }
    }
}
