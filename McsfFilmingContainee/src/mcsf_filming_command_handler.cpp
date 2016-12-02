//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_filming_command_handler.cpp
///  \brief   filming command handler
///
///  \version 1.0
///  \date    Dec. 26, 2011
///  
//////////////////////////////////////////////////////////////////////////

#include <sstream>
#include <exception>
//#include <ctime>

#include "boost/lexical_cast.hpp"
#include "boost/filesystem.hpp" // for boost filesystem
#include <boost/thread/mutex.hpp>
#include "ace/Semaphore.h"

#include "McsfNetBase/mcsf_communication_node_name.h"

#include "Common/McsfSystemEnvironmentConfig/mcsf_systemenvironment_factory.h"

//DB-importer
#include "McsfDicomConvertor/mcsf_dbimporter_interface.h"
#include "McsfMHC/mcsf_mhc_statusinfo_handler.h"

#include "McsfArchiving/mcsf_archiving_auto_interface.h"

//#include "McsfService/McsfSendLogToSC.h"

//#include "PrinterConfig.pb.h"
//#include "McsfAppToolkit/McsfAppSaveFilming/mcsf_save_filming_cmd_context.pb.h"
#include "mcsf_dcm_printer_config.h"
#include "mcsf_filming_command_handler.h"

MCSF_FILMING_BEGIN_NAMESPACE

using namespace McsfCommonSave;
using namespace Mcsf::MHC;

//static ACE_Semaphore kSuspendedJobListMutex_;    //for mutually access to suspended job list
static boost::mutex kJobListMutex;         //for mutually access to suspended job list
static boost::mutex m_mutexSuspendAll;
static bool m_locked = false;
static std::string m_sJobManagerName = CommunicationNodeName::CreateCommunicationProxyName("JobManager", "FE");
static std::string m_sPAName = CommunicationNodeName::CreateCommunicationProxyName("PA", "FE");
static std::string m_sFilmingServiceName = CommunicationNodeName::CreateCommunicationProxyName("FilmingService");

std::map<unsigned int, FilmingPrintJob>  McsfFilmingCommandHandler::m_PrintJobMap;
McsfFilmingDB* McsfFilmingCommandHandler::m_pFilmingDB = NULL;
IDatabasePtr McsfFilmingCommandHandler::m_pDatabase = IDatabaseFactory::Instance() ->CreateDBWrapper();

ICommunicationProxy* McsfFilmingCommandHandler::m_pCommProxy = NULL;
McsfDicomJobManager* McsfFilmingCommandHandler::m_pDicomJobManager = NULL;
int McsfFilmingCommandHandler::m_iRetryConnectPrinterTimes = 5;
int McsfFilmingCommandHandler::m_iSetFilmBoxTimeOutTime = 120;

McsfFilmingCommandHandler::~McsfFilmingCommandHandler(void)
{
    try
    {
        if(NULL != m_pDicomJobManager)
        {
            delete m_pDicomJobManager;
            m_pDicomJobManager = NULL;
        }
    }
    catch(...)
    {

    }
}

McsfFilmingCommandHandler::McsfFilmingCommandHandler(ICommunicationProxy* pCommProxy)
    : m_iThreadId(0), m_sCurrentCommandSender("")
{
    try
    {
        if(NULL == pCommProxy)
        {
            LOG_ERROR("McsfFilmingCommandHandler has null parameter!");
            return ;
        }

        m_pCommProxy = pCommProxy;

        m_pDicomJobManager = new McsfDicomJobManager();

        m_pFilmingDB = McsfFilmingDB::GetInstance();

        McsfPrintJobObject pPrintJobObject;// = new McsfPrintJobObject();
        //McsfFilmingLibaryFactory* pFilming = McsfFilmingLibaryFactory::Instance();

        m_pDatabase = IDatabaseFactory::Instance() ->CreateDBWrapper();
        if(! m_pDatabase->Initialize())//Initialize
        {
            std::cout<<"Can't connect to database!"<<std::endl;
            LOG_ERROR("Filiming BE can't connnect to DB!");
            return ;//connect to DB error!
        }
        m_pDatabase->SetAutoNotifyOn(pCommProxy);

        m_pDBNotifier = IDatabaseFactory::Instance() ->CreateDBWrapper();
        if(! m_pDBNotifier->Initialize())//Initialize
        {
            std::cout<<"Can't connect to database!"<<std::endl;
            LOG_ERROR("Filiming BE can't connnect to DB!");
            return ;//connect to DB error!
        }
        //m_pDBNotifier->SetAutoNotifyOn(pCommProxy);
        m_pDBNotifier->SetAutoNotifyOff();


        ////////////////////////////////////////////////////////////////////////////
        ////begin to read config

        //IFileParser* pFileParser = Mcsf::ConfigParserFactory::Instance()->GetXmlFileParser();
        //if (NULL == pFileParser)
        //{
        //	LOG_ERROR_FILMING("Can't get Printer FileParser instance");
        //	return;
        //}
        //pFileParser->Initialize();

        //string sPrinterConfigFilePath;
        //ISystemEnvironmentConfig *pSysConfig = 
        //	ConfigSystemEnvironmentFactory::Instance()->GetSystemEnvironment();
        //if (NULL != pSysConfig)
        //{
        //	sPrinterConfigFilePath = pSysConfig->GetApplicationPath("FilmingConfigPath");
        //}
        //else
        //{
        //	LOG_ERROR_FILMING("ISystemEnvironmentConfig get Instance failed!");
        //	return;
        //}

        //if(sPrinterConfigFilePath.empty())
        //{
        //	LOG_ERROR_FILMING("GetPrinterConfigFilePath function get null path by using \
        //					  GetApplicationPath('FilmingConfigPath'),please check!!");
        //}

        //sPrinterConfigFilePath += MCSF_FILMING_CONFIG_FILE_NAME;

        //if (false == pFileParser->Validate(sPrinterConfigFilePath))
        //{
        //	LOG_WARN_FILMING("Printer Config File " + sPrinterConfigFilePath + " is not valid") ;
        //	return ;
        //}
        //if (false == pFileParser->ParseByURI(sPrinterConfigFilePath))
        //{
        //	LOG_WARN_FILMING("Printer Config File " + sPrinterConfigFilePath + " has been parsed failed") ;
        //	return ;
        //}
        //string sPrintObjectStoragePath;
        //if (false == pFileParser->GetStringValueByPath("/FeatureSwitch/NewSeriesDescription", &m_sArchivedSeriesDescription) )
        //{
        //	LOG_WARN_FILMING("error when parsing Tag  \"FeatureSwitch/NewSeriesDescription ");
        //	return;
        //}
        //pFileParser->Terminate();

        //delete pSysConfig;
        //pSysConfig = NULL;

        ////end to read config
        ////////////////////////////////////////////////////////////////////////////
        

    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

int McsfFilmingCommandHandler::HandleCommand(const CommandContext* pContext,std::string* pReplyObject)
{
    try
    {
        // TODO:....
        if (nullptr == pContext)
        {
            LOG_ERROR("command parameter pContext is null");
            return -1;
        }

        if (nullptr == pReplyObject)
        {
            LOG_ERROR("command parameter pReplyObject is null");
            return -1;
        }

        FilmingJobStatus jobStatus;

        int iCommandID = 0;
        iCommandID = pContext->iCommandId;
        m_sCurrentCommandSender = pContext->sSender;

        McsfJobManagerInfoWrapper jobInfoWrapper;
        std::vector<McsfJobManagerInfo> jobInfos;
        unsigned long iMaxPriority;
        unsigned long jobCount;

        switch(iCommandID)
        {
            //receive a print job serialize object
        case ADD_PRINT_JOB_COMMAND://6888:
            {
                LOG_INFO("begin to add a job");
                //// step1: insert this job to DB, and return a new jobID.
                //unsigned int uiJobID = AddPrintJobToDB(pContext->sSerializeObject);
                //// step2: add the print job object to ACE_MESSAGE_QUEUE
                //AddPrintJobToQueue(pContext->sSerializeObject, uiJobID, &jobStatus);
                AddPrintJob(pContext->sSerializeObject, &jobStatus);
                LOG_INFO("end to add a job");
                break;
            }
        case QUERY_CURRENT_PRINT_JOBS_COMMAND://6889:
            //TODO.. queue all print job objects in ACE_MESSAGE_QUEUE
            LOG_INFO("begin to query job queue");
            QueryCurrentPrintJobs(pReplyObject);
            LOG_INFO("end to query job queue");
            return 0;
        case ADJUST_PRIORITY_OF_PRINT_JOB_COMMAND://6881:
            //TODO.. adjust the priority of a print job object in ACE_MESSAGE_QUEUE
            LOG_INFO("begin to adjust priority of a job");
            AdjustPriorityOfPrintJob(pContext->sSerializeObject, &jobStatus);
            LOG_INFO("end to adjust priority of a job");
            break;
        case DELETE_PRINT_JOB_COMMAND://6882:
            LOG_INFO("begin to delete a job");
            DeletePrintJob(pContext->sSerializeObject, &jobStatus);
            LOG_INFO("end to delete a job");
            break;
        //case GET_PRINTER_CONFIG_COMMAND://6883:
        //    //TODO.. return the printer setting to Filming FE to show.
        //    LOG_INFO("begin to get printer configure");
        //    ReplyPrinterConfig(pReplyObject);
        //    LOG_INFO("end to get printer configure");
        //    return 0;
        case PAUSE_PRINT_JOB_COMMAND: //6886
            LOG_INFO("begin to suspend a job");
            PausePrintJob(pContext->sSerializeObject, &jobStatus);
            LOG_INFO("end to suspend a job");
            break;
        case RESUME_PRINT_JOB_COMMAND: //6887
            LOG_INFO("begin to resume a suspended job");
            ResumePrintJob(pContext->sSerializeObject, &jobStatus);
            LOG_INFO("end to resume a suspended job");
            break;
        case QUERY_HISTORY_PRINT_JOB_COMMAND: //6884
            LOG_INFO("beigin to get history print job list");
            QueryHistoryPrintJobs(pContext->sSerializeObject,pReplyObject);
            LOG_INFO("end to get history print job list");
            return 0;
            //TO DO ... query history job
        case REPRINT_COMMAND://6885:
            // TO DO... reprint a job
            LOG_INFO("begin to reprint a historic job");
            RePrintJob(pContext->sSerializeObject, &jobStatus);
            LOG_INFO("end to reprint a historic job");
            break;

        case FromMainFrameContinueCmd:  //id
        case FromMainFrameRestartCmd:
            LOG_INFO("begin to resume a suspended job (mainframe command)");
            jobInfoWrapper.Deserialize(pContext->sSerializeObject);
            jobInfoWrapper.GetJobManagerInfos(jobInfos);
            for (auto it=jobInfos.begin(); it!=jobInfos.end(); it++)
            {
                 RestartPrintJob(it->GetJobitemid(), &jobStatus);
            }
            LOG_INFO("end to resume a suspended job");
            break;
        case FromMainFrameDeleteCmd:    //id
            LOG_INFO("begin to delete a job (mainframe command) ");
            jobInfoWrapper.Deserialize(pContext->sSerializeObject);
            jobInfoWrapper.GetJobManagerInfos(jobInfos);
            for (auto it=jobInfos.begin(); it!=jobInfos.end(); it++)
            {
                DeletePrintJob(it->GetJobitemid(), &jobStatus);
            }
            LOG_INFO("end to delete a job");
            break;
        case FromMainFramePauseCmd:     //id
        case FromMainFrameStopCmd:
            LOG_INFO("begin to suspend a job (mainframe command)");
            jobInfoWrapper.Deserialize(pContext->sSerializeObject);
            jobInfoWrapper.GetJobManagerInfos(jobInfos);
            for (auto it=jobInfos.begin(); it!=jobInfos.end(); it++)
            {
                PausePrintJob(it->GetJobitemid(), &jobStatus);
            }
            LOG_INFO("end to suspend a job");
            break;
        case FromMainFrameRefreshCmd:   //later will call RePlyJobInfoToMainFrame
            LOG_INFO("Refresh Job info (mainframe command) ");
            break;
        case FromMainFrameUrgentCmd:    //id
            LOG_INFO("begin to adjust priority of jobs to urgent (mainframe command) ");
            jobInfoWrapper.Deserialize(pContext->sSerializeObject);
            jobInfoWrapper.GetJobManagerInfos(jobInfos);
            iMaxPriority = CalculateMaxPriority();
            jobCount = jobInfos.size();
            iMaxPriority = (iMaxPriority >= jobCount+MAX_PRIORITY) ? iMaxPriority-jobCount : iMaxPriority;
            for (auto it=jobInfos.begin(); it!=jobInfos.end(); it++)
            {
                //AdjustPriorityOfPrintJob(it->GetJobitemid() + " " + boost::lexical_cast<std::string>(URGENT_JOB_PRIORITY), &jobStatus);
                AdjustPriorityOfPrintJob(boost::lexical_cast<unsigned int>(it->GetJobitemid()), iMaxPriority, &jobStatus);
                LOG_INFO("end to adjust priority of a job");
                iMaxPriority++;
            }
            break;
        case SAVE_EFILMS_COMMAND:
            LOG_INFO("begin to Save electronic films");
            SaveEFilms(pContext->sSerializeObject, pReplyObject);
            LOG_INFO("end to Save electronic films");
            return 0;
        case SUSPEND_ALL_PRINT_JOB_COMMAND:
            LOG_INFO("begin to suspend all print jobs");
            m_mutexSuspendAll.lock();
            m_locked = true;
            break;
        case RESUME_ALL_PRINT_JOB_COMMAND:
            LOG_INFO("begin to resume all print jobs");
            m_mutexSuspendAll.unlock();
            m_locked = false;
            break;
        case TRANSFER_TO_PRINTER://15058
            LOG_INFO("begin to Transfer Printer to a job");
            TransferToPrinterOfPrintJob(pContext->sSerializeObject, &jobStatus);
            LOG_INFO("end to Transfer Printer to a job");
            break;
        default:
            ostringstream os; 
            os << "unregistered command id:" << iCommandID;
            LOG_WARN(os.str());
            break;
        }

        *pReplyObject = jobStatus.SerializeAsString();
        
        RePlyJobInfoToMainFrame();

        return 0;
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
    return -1;
}


void McsfFilmingCommandHandler::SetFilmingJobStatus(FilmingJobStatus *pJobStatus,
    JobStatus eJobStatus, int iReturnValue, std::string sReturnMeaning)
{
    if(NULL == pJobStatus)
    {
        LOG_ERROR("SetFilmingJobStatus has null parameter!");
        return ;
    }

    pJobStatus->set_job_status(eJobStatus);
    pJobStatus->set_return_value(iReturnValue);
    pJobStatus->set_return_meaning(sReturnMeaning);
}

void McsfFilmingCommandHandler::AddPrintJob(FilmingPrintJob filmingPrintJob, FilmingJobStatus *pJobStatus )
{
    if(filmingPrintJob.mutable_film_box() == nullptr) 
    {
        LOG_ERROR("Error: The filmbox in filmPrintJob is null!! ");
        return;
    }

    auto studyInstanceUid = filmingPrintJob.film_box(0).study_instance_uid();

    AutoArchiving(filmingPrintJob.series_instance_uid(), studyInstanceUid);

    filmingPrintJob.mutable_job_status()->set_job_status(JobStatus::PENDING);

    std::vector<McsfPrintJobObject> printJobObjectVector;

    InitMcsfPrintJob(filmingPrintJob,&printJobObjectVector);

    if (printJobObjectVector.empty())
    {
        LOG_ERROR("No object to film");
        SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"No object to film");
        return;
    }

    //TODO: need to fix the bug of InsertPrintJobToDB,  which return -1 to indicate error
    unsigned int uiJobID = 0;
    for(std::vector<McsfPrintJobObject>::iterator it=printJobObjectVector.begin(); 
        it !=printJobObjectVector.end(); it++)
    {
        //if uiJobID!=0, means that this job have more than one film box, 
        //and the first has been insert to DB.
        //so other film box print job just use this job id.
        if(uiJobID != 0)
        {
            it->SetJobID(uiJobID);
        }
        uiJobID = m_pFilmingDB->InsertPrintJobToDB(&(*it));
    }

    if(uiJobID<1)
    {
        LOG_ERROR("insert print job to db get an invalid job ID!");
        SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"Invalid Job ID");
        return;
    }

    filmingPrintJob.set_job_id(uiJobID);
    int iFilmBox;
    iFilmBox = printJobObjectVector.size();
    int iCopies = filmingPrintJob.copies();
    filmingPrintJob.set_film_amount(iFilmBox*iCopies);
    filmingPrintJob.set_film_finished_amount(0);

    filmingPrintJob.set_priority(boost::lexical_cast<string>(MIN_PRIORITY));	
    //Enqueue the print job to job list


    {
        boost::mutex::scoped_lock addJobLock(kJobListMutex);

        m_PrintJobMap[uiJobID] = filmingPrintJob;

        int iFailureJobCount = 0;

        //current job list volume is limited
        if (m_PrintJobMap.size() > FILMING_JOB_LIST_VOLUME )
        {
            for (std::map<unsigned int, FilmingPrintJob>::iterator it = m_PrintJobMap.begin(); it != m_PrintJobMap.end();)
            {
                JobStatus jobStatus = it->second.mutable_job_status()->job_status();
                if (jobStatus == JobStatus::DONE || jobStatus==JobStatus::PAUSE)
                {  
                    auto job = it->second;
                    DeleteEFilms(job);

                    //lint -e534  False alarm : there is no return of the below expression to be handled
                    it = m_PrintJobMap.erase(it);   //LOG_INFO("There is some thread mutex access exception by erasing job when the amount of job is large than the volume of the job list")                    

                    if(m_PrintJobMap.size() <= FILMING_JOB_LIST_VOLUME)
                        break;
                    //break;//lint +e534
                } 
                else
                {
                    it++;
                    if(jobStatus == JobStatus::FAILURE) iFailureJobCount++;
                }
            }
        }

        if(iFailureJobCount > FILMING_JOB_LIST_VOLUME)
        {
            MHC::McsfMhcStatusInfoHandler messageHandler(m_pCommProxy);
            (void)messageHandler.ShowStatusWithRemoteUid(MHC::Warning, "UID_Filming_Job_Full");
        }

        //add job to Job Manager
        //int iRet = m_pDicomJobManager->AddJob(uiJobID,filmingPrintJob.print_priority());
        int iRet = m_pDicomJobManager->AddJob(uiJobID,boost::lexical_cast<unsigned long>(filmingPrintJob.priority()));
        if(iRet != 0)
        {
            LOG_ERROR("failed to add a Print job");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,iRet,"failed to add a Print job");
            return ;
        }

        m_pDatabase->Lock(studyInstanceUid, MCSF_FILMING_SEVICE_LOGGER_NAME, Mcsf::enMcsfDBLockLevel::LOCK_STUDY);

    }

    LOG_INFO(string("+++++++++++++Succeeded to add a Print job ") + boost::lexical_cast<string>(uiJobID));
    SetFilmingJobStatus(pJobStatus, JobStatus::PENDING, 0, "Succeeded to add a Print job");

    MHC::McsfMhcStatusInfoHandler messageHandler(m_pCommProxy);
    (void)messageHandler.ShowStatusWithRemoteUid(MHC::Info, "UID_Filming_Job_Created");
}

void McsfFilmingCommandHandler::AddPrintJob(const std::string& sSerializeObject, FilmingJobStatus *pJobStatus )
{
    try
    {
        
        if(NULL == pJobStatus)
        {
            LOG_ERROR("parameter pJobStatus is NULL");
            return;
        }

        if(sSerializeObject.empty())
        {
            LOG_ERROR("receive null print job!");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"NO DATA FROM FE");
            return;
        }
        LOG_INFO("begin  Parse From String to general filmingPrintJob");
        FilmingPrintJob filmingPrintJob;
        if (false == filmingPrintJob.ParseFromString(sSerializeObject))
        {
            LOG_WARN("There may be some optional field of serialized FilmingPrintJob not setted");
        }
        LOG_INFO("end Parse From String to general filmingPrintJob");
        AddPrintJob(filmingPrintJob, pJobStatus);
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}
//lint +e429

//void McsfFilmingCommandHandler::ReplyPrinterConfig(std::string* pReplyObject)
//{
//    try
//    {
//        if(NULL == pReplyObject)
//        {
//            LOG_ERROR("ReplyPrinterConfig has null parameter!");
//            return ;
//        }
//        // define a PrinterConfig class, it will parse the PrinterConfig.xml when
//        // constructor function is called.
//        PrinterConfig printerConfig;
//        FilmingPrinterList filmingPrinterList;
//
//        for(int i=0; i<printerConfig.GetFilmSessionObjectSize(); i++)
//        {
//            FilmingPrinterList_Printer* pFilmingPrintList_Printer = filmingPrinterList.add_printer();
//
//            McsfDcmFilmSessionObject* pDcmFilmSessionObject = printerConfig.GetFilmSessionObject(i);
//
//            pFilmingPrintList_Printer->set_printer_id(pDcmFilmSessionObject->GetPrinterID());
//            pFilmingPrintList_Printer->set_printer_ae(pDcmFilmSessionObject->GetTargetAETitle());
//            pFilmingPrintList_Printer->set_printer_ip(pDcmFilmSessionObject->GetTargetHostname());
//            pFilmingPrintList_Printer->set_printer_port(pDcmFilmSessionObject->GetPrinterPort());
//            pFilmingPrintList_Printer->set_is_disable_new_vrs(pDcmFilmSessionObject->GetDisableNewVRs());
//            pFilmingPrintList_Printer->set_is_implicit_only(pDcmFilmSessionObject->GetImplictOnly());
//            pFilmingPrintList_Printer->set_max_density(pDcmFilmSessionObject->GetMaxDensity());
//            pFilmingPrintList_Printer->set_min_density(pDcmFilmSessionObject->GetMinDensity());
//            pFilmingPrintList_Printer->set_max_pdu(pDcmFilmSessionObject->GetMaxPDU());
//
//            //split layout
//            std::string sLayout = pDcmFilmSessionObject->GetImageDisplayFormat();
//            SplitDisplayFormat(pFilmingPrintList_Printer,sLayout);
//
//            //split border density
//            std::string sBorderDensity = pDcmFilmSessionObject->GetBorderDensity();
//            SplitBorderDensity(pFilmingPrintList_Printer, sBorderDensity);
//
//            //split empty image density:20\BLACK\WHITE
//            std::string sEmptyImageDensity = pDcmFilmSessionObject->GetEmptyImageDensity();
//            SplitEmptyImageDensity(pFilmingPrintList_Printer, sEmptyImageDensity);
//
//            //split film destination:MAGAZINE\PROCESSOR\BIN_1\BIN_2
//            std::string sFilmDestination = pDcmFilmSessionObject->GetFilmDestination();
//            SplitFilmDestination(pFilmingPrintList_Printer, sFilmDestination);
//
//            //split film size id: 8INX10IN\10INX12IN\10INX14IN\11INX14IN\14INX14IN\14INX17IN\24CMX24CM\24CMX30CM
//            std::string sFilmSize = pDcmFilmSessionObject->GetFilmSize();
//            SplitFilmSize(pFilmingPrintList_Printer, sFilmSize);
//
//            //brief split magnification: REPLICATE\BILINEAR\CUBIC\NONE
//            std::string sMagnification = pDcmFilmSessionObject->GetMagnification();
//            SplitMagnificationType(pFilmingPrintList_Printer, sMagnification);
//
//            //split medium type: PAPER\CLEAR FILM\BLUE FILM
//            std::string sMediumType = pDcmFilmSessionObject->GetMediumType();
//            SplitMediumType(pFilmingPrintList_Printer, sMediumType);
//        }
//        *pReplyObject = filmingPrinterList.SerializeAsString();
//    }
//    catch(...)
//    {
//        LOG_ERROR("exception occur in ReplyPrinterConfig!");
//        //pReplyObject = NULL;
//    }
//    
//}
//
//void McsfFilmingCommandHandler::SplitDisplayFormat(FilmingPrinterList_Printer* pPrinter, std::string sArrayString)
//{
//    if(NULL == pPrinter)
//    {
//        LOG_ERROR("SplitDisplayFormat has null parameter!");
//        return ;
//    }
//
//    std::istringstream isInput(sArrayString);
//    std::string sTemp;
//    while(getline(isInput,sTemp,'\\'))
//    {
//        pPrinter->add_layout(sTemp);
//    }
//}
//
///// \brief split border density string:150\20\BLACK\WHITE
//void McsfFilmingCommandHandler::SplitBorderDensity(FilmingPrinterList_Printer* pPrinter, std::string sArrayString)
//{
//    if(NULL == pPrinter)
//    {
//        LOG_ERROR("SplitBorderDensity has null parameter!");
//        return ;
//    }
//
//    std::istringstream isInput(sArrayString);
//    std::string sTemp;
//    while(getline(isInput,sTemp,'\\'))
//    {
//        pPrinter->add_border_density(sTemp);
//    }
//}
//
///// \brief split empty image density:20\BLACK\WHITE
//void McsfFilmingCommandHandler::SplitEmptyImageDensity(FilmingPrinterList_Printer* pPrinter, std::string sArrayString)
//{
//    if(NULL == pPrinter)
//    {
//        LOG_ERROR("SplitEmptyImageDensity has null parameter!");
//        return ;
//    }
//
//    std::istringstream isInput(sArrayString);
//    std::string sTemp;
//    while(getline(isInput,sTemp,'\\'))
//    {
//        pPrinter->add_emptyimagedensity(sTemp);
//    }
//}
//
///// \brief split film destination:MAGAZINE\PROCESSOR\BIN_1\BIN_2
//void McsfFilmingCommandHandler::SplitFilmDestination(FilmingPrinterList_Printer* pPrinter, std::string sArrayString)
//{
//    if(NULL == pPrinter)
//    {
//        LOG_ERROR("SplitFilmDestination has null parameter!");
//        return ;
//    }
//
//    std::istringstream isInput(sArrayString);
//    std::string sTemp;
//    while(getline(isInput,sTemp,'\\'))
//    {
//        pPrinter->add_film_destination(sTemp);
//    }
//}
//
///// \brief split film size id: 8INX10IN\10INX12IN\10INX14IN\11INX14IN\14INX14IN\14INX17IN\24CMX24CM\24CMX30CM
//void McsfFilmingCommandHandler::SplitFilmSize(FilmingPrinterList_Printer* pPrinter, std::string sArrayString)
//{
//    if(NULL == pPrinter)
//    {
//        LOG_ERROR("SplitFilmSize has null parameter!");
//        return ;
//    }
//
//    std::istringstream isInput(sArrayString);
//    std::string sTemp;
//    while(getline(isInput,sTemp,'\\'))
//    {
//        pPrinter->add_film_size(sTemp);
//    }
//}
//
///// \brief split magnification: REPLICATE\BILINEAR\CUBIC\NONE
//void McsfFilmingCommandHandler::SplitMagnificationType(FilmingPrinterList_Printer* pPrinter, std::string sArrayString)
//{
//    if(NULL == pPrinter)
//    {
//        LOG_ERROR("SplitMagnificationType has null parameter!");
//        return ;
//    }
//
//    std::istringstream isInput(sArrayString);
//    std::string sTemp;
//    while(getline(isInput,sTemp,'\\'))
//    {
//        pPrinter->add_magnification_type(sTemp);
//    }
//}
//
///// \brief split medium type: PAPER\CLEAR FILM\BLUE FILM
//void McsfFilmingCommandHandler::SplitMediumType(FilmingPrinterList_Printer* pPrinter, std::string sArrayString)
//{
//    if(NULL == pPrinter)
//    {
//        LOG_ERROR("SplitMediumType has null parameter!");
//        return ;
//    }
//
//    std::istringstream isInput(sArrayString);
//    std::string sTemp;
//    while(getline(isInput,sTemp,'\\'))
//    {
//        pPrinter->add_medium_type(sTemp);
//    }
//}

//void McsfFilmingCommandHandler::StatisticalServe()
//{
//    //todo: 有内存泄露
//    int timeSpan_s = 24*3600;
//    while (true)
//    {
//	    ACE_OS::sleep(timeSpan_s);
//
//        //get time
//        time_t now_time = time(0);
//        char tmp[32];
//        struct tm * lTime = localtime(&now_time);
//        strftime(tmp, sizeof(tmp), "%Y-%m-%d", lTime);
//        string sDate = std::string(tmp);
//        strftime(tmp, sizeof(tmp), "%Y/%m/%d %H:%M:%S", lTime);
//        string sTime = tmp;
//
//        //get film count
//        int filmCount = 0;//todo: m_pFilmingDB->GetFilmCountYesterday();
//        filmCount = m_pFilmingDB->GetFilmCountYesterday();
//        LOG_INFO("Yesterday Film Count");
//        LOG_INFO(filmCount);
//
//        //construct CLogInfo
//        CLogInfo logInfo;
//        logInfo.OperationDateTime = sTime;
//        logInfo.Type = "FLMNUM";
//        
//        //insert item
//        CItem item;
//        CAttribute attribute;
//        attribute.AttrName = "FilmAmount";
//        std::string info = sDate + ":" + boost::lexical_cast<string>(filmCount);
//        attribute.Values.push_back(info);
//        
//
//        //send log to ServiceCenter
//        McsfSendLogToSC sender;
//        if(!sender.SendLogToSC(logInfo, false, CPriority::Normal))
//        {
//            LOG_WARN("[Fail to send Filming log to service center][DateTime]" +sTime + "[Service Info]" + info );
//        }
//
//    }
//}

void McsfFilmingCommandHandler::DoPrint()
{
    try
    {
        int iRet = m_pDicomJobManager->DoJob(DoRealPrint);
        if(iRet != 0)
        {
            LOG_ERROR("Do Print Job failed!");
        }
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

bool McsfFilmingCommandHandler::DoRealPrint(const unsigned int uiJobID )
{
    if(!uiJobID || m_PrintJobMap.find(uiJobID) == m_PrintJobMap.end())
    {
        ostringstream os;
        os << "Can't find the job in job list, the id is " << uiJobID;
        LOG_WARN(os.str());
        return false;
    }

    std::vector<McsfPrintJobObject> printJobObjectVector;
    FilmingPrintJob& filmingPrintJob = m_PrintJobMap[uiJobID];
    //FilmingPrintJob filmingPrintJob ;
    int iFilmBox;
    {
        boost::mutex::scoped_lock doRealPrintLock(kJobListMutex);
        //filmingPrintJob = m_PrintJobMap[uiJobID];
        filmingPrintJob.mutable_job_status()->set_job_status(JobStatus::PRINTING);
        InitMcsfPrintJob(filmingPrintJob,&printJobObjectVector);

        iFilmBox = printJobObjectVector.size();
        if (!iFilmBox)
        {
            //任务中打印的胶片数为0的情况，打印停止。
            LOG_WARN("No object need to film");
            filmingPrintJob.mutable_job_status()->set_job_status(JobStatus::FAILURE);
            RePlyJobInfoToMainFrame();
            return false;
        }

        printJobObjectVector[0].SetJobStatus(JobStatus::PRINTING);
        if(-1 == m_pFilmingDB->UpdatePrintJobStatusInDB(printJobObjectVector[0]))
        {
            LOG_ERROR("There is something exception on updating print job status in db");
        }
        LOG_INFO(string("end to update db before print job ") + boost::lexical_cast<string>(uiJobID) );
    }
    RePlyJobInfoToMainFrame();

    int iCopies = filmingPrintJob.copies();
    filmingPrintJob.set_film_amount(iFilmBox*iCopies);//这行多余代码是为了防止打印失败后数目异常
    McsfFilmingLibaryFactory* pFilming = McsfFilmingLibaryFactory::Instance();

    IFilmingLibary *pIFilmingLibary = NULL;

    McsfPrintJobObject printJobObject;

    int iFilmAmount = 0;
    bool bResult = true;

    try
    {
        for(int i=0;i<iFilmBox;i++)
        {
            printJobObject = printJobObjectVector.at(i);
            pIFilmingLibary	 = pFilming->CreateFilmingLibary(&printJobObject,m_pFilmingDB);

            if (NULL == pIFilmingLibary)
            {
                LOG_ERROR("CreateFilmingLibary failure");
                bResult = false;
                break;
            }
    
            if(-1 == pIFilmingLibary->CreatePrintObject())
            {

                LOG_ERROR("CreatePrintObject failure");

                bResult = false;
                
                break;
            }
           
            if(i==0) 
                {
                    int count = 0;
                    bool isSucceed = false;
                    while(count < m_iRetryConnectPrinterTimes && !isSucceed)
                    {
                        LOG_INFO(string("Try to ConnectPrinter! Times: ") 
                            + boost::lexical_cast<string>(count) );  
                        pIFilmingLibary->DisConnectPrinter();
                        count++;
                        isSucceed = pIFilmingLibary->ConnectPrinter() == 0;    //try to connect printer          
                        LOG_INFO(string("Try to ConnectPrinter isSucceed : ") 
                            + boost::lexical_cast<string>(isSucceed) );  
                    }
                }
    
            //reprint one film by Copies count.
            bool bPrinted = false;
            for(int j=0;j<iCopies;j++)
            {
                m_mutexSuspendAll.lock();
                m_mutexSuspendAll.unlock();
                pIFilmingLibary->SetSetFilmBoxTimeOut(m_iSetFilmBoxTimeOutTime);
                if(!pIFilmingLibary->DoPrint())
                {
                    //TODO: update print status
                    if (!bPrinted)
                    {
                        //update images print status in ImageTable
                        std::vector<std::string> vsImagePathList = printJobObject.GetFileNameList();
                        std::vector<std::string> vsSOPInstanceUIDList = printJobObject.GetOriginSOPInstanceUIDList();
                        auto pathIt = vsImagePathList.begin();
                        auto UIDIt = vsSOPInstanceUIDList.begin();
                        std::string sCondition;
                        std::string sStatement;
                        for (; UIDIt != vsSOPInstanceUIDList.end(); UIDIt++)
                        {//lint -e56  auto
                            sCondition = "SOPInstanceUID='" + *UIDIt + "'";
                            ostringstream os;
                            os << "PrintStatus=" << "'" << 1 << "'";
                            sStatement = os.str();

                            int iDBAccessResult = m_pDatabase->UpdateImageListBySQL(sCondition, sStatement);

                            if( ERROR_DB_NULL != iDBAccessResult)
                            {
                                LOG_WARN("something wrong when update print status of a image object");
                            }
                        }//lint +e56 auto
                     

                        bPrinted = true;
                    }
                    iFilmAmount++;
                    filmingPrintJob.set_film_finished_amount(iFilmAmount);
                    RePlyJobInfoToMainFrame();
                }else
                {
                    ostringstream os;
                    os << "Print Failure: job id ---" << filmingPrintJob.job_id() << ", FileBox index --- " 
                        << i << ", copy index --- " << j;
                    LOG_ERROR(os.str());
                    bResult = false;
                    filmingPrintJob.mutable_job_status()->set_job_status(JobStatus::FAILURE);
                    RePlyJobInfoToMainFrame();

                    break;
                }
            }
            if(i== iFilmBox-1)
            {
                pIFilmingLibary->SetSetFilmBoxTimeOut(10);
                pIFilmingLibary->DisConnectPrinter();
            }
            if(false == bResult)
            {
                break;
            }
        }
    }
    catch (...)
    {
        LOG_ERROR("exception on printing image");
        bResult = false;
    }

    LOG_INFO("Communication Done.Begin Update Info For DataBase.");
    //update job data structure
    {
        boost::mutex::scoped_lock doRealPrintLock(kJobListMutex);
        //Update print info for service
        AppActionInfo appActionInfo;
        appActionInfo.actionDescription = "Photo";
        appActionInfo.actionName = "Print";
        appActionInfo.actionPlayer = m_sFilmingServiceName;
        appActionInfo.appName="UIH_HSW_Filming";
        appActionInfo.Number = iFilmAmount;
        appActionInfo.objectName.push_back("Photo");
        if(ERROR_DB_NULL != m_pDatabase->InsertAppActionInfo(&appActionInfo))
            LOG_WARN("Fail to insert Film Amount to table for service");

        printJobObject.SetFilmAmount(iFilmAmount);
        m_mutexSuspendAll.lock();
        m_mutexSuspendAll.unlock();
        if (bResult)
        {
            printJobObject.SetJobStatus(static_cast<int>(DONE));			
            m_PrintJobMap[uiJobID].mutable_job_status()->set_job_status(JobStatus::DONE);
            MHC::McsfMhcStatusInfoHandler messageHandler(m_pCommProxy);
            (void)messageHandler.ShowStatusWithRemoteUid(MHC::Info, "UID_Filming_Job_Complete");
        } 
        else
        {
            printJobObject.SetJobStatus(static_cast<int>(FAILURE));
            m_PrintJobMap[uiJobID].mutable_job_status()->set_job_status(JobStatus::FAILURE);
            MHC::McsfMhcStatusInfoHandler messageHandler(m_pCommProxy);
            (void)messageHandler.ShowStatusWithRemoteUid(MHC::Warning, "UID_Filming_Job_Fail");
        }

        //get image uid list
        std::vector<std::string> imageInstanceUIDVector;
        for(int i =0; i< filmingPrintJob.film_box_size(); i++)
        {
            //use DB's new interface to update print status.
            for(int j=0;j<filmingPrintJob.film_box(i).image_box().size();j++)
            {
                imageInstanceUIDVector.push_back(filmingPrintJob.film_box(i).image_box(j).origin_sop_instance_uid());
            }
        }

        //updata image printing status
        auto uidIt = imageInstanceUIDVector.begin();
        std::string uid;
        std::string sStatement;
        for (; uidIt != imageInstanceUIDVector.end(); uidIt++)
        {
            uid = "SOPInstanceUID='" + *uidIt + "'";
            ostringstream os;
            os << "Printing=" << "'" << 0 << "'";
            sStatement = os.str();

            int iDBAccessResult = m_pDatabase->UpdateImageListBySQL(uid, sStatement);

            if( ERROR_DB_NULL != iDBAccessResult)
            {
                LOG_WARN("Something Wrong when Update Printing Status of a Image Object For DX");
            }
        }

        //update series and study table's print status
        int iRet = m_pDatabase->UpdateStatusByImageUIDList(imageInstanceUIDVector,STATUS_FILMING);
        if(iRet != ERROR_DB_NULL)
        {
            LOG_ERROR("Update Study and Series Print Status Failed!");
        }

        auto studyInstanceUid = filmingPrintJob.film_box(0).study_instance_uid();
        m_pDatabase->UnLock(studyInstanceUid,MCSF_FILMING_SEVICE_LOGGER_NAME);
        
        //UpdatePrintStatus(filmingPrintJob, bResult);

        LOG_INFO(
            string("begin to update db after print job ") 
            + boost::lexical_cast<string>(uiJobID));
        if(-1 == m_pFilmingDB->UpdatePrintJobStatusInDB(printJobObject) )
        {
            LOG_ERROR("There is something exception on updating print job status in db");
        }
        LOG_INFO(
            string("end to update db after print job ") 
            + boost::lexical_cast<string>(uiJobID));
        //if(iFilmAmount==0) iFilmAmount=filmingPrintJob.film_amount(); //全部打印失败的时候，获取打印总数
        //m_PrintJobMap[uiJobID].set_film_amount(iFilmAmount);
    }

    RePlyJobInfoToMainFrame();
    
    //send film complete to PA
    CommandContext filmingCmdContext;
    //filmingCmdContext.sReceiver = PA_CMD_RECEIVER;
    filmingCmdContext.sReceiver = m_sPAName;
    filmingCmdContext.iCommandId = static_cast<int>(FILMING_COMPLETE_COMMAND);
    filmingCmdContext.pCommandCallback = NULL;

    if(m_pCommProxy->AsyncSendCommand(&filmingCmdContext) )
    {
        LOG_WARN("Exception when sending job status back to PA");
    }
    LOG_INFO("Has sent job status back to PA");


    return bResult;
}

void McsfFilmingCommandHandler::InitMcsfPrintJob(
    const FilmingPrintJob& filmingPrintJob,
    /*out*/std::vector<McsfPrintJobObject>* pMcsfPrintJobVector)
{
    try
    {
        if(NULL == pMcsfPrintJobVector)
        {
            LOG_ERROR("InitMcsfPrintJob has null parameter!");
            return ;
        }

        PrinterConfig aPrinterConfig;
        McsfDcmFilmSessionObject* pFilmSessionObjectVector;


        for(int i =0; i< filmingPrintJob.film_box_size(); i++)
        {
            McsfPrintJobObject printJobObject;
            printJobObject.SetJobID(filmingPrintJob.job_id());
            printJobObject.SetPriority(static_cast<unsigned int> (filmingPrintJob.print_priority() ) );
            JobStatus eJobStatus = filmingPrintJob.job_status().job_status();
            int iJobStatus = static_cast<int>(eJobStatus);
            printJobObject.SetJobStatus(iJobStatus);
            printJobObject.SetCopies(filmingPrintJob.copies());
            printJobObject.SetMyAETitle(filmingPrintJob.our_ae());
            printJobObject.SetTargetAETitle(filmingPrintJob.printer_ae());
            printJobObject.SetTargetHostName(filmingPrintJob.printer_ip());
            printJobObject.SetTargetPort(filmingPrintJob.port());
            printJobObject.SetLayout(filmingPrintJob.film_box(i).layout());
            printJobObject.SetLutFile(filmingPrintJob.film_box(i).lut_file_path());
            printJobObject.SetFilmSize(filmingPrintJob.film_box(i).film_size());
            printJobObject.SetOrientation(filmingPrintJob.film_box(i).orientation());
            //set media type
            printJobObject.SetMediumType(filmingPrintJob.media_type());
            //set film destination
            printJobObject.SetDestination(filmingPrintJob.film_destination());
            //set filmAmount
            printJobObject.SetFilmAmount(filmingPrintJob.film_amount());
            //set filmifColorPrint
            printJobObject.SetColorPrint(filmingPrintJob.if_color_print());

            pFilmSessionObjectVector = aPrinterConfig.GetFilmSessionObject(filmingPrintJob.printer_ae());
            if(pFilmSessionObjectVector!=NULL){
                printJobObject.SetTargetPLUTinFilmSession( pFilmSessionObjectVector->GetPresentationLUTinFilmSession());
            }

            //printJobObject.SetPreferSCPLUTRendering(true);
            //printJobObject.SetLutShape(2);

            std::vector<std::string> imagePathVector;
            std::vector<std::string> originSOPInstanceUIDVector;
            for(int j = 0; j<filmingPrintJob.film_box(i).image_box_size(); j++)
            {
                string temp = filmingPrintJob.film_box(i).image_box(j).image_path();
                if(temp != "")
                    imagePathVector.push_back(temp);
                originSOPInstanceUIDVector.push_back(filmingPrintJob.film_box(i).image_box(j).origin_sop_instance_uid());
            }
            if(!printJobObject.SetFileNameList(imagePathVector))
            {
                LOG_WARN("warning on printJobObject.SetFileNameList");
            }
            printJobObject.SetOriginSOPInstanceUIDList(originSOPInstanceUIDVector);
            pMcsfPrintJobVector->push_back(printJobObject);
        }
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingCommandHandler::ExportMcsfPrintJob( 
    const std::vector<McsfPrintJobObject>& mcsfPrintJobVector, 
    FilmingPrintJob* pFilmingPrintJob )
{
    try
    {
        if(NULL == pFilmingPrintJob)
        {
            LOG_ERROR("ExportMcsfPrintJob has null parameter!");
            return ;
        }

        if (mcsfPrintJobVector.empty())
        {
            LOG_WARN("There is no McsfPrintJobObject to export");
            return;
        }
    
        McsfPrintJobObject printJobObject = *(mcsfPrintJobVector.begin());
        pFilmingPrintJob->set_job_id(printJobObject.GetJobID());
        pFilmingPrintJob->set_print_priority(static_cast<FilmingPrintJob_PrintPriority>(printJobObject.GetPriority()) );
        //pFilmingPrintJob->mutable_job_status()->set_job_status( static_cast<JobStatus> (printJobObject.GetJobStatus() ) );
        pFilmingPrintJob->set_copies(printJobObject.GetCopies());
        pFilmingPrintJob->set_our_ae(printJobObject.GetMyAETitle());
        pFilmingPrintJob->set_printer_ae(printJobObject.GetTargetAETitle());
        pFilmingPrintJob->set_printer_ip(printJobObject.GetTargetHostname());
        pFilmingPrintJob->set_port(printJobObject.GetTargetPort());
        //set media type
        pFilmingPrintJob->set_media_type(printJobObject.GetMediumType());
        //set film destination
        pFilmingPrintJob->set_film_destination(printJobObject.GetDestination());
        //set filmAmount 
        pFilmingPrintJob->set_film_amount(printJobObject.GetFilmAmount());
        //set filmifColorPrint
        pFilmingPrintJob->set_if_color_print(printJobObject.GetIfColorPrint());
        for (std::vector<McsfPrintJobObject>::const_iterator it = mcsfPrintJobVector.begin(); it!=mcsfPrintJobVector.end(); it++)
        {
            FilmingPrintJob_FilmBox* pFilmBox = pFilmingPrintJob->add_film_box();
            pFilmBox->set_layout(it->GetLayout());
            pFilmBox->set_film_size(it->GetFilmSize());
            pFilmBox->set_lut_file_path(it->GetLutFile());

           std::vector<std::string> vsImagePath = it->GetFileNameList();
           std::vector<std::string> vsSOPInstanceUID = it->GetOriginSOPInstanceUIDList();
           auto pathIt = vsImagePath.begin();
           auto UIDIt = vsImagePath.begin();
           size_t ivsSOPInstanceUID = vsSOPInstanceUID.size();
           for (; pathIt != vsImagePath.end(); pathIt++)
           {
               FilmingPrintJob_ImageBox* pImageBox = pFilmBox->add_image_box();
               pImageBox->set_image_path(*pathIt);
               if (!ivsSOPInstanceUID)
               {
                    pImageBox->set_origin_sop_instance_uid("");
               } 
               else
               {
                   pImageBox->set_origin_sop_instance_uid(*UIDIt);  
                   UIDIt++;
               }
                
           }
        }
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}


void McsfFilmingCommandHandler::OpenPrintThread()
{
    try
    {
        if(-1 == ACE_Thread_Manager::instance()->spawn(
            ACE_THR_FUNC(DoPrint),
            0,
            THR_NEW_LWP | THR_JOINABLE, 
            &m_iThreadId
            ) )
        {
            LOG_ERROR("Can't Start Print Thread");
            return;
        }
         
        //
        //if(-1 == ACE_Thread_Manager::instance()->spawn(
        //    ACE_THR_FUNC(StatisticalServe),
        //    0,
        //    THR_NEW_LWP | THR_JOINABLE, 
        //    &m_iThreadId
        //    ) )
        //{
        //    LOG_ERROR("Can't Start Statistical Server Thread");
        //    return;
        //}

        //InitialJobsFromDB();
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingCommandHandler::ClosePrintThread()
{
    try
    {
        if(-1 == ACE_Thread_Manager::instance()->cancel(m_iThreadId) )
        {
            LOG_ERROR("Can't Stop Print Thread");
        }
        //if(-1 == ACE_Thread_Manager::instance()->cancel(m_iServerThreadId) )
        //{
        //    LOG_ERROR("Can't Stop Statistical Server Thread");
        //}
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingCommandHandler::QueryCurrentPrintJobs( std::string* pReplyObject )
{
    try
    {
        if(NULL == pReplyObject)
        {
            LOG_ERROR("QueryCurrentPrintJobs has null parameter!");
            return ;
        }

        boost::mutex::scoped_lock queryCurrentPrintJobsLock(kJobListMutex);
    
        FilmingPrintJobQueue printJobQueue;
        FilmingPrintJob* pPrintJob = NULL;
        for (std::map<unsigned int, FilmingPrintJob>::const_iterator it = m_PrintJobMap.begin(); it!=m_PrintJobMap.end(); it++)
        {
            pPrintJob = printJobQueue.add_job();
            pPrintJob->CopyFrom(it->second);
        }
        *pReplyObject = printJobQueue.SerializeAsString();
    }
    catch (exception &e)
    {//lint -e665
        LOG_ERROR(
            string("Exception: ")  + e.what() );
    }//lint +e665
    catch (...)
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingCommandHandler::QueryHistoryPrintJobs(const std::string& sSerializeObject,
    std::string* pReplyObject)
{
    try
    {
        if(NULL == pReplyObject)
        {
            LOG_ERROR("QueryHistoryPrintJobs has null parameter!");
            return ;
        }

        FilmingHistoryJobQueryCondition historyJobQuery;
        FilmingJobStatus jobStatus;
        if(true != historyJobQuery.ParseFromString(sSerializeObject))
        {
            LOG_ERROR("invalid history job list query condition!");
            SetFilmingJobStatus(&jobStatus,JobStatus::FAILURE,-1,"invalid history job list query condition!");
            return;
        }
    
        std::string sStudyRange = historyJobQuery.study_range();
        std::string sPatientID = historyJobQuery.patient_id();
        std::string sAccessionNo = historyJobQuery.accession_no();
        //std::string sModalityType = historyJobQuery.modaltiy_type();
        unsigned int iJobStatus = static_cast<unsigned int> ( historyJobQuery.job_status() );
    
        // as a root sql condition for using add in the sub-condition string head.
        std::string sSqlCondition("1=1");
        if(!sStudyRange.empty())
        {
            //TO DO...
            std::istringstream isInput(sStudyRange);
            std::string sTemp;
            std::vector<std::string> vectorStudyDate;
            while(getline(isInput,sTemp,'-'))
            {
                vectorStudyDate.push_back(sTemp);
            }
    
            if(vectorStudyDate.size() == 1)
            {
                sSqlCondition += " and FilmingDate<="+vectorStudyDate[0];
            }
            else if(vectorStudyDate.size() == 2)
            {
                sSqlCondition += " and FilmingDate>="+vectorStudyDate[0]+ " and FilmingDate<="+vectorStudyDate[1];
            }
            else
            {
                LOG_ERROR("wrong study range condition!");
            }
        }
    
        if(!sPatientID.empty())
        {
            sSqlCondition += " and RefPatientID = "+sPatientID;
        }
    
        if(!sAccessionNo.empty())
        {
            sSqlCondition += " and RefAccessionNo = "+sAccessionNo;
        }
    
        //if job status is not 'UNKNOWN', then add this condition.
        if( iJobStatus != static_cast<unsigned int>(UNKNOWN))
        {
            sSqlCondition += " and JobStatus = "+boost::lexical_cast<std::string>(iJobStatus);
        }
        //lint -e665
        LOG_INFO(string("query history print condition sql is :" ) + sSqlCondition);
        //lint +e665
        std::vector<McsfPrintJobObject> printJobObjectVector;
        int iRet = this->m_pFilmingDB->GetHistoryPrintJob(sSqlCondition,&printJobObjectVector);
    
        if(iRet > 0)
        {
            FilmingPrintJobQueue printJobQueue;
            FilmingPrintJob* pPrintJob = NULL;
    
            for(int i=0; i<iRet; i++)
            {
                pPrintJob = printJobQueue.add_job();
    
                //in get history print job list interface, just return the PrintJob level information
                //if you want get the film sheet level information, you need call another interface.
                pPrintJob->set_job_id(printJobObjectVector[i].GetJobID());
                McsfCommonSave::JobStatus jobStatus = static_cast<McsfCommonSave::JobStatus>(printJobObjectVector[i].GetJobStatus());
                pPrintJob->mutable_job_status()->set_job_status(jobStatus);
                pPrintJob->set_our_ae(printJobObjectVector[i].GetMyAETitle());
                pPrintJob->set_printer_ae(printJobObjectVector[i].GetTargetAETitle());
                pPrintJob->set_printer_ip(printJobObjectVector[i].GetTargetHostname());
                pPrintJob->set_port(printJobObjectVector[i].GetTargetPort());
                pPrintJob->set_copies(printJobObjectVector[i].GetCopies());
            }
    
            *pReplyObject = printJobQueue.SerializeAsString();
            return;
        }
        else if(iRet == 0)
        {
            LOG_WARN("there is no history print job!");
            return;
        }
        else
        {
            LOG_ERROR("error occur!");
            return;
        }
    }

    catch (std::exception& e)
    {//lint -e665
        LOG_ERROR(
            string("Exception: " ) + e.what());
    }//lint +e665

    catch (...)
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingCommandHandler::AdjustPriorityOfPrintJob( const std::string& sSerializedJobInfo,
    FilmingJobStatus *pJobStatus )
{
    try
    {
        if(sSerializedJobInfo.empty())
        {
            LOG_ERROR("receive null print job!");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"NO DATA FROM FE");
            return;
        }

        LOG_INFO(string("Command parameter from FE is :") + sSerializedJobInfo);
        istringstream is(sSerializedJobInfo);
        unsigned int uiJobID;
        unsigned long ulNewPriority;
        is >> uiJobID >> ulNewPriority;
    
        //if( false == FilmingPrintJob::PrintPriority_IsValid(ulNewPriority) )
        //{
        //    LOG_ERROR(string("invalid job priority, the job id and priority is ") + sSerializedJobInfo);

        //    SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,
        //        "invalid job priority, the job id and priority is "+sSerializedJobInfo);
        //    return;
        //}

        AdjustPriorityOfPrintJob(uiJobID, ulNewPriority, pJobStatus );
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingCommandHandler::DeletePrintJob( const std::string& sSerializedJobID, FilmingJobStatus *pJobStatus )
{
    try
    {
        if(NULL == pJobStatus)
        {
            LOG_ERROR("DeletePrintJob has null parameter!");
            return ;
        }

        if(sSerializedJobID.empty())
        {
            LOG_ERROR("receive null print job!");

            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"NO DATA FROM FE");
            return;
        }
    
        //TODO: deserializing sSerializedJobID to get id of the job to be deleted
        LOG_INFO(string("Command parameter from FE is :") + sSerializedJobID);

        unsigned int uiJobID = boost::lexical_cast<unsigned int>(sSerializedJobID);

        {
            boost::mutex::scoped_lock deleteLock(kJobListMutex);
            if(m_PrintJobMap.find(uiJobID) == m_PrintJobMap.end()
                || m_PrintJobMap[uiJobID].job_status().job_status() == JobStatus::PRINTING)
            {
                MHC::McsfMhcStatusInfoHandler messageHandler(m_pCommProxy);
                (void)messageHandler.ShowStatusWithRemoteUid(MHC::Warning, "UID_Filming_Job_Cannot_Be_Deleted");
                return;
            }
        }

        auto filmingPrintJob = m_PrintJobMap[uiJobID];
        //get image uid list
        std::vector<std::string> imageInstanceUIDVector;
        for(int i =0; i< filmingPrintJob.film_box_size(); i++)
        {
            //use DB's new interface to update print status.
            for(int j=0;j<filmingPrintJob.film_box(i).image_box().size();j++)
            {
                imageInstanceUIDVector.push_back(filmingPrintJob.film_box(i).image_box(j).origin_sop_instance_uid());
            }
        }

        //updata image printing status
        auto uidIt = imageInstanceUIDVector.begin();
        std::string uid;
        std::string sStatement;
        for (; uidIt != imageInstanceUIDVector.end(); uidIt++)
        {
            uid = "SOPInstanceUID='" + *uidIt + "'";
            ostringstream os;
            os << "Printing=" << "'" << 0 << "'";
            sStatement = os.str();

            int iDBAccessResult = m_pDatabase->UpdateImageListBySQL(uid, sStatement);

            if( ERROR_DB_NULL != iDBAccessResult)
            {
                LOG_WARN("Something Wrong when Update Printing Status of a Image Object For DX");
            }
        }
        //updata study print status
        LOG_INFO(string("Begin update status when delete Job."));
        m_pDatabase->UpdateStatusByImageUIDList(imageInstanceUIDVector,STATUS_FILMING);
        //清掉数据库锁        
        LOG_INFO(string("Begin clear DB locks when delete Job."));
        auto studyInstanceUid = filmingPrintJob.film_box(0).study_instance_uid();
        m_pDatabase->UnLock(studyInstanceUid,MCSF_FILMING_SEVICE_LOGGER_NAME);

        AdjustPriorityOfPrintJob(uiJobID, 0, pJobStatus);


        if (!pJobStatus->return_value())
        {
            //check if the job is an reprinting job
            std::string sSqlCondition = "1=1";
            sSqlCondition += " and  JobStatus = " + boost::lexical_cast<std::string>(JobStatus::PENDING);
            sSqlCondition += " and  JobID = " + boost::lexical_cast<std::string>(uiJobID);
            std::vector<McsfPrintJobObject> printJobObjectVector;
            int iRet = this->m_pFilmingDB->GetHistoryPrintJob(sSqlCondition,&printJobObjectVector);

            //delete the DB record
            if(iRet>0)
            {
                if(-1 == m_pFilmingDB->DeleteJobByID(uiJobID) ) LOG_WARN("There is some exception when delete job from db");
                //delete dicom file generated by screen copying
                FilmingPrintJob job = m_PrintJobMap[uiJobID];
                DeleteEFilms(job);

            }
        }
        m_PrintJobMap.erase(uiJobID);
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch (...)
    {
        LOG_ERROR("General exception when deleting a job");
    }
}

void McsfFilmingCommandHandler::AdjustPriorityOfPrintJob( unsigned int uiJobID, unsigned long ulNewPriority, FilmingJobStatus *pJobStatus )
{
    try
    {
        if (0 == uiJobID)
        {
            LOG_INFO("Parameter error, jobid = 0 is perserved by backend");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"Parameter error, jobid = 0 is perserved by backend");
            return;
        }
    
        boost::mutex::scoped_lock adjustPriorityLock(kJobListMutex);
        if (m_PrintJobMap.find(uiJobID) == m_PrintJobMap.end() )
        {
            LOG_INFO("Has not found the job, I can't tell whether the job is done or  exception");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,
                "Has not found the job, I can't tell whether the job is done or  exception");
            return;
        } 

        //lint -e665
        LOG_INFO(
            string("Has found the job") + boost::lexical_cast<string>(uiJobID));
        //lint +e665
        if (!ulNewPriority)
        {
            if(!m_PrintJobMap.erase(uiJobID))
            {
                LOG_WARN("exception when erase a job from job list");
            }
        } 
        else
        {
            //LOG_WARN("adjust job, id = " << uiJobID << " priority = " << ulNewPriority);
            //m_PrintJobMap[uiJobID].set_print_priority( static_cast<FilmingPrintJob_PrintPriority>(ulNewPriority) );
            m_PrintJobMap[uiJobID].set_priority(boost::lexical_cast<string>(ulNewPriority));
        }

        int iRet = m_pDicomJobManager->ChangePriority(uiJobID,ulNewPriority);
        if(iRet != 0)
        {
            LOG_ERROR(
                string("Error when adjust priority of a job, job id is ")  
                + boost::lexical_cast<string>(uiJobID) 
                + " New Priority is "
                + boost::lexical_cast<string>(ulNewPriority));
            //lint +e665
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"Error when adjust priority");
            return;
        }

        LOG_INFO( 
            string("The priority has been changed of job:") 
            + boost::lexical_cast<string>(uiJobID) 
            + "with new Priority:" 
            + boost::lexical_cast<string>(ulNewPriority));
        SetFilmingJobStatus(pJobStatus,JobStatus::DONE,0,"Command Done");
    }
    catch (...)
    {   //lint -e665
        LOG_ERROR(
            string("Exception when adjust priority of a job, job id is ")  
            + boost::lexical_cast<string>(uiJobID)
            + " New Priority is "
            + boost::lexical_cast<string>(ulNewPriority));
        //lint +e665
        SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"Exception when adjust priority");
    }
}

void McsfFilmingCommandHandler::PausePrintJob( const std::string& sSerializedJobID, FilmingJobStatus *pJobStatus )
{
    try
    {
        if(sSerializedJobID.empty())
        {
            LOG_ERROR("receive null print job!");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"NO DATA FROM FE");
            return;
        }

        LOG_INFO(string("Command parameter from FE is :") + sSerializedJobID);
        unsigned int uiJobID = boost::lexical_cast<unsigned int>(sSerializedJobID);

        if (0 == uiJobID)
        {
            LOG_INFO("Parameter error, jobid = 0 is perserved by backend");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,
                "Parameter error, jobid = 0 is perserved by backend");
            return;
        }

        boost::mutex::scoped_lock suspendedJobListLock(kJobListMutex);

        int iRet = m_pDicomJobManager->PauseJob(uiJobID);
        
        if(iRet == 0)
        {
            m_PrintJobMap[uiJobID].mutable_job_status()->set_job_status(JobStatus::PAUSE);

            LOG_INFO(
                string("Suspended a job, id is : ")
                + boost::lexical_cast<string>(uiJobID));
            SetFilmingJobStatus(pJobStatus,JobStatus::PAUSE,0,"Suspended a job, id is : " +sSerializedJobID);
        }
        else
        {
            LOG_INFO("Has not found the job, I can't tell whether the job is done or  exception");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,
                "Has not found the job, I can't tell whether the job is done or  exception");
        }
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingCommandHandler::ResumePrintJob( const std::string& sSerializedJobID, FilmingJobStatus *pJobStatus )
{
    try
    {
        if(sSerializedJobID.empty())
        {
            LOG_ERROR("receive null print job!");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"NO DATA FROM FE");
            return;
        }

        LOG_INFO(string("Command parameter from FE is :") + sSerializedJobID);
        unsigned int uiJobID = boost::lexical_cast<unsigned int>(sSerializedJobID);

        if (0 == uiJobID)
        {
            LOG_INFO("Parameter error, jobid = 0 is perserved by backend");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,
                "Parameter error, jobid = 0 is perserved by backend");
            return;
        }

        //add lock for suspended job list
        boost::mutex::scoped_lock suspendedJobListLock(kJobListMutex);
        if (!m_PrintJobMap.count(uiJobID) || m_PrintJobMap[uiJobID].mutable_job_status()->job_status() == JobStatus::PRINTING)
        {
            LOG_INFO("Has not found the job in suspended job list");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,
                "Has not found the job in suspended job list");
            return;
        }

        //add at 2012/11/3, when there is no e-film saved, and the job need to be resumed is not pending or paused, send info to mainframe
        JobStatus status = m_PrintJobMap[uiJobID].mutable_job_status()->job_status();
        //add at 2013/10/15,the job need not to be e-film saved
        //if (!m_PrintJobMap[uiJobID].if_save_efilm() && status != JobStatus::PENDING && status != JobStatus::PAUSE )
        if (status != JobStatus::PENDING && status != JobStatus::PAUSE )
        {
            //send info to mainframe
            MHC::McsfMhcStatusInfoHandler messageHandler(m_pCommProxy);
            (void)messageHandler.ShowStatusWithRemoteUid(MHC::Warning, "UID_Filming_Job_Cannot_RePrint");
            return;
        }

        //add the job back to the job queue
        //Enqueue the print job message block onto the message queue
        //create a new message block specifying exactly how large
        //an underlying data block should be created.
        m_PrintJobMap[uiJobID].set_film_finished_amount(0);
        int iRet = m_pDicomJobManager->ResumeJob(uiJobID,m_PrintJobMap[uiJobID].print_priority());

        if(iRet != 0)
        {
            LOG_ERROR("\nCould not enqueue on to mq!!\n");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,
                "Can't resume the print Job for some exception");
            return;
        }

        //remove the job from the suspended job list
        m_PrintJobMap[uiJobID].mutable_job_status()->set_job_status(JobStatus::PENDING);

        LOG_INFO("Succeeded to resume a Print job");
        SetFilmingJobStatus(pJobStatus,JobStatus::PENDING,0,"Succeeded to resume a Print job");
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}
//lint +e429

void McsfFilmingCommandHandler::RePrintJob( const std::string& sSerializedJobID, FilmingJobStatus *pJobStatus )
{
    try
    {
        boost::mutex::scoped_lock kRePrintLock(kJobListMutex);

        if(sSerializedJobID.empty())
        {
            LOG_ERROR("receive null print job!");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"NO DATA FROM FE");
            return;
        }

        LOG_INFO(string("Command parameter from FE is :") + sSerializedJobID);
        unsigned int uiJobID = boost::lexical_cast<unsigned int>(sSerializedJobID);

        if (0 == uiJobID)
        {
            LOG_INFO("Parameter error, jobid = 0 is perserved by backend");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,
                "Parameter error, jobid = 0 is perserved by backend");
            return;
        }

        if (m_PrintJobMap.find(uiJobID) != m_PrintJobMap.end())
        {
            JobStatus jobStatus = m_PrintJobMap[uiJobID].mutable_job_status()->job_status();
            if (jobStatus == JobStatus::PENDING || jobStatus == JobStatus::PRINTING)
            {
                LOG_WARN("The print job is being printed or already in the print queue wait to be printed");
                SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,
                    "The print job is being printed or already in the print queue wait to be printed");
                return;
            }
        }

        //TODO: QUERY DB to get job information
        //string sSerializedJobInfo;
        std::vector<McsfPrintJobObject> filmingJobVector;
        //add 2013/10/15,if get job  from DB,then job info (such as SetPatientname)will be null
        /* FilmingPrintJob filmingPrintJob = m_PrintJobMap[uiJobID];
       McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
        if (-1 == pFilmingDB->GetPrintFilmJobByID(uiJobID, &filmingJobVector))
        {    
            LOG_WARN(
                string("Can't find that jobid to reprint, id is : ") 
                + boost::lexical_cast<string>(uiJobID));
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,
                "Can't find that jobid to reprint, id is : "+sSerializedJobID);
            return;
        }
        ExportMcsfPrintJob(filmingJobVector, &filmingPrintJob);*/

        FilmingPrintJob filmingPrintJob = m_PrintJobMap[uiJobID];
        filmingPrintJob.set_film_finished_amount(0);
        filmingPrintJob.mutable_job_status()->set_job_status(JobStatus::PENDING);

        //create a new message block specifying exactly how large
        //an underlying data block should be created.

        int iRet = m_pDicomJobManager->AddJob(uiJobID,filmingPrintJob.print_priority());

        if(iRet != 0)
        {
            LOG_ERROR(
                string("RePrint a job failed! ID:") 
                + boost::lexical_cast<string>(uiJobID));
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"RePrint a job failed! ID:"+sSerializedJobID);
            return; 
        }

        //Enqueue the print job to job list
        m_PrintJobMap[uiJobID] = filmingPrintJob;

        auto studyInstanceUid = filmingPrintJob.film_box(0).study_instance_uid();
        m_pDatabase->Lock(studyInstanceUid, MCSF_FILMING_SEVICE_LOGGER_NAME, Mcsf::enMcsfDBLockLevel::LOCK_STUDY);

        LOG_INFO("+++++++++++++Succeeded to add a Print job");
        SetFilmingJobStatus(pJobStatus,JobStatus::PENDING,0,"Succeeded to add a Print job");
        
        return ;    //pTempMB will be freed
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}
//lint +e429

void McsfFilmingCommandHandler::RestartPrintJob( const std::string& sSerializedJobID, FilmingJobStatus *pJobStatus )
{
    try
    {
//        boost::mutex::scoped_lock kReStartLock(kJobListMutex);

        const unsigned int uiJobID = boost::lexical_cast<unsigned int>(sSerializedJobID);
        if (m_PrintJobMap.find(uiJobID) != m_PrintJobMap.end() && m_PrintJobMap[uiJobID].mutable_job_status()->job_status()== JobStatus::PAUSE)
        {
            ResumePrintJob(sSerializedJobID, pJobStatus);
        }
        else
        {
            RePrintJob(sSerializedJobID, pJobStatus);
        }
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

//a temporary solution
void McsfFilmingCommandHandler::InitialJobsFromDB()
{
    try
    {
        //get ids of pending jobs from db
        //GetPending jobs from DB
        string  sSqlCondition = "JobStatus = "+boost::lexical_cast<std::string>(JobStatus::PENDING);
        sSqlCondition += " or JobStatus = ";
        sSqlCondition += boost::lexical_cast<std::string>(JobStatus::PRINTING);
        //lint -e665
        LOG_INFO(string("query history print condition sql is :") + sSqlCondition);
        //lint +e665
        std::vector<McsfPrintJobObject> printJobObjectVector;
        int iRet = this->m_pFilmingDB->GetHistoryPrintJob(sSqlCondition,&printJobObjectVector);
        if (iRet < 0)
        {
            LOG_ERROR("error occur!");
            return;
        } 
        else if (0 == iRet)
        {
            LOG_INFO("No pending jobs in DB");
            return;
        }
        //reprint these jobs
        else//iRet > 0
        {
            FilmingJobStatus jobStatus;
            for (std::vector<McsfPrintJobObject>::const_iterator it = printJobObjectVector.begin(); it!=printJobObjectVector.end(); it++ )
            {
                RePrintJob(boost::lexical_cast<std::string>(it->GetJobID()), &jobStatus);
            }
        }
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

int McsfFilmingCommandHandler::SaveEFilms( const std::string& sSerializedPara, std::string *pReplyObject )
{
    try
    {
        LOG_INFO(sSerializedPara);

        std::vector<std::string> imagePathVector;

        std::istringstream isInput(sSerializedPara);
        std:: string sTemp;
        while (std::getline(isInput, sTemp, '#'))
        {
            imagePathVector.push_back(sTemp);
        }

        return SaveEFilms(imagePathVector, pReplyObject);

    }
    catch(std::exception& e)
    {
        LOG_ERROR(e.what());
        //AskFilmingFEToUpdatePagesInfo();
        return -1;
    }
    catch (...)
    {
        LOG_ERROR("general exception");
        return -1;
    }
}

int McsfFilmingCommandHandler::SaveEFilms( const FilmingPrintJob& job, std::string *pReplyObject)
{
    std::vector<string> imagePathVector;
    std::set<string> patientNameSet;
    std::set<string> studyInstanceUIDSet;
    
    for (int i=0; i<job.film_box_size(); i++)
    {
        const FilmingPrintJob_FilmBox& filmBox = job.film_box(i);
        string patientName = filmBox.patient_name();
        string studyInstanceUID = filmBox.study_instance_uid();
        if (patientName != MIXED_FILM_PATIENT_NAME && patientName != "")
        {
            patientNameSet.insert(patientName);
            studyInstanceUIDSet.insert(studyInstanceUID);
            if(filmBox.has_efilm_path())
            {
                string efilmPath = filmBox.efilm_path();
                if(efilmPath != EMPTY_STRING)
                    imagePathVector.push_back(efilmPath);
            }
            
            /*imagePathVector.push_back(filmBox.)*/

            //for (int j=0; j<filmBox.image_box_size(); j++)
            //{
            //	auto imageBox = filmBox.image_box(j);
            //	if (imageBox.has_image_path())
            //	{
            //		imagePathVector.push_back(filmBox.image_box(j).image_path());
            //	}				
            //}
        }

    }

    //todo: insert study instance uids;
    string studyInstanceUIDString = "";
    for (auto it = studyInstanceUIDSet.begin(); it!= studyInstanceUIDSet.end(); it++)
    {
        studyInstanceUIDString += *it;
        studyInstanceUIDString += ";";
    }
    //remove last ";"
    if(studyInstanceUIDString.length() > 1)	studyInstanceUIDString.erase(studyInstanceUIDString.end()-1);
    imagePathVector.insert(imagePathVector.begin(), studyInstanceUIDString);


    string patientNameString = "";
    for (auto it = patientNameSet.begin(); it!= patientNameSet.end(); it++)
    {
        patientNameString += *it;
        patientNameString += ";";
    }
    //remove last ";"
    if(patientNameString.length() > 1)	patientNameString.erase(patientNameString.end()-1);
    imagePathVector.insert(imagePathVector.begin(), patientNameString);

    return SaveEFilms(imagePathVector, pReplyObject);
}

int McsfFilmingCommandHandler::SaveEFilms(const std::vector<string>& imagePathVector, std::string *pReplyObject )
{
    if(NULL == pReplyObject)
    {
        LOG_ERROR("SaveEFilms has null parameter!");
        return -1;
    }

    *pReplyObject = EMPTY_STRING;
    std::string sPatientName;

    if (imagePathVector.size() <= 2)	//imagePathVector[0] is patientName, imagePathVector[1] is StudyInstanceUID for DB notification
    {
        LOG_WARN("unqualified parameter");
        return -1;
    }

    if(!DBImporterInit())
    {
        LOG_ERROR("DB importer now is not usable");
        *pReplyObject = "DB importer now is not usable";
    }
    else
    {
        std::string sPath("");
        
        sPatientName = imagePathVector[0];

        int i = 2;
        for ( ; i<imagePathVector.size(); i++)
        {
            try
            {
                sPath = imagePathVector[i];
    
    //#ifdef _DEBUG
    //            std::string sSOPInstanceUID("");
    //            if(!ImportDicomFileToDBWithUID(sPath,sSOPInstanceUID))
    //            {
    //                LOG_ERROR("import file " + sPath + " failure");
    //                *pReplyObject = "import file " + sPath + " failure";
    //                break;
    //            }
    //            else
    //            {
    //                LOG_INFO("import file " + sPath + " success, SOPInstanceUID is " + sSOPInstanceUID);
    //            }
    //#else
                if(!ImportDicomFileToDB(sPath, m_pDBNotifier))
                {
                    LOG_ERROR("import file " + sPath + " failure");
                    *pReplyObject = "import file " + sPath + " failure";
                    break;
                }
    //#endif
            }
            catch (std::exception& e)
            {
                *pReplyObject = e.what();
                *pReplyObject += " When Saving E-films";
                *pReplyObject +=  sPath;
                LOG_ERROR(*pReplyObject);
                //AskFilmingFEToUpdatePagesInfo();
            }
            catch (...)
            {//lint -e665
                *pReplyObject = "general exception";
                *pReplyObject += " When Saving E-films";
                *pReplyObject +=  sPath;
                LOG_ERROR(*pReplyObject);
                //lint +e665
                //AskFilmingFEToUpdatePagesInfo();
            }
        }

        //send DB update event
        if(i == imagePathVector.size()) 
            NotifyStudyImported(imagePathVector[1]);
    }
    //AskFilmingFEToUpdatePagesInfo();

    //Tell filmingcard FE that efilms saved completely
    CommandContext filmingCmdContext;
    filmingCmdContext.sReceiver = m_sCurrentCommandSender;
    filmingCmdContext.iCommandId = static_cast<int>(SAVE_EFILM_COMPLETE_COMMAND);
    filmingCmdContext.sSerializeObject = sPatientName;
    if (*pReplyObject != EMPTY_STRING)
    {
        filmingCmdContext.sSerializeObject +=  "#" +  *pReplyObject;
    }
    if(-1 == m_pCommProxy->AsyncSendCommand(&filmingCmdContext))
    {
        LOG_ERROR("Fail to Send command to FilmingCard FE");
    }

    return 0;
}

    //todo: add status bar notification, if notify study import event failed 
    void  McsfFilmingCommandHandler::NotifyStudyImported(string sStudyInstanceUIDs)
    {

        IEventMessageInfoPtr pMessageInfo(
            DatabaseProxyFactory::Instance()->CreateEventMessageInfo());
        std::vector<IEventMessageItemPtr> EventArray;

        std::istringstream isInput(sStudyInstanceUIDs);
        string sStudyInstanceUID;
        while (std::getline(isInput, sStudyInstanceUID, ';'))
        {
            IEventMessageItemPtr pItem(
                IEventMessageItemPtr(
                DatabaseProxyFactory::Instance()->CreateEventMessageItem()));
            pItem->SetType(EventStudyImported);
            pItem->SetUID(sStudyInstanceUID);
            pItem->SetSubType(EventAll);
            pItem->SetSubUID("");
            EventArray.push_back(pItem);
        }


        pMessageInfo->SetTiggerItems(EventArray);
        IDatabaseProxyPtr m_pDatabaseProxyPtr;
        m_pDatabaseProxyPtr = DatabaseProxyFactory::Instance()->CreateDatabaseProxy(m_pCommProxy);
        m_pDatabaseProxyPtr->SendEventMessage(pMessageInfo);

    }

//enum JobStatus {
//	PENDING = 0,
//	PRINTING = 1,
//	PAUSE = 5,
//	FAILURE = 3,
//	DONE = 4,
//	UNKNOWN = 6
//};

int ConvertFromPrintStatus(JobStatus status)
{
    switch(status)
    {
    case PRINTING :
        return 1;
    case PENDING :
        return 2;
    case PAUSE :
        return 3;
    case FAILURE:
        return 0;
    case DONE:
        return 4;
    default:
        return 10;
    }
}

int ComparePrintStatus(JobStatus status1, JobStatus status2)
{
    int jobStatus1 = ConvertFromPrintStatus(status1);
    int jobstatus2 = ConvertFromPrintStatus(status2);
    
    return jobStatus1 - jobstatus2;

}

bool CompareFilmingPrintJob(FilmingPrintJob job1, FilmingPrintJob job2)
{
    //1. sort by print status: InProgress > Pending > Paused > Failure > Complete
    int statusDifference = ComparePrintStatus(job1.job_status().job_status(), job2.job_status().job_status());
    if(statusDifference < 0) return true;
    if(statusDifference > 0) return false; 

    //2. sort by priority
    //if(job1.print_priority() < job2.print_priority())	return true;
    //if(job1.print_priority() > job2.print_priority())	return false;
    //2014-10-24
    unsigned long job1Priority = boost::lexical_cast<unsigned long> (job1.priority());
    unsigned long job2Priority = boost::lexical_cast<unsigned long> (job2.priority());
    if(job1Priority < job2Priority) return true;
    if(job1Priority > job2Priority) return false;

    //3. sort by jobid
    if(job1.job_id() < job2.job_id()) return true;

    return false;

}
void McsfFilmingCommandHandler::RePlyProgressInfoToMainFrame(unsigned int uiJobID,int iTotalNum, int iFinishedNum, int iErrorNum)
{
    try
    {
            LOG_FUNC_BEGIN
            boost::mutex::scoped_lock queryCurrentPrintJobsLock(kJobListMutex);

            CommandContext filmingCmdContext;
            //filmingCmdContext.sReceiver = FILMING_FE_CMD_RECEIVER;
            filmingCmdContext.sReceiver = m_sJobManagerName;
            filmingCmdContext.iCommandId = static_cast<int>(ToMainFrameCmd);

            //TODO: serialized job info for mainframe
            McsfJobManagerInfoWrapper jobInfos;
            jobInfos.SetJobModality(Film);
            std::vector<McsfJobManagerInfo> jobInfoVector;

            std::vector<FilmingPrintJob> jobVector;
            for (auto it = m_PrintJobMap.begin(); it != m_PrintJobMap.end(); it++)
            {
                FilmingPrintJob job = it->second;
                jobVector.push_back(job);
            }
            std::sort(jobVector.begin(), jobVector.end(), CompareFilmingPrintJob);

            for (auto it=jobVector.begin(); it!=jobVector.end(); it++)
            {
                FilmingPrintJob job = *it;

                McsfJobManagerInfo jobInfo;
                jobInfo.SetJobitemid(boost::lexical_cast<string>(job.job_id()));

                if(m_locked)
                {
                    JobManagerItemStatus enumJobStatus = static_cast<JobManagerItemStatus>(job.job_status().job_status());
                    if(JobManagerItemStatus::Inprocess == enumJobStatus || JobManagerItemStatus::Paused == enumJobStatus || JobManagerItemStatus::Waiting == enumJobStatus)
                    {
                        jobInfo.SetJobitemstatus(JobManagerItemStatus::Suspend);
                    }
                    else
                    {
                        jobInfo.SetJobitemstatus(enumJobStatus);
                    }
                }
                else
                {
                    jobInfo.SetJobitemstatus(static_cast<JobManagerItemStatus>(job.job_status().job_status()));
                }		
                jobInfo.SetPatientid(job.patient_id());
                jobInfo.SetPatientname(job.patient_name());
                jobInfo.SetStudyid(job.study_id());
                jobInfo.SetDatetime(string("") + job.filming_date() + "/" + job.filming_time());
                jobInfo.SetDestinationname(std::string("@") + job.printer_ae());
                if (job.job_id() == uiJobID)
                {
                    jobInfo.SetTotalFiles(iTotalNum);
                    jobInfo.SetFinishedFiles(iFinishedNum);
                    jobInfo.SetFailedFiles(iErrorNum);
                }
                jobInfoVector.push_back(jobInfo);
            }

            jobInfos.AddJobManagerInfos(jobInfoVector);

            string sSerialized = jobInfos.Serialize();
            filmingCmdContext.sSerializeObject = sSerialized;

            // if(-1 == m_pCommProxy->AsyncSendCommand(&filmingCmdContext))
            if(-1 == m_pCommProxy->SyncSendCommand(&filmingCmdContext,sSerialized))
            {
                LOG_ERROR("Fail to Send command to MainFrame");
            }
            LOG_FUNC_END
        }
        catch (std::exception& e)
        {
            LOG_FUNC_EXCEPTION(e.what());
        }
        catch(...) 
        {
            LOG_FUNC_EXCEPTION("general exception");
        }
}
void McsfFilmingCommandHandler::RePlyJobInfoToMainFrame()
{
    try
    {
        LOG_FUNC_BEGIN

        boost::mutex::scoped_lock queryCurrentPrintJobsLock(kJobListMutex);

        CommandContext filmingCmdContext;
        //filmingCmdContext.sReceiver = FILMING_FE_CMD_RECEIVER;


        filmingCmdContext.sReceiver = m_sJobManagerName;


        filmingCmdContext.iCommandId = static_cast<int>(ToMainFrameCmd);
        
        //TODO: serialized job info for mainframe
        McsfJobManagerInfoWrapper jobInfos;
        jobInfos.SetJobModality(Film);
        std::vector<McsfJobManagerInfo> jobInfoVector;

        std::vector<FilmingPrintJob> jobVector;
        for (auto it = m_PrintJobMap.begin(); it != m_PrintJobMap.end(); it++)
        {
            FilmingPrintJob job = it->second;
            jobVector.push_back(job);
        }
        std::sort(jobVector.begin(), jobVector.end(), CompareFilmingPrintJob);
        int iTotalNum , iFinishedNum;
        int iJobIndex=0;
        for (; iJobIndex<jobVector.size() && iJobIndex<FILMING_JOB_LIST_VOLUME; iJobIndex++)
        {
            FilmingPrintJob job = jobVector[iJobIndex];

            McsfJobManagerInfo jobInfo;
            jobInfo.SetJobitemid(boost::lexical_cast<string>(job.job_id()));
            
            if(m_locked)
            {
                JobManagerItemStatus enumJobStatus = static_cast<JobManagerItemStatus>(job.job_status().job_status());
                if(JobManagerItemStatus::Inprocess == enumJobStatus || JobManagerItemStatus::Paused == enumJobStatus || JobManagerItemStatus::Waiting == enumJobStatus)
                {
                    jobInfo.SetJobitemstatus(JobManagerItemStatus::Suspend);
                }
                else
                {
                    jobInfo.SetJobitemstatus(enumJobStatus);
                }
            }
            else
            {
                jobInfo.SetJobitemstatus(static_cast<JobManagerItemStatus>(job.job_status().job_status()));
            }		
            jobInfo.SetPatientid(job.patient_id());
            jobInfo.SetPatientname(job.patient_name());
            jobInfo.SetStudyid(job.study_id());
            jobInfo.SetDatetime(string("") + job.filming_date() + "/" + job.filming_time());
            jobInfo.SetDestinationname(std::string("@") + job.printer_ae());
            iTotalNum= job.film_amount();
            iFinishedNum = job.film_finished_amount();
            jobInfo.SetTotalFiles(iTotalNum);
            jobInfo.SetFinishedFiles(iFinishedNum);
            jobInfo.SetFailedFiles(iTotalNum - iFinishedNum);
            jobInfoVector.push_back(jobInfo);
        }

        ////todo: 超过200个失败任务，警告
        //if(iJobIndex<jobInfoVector.size() && jobVector[iJobIndex].job_status().job_status() == JobStatus::FAILURE)
        //{
        //	MHC::McsfMhcStatusInfoHandler messageHandler(m_pCommProxy);
        //	(void)messageHandler.ShowStatusWithRemoteUid(MHC::Warning, "UID_Filming_Job_Full");
        //}

        jobInfos.AddJobManagerInfos(jobInfoVector);

        string sSerialized = jobInfos.Serialize();
        filmingCmdContext.sSerializeObject = sSerialized;

        // if(-1 == m_pCommProxy->AsyncSendCommand(&filmingCmdContext))
        if(-1 == m_pCommProxy->SyncSendCommand(&filmingCmdContext,sSerialized))
        {
            LOG_ERROR("Fail to Send command to MainFrame");
        }
        LOG_FUNC_END
    }
    catch (std::exception& e)
    {
        LOG_FUNC_EXCEPTION(e.what());
    }
    catch(...) 
    {
        LOG_FUNC_EXCEPTION("general exception");
    }
}

int McsfFilmingCommandHandler::EvaluateRemainingWorkload()
{
    try
    {
        LOG_FUNC_BEGIN

            boost::mutex::scoped_lock queryCurrentPrintJobsLock(kJobListMutex);

        int iFilms = 0;

        for (auto it=m_PrintJobMap.begin(); it!=m_PrintJobMap.end(); it++)
        {
            FilmingPrintJob job = it->second;
            JobStatus jobStatus = job.job_status().job_status();

            if (!(jobStatus == JobStatus::PENDING || jobStatus == JobStatus::PRINTING))
            {
                continue;
            }

            iFilms += job.copies() * job.film_box_size();
        }

        return iFilms * TIME_FOR_ONE_FILM_SENDING_TO_PRINTER;

    }
    catch (std::exception& e)
    {
        LOG_FUNC_EXCEPTION(e.what());
    }
    catch(...) 
    {
        LOG_FUNC_EXCEPTION("general exception");
    }

    return 0;
}

void McsfFilmingCommandHandler::DeleteEFilms( const FilmingPrintJob& job )
{
    try
    {
        for (int i=0; i<job.film_box_size(); i++)
        {
            auto filmBox = job.film_box(i);
            string efilmPath = filmBox.efilm_path();
            if (boost::filesystem::exists(efilmPath))
            {
                boost::filesystem::remove(efilmPath);
            }
            for (int j=0; j<filmBox.image_box_size(); j++)
            {
                auto imageBox = filmBox.image_box(j);
                string filePath = imageBox.image_path();
                if (boost::filesystem::exists(filePath))
                {
                    boost::filesystem::remove(filePath);
                }
            }
        }
    }
    catch (std::exception& e)
    {
        LOG_FUNC_EXCEPTION(e.what());
    }
    catch (...)
    {
        LOG_FUNC_EXCEPTION("general exception")
    }
}

void McsfFilmingCommandHandler::TransferToPrinterOfPrintJob( unsigned int uiJobID, 
    string sPrinterAE, string sOurAE,string sPrinterIP,unsigned iPrinterPort,FilmingJobStatus *pJobStatus )
{
    try
    {
        if (0 == uiJobID)
        {
            LOG_INFO("Parameter error, jobid = 0 is perserved by backend");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"Parameter error, jobid = 0 is perserved by backend");
            return;
        }
        FilmingPrintJob* printJobObject = NULL;
        {
            boost::mutex::scoped_lock transferToPrinterLock(kJobListMutex);
            if (m_PrintJobMap.find(uiJobID) == m_PrintJobMap.end() )
            {
                LOG_INFO("Has not found the job, I can't tell whether the job is done or  exception");
                SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,
                    "Has not found the job, I can't tell whether the job is done or  exception");
                return;
            }
            LOG_INFO(
                string("Has found the job") + boost::lexical_cast<string>(uiJobID));
            printJobObject = &m_PrintJobMap[uiJobID];
            JobStatus jobStatus = printJobObject->mutable_job_status()->job_status();
            if (jobStatus == JobStatus::PRINTING) return;
            printJobObject->set_printer_ae(sPrinterAE);
            printJobObject->set_our_ae(sOurAE);
            printJobObject->set_printer_ip(sPrinterIP);
            printJobObject->set_port(iPrinterPort);
            //暂停或者失败或者已完成任务，转印后都需要重启
            //if (jobStatus == JobStatus::FAILURE || jobStatus == JobStatus::PAUSE || jobStatus == JobStatus::DONE)
            if (jobStatus == JobStatus::PENDING) return;
        }
        std::stringstream ss;
        ss << uiJobID;
        std::string sSerializedJobID  = ss.str();
        RestartPrintJob( sSerializedJobID, pJobStatus );
    }
    catch (...)
    {  
        LOG_ERROR(
            string("Exception when Transfer To Printer of a job, job id is ")  
            + boost::lexical_cast<string>(uiJobID));
        SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"Exception when adjust priority");
    }
}
void McsfFilmingCommandHandler::TransferToPrinterOfPrintJob( const std::string& sSerializedJobInfo,
    FilmingJobStatus *pJobStatus )
{
    try
    {
        if(NULL == pJobStatus)
        {
            LOG_ERROR("parameter pJobStatus is NULL");
            return;
        }

        if(sSerializedJobInfo.empty())
        {
            LOG_ERROR("receive null print job!");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"NO DATA FROM FE");
            return;
        }
        FilmingPrintJobQueue filmPrintJobQueue;
        if (false == filmPrintJobQueue.ParseFromString(sSerializedJobInfo))
        {
            LOG_ERROR("There may be some optional field of serialized FilmingPrintJob not setted");
            SetFilmingJobStatus(pJobStatus,JobStatus::FAILURE,-1,"filmPrintJobQueue.ParseFromString  failure");
            return;
        }

        for (int i =0 ; i < filmPrintJobQueue.job_size(); i++)
        {
            const FilmingPrintJob& it = filmPrintJobQueue.job(i);
            TransferToPrinterOfPrintJob(it.job_id(), it.printer_ae(), it.our_ae(), it.printer_ip(), it.port(),pJobStatus);
 
        }

    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingCommandHandler::AutoArchiving(string seriesInstanceUid, string studyInstanceUid)
{
    try
    {
        ////todo:autoArchiving
        //0. Read or get configure to decide whether to archive or not.
        if(!m_bAutoArchiving) return;
        if(m_sArchivedSeriesDescription == "") return;
        //if(seriesInstanceUid=="") return;  
    
        //////1. get last series of the study(study id)
        //string sql = "StudyID=\""+studyId+"\"";
        //std::vector<IStudyPtr> studies;
        //int dbRet = m_pDBNotifier->GetStudyListBySQL(sql, studies);
        //if(dbRet != ERROR_DB_NULL || studies.size()==0)
        //{
        //	string info = "DB access error or No study of studyid = " + studyId;
        //	LOG_WARN_DB(info);
        //	return;
        //}

        //string studyInstanceUid = studyId;// studies[0]->GetStudyInstanceUID();
        string sql = "StudyInstanceUIDFk=\""+studyInstanceUid + "\" AND SeriesDescription=\"" + m_sArchivedSeriesDescription  +"\"";
        std::vector<ISeriesPtr> serieses;
        LOG_INFO("begin Query from DB");
        int dbRet = m_pDBNotifier->GetSeriesListBySQL(sql, serieses);
        LOG_INFO("end Query from DB");
        if(dbRet != ERROR_DB_NULL || serieses.size()==0)
        {
            string info = "DB access error or No series of studyInstancUid = " + studyInstanceUid;
            LOG_WARN_DB(info);
            return;
        }


        //m_pDBNotifier->getser
        string seriesInstanceUid = serieses[0]->GetSeriesInstanceUID();


        //2. call auto archiving interface
        McsfArchivingAutoInterface* pArchivingInterface = new McsfArchivingAutoInterface(m_pCommProxy);
        pArchivingInterface->Archive(STORE_SEIRES_LEVEL, seriesInstanceUid);
        delete pArchivingInterface;
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

unsigned long McsfFilmingCommandHandler::CalculateMaxPriority()
{
    unsigned long ulMaxPriority = MIN_PRIORITY;
    try
    {
        LOG_FUNC_BEGIN

        boost::mutex::scoped_lock queryCurrentPrintJobsLock(kJobListMutex);
        for (auto it = m_PrintJobMap.begin(); it != m_PrintJobMap.end(); it++)
        {
            FilmingPrintJob job = it->second;
            unsigned long jobPriority = boost::lexical_cast<unsigned long>(job.priority());
            if(ulMaxPriority > jobPriority) ulMaxPriority = jobPriority;
        }

        LOG_FUNC_END
    }
    catch (std::exception& e)
    {
        LOG_FUNC_EXCEPTION(e.what());
    }
    catch(...) 
    {
        LOG_FUNC_EXCEPTION("general exception");
    }

    return ulMaxPriority;
}


void McsfFilmingCommandHandler::UpdatePrintStatus( FilmingPrintJob &filmingPrintJob, bool bResult )
{
    //todo: update study table's print status when print failure
    bool isAnyDBObjectLocked =false;
    std::set<std::string> studyInstanceUidSet;
    for(int i =0; i< filmingPrintJob.film_box_size(); i++)
    {
        string uids = filmingPrintJob.film_box(i).study_instance_uid();
        std::istringstream isInput(uids);
        std:: string sTemp;
        while (std::getline(isInput, sTemp, ';'))
        {
            studyInstanceUidSet.insert(sTemp);
        }
    }

    for(auto iter = studyInstanceUidSet.begin(); iter != studyInstanceUidSet.end(); iter++)
    {
        if(isAnyDBObjectLocked) break;
        //use DB's new interface to update print status.

        //if(m_pDatabase->IsLocked(*iter))
        //{
        //	isAnyDBObjectLocked = true;
        //	LOG_WARN("Fail to update print status of an locked study: ");
        //	LOG_WARN(*iter);
        //	break;
        //}
    }
    if(!isAnyDBObjectLocked)
    {
        int iPrintStatus = bResult ? 1 : 3;

        ostringstream os;
        os << "StudyPrintStatus=" << "'" << iPrintStatus << "'";
        string sStatement = os.str();
        for (auto iter = studyInstanceUidSet.begin(); iter != studyInstanceUidSet.end(); iter++)
        {
            int iRet = m_pDatabase->UpdateStudyObjectByUID(*iter, sStatement);
            if(iRet != ERROR_DB_NULL)
            {
                LOG_ERROR("Update Study Print Status Failed!");
            }
        }
    }
}

void McsfFilmingCommandHandler::SetRetryConnectPrinterTimes(int iTimes)
{
    m_iRetryConnectPrinterTimes = iTimes;
    LOG_INFO("Set SetRetryConnectPrinterTimes " + boost::lexical_cast<string>(iTimes));
}

void McsfFilmingCommandHandler::SetSetFilmBoxTimeOutTime(int time)
{
    m_iSetFilmBoxTimeOutTime = time;
    LOG_INFO("Set SetFilmBoxTimeOutTime " + boost::lexical_cast<string>(time));
}


MCSF_FILMING_END_NAMESPACE

