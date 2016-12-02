using System;
using System.Collections.Generic;
using McsfCommonSave;
using UIH.Mcsf.Core;
//using Mcsf;

namespace UIH.Mcsf.Filming
{
    public class JobCreator
    {
        #region Interface

        /// <summary>
        /// send filming command to filming module
        /// </summary>
        /// <param name="proxy">communication proxy of the filming command sender</param>
        /// 

        /// <key>\n 
        /// PRA:Yes \n
        /// Traced from: SSFS_PRA_ImageError_SW_SW_DataTransfer \n
        /// Tests: N/A \n
        /// Description:  Send Filming Job Info to Backend Printer Serives\n
        /// Short Description:  FilmingCommand\n
        /// Component:Filming \n
        /// </key> \n

        public void SendFilmingJobCommand(ICommunicationProxy proxy)
        {
            try
            {
                Logger.LogFuncUp();

                CommandContext cs = new CommandContext();
                cs.iCommandId = MCSF_AUTO_FILMING_COMMAND_ID;//7088
                //cs.sSender = param.GetValue(2).ToString();// "FilmingFE";
                //cs.sReceiver = param.GetValue(3).ToString();// "FilmingBE";
                //cs.sReceiver = "FilmingBE";
                cs.sReceiver = CommunicationNodeName.CreateCommunicationProxyName(MCSF_FILMING_NAME);//CreateCommunicationProxyName(MCSF_FILMING_NAME, FRONT_END);
                //cs.pCommandCallback = (ICommandCallbackHandler)param.GetValue(4);
                //cs.bServiceAsyncDispatch = true;
                byte[] serializedJob = CreateFilmingJobInstance();
                if (serializedJob == null) throw new Exception("send filming job command failure, serializedJob is null ");
                cs.sSerializeObject = serializedJob;

               // cs.bServiceAsyncDispatch = true;

                int errorCode = proxy.AsyncSendCommand(cs);
                if (0 != errorCode)
                    throw new Exception("send filming job command failure, error code: " + errorCode);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        #endregion  //Interface

        #region Properties

        private string _archivedSeriesInstanceUid = string.Empty;
        public string ArchivedSeriesInstanceUid { get { return _archivedSeriesInstanceUid; } set { _archivedSeriesInstanceUid = value; } }   //2014-05-04 For xiongke hospital, archive new series of all images printed

        public Patient Patient
        {
            get { return _patient; }
            set { _patient = value; }
        }

        public PeerNode Peer
        {
            get { return _peer; }
            set { _peer = value; }
        }

        public PrintSetting PrintSetting
        {
            get { return _printSetting; }
            set { _printSetting = value; }
        }

        public IList<FilmingPrintJob.Types.FilmBox.Builder> FilmBoxList
        {
            get { return _filmBoxList; }
            set { _filmBoxList = value; }
        }

        #endregion  //Properties

        #region Command Parameters

        private readonly int MCSF_AUTO_FILMING_COMMAND_ID = 7088;//7076
        //private const int MCSF_ADD_FILMING_JOB_COMMAND_ID = 7088;
        private readonly string MCSF_FILMING_NAME = "FilmingService";
        //private readonly string FRONT_END = "FE";
        //private readonly string BACK_END = "BE";

        #endregion  //Command Parameters

        #region Fields

        Patient _patient = new Patient();
        PeerNode _peer = new PeerNode();
        PrintSetting _printSetting = new PrintSetting();
        IList<FilmingPrintJob.Types.FilmBox.Builder> _filmBoxList = new List<FilmingPrintJob.Types.FilmBox.Builder>();

        #endregion  //Fields

        #region Private Methods

            /// <summary>
        /// after call Set* methods init instance, then call this 
        /// function to create a ProtoBuffer serialized object
        /// </summary>
        /// <returns>
        /// a serialized filming job instance
        /// if there is an exception, return null
        /// </returns>
        byte[] CreateFilmingJobInstance()
        {
            byte[] serializedJob = null;
            try
            {
                Logger.LogFuncUp();

                FilmingPrintJob filmingPrintJob = new FilmingPrintJob();
                FilmingPrintJob.Builder filmingPrintJobBuilder = new FilmingPrintJob.Builder();

                //set printer
                filmingPrintJobBuilder.SetPrinterAE(Peer.PeerAE);
                filmingPrintJobBuilder.SetOurAE(Printers.Instance.OurAE);
                filmingPrintJobBuilder.SetPrinterIP(Peer.PeerIP);
                filmingPrintJobBuilder.SetPort(Peer.PeerPort);

                //set print settign
                filmingPrintJobBuilder.SetPrintPriority((FilmingPrintJob.Types.PrintPriority)PrintSetting.Priority);
                filmingPrintJobBuilder.SetPrintTiming(FilmingPrintJob.Types.PrintTiming.IMMEDIATELY);
                filmingPrintJobBuilder.SetCopies((int)PrintSetting.Copies);
                filmingPrintJobBuilder.SetFilmingDate(PrintSetting.FilmingDateTime.ToShortDateString());
                filmingPrintJobBuilder.SetFilmingTime(PrintSetting.FilmingDateTime.ToShortTimeString());
                filmingPrintJobBuilder.SetIfSaveEFilm(PrintSetting.IfSaveElectronicFilm);

                //set patient
                filmingPrintJobBuilder.SetPatientId(Patient.PatientID);
                filmingPrintJobBuilder.SetPatientName(Patient.PatientName);
                filmingPrintJobBuilder.SetPatientSex(Patient.PatientSex);
                filmingPrintJobBuilder.SetPatientAge(Patient.PatientAge);
                filmingPrintJobBuilder.SetOperatorName(Patient.OperatorName);
                filmingPrintJobBuilder.SetAccessionNo(Patient.AccessionNo);
                filmingPrintJobBuilder.SetStudyId(Patient.StudyID);

                //2014-05-04 for xiongke hospital
                filmingPrintJobBuilder.SetMediaType(PrintSetting.MediaType);
                filmingPrintJobBuilder.SetFilmDestination(PrintSetting.FilmDestination);
                filmingPrintJobBuilder.SetIfColorPrint(PrintSetting.IfColorPrint);
                //filmingPrintJobBuilder.SetFilmSessionLabel("");
                //filmingPrintJobBuilder.SetPriority("");
                //filmingPrintJobBuilder.SetOwnerId("");
                filmingPrintJobBuilder.SetSeriesInstanceUid(string.IsNullOrEmpty(ArchivedSeriesInstanceUid) ? string.Empty : ArchivedSeriesInstanceUid);
                //2014-05-04 for xiongke hospital

                foreach (FilmingPrintJob.Types.FilmBox.Builder filmBoxBuilder 
                    in FilmBoxList)
                {
                    filmingPrintJobBuilder.AddFilmBox(filmBoxBuilder);
                }

                filmingPrintJob = filmingPrintJobBuilder.Build();

                serializedJob = filmingPrintJob.ToByteArray();

                Logger.LogFuncDown();
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                serializedJob = null;
            }
            return serializedJob;
        }

        //public FilmingPrintJob CreateFilmingJob()
        //{
        //    FilmingPrintJob filmingPrintJob = new FilmingPrintJob();

        //    try
        //    {
        //        Logger.LogFuncUp();

        //        FilmingPrintJob.Builder filmingPrintJobBuilder = new FilmingPrintJob.Builder();

        //        //set printer
        //        filmingPrintJobBuilder.SetPrinterAE(Peer.PeerAE);
        //        filmingPrintJobBuilder.SetOurAE(Printers.Instance.OurAE);
        //        filmingPrintJobBuilder.SetPrinterIP(Peer.PeerIP);
        //        filmingPrintJobBuilder.SetPort(Peer.PeerPort);

        //        //set print setting
        //        filmingPrintJobBuilder.SetPrintPriority((FilmingPrintJob.Types.PrintPriority)PrintSetting.Priority);
        //        filmingPrintJobBuilder.SetPrintTiming(FilmingPrintJob.Types.PrintTiming.IMMEDIATELY);
        //        filmingPrintJobBuilder.SetCopies((int)PrintSetting.Copies);
        //        filmingPrintJobBuilder.SetFilmingDate(PrintSetting.FilmingDateTime.ToShortDateString());
        //        filmingPrintJobBuilder.SetFilmingTime(PrintSetting.FilmingDateTime.ToShortTimeString());
        //        filmingPrintJobBuilder.SetIfSaveEFilm(PrintSetting.IfSaveElectronicFilm);

        //        //set patient
        //        filmingPrintJobBuilder.SetPatientId(Patient.PatientID);
        //        filmingPrintJobBuilder.SetPatientName(Patient.PatientName);
        //        filmingPrintJobBuilder.SetPatientSex(Patient.PatientSex);
        //        filmingPrintJobBuilder.SetPatientAge(Patient.PatientAge);
        //        filmingPrintJobBuilder.SetOperatorName(Patient.OperatorName);
        //        filmingPrintJobBuilder.SetAccessionNo(Patient.AccessionNo);
        //        filmingPrintJobBuilder.SetStudyId(Patient.StudyID);

        //        //2014-05-04 for xiongke hospital
        //        filmingPrintJobBuilder.SetMediaType(PrintSetting.MediaType);
        //        filmingPrintJobBuilder.SetFilmDestination(PrintSetting.FilmDestination);
        //        filmingPrintJobBuilder.SetIfColorPrint(PrintSetting.IfColorPrint);
        //        //filmingPrintJobBuilder.SetFilmSessionLabel("");
        //        //filmingPrintJobBuilder.SetPriority("");
        //        //filmingPrintJobBuilder.SetOwnerId("");
        //        filmingPrintJobBuilder.SetSeriesInstanceUid(string.IsNullOrEmpty(ArchivedSeriesInstanceUid) ? string.Empty : ArchivedSeriesInstanceUid);
        //        //2014-05-04 for xiongke hospital


        //        foreach (FilmingPrintJob.Types.FilmBox.Builder filmBoxBuilder
        //            in FilmBoxList)
        //        {
        //            filmingPrintJobBuilder.AddFilmBox(filmBoxBuilder);
        //        }

        //        filmingPrintJob = filmingPrintJobBuilder.Build();

        //        Logger.LogFuncDown();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //        filmingPrintJob = null;
        //    }

        //    return filmingPrintJob;
        //}

        #endregion  //Private Methods

    }
}
