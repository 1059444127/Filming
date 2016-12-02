using System;
using System.Linq;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Database;
using UIH.Mcsf.Filming.Models;
using UIH.Mcsf.Pipeline.Data;
using UIH.Mcsf.Pipeline.Dictionary;


namespace UIH.Mcsf.Filming.Widgets
{
    public class DicomElementWidget
    {

        #region 生成dicom文件

        //public DicomAttributeCollection AssembleSendData(byte[] imageData, double imageWidth, double imageHeight,
        //                                                 int filmPageIndex, bool ifSaveImageAsGreyScale = true)
        public DicomAttributeCollection AssembleSendData(byte[] imageData,EFilmModel dicomTags)
        {
            try
            {
                Logger.LogFuncUp();
                
                var dataHeader = new DicomAttributeCollection();

                #region DICOM File Meta Information  
                //DICOM File Meta Information
                //string strLen = GetPatientInfo("PatientsBirthTime", string.Empty, string.Empty);
                //UInt32 nLen = Convert.ToUInt32(strLen);
                //DicomElementWidget.InsertUInt32DicomElement(dataHeader, Tag.FileMetaInformationGroupLength, nLen);
                //byte[] btVersion = new byte[1];
                //btVersion[0] = 55;
                //InsertBytesDicomElement(dataHeader, Tag.FileMetaInformationVersion, btVersion);
                //InsertUInt16DicomElement(dataHeader, Tag.MediaStorageSOPClassUID, 0);
                //InsertUInt16DicomElement(dataHeader, Tag.MediaStorageSOPInstanceUID, 0);
                //InsertUInt16DicomElement(dataHeader, Tag.TransferSyntaxUID, 0);//"1.2.840.10008.1.2.1"
                //InsertUInt16DicomElement(dataHeader, Tag.ImplementationClassUID, 0);
                //InsertStringDicomElement(dataHeader, Tag.ImplementationVersionName, string.Empty);
                InsertStringDicomElement(dataHeader, Tag.SpecificCharacterSet, "ISO.IR 192");         //中文字符集
                #endregion



                //Patient Module
                #region Patient Module
                InsertStringDicomElement(dataHeader, Tag.PatientsName, dicomTags.PatientName);              //患者姓名
                InsertStringDicomElement(dataHeader, Tag.PatientID, dicomTags.PatientId);                   //患者ID
                InsertStringDicomElement(dataHeader, Tag.PatientsBirthDate, dicomTags.PatientBirthDate);    //患者出生日期
                InsertStringDicomElement(dataHeader, Tag.PatientsSex, dicomTags.PatientSex);                //患者性别
                InsertStringDicomElement(dataHeader, Tag.PatientsBirthTime, dicomTags.PatientsBirthTime);   //患者出生时间
                InsertStringDicomElement(dataHeader, Tag.PatientComments, dicomTags.PatientComments);       //患者注释
                InsertStringDicomElement(dataHeader, Tag.PatientsAge, dicomTags.PatientsAge);               //患者年龄
                InsertStringDicomElement(dataHeader, Tag.PatientsSize, dicomTags.PatientsSize);             //患者身高
                InsertStringDicomElement(dataHeader, Tag.PatientsWeight, dicomTags.PatientsWeight);         //患者体重
                InsertStringDicomElement(dataHeader, Tag.Occupation, dicomTags.Occupation);                 //患者职业

                #endregion


                //General Study Module

                #region General Study Module
                InsertStringDicomElement(dataHeader, Tag.StudyInstanceUid, dicomTags.StudyInstanceUid);     //检查UID
                InsertStringDicomElement(dataHeader, Tag.StudyDate,dicomTags.StudyDate);                    //检查日期
                InsertStringDicomElement(dataHeader, Tag.StudyTime, dicomTags.StudyTime);                   //检查时间
                InsertStringDicomElement(dataHeader, Tag.ReferringPhysiciansName,dicomTags.ReferringPhysiciansName);   //咨询医师姓名
                InsertStringDicomElement(dataHeader, Tag.AccessionNumber,dicomTags.AccessionNumber);        //检查编号
                //InsertStringDicomElement(dataHeader, Tag.StudyDescription, dicomTags.StudyDescription);
                var now = DateTime.Now;
                InsertStringDicomElement(dataHeader, Tag.AcquisitionDate, now.Date.ToShortDateString());    //获取日期
                InsertStringDicomElement(dataHeader, Tag.AcquisitionTime, now.TimeOfDay.ToString());        //获取时间
                #endregion

                //General Series Module
                #region General Series Module
                InsertStringDicomElement(dataHeader, Tag.SeriesInstanceUid, dicomTags.EFilmSeriesUid);      //系列UID
                InsertStringDicomElement(dataHeader, Tag.SeriesNumber, GetSerieNumber(dicomTags.StudyInstanceUid));  //系列号 ？？是否每张都去找数据库算一次？？
                InsertStringDicomElement(dataHeader, Tag.SeriesDate,now.Date.ToShortDateString());          //系列日期
                InsertStringDicomElement(dataHeader, Tag.SeriesTime, now.TimeOfDay.ToString());             //系列时间

                string descreption = "Electronic film_" + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);    //系列描述
                InsertStringDicomElement(dataHeader, Tag.SeriesDescription, descreption);
                #endregion

                #region 患者病理相关
                //InsertStringDicomElement(dataHeader, Tag.ProtocolName, dicomTags.ProtocolName);           //协议名称
                InsertStringDicomElement(dataHeader, Tag.BodyPartExamined, dicomTags.BodyPartExamined);     //检查部位
                InsertStringDicomElement(dataHeader, Tag.PatientPosition, dicomTags.PatientPosition);       //病人进入设备体位
                #endregion

                //General Equipment Module
                #region General Equipment Module
                InsertStringDicomElement(dataHeader, Tag.Modality, dicomTags.EFilmModality); //胶片设备类型
                InsertStringDicomElement(dataHeader, Tag.InstitutionName, dicomTags.InstitutionName); //医院信息
                InsertStringDicomElement(dataHeader, Tag.Manufacturer, dicomTags.Manufacturer); //原图厂商信息
                #endregion


                #region SC Equipment Module
                //SC Equipment Module
                InsertStringDicomElement(dataHeader, Tag.ConversionType, "WSD");            //转化类型，WSD：Workstation
                InsertStringDicomElement(dataHeader, Tag.SecondaryCaptureDeviceID,                          //二次获取设备ID
                                         dicomTags.SecondaryCaptureDeviceID);
                InsertStringDicomElement(dataHeader, Tag.SecondaryCaptureDeviceManufacturer,                //二次获取设备厂商
                                         FilmingUtility.SecondaryCaptureDeviceManufacturer);
                InsertStringDicomElement(dataHeader, Tag.SecondaryCaptureDeviceManufacturersModelName,      //二次获取设备型号
                                         dicomTags.SecondaryCaptureDeviceManufacturersModelName);
                InsertStringDicomElement(dataHeader, Tag.SecondaryCaptureDeviceSoftwareVersions,            //二次获取设备软件版本
                                         dicomTags.SecondaryCaptureDeviceSoftwareVersions);
                var dateOfSecondaryCapture = string.Format("{0:D4}{1:D2}{2:D2}", now.Year, now.Month, now.Day); //二次获取日期
                InsertStringDicomElement(dataHeader, Tag.DateOfSecondaryCapture, dateOfSecondaryCapture);

                var timeOfSecondaryCapture = string.Format("{0:D2}{1:D2}{2:D2}", now.Hour, now.Minute, now.Second); //二次获取时间
                InsertStringDicomElement(dataHeader, Tag.TimeOfSecondaryCapture, timeOfSecondaryCapture);
                #endregion

                //General Image Module
                #region 图片相关

                InsertStringDicomElement(dataHeader, Tag.InstanceNumber, dicomTags.FilmPageIndex); //图片序号
                const string imageType = FilmingUtility.EFilmImageType; //图片类型
                var imageTypes = imageType.Split('\\');
                InsertStringArrayDicomElement(dataHeader, Tag.ImageType, imageTypes);

                InsertStringDicomElement(dataHeader, Tag.AcquisitionNumber, dicomTags.AcquisitionNumber); //图片获取号

                InsertStringDicomElement(dataHeader, Tag.ImagesInAcquisition, dicomTags.ImagesInAcquisition); //图片获取总数
                InsertStringDicomElement(dataHeader, Tag.ImageComments, ""); //图片注释
                InsertStringDicomElement(dataHeader, Tag.BurnedInAnnotation, dicomTags.BurnedInAnnotation); //图片烧图注释？？

                #endregion


                //Image Pixel Module
                #region 图像像素相关
                if (dicomTags.IfSaveImageAsGrayScale)
                {
                    InsertUInt16DicomElement(dataHeader, Tag.SamplesPerPixel, 1);               //取样点数
                    InsertStringDicomElement(dataHeader, Tag.PhotometricInterpretation, "MONOCHROME2");  //灰度类型
                }
                else
                {
                    InsertUInt16DicomElement(dataHeader, Tag.SamplesPerPixel, 3);               //取样点数彩图
                    InsertStringDicomElement(dataHeader, Tag.PhotometricInterpretation, "RGB"); //彩图
                    InsertStringDicomElement(dataHeader, Tag.PlanarConfiguration, "0");         //彩图依赖
                }
                InsertUInt16DicomElement(dataHeader, Tag.BitsAllocated, dicomTags.BitsAllocated); //允许存储位数
                InsertUInt16DicomElement(dataHeader, Tag.BitsStored, dicomTags.BitsStored); //实际存储位数
                InsertUInt16DicomElement(dataHeader, Tag.HighBit, (ushort) (dicomTags.BitsAllocated - 1)); //高位
                InsertUInt16DicomElement(dataHeader, Tag.PixelRepresentation, 0); //是否有符号位
                InsertUInt16DicomElement(dataHeader, Tag.Columns, dicomTags.Columns); //图片宽
                InsertUInt16DicomElement(dataHeader, Tag.Rows, dicomTags.Rows); //图片高

                #endregion

                //SOP Common Module
                #region 图片标识
                var uidManager = McsfDatabaseDicomUIDManagerFactory.Instance().CreateUIDManager();
                var str = uidManager.CreateImageUID("");
                InsertStringDicomElement(dataHeader, Tag.SOPInstanceUID, str);
                InsertStringDicomElement(dataHeader, Tag.SOPClassUID, "1.2.840.10008.5.1.4.1.1.7");
                InsertStringDicomElement(dataHeader, Tag.SpecificCharacterSet, "127");
                InsertStringDicomElement(dataHeader, Tag.InstanceCreatorUID, "256");
                InsertStringDicomElement(dataHeader, Tag.InstanceNumber, "256");

                #endregion


                #region LUT
                //LUT
                InsertStringDicomElement(dataHeader, Tag.RescaleIntercept, "0");      //截距
                InsertStringDicomElement(dataHeader, Tag.RescaleSlope, "1");          //斜率
                InsertStringDicomElement(dataHeader, Tag.WindowCenter, "127");        //窗位
                InsertStringDicomElement(dataHeader, Tag.WindowWidth, "256");         //窗宽
                #endregion

                #region 插入PixelData
                var element = DicomAttribute.CreateAttribute(Tag.PixelData, VR.OB);
                if (!element.SetBytes(0, imageData))
                {
                    Logger.LogWarning("Failed to Insert NULL  image Data to Data header");
                }
                dataHeader.AddDicomAttribute(element);
                #endregion


                Logger.LogFuncDown();
                return dataHeader;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        #endregion

        #region 插入Dicom文件Tag方法

        private void InsertUInt16DicomElement(DicomAttributeCollection dataHeader, Tag uInt16TagName,
                                                    ushort tagValue)
        {
            var element = DicomAttribute.CreateAttribute(uInt16TagName);

            if (!element.SetUInt16(0, (ushort) tagValue))
            {
                Logger.LogWarning("Failed to Insert NULL " + uInt16TagName.ToString() + " to Data header");
            }
            dataHeader.AddDicomAttribute(element);
        }
        private void InsertStringDicomElement(DicomAttributeCollection dataHeader, Tag stringTagName,
                                                    string tagValue)
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
        private void InsertStringArrayDicomElement(DicomAttributeCollection dataHeader, Tag stringTagName,
                                                         string[] tagValues)
        {
            var element = DicomAttribute.CreateAttribute(stringTagName);
            for (int i = 0; i < tagValues.Length; i++)
            {
                if (!element.SetString(i, tagValues[i]))
                    Logger.LogWarning("Failed to Insert NULL " + stringTagName.ToString() + " to Data header");
            }
            dataHeader.AddDicomAttribute(element);
        }

        #endregion

        #region Dicom文件Tag信息获取方法

        //根据当前检查中的系列数生成系列号信息
        private string GetSerieNumber(string studyInstanceUid)
        {
            int serieNumber = 0;
            try
            {
                var db = DBWrapperHelper.DBWrapper;
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

        #endregion


    }
}
