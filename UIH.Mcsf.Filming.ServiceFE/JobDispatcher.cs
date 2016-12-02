using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Xml;
using McsfCommonSave;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Core;
using UIH.Mcsf.Database;
using UIH.Mcsf.DicomConvertor;
using UIH.Mcsf.Filming.Models;
using UIH.Mcsf.Filming.Views;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.MedDataManagement;

namespace UIH.Mcsf.Filming.ServiceFE
{
    public class JobDispatcher : IEventHandler
    {

        #region [--Static Fields--]
        private RenderWindow _renderWindow;
        #endregion



        #region  [---Fields---]
        private List<CardModel> cardModelList = new List<CardModel>(); //电子胶片实体集合
        private bool isPrint = true; //是否打印
        private int saveImgCurrentNum = 0;//当前保存电子胶片数

        private Dictionary<string, string> eFilmGroupSeriesUid = new Dictionary<string, string>(); //根据检查序列分组，key是检查序列号，value电子胶片创建序列号
        private Dictionary<string, string> eFilmGroupTime = new Dictionary<string, string>();//key是检查序列号
        private Dictionary<string, int> eFilmGroupCount = new Dictionary<string, int>();//key是检查序列号
        private Dictionary<string, int> eFilmGroupSaveCount = new Dictionary<string, int>();//key是检查序列号
        #endregion



        #region [---Properties---]

        #endregion



        #region  [---Singleton--]
        private static volatile JobDispatcher _instance;
        private static readonly object LockHelper = new object();

        

