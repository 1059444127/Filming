//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_filming_config.h
///  \brief   global macro and variable define
///
///  \version 1.0
///  \date    Oct. 25, 2011
///  
//////////////////////////////////////////////////////////////////////////

#ifndef MCSF_FILMING_DB_CONFIG_H_
#define MCSF_FILMING_DB_CONFIG_H_

#include "mcsf_filming_common.h"
#include "McsfLogger/mcsf_logger.h"
//#include "McsfLogger/mcsf_logger_ex.h"

MCSF_FILMING_BEGIN_NAMESPACE

/// \def define logger macro
const std::string MCSF_FILMING_DB_LOGGER_CONFIG_PATH = "McsfFilmingLog.xml";
const std::string MCSF_FILMING_DATABASE_CONFIG_NAME = "McsfDatabaseConfig.xml";

const std::string MCSF_FILMING_DB_LOGGER_NAME = "SW_FILMING_DB_DLL";
const std::string MCSF_FILMING_DB_LOGGER_SOURCE = "SW_FILMING_DB_DLL";
const int MCSF_FILMING_DB_LOGGER_UID = 001035007;

//extern McsfFilmingLogger gFilmingDBLogger;

#define LOG_INFO_DB(Info)              \
    LOG_DEV_INFO_2(            \
    MCSF_FILMING_DB_LOGGER_SOURCE,  \
    MCSF_FILMING_DB_LOGGER_UID     \
    ) << (Info)                               

#define LOG_WARN_DB(Info)              \
    LOG_DEV_WARNING_2(            \
    MCSF_FILMING_DB_LOGGER_SOURCE,  \
    MCSF_FILMING_DB_LOGGER_UID     \
    ) << (Info)                               

#define LOG_ERROR_DB(Info)             \
    LOG_DEV_ERROR_2(                  \
    MCSF_FILMING_DB_LOGGER_SOURCE,  \
    MCSF_FILMING_DB_LOGGER_UID     \
    ) << (Info)

#define LOG_FUNC_BEGIN_DB            \
    LOG_INFO_DB(std::string("<<<<Begin ") + __FUNCTION__ + "<<<<");

#define LOG_FUNC_END_DB                \
    LOG_INFO_DB(std::string(">>>>End ") + __FUNCTION__ + ">>>>");

#define  LOG_FUNC_EXCEPTION_DB(Info)     \
    LOG_ERROR_DB(std::string("!!!!End ") + __FUNCTION__ + " " + Info + " Abnormally!!!!");

MCSF_FILMING_END_NAMESPACE
#endif  //MCSF_FILMING_DB_CONFIG_H_

