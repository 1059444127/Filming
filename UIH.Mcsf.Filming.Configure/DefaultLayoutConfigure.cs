using System;
using System.Xml.Serialization;
using UIH.Mcsf.Filming.Interface;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Configure
{
    public class DefaultLayoutConfigure 
    {
        private LayoutBase _layout;
        private readonly string _filePath;
        private readonly DefaultLayoutInfo _defaultLayoutInfo;
        
        public DefaultLayoutConfigure(string filePath)
        {
            _filePath = filePath;
            _defaultLayoutInfo = ConfigFileHelper.LoadConfigObject<DefaultLayoutInfo>(_filePath);
            _layout = LayoutFactory.Instance.CreateLayout(_defaultLayoutInfo.LayoutCell.Columns,
                _defaultLayoutInfo.LayoutCell.Rows);
        }
        public LayoutBase Layout
        {
            get { return _layout; }
            set
            {
                _layout = value;
                _defaultLayoutInfo.LayoutCell.Rows = _layout.Rows;
                _defaultLayoutInfo.LayoutCell.Columns = _layout.Columns;
                _defaultLayoutInfo.MaxImageCount = _layout.Columns * _layout.Rows;
                ConfigFileHelper.SaveConfigObject(_defaultLayoutInfo,_filePath);
            }
        }

        public void WriteBackToDefaultLayout(int row, int column)
        {
            _layout = LayoutFactory.Instance.CreateLayout(column,row);
            _defaultLayoutInfo.LayoutCell.Rows = row;
            _defaultLayoutInfo.LayoutCell.Columns = column;
            _defaultLayoutInfo.MaxImageCount = row * column;
            ConfigFileHelper.SaveConfigObject(_defaultLayoutInfo, _filePath);
        }
    }

    [XmlRoot("MedViewerLayout")]
    public class DefaultLayoutInfo
    {
        #region Properties
        [XmlElement]
        public int MaxImageCount;
        [XmlElement]
        public LayoutCell LayoutCell;
        #endregion
    }

    public class LayoutCell
    {
        #region Properties
        [XmlAttribute]
        public int Rows;
        [XmlAttribute]
        public int Columns;

        #endregion
    }
}
