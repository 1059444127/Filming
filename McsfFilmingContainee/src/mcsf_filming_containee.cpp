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

#include "boost/filesystem.hpp" // for boost filesystem

#include "Common/McsfSystemEnvironmentConfig/mcsf_systemenvironment_factory.h"

#include "McsfMedViewer/mcsf_med_viewer_controller_interface.h"
#include "McsfMedViewer/mcsf_med_viewer_controller_factory.h"

#include "McsfContainee/mcsf_containee_cmd_id.h"

#include "mcsf_filming_containee_config.h"

#include "mcsf_filming_containee.h"

#include "mcsf_efilm_info_accessor.h"


MCSF_FILMING_BEGIN_NAMESPACE

IMPLEMENT_CONTAINEE(McsfFilmingContainee);
McsfFilmingContainee::McsfFilmingContainee() 
    : m_pFilmingCommandHandler(NULL), 
	  //m_pSaveFilmingCommandHandler(NULL), 
      m_pCommProxy(NULL)
{

}

McsfFilmingContainee::~McsfFilmingContainee()
{
    try
    {
        if (NULL!=m_pFilmingCommandHandler)
        {
            delete m_pFilmingCommandHandler;
            m_pFilmingCommandHandler=NULL;
        }

		//if (NULL!=m_pSaveFilmingCommandHandler)
		//{
		//	delete m_pSaveFilmingCommandHandler;
		//	m_pSaveFilmingCommandHandler=NULL;
		//}
    }
    catch(...)
    {
        return ;
    }
}

void McsfFilmingContainee::SetCommunicationProxy(ICommunicationProxy* pContainer)
{
    if (NULL == pContainer)
    {
        LOG_ERROR( "Error:the param is null! in SetContainer(*pC)" );
    }

    m_pCommProxy = pContainer;

    LOG_INFO( "Info:Set filmingbe container ok!" );
}

