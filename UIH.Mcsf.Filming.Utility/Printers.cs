using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Printing;
using UIH.Mcsf.AppControls.Viewer;
using UIH.Mcsf.Core;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Printers configure parser
    /// </summary>
    public class Printers
    {

        #region [--Constructor--]
        
        private static readonly object LockHelper = new object();

        public static Printers Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockHelper)
                    {
                        if (_instance == null)
                            _instance = new Printers();
                    }
                }
                return _instance;
            }
        }

        private static volatile Printers _instance;

        public Dictionary<string, string> Modality2FilmingImageTextConfigPath { get; private set; }
        public Dictionary<string, string> Modality2FilmingImageTextConfigContent { get; private set; }

        public string FilmingUserConfigPath = @"config/filming";

        public IViewerConfiguration MedViewerConfiguration { get; set; }

        ~Printers()
        {
            try
            {
                if(null != _fileParser)
                    _fileParser.Terminate();
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevWarning(e.Message);
            }
        }

        /// \brief  Type constructor.
        private Printers()
        {
            try
            {
                Logger.LogFuncUp();
                String sEntryPath = mcsf_clr_systemenvironment_config.GetApplicationPath("FilmingConfigPath");

                string modalityName;
                mcsf_clr_systemenvironment_config.GetModalityName(out modalityName);

                Modality2FilmingImageTextConfigContent = new Dictionary<string, string>();
                MedViewerConfiguration = new MedViewerConfiguration(FilmingUserConfigPath);
                _entryFilmingConfigPath = @"config\filming\config\McsfFilming.xml";

                _entryPrinterConfigPath = @"config\filming\config\PrinterConfig.xml";

                _entryWindowLevelPTConfigPath = @"config\viewer2d\config\McsfMedViewerConfig\mcsf_med_viewer_basic.xml";
                _entryFilmingMiscellaneousPath = @"config\filming\config\Miscellaneous.xml";
                //_entryFilmingDefaultLayoutPath = @"data\filming\config\mcsf_med_viewer_layout_type_00_1x1.xml";

                _entryAnnotationConfigPath = sEntryPath + "McsfMedViewerConfig\\mcsf_med_viewer_text_item.xml";
                _entryWindowLevelConfigPath = sEntryPath + "..\\..\\appcommon\\config\\app_miscellaneous.xml";

                #region 四角信息配置
                this.FilmingImageTextPath = @"config/filming/McsfMedViewerConfig/MedViewerImageText/";
                Modality2FilmingImageTextConfigPath = MedViewerConfiguration.EntryConfig.ImageTextMap;
                if (modalityName =="DBT")
                    this.Modality2FilmingImageTextConfigPath.Add("MG", FilmingImageTextPath + "mcsf_med_viewer_image_text_dbt.xml");
                this.ParseOrReloadImageTextConfigFileContent();
                #endregion

                
                ReloadDefaultConfig();
                ParsePrinterConfig();
                ParseAnnotationConfig();
                ParseWindowLevelConfig();

                Logger.LogFuncDown();
            }
            catch (Exception exp)
            {
                Logger.LogFuncException(exp.Message);
            }
        }

        public void ParseOrReloadImageTextConfigFileContent()
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[Printers.ParseOrReloadImageTextConfigFileContent]" + "[]" );

                //清空内容
                this.Modality2FilmingImageTextConfigContent.Clear();

                foreach (var entry in this.Modality2FilmingImageTextConfigPath)
                {
                    var xmlString = ConfigFileHelper.GetXmlDocument(entry.Value).WriteToString();
                    Modality2FilmingImageTextConfigContent.Add(entry.Key, xmlString);
                }
            
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevWarning("[Printers.ParseOrReloadImageTextConfigFileContent]" + "[Exception]" + e + "[Message]" + e.Message);
                Logger.Instance.LogSvcError(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcErrorImgTxtConfig,
"[Fail to read image text configure for filming]"  + "[Exception]" + e + "[Message]" + e.Message );
            }
        }

        public void ReloadDefaultConfig()
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[LoadDefaultConfig]");
                ParseFilmingConfig(_entryFilmingConfigPath);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevWarning("[Printers.ReloadDefaultConfig]" + "[Exception]" + e + "[Message]" + e.Message);
            }
        }

        public void ReloadPrintersConfig()
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[LoadDefaultConfig]");
            ParsePrinterConfig();
        }

        public void ReloadPresetWindowConfig()
        {
            ParseWindowLevelConfig();
            ParseWindowLevelPtConfig();
        }

        #endregion [--Constructor--]


        #region Properties

        public string FilmingImageTextPath { get; private set; }

        public string PrintObjectStoragePath
        {
            get;
            set;
        }

        public ushort OurPort
        {
            get { return _ourPort; }
            set { _ourPort = value; }
        }

        public string DefaultAE
        {
            get { return _defaultAE; }
            set
            {
                try
                {
                    if (_defaultAE == value)
                    {
                        return;
                    }
                    _defaultAE = value;
                    McsfFilmingInfo.DefaultPeerAE = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/DefaultPeerAE", value);
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "exception when setting default AE node, " + ex.StackTrace);
                }
            }
        }

        public string DefaultFilmSize
        {
            get { return _defaultFilmSize; }
            set
            {
                try
                {
                    if (_defaultFilmSize == value)
                    {
                        return;
                    }
                    _defaultFilmSize = value;
                    McsfFilmingInfo.DefaultFilmSize = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/DefaultFilmSize", value);
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "exception when setting default Film Size, " + ex.StackTrace);
                }
            }
        }

        public string DefaultMediumType
        {
            get { return _defaultMediumType; }
            set
            {
                try
                {
                    if (_defaultMediumType == value)
                    {
                        return;
                    }
                    _defaultMediumType = value;
                    McsfFilmingInfo.DefaultMediumType = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/DefaultMediumType", value);
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "exception when setting default Medium Type, " + ex.StackTrace);
                }
            }
        }

        public string DefaultFilmDestination
        {
            get { return _defaultFilmDestination; }
            set
            {
                try
                {
                    if (_defaultFilmDestination == value)
                    {
                        return;
                    }
                    _defaultFilmDestination = value;
                    McsfFilmingInfo.DefaultFilmDestination = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/DefaultFilmDestination", value);
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "exception when setting default Film Destination, " + ex.StackTrace);
                }
            }
        }

        public int DefaultPaperPrintDPI
        {
            get { return _defaultPaperPrintDPI; }
            set
            {
                try
                {
                    if (_defaultPaperPrintDPI == value)
                    {
                        return;
                    }
                    _defaultPaperPrintDPI = value;
                    McsfFilmingInfo.GeneralPrinterDPI = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/GeneralPrinterDPI", value);
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "exception when setting default GeneralPrinterDPI, " + ex.StackTrace);
                }
            }
        }

        public int DefaultOrientation
        {
            get { return _defaultOrientation; }
            set
            {
                try
                {
                    if (_defaultOrientation == value)
                    {
                        return;
                    }
                    _defaultOrientation = value;
                    McsfFilmingInfo.DefaultOrientation = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/DefaultOrientation", value);
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "exception when setting default Film Orientation, " + ex.StackTrace);
                }
            }
        }

        public int LocalizedImagePosition
        {
            get { return _localizedImagePosition; }
            set
            {
                try
                {
                    if (_localizedImagePosition == value)
                    {
                        return;
                    }
                    _localizedImagePosition = value;
                    McsfFilmingInfo.LocalizedImagePosition = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/LocalizedImagePosition", value);
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "exception when setting default Film Size, " + ex.StackTrace);
                }
            }
        }
        ///// <summary>
        ///// time threshold of connection to printer
        ///// </summary>
        public uint TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        public List<PeerNode> PeerNodes
        {
            get { return _peerNodes; }
            set { _peerNodes = value; }
        }

        public string OurAE
        {
            get { return _ourAE; }
            set { _ourAE = value; }
        }

        public bool CanAutoFilming
        {
            get { return _canAutoFilming; }
            set
            {
                try
                {
                    if (value == _canAutoFilming)
                    {
                        return;
                    }
                    //XmlDocument doc = new XmlDocument();
                    //doc.Load(_entryFilmingConfigPath);
                    //XmlNode xn = doc.SelectSingleNode("/Filming/EnableAutoFilming");
                    //XmlElement xe = (XmlElement)xn;
                    //int iValue = value ? 1 : 0;
                    //xe.InnerText = iValue.ToString();
                    //doc.Save(_entryFilmingConfigPath);
                    _canAutoFilming = value;
                    McsfFilmingInfo.EnableAutoFilming = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/EnableAutoFilming", value ? 1 : 0);
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "set autoFilming enable flag exception, " + ex.StackTrace);
                }
            }
        }

        public EnumAutoFilmStrategy AutoFilmStrategy
        {
            get { return _autoFilmStrategy; }
        }

        /// <key>\n 
        /// PRA:Yes \n
        /// Traced from: SSFS_PRA_Filming_ColorLUT \n
        /// Tests: N/A \n
        /// Description: lut file directory property \n
        /// Short Description: LutDirectory \n
        /// Component: Filming \n
        /// </key> \n
        public string LutFileDirectory
        {
            get { return _lutFileDirectoryPath; }
        }

        /// <key>\n 
        /// PRA:Yes \n
        /// Traced from: SSFS_PRA_Filming_ColorLUT \n
        /// Tests: N/A \n
        /// Description: lut files property \n
        /// Short Description: LutFiles \n
        /// Component: Filming \n
        /// </key> \n
        public List<string> LutFiles
        {
            get { return _lutFileList; }
        }

        public bool IfPrintSplitterLine
        {
            get { return _ifPrintSplitterLine; }
            private set { _ifPrintSplitterLine = value; }
        }

        public bool IfClearAfterAddFilmingJob
        {
            get { return _ifClearAfterAddFilmingJob; }
            set
            {
                try
                {
                    if (_ifClearAfterAddFilmingJob == value)
                    {
                        return;
                    }
                    _ifClearAfterAddFilmingJob = value;
                    McsfFilmingInfo.Clear = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/Clear", value ? 1 : 0);

                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "exception when setting flag of Clear image after adding filming job, " + ex.StackTrace);
                    //throw;
                }
            }
        }

        public bool IfClearAfterSaveEFilm
        {
            get { return _ifClearAfterSaveEFilm; }
            set
            {
                try
                {
                    if (_ifClearAfterSaveEFilm == value)
                    {
                        return;
                    }
                    _ifClearAfterSaveEFilm = value;
                    McsfFilmingInfo.ClearAfterSaveEFilm = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/ClearAfterSaveEFilm", value ? 1 : 0); 
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "exception when setting flag of Clear image after save EFilm" + ex.StackTrace);
                    //throw;
                }
            }
        }

        private bool _ifShutDownAfterPrint = false;

        public bool IfShutDownAfterPrint
        {
            get { return _ifShutDownAfterPrint; }
            set
            {
                try
                {
                    if (_ifShutDownAfterPrint == value)
                    {
                        return;
                    }
                    _ifShutDownAfterPrint = value;
                    McsfFilmingInfo.AutoShutDown = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/AutoShutDown", value ? 1 : 0);

                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "exception when setting flag of Auto Shut Down After Print, " + ex.StackTrace);
                    //throw;
                }
            }
        }

        private bool _ifColorPrint = false;

        public bool IfColorPrint
        {
            get { return _ifColorPrint; }
            set
            {
                try
                {
                    _ifColorPrint = value;
                    var index = PeerNodes.FindIndex(p => p.PeerAE == DefaultAE);
                    if (index >= 0)
                    {
                        if (PeerNodes[index].IsColorPrintingOptionChecked == value) return;
                        PeerNodes[index].IsColorPrintingOptionChecked = value;
                        if (PeerNodes[index].NodeType != PeerNodeType.GENERAL_PRINTER)
                        {
                            PrinterListInfo.CollectionOfPrinter[index].IsColorPrintingOptionChecked = value?"false":"true";
                            WriteBackToPrinterConfig();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "exception when setting flag of Color Print, " + ex.StackTrace);
                    //throw;
                }
            }
        }
        public bool IfSaveEFilmWhenFilming
        {
            get { return _ifSaveEFilmWhenFilming; }
            set
            {
                try
                {
                    if (_ifSaveEFilmWhenFilming == value)
                    {
                        return;
                    }

                    _ifSaveEFilmWhenFilming = value;
                    McsfFilmingInfo.Save = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/Save", value ? 1 : 0);

                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "exception when setting flag of Save E-film when adding filming job, " + ex.StackTrace);
                    //throw;
                }
            }
        }

        public IList<ImgTextItem> TextItems
        {
            get { return _textItems; }
        }

        public IList<FilmingWindowLevel> WindowLevels
        {
            get { return _windowLevels; }
        }

        public bool IfStandAlone
        {
            get { return _ifStandAlone; }
            set { _ifStandAlone = value; }
        }

        private bool _ifSaveEFilmsAvailable = true;
        public bool IfSaveEFilmsAvailable
        {
            get
            {
                if (!_ifSaveEFilmsAvailable && IfSaveEFilmWhenFilming)
                {
                    IfSaveEFilmWhenFilming = false;
                }
                return _ifSaveEFilmsAvailable;
            }
        }

        private bool _realSizePrintingAvailable = false;
        public bool RealSizePrintingAvailable
        {
            get { return _realSizePrintingAvailable; }
            set { _realSizePrintingAvailable = value; }
        }
        

        private bool _customizedLayoutVisible = true;
        public bool CustomizedLayoutVisible
        {
            get { return _customizedLayoutVisible; }
            set { _customizedLayoutVisible = value; }
        }

        private bool _ifSaveAsNewSeries = false;
        public bool IfSaveAsNewSeries { get { return _ifSaveAsNewSeries; } set { _ifSaveAsNewSeries = value; } }

        private string _newSeriesDescription = "3D-1";
        public string NewSeriesDescription { get { return _newSeriesDescription; } set { _newSeriesDescription = value; } }

        public bool IfSaveHighPrecisionEFilm
        {
            get { return _ifSaveHighPrecisionEFilm; }
        }

        public PrintQueueCollection GeneralPrinters
        {
            get { return _generalPrinters; }
            set { _generalPrinters = value; }
        }

        public bool IfShowImageRuler
        {
            get { return _ifShowImageRuler; }
            set
            {
                try
                {
                    if (_ifShowImageRuler != value)
                    {
                        _ifShowImageRuler = value;
                        McsfFilmingInfo.DisplayRuler = value;
                        WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/DisplayRuler", value.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "set Ruler enable flag exception, " + ex.StackTrace);
                }
            }
        }

        public int DisplayMode
        {
            get { return _displayMode; }
            set
            {
                try
                {
                    if (_displayMode == value) return;
                    _displayMode = value;
                    McsfFilmingInfo.ViewMode = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/ViewMode", value.ToString());
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "set displayMode exception, " + ex.StackTrace);
                }
            }
        }

        public bool IfRepack
        {
            get { return _ifRepack; }
            set
            {
                try
                {
                    if (_ifRepack == value) return;
                    _ifRepack = value;
                    McsfFilmingInfo.LastRepackSetting = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/LastRepackSetting", value.ToString());
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "set LastRepackSetting exception, " + ex.StackTrace);
                }
            }
        }
        public int RepackMode
        {
            get { return _repackMode; }
            set
            {
                try
                {
                    if (_repackMode == value) return;
                    _repackMode = value;
                    McsfFilmingInfo.RepackMode = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/RepackMode", value.ToString());
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        "set RepackMode exception, " + ex.StackTrace);
                }
            }
        }
        public int MultiSeriesCompareOrientIndex
        {
            get { return _multiSeriesCompareOrientIndex; }
            set
            {
                try
                {
                    if (_multiSeriesCompareOrientIndex == value) return;
                    _multiSeriesCompareOrientIndex = value;
                    McsfFilmingInfo.MultiSeriesCompareOrientIndex = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/MultiSeriesCompareOrientIndex", value.ToString());
                }
                catch (Exception e)
                {
                    Logger.LogError(
                        "set MultiSeriesCompareOrientIndex exception, " + e.StackTrace);
                }

            }
        }

        public int MultiSeriesCompareColIndex
        {
            get { return _multiSeriesCompareColIndex; }
            set
            {
                try
                {
                    if (_multiSeriesCompareColIndex == value) return;
                    _multiSeriesCompareColIndex = value;
                    McsfFilmingInfo.MultiSeriesCompareColIndex = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/MultiSeriesCompareColIndex", value.ToString());
                }
                catch (Exception e)
                {
                    Logger.LogError(
                        "set MultiSeriesCompareColIndex exception, " + e.StackTrace);
                }

            }
        }

        public int MultiSeriesCompareRowIndex
        {
            get { return _multiSeriesCompareRowIndex; }
            set
            {
                try
                {
                    if (_multiSeriesCompareRowIndex == value) return;
                    _multiSeriesCompareRowIndex = value;
                    McsfFilmingInfo.MultiSeriesCompareRowIndex = value;
                    WriteBackToMcsfFilmingConfigure(_entryFilmingConfigPath, "/Filming/MultiSeriesCompareRowIndex", value.ToString());
                }
                catch (Exception e)
                {
                    Logger.LogError(
                        "set MultiSeriesCompareRowIndex exception, " + e.StackTrace);
                }

            }
        }

        public IList<PresetLayout> PresetCellLayouts = new List<PresetLayout>
                                                     {
                                                         new PresetLayout(1, 1),
                                                         new PresetLayout(2, 1),
                                                         new PresetLayout(1, 2),
                                                         new PresetLayout(2, 2),
                                                         new PresetLayout(2, 3),
                                                         new PresetLayout(3, 2),
                                                         new PresetLayout(3, 3)
                                                     };

        public class PresetLayout
        {
            private int _row;
            private int _col;

            public PresetLayout(int row, int col)
            {
                _row = row;
                _col = col;
            }

            public int Row { get { return _row; } }
            public int Col { get { return _col; } }

            public void SetLayout(int index, int row, int col)
            {
                if (_row == row && _col == col) return;
                _row = row;
                _col = col;
                Printers.Instance.WriteBackPresetCellLayoutToConfigure(index, row, col);
            }
        }

        public IList<PresetViewPortLayout> PresetViewportLayouts = new List<PresetViewPortLayout>
                                                     {
                                                         new PresetViewPortLayout("Layout_01_1x2", "Origin"),
                                                         new PresetViewPortLayout("Layout_02_2x2", "Origin"),
                                                         new PresetViewPortLayout("Layout_03_2x1", "Origin"),
                                                         new PresetViewPortLayout("Layout_04_3x1", "Origin"),
                                                         new PresetViewPortLayout("Layout_05_1x3", "Origin"),
                                                         new PresetViewPortLayout("Layout_06_1x2(2x1,1x1)", "Origin"),
                                                     };

        public class PresetViewPortLayout
        {
            //private int _index;
            private string _name;
            private string _paraFolder;
            public PresetViewPortLayout(string name, string paraFolder)
            {
                _name = name;
                _paraFolder = paraFolder;
            }

            public string Name { get { return _name; } }
            public string ParaFolder { get { return _paraFolder; } }

            public void SetViewPortLayout(int index, string name, string paraFolder)
            {
                if (name == _name && _paraFolder == paraFolder) return;
                _paraFolder = paraFolder;
                _name = name;
                Printers.Instance.WriteBackPresetViewportLayoutToConfigure(index-1, _name, _paraFolder);
            }
        }

        #endregion


        #region Getter

        public int GetDefaultOrientation(string ae)
        {
            var peer = PeerNodes.FirstOrDefault((node) => node.PeerAE == ae);
            if (peer == null || peer.DefaultOrientation <= 0) return 0;
            return peer.DefaultOrientation;
        }

        public int GetMaxDensityOf(string ae)
        {
            var peer = PeerNodes.FirstOrDefault((node) => node.PeerAE == ae);
            if (peer == null || peer.MaxDensity <= 0) return FilmingUtility.DPI;
            return peer.MaxDensity;
        }

        public string GetDefaultFilmSizeOf(string ae)
        {
            var peer = PeerNodes.FirstOrDefault((node) => node.PeerAE == ae);
            if (peer == null) return null;
            return peer.DefaultFilmSize;
        }

        public int GetPacsNodeParametersByAE(string sNodeAE, ref PeerNode peerNode)
        {
            foreach (PeerNode node in _peerNodes)
            {
                if (sNodeAE == node.PeerAE)
                {
                    peerNode = node;
                    return 0;
                }
            }
            Logger.LogError("Can't find Node:" + sNodeAE + "from config file!");
            return -1;
        }

        /// <summary>
        /// TextItems factory
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<ImgTextItem> CreateTextItems(ImgTxtDisplayState type)
        {
            try
            {
                Logger.LogFuncUp();

                switch (type)
                {
                    case ImgTxtDisplayState.All:
                        if (null == _allShownTextItems)
                        {
                            _allShownTextItems = new List<ImgTextItem>();
                            foreach (var item in TextItems)
                            {
                                ImgTextItem it = item.Clone();
                                it.Show = true;
                                _allShownTextItems.Add(it);
                            }
                        }
                        return _allShownTextItems;
                    case ImgTxtDisplayState.None:
                        if (null == _allHidedTextItems)
                        {
                            _allHidedTextItems = new List<ImgTextItem>();
                            foreach (var item in TextItems)
                            {
                                ImgTextItem it = item.Clone();
                                it.Show = false;
                                _allHidedTextItems.Add(it);
                            }
                        }
                        return _allHidedTextItems;
                    case ImgTxtDisplayState.Customization:
                        if (null == _partiallyShownTextItems)
                        {
                            _partiallyShownTextItems = new List<ImgTextItem>();
                            foreach (var item in TextItems)
                            {
                                ImgTextItem it = item.Clone();
                                _partiallyShownTextItems.Add(it);
                            }
                        }
                        return _partiallyShownTextItems;
                    case ImgTxtDisplayState.Anonymous:
                        if (null == _annomiousTextItems)
                        {
                            _annomiousTextItems = new List<ImgTextItem>();
                            var items = CreateTextItems(ImgTxtDisplayState.All);
                            foreach (var item in items)
                            {
                                ImgTextItem it = item.Clone();
                                if ("PatientsName" == it.Tag)
                                    it.Show = false;
                                _annomiousTextItems.Add(it);
                            }
                        }
                        return _annomiousTextItems;
                    default:
                        Logger.LogFuncException("Not supported annotation type");
                        return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return null;
            }
        }

        #endregion

        #region Private Methods



        public McsfFilming McsfFilmingInfo;
        /// <summary>
        /// parser configure file to get filming configuration
        /// </summary>
        /// 
        private void ParseFilmingConfig(string filmingPath)
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ParseFilmingConfig]" + filmingPath);

            McsfFilmingInfo = ConfigFileHelper.LoadConfigObject<McsfFilming>(_entryFilmingConfigPath);
            _ourAE = McsfFilmingInfo.OurAE;
            _defaultAE = McsfFilmingInfo.DefaultPeerAE;
            _defaultFilmSize = McsfFilmingInfo.DefaultMediumType;
            _defaultMediumType = McsfFilmingInfo.DefaultMediumType;
            _defaultFilmDestination = McsfFilmingInfo.DefaultFilmDestination;
            _defaultOrientation = McsfFilmingInfo.DefaultOrientation;
            _defaultPaperPrintDPI = McsfFilmingInfo.GeneralPrinterDPI;
            _timeOut = McsfFilmingInfo.Timeout;
            _canAutoFilming = McsfFilmingInfo.EnableAutoFilming;
            _autoFilmStrategy = (EnumAutoFilmStrategy)(Convert.ToInt32(McsfFilmingInfo.AutoFilmStrategy));
            PrintObjectStoragePath = McsfFilmingInfo.PrintObjectStoragePath;
            _ifClearAfterAddFilmingJob = McsfFilmingInfo.Clear;
            _ifClearAfterSaveEFilm = McsfFilmingInfo.ClearAfterSaveEFilm;
            _ifSaveEFilmWhenFilming = McsfFilmingInfo.Save;
            _repackMode = McsfFilmingInfo.RepackMode;
            _ifShutDownAfterPrint = McsfFilmingInfo.AutoShutDown;
            _ifPrintSplitterLine = McsfFilmingInfo.WithSplitter;
            _ifStandAlone = McsfFilmingInfo.StandAlone;
            _ifSaveEFilmsAvailable = McsfFilmingInfo.SaveEFilm;
            _ifSaveHighPrecisionEFilm = McsfFilmingInfo.HighPrecisionEFilm;
            _ifSaveAsNewSeries = McsfFilmingInfo.SaveAsNewSeries;
            _newSeriesDescription = McsfFilmingInfo.NewSeriesDescription;
            _customizedLayoutVisible = true;
            _realSizePrintingAvailable = McsfFilmingInfo.RealSizePrintingAvailable;
            IfUseFilmingServiceFE = McsfFilmingInfo.UseFilmingServiceFE;
            _realSizePrintingAvailable = McsfFilmingInfo.RealSizePrintingAvailable;
            _ifShowImageRuler = McsfFilmingInfo.DisplayRuler;

            _displayMode = McsfFilmingInfo.ViewMode;
            _localizedImagePosition = McsfFilmingInfo.LocalizedImagePosition;
            _ifRepack = McsfFilmingInfo.LastRepackSetting;

            _multiSeriesCompareOrientIndex = McsfFilmingInfo.MultiSeriesCompareOrientIndex;

            _multiSeriesCompareRowIndex = McsfFilmingInfo.MultiSeriesCompareRowIndex;
            _multiSeriesCompareColIndex = McsfFilmingInfo.MultiSeriesCompareColIndex;


            for (int i = 0; i < 7; i++)
            {
                if (McsfFilmingInfo.PresetCellLayout.Count > i)
                {
                    var row = McsfFilmingInfo.PresetCellLayout[i].Rows;
                    var col = McsfFilmingInfo.PresetCellLayout[i].Columns;
                    PresetCellLayouts[i] = new PresetLayout(row, col);
                }
                else
                {
                    PresetCellLayouts[i] = new PresetLayout(i+1,i+1);
                }
            }
            for (int i = 0; i < 6; i++)
            {
                if (McsfFilmingInfo.PresetViewportLayout.Count > i)
                {
                    var sName = McsfFilmingInfo.PresetViewportLayout[i].Name;
                    var sParaFolder = McsfFilmingInfo.PresetViewportLayout[i].ParaFolder;
                    PresetViewportLayouts[i] = new PresetViewPortLayout(sName, sParaFolder);
                }
            }
        }

        /// <summary>
        /// parser configure file to get printer configuration
        /// </summary>
        private void ParsePrinterConfig()
        {
            try
            {

                PrinterListInfo = ConfigFileHelper.LoadConfigObject<PrinterList>(_entryPrinterConfigPath);
                PeerNodes.Clear();
                foreach (var printerModel in PrinterListInfo.CollectionOfPrinter)
                {
                    var tempPeerNode = new PeerNode();
                    tempPeerNode.PeerAE = printerModel.peerAE;
                    tempPeerNode.PeerPort = Convert.ToUInt16(printerModel.peerPort);
                    tempPeerNode.PeerIP = printerModel.peerIP;
                    tempPeerNode.PrinterDiscription = printerModel.description;
                    tempPeerNode.SupportLayoutList = printerModel.DisplayFormat.Split('\\').ToList();

                    var filmSizeList = printerModel.FilmSizeID.Split('\\').ToList();
                    foreach (var filmSize in filmSizeList)
                    {
                        tempPeerNode.SupportFilmSizeList.Add(filmSize);
                    }

                    tempPeerNode.AllowAutoFilming = Convert.ToBoolean(printerModel.AllowAutoFilming);
                    tempPeerNode.DefaultFilmSize = printerModel.DefaultFilmSize;
                    tempPeerNode.MaxDensity = Convert.ToInt32(printerModel.MaxDensity);
                    tempPeerNode.DefaultOrientation = Convert.ToInt32(printerModel.DefaultOrientation);

                    var mediumTypeList = printerModel.MediumType.Split('\\').ToList();
                    foreach (var mediumType in mediumTypeList)
                    {
                        tempPeerNode.SupportMediumTypeList.Add(mediumType);
                    }

                    var filmDestinationList = printerModel.FilmDestination.Split('\\').ToList();
                    foreach (var filmDestination in filmDestinationList)
                    {
                        tempPeerNode.SupportFilmDestinationList.Add(filmDestination);
                    }

                    tempPeerNode.IsColorPrintingOptionChecked =
                        Convert.ToBoolean(printerModel.IsColorPrintingOptionChecked);

                    var correctedRatioForRealSizeList = printerModel.CorrectedRatioForRealSizePrint.Split('\\').ToList();
                    foreach (var ratioAndFilmSize in correctedRatioForRealSizeList)
                    {
                        var infoList = ratioAndFilmSize.Split(':').ToList();
                        if (infoList.Count != 2) continue;
                        var ratio = Convert.ToDouble(infoList[1]);
                        tempPeerNode.CorrectedRatioForRealSizeConfig.Add(infoList[0], ratio);
                    }
                    tempPeerNode.Index = PeerNodes.Count;
                    PeerNodes.Add(tempPeerNode);
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            } 
            string modalityName;
            mcsf_clr_systemenvironment_config.GetModalityName(out modalityName);
            if (modalityName.ToUpper() != "MR" && modalityName.ToUpper() != "PETMR")
            {
                ParsePrinterTypesSetConfig();
                GetGeneralPrinters();
            }
        }
        

        private void ParsePrinterTypesSetConfig()
        {

            try
            {
                Logger.LogFuncUp();
                if (string.IsNullOrEmpty(_entryFilmingMiscellaneousPath))
                {
                    Logger.LogWarning(
                         "Unable to open Filming Miscellaneous config file. ");
                    return;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(_entryFilmingMiscellaneousPath);
                XmlNode root = null;
                XmlNode printerfilter = null;
                foreach (XmlNode node in doc.ChildNodes)
                {
                    if (node.Name == "Miscellaneous")
                    {
                        root = node;
                        break;
                    }
                }
                if (root != null)
                {
                    foreach (XmlNode node in root.ChildNodes)
                    {
                        if (node.Name == "PrinterFilter")
                        {
                            printerfilter = node;
                            break;
                        }
                    }
                }
                if (printerfilter == null || string.IsNullOrWhiteSpace(printerfilter.InnerText))
                {

                    _strPrintQueueTypes.Add("Connections");

                    Logger.LogWarning(
                         "Unable to find node PrinterFilter.");
                    return;
                }
                _strPrintQueueTypes = printerfilter.InnerText.Split('\\').ToList();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogDevWarning(/*Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnConfigureFile,*/
"[Fail to parse file]" + _entryFilmingMiscellaneousPath);
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void GetGeneralPrinters()
        {

            try
            {
                Logger.LogFuncUp();

                var printQueueTypes = new List<EnumeratedPrintQueueTypes>();
                _generalPrinters = new PrintQueueCollection();

                if (_strPrintQueueTypes != null)
                {
                    foreach (var strPrintQueueType in _strPrintQueueTypes)
                    {
                        var type = new EnumeratedPrintQueueTypes();
                        if (Enum.TryParse<EnumeratedPrintQueueTypes>(strPrintQueueType, true, out type))
                            printQueueTypes.Add(type);
                        var tempprinters = new LocalPrintServer().GetPrintQueues(printQueueTypes.ToArray());
                        foreach (var tempprinter in tempprinters)
                        {
                            if (_generalPrinters.All(t => t.Name != tempprinter.Name))
                                _generalPrinters.Add(tempprinter);
                        }

                    }
                }

                //var printers = new LocalPrintServer().GetPrintQueues(
                //    new[] { 
                //        //EnumeratedPrintQueueTypes.Queued						,
                //        //EnumeratedPrintQueueTypes.DirectPrinting				,
                //        //EnumeratedPrintQueueTypes.Shared						,
                //        //EnumeratedPrintQueueTypes.Connections 				,
                //        //EnumeratedPrintQueueTypes.Local						,
                //        //EnumeratedPrintQueueTypes.EnableDevQuery 				,
                //        //EnumeratedPrintQueueTypes.KeepPrintedJobs				,
                //        //EnumeratedPrintQueueTypes.WorkOffline					,
                //        //EnumeratedPrintQueueTypes.EnableBidi 					,
                //        //EnumeratedPrintQueueTypes.RawOnly 					,
                //        //EnumeratedPrintQueueTypes.PublishedInDirectoryServices,
                //        //EnumeratedPrintQueueTypes.Fax							,
                //        //EnumeratedPrintQueueTypes.TerminalServer				,
                //        //EnumeratedPrintQueueTypes.PushedUserConnection		,
                //        //EnumeratedPrintQueueTypes.PushedMachineConnection		 
                //    });
                //var printers = new LocalPrintServer().GetPrintQueues();
                foreach (var printer in _generalPrinters)
                {
                    var node = new PeerNode(PeerNodeType.GENERAL_PRINTER);
                    node.PeerAE = printer.Name;
                    node.SupportFilmSizeList.Add(PageMediaSizeName.ISOA4);
                    node.SupportFilmSizeList.Add(PageMediaSizeName.ISOA3);
                    node.SupportFilmSizeList.Add(PageMediaSizeName.NorthAmerica14x17);
                    node.IsColorPrintingOptionChecked = true;
                    PeerNodes.Add(node);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void ParseAnnotationConfig()
        {
            Logger.LogFuncUp();

            try
            {
                if (string.IsNullOrEmpty(_entryAnnotationConfigPath))
                {
                    Logger.LogWarning(
                         "Unable to open text item config file. ");
                    return;
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(_entryAnnotationConfigPath);
                XmlNode root = null;
                foreach (XmlNode node in doc.ChildNodes)
                {
                    if (node.Name == "McsfMedViewerTextItem")
                    {
                        root = node;
                        break;
                    }
                }
                if (root == null)
                {
                    Logger.LogWarning(
                         "Unable to find node McsfMedViewerTextItem.");
                    return;
                }

                foreach (XmlNode node in root.ChildNodes) // for each text item (dicom tag)
                {
                    if (node.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    var textItem = new ImgTextItem();
                    textItem.Tag = node.Name;
                    foreach (XmlNode subNode in node.ChildNodes) // for each attribute
                    {
                        try
                        {
                            if (subNode.NodeType != XmlNodeType.Element)
                            {
                                continue;
                            }

                            switch (subNode.Name)
                            {
                                case "Format":
                                    Regex regex = new Regex(@"{.*}");
                                    textItem.Format = regex.Replace(subNode.InnerText, ".*");
                                    break;
                                case "Show":
                                    textItem.Show = Convert.ToBoolean(subNode.InnerText);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogFuncException(ex.Message + ex.StackTrace);
                        }
                    }

                    TextItems.Add(textItem);
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            Logger.LogFuncDown();
        }

       

        /// <summary>
        /// Window Level and width configuration read from "appdata\appcommon\config\app_miscellaneous.xml"
        /// </summary>
        private void ParseWindowLevelConfig()
        {
            try
            {
                var appMisReader = AppMiscellaneousReader.Instance;
                var appMisConfigInfo = appMisReader.Analyze();
                var presetWindows = appMisConfigInfo.Windowing.Protocols.Where(protocols => protocols.Modality == Modality.CT);
                foreach(var presetItem in presetWindows)
                {
                   foreach(var item in presetItem.Items)
                   {
                       var filmingWindowLevel = new FilmingWindowLevel();
                       filmingWindowLevel.Name = item.Name;
                       filmingWindowLevel.Center = item.WC;
                       filmingWindowLevel.Width = item.WW;
                       _windowLevels.Add(filmingWindowLevel);
                   }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                Logger.Instance.LogDevWarning(/*Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnConfigureFile,*/
                    "[Fail to parse file]" + _entryWindowLevelConfigPath);
            }
        }

        public string HeadTB = "";
        public string BodyTB = "";
        public  void ParseWindowLevelPtConfig()
        {
            try
            {
                Logger.LogFuncUp();
                string headT = "";
                string headB="";
                string bodyT = "";
                string bodyB = "";
                var basicConfig = MedViewerConfiguration.BasicConfig;
                headT = basicConfig.TBValue.HeadT.ToString();
                headB = basicConfig.TBValue.HeadB.ToString();
                bodyT = basicConfig.TBValue.BodyT.ToString();
                bodyB = basicConfig.TBValue.BodyB.ToString();
                HeadTB = headT + "|" + headB;
                BodyTB = bodyT + "|" + bodyB;
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                Logger.Instance.LogDevWarning("[Fail to parse file]" + _entryWindowLevelPTConfigPath);
            }
        }

        private void WriteBackPresetCellLayoutToConfigure(int index, int row, int col)
        {
            var functionInfo = "[WriteBackPresetCellLayoutToConfigure] : index = " + index + " , row = " + row +
                               " , col = " + col;
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + functionInfo);
                McsfFilmingInfo.PresetCellLayout[index].Rows = row;
                McsfFilmingInfo.PresetCellLayout[index].Columns = col;
                ConfigFileHelper.SaveConfigObject(McsfFilmingInfo,_entryFilmingConfigPath);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + functionInfo);
            }
        }

        private void WriteBackPresetViewportLayoutToConfigure(int index, string name, string paraFolder = "Origin")
        {
            var functionInfo = "[WriteBackPresetViewportLayoutToConfigure] : index = " + index + " , name = " + name +
                               " , paraFolder = " + paraFolder;
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + functionInfo);
                McsfFilmingInfo.PresetViewportLayout[index].Name = name;
                McsfFilmingInfo.PresetViewportLayout[index].ParaFolder = paraFolder;
                ConfigFileHelper.SaveConfigObject(McsfFilmingInfo, _entryFilmingConfigPath);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + functionInfo);
            }
        }

        private void WriteBackToMcsfFilmingConfigure(string configureFile, string path, object value)
        {
            Logger.Instance.LogDevInfo( "[WriteBackToConfigure]: File : " + configureFile + " , Path : " + path + " , Value = " + value);
            ConfigFileHelper.SaveConfigObject(McsfFilmingInfo, _entryFilmingConfigPath);
        }

        private void WriteBackToPrinterConfig()
        {
            ConfigFileHelper.SaveConfigObject(PrinterListInfo,_entryPrinterConfigPath);
        }




        #endregion

        #region Fields

        private string _entryPrinterConfigPath;
        private string _entryFilmingConfigPath;
        private string _entryAnnotationConfigPath;
        private string _entryWindowLevelConfigPath;
        private string _entryFilmingMiscellaneousPath;
        private string _entryWindowLevelPTConfigPath;

        private string _ourAE;

        private ushort _ourPort;

        private string _defaultAE;

        private string _defaultFilmSize;

        private string _defaultMediumType;

        private string _defaultFilmDestination;

        private int _defaultPaperPrintDPI = FilmingUtility.DPI;

        private int _defaultOrientation = 0;

        private List<string> _strPrintQueueTypes = new List<string>();

        private uint _timeOut = 30;

        private List<PeerNode> _peerNodes = new List<PeerNode>();

        private bool _canAutoFilming;

        private EnumAutoFilmStrategy _autoFilmStrategy;

        private string _lutFileDirectoryPath = string.Empty;

        private List<string> _lutFileList = new List<string>();

        private bool _ifClearAfterAddFilmingJob = false;

        private bool _ifClearAfterSaveEFilm = false;

        private bool _ifSaveEFilmWhenFilming = false;

        private IList<ImgTextItem> _textItems = new List<ImgTextItem>();

        private IList<ImgTextItem> _allShownTextItems;

        private IList<ImgTextItem> _allHidedTextItems;

        private IList<ImgTextItem> _partiallyShownTextItems;

        private IList<ImgTextItem> _annomiousTextItems;

        private IList<FilmingWindowLevel> _windowLevels = new List<FilmingWindowLevel>();

        private bool _ifSaveHighPrecisionEFilm = false;

        private PrintQueueCollection _generalPrinters;
        public bool IfUseFilmingServiceFE = true;
        private bool _ifShowImageRuler;
        private int _displayMode = 1;
        private bool _ifRepack = true;
        private int _repackMode = 0;
        private int _multiSeriesCompareOrientIndex = 0;
        private int _multiSeriesCompareColIndex = 1;
        private int _multiSeriesCompareRowIndex = 1;
        private bool _ifPrintSplitterLine;
        private bool _ifStandAlone;
        private int _localizedImagePosition;

        #endregion


        #region FileParser

        public IFileParserCSharp GetFileParser()
        {
            try
            {
                if (_fileParser == null)
                {
                    _fileParser = ConfigParserFactory.Instance().CreateCSharpFileParser();
                    _fileParser.Initialize();
                }
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevWarning(e.Message);
            }
            return _fileParser;
        }

        private IFileParserCSharp _fileParser;

        #endregion



        public PrinterList PrinterListInfo { get; set; }
    }

    internal static class XmlDocumentEx
    {
        public static string WriteToString(this XmlDocument document)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(stream, null);
                writer.Formatting = Formatting.Indented;
                document.Save(writer);
                StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                stream.Position = 0;
                string xmlString = sr.ReadToEnd();
                sr.Close();
                stream.Close();
                stream.Dispose();
                sr.Dispose();
                return xmlString;
            }
            catch (Exception e)
            {
                Logger.LogFuncException(e.Message);
                return string.Empty;
            }
        }
    }

    public enum EnumAutoFilmStrategy
    {
        INCREMENTAL = 0,
        COMPLETE = 1
    }

    #region Printer相关
        [XmlRoot("Root")]
    public class PrinterList
    {
        #region Properties

        [XmlArrayItem("Item")]
        [XmlArray("Printer")]
        public Collection<PrinterItem> CollectionOfPrinter;

        #endregion
    }
        public class PrinterItem
    {
        #region Properties
        [XmlAttribute] 
        public string peerAE;
        [XmlAttribute] 
        public string peerIP;
        [XmlAttribute] 
        public string peerPort;
        [XmlAttribute] 
        public string description;
        [XmlAttribute] 
        public string type;
        [XmlAttribute] 
        public string BorderDensity;
        [XmlAttribute]
        public string DisableNewVRs;
        [XmlAttribute]
        public string DisplayFormat;
        [XmlAttribute]
        public string EmptyImageDensity;
        [XmlAttribute]
        public string FilmDestination;
        [XmlAttribute] 
        public string CorrectedRatioForRealSizePrint="";
        [XmlAttribute]
        public string FilmSizeID;
        [XmlAttribute]
        public string ImplicitOnly;
        [XmlAttribute]
        public string DefaultFilmSize;
        [XmlAttribute]
        public string DefaultOrientation;
        [XmlAttribute]
        public string MagnificationType;
        [XmlAttribute]
        public string MaxDensity;
        [XmlAttribute]
        public string MaxPDU;
        [XmlAttribute]
        public string MediumType;
        [XmlAttribute]
        public string MinDensity;
        [XmlAttribute]
        public string OmitSOPClassUIDFromCreateResponse;
        [XmlAttribute]
        public string PresentationLUTMatchRequired;
        [XmlAttribute]
        public string PresentationLUTinFilmSession;
        [XmlAttribute]
        public string ResolutionID;
        [XmlAttribute]
        public string SmoothingType;
        [XmlAttribute]
        public string Supports12Bit;
        [XmlAttribute]
        public string SupportsDecimateCrop;
        [XmlAttribute]
        public string SupportsImageSize;
        [XmlAttribute]
        public string SupportsPresentationLUT;
        [XmlAttribute]
        public string SupportsTrim;
        [XmlAttribute]
        public string AllowAutoFilming;
        [XmlAttribute]
        public string IsColorPrintingOptionChecked;
        [XmlAttribute]
        public string BasicGrayOrColorSettingControl;
        #endregion
    }   
    #endregion
    #region McsfFilming
    [XmlRoot("Root")]
    public class McsfFilming
    {
        public int AutoFilmStrategy;

        public bool EnableAutoFilming;

        public string OurAE;

        public string DefaultPeerAE;

        public string DisplayPrinterSetting;

        public string DefaultFilmSize;

        public int DefaultOrientation;
 
        public string DefaultMediumType;

        public string DefaultFilmDestination;

        public uint Timeout = 30;

        public string PrintObjectStoragePath;

        public bool WithSplitter = true;

        public bool Clear = true;

        public bool Save = false;

        public string ColorPrint;

        public bool AutoShutDown = false;


        public bool SaveEFilm = false;

        public bool HighPrecisionEFilm = false;

        public bool SaveAsNewSeries = false;

        public string NewSeriesDescription;

        public bool StandAlone = false;
        public bool LastRepackSetting = true;
        public int RepackMode = 0;
        public bool RealSizePrintingAvailable = false;

        public bool DisplayRuler = true;

        [XmlArrayItem("Item")]
        public Collection<PresetCellLayout> PresetCellLayout;

        [XmlArrayItem("Item")]
        public Collection<PresetViewportLayout> PresetViewportLayout;

        public int GeneralPrinterDPI = FilmingUtility.DPI;
        public bool UseFilmingServiceFE = true;
        public int ViewMode =1;
        public int MultiSeriesCompareOrientIndex = 0;
        public int MultiSeriesCompareRowIndex = 1;
        public int MultiSeriesCompareColIndex = 1;
        public int LocalizedImagePosition = 2;
        public bool ClearAfterSaveEFilm = false;
    }

    public class PresetCellLayout
    {
        [XmlAttribute]
        public int Columns;
        [XmlAttribute]
        public int Index;
        [XmlAttribute]
        public int Rows;
    }
    public class PresetViewportLayout
    {
        [XmlAttribute] 
        public int Index;
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string ParaFolder;
    }
    #endregion


}
