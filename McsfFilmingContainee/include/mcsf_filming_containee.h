//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_filming_containee.h
///  \brief   the uplayer class from a basic interface class IContainee  
///
///  \version 1.0
///  \date    Nev. 3, 2011
///  
//////////////////////////////////////////////////////////////////////////
#ifndef FILMING_CONTAINEE_H_
#define FILMING_CONTAINEE_H_

#include <string>
#include <list>

#include "McsfContainee/mcsf_containee_interface.h"
#include "McsfNetBase/mcsf_netbase_interface.h"

#include "mcsf_filming_command_handler.h"

#include "mcsf_adv_app_save_filming_command_handler.h"

MCSF_FILMING_BEGIN_NAMESPACE
class ICommunicationProxy;
class IEventHandler;
class ICommandHandler;
class IBigDataHandler;

class McsfFilmingContainee : public Mcsf::IContainee
{
public:
    McsfFilmingContainee(void);
    ~McsfFilmingContainee(void);

    // interface implementation
    // \brief set container
    void SetCommunicationProxy(ICommunicationProxy* pContainer);
    // \brief startup
    void Startup();

    // \brief start a customer thread to do real print work
    void DoWork();    

    // \brief shutdown
    bool Shutdown(bool bReboot);

    bool WaitForShutdown();

    bool IsShutDownAble();

    //void FinishJob();

    //int GetEstimatedTimeToFinishJob(bool bReboot);

    void StartShutdown(bool bReboot);

    int GetTaskRemainingProgress(std::list<TaskProgress> & taskProgress);

	std::list<std::string> GetRunningTasks();

    void SetCustomConfigFile(const std::string& sFilename);
    // \brief force shut down
    //void ForceShutdown();
    // \brief handle shutdown request
    //void HandleShutdownRequest();

    //int  HandleEvent( const std::string& sSender, int iChannelId, int iEventId, const std::string& sEvent ) ;

    /// \brief  Set Containee name
    virtual void SetName( const std::string& sName );

    /// \brief  Return Containee name
    virtual const std::string& GetName( );

private:
    void RegisterCommandHandler(ICommandHandler* const & pCmdHandler, eFilmingCommandID id);

    bool ParseFilmingConfigFile( IFileParser* pFileParser, string sPrinterConfigFilePath );

	bool ParseFilmingServiceConfigFile( IFileParser* pFileParser, string sPrinterConfigFilePath );

private:
    std::string m_sName;

    /// \brief	a filming command handler is registered for Command 3888.
    McsfFilmingCommandHandler* m_pFilmingCommandHandler;

	//AppSaveFilmingCommandHandler* m_pSaveFilmingCommandHandler;
	boost::shared_ptr<AppSaveFilmingCommandHandler> m_pSaveFilmingCommandHandler;//m_cmdHandlerCommonSaveFilmingImage;

protected:
    /// \brief communication proxy ,init by Container.
    ICommunicationProxy* m_pCommProxy;
	IDatabasePtr m_pDatabase;
};
DECLARE_CONTAINEE(McsfFilmingContainee);
MCSF_FILMING_END_NAMESPACE

#endif      //FILMING_CONTAINEE_H_