void McsfFilmingContainee::Startup()
{
    try
    {
        if (NULL == m_pCommProxy)
        {
            LOG_ERROR( "Initial McsfFilmingContainee failed!" );
            return;
        }

		m_pDatabase = IDatabaseFactory::Instance()->CreateDBWrapper();
		if (NULL == m_pDatabase)
		{
			LOG_ERROR( "Create database of McsfFilmingContainee failed!" );
			return;
		}

		if (!m_pDatabase->Initialize())
		{
			LOG_ERROR( "Initial database of McsfFilmingContainee failed!" );
			return;
		}

        m_pDatabase->UnLock(MCSF_FILMING_SEVICE_LOGGER_NAME);

        //register
        LOG_INFO( "Info:McsfFilmingContainee initialized start!" );
    
        if (NULL == m_pFilmingCommandHandler)
        {
            m_pFilmingCommandHandler = new McsfFilmingCommandHandler(m_pCommProxy);
            if(NULL == m_pFilmingCommandHandler)
            {
                LOG_ERROR( "m_pFilmingCommandHandler is NULL!" );
                return;
            }
        }
    
		//if (NULL == m_pSaveFilmingCommandHandler)
		//{
		//	m_pSaveFilmingCommandHandler = new AppSaveFilmingCommandHandler(m_pDatabase, m_pCommProxy);
		//	if(NULL == m_pSaveFilmingCommandHandler)
		//	{
		//		LOG_ERROR( "m_pSaveFilmingCommandHandler is NULL!" );
		//		return;
		//	}
		//}

		//m_pSaveFilmingCommandHandler->SetFilmingCommandHandler(m_pFilmingCommandHandler);
		 m_pSaveFilmingCommandHandler.reset(new AppSaveFilmingCommandHandler(m_pDatabase, m_pCommProxy));
         LOG_INFO( "Info:m_pSaveFilmingCommandHandler reset ok!" );
		 m_pSaveFilmingCommandHandler->SetFilmingCommandHandler(m_pFilmingCommandHandler);

		ImageInfoAccessorPtr pImgAccessor(new EFilmInfoAccessor(m_pDatabase));
		//m_pSaveFilmingCommandHandler->RegisterImageInfoAccessor(pImgAccessor);
		m_pSaveFilmingCommandHandler->RegisterImageInfoAccessor(pImgAccessor);
        LOG_INFO( "Info:RegisterImageInfoAccessor  ok!" );
		////IProgressPtr pProgress(new ReviewBEProgress(m_pCommProxy));
		//ReviewBEProgressPtr pProgress(new ReviewBEProgress(m_pCommProxy));
		//pProgress->SetApplicationName("review2d");
		//pProgress->SetProgressInfo("Saving");
		//pAppSaveAs->RegisterProgress(pProgress);
		//TagInserterPtr pSCTagInserter(new SecondCaptureTagInserter(m_pReviewController->GetDBWrap()));
		//const std::string sSOPClassUID("1.2.840.10008.5.1.4.1.1.7");
		//m_pSaveFilmingCommandHandler->RegisterEssentialTagInserter(sSOPClassUID, pSCTagInserter);

        //Register Filming Command
        RegisterCommandHandler(m_pFilmingCommandHandler, ADJUST_PRIORITY_OF_PRINT_JOB_COMMAND);
        RegisterCommandHandler(m_pFilmingCommandHandler, DELETE_PRINT_JOB_COMMAND);
        RegisterCommandHandler(m_pFilmingCommandHandler, GET_PRINTER_CONFIG_COMMAND);
        RegisterCommandHandler(m_pFilmingCommandHandler, QUERY_HISTORY_PRINT_JOB_COMMAND);
        RegisterCommandHandler(m_pFilmingCommandHandler, REPRINT_COMMAND);
        RegisterCommandHandler(m_pFilmingCommandHandler, PAUSE_PRINT_JOB_COMMAND);
        RegisterCommandHandler(m_pFilmingCommandHandler, RESUME_PRINT_JOB_COMMAND);
        RegisterCommandHandler(m_pFilmingCommandHandler, ADD_PRINT_JOB_COMMAND);
        RegisterCommandHandler(m_pFilmingCommandHandler, QUERY_CURRENT_PRINT_JOBS_COMMAND);
        RegisterCommandHandler(m_pFilmingCommandHandler, SUSPEND_ALL_PRINT_JOB_COMMAND);
        RegisterCommandHandler(m_pFilmingCommandHandler, RESUME_ALL_PRINT_JOB_COMMAND);
        RegisterCommandHandler(m_pFilmingCommandHandler, TRANSFER_TO_PRINTER);

        RegisterCommandHandler(m_pFilmingCommandHandler, SAVE_EFILMS_COMMAND);

        RegisterCommandHandler(m_pFilmingCommandHandler, (eFilmingCommandID)FromMainFramePauseCmd);
        RegisterCommandHandler(m_pFilmingCommandHandler, (eFilmingCommandID)FromMainFrameContinueCmd);
        RegisterCommandHandler(m_pFilmingCommandHandler, (eFilmingCommandID)FromMainFrameRestartCmd);
        RegisterCommandHandler(m_pFilmingCommandHandler, (eFilmingCommandID)FromMainFrameDeleteCmd);
        RegisterCommandHandler(m_pFilmingCommandHandler, (eFilmingCommandID)FromMainFrameUrgentCmd);
        RegisterCommandHandler(m_pFilmingCommandHandler, (eFilmingCommandID)FromMainFrameRefreshCmd);
        RegisterCommandHandler(m_pFilmingCommandHandler, (eFilmingCommandID)FromMainFrameStopCmd);

        //save EFilm
        //RegisterCommandHandler(m_pSaveFilmingCommandHandler, SAVE_EFILMS_COMMON_SAVE);
        m_pCommProxy->RegisterCommandHandlerEx(SAVE_EFILMS_COMMON_SAVE, m_pSaveFilmingCommandHandler);
        
        //Clear print temp directory
        
        IFileParser* pFileParser = Mcsf::ConfigParserFactory::Instance()->GetXmlFileParser();
        if (NULL == pFileParser)
        {
            LOG_ERROR_FILMING("Can't get Printer FileParser instance");
            return;
        }
        pFileParser->Initialize();
        

        string sFilmingServiceConfigFilePath = FILMING_SERVICE_CONFIG_FILE_NAME;
        string sMcsfFilmingFilePath = MCSF_FILMING_CONFIG_FILE_NAME;

        LOG_INFO("Info:McsfFilmingContainee Parse Filming Config File Begin!");

        if(!ParseFilmingConfigFile(pFileParser, sMcsfFilmingFilePath))
        {
            std::string sLog = "Filming Config File " + sMcsfFilmingFilePath + " has not been parsed correctly";
            LOG_WARN_FILMING(sLog);
        }
        LOG_INFO("Info:McsfFilmingContainee Parse FilmingService Config File Begin!");
        ParseFilmingServiceConfigFile(pFileParser,sFilmingServiceConfigFilePath);
        pFileParser->Terminate();

        LOG_INFO("Info:McsfFilmingContainee initialized end!");
    }
    catch (...)
    {
        LOG_ERROR("There is some exception when starting up Filming containee");
    }

}

bool McsfFilmingContainee::Shutdown(bool bReboot)
{
	LOG_INFO("bReboot: " + bReboot);
    return false;
}

bool McsfFilmingContainee::WaitForShutdown()
{
    return false;
}

void McsfFilmingContainee::SetName(const std::string& sName)
{
    m_sName = sName;
}

const std::string& McsfFilmingContainee::GetName()
{
    return m_sName;
}

void McsfFilmingContainee::SetCustomConfigFile(const std::string& sConfigFile)
{
    std::cout<<sConfigFile<<std::endl;		
}

bool McsfFilmingContainee::IsShutDownAble()
{
    return true;
}

void McsfFilmingContainee::DoWork()
{
    try
    {
        LOG_INFO( "Info:McsfFilmingContainee::DoWork()! start the print Thread!" );

        //start the print thread as a producer.
        if (NULL != m_pFilmingCommandHandler)
        {
            m_pFilmingCommandHandler->OpenPrintThread();
        }

        if(NULL == m_pCommProxy || m_pCommProxy->SendSystemEvent( "", static_cast<int>(SYSTEM_COMMAND_EVENT_ID_COMPONENT_READY), m_pCommProxy->GetName() ) )
        {
            LOG_ERROR("The event send to System manager fail,Please restart the FilmingBEContainee");
        }	
    }
    catch (...)
    {
    	LOG_ERROR("Exception at DoWork");
    }


}

void McsfFilmingContainee::RegisterCommandHandler( 
    ICommandHandler* const &pCmdHandler, eFilmingCommandID id )
{
    try
    {
        if (NULL == m_pCommProxy || NULL == pCmdHandler)
        {
            LOG_ERROR("Can't register command because m_pCommProxy or m_pCmdHandler is NULL");
            return;
        }
        std::ostringstream os;
        os << "Register Command " << static_cast<int>(id);

        if(0 != m_pCommProxy->RegisterCommandHandler(static_cast<int>(id), pCmdHandler))
        {
            os << " failed!";
            LOG_ERROR( os.str() );
            return;
        }

        os << " succeeded!";      
        LOG_INFO( os.str() );
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

//void McsfFilmingContainee::FinishJob()
//{
//    if (NULL != m_pFilmingCommandHandler)
//    {
//        m_pFilmingCommandHandler->ClosePrintThread();
//    }
//}

//int McsfFilmingContainee::GetEstimatedTimeToFinishJob(bool bReboot)
//{
//    string log = bReboot ? "bReboot: True" : "bReboot: False";
//	LOG_INFO(log);
//
//	if (NULL != m_pFilmingCommandHandler)
//	{
//		return m_pFilmingCommandHandler->EvaluateRemainingWorkload();
//	}
//    return 0;
//}

		void McsfFilmingContainee::StartShutdown(bool /*bReboot*/){}

		int McsfFilmingContainee::GetTaskRemainingProgress(std::list<TaskProgress> & /*taskProgress*/)	{return 0;}

		std::list<std::string> McsfFilmingContainee::GetRunningTasks(){return std::list<std::string>();}


bool McsfFilmingContainee::ParseFilmingConfigFile( IFileParser* pFileParser, string sFilmingConfigFilePath )
{
	if (false == pFileParser->OpenFromUserSettingsDir(sFilmingConfigFilePath))
	{
		LOG_WARN_FILMING("Printer Config File " + sFilmingConfigFilePath + " has been parsed failed") ;
		return false;
	}
	string sPrintObjectStoragePath;
	if (false == pFileParser->GetStringValueByTag("PrintObjectStoragePath", &sPrintObjectStoragePath) )
	{
		LOG_WARN_FILMING("error when parsing Tag  \"PrintObjectStoragePath ");
		return false;
	}

	string sArchivedSeriesDescription;
	if (false == pFileParser->GetStringValueByPath("NewSeriesDescription", &sArchivedSeriesDescription) )
	{
		LOG_WARN_FILMING("error when parsing Tag  \"NewSeriesDescription ");
		return false;
	}

    bool bAutoArchive = false;
    if (false == pFileParser->GetBoolValueByPath("SaveAsNewSeries", &bAutoArchive) )
    {
        LOG_WARN_FILMING("error when parsing Tag  \"SaveAsNewSeries ");
        return false;
    }

	boost::filesystem::remove_all(sPrintObjectStoragePath);
    LOG_INFO("Info:McsfFilmingContainee Set AutoAchiving Setting");
	m_pFilmingCommandHandler->SetArchivedSeriesDescription(sArchivedSeriesDescription);
    m_pFilmingCommandHandler->SetAutoArchivingFlag(bAutoArchive);

	return true;
}

bool McsfFilmingContainee::ParseFilmingServiceConfigFile( IFileParser* pFileParser, string sFilmingServiceConfigFilePath )
{

    if (false == pFileParser->OpenFromUserSettingsDir(sFilmingServiceConfigFilePath))
	{
		LOG_WARN_FILMING("Printer Config File " + sFilmingServiceConfigFilePath + " has been parsed failed") ;
		return false;
	}
	int iRetryConnectPrinterTimes;
	if (false == pFileParser->GetIntValueByTag("RetryConnectPrinterTimes", &iRetryConnectPrinterTimes) )
	{
		LOG_WARN_FILMING("Error when parsing Tag  \"RetryConnectPrinterTimes\",Set DefaultValue : 5. ");
		m_pFilmingCommandHandler->SetRetryConnectPrinterTimes(5);
	}
	else
	{
		m_pFilmingCommandHandler->SetRetryConnectPrinterTimes(iRetryConnectPrinterTimes);
	}

	int iSetFilmBoxTimeOutTime;
	if (false == pFileParser->GetIntValueByTag("SetFilmBoxTimeOut", &iSetFilmBoxTimeOutTime) )
	{
		LOG_WARN_FILMING("Error when parsing Tag  \"SetFilmBoxTimeOut\",Set DefaultValue : 120. ");
		m_pFilmingCommandHandler->SetSetFilmBoxTimeOutTime(120);
	}
	else
	{
		m_pFilmingCommandHandler->SetSetFilmBoxTimeOutTime(iSetFilmBoxTimeOutTime);
	}

	return true;
}

MCSF_FILMING_END_NAMESPACE

