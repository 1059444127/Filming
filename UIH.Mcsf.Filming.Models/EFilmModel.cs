using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using UIH.Mcsf.Database;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.Pipeline.Data;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Models
{
    public class EFilmModel
    {
        public EFilmModel()
        {
            this.OriginalSeriesInstanceUids = new List<string>();
            this.OriginalImageSopInstanceUids = new List<string>();
            this.BitsStored = 8;
            this.BitsAllocated = 8;
            this.IfSaveEFilm = false;

            Initialize();

        }

        //assume that film is mixed
        private void Initialize()
        {
            PatientName                                   = GlobalDefinition.Mixed;
            PatientId                                     = string.Empty;
            PatientBirthDate                              = string.Empty;
            PatientSex                                    = string.Empty;
            PatientsBirthTime                             = string.Empty;
            PatientsAge                                   = string.Empty;
            InstitutionName                               = string.Empty;
            Manufacturer                                  = string.Empty;
            StudyInstanceUid                              = GlobalDefinition.StarsString;
            StudyDate                                     = string.Empty;
            StudyTime                                     = string.Empty;
            PatientComments                               = string.Empty;
            ReferringPhysiciansName                       = string.Empty;
            PatientsSize                                  = string.Empty;
            PatientsWeight                                = string.Empty;
            Occupation                                    = string.Empty;
            AccessionNumber                               = string.Empty;
            EFilmModality                                 = GlobalDefinition.MixedEFilmModality;
            EFilmSeriesUid                                = string.Empty;
            ProtocolName                                  = string.Empty;
            BodyPartExamined                              = string.Empty;
            PatientPosition                               = string.Empty;
            FilmPageIndex                                 = string.Empty;
            AcquisitionNumber                             = string.Empty;
            ImagesInAcquisition                           = string.Empty;
            ImageComments                                 = string.Empty;
            BurnedInAnnotation                            = string.Empty;
            SecondaryCaptureDeviceID                      = string.Empty;
            SecondaryCaptureDeviceManufacturersModelName  = string.Empty;
            SecondaryCaptureDeviceSoftwareVersions        = string.Empty;
        }

        #region [--Print Settings--]

        #endregion



        #region [--Ouput Parameters--]

        public DicomAttributeCollection DataHeaderForPrint { get; set; }

        public DicomAttributeCollection DataHeaderForSave { get; set; }


        #endregion

        #region [---EFilm Tags---]

        public string PatientName { get; set; }
        public string PatientId { get; set; }
        public string PatientBirthDate { get; set; }
        public string PatientSex { get; set; }
        public string PatientsBirthTime { get; set; }
        public string PatientsAge { get; set; }
        public string InstitutionName { get; set; }
        public string Manufacturer { get; set; }
        public string StudyInstanceUid { get; set; }
        public string StudyDate { get; set; }
        public string StudyTime { get; set; }
        public string PatientComments { get; set; }
        public string ReferringPhysiciansName { get; set; }
        public string PatientsSize { get; set; } //患者身高
        public string PatientsWeight { get; set; }
        public string Occupation { get; set; } //患者职业
        public string AccessionNumber { get; set; }
        public string EFilmModality { get; set; } //电子胶片**设备类型 
        public string EFilmSeriesUid { get; set; } //系列**UID
        public string ProtocolName { get; set; }
        public string BodyPartExamined { get; set; }
        public string PatientPosition { get; set; }
        public string FilmPageIndex { get; set; }
        public string AcquisitionNumber { get; set; }
        public string ImagesInAcquisition { get; set; } //图片获取总数
        public string ImageComments { get; set; }
        public string BurnedInAnnotation { get; set; } //图片烧图注释？？
        public bool   IfSaveImageAsGrayScale { get; set; } //图片是否灰度图
        public ushort BitsAllocated { get; set; } // 允许存储位数
        public ushort BitsStored { get; set; } //实际存储位数
        public ushort Columns { get; set; } //图片**宽     临时变量    用于存储胶片（打印用）或者电子胶片的宽度    
        public ushort Rows { get; set; } //图片**高           临时变量    用于存储胶片（打印用）或者电子胶片的高度
        public string SecondaryCaptureDeviceID { get; set; } //二次获取设备ID
        public string SecondaryCaptureDeviceManufacturersModelName { get; set; }
        public string SecondaryCaptureDeviceSoftwareVersions { get; set; } //二次获取设备软件版本
        public bool IsMixed { get; set; } //是否混合患者胶片
        public bool IfSaveEFilm { get; set; } //是否执行保存电子胶片
        public bool IfPrint { get; set; }   //是否执行打印操作
        #endregion


        public IEnumerable<string> OriginalImageSopInstanceUids { get; private set; }
        public IEnumerable<string> OriginalSeriesInstanceUids { get; private set; }
        public Size FilmSize { get; set; }  //打印的胶片的大小
        public Size LowPrecisionEFilmSize { get; set; } //电子胶片大小

        public PeerNode PeerNode { get; set; }
        public PrinterSettingInfoModel PrintSettings { get; set; }

        public string PageTitlePosition { get; set; }

        public void FillEFilmTags()
        {
            //this.SecondaryCaptureDeviceID                            = Widget.GetTagValueFromCell(sampleCell, "");
            //this.SecondaryCaptureDeviceManufacturersModelName        = Widget.GetTagValueFromCell(sampleCell, "");
            //this.SecondaryCaptureDeviceSoftwareVersions              = Widget.GetTagValueFromCell(sampleCell, ""); 
        }
        public void FillTagsFrom(PageModel pageModel)
        {
            try
            {
                Logger.LogFuncUp();

                var pageNo = pageModel.PageTitleInfoModel.PageNo;
                if (!string.IsNullOrEmpty(pageNo))
                {
                    var parameters = pageNo.Split('/');
                    this.FilmPageIndex = parameters[0];
                    this.ImagesInAcquisition = parameters[1];
                }
                this.IsMixed = pageModel.PageTitleInfoModel.PatientName == GlobalDefinition.Mixed
                               || pageModel.PageTitleInfoModel.StudyDate == GlobalDefinition.Mixed;
                this.IfSaveImageAsGrayScale = !bool.Parse(pageModel.PrinterSettingInfoModel.IfColorPrint);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void FillTagsFrom(MedViewerControlCell sampleCell)
        {
            try
            {
                Logger.LogFuncUp();

                this.PatientName = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.PatientName);
                this.PatientId = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.PatientID);
                this.PatientBirthDate = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.PatientBirthDate);
                this.PatientSex = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.PatientSex);
                this.PatientsBirthTime = string.Empty;
                this.PatientsAge = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.PatientAge);
                this.InstitutionName = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.InstitutionName);
                this.Manufacturer = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.Manufacturer);
                this.StudyInstanceUid = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.StudyInstanceUID);
                this.StudyDate = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.StudyDate);
                this.StudyTime = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.StudyTime);
                this.PatientComments = string.Empty;
                this.ReferringPhysiciansName = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.ReferringPhysicianName);
                this.PatientsSize = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.PatientSize);
                this.PatientsWeight = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.PatientWeight);
                this.Occupation = string.Empty;
                this.AccessionNumber = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.AccessionNumber);
                //this.EFilmModality = Widget.GetTagValueFromCell(sampleCell, "Modality");
                //this.EFilmSeriesUid = string.Empty; //todo:生成seriesUID                      
                this.ProtocolName = string.Empty;
                this.BodyPartExamined = string.Empty;
                this.PatientPosition = string.Empty;
                this.AcquisitionNumber = string.Empty;
                this.ImagesInAcquisition = string.Empty;
                this.ImageComments = Widget.GetTagValueFromCell(sampleCell, ServiceTagName.ImageComments);
                this.BurnedInAnnotation = string.Empty;

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
    }
}
