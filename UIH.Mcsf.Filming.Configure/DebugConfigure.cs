using System.Xml.Serialization;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Configure
{
    public class DebugConfigure 
    {
        private readonly string _filePath;
        public DebugConfigure(string filePath)
        {
            _filePath = filePath;
            var debug = ConfigFileHelper.LoadConfigObject<DebugConfigureInfo>(_filePath);
            StandAlone = debug.StandAlone;
            PrinterDPI = debug.PrinterDPI;
        }


        public bool StandAlone { get; private set; }
        public int PrinterDPI { get; private set; } 
    }

    [XmlRoot("Root")]
    public class DebugConfigureInfo
    {
        public bool StandAlone;

        public int PrinterDPI;
    }

}