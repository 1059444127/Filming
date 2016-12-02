using System;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming
{
    public enum PeerNodeType
    {
        FILM_PRINTER = 0,
        GENERAL_PRINTER = 1         //only concerns PeerAE
    }

    /// <summary>
    /// AE Node
    /// </summary>
    public class PeerNode
    {
        public PeerNode(PeerNodeType type = PeerNodeType.FILM_PRINTER) { NodeType = type; }

        #region Properties

        public int Index { get; set; }

        public PeerNodeType NodeType
        {
            get; set;
        }

        public System.String PeerAE
        {
            get { return _peerAE; }
            set { _peerAE = value; }
        }

        public ushort PeerPort
        {
            get { return peerPort; }
            set { peerPort = value; }
        }

        public string PeerIP
        {
            get { return _peerIP; }
            set { _peerIP = value; }
        }

        public bool AllowAutoFilming
        {
            get { return _canAutoFilming; }
            set { _canAutoFilming = value; }
        }

        public bool IsColorPrintingOptionChecked
        {
            get { return _isColorPrintingOptionChecked; }
            set { _isColorPrintingOptionChecked = value; }
        }
        public string DefaultFilmSize
        {
            get { return _defaultFilmSize; }
            set { _defaultFilmSize = value; }
        }

        public int MaxDensity
        {
            get { return _maxDensity; }
            set { _maxDensity = value; }
        }

        public Dictionary<string,double> CorrectedRatioForRealSizeConfig
        {
            get { return _correctedRatioForRealSizeConfig; }
            set { _correctedRatioForRealSizeConfig = value; }
        }

        public string PrinterDiscription { get; set; }
        public List<string> SupportLayoutList { get; set; }
        private List<object> _supportFilmSizeList = new List<object>();
        public List<object> SupportFilmSizeList { get { return _supportFilmSizeList; } set { _supportFilmSizeList = value; } }

        private List<object> _supportMediumTypeList = new List<object>();
        public List<object> SupportMediumTypeList { get { return _supportMediumTypeList; } set { _supportMediumTypeList = value; } }
        
        private List<object> _supportFilmDestinationList = new List<object>();
        public List<object> SupportFilmDestinationList { get { return _supportFilmDestinationList; } set { _supportFilmDestinationList = value; } }

        public int DefaultOrientation
        {
            get { return _defaultOrientation; }
            set { _defaultOrientation = value; }
        }

        public override string ToString()
        {
            return PrinterDiscription + "'s AE:" + PeerAE + ";IP:" + PeerIP + ";Port:" + PeerPort;
        }
        #endregion

        #region Fields

        private String _peerAE;
        private ushort peerPort;
        private string _peerIP;
        private bool _canAutoFilming;
        private bool _isColorPrintingOptionChecked;
        private string _defaultFilmSize;
        private int _maxDensity;
        private int _defaultOrientation = 0;
        private Dictionary<string,double> _correctedRatioForRealSizeConfig = new Dictionary<string,double>{};

        #endregion
    }
}
