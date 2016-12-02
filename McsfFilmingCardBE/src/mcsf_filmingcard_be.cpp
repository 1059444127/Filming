//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Wang Hui  hui.wang@united-imaging.com
///
///  \file    mcsf_filming_command_handler.cpp
///  \brief   filming command handler
///
///  \version 1.0
///  \date    Dec. 26, 2011
///  
//////////////////////////////////////////////////////////////////////////

#include "McsfFoundation/mcsf_video_memory_management.h"

#include "McsfMedViewer/mcsf_med_viewer_controller_interface.h"
#include "McsfMedViewer/mcsf_med_viewer_controller_factory.h"

#include "Common/McsfSystemEnvironmentConfig/mcsf_systemenvironment_factory.h"

#include "McsfContainee/mcsf_containee_cmd_id.h"

#include "mcsf_filmingcard_config.h"

#include "mcsf_filmingcard_be.h"

using namespace std;

MCSF_FILMING_BEGIN_NAMESPACE

IMPLEMENT_CONTAINEE(McsfFilmingCardBE);
McsfFilmingCardBE::McsfFilmingCardBE() 
    :m_pFilmingImageCommandHandler(NULL), 
    m_pCommProxy(NULL)
{

}

McsfFilmingCardBE::~McsfFilmingCardBE()
{
    try
    {
        if (NULL!=m_pFilmingImageCommandHandler)
        {
            delete m_pFilmingImageCommandHandler;
            m_pFilmingImageCommandHandler=NULL;
        }
    }
    catch(...)
    {
        return ;
    }
}

void McsfFilmingCardBE::SetCommunicationProxy(ICommunicationProxy* pContainer)
{
    if (NULL == pContainer)
    {
        LOG_ERROR( "Error:the param is null! in SetContainer(*pC)" );
    }

    m_pCommProxy = pContainer;

    LOG_INFO( "Info:Set filmingbe container ok!" );
}

void McsfFilmingCardBE::DisplayVideoMemoryStatus()
{
	if(NULL == m_pCommProxy) return;

	std::unique_ptr< VideoMemoryManagement > upVDMM( 
		new VideoMemoryManagement(m_pCommProxy));

	unsigned int iTotalSize = 0;
	unsigned int iFreeSize = 0;
	if(!upVDMM->GetVideoMemInfo(iTotalSize, iFreeSize))
		LOG_WARN("Fail to Get Video Memory info");

	int iReseveredSize = 0;
	if(!upVDMM->GetTotalReservedVideoMemorySize(iReseveredSize))
		LOG_WARN("Fail to Get Reserved Video Memory Size");


	std::ostringstream os;
	os << "Total GPU " << iTotalSize 
		<<" Free GPU " << iFreeSize 
		<<" Already Reserved GPU " << iReseveredSize;

	LOG_INFO(os.str());

}

void McsfFilmingCardBE::Startup()
{
    try
    {
		LOG_INFO("Begin to startup");

        if (NULL == m_pCommProxy)
        {
            LOG_ERROR( "Initial McsfFilmingCardBE failed!" );
            return;
        }
    
   
        //register
        LOG_INFO( "Info:McsfFilmingCardBE initialized ok!" );
    
        if (NULL == m_pFilmingImageCommandHandler)
        {
            m_pFilmingImageCommandHandler = 
                new McsfFilmingImageCommandHandler(m_pCommProxy);
        }
    
        if(NULL == m_pCommProxy 
            || NULL == m_pFilmingImageCommandHandler)
        {
            LOG_ERROR( "Communication Proxy or one of the command handler is NULL!" );
            return;
        }
    
        //Register Image Command
        RegisterCommandHandler(m_pFilmingImageCommandHandler, SAVE_IMAGE_COMMAND);
        RegisterCommandHandler(m_pFilmingImageCommandHandler, LOAD_IMAGE_COMMAND);
        RegisterCommandHandler(m_pFilmingImageCommandHandler, LOAD_STUDY_COMMAND);
        RegisterCommandHandler(m_pFilmingImageCommandHandler, REMOVE_CELL_COMMAND);
        RegisterCommandHandler(m_pFilmingImageCommandHandler, REMOVE_ALL_COMMAND);
        RegisterCommandHandler(m_pFilmingImageCommandHandler, REMOVE_CELL_COMMAND);

        RegisterCommandHandler(m_pFilmingImageCommandHandler, SAVE_FILMS_COMMAND);

        RegisterCommandHandler(m_pFilmingImageCommandHandler, SAVE_EFILMS_COMMAND);

        RegisterCommandHandler(m_pFilmingImageCommandHandler, CREATE_NEW_VIEWER_CONTROLLER);

		//apply for Reserved Video Memory
		DisplayVideoMemoryStatus();

		ostringstream os;
		os << "apply for Reserved Video Memory " << ReservedVideoMemoryMBs << " MB" ;
		LOG_INFO(os.str());

		std::unique_ptr< VideoMemoryManagement > upVDMM( 
			new VideoMemoryManagement(m_pCommProxy));

		if(!upVDMM->AddReserveVideoMemory(ReservedVideoMemoryMBs))	//apply for 100MB video memory
			LOG_WARN("Fail to Apply for Reserved Video Memory");

		DisplayVideoMemoryStatus();


    }
    catch (...)
    {
        LOG_ERROR("There is some exception when starting up Filming containee");
    }
	LOG_INFO("End to startup");

}

bool McsfFilmingCardBE::Shutdown(bool bReboot)
{
	LOG_INFO("bReboot: " + bReboot); 

	LOG_INFO("Begin to shutdown");

	if(NULL != m_pCommProxy)
	{

		DisplayVideoMemoryStatus();

		ostringstream os;
		os << "Release Video Memory " << ReservedVideoMemoryMBs << " MB";
		LOG_INFO(os.str());

		std::unique_ptr< VideoMemoryManagement > upVDMM( 
			new VideoMemoryManagement(m_pCommProxy));

		
		if(!upVDMM->ReleaseReserveVideoMemory(ReservedVideoMemoryMBs))
			LOG_WARN("Fail to Release Video Memory");

		DisplayVideoMemoryStatus();
	}

	LOG_INFO("End to shutdown");

    return false;
}

bool McsfFilmingCardBE::WaitForShutdown()
{
    return false;
}

void McsfFilmingCardBE::SetName(const std::string& sName)
{
    m_sName = sName;
}

const std::string& McsfFilmingCardBE::GetName()
{
    return m_sName;
}

void McsfFilmingCardBE::SetCustomConfigFile(const std::string& sConfigFile)
{
    std::cout<<sConfigFile<<std::endl;		
}

bool McsfFilmingCardBE::IsShutDownAble()
{
    return true;
}

void McsfFilmingCardBE::DoWork()
{
    try
    {
        LOG_INFO( "Info:McsfFilmingCardBE::DoWork()! start the print Thread!" );

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

void McsfFilmingCardBE::RegisterCommandHandler( 
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

//void McsfFilmingCardBE::FinishJob()
//{
//
//}

//int McsfFilmingCardBE::GetEstimatedTimeToFinishJob(bool bReboot)
//{
//    string log = bReboot ? "bReboot: True" : "bReboot: False";
//	LOG_INFO(log);
//    return 0;
//}

	void McsfFilmingCardBE::StartShutdown(bool /*bReboot*/){}

		int McsfFilmingCardBE::GetTaskRemainingProgress(std::list<TaskProgress> & /*taskProgress*/)	{return 0;}

		std::list<std::string> McsfFilmingCardBE::GetRunningTasks(){return std::list<std::string>();}

MCSF_FILMING_END_NAMESPACE

