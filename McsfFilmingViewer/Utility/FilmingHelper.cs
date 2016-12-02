using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.Wrappers;
using UIH.Mcsf.Pipeline.Data;
using UIH.Mcsf.Pipeline.Dictionary;
using UIH.Mcsf.Viewer;
using UIH.Mcsf.Utility;
using System.Diagnostics;
using System.Xml;


namespace UIH.Mcsf.Filming.Utility
{
    public static class FilmingHelper
    {
        //todo: performance optimization begin pool
        static FilmingHelper()
        {
            MedViewCellImpPool = new List<MedViewerControlCellImpl>();
            MedViewLayoutCellImpPool = new List<MedViewerLayoutCellImpl>();
        }
        //todo: performance optimization end
        public static object CallNonPublicMethod(object instance, string methodName, object[] param)
        {

            object result;
            try
            {
                Type type = instance.GetType();
                MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
                result = method.Invoke(instance, param);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            return result;
        }

        //todo: performance optimization begin pool
        public static List<MedViewerControlCellImpl> MedViewCellImpPool { get; set; }
        public static List<MedViewerLayoutCellImpl> MedViewLayoutCellImpPool { get; set; }
        //todo: performance optimization end
        public static BitmapSource CaptureScreen(Visual target, double dpiX, double dpiY)
        {
            if (target == null)
            {
                return null;
            }
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);

