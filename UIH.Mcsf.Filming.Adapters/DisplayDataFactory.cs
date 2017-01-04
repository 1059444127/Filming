using System.Diagnostics;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Adapters
{
    public class DisplayDataFactory
    {
        #region [--Singleton--]

        public static readonly DisplayDataFactory Instance = new DisplayDataFactory();
        private readonly DisplayData _emptyDisplayData = new DisplayData();
        private IBasicLoader _syncDataLoader; 
        private DataAccessor _dataAccessor;

        private DisplayDataFactory()
        {
            // TODO-Later: create IViewerConfigure for dataAccessor
        }

        private DataAccessor DataAccessor
        {
            get { return _dataAccessor ?? (_dataAccessor = new DataAccessor()); }
        }

        private IBasicLoader SyncDataLoader
        {
            get { return _syncDataLoader ?? (_syncDataLoader = DataLoaderFactory.Instance().CreateSyncSopLoader(DBWrapperHelper.DBWrapper)); }
        }

        #endregion [--Singleton--]

        public DisplayData CreateDisplayData()
        {
            return _emptyDisplayData;
        }

        public DisplayData CreateDisplayData(string sopInstanceUid)
        {
            // TODO-later: use ConcurrentDictionary manage sop
            var sop = SyncDataLoader.LoadSopByUid(sopInstanceUid);
            var imageSop = sop as ImageSop;
            Debug.Assert(imageSop != null);
            var displayData = DataAccessor.CreateImageData(sop.DicomSource, imageSop.GetNormalizedPixelData(), imageSop.PresentationState);
            return displayData ?? DisplayDataFactory.Instance.CreateDisplayData();
        }
    }
}