        public static JobDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockHelper)
                    {
                        if (_instance == null)
                            _instance = new JobDispatcher();
                    }
                }
                return _instance;
            }
        }
        #endregion



        #region  [---Constructor---]
        private JobDispatcher()
        {
            var thread = new Thread(this.StartUI);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }
        #endregion

        #region  [---Public Methods---]

        public void DeserializedFromXml(byte[] contentsByte)
        {
            bool canDeserialized = true;
            try
            {
                Logger.LogFuncUp();
                
                cardModelList = new List<CardModel>();
                string contentsString = Encoding.UTF8.GetString(contentsByte);
                var xDoc = new XmlDocument();
                xDoc.LoadXml(contentsString);

                var rootNode = xDoc.DocumentElement;
                if (null != rootNode)
                {
                    var allFilmPagesNode = rootNode.SelectSingleNode(OffScreenRenderXmlHelper.ALL_FILMING_PAGE_INFO);
                    if (allFilmPagesNode != null)
                    {
                        foreach (XmlNode filmPageNode in allFilmPagesNode)
                        {
                            var filmCard = new CardModel();
                            filmCard.DeserializedFromXml(contentsByte, filmPageNode);
                            cardModelList.Add(filmCard);
                        }
                        isPrint = cardModelList[0].IsPrint;
                    }
                    else
                    {
                        Logger.LogWarning("[FilmingSerivceFE]No Common Page info for print");
                        canDeserialized = false;
                    }
                }
                else
                {
                    Logger.LogWarning("[FilmingSerivceFE]No Card info for print");
                    canDeserialized = false;
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                canDeserialized = false;
            }

            if(!canDeserialized)
            {
                Logger.LogError("[FilmingSerivceFE]Fail to Deserialized card info for print");
                Containee.Main.ShowStatusWarning("UID_Filming_OffScreen_Job_Fail");
            }

        }

        public void AddJobToFilmCard(byte[] buffer)
        {
            try
            {
                isPrint = true; 
                saveImgCurrentNum = 0; 
                DeserializedFromXml(buffer);
                CardModel.filmBoxList.Clear();

                foreach (var cardModel in cardModelList)
                {
                    GenerateEFilmModel(cardModel);
                    CreatePrintJob(cardModel);
                    SaveEFilm(cardModel);
                    ReleaseMemory(cardModel);
                }
                SendToServiceBE();
                
            }catch(Exception ex)
            {
                Logger.LogError("[FilmingSerivceFE]Fail to AddJobToFilmCard");
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }finally
            {
                CardModel.filmBoxList.Clear();
                CardModel.CurrentPeer = null;
                CardModel.CurrentPatient = null;
                cardModelList.Clear();
                eFilmGroupSeriesUid.Clear();
                eFilmGroupTime.Clear();
                eFilmGroupCount.Clear();
                eFilmGroupSaveCount.Clear();
                ReleaseMemory(null);
            }

        }

        private void GenerateEFilmModel(CardModel cardModel)
        {
            try
            {
                var filmCard = cardModel;
                var tempFilmCard = filmCard;  //处理 Resharper 提示Access to modified closure
                _renderWindow.Dispatcher.Invoke(new Action(() => _renderWindow.AddFilmCard(tempFilmCard)));
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
       
        private void GetEFilmGroupInfo(CardModel cardModel)
        {
            try
            {
                var eFilmPage = cardModel.FilmPageModel;
                if (!eFilmPage.EFilmModel.IsMixed)
                {
                    var studyUid = eFilmPage.PageTitleInfoModel.StudyInstanceUid;
                    string time = null;
                    string efilmSeriesUid = null;
                    int sum=0;
                    if (!eFilmGroupSeriesUid.ContainsKey(studyUid))
                    {
                        time = DateTime.Now.ToString();
                        efilmSeriesUid = CreateSeries(eFilmPage.EFilmModel);
                        eFilmGroupSeriesUid.Add(studyUid, efilmSeriesUid);
                        eFilmGroupTime.Add(studyUid, time);
                        int count = cardModelList.Count();
                        for (int i = 0; i < count; i++)
                        {
                            CardModel model = cardModelList[i];
                            if ((model != null) && (model.FilmPageModel != null) && (model.FilmPageModel.EFilmModel != null) && (!model.FilmPageModel.EFilmModel.IsMixed))
                            {
                                if ( (model.FilmPageModel.PageTitleInfoModel != null) && (model.FilmPageModel.PageTitleInfoModel.StudyInstanceUid == studyUid))
                                {
                                    sum++;
                                }
                            }
                        }
                        eFilmGroupCount.Add(studyUid, sum);
                        eFilmGroupSaveCount.Add(studyUid, 0);
                    }


                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
        private void SaveEFilm(CardModel cardModel)
        {
            try
            {
                if (cardModel.IsPrint && bool.Parse(cardModel.PrinterSettingInfoModel.IfSave) || !cardModel.IsPrint)
                {
                    GetEFilmGroupInfo(cardModel);
                    var eFilmPage = cardModel.FilmPageModel;
                    if (!eFilmPage.EFilmModel.IsMixed)
                    {
                        var studyUid = eFilmPage.PageTitleInfoModel.StudyInstanceUid;
                        string time = null;
                        string efilmSeriesUid = null;
                        int sum = 0;
                        int saveSum = 0;
                        if (eFilmGroupSeriesUid.ContainsKey(studyUid))
                        {
                            time = eFilmGroupTime[studyUid];
                            efilmSeriesUid = eFilmGroupSeriesUid[studyUid];
                            sum = eFilmGroupCount[studyUid];
                            saveSum = eFilmGroupSaveCount[studyUid] + 1 ;
                            eFilmGroupSaveCount[studyUid] = saveSum;
                        }
                        
                        if (string.IsNullOrWhiteSpace(efilmSeriesUid))
                        {
                            Logger.LogFuncException(FilmingUtility.FunctionTraceEnterFlag +
                                                    "Fail to create series for efilm");

                        }
                        else
                        {
                            eFilmPage.EFilmModel.EFilmSeriesUid = efilmSeriesUid;
                            SaveEFilmInCommonSave(eFilmPage.EFilmModel, time,
                                                  ((double)saveSum) / sum);
                        }

                    }
                  //  ReleaseMemory(cardModel);
                    ++saveImgCurrentNum;
                    if (saveImgCurrentNum == cardModelList.Count)
                    {
                        if (Containee.Main != null)
                        {
                            Containee.Main.ShowStatusInfo("UID_Filming_End_To_Save_EFilm");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void CreatePrintJob(CardModel filmCard)
        {
            try
            {
                if (filmCard.IsPrint)
                {
                    var peer = filmCard.PeerNode;
                    if (peer.NodeType == PeerNodeType.FILM_PRINTER)
                    {
                        // JobCreator jobCreator = filmCard.CreateJobCreator(peer, Containee.Main.GetCommunicationProxy(),cardModelList);
                        //  jobCreator.SendFilmingJobCommand(Containee.Main.GetCommunicationProxy());
                        filmCard.CreateFilmBoxList(peer, Containee.Main.GetCommunicationProxy(), cardModelList);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void SendToServiceBE()
        {
            try
            {
                if (!isPrint) return;
                JobCreator jobCreator = CardModel.CreateJobCreator();
                jobCreator.SendFilmingJobCommand(Containee.Main.GetCommunicationProxy());
            }catch(Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private static void ReleaseMemory(CardModel cardModel)
        {
            if (cardModel != null)
            {
                cardModel.Dispose();
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        #endregion

        #region [---Overrided Event Handler---]

        public override int HandleEvent(string sender, int channelId, int eventId, string serialzedObject)
        {

            Logger.LogWarning("chanelID: " + channelId + ", eventID: " + eventId + ", serialzedObject: " + serialzedObject);

            if (eventId == EventID.ImageText_Updated_Service_Config_Panel && serialzedObject == "ImageText_Updated_Service_Config_Panel")
                _renderWindow.Dispatcher.Invoke(new Action(() => _renderWindow.UpdateFilmingImageText()));
            else if (eventId == EventID.ImageProperty_Updated_Service_Config_Panel && serialzedObject == "ImageProperty_Updated_Service_Config_Panel")
                _renderWindow.Dispatcher.Invoke(new Action(() => _renderWindow.UpdateFilmingImageProperty()));
            else if (eventId == EventID.MCSF_SERVICE_EVENT_CONFIGURATION_XML_MODIFYED && serialzedObject == "PrinterConfig_Updated_Service_Configuration")
                _renderWindow.Dispatcher.Invoke(new Action(() => _renderWindow.UpdateFilmingPrinterConfig()));

            return 0;
        }


        #endregion //[---Overrided Event Handler---]

        

        #region  [---Private Methods---]
        private void StartUI()
        {
            _renderWindow = new RenderWindow();
            _renderWindow.ShowDialog();
        }

        private void SaveEFilmInCommonSave(EFilmModel eFilmModel , string jobID,double progress)
        {

            //系列化DataHeader
            byte[] serializedInfo;
            if (eFilmModel.DataHeaderForSave != null)
            {
                eFilmModel.DataHeaderForSave.Serialize(out serializedInfo);
            }
            else
            {
                serializedInfo = new byte[0];
            }

            //设置commonsave参数，todo 主干PS接口有变更
            SaveFilmingCommandContext.Builder builder = SaveFilmingCommandContext.CreateBuilder();
            
            builder.SetCellIndex(0);
            builder.SetSaveImageType(SavingType.SecondaryCapture);
            builder.SetOperationType(SaveFilmingMode.Save);
            builder.SetStrategy(SaveFilmingStrategy.SaveImages);
            builder.SetKeepSameSeries(true);
            builder.SetSeriesUID(eFilmModel.EFilmSeriesUid);
            var rangeBuilder = new PrgressRange.Builder();
            rangeBuilder.JobID = jobID;
            rangeBuilder.SetMaxForCurrentSubJob(progress);
            builder.SetRange(rangeBuilder);
            ImageAuxiliary.Builder auxiliaryBuilder = ImageAuxiliary.CreateBuilder();
            //auxiliaryBuilder.ActivePS = string.Empty;
            //auxiliaryBuilder.BurnedPS = string.Empty;
            auxiliaryBuilder.PS = string.Empty;
            auxiliaryBuilder.CellIndex = -1;//区别于 SaveSeries
            auxiliaryBuilder.SaveAsDisplay = true;
            auxiliaryBuilder.DataHeaderBytes = Google.ProtocolBuffers.ByteString.CopyFrom(serializedInfo);
            builder.AddImageAuxiliaries(auxiliaryBuilder);
            byte[] btInfo = builder.Build().ToByteArray();

            

            //SAVE_EFILM_COMMAND
            CommandContext cs = new CommandContext();
            cs.iCommandId = 16000; //7088
            cs.sReceiver = CommunicationNodeName.CreateCommunicationProxyName("FilmingService");
            cs.sSerializeObject = btInfo;
            //var result = Containee.Main.GetCommunicationProxy().SyncSendCommand(cs);
            int errorCode = Containee.Main.GetCommunicationProxy().AsyncSendCommand(cs);
            if (0 != errorCode)
            {
                throw new Exception("send filming job command failure, error code: " + errorCode);
            }

            builder.Clear();
           // if (eFilmModel.DataHeaderForSave != null) eFilmModel.DataHeaderForSave.Dispose();
           
        }

        //string study, string modality, out string seriesUID, bool bSaveEFilm
        private string CreateSeries(EFilmModel eFilmModel)
        {
            var studyInstanceUid = eFilmModel.StudyInstanceUid;

            Series series = DBWrapperHelper.DBWrapper.CreateSeries();

            var uidManager = McsfDatabaseDicomUIDManagerFactory.Instance().CreateUIDManager();
            series.SeriesInstanceUID = uidManager.CreateSeriesUID("");//eFilmModel.EFilmSeriesUid;
            series.StudyInstanceUIDFk = studyInstanceUid;
            series.Modality = eFilmModel.EFilmModality;
            // new seriesNumber equals the max seriesNumber of exist series add one
            series.SeriesNumber = Convert.ToInt32(GetSerieNumber(studyInstanceUid)) + 1;
            series.ReconResult = 2; //代表重建已完成
            if (studyInstanceUid == "****") return string.Empty; //FilmingHelper.StarsString) return;


                //check whether disk space is enough
                ICommunicationProxy pCommProxy = Containee.Main.GetCommunicationProxy();
                var target = new SystemResManagerProxy(pCommProxy);
                if (!target.HaveEnoughSpace())
                {
                    Logger.LogError("No enough disk space, so Electronic Image Series will not be created");
                    Containee.Main.ShowStatusError("UID_Filming_No_Enough_Disk_Space_To_Create_Electronic_Image_Series");
                    return string.Empty;
                }

                series.Save();
            return series.SeriesInstanceUID;
        }


        public static string GetSerieNumber(string studyInstanceUid)
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
