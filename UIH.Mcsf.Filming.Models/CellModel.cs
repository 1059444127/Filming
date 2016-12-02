using System;
using System.IO;
using System.Xml;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Pipeline.Data;

namespace UIH.Mcsf.Filming.Models
{
    public class CellModel
    {
        private static readonly IBasicLoader DataLoader;

        static CellModel()
        {
            DataLoader = DataLoaderFactory.Instance().CreateSyncSopLoader(DBWrapperHelper.DBWrapper);
        }

        #region [---Properties---]

        public string SopUid { get; private set; }

        public string Ps { get; private set; }
        public Sop Sop { get; private set; }

        public string SeriesUid { get; set; }

        public string PatientAge { get; set; }

        public string PatientId { get; set; }

        public string PatientsName { get; set; }

        public string PatientsSex { get; set; }

        public string StudyId { get; set; }

        public string StudyInstanceUid { get; set; }

        public string LocalizedImageUid { get; private set; }

        public Sop LocalizedImageSop { get; private set; }

        public bool RulerVisible { get; private set; }

        public string ImgTxtFilePathOrContent { get; private set; }

        public string ImgTxtItemPathOrContent { get; private set; }

        public string ImgTxTContent { get; private set; }

        public string ImgTxTPosition { get; private set; }

        public string MgOrientationIsShow { get; private set; }

        public string IsShowImgTxt { get; private set; }

        public string IsShowRuler { get; private set; }

        public string DataHeaderFilePath { get; private set; }

        public DicomAttributeCollection DataHeader { get; private set; }

        public int DataHeaderLength { get; private set; }

        public string Modality { get; set; }

        public string LocalizedImagePSInfo { get;private set; }

        public string LocalizedImagePosion { get; private set; }

        public string PtUnit { get; private set; } //传递f1的pt单位至f2
        #endregion



        #region [---Public Methods---]

        public bool DeserializedFromXml(XmlNode currNode)
        {
            try
            {
                Logger.LogFuncUp();

                this.SopUid = currNode.Attributes[OffScreenRenderXmlHelper.SOP_UID].Value;
                if (!string.IsNullOrWhiteSpace(this.SopUid))
                {
                    var sop = DataLoader.LoadSopByUid(this.SopUid);
                    if (null == sop)
                    {
                        Logger.LogWarning("[FilmingSerivceFE] Fail to Load an image of sopInstanceUid : " + this.SopUid);
                        if(!CreatSOPFromDataHerderFile(currNode)) return false;//todo:填写SopInstance不规范，数据库查不到
                    }
                    else
                    {
                        this.Sop = sop;
                        this.Ps = currNode.InnerXml;
                        CreatSOPFromDataHerderFile(currNode);
                    }
                }

                //定位像
                this.LocalizedImageUid = currNode.Attributes[OffScreenRenderXmlHelper.LOCALIZED_SOP_UID].Value;
                this.LocalizedImageSop = string.IsNullOrWhiteSpace(this.LocalizedImageUid) ? null : DataLoader.LoadSopByUid(this.LocalizedImageUid);
                this.LocalizedImagePSInfo = currNode.Attributes[OffScreenRenderXmlHelper.LOCALIZED_IMAGE_PS_INFO] != null
                                            ? currNode.Attributes[OffScreenRenderXmlHelper.LOCALIZED_IMAGE_PS_INFO].Value 
                                            : "";
                this.LocalizedImagePosion = currNode.Attributes[OffScreenRenderXmlHelper.LOCALIZED_IMAGE_POSITION] !=null
                                            ? currNode.Attributes[OffScreenRenderXmlHelper.LOCALIZED_IMAGE_POSITION].Value
                                            : "";
                //比例尺
                this.RulerVisible = bool.Parse(currNode.Attributes[OffScreenRenderXmlHelper.SCALE_RULER].Value);

                //四角信息
                this.ImgTxtFilePathOrContent = currNode.Attributes[OffScreenRenderXmlHelper.IMG_TEXT_FILE_PATH_OR_CONTENT].Value;
                this.ImgTxtItemPathOrContent = currNode.Attributes[OffScreenRenderXmlHelper.IMG_TEXT_ITEM_PATH_OR_CONTENT].Value;
                this.ImgTxTContent = currNode.Attributes[OffScreenRenderXmlHelper.IMG_TEXT_CONTENT].Value;
                this.ImgTxTPosition = currNode.Attributes[OffScreenRenderXmlHelper.IMG_TEXT_POSITION] != null ? currNode.Attributes[OffScreenRenderXmlHelper.IMG_TEXT_POSITION].Value : "Center";
                this.MgOrientationIsShow =currNode.Attributes[OffScreenRenderXmlHelper.MG_Orientation_IsShow]!=null? currNode.Attributes[OffScreenRenderXmlHelper.MG_Orientation_IsShow].Value:"0";
                this.IsShowImgTxt = currNode.Attributes[OffScreenRenderXmlHelper.Is_Show_ImgText] != null ? currNode.Attributes[OffScreenRenderXmlHelper.Is_Show_ImgText].Value : "0";
                this.IsShowRuler = currNode.Attributes[OffScreenRenderXmlHelper.Is_Show_Ruler] != null ? currNode.Attributes[OffScreenRenderXmlHelper.Is_Show_Ruler].Value : "0";

                this.PtUnit = currNode.Attributes[OffScreenRenderXmlHelper.PT_UNIT] != null ? currNode.Attributes[OffScreenRenderXmlHelper.PT_UNIT].Value : "";
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return false;
            }

            return true;
        }

        private bool CreatSOPFromDataHerderFile(XmlNode currNode)
        {
            //没有SopInstanceUID或者从数据库中找不到该SopInstanceUID
            bool result = false;
            this.DataHeaderFilePath = currNode.Attributes[OffScreenRenderXmlHelper.DATA_HEADER_FILE_PATH].Value;
            if (!string.IsNullOrEmpty(currNode.Attributes[OffScreenRenderXmlHelper.DATA_HEADER_LENGTH].Value))
            {
                this.DataHeaderLength = int.Parse(currNode.Attributes[OffScreenRenderXmlHelper.DATA_HEADER_LENGTH].Value);
            }

            if (!string.IsNullOrEmpty(this.DataHeaderFilePath))
            {
                if (File.Exists(this.DataHeaderFilePath))
                {
                    byte[] byteData = new byte[this.DataHeaderLength];
                    using (var fs = new FileStream(this.DataHeaderFilePath, FileMode.Open))
                    {
                        int read;
                        while ((read = fs.Read(byteData,0,byteData.Length))>0)
                        {
                            fs.Write(byteData,0,read);
                        }
                        fs.Close();
                    }
                    //删除文件
                    File.Delete(this.DataHeaderFilePath);

                    this.DataHeader = DicomAttributeCollection.Deserialize(byteData);
                    if (null == this.DataHeader)
                    {
                        Logger.LogWarning("[FilmingSerivceFE] Fail to Deserialized DicomAttributeCollection !");
                    }
                    this.Sop = SopInstanceFactory.Create(this.DataHeader, string.Empty);
                    this.Ps = (this.Sop as ImageSop).PresentationState;
                    result= true;
                }
                else
                {
                    Logger.LogError("[FilmingSerivceFE] CreatSOPFromDataHerderFile DataHeaderFilePath not exist !");
                }
            }
            return result;
        }
        #endregion


    }
}
