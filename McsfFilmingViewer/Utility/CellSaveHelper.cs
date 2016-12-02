using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;
using UIH.Mcsf.Core;
using UIH.Mcsf.Database;
using UIH.Mcsf.Pipeline.Data;
using UIH.Mcsf.Pipeline.Dictionary;
using UIH.Mcsf.Viewer;
using UIH.Mcsf.Filming.Wrappers;

namespace UIH.Mcsf.Filming.Utility
{
    public class CellSaveHelper
    {

        static CellSaveHelper()
        {
            GetTagsFromConfigFile();
        }


        public static string SeriesInstanceUid = string.Empty;

        private static IList<uint> _tags;

        private static int _seriesNumber = 8000;

        public static List<string> SavedImageUids = new List<string>();

        /// <summary>
        /// Assume that cell is not empty
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static DicomAttributeCollection SaveCellAsDataHeader(MedViewerControlCell cell, int index)
        {
            try
            {
                Logger.LogFuncUp();

                Debug.Assert(cell != null && cell.Image != null && cell.Image.CurrentPage != null);

                var dataHeader = new DicomAttributeCollection();

                var displayData = cell.Image.CurrentPage;
                var dicomHeader = displayData.ImageHeader.DicomHeader;

                var uidManager = McsfDatabaseDicomUIDManagerFactory.Instance().CreateUIDManager();
                if (index==0)
                {
                    SeriesInstanceUid = uidManager.CreateSeriesUID("1", "2", ""); //seriesUID; //
                    ClearSeries(dicomHeader);
                }

                //if (displayData.PixelData == null) return null;

                var pixelData = displayData.PixelData;
                if (pixelData == null)
                {
                    pixelData = new byte[displayData.ImageDataLength];
                    Marshal.Copy(displayData.ImageDataPtr, pixelData, 0, displayData.ImageDataLength);
                }

                var element = DicomAttribute.CreateAttribute(Tag.PixelData, VR.OB);
                if (!element.SetBytes(0, pixelData))
                {
                    Logger.LogWarning("Failed to Insert NULL  image Data to Data header");
                }
                dataHeader.AddDicomAttribute(element);

                FillTags(dicomHeader, dataHeader);

                //1. need to fill seriesDescription, from configuration, Default value is 3D-1
                FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SeriesDescription, Printers.Instance.NewSeriesDescription);
                
                //2. remove wrong tag
                dataHeader.RemoveDicomAttribute(Tag.SmallestImagePixelValue);
                dataHeader.RemoveDicomAttribute(Tag.LargestImagePixelValue);
                
                //3. update some tag
                dataHeader.RemoveDicomAttribute(Tag.InstanceNumber);
                FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstanceNumber, ""+(index+1));
                dataHeader.RemoveDicomAttribute(Tag.SeriesNumber);
                FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SeriesNumber, ""+_seriesNumber);
                dataHeader.RemoveDicomAttribute(Tag.SeriesInstanceUid);
                FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SeriesInstanceUid, SeriesInstanceUid); 
                dataHeader.RemoveDicomAttribute(Tag.SOPInstanceUID);
                
                FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SOPInstanceUID, uidManager.CreateImageUID(""));

                var ww = FilmingHelper.GetTagValueFrom(dataHeader, ServiceTagName.WindowWidth);
                dataHeader.RemoveDicomAttribute(Tag.WindowWidth);
                if (dicomHeader.ContainsKey(ServiceTagName.WindowWidth))
                {                    
                    if (!string.IsNullOrWhiteSpace(ww))
                    {
                        FilmingHelper.InsertStringArrayDicomElement(dataHeader, Tag.WindowWidth, dicomHeader[ServiceTagName.WindowWidth]);
                    }
                        
                }

                var wc = FilmingHelper.GetTagValueFrom(dataHeader, ServiceTagName.WindowCenter);
                dataHeader.RemoveDicomAttribute(Tag.WindowCenter);
                if (dicomHeader.ContainsKey(ServiceTagName.WindowCenter))
                {                    
                    if (!string.IsNullOrWhiteSpace(wc))
                    {
                        FilmingHelper.InsertStringArrayDicomElement(dataHeader, Tag.WindowCenter, dicomHeader[ServiceTagName.WindowCenter]);
                    }
                    
                }
                
                if (dicomHeader.ContainsKey(ServiceTagName.ImageType))
                {
                    dataHeader.RemoveDicomAttribute(Tag.ImageType);
                    FilmingHelper.InsertStringArrayDicomElement(dataHeader, Tag.ImageType, dicomHeader[ServiceTagName.ImageType]);
                }

                ////DICOM File Meta Information
                ////FillDicomFileMetaInformationTags(dataHeader);

                ////Patient Module
                //FillPatientModuleTags(dataHeader, dicomHeader);

                ////General Study Module
                //FillGeneralStudyModuleTags(dataHeader, dicomHeader);

                ////Patient Study Module
                //FillPatientStudyModuleTags(dataHeader, dicomHeader);

                ////General Series Module
                //FillGeneralSeriesModuleTags(dataHeader, dicomHeader);

                ////General Equipment Module

                //////SC Equipment Module
                ////FillSCEquipmentModuleTags(dataHeader);

                ////General Image Module
                //FillGeneralImageModuleTags(dataHeader, index, dicomHeader);

                //Image Pixel Module
                //FilmingHelper.AddConstDICOMAttributes(dataHeader);
                //FilmingHelper.InsertUInt16DicomElement(dataHeader, Tag.Columns, (ushort)imageWidth);
                //FilmingHelper.InsertUInt16DicomElement(dataHeader, Tag.Rows, (ushort)imageHeight);


                ////SC Image Module
                ////FillSCImageModuleTags(dataHeader);

                ////VOI LUT Module
                //FillVoilutMacroTags(dataHeader, dicomHeader);

                ////SOP Common Module
                //FillSopCommonModuleTags(dataHeader, dicomHeader);

                Logger.LogFuncDown();
                return dataHeader;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                return null;
            }
        }

        private static void FillTags(IDictionary<uint, string> dicomHeader, DicomAttributeCollection dataHeader)
        {
            //0. 测试代码写在什么地方?

            //1. 读取tags配置文件,获取所有的tag
                //放置在静态构造函数之中//var tags = GetTagsFromConfigFile();

            //2.查询dicomHeader填入所有的值
            foreach (var tag in _tags)
            {
                string tagValue = dicomHeader.ContainsKey(tag) ? dicomHeader[tag] : string.Empty;

                //Tag tagEnum;
                //Enum.Parse(typeof (Tag), tag, true);
                //if(Enum.TryParse(tag, true, out tagEnum))
                FilmingHelper.InsertStringDicomElement(dataHeader, tag, tagValue);
            }
        }

        private static void GetTagsFromConfigFile()
        {
            var sEntryPath = mcsf_clr_systemenvironment_config.GetApplicationPath("FilmingConfigPath");

            var tagConfigFilePath = sEntryPath + "McsfMedViewerConfig\\mcsf_med_viewer_tags.xml";

            var doc = new XmlDocument();
            doc.Load(tagConfigFilePath);
            var root = doc.DocumentElement;
            if(root == null) return;
            
            _tags = new List<uint>();
            foreach (XmlNode node in root.ChildNodes)
            {
                uint tag = 0;
                if (uint.TryParse(node.Value, out tag)) _tags.Add(tag);
            }

        }


        public static void ClearSeries(IDictionary<uint, string> dicomHeader)
        {

            var studyInstanceUid = dicomHeader.ContainsKey(ServiceTagName.StudyInstanceUID) ? dicomHeader[ServiceTagName.StudyInstanceUID] : "";
            var modality = dicomHeader.ContainsKey(ServiceTagName.Modality) ? dicomHeader[ServiceTagName.Modality] : "";
            ClearSeries(studyInstanceUid);
        }

        public static void RefreshSeries(string studyInstanceUid, string modality)
        {
            if (!ClearSeries(studyInstanceUid))
            {
                CreateSeries(studyInstanceUid, modality);
                SavedImageUids.Clear();
            }
        }

        private static void CreateSeries(string studyInstanceUid, string modality)
        {
            var db = FilmingDbOperation.Instance.FilmingDbWrapper;
            Series series = db.CreateSeries();
            SeriesInstanceUid = series.SeriesInstanceUID;
            series.StudyInstanceUIDFk = studyInstanceUid;
            // new seriesNumber equals the max seriesNumber of exist series add one
            _seriesNumber = Convert.ToInt32(FilmingHelper.GetSerieNumber(studyInstanceUid)) + 1;
            series.SeriesNumber = _seriesNumber;
            series.SeriesDescription = Printers.Instance.NewSeriesDescription;
            series.Modality = modality;

            if (studyInstanceUid == FilmingHelper.StarsString) return;


            //check whether disk space is enough
            ICommunicationProxy pCommProxy = FilmingViewerContainee.Main.GetCommunicationProxy();
            var target = new SystemResManagerProxy(pCommProxy);
            if (!target.HaveEnoughSpace())
            {
                Logger.LogWarning("No enough disk space, so Electronic Image Series will not be created");
                //FilmingViewerContainee.ShowStatusWarning("UID_Filming_No_Enough_Disk_Space_To_Create_Electronic_Image_Series");
                FilmingViewerContainee.Main.ShowStatusWarning("UID_Filming_No_Enough_Disk_Space_To_Create_Electronic_Image_Series");
                return;
            }

            series.Save();
        }

        private static bool ClearSeries(string studyInstanceUid)
        {
            var seriesDescription = Printers.Instance.NewSeriesDescription; //read or get from configuration file


            //find if there are archived series
            var db = FilmingDbOperation.Instance.FilmingDbWrapper;

            var serieses = db.GetSeriesListByConditionWithOrder(
                "StudyInstanceUIDFk=\"" + studyInstanceUid + "\" AND SeriesDescription=\"" + seriesDescription + "\""
                , "SeriesNumber");
            //if (serieses != null)
            //{
            //    foreach (var seriesBase in serieses)
            //    {
            //        if (seriesBase != null && seriesBase.SeriesNumber >= 8000)
            //            seriesBase.Erase();
            //    }
            //}
            if (serieses == null || serieses.Count <= 0) return false;
            var series = serieses[0] as SeriesBase;
            SavedImageUids.Clear();
            series.GetImageBaseList().ForEach(image => SavedImageUids.Add(image.SOPInstanceUID));
            SeriesInstanceUid = series.SeriesInstanceUID;
            return true;
        }


        //private static void FillDicomFileMetaInformationTags(DicomAttributeCollection dataHeader)
        //{
        //    //string strLen = GetPatientInfo("PatientsBirthTime", string.Empty, string.Empty);
        //    //UInt32 nLen = Convert.ToUInt32(strLen);
        //    //FilmingHelper.InsertUInt32DicomElement(dataHeader, Tag.FileMetaInformationGroupLength, nLen);
        //    //byte[] btVersion = new byte[1];
        //    //btVersion[0] = 55;
        //    //FilmingHelper.InsertBytesDicomElement(dataHeader, Tag.FileMetaInformationVersion, btVersion);
        //    //FilmingHelper.InsertUInt16DicomElement(dataHeader, Tag.MediaStorageSOPClassUID, 0);
        //    //FilmingHelper.InsertUInt16DicomElement(dataHeader, Tag.MediaStorageSOPInstanceUID, 0);
        //    //FilmingHelper.InsertUInt16DicomElement(dataHeader, Tag.TransferSyntaxUID, 0);//"1.2.840.10008.1.2.1"
        //    //FilmingHelper.InsertUInt16DicomElement(dataHeader, Tag.ImplementationClassUID, 0);
        //    //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.ImplementationVersionName, string.Empty);
        //    //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SpecificCharacterSet, "ISO.IR 192");
        //}

        //private static void FillPatientModuleTags(DicomAttributeCollection dataHeader, IDictionary<string, string> dicomHeader )
        //{
        //    //var temp = dicomHeader.ContainsKey("PatientsName") ? dicomHeader["PatientsName"] : string.Empty;
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsName, dicomHeader["PatientsName"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientID, dicomHeader["PatientID"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsBirthDate, dicomHeader["PatientsBirthDate"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsSex, dicomHeader["PatientsSex"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsBirthTime, dicomHeader["PatientsBirthDate"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientComments, string.Empty);

        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsAge, dicomHeader["PatientsAge"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstitutionName, dicomHeader["InstitutionName"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.Manufacturer, dicomHeader["Manufacturer"]);


        //}

        //private static void FillGeneralStudyModuleTags(DicomAttributeCollection dataHeader, IDictionary<string, string> dicomHeader)
        //{
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.StudyInstanceUid, dicomHeader["StudyInstanceUid"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.StudyDate, dicomHeader["StudyDate"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.StudyTime, dicomHeader["StudyTime"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.ReferringPhysiciansName, dicomHeader["ReferringPhysiciansName"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.StudyId, dicomHeader["StudyID"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.AccessionNumber, dicomHeader["AccessionNumber"]);
        //    //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.StudyDescription, GetPatientInfo("StudyDescription", string.Empty, string.Empty));

        //    //var now = DateTime.Now;
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.AcquisitionDate, dicomHeader["AcquisitionDate"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.AcquisitionTime, dicomHeader["AcquisitionTime"]);
        //}

        //private static void FillPatientStudyModuleTags(DicomAttributeCollection dataHeader, IDictionary<string, string> dicomHeader)
        //{
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsAge, dicomHeader["PatientsAge"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsSize, string.Empty);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsWeight, string.Empty);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.Occupation, string.Empty);
        //}

        //private static void FillGeneralSeriesModuleTags(DicomAttributeCollection dataHeader, IDictionary<string, string> dicomHeader)
        //{
        //    //string str = GetPatientInfo("Modality", string.Empty, string.Empty);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.Modality, dicomHeader["Modality"]);
            
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SeriesNumber, dicomHeader["StudyInstanceUid"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.Laterality, string.Empty);
        //    //odo: FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SeriesDate, GetPatientInfo("SeriesDate", string.Empty, string.Empty));
        //    //odo: FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SeriesTime, GetPatientInfo("SeriesTime", string.Empty, string.Empty));
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.ProtocolName, string.Empty);

        //    //string descreption = "Electronic film_" + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
        //    //odo: read from configure file FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SeriesDescription, descreption);

        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.BodyPartExamined, dicomHeader["BodyPartExamined"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientPosition, dicomHeader["PatientPosition"]);
        //}

        //private static void FillScEquipmentModuleTags(DicomAttributeCollection dataHeader)
        //{
        //    //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.ConversionType, "WSD");
        //    ////FilmingHelper.InsertStringDicomElement(dataHeader, Tag.Modality, "SC");
        //    //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SecondaryCaptureDeviceID, GetPatientInfo("SecondaryCaptureDeviceID", string.Empty, string.Empty));
        //    //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SecondaryCaptureDeviceManufacturer, FilmingUtility.SecondaryCaptureDeviceManufacturer);
        //    //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SecondaryCaptureDeviceManufacturersModelName,
        //    //                                                   GetPatientInfo("SecondaryCaptureDeviceManufacturersModelName", string.Empty, string.Empty));
        //    //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SecondaryCaptureDeviceSoftwareVersions,
        //    //                                                   GetPatientInfo("SecondaryCaptureDeviceSoftwareVersions", string.Empty, string.Empty));
        //}

        //private static void FillGeneralImageModuleTags(DicomAttributeCollection dataHeader, int index, IDictionary<string, string> dicomHeader)
        //{
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstanceNumber, Convert.ToString(index + 1));
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientOrientation, dicomHeader["PatientOrientation"]);

        //    const string imageType = FilmingUtility.EFilmImageType;
        //    var imageTypes = imageType.Split('\\');
        //    FilmingHelper.InsertStringArrayDicomElement(dataHeader, Tag.ImageType, imageTypes);

        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.AcquisitionNumber, string.Empty);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.ImagesInAcquisition,
        //                                           dicomHeader["ImagesInAcquisition"]); 
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.ImageComments, string.Empty);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.BurnedInAnnotation, string.Empty);
        //    //if Modality is not MR, Set RescaleIntercept to 0 and set RescaleSlope to 1
        //    //if ("MR" != FilmingPageModality)
        //    {
        //        FilmingHelper.InsertStringDicomElement(dataHeader, Tag.RescaleIntercept, "0");
        //        FilmingHelper.InsertStringDicomElement(dataHeader, Tag.RescaleSlope, "1");
        //    }

        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.Rows, dicomHeader["Rows"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.Columns, dicomHeader["Columns"]);
        //}

        //private static void FillVoilutMacroTags(DicomAttributeCollection dataHeader, IDictionary<string, string> dicomHeader)
        //{
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.WindowCenter, dicomHeader["WindowCenter"]);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.WindowWidth, dicomHeader["WindowWidth"]);
        //}

        //private static void FillSCImageModuleTags(DicomAttributeCollection dataHeader)
        //{
        //    var time = DateTime.Now;
        //    var dateOfSecondaryCapture = string.Format("{0:D4}{1:D2}{2:D2}", time.Year, time.Month, time.Day);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.DateOfSecondaryCapture, dateOfSecondaryCapture);

        //    var timeOfSecondaryCapture = string.Format("{0:D2}{1:D2}{2:D2}", time.Hour, time.Minute, time.Second);
        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.TimeOfSecondaryCapture, timeOfSecondaryCapture);
        //}

        //private static void FillSopCommonModuleTags(DicomAttributeCollection dataHeader, IDictionary<string, string> dicomHeader)
        //{
        //    var uidManager = McsfDatabaseDicomUIDManagerFactory.Instance().CreateUIDManager();

        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SOPClassUID, dicomHeader["SOPClassUID"]);
        //    string str = uidManager.CreateImageUID("");

        //    FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SOPInstanceUID, str);

        //    //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SpecificCharacterSet, "127");
        //    ////FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstanceCreationDate, "256");
        //    ////FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstanceCreationTime, "256");
        //    //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstanceCreatorUID, "256");
        //    //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstanceNumber, "256");
        //}



