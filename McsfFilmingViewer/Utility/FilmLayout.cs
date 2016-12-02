using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UIH.Mcsf.Core;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Utility
{
    public enum  LayoutTypeEnum
    {
        StandardLayout,
        DefinedLayout,
        RegularLayout
    }

    public class FilmLayout 
        //: IEqualityComparer<FilmLayout>
    {
        public static readonly string RegularLayoutString = "_" + LayoutTypeEnum.RegularLayout;

        public const int MaxRowCount = 10;
        public const int MaxColCount = 10;

        public string LayoutName { get; private set; }

        public LayoutTypeEnum LayoutType { get; set; }

        public int LayoutRowsSize { get; private set; }

        public int LayoutColumnsSize { get; private set; }

        private string _layoutXmlFileStream;
        public string LayoutXmlFileStream
        {
            get { return _layoutXmlFileStream; }
            set { _layoutXmlFileStream = value; }
        }

        private static string _layoutStringTemplate;
        //public static bool Equals(FilmLayout x, FilmLayout y)    //比较x和y对象是否相同
        //{
        //    if ((x.LayoutRowsSize == y.LayoutRowsSize) && (x.LayoutColumnsSize == y.LayoutColumnsSize))
        //    {
        //        return true;
        //    }
        //    else
        //        return false;
        //}
        public  bool Compare(object obj)
        {
            var layout = obj as FilmLayout;
            if (layout == null) 
                return false;
            return LayoutColumnsSize == layout.LayoutColumnsSize && LayoutRowsSize == layout.LayoutRowsSize;
        }
        public override bool Equals(object obj)
        {
            var layout = obj as FilmLayout;
            if (layout == null) return false;
            if (LayoutType != layout.LayoutType) return false;

            if (LayoutType == LayoutTypeEnum.StandardLayout)
                return LayoutColumnsSize == layout.LayoutColumnsSize && LayoutRowsSize == layout.LayoutRowsSize;
            return LayoutXmlFileStream == layout.LayoutXmlFileStream;
        }

        private readonly int _index = 0;
        public override int GetHashCode()
        {
            return _index.GetHashCode();
        }

        //public override static bool Equals()

        //public int GetHashCode(FilmLayout obj)
        //{
        //    return obj.ToString().GetHashCode();
        //}

        /// <key>\n 
        /// PRA:Yes \n
        /// Traced from: SSFS_PRA_Filming_ImageScale \n
        /// Tests: N/A \n
        /// Description: film layout constructor \n
        /// Short Description: WYSWYG \n
        /// Component: Filming \n
        /// </key> \n
        private FilmLayout()
        {
        }

        public FilmLayout(int layoutRowsSize, int layoutColumnsSize, string name = null)
        {
            LayoutType = LayoutTypeEnum.StandardLayout;
            LayoutRowsSize = layoutRowsSize;
            LayoutColumnsSize = layoutColumnsSize;

            LayoutName = name ?? string.Format("{0}x{1}", LayoutColumnsSize, LayoutRowsSize);
        }

        public FilmLayout(string layoutXmlFileStream, string name = null)
        {
            LayoutType = LayoutTypeEnum.DefinedLayout;
            LayoutXmlFileStream = layoutXmlFileStream;

            LayoutName = name ?? "DefinedLayout";
        }

        public static FilmLayout CreateStandardLayout(string layoutXmlFileStream, string name = null)
        {
            int row;
            int col;

            var configures = XDocument.Parse(layoutXmlFileStream);
            var layoutCell = configures.Descendants().First(e => e.Name.LocalName == "LayoutCell");

            //complement

            var rowAttr = layoutCell.Attribute(XName.Get("Rows"));// Attributes().First(a => a.Name.LocalName.ToUpper() == "ROWS");
            int.TryParse(rowAttr.Value, out row);
            var colAttr = layoutCell.Attribute(XName.Get("Columns"));//.Attributes().First(a => a.Name.LocalName.ToUpper() == "COLUMNS");
            int.TryParse(colAttr.Value, out col);

            return new FilmLayout(row, col, name);
        }

        public static FilmLayout CreateDefinedLayout(int layoutRowsSize, int layoutColumnsSize)
        {
            if (layoutRowsSize < 1 || layoutColumnsSize < 1) return null;
            var medViewerLayout = new XElement("MedViewerLayout",
                new XElement("MaxImageCout", layoutRowsSize*layoutColumnsSize),
                new XElement("LayoutCell", new XAttribute("Columns", layoutColumnsSize)
                    , new XAttribute("Rows", layoutRowsSize)
                    , new XAttribute("SplitterWidth", 2)));
            var layoutElement = medViewerLayout.Element("LayoutCell");
            if (layoutElement == null) return null;
            for (int i=0; i < layoutRowsSize*layoutColumnsSize; i++)
            {
                layoutElement.Add(new XElement("LayoutCell"));
            }

            var layout = new FilmLayout(medViewerLayout.ToString()) {LayoutType = LayoutTypeEnum.DefinedLayout};
            return layout;
        }

        public FilmLayout Clone()
        {
            var newFilmLayout = new FilmLayout
                                    {
                                        LayoutName = LayoutName,
                                        LayoutType = LayoutType,
                                        LayoutRowsSize = LayoutRowsSize,
                                        LayoutColumnsSize = LayoutColumnsSize,
                                        LayoutXmlFileStream = LayoutXmlFileStream
                                    };

            return newFilmLayout;
        }
       


        private static readonly List<FilmLayout> _definedFilmRegionLayoutList = new List<FilmLayout>(); 

        public static IEnumerable<FilmLayout> DefinedFilmRegionLayoutList
        {
            get {
                return _definedFilmRegionLayoutList.Any() ? _definedFilmRegionLayoutList : LoadDefinedFilmLayout();
            }
        }

        //public static void SingleViewportLayoutReload()
        //{
        //    if (_definedFilmRegionLayoutList.Count<1) return;
        //    var layoutFilesStoredPath = mcsf_clr_systemenvironment_config.GetApplicationPath("FilmingConfigPath");
        //    layoutFilesStoredPath += @"\McsfMedViewerConfig\MedViewerLayouts";

        //    var layoutConfigFilesArray = Directory.GetFiles(layoutFilesStoredPath).ToList();

        //    layoutConfigFilesArray.RemoveAll(path => path.ToUpper().Contains("TEMP"));  //remove temp layout file generated by service
        //    if(layoutConfigFilesArray.Count<1) return;

        //    var layoutConfigFile = layoutConfigFilesArray[0];
        //    int index = 1;
        //    using (var sr = new StreamReader(layoutConfigFile))
        //    {
        //        var layout = new FilmLayout(sr.ReadToEnd(),
        //                                    index.ToString(CultureInfo.InvariantCulture));
        //        var layoutString = layout.LayoutXmlFileStream;
        //        ComplementLayout(ref layoutString);
        //        _definedFilmRegionLayoutList[0].LayoutXmlFileStream = layoutString;
        //    }
        //}

        private static IEnumerable<string> ParseOrderRuleFile(string path)
        {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(fileStream);
            var fileNames = new List<string>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.Contains(";")) continue;
                fileNames.Add(line);
            }
            reader.Close();
            fileStream.Close();
            fileStream.Dispose();
            reader.Dispose();
            return fileNames;
        }


        private static List<FilmLayout> LoadDefinedFilmLayout()
        {
            _definedFilmRegionLayoutList.Clear();
            var filmingConfigPath = mcsf_clr_systemenvironment_config.GetApplicationPath("FilmingConfigPath");
            var defaultlayoutFilePath = @"data\filming\config\mcsf_med_viewer_layout_type_00_1x1.xml";

            var sr00 = ConfigFileHelper.GetXmlDocument(defaultlayoutFilePath).WriteToString();
            var layout00 = new FilmLayout(sr00,
                                        "Layout_00_1x1");

            layout00.LayoutType = defaultlayoutFilePath.Contains("regular")
                                    ? LayoutTypeEnum.RegularLayout
                                    : LayoutTypeEnum.DefinedLayout;
            _definedFilmRegionLayoutList.Add(layout00);
           
            

            var layoutFilesStoredPath = filmingConfigPath + @"\McsfMedViewerConfig\MedViewerLayouts\Origin";

            var layoutConfigFilesArray = new List<string>();
            var layoutNames = new List<string>();
            //var layoutConfigFilesArray = Directory.GetFiles(layoutFilesStoredPath).ToList();

            //layoutConfigFilesArray.RemoveAll(path => path.ToUpper().Contains("TEMP"));  //remove temp layout file generated by service

            #region 读取配置文件来调整布局的顺序

            try
            {
                var orderRuleFilePath = layoutFilesStoredPath + "\\ViewportLayoutOrderRule.ini";
                var configureFileNames = ParseOrderRuleFile(orderRuleFilePath).ToList();
                layoutConfigFilesArray.AddRange(configureFileNames.Select(configureFileName => layoutFilesStoredPath + "\\" + configureFileName));
                layoutNames.AddRange(configureFileNames);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevWarning("[Exception]" + e + "[Message]" + e.Message +
                                              "[When adjust orders of view port layout]");
            }
            #endregion

            int index = 0;
            foreach (var layoutConfigFile in layoutConfigFilesArray)
            {
                using (var sr = new StreamReader(layoutConfigFile))
                {
                    var name = layoutNames[index].Substring(0, layoutNames[index].Length - 4);
                    var layout = new FilmLayout(sr.ReadToEnd(),
                                                name);
                    
                    layout.LayoutType = layoutConfigFile.Contains("regular")
                                            ? LayoutTypeEnum.RegularLayout
                                            : LayoutTypeEnum.DefinedLayout;
                    _definedFilmRegionLayoutList.Add(layout);
                }
                ++index;
            }

            if (_definedFilmRegionLayoutList.Count > 0)
            {
                var layout = _definedFilmRegionLayoutList.First();
                _layoutStringTemplate = layout.LayoutXmlFileStream;
                var layoutString = layout.LayoutXmlFileStream;
                ComplementLayout(ref layoutString);
                layout.LayoutXmlFileStream = layoutString;
                layout.LayoutType = LayoutTypeEnum.RegularLayout;
            }


            return _definedFilmRegionLayoutList;
        }

        private static void ComplementLayout(ref string layoutString)
        {
            try
            {
                Logger.LogFuncUp();

                var configures = XDocument.Parse(layoutString);
                var layoutCell = configures.Descendants().First(e => e.Name.LocalName == "LayoutCell");
                if (layoutCell == null) return;

                //complement
                var row = 0;
                var col = 0;
                var rowAttr = layoutCell.Attribute(XName.Get("Rows"));// Attributes().First(a => a.Name.LocalName.ToUpper() == "ROWS");
                if (rowAttr != null) int.TryParse(rowAttr.Value, out row);
                var colAttr = layoutCell.Attribute(XName.Get("Columns"));//.Attributes().First(a => a.Name.LocalName.ToUpper() == "COLUMNS");
                if (colAttr != null) int.TryParse(colAttr.Value, out col);

                var count = row * col;
                for (var i = 0; i < count; i++)
                {
                    layoutCell.Add(new XElement("LayoutCell"));
                }

                var stringStream = new StringWriter();
                configures.Save(stringStream);
                layoutString = stringStream.ToString();
                stringStream.Dispose();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private static void ComplementDefaultLayout(int row, int col, ref string layoutString)
        {
            try
            {
                Logger.LogFuncUp();
                var configures = XDocument.Parse(_layoutStringTemplate);
                var layoutCell = configures.Descendants().First(e => e.Name.LocalName == "LayoutCell");
                if (layoutCell == null) return;

                //complement

                var rowAttr = layoutCell.Attribute(XName.Get("Rows"));// Attributes().First(a => a.Name.LocalName.ToUpper() == "ROWS");
                if (rowAttr != null) rowAttr.SetValue(row);
                var colAttr = layoutCell.Attribute(XName.Get("Columns"));//.Attributes().First(a => a.Name.LocalName.ToUpper() == "COLUMNS");
                if (colAttr != null) colAttr.SetValue(col);

                var maxImageCount = configures.Descendants().First(e => e.Name.LocalName == "MaxImageCount");
                if (maxImageCount != null) maxImageCount.SetValue(row*col);
                
                var count = row * col;
                for (var i = 0; i < count; i++)
                {
                    layoutCell.Add(new XElement("LayoutCell"));
                }

                var stringStream = new StringWriter();
                configures.Save(stringStream);
                layoutString = stringStream.ToString();
                stringStream.Dispose();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
        private int _maxImagesCount;
        public int MaxImagesCount
        {
            get
            {
                if (LayoutType == LayoutTypeEnum.StandardLayout || string.IsNullOrWhiteSpace(LayoutXmlFileStream))
                {
                    _maxImagesCount = LayoutColumnsSize * LayoutRowsSize;
                }
                else// if (LayoutType == LayoutTypeEnum.DefinedLayout)
                {
                    if (_maxImagesCount == 0)
                    {
                        _maxImagesCount = GetDefinedLayoutMaxImageCount();
                    }
                }

                return _maxImagesCount;
            }
        }

        private int GetDefinedLayoutMaxImageCount()
        {
            try
            {
                int maxImageCount;
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(_layoutXmlFileStream);

                var root = xmlDoc.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "MedViewerLayout");
                if (root == null)
                {
                    Logger.LogError(
                         "Wrong format film layout config file: Root node is not MedViewerLayout.");
                    return 0;
                }

                maxImageCount = CaculateLayoutMaxImageCount(root);

                return maxImageCount;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }

            return 0;
        }

        private int CaculateLayoutMaxImageCount(XmlNode layoutNode)
        {
            int maxCellCount = 0;
            foreach (XmlNode childNode in layoutNode.ChildNodes)
            {
                if (childNode.Name == "LayoutCell")
                {
                    if (!childNode.HasChildNodes|| childNode.ChildNodes[0].Name == "ControlCell")
                    {
                        maxCellCount += GetIntAttributeFromLayoutNodeInXml(childNode, "Rows")
                            * GetIntAttributeFromLayoutNodeInXml(childNode, "Columns");
                    }
                    else//childNode.HasChildNodes
                    {
                        maxCellCount += CaculateLayoutMaxImageCount(childNode);
                    }
                }
            }
            return maxCellCount;
        }

        private int GetIntAttributeFromLayoutNodeInXml(XmlNode node, string attrName)
        {
            try
            {
                if (node.Attributes != null) return Convert.ToInt32(node.Attributes.GetNamedItem(attrName).InnerText);
            }
            catch (Exception ex)
            {
            	Logger.LogInfo(ex.StackTrace + ": " + node + " has no attribute with the name of " + attrName);
            }
            return 1;
        }

        public override string ToString()
        {
            return LayoutName;
        }

        //public FilmLayout CalculateFirstViewportCellLayout()
        //{
        //    if (LayoutType == LayoutTypeEnum.StandardLayout)
        //    {
        //        return this;
        //    }

        //    try
        //    {
        //        var xmlDoc = new XmlDocument();
        //        xmlDoc.LoadXml(_layoutXmlFileStream);

        //        XmlNode root = null;
        //        foreach (XmlNode node in xmlDoc.ChildNodes)
        //        {
        //            if (node.Name == "MedViewerLayout")
        //            {
        //                root = node;
        //                break;
        //            }
        //        }
        //        if (root == null)
        //        {
        //            Logger.LogError(
        //                 "Wrong format film layout config file: Root node is not MedViewerLayout.");
        //            return null;
        //        }

        //        return CalculateFirstViewportCellLayout(root);

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //        return null;
        //    }

        //}

        //private FilmLayout CalculateFirstViewportCellLayout(XmlNode layoutNode)
        //{
        //    foreach (XmlNode childNode in layoutNode.ChildNodes)
        //    {
        //        if (childNode.Name == "LayoutCell")
        //        {
        //            if (!childNode.HasChildNodes)
        //            {
        //                return new FilmLayout(GetIntAttributeFromLayoutNodeInXml(childNode, "Rows")
        //                    , GetIntAttributeFromLayoutNodeInXml(childNode, "Columns"));
        //            }
        //            else//childNode.HasChildNodes
        //            {
        //                return CalculateFirstViewportCellLayout(childNode);
        //            }
        //        }
        //    }
        //    return null;
        //}
        public static bool RefreshDefaultLayout(int customCellRows, int customCellColumns)
        {
            if (!_definedFilmRegionLayoutList.Any()) return false;
            var layout = _definedFilmRegionLayoutList.First();
            if (customCellColumns == 0 || customCellRows == 0) return false;
            if(layout.LayoutRowsSize == customCellRows && layout.LayoutColumnsSize == customCellColumns) return true;
            layout.LayoutRowsSize = customCellRows;
            layout.LayoutColumnsSize = customCellColumns;

            string layoutString = _layoutStringTemplate ;
            ComplementDefaultLayout(customCellRows, customCellColumns,ref layoutString);
            layout.LayoutXmlFileStream = layoutString;
            return true;
        }
    }
}