            if (double.IsInfinity(bounds.Width) || double.IsInfinity(bounds.Height))
            {
                var element = (target as FrameworkElement);
                bounds = new Rect(0, 0, element.ActualWidth, element.ActualHeight);
            }

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)Math.Round(bounds.Width * dpiX / 96.0),
                                                            (int)Math.Round(bounds.Height * dpiY / 96.0),
                                                            dpiX,
                                                            dpiY,
                                                            PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(target);
                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }
            rtb.Render(dv);
            return rtb;
        }

        public static string EntryPath
        {
            get
            {
                return mcsf_clr_systemenvironment_config
                    .GetApplicationPath("FilmingConfigPath");
            }
        }
        public static string LeaveFactoryFilmingCornerImageTextPath
        {
            get
            {
                return EntryPath + "McsfMedViewerConfig/MedViewerImageText/";
            }
        }
        public static string CustomizedImageTextPath
        {
            get
            {
                return EntryPath + "McsfMedViewerConfig/MedViewerImageText/";
            }
        }
        public static string EntryFilmingPageConfigPath
        {
            get { return EntryPath + "FilmingPage.xml"; }
        }
        public static string LeaveFactoryImageTextPath(string modality)
        {
            modality = modality.ToLower();
            if (modality.Trim() == "pt")
                modality = "mi";
            return @"config/filming/McsfMedViewerConfig/MedViewerImageText/" + "mcsf_med_viewer_image_text_" + modality + ".xml";
        }

        public const string MixedPatientName = "Mixed";
        public const string StarsString = "****";
        public const string EmptyString = "";

        public const ushort iSamplesPerPixel = 1;
        public const string sPhotometric = "MONOCHROME2";
        public const ushort bitsAllocated = 8;

        public static string LeaveFactoryTextItemPath(string modality)
        {
            modality = modality.ToLower();
            if (modality.Trim() == "pt")
                modality = "mi";
            return LeaveFactoryFilmingCornerImageTextPath + "mcsf_med_viewer_text_item_" + modality + ".xml";
        }

        public static void SetSUVTypeForCell(MedViewerControlCell cell, string SUVType)
        {
            try
            {
                Logger.LogFuncUp();
                if (CellModality(cell) != Modality.PT)
                {
                    return;
                }
                cell.Image.SetDataUnit(SUVType);
                cell.Refresh();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.StackTrace);
            }
        }

        private static object _syncObj = new object();
        public static void UpdateCornerTextDisplayData(DisplayData displaydata, ImgTxtDisplayState type, bool shouldRefresh = true)
        {
            lock (_syncObj)
            {
                try
                {
                    Logger.LogFuncUp();

                    if (null == displaydata)
                    {
                        Logger.LogError("displaydata is Null! ");
                        return;
                    }

                    if (!shouldRefresh)
                    {
                        ImgTxtDisplayState displayMode;
                        if (Enum.TryParse(type.ToString(), out displayMode))
                        {
                            displaydata.IsDirty = true;
                            displaydata.PState.DisplayMode = displayMode;
                            return;
                        }
                        Logger.LogError("Transfor ImgTxtDisplayState failed! ");
                        return;
                    }

                    var overlayText = (OverlayFilmingF1ProcessText)displaydata.GetOverlay(OverlayType.FilmingF1ProcessText);
                    if (overlayText == null)
                    {
                        throw new NullReferenceException("no overlayText to be updated");
                    }

                    var graphicImageTxt = overlayText.GraphicFilmingF1ProcessText as GraphicFilmingF1ProcessText;
                    if (graphicImageTxt == null)
                    {
                        throw new NullReferenceException("no graphic image text to be updated");
                    }

                    switch (type)
                    {
                        case ImgTxtDisplayState.Customization:
                            var displaymode = overlayText.ImgTxtDisplayMode.ToString(); //保留高级应用显示状态
                            overlayText.SetImageTextDisplayMode(displaymode);
                            break;
                        case ImgTxtDisplayState.None:
                            overlayText.SetImageTextDisplayMode(ImgTxtDisplayState.None.ToString());
                            displaydata.PState.DisplayMode = ImgTxtDisplayState.None;
                            break;
                    }
                    Logger.LogFuncDown();
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.StackTrace);
                }
            }
        }

        public static void RefereshDisplayMode(DisplayData displaydata)
        {
            try
            {
                Logger.LogFuncUp();

                //目前四角信息只有两种配置模式：1，自定义显示;2，不显示；没有出厂设置了
                switch (displaydata.PState.DisplayMode)
                {
                    //case ImgTxtDisplayState.All:
                    //    UpdateCornerTextDisplayData(displaydata, ImgTxtDisplayState.All);
                    //    break;
                    case ImgTxtDisplayState.Customization:
                        UpdateCornerTextDisplayData(displaydata, ImgTxtDisplayState.Customization);
                        break;
                    //case ImgTxtDisplayState.Customization2:
                    //    UpdateCornerTextDisplayData(displaydata, ImgTxtDisplayState.Partial2);
                    //    break;
                    case ImgTxtDisplayState.None:
                        UpdateCornerTextDisplayData(displaydata, ImgTxtDisplayState.None);
                        break;
                    //case ImgTxtDisplayState.FromApplication:
                    //    #region  [edit by jinyang.li_OK]
                    //    var overlayText = (OverlayFilmingText)displaydata.GetOverlay(OverlayType.FilmingText);
                    //    if (overlayText == null) throw new NullReferenceException("no overlayText to be updated");
                    //    var graphicImageTxt = overlayText.Graphics[0] as GraphicFilmingImageText;
                    //    if (graphicImageTxt == null) throw new NullReferenceException("no graphic image text to be updated");
                    //    overlayText.SetImageTextDisplayMode(ImgTxtDisplayState.FromApplication.ToString());
                    //    break;
                    //    #endregion
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.StackTrace);
            }
        }
        public static void UpdateCornerTextForFilmingPages(List<FilmingPageControl> entityFilmingPageList,
                                                           ImgTxtDisplayState type)
        {
            foreach (var film in entityFilmingPageList)
            {
                if (!film.IsVisible) film.IsBeenRendered = false;
                film.UpdateCornerText(type);
            }
        }

        public static void InsertUInt16DicomElement(DicomAttributeCollection dataHeader, Tag uInt16TagName, ushort tagValue)
        {
            var element = DicomAttribute.CreateAttribute(uInt16TagName);

            if (!element.SetUInt16(0, (ushort)tagValue))
            {
                Logger.LogWarning("Failed to Insert NULL " + uInt16TagName.ToString() + " to Data header");
            }
            dataHeader.AddDicomAttribute(element);
        }

        public static void InsertUInt32DicomElement(DicomAttributeCollection dataHeader, Tag uInt32TagName, UInt32 tagValue)
        {
            var element = DicomAttribute.CreateAttribute(uInt32TagName);

            if (!element.SetUInt32(0, (UInt32)tagValue))
            {
                Logger.LogWarning("Failed to Insert NULL " + uInt32TagName.ToString() + " to Data header");
            }
            dataHeader.AddDicomAttribute(element);
        }
        public static void InsertStringDicomElement(DicomAttributeCollection dataHeader, uint tagName, string tagValue)
        {
            var element = DicomAttribute.CreateAttribute(tagName);

            if (tagValue == null)
                return;
            if (!element.SetString(0, tagValue))
            {
                Logger.LogWarning("Failed to Insert NULL " + tagName.ToString() + " to Data header");
            }
            dataHeader.AddDicomAttribute(element);
        }
        public static void InsertStringDicomElement(DicomAttributeCollection dataHeader, Tag stringTagName, string tagValue)
        {
            var element = DicomAttribute.CreateAttribute(stringTagName);
            if (tagValue == null)
                return;
            if (!element.SetString(0, tagValue))
            {
                Logger.LogWarning("Failed to Insert NULL " + stringTagName.ToString() + " to Data header");
            }
            dataHeader.AddDicomAttribute(element);
        }

        public static void ModifyStringDicomElement(DicomAttributeCollection dataHeader, Tag stringTagName, string tagValue)
        {
            if (dataHeader.Contains(stringTagName))
                dataHeader.RemoveDicomAttribute(stringTagName);
            FilmingHelper.InsertStringDicomElement(dataHeader, stringTagName, tagValue);
        }

        public static void InsertBytesDicomElement(DicomAttributeCollection dataHeader, Tag stringTagName, byte[] tagValues)
        {
            var element = DicomAttribute.CreateAttribute(stringTagName);
            if (tagValues == null)
                return;

            if (!element.SetBytes(0, tagValues))
            {
                Logger.LogWarning("Failed to Insert NULL " + stringTagName.ToString() + " to Data header");
            }

            dataHeader.AddDicomAttribute(element);
        }

        public static void InsertStringArrayDicomElement(DicomAttributeCollection dataHeader, Tag stringTagName, string tagValue)
        {
            InsertStringArrayDicomElement(dataHeader, stringTagName, tagValue.Split('\\'));
        }

        public static void InsertStringArrayDicomElement(DicomAttributeCollection dataHeader, Tag stringTagName, string[] tagValues)
        {
            var element = DicomAttribute.CreateAttribute(stringTagName);
            for (int i = 0; i < tagValues.Length; i++)
            {
                if (!element.SetString(i, tagValues[i]))
                    Logger.LogWarning("Failed to Insert NULL " + stringTagName.ToString() + " to Data header");
            }
            dataHeader.AddDicomAttribute(element);
        }

        public static void AddConstDICOMAttributes(DicomAttributeCollection dataHeader, bool ifSaveImageAsGreyScale = true)
        {

            if (ifSaveImageAsGreyScale)
            {
                InsertUInt16DicomElement(dataHeader, Tag.SamplesPerPixel, iSamplesPerPixel);
                InsertStringDicomElement(dataHeader, Tag.PhotometricInterpretation, sPhotometric);
            }
            else
            {
                InsertUInt16DicomElement(dataHeader, Tag.SamplesPerPixel, 3);
                InsertStringDicomElement(dataHeader, Tag.PhotometricInterpretation, "RGB");
                InsertStringDicomElement(dataHeader, Tag.PlanarConfiguration, "0");
            }
            InsertUInt16DicomElement(dataHeader, Tag.BitsAllocated, bitsAllocated);
            InsertUInt16DicomElement(dataHeader, Tag.BitsStored, bitsAllocated);
            InsertUInt16DicomElement(dataHeader, Tag.HighBit, bitsAllocated - 1);
            InsertUInt16DicomElement(dataHeader, Tag.PixelRepresentation, 0);
        }

        public static string GetSerieNumber(string studyInstanceUid)
        {
            int serieNumber = 0;
            try
            {
                var db = FilmingDbOperation.Instance.FilmingDbWrapper;
                var serieses = db.GetSeriesListByStudyInstanceUID(studyInstanceUid);
                serieNumber = serieses.Max((series) => series.SeriesNumber);
            }
            catch
            {
                serieNumber = 0;
            }
            if (serieNumber < 8000)
            {
                serieNumber = 8000;
            }
            return Convert.ToString(serieNumber);
        }
        public static string GetTagValueFrom(DicomAttributeCollection dataHeader, uint tag)
        {
            try
            {
                if (!dataHeader.Contains(tag)) return string.Empty;
                var attr = dataHeader[tag];

                string value = string.Empty;

                byte[] bytePath;
                attr.GetBytes(0, out bytePath);

                if (null != bytePath)
                {
                    value = Encoding.UTF8.GetString(bytePath);
                }
                else
                {
                    attr.GetString(0, out value);
                }
                return value;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException("Exception: " + ex.StackTrace);
            }
            return string.Empty;
        }
        public static string GetDicomHeaderInfoByTagName(Dictionary<uint, string> dicomHeader, uint tag)
        {
            try
            {
                if (!dicomHeader.ContainsKey(tag))
                {
                    if (tag != ServiceTagName.OperatorsName) return string.Empty;

                    var studyInstancUid = GetDicomHeaderInfoByTagName(dicomHeader, ServiceTagName.StudyInstanceUID);
                    var db = FilmingDbOperation.Instance.FilmingDbWrapper;
                    var study = db.GetStudyByStudyInstanceUID(studyInstancUid);
                    if (study == null) return string.Empty;
                    return study.operatorsName;
                }
                var result = "";
                result = dicomHeader[tag];
                if (tag == ServiceTagName.PatientName)
                {
                    result = PatientNameConvertor.PatientNameConvertor.TrimAndReplacePatientName(result);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException("Exception: " + ex.StackTrace);
            }
            return string.Empty;
        }
        public static void PrintTimeInfo(string title)
        {
            Debug.Print(title + ":" + DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString());

        }

        #region [--ReadOrWriteMcsfFilming--]
        public static string McsfFilmingPath
        {
            get
            {
                return EntryPath + "McsfFilming.xml";
                //string filename = EntryPath + "McsfFilming.xml";
                //if (System.IO.File.Exists(filename))
                //{
                //    return filename;
                //}
                //else
                //{
                //    return null;
                //}
            }
        }
        public static string McsfFilmingBakPath
        {
            get
            {
                return EntryPath + "McsfFilmingbak.xml";
                //string filename = EntryPath + "McsfFilmingbak.xml";
                //if (System.IO.File.Exists(filename))
                //{
                //    return filename;
                //}
                //else
                //{
                //    return null;
                //}
            }
        }
        #endregion [--ReadOrWriteMcsfFilming--]
        //private static bool GetLastRepackSettingNodeFromConfig(out XmlDocument doc, out XmlNode root, out XmlNode lastRepackNode)
        //{
        //    root = null;
        //    lastRepackNode = null;
        //    doc = null;
        //    CheckFilmingConfig();
        //    if (McsfFilmingPath != null)
        //    {
        //        doc = new XmlDocument();
        //        doc.Load(McsfFilmingPath);
        //        root = doc.DocumentElement;
        //        if (root != null)
        //        {
        //            lastRepackNode = root.SelectSingleNode("LastRepackSetting");
        //            if (lastRepackNode != null)
        //                return true;
        //        }
        //    }
        //    return false;
        //}
        //public static bool GetRepackStatusFromConfigFile()
        //{
        //    bool result = true;

        //    XmlNode root = null;
        //    XmlNode lastRepackNode = null;
        //    XmlDocument doc = null;

        //    if (GetLastRepackSettingNodeFromConfig(out doc, out root, out lastRepackNode))
        //    {
        //        bool.TryParse(lastRepackNode.InnerText, out result);
        //    }
        //    return result;
        //}
        //public static bool WriteRepackStatusToConfigFile(bool value)
        //{
        //    XmlNode root = null;
        //    XmlNode lastRepackNode = null;
        //    XmlDocument doc = null;
        //    CheckFilmingConfig();
        //    if (GetLastRepackSettingNodeFromConfig(out doc, out root, out lastRepackNode))
        //    {
        //        lastRepackNode.InnerText = value.ToString();
        //        doc.Save(McsfFilmingPath);
        //    }
        //    else
        //    {
        //        if (root != null)
        //        {
        //            XmlElement elem = doc.CreateElement("LastRepackSetting");
        //            elem.InnerText = value.ToString();
        //            root.AppendChild(elem);
        //            doc.Save(McsfFilmingPath);
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public static bool FilmingRepackStatus { get; set; }

        //public static int GetDisplayModeFromConfigFile()
        //{
        //    int result = 1;

        #region [--ReadOrWriteMcsfFilming--]
        // //         //    XmlNode root = null;
        // //         //    XmlNode lastDisplayModeNode = null;
        // //         //    XmlDocument doc = null;
        // //         //    CheckFilmingConfig();
        // //         //    if (GetDisplayModeNodeFromConfig(out doc, out root, out lastDisplayModeNode))
        // //         //    {
        // //         //        int.TryParse(lastDisplayModeNode.InnerText, out result);
        // //         //    }
        // //         //    return result;
        // //         //}
        // //         //private static bool GetDisplayModeNodeFromConfig(out XmlDocument doc, out XmlNode root, out XmlNode lastDisplayModeNode)
        // //         //{
        // //         //    root = null;
        // //         //    lastDisplayModeNode = null;
        // //         //    doc = null;
        // //         //    CheckFilmingConfig();
        // //         //    if (McsfFilmingPath != null)
        // //         //    {
        // //         //        doc = new XmlDocument();
        // //         //        doc.Load(McsfFilmingPath);
        // //         //        root = doc.DocumentElement;
        // //         //        if (root != null)
        // //         //        {
        // //         //            lastDisplayModeNode = root.SelectSingleNode("ViewMode");
        // //         //            if (lastDisplayModeNode != null)
        // //         //                return true;
        // //         //        }
        // //         //    }
        // //         //    return false;
        // //         //}
        // // 
        // //         public static bool WriteMultiSeriesCompareConfigByNode(string nodeName, int value)
        // //         {
        // //             XmlNode root = null;
        // //             XmlNode lastRepackNode = null;
        // //             XmlDocument doc = null;
        // //             CheckFilmingConfig();
        // //             if (GetMultiSeriesCompareNode(nodeName, out doc, out root, out lastRepackNode))
        // //             {
        // //                 lastRepackNode.InnerText = value.ToString();
        // //                 doc.Save(McsfFilmingPath);
        // //                 return true;
        // //             }
        // //             return false;
        // //         }
        // // 
        // //         public static int GetMultiSeriesCompareConfigByNode(string nodeName)
        // //         {
        // //             int result = 1;
        // // 
        // //             XmlNode root = null;
        // //             XmlNode lastDisplayModeNode = null;
        // //             XmlDocument doc = null;
        // //             CheckFilmingConfig();
        // //             if (GetMultiSeriesCompareNode(nodeName, out doc, out root, out lastDisplayModeNode))
        // //             {
        // //                 int.TryParse(lastDisplayModeNode.InnerText, out result);
        // //             }
        // //             return result;
        // //         }
        // //         private static bool GetMultiSeriesCompareNode(string nodeName, out XmlDocument doc, out XmlNode root, out XmlNode lastDisplayModeNode)
        // //         {
        // //             root = null;
        // //             lastDisplayModeNode = null;
        // //             doc = null;
        // //             CheckFilmingConfig();
        // //             if (McsfFilmingPath != null)
        // //             {
        // //                 doc = new XmlDocument();
        // //                 doc.Load(McsfFilmingPath);
        // //                 root = doc.DocumentElement;
        // //                 if (root != null)
        // //                 {
        // //                     lastDisplayModeNode = root.SelectSingleNode(nodeName);
        // //                     if (lastDisplayModeNode != null)
        // //                         return true;
        // //                 }
        // //             }
        // //             return false;
        // //         }
        // // 
        //public static void SetPresetCellLayoutByIndex(int index, int row, int col)
        //{
        //    var cellLayout = Printers.Instance.PresetCellLayouts[index];
        //    cellLayout.SetLayout(row, col);
        //             CheckFilmingConfig();
        //             if (row <= 0 || col <= 0 || row > FilmLayout.MaxRowCount || col > FilmLayout.MaxColCount) return false;
        //             if (McsfFilmingPath == null) return false;
        //             try
        //             {
        //                 XDocument doc = XDocument.Load(McsfFilmingPath);
        //                 var root = doc.Root;
        //                 var preSetCellLayoutNode = root.Element("PresetCellLayout");
        //                 foreach (var element in preSetCellLayoutNode.Elements("CellLayout"))
        //                 {
        //                     var idx = NumeralConversion.AsInt(element.Attribute("Index").Value);
        //                     if (idx == index)
        //                     {
        //                         element.Attribute("Rows").SetValue(row.ToString());
        //                         element.Attribute("Columns").SetValue(col.ToString());
        //                         doc.Save(McsfFilmingPath);
        //                         return true;
        //                     }
        //                 }
        //                 return false;
        //             }
        //             catch (Exception ex)
        //             {
        //                 Logger.LogFuncException(ex.Message+ex.StackTrace);
        //                 return false;
        //             }
        //}
        //public static void GetPresetCellLayoutByIndex(int index, out int row, out int col)
        //{
        //    var cellLayout = Printers.Instance.PresetCellLayouts[index];
        //    row = cellLayout.Row;
        //    col = cellLayout.Col;
        //CheckFilmingConfig();
        //if (McsfFilmingPath == null) { row = 0; col = 0; return false; }
        //try
        //{
        //    var root = XDocument.Load(McsfFilmingPath).Root;
        //    var preSetCellLayoutNode = root.Element("PresetCellLayout");
        //    foreach (var element in preSetCellLayoutNode.Elements("CellLayout"))
        //    {
        //        var idx = NumeralConversion.AsInt(element.Attribute("Index").Value);
        //        if (idx == index)
        //        {
        //            row = NumeralConversion.AsInt(element.Attribute("Rows").Value);
        //            col = NumeralConversion.AsInt(element.Attribute("Columns").Value);
        //            return true;
        //        }
        //    }
        //    row = 0; col = 0;
        //    return false;
        //}
        //catch (Exception ex)
        //{
        //    Logger.LogFuncException(ex.Message + ex.StackTrace);
        //    row = 0; col = 0;
        //    return false;
        //}
        //}
        #endregion [--ReadOrWriteMcsfFilming--]
        public static string SystemModality
        {
            get
            {
                string modalityName = "";
                mcsf_clr_systemenvironment_config.GetModalityName(out modalityName);
                return modalityName.ToUpper();
            }
        }

        public static bool IsEnablePresetLayoutBar
        {
            get { return (SystemModality == "PT" || SystemModality == "CT" || SystemModality == "MR" || SystemModality == "PETMR"); }
        }

        public static Modality CellModality(MedViewerControlCell cell)
        {
            if (cell != null && !cell.IsEmpty && cell.Image != null && cell.Image.CurrentPage != null)
            {
                return cell.Image.CurrentPage.Modality;
            }
            return Modality.Unknown;
        }

        private static List<string> GetDistinctCustomLayoutNameList()
        {
            List<string> lstLayoutNames = new List<string>();
            string layoutRootPath = EntryPath + @"\McsfMedViewerConfig\MedViewerLayouts\CustomerLayout\";

            if (!Directory.Exists(layoutRootPath))
            {
                Directory.CreateDirectory(layoutRootPath);
            }

            string[] customViewportLayoutFilesArray = Directory.GetFiles(layoutRootPath);

            foreach (var customViewportLayoutFile in customViewportLayoutFilesArray)
            {
                if (customViewportLayoutFile.Contains(".png"))
                {
                    string layoutName = customViewportLayoutFile.Split('\\').Last();
                    layoutName = layoutName.Remove(layoutName.Length - ".png".Length);

                    if (layoutName.Contains(FilmLayout.RegularLayoutString))
                    {
                        layoutName = layoutName.Remove(layoutName.Length - FilmLayout.RegularLayoutString.Length);
                    }

                    lstLayoutNames.Add(layoutName);
                }
            }
            return lstLayoutNames;
        }
        public static string GetDistinctCustomLayoutName(string tempName)
        {
            List<string> lstLayoutNames = GetDistinctCustomLayoutNameList();
            for (int i = 1; i < 999; i++)
            {
                string name = tempName + i.ToString();
                if (!lstLayoutNames.Contains(name))
                    return name;
            }
            return "";
        }

        public static string TryTranslateFilmingResource(string uid)
        {
            if (string.IsNullOrEmpty(uid) || FilmingViewerContainee.FilmingResourceDict == null)
            {
                return uid;
            }

            // if did not have this key , result is null.
            string result = FilmingViewerContainee.FilmingResourceDict[uid] as string;
            if (result == null)
            {
                return uid;
            }

            return result;
        }

        public static bool CheckMemoryForLoadingSeries(List<Tuple<string, int>> seriesUidListAndCount, ICommunicationProxy proxy)
        {
            var filmingDb = FilmingDbOperation.Instance.FilmingDbWrapper;
            double estimatedMBytes = 0D;
            foreach (var series in seriesUidListAndCount)
            {
                var imageList = filmingDb.GetImageListBySeriesInstanceUID(series.Item1);
                estimatedMBytes += (double)imageList[0].Rows * imageList[0].Columns * imageList[0].SamplesPerPixel / 8.0 * imageList[0].BitsAllocated / 1024.0 / 1024.0 * FilmingUtility.MultipleOfCellPerImage * series.Item2;
                Logger.LogInfo("Cal:" + imageList[0].Rows + "*" + imageList[0].Columns + "*" + imageList[0].SamplesPerPixel+" / 8.0 *" + imageList[0].BitsAllocated +"/ 1024.0 / 1024.0 *" + FilmingUtility.MultipleOfCellPerImage + "*" + series.Item2);
            }
            MemoryManagement memoryManagement = new MemoryManagement();
            MemoryManagement._pCommProxy = proxy;
            //return memoryManagement.IsMemoryEnough((int)estimatedMBytes + 1);
            var canLoaded = memoryManagement.ReserveMemoryForCurrentApp((uint) estimatedMBytes);
            if (!canLoaded)
            {                
                Logger.LogInfo("ReserveMemoryForCurrentApp:" + estimatedMBytes + "MBytes");
            }
            return canLoaded;
        }
      
        public static void DoEvents()
        {
            var nestedFrame = new DispatcherFrame();
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), nestedFrame);
            Dispatcher.PushFrame(nestedFrame);
            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        private static Object ExitFrame(Object state)
        {
            var frame = state as DispatcherFrame;
            frame.Continue = false;
            return null;
        }
    }
}