//         /// <summary>
//         /// Save cells to be a series
//         /// </summary>
//         /// <param name="originalCells"> </param>
//         public static void SaveAsSeries(IEnumerable<MedViewerControlCell> originalCells)
//         {
//             var cells =
//                 originalCells.Where(cell => (cell != null && cell.Image != null && cell.Image.CurrentPage != null));
// 
//             if (cells == null || !cells.Any()) return;
// 
//             var sampleCell = cells.FirstOrDefault();
// 
//             //ImageType
//             string imageType = GetImageType(sampleCell);
//             //SeriesNumber
//             int seriesNumber = GetSeriesNumber(sampleCell);
// 
//             var tagsProviderList = new List<ImageTagsProvider>();
//             int imgNumber = 1;
//             foreach (var cell in cells)
//             {
//                 var tagsProvider = new ImageTagsProvider
//                                        {
//                                            ImageNumber = imgNumber,
//                                            ActivePS = cell.Image.CurrentPage.SerializePSInfo(),
//                                            ImageType = imageType,
//                                            SOPInstanceUID = cell.Image.CurrentPage.SOPInstanceUID,
//                                            SeriesNumber = seriesNumber,
//                                            CellIndex = cell.CellIndex
//                                        };
//                 //ExtraFilling
//                 string photometricInterpretationValue = GetPhotometricInterpretation(cell);
//                 string[] windowLevel = GetWindowLevel(cell);
//                 tagsProvider.ExtraFilling = m => ExtraFilling(m, string.Empty, string.Empty, photometricInterpretationValue, windowLevel);
// 
//                 tagsProviderList.Add(tagsProvider);
//                 imgNumber++;
//             }
// 
//             var multiCellInfo = new MultiImagesSaveFilmingInfo(sampleCell, 11062, //(int)Review2dCommonSaveCmdId.NotifyBEToSaveAsCommonSave,
//                 SavingType.Original) { ImageTagList = tagsProviderList };
//             multiCellInfo.Send();
//         }
// 
//         /// <summary>
//         /// if the PhotometricInterpretation is RGB, return
//         /// if a PET image(judged by SOPClassUID not Modality, PET图像默认反色显示) in the cell, 
//         ///     if saving type is original, the return value should be MONOCHROME2, whether the user do reversed or not
//         ///     if saving type is secondary capture, the user do reversed, the return value should be MONOCHROME2, 
//         ///                                                                   the user not do reversed, the return value should be MONOCHROME1.
//         /// and if not a PET image in the cell, the user do reversed, the return value should be MONOCHROME1
//         ///                                                           the user not do reversed, the return value should be MONOCHROME2
//         /// </summary>
//         /// <param name="cell">which need to save as</param>
//         /// <param name="savingType">saving type</param>
//         /// <returns>PhotometricInterpretation value</returns>
//         private static string GetPhotometricInterpretation(MedViewerControlCell cell, SavingType savingType = SavingType.Original)
//         {
//             try
//             {
//                 string photometricInterpretationValue;
//                 cell.Image.CurrentPage.ImageHeader.DicomHeader.TryGetValue("PhotometricInterpretation", out photometricInterpretationValue);
//                 if (photometricInterpretationValue == "RGB")
//                     return photometricInterpretationValue;
//                 string sopClassUid;
//                 cell.Image.CurrentPage.ImageHeader.DicomHeader.TryGetValue("SOPClassUID", out sopClassUid);
//                 bool bReversed = cell.Image.CurrentPage.PState.IsPaletteReversed;
//                 // if image is PET(judged by SOPClassUID not Modality), SavingType is Original, 
//                 // whether reversed or not reversed, the photometricInterpretation value is MONOCHROME2
//                 if (SavingType.Original == savingType)
//                 {
//                     if (sopClassUid == "1.2.840.10008.5.1.4.1.1.128" || !bReversed)
//                         photometricInterpretationValue = "MONOCHROME2";
//                     else
//                         photometricInterpretationValue = "MONOCHROME1";
//                 }
//                 else
//                 {
//                     if (sopClassUid == "1.2.840.10008.5.1.4.1.1.128")
//                     {
//                         photometricInterpretationValue = bReversed ? "MONOCHROME2" : "MONOCHROME1";
//                     }
//                     else
//                     {
//                         photometricInterpretationValue = bReversed ? "MONOCHROME1" : "MONOCHROME2";
//                     }
//                 }
// 
//                 return photometricInterpretationValue;
//             }
//             catch (Exception exp)
//             {
//                 Logger.LogWarning("CommonSaveAs GetPhotometricInterpretation Exception: " + exp);
//                 return string.Empty;
//             }
//         }
// 
//         private static string[] GetWindowLevel(MedViewerControlCell cell)
//         {
//             try
//             {
//                 var dicomHeader = cell.Image.CurrentPage.ImageHeader.DicomHeader;
//                 string photometricInterpretationValue;
//                 dicomHeader.TryGetValue("PhotometricInterpretation", out photometricInterpretationValue);
//                 if (photometricInterpretationValue == "RGB")
//                 {
//                     string[] emptyWl = { string.Empty, string.Empty };
//                     return emptyWl;
//                 }
//                 double wcInPstate = cell.Image.CurrentPage.PState.WindowLevel.WindowCenter;
//                 double wwInPstate = cell.Image.CurrentPage.PState.WindowLevel.WindowWidth;
//                 string[] newWl = { wcInPstate.ToString(CultureInfo.InvariantCulture), wwInPstate.ToString(CultureInfo.InvariantCulture) };
//                 if (dicomHeader.ContainsKey("WindowCenter")
//                     && dicomHeader.ContainsKey("WindowWidth"))
//                 {
//                     string[] centers = dicomHeader["WindowCenter"].Split('\\');
//                     string[] widths = dicomHeader["WindowWidth"].Split('\\');
// 
//                     double wcInDicom = double.Parse(centers[0]);
//                     double wwInDicom = double.Parse(widths[0]);
// 
//                     string newWc = dicomHeader["WindowCenter"];
//                     string newWw = dicomHeader["WindowWidth"];
//                     newWl[0] = newWc;
//                     newWl[1] = newWw;
//                     if (Math.Abs(wcInPstate - wcInDicom) > 1E-9 || Math.Abs(wwInPstate - wwInDicom) > 1E-9)
//                     {
//                         // here, we retained 7 bits after the decimal point. 
//                         //Because if the length of result is more than 16, the Pipeline will not allow us to insert
//                         newWc = wcInPstate.ToString("#.0000000") + "\\" + dicomHeader["WindowCenter"];
//                         newWw = wwInPstate.ToString("#.0000000") + "\\" + dicomHeader["WindowWidth"];
//                         newWl[0] = newWc;
//                         newWl[1] = newWw;
//                     }
//                 }
//                 return newWl;
//             }
//             catch (Exception exp)
//             {
//                 Logger.LogWarning("CommonSaveAs GetWindowCenter Exception: " + exp);
//                 string[] newWl = { string.Empty, string.Empty };
//                 return newWl;
//             }
//         }
// 
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <param name="dataHeader"></param>
//         /// <param name="patientOrientationValue"></param>
//         /// <param name="imageOrientationValue"> </param>
//         /// <param name="photometricInterpretationValue"></param>
//         /// <param name="windowLevel"></param>
//         private static void ExtraFilling(DicomAttributeCollection dataHeader, string patientOrientationValue, string imageOrientationValue, string photometricInterpretationValue,
//             string[] windowLevel)
//         {
//             try
//             {
//                 if (patientOrientationValue.Length != 0)
//                 {
//                     InsertStringDicomElement(dataHeader, Tag.PatientOrientation, patientOrientationValue, 0);
//                 }
//                 if (imageOrientationValue != string.Empty)
//                 {
//                     string[] imageOrientation = imageOrientationValue.Split('\\');
//                     for (int i = 0; i < imageOrientation.Length; i++)
//                     {
//                         InsertStringDicomElement(dataHeader, Tag.ImageOrientationPatient, imageOrientation[i], i);
//                     }
//                 }
//                 InsertStringDicomElement(dataHeader, Tag.PhotometricInterpretation, photometricInterpretationValue, 0);
//                 InsertStringDicomElement(dataHeader, Tag.PresentationLutShape,
//                                          photometricInterpretationValue == "MONOCHROME1" ? "INVERSE" : "IDENTITY", 0);
// 
//                 if (windowLevel[0] != string.Empty && windowLevel[1] != string.Empty)
//                 {
//                     string[] centers = windowLevel[0].Split('\\');
//                     string[] widths = windowLevel[1].Split('\\');
//                     for (int i = 0; i < centers.Length; i++)
//                     {
//                         InsertStringDicomElement(dataHeader, Tag.WindowCenter, centers[i], i);
//                         InsertStringDicomElement(dataHeader, Tag.WindowWidth, widths[i], i);
//                     }
//                 }
//             }
//             catch (Exception exp)
//             {
//                 Logger.LogWarning("CommonSaveAs extraFilling Exception: " + exp);
//             }
//         }
// 
//         private static void InsertStringDicomElement(DicomAttributeCollection dataHeader, Tag tagName, string tagValue, int index)
//         {
//             try
//             {
//                 if (!dataHeader.Contains(tagName))
//                 {
//                     var element = DicomAttribute.CreateAttribute(tagName);
//                     if (tagValue == null)
//                         return;
//                     if (!element.SetString(index, tagValue))
//                     {
//                         Logger.LogError("Failed to Insert " + tagName.ToString() + "to DataHeader");
//                         return;
//                     }
// 
//                     dataHeader.AddDicomAttribute(element);
//                 }
//                 else
//                 {
//                     if (!dataHeader[tagName].SetString(index, tagValue))
//                     {
//                         Logger.LogError("Failed to Insert " + tagName.ToString() + "to DataHeader");
//                     }
//                 }
//             }
//             catch (Exception exp)
//             {
//                 Logger.LogWarning("XRSave and SaveAs InsertStringDicomElement Exception: " + exp);
//             }
//         }
// 
//         private static string GetImageType(MedViewerControlCell cell)
//         {
//             try
//             {
//                 var imageType = cell.Image.CurrentPage.ImageType;
//                 for (int i = 0; i < imageType.Length; i++)
//                 {
//                     if (0 == i && imageType[i].Equals("ORIGINAL"))
//                         imageType[i] = "DERIVED";
//                     if (1 == i && imageType[i].Equals("PRIMARY"))
//                         imageType[i] = "SECONDARY";
//                 }
// 
//                 return string.Join("\\", imageType);
//             }
//             catch (Exception exp)
//             {
//                 Logger.LogFuncException("XRSave and SaveAs GetImageType Exception: " + exp);
//                 return string.Empty;
//             }
//         }
// 
//         private static int GetSeriesNumber(MedViewerControlCell cell)
//         {
//             try
//             {
//                 var imageHeader = cell.Image.CurrentPage.ImageHeader;
//                 string seriesUid;
//                 imageHeader.DicomHeader.TryGetValue("SeriesInstanceUID", out seriesUid);
//                 int seriesNumber = AdvAppSavingHelper.GetSeriesNumber(seriesUid);
// 
//                 return seriesNumber;
//             }
//             catch (Exception exp)
//             {
//                 Logger.LogFuncException("XRSave and SaveAs GetSeriesNumber Exception: " + exp);
//                 return 0;
//             }
//         }
    }
}
