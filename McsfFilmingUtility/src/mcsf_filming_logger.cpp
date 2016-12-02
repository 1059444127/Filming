////////////////////////////////////////////////////////////////////////////////
/// Copyright (c) Shanghai United Imaging Healthcare Inc., 2012
///    All rights reserved.
///
///    /author    WANG Hui mailto:hui.wang@united-imaging.com
///
///    /file     mcsf_filming_logger.cpp
///
///    /brief
///
///    /version    1.0
///    /date 10/4/2012
/////////////////////////////////////////////////////////////////////////////////

#include "Common/McsfSystemEnvironmentConfig/mcsf_systemenvironment_factory.h"

#include "mcsf_filming_logger.h"

#ifdef DEVELOP_DEBUG_MODE
#include "dcmtk/oflog/oflog.h"
#endif // DEVELOP_DEBUG_MODE

MCSF_FILMING_BEGIN_NAMESPACE

const std::string McsfFilmingLogger::MCSF_FILMING_BE_LOGGER_CONFIG_PATH = "McsfFilmingLog.xml";

#ifdef  DEVELOP_DEBUG_MODE
OFLogger printScuLogger = OFLog::getLogger(EMPTY_STRING.c_str());
#endif//DEVELOP_DEBUG_MODE

//void McsfFilmingLogger::InitLogger()
//{
//    try
//    {
//	    std::string sFilePath(EMPTY_STRING);
//	    ISystemEnvironmentConfig *pSysConfig = 
//	        ConfigSystemEnvironmentFactory::Instance()->GetSystemEnvironment();
//	    if (NULL != pSysConfig)
//	    {
//	        sFilePath = pSysConfig->GetApplicationPath("FilmingConfigPath");
//	    }
//	
//	    if(sFilePath.empty())
//	    {
//	        std::cout<<"get null path of filming logger configure file, please check!!"
//	                   <<std::endl;
//	    }
//	
//	    sFilePath += MCSF_FILMING_BE_LOGGER_CONFIG_PATH;
//	
//	    (void)CLogClientManager::Instance()->GetLoggerInstance(
//	        sFilePath, m_sLoggerName);
//    }
//    catch (std::exception& e)
//    {
//    	std::cout << "Std Exception at Init Filming Logger: " << e.what() << std::endl;
//    }
//    catch (...)
//    {
//        std::cout << "General Exception at Init Filming Logger: " << std::endl;
//    }
//}

McsfFilmingLogger::McsfFilmingLogger( const string& sLoggerName /*= EMPTY_STRING*/, const string& sSourceName/*=EMPTY_STRING*/ ) : m_sLoggerName(sLoggerName), m_sSourceName(sSourceName)
{
//#ifndef  DEVELOP_DEBUG_MODE
    //if client does not use logger source name, set it to be logger name
    if (EMPTY_STRING == m_sSourceName)
    {
        m_sSourceName = m_sLoggerName;
    }
    //InitLogger();
//#endif  //DEVELOP_DEBUG_MODE
}

void McsfFilmingLogger::EraseLogger()
{
//#ifndef DEVELOP_DEBUG_MODE
    //ERASE_LOG_INSTANCE(m_sLoggerName);
//#endif
}

void McsfFilmingLogger::LogInfo( const string& info )
{
#ifdef  DEVELOP_DEBUG_MODE
    OFLOG_INFO(printScuLogger, info);
#endif//#else   //DEVELOP_DEBUG_MODE
    LOG_DEV_INFO_2( m_sSourceName, MCSF_FILMING_LOGGER_UID ) << (info);
//#endif  //DEVELOP_DEBUG_MODE
}

void McsfFilmingLogger::LogWarn( const string& info )
{
#ifdef  DEVELOP_DEBUG_MODE
    OFLOG_WARN(printScuLogger, info); 
#endif//#else   //DEVELOP_DEBUG_MODE
    LOG_DEV_WARNING_2( m_sSourceName, MCSF_FILMING_LOGGER_UID ) << (info);
//#endif  //DEVELOP_DEBUG_MODE
}

void McsfFilmingLogger::LogError( const string& info )
{
#ifdef  DEVELOP_DEBUG_MODE
    OFLOG_ERROR(printScuLogger, info); 
#endif//#else   //DEVELOP_DEBUG_MODE
    LOG_DEV_ERROR_2( m_sSourceName, MCSF_FILMING_LOGGER_UID ) << (info);
//#endif  //DEVELOP_DEBUG_MODE
}


MCSF_FILMING_END_NAMESPACE

