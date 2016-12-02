using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UIH.Mcsf.Core;
using UIH.Mcsf.Database;
using UIH.Mcsf.Database.Proxy;

namespace UIH.Mcsf.Filming
{
    public enum Orientation
    {
        Portrait = 0,
        Landscape = 1
    }

    public class FilmingUtility
    {
        public static void AssertEnumerable<T>(IEnumerable<T> o)
        {
            Debug.Assert(o != null && o.Count() != 0);
        }

        public static void GenerateRowsAndColsForSeriesCompare(
            bool bCompareOrientationIsVertical, int seriesCount,
            out List<int> rows, out List<int> cols, uint maxRow = MaxRow, uint maxCol = MaxCol)
        {
            if (bCompareOrientationIsVertical)
            {
                cols = Enumerable.Range(1, (int)maxCol).Where(i => i % seriesCount == 0).ToList();
                rows = Enumerable.Range(1, (int)maxRow).ToList();
            }
            else // CompareOrientationIsVertical = false
            {
                cols = Enumerable.Range(1, (int)maxCol).ToList();
                rows = Enumerable.Range(1, (int)maxRow).Where(i => i % seriesCount == 0).ToList();
            }
        }

        public static void UpdatePrintStatus(List<string> uids, DBWrapper db, ICommunicationProxy proxy)
        {
            try
            {
                Logger.LogFuncUp();

                uids.RemoveAll(uid => string.IsNullOrWhiteSpace(uid));
                if (uids.Count == 0) return;

                //var db = FilmingDbOperation.Instance.FilmingDbWrapper;

                //var proxy = FilmingViewerContainee.Main.GetCommunicationProxy();

                db.SetAutoNotifyOn(proxy);
                foreach (var uid in uids)
                {
                    db.Update("studytable", "StudyInstanceUID='" + uid + "'", "StudyPrintStatus='1'");
                }
                //send db update notification

                IMcsfDBInfoModal dbInfoModal = new IMcsfDBInfoModal();
                dbInfoModal.EventItems = new List<IMcsfDBInfoModalItem>();
                IMcsfDBInfoModalItem item;
                //add event for itself
                foreach (var uid in uids)
                {
                    item = new IMcsfDBInfoModalItem();
                    item.Type = DBEventType.EventStudyUpdate;
                    item.UID = uid;
                    item.SubType = DBEventSubType.EventAll;
                    item.SubUID = "";
                    dbInfoModal.EventItems.Add(item);
                }
                var DBCommProxy = McsfDatabaseProxyFactory.Instance().CreateDatabaseProxy(proxy);
                DBCommProxy.SendEventInfo(dbInfoModal);


                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public const uint MaxRow = 10;
        public const uint MaxCol = 10;

        public const uint FilmingPageSyncBoundary = 200;

        public static readonly int DPI = 254;

        public static readonly int ScreenDPI = 96;

        public static readonly int COUNT_OF_IMAGES_WARNING_THRESHOLD_ONCE_LOADED = 1000;

        public static readonly double HEADER_PERCENTAGE_OF_FILMPAGE = 0.05;
        public static readonly double VIEWERCONTROL_PERCENTAGE_OF_FILMPAGE = 1 - HEADER_PERCENTAGE_OF_FILMPAGE;
        public static readonly double HEADER_PERCENTAGE_OF_FILMPAGE_SIMPLE = 0.97;

        public static readonly int MAX_FILM_COUNT = 100;

        public const string EFilmImageType = @"DERIVED\SECONDARY\DISPLAY\FILMING\EFILM";

        public const string StitchingImageType = @"DERIVED\SECONDARY\STITCHING\STITCHINGRESULT";

        public const string MultiFormatCellImageType = @"MultiFormat";

        public const string EFilmModality = @"EFILM";

        public const string EFilmDescriptionHeader = @"Electronic";

        public static double DisplayedFilmPageHeight = 850; //17*50,设置胶片size会改变此值;

        public static double DisplayedFilmPageWidth = 700;  //14*50;

        private static double _displayedFilmPageViewerHeight = 850; //17*50,设置胶片size会改变此值
        //public static readonly double DisplayedFilmPageHeight = DisplayedFilmPageMaxLength;

        public const string SecondaryCaptureDeviceManufacturer = @"UIH";

        //public static readonly double FilmScale = 1;

        //public static double OriginalFilmPageWidth = DisplayedFilmPageWidth * FilmScale;

        //public static double OriginalFilmPageHeight = DisplayedFilmPageHeight * FilmScale;

        public static readonly double MaxDisplayFilmSize = 6;

        public static readonly double MinDisplayFilmSize = 4;

        //public static readonly double MAX_FONT_SIZE_OF_ANNOTATION = FilmScale * MaxDisplayFilmSize;

        //public static readonly double MIN_FONT_SIZE_OF_ANNOTATION = FilmScale * MinDisplayFilmSize;

        public static readonly int MAX_NUMBER_LENGTH = 10;

        public static readonly double MultipleOfCellPerImage = 2.5;

        //howto use: Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag);
        public static readonly string FunctionTraceEnterFlag = "[!@#$][Main][Enter]"; //[Label][Version][Function]
        public static readonly string FunctionTraceExitFlag = "[!@#$][Main][Exit]"; //[Label][Version][Function]

        public static int JobTimeOutLength = 30;

        //public static uint PrivateProtocolTag = 0x00635049; //CT 产品在图像中使用的是Private tag 00635049（CS）保存 扫描协议
        public const uint ProtocolTag = 0x00180015;

        public static uint PatientsNameTag = 0x00100010;

        public static double DisplayedFilmPageViewerHeight
        {
            get { return _displayedFilmPageViewerHeight; }
            set { _displayedFilmPageViewerHeight = value; }
        }
    }
}
