using System.Windows;
using UIH.Mcsf.Core;
using UIH.Mcsf.NLS;

namespace UIH.Mcsf.Filming.Configure
{
    public class Environment
    {
        #region [--Singleton--]

        private static volatile Environment _instance;
        private static readonly object LockHelper = new object();
        private DebugConfigure _debugConfigure;
        private ProtocolsConfigure _protocolsConfigure;
        private ResourceDictionary _filmingNlsDictionary;
        private FilmingConfigure _filmingConfigure;
        private DefaultLayoutConfigure _defaultLayoutConfigure;
        private string _defaultLayoutConfigurePath;

        public static Environment Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockHelper)
                    {
                        if (_instance == null)
                            _instance = new Environment();
                    }
                }
                return _instance;
            }
        }

        #endregion //[--Singleton--]


        private Environment()
        {
            ApplicationPath = mcsf_clr_systemenvironment_config.GetApplicationPath(FilmingConfigureString);
            if (ApplicationPath == string.Empty) ApplicationPath = DefaultEntryPath;
            FilmingConfigurePath = ApplicationPath + FilmingConfigureFileName;
            MiscellaneousConfigurePath = ApplicationPath + MiscellaneousConfigureFileName;
            FilmTitleConfigurePath = ApplicationPath + FilmTitleConfigureFileName;
            PrintersConfigurePath = ApplicationPath + PrintersConfigureFileName;
            WindowLevelAndWidthConfigurePath = ApplicationPath + WindowLevelAndWidthConfigureRelativePath;
            FilmingLogConfigurePath = ApplicationPath + FilmingLogConfigureFileName;
            DebugConfigurePath =  DebugConfigureFileName;
            _defaultLayoutConfigurePath =  DefaultLayoutConfigureFileName;

            string modalityName;
            mcsf_clr_systemenvironment_config.GetModalityName(out modalityName);
            Modality = modalityName;

            var protocolsConfigureFilePathSuffix = GetProtocolsConfigureFilePathSuffix(modalityName);

            ProtocolsConfigureFilePath = ProtocolsConfigureFileName + protocolsConfigureFilePathSuffix + ".xml";
        }

        private string GetProtocolsConfigureFilePathSuffix(string modalityName)
        {
            var suffix = modalityName.ToUpper();
            return "For"+suffix;
        }

        #region [--Path--]

        public string ApplicationPath { get; private set; }
        public string FilmingConfigurePath { get; private set; }
        public string MiscellaneousConfigurePath { get; private set; }
        public string FilmTitleConfigurePath { get; private set; }
        public string PrintersConfigurePath { get; private set; }
        public string WindowLevelAndWidthConfigurePath { get; private set; }
        public string FilmingLogConfigurePath { get; private set; }
        public string DebugConfigurePath { get; private set; }
        public string ProtocolsConfigureFilePath { get; private set; }
        /// <summary>
        /// Zhenghe Modality, for example : CT / MR
        /// </summary>
        public string Modality { get; private set; }

        private const string FilmingConfigureString = "FilmingConfigPath";
        private const string FilmingConfigureFileName = "McsfFilming.xml";
        private const string MiscellaneousConfigureFileName = "Miscellaneous.xml";
        private const string FilmTitleConfigureFileName = "FilmingPage.xml";
        private const string PrintersConfigureFileName = "PrinterConfig.xml";
        private const string WindowLevelAndWidthConfigureRelativePath = "..\\..\\appcommon\\config\\app_miscellaneous.xml";
        private const string FilmingLogConfigureFileName = "McsfFilmingLog.xml";
        private const string DebugConfigureFileName = @"config\filming\config\McsfFilmingDebug.xml";
        private const string ProtocolsConfigureFileName = "config\\filming\\config\\ProtocolBindingLayouts";

        private const string DefaultLayoutConfigureFileName =
            @"data\filming\config\mcsf_med_viewer_layout_type_00_1x1.xml";

        private const string DefaultEntryPath = @"D:\UIH\appdata\filming\config\";
	    private const string FilmingNlsResourceName = "Filming";

        public  string FilmingUserConfigPath = @"config/filming";

        #endregion //[--Path--]

        public DefaultLayoutConfigure GetDefaultLayoutConfigure()
        {
            return _defaultLayoutConfigure ??
                   (_defaultLayoutConfigure = new DefaultLayoutConfigure(_defaultLayoutConfigurePath));
        }

        public FilmingConfigure GetFilmingConfigure()
        {
            return _filmingConfigure ?? (_filmingConfigure = new FilmingConfigure(FilmingConfigurePath));
        }

        public DebugConfigure GetDebugConfigure()
        {
            return _debugConfigure ?? (_debugConfigure = new DebugConfigure(DebugConfigurePath));
        }

        public ProtocolsConfigure GetProtocolsConfig()
        {
            return _protocolsConfigure ?? (_protocolsConfigure = new ProtocolsConfigure(ProtocolsConfigureFilePath));
        }

        public void ReloadProtocolsConfig()
        {
            if (_protocolsConfigure != null)
            {
                _protocolsConfigure.ReloadProtocols();
            }
        }
        public ResourceDictionary GetFilmingNlsDictionary()
        {
            return _filmingNlsDictionary ?? (_filmingNlsDictionary = ResourceMgr.Instance().Init(FilmingNlsResourceName));
        }
    }
}
