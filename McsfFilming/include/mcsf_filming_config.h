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

#ifndef MCSF_FILMING_CONFIG_H_
#define MCSF_FILMING_CONFIG_H_

#ifdef DEVELOP_DEBUG_MODE
#include "dcmtk/oflog/oflog.h"
#endif // DEVELOP_DEBUG_MODE

#include "mcsf_filming_common.h"
#include "McsfLogger/mcsf_logger.h"
//#include "McsfLogger/mcsf_logger_ex.h"

MCSF_FILMING_BEGIN_NAMESPACE

const std::string DEFAULT_AE = "UNITEDIMAGING";
const std::string DEFAULT_LAYOUT = "STANDARD\\1,1";
const std::string DEFAULT_STORAGE_ROOT_PATH = "";
const std::string MCSF_PINTER_CONFIG_FILE_NAME = "config\\filming\\PrinterConfig.xml";
const std::string MCSF_FILMING_CONFIG_FILE_NAME = "config\\filming\\McsfFilming.xml";
const std::string MCSF_MISCELLANEOUS_CONFIG_FILE_NAME = "config\\filming\\Miscellaneous.xml";
const std::string FILMING_SERVICE_CONFIG_FILE_NAME = "config\\filming\\FilmingServiceConfig.xml";

enum BORDER_DENSITY_ENUM
{
    BLACK_BORDER,
    WHITE_BORDER
};

enum EMPTY_IMAGE_DENSITY_ENUM
{
    BLACK_EMPTY_DENSITY,
    WHITE_EMPTY_DENSITY
};
const std::string SVCLOGUID_SOURCE = "MCSF/Filming";
const LogTypes::uint64_t SVCLOGUID_ERROR_CONNECTPRINTER = 0x0000320000000006; 
const LogTypes::uint64_t SVCLOGUID_ERROR_COMMUNICATION = 0x0000320000000001;
const LogTypes::uint64_t SVCLOGUID_ERROR_IMGTXT_CONFIG = 0x0000320000000002;
const LogTypes::uint64_t SVCLOGUID_ERROR_RESFILE = 0x0000320000000003;
const LogTypes::uint64_t SVCLOGUID_INFO_UNSUPPORT_IMG = 0x0002320000000004;
const LogTypes::uint64_t SVCLOGUID_WARN_RESOLUTION = 0x0001320000000005;

const std::string MCSF_FILMING_LOGGER_NAME = "SW_FILMING_DLL";
const std::string MCSF_FILMING_LOGGER_SOURCE = "SW_FILMING_DLL";
const int MCSF_FILMING_LOGGER_UID = 001035001;

//extern McsfFilmingLogger gFilmingLogger;

#ifdef DEVELOP_DEBUG_MODE
extern OFLogger gPrintScuLogger;
#endif

//if you want to get the log info on the command console,
//make this macro alive. Otherwise, comment this line.

#ifdef     DEVELOP_DEBUG_MODE           
#define LOG_INFO_FILMING(Info)                  \
    OFLOG_INFO_FILMING(gPrintScuLogger,         \
    Info)                               

#define LOG_WARN_FILMING(Info)                  \
    OFLOG_WARN_FILMING(gPrintScuLogger,         \
    Info)                               

#define LOG_ERROR_FILMING(Info)                  \
	OFLOG_ERROR_FILMING(gPrintScuLogger,         \
	Info)                               

#define LOG_SVC_ERROR_FILMING(Info)                  \
	OFLOG_ERROR_FILMING(gPrintScuLogger,         \
	Info)                               

#else
#define LOG_INFO_FILMING(Info)              \
    LOG_DEV_INFO_2(            \
    MCSF_FILMING_LOGGER_SOURCE,  \
    MCSF_FILMING_LOGGER_UID     \
    ) << (Info)                               

#define LOG_WARN_FILMING(Info)              \
    LOG_DEV_WARNING_2(            \
    MCSF_FILMING_LOGGER_SOURCE,  \
    MCSF_FILMING_LOGGER_UID     \
    ) << (Info)                               

#define LOG_ERROR_FILMING(Info)             \
	LOG_DEV_ERROR_2(                  \
	MCSF_FILMING_LOGGER_SOURCE,  \
	MCSF_FILMING_LOGGER_UID     \
	) << (Info)                              

#define LOG_SVC_ERROR_FILMING(Info)             \
	LOG_SVC_ERROR_2(                  \
	MCSF_FILMING_LOGGER_SOURCE,  \
	MCSF_FILMING_LOGGER_UID     \
	) << (Info)                              

#define LOG_SVC_ERROR_LOGUID(Source, LogUid, Info)             \
	LOG_SVC_ERROR_2(                  \
	Source, LogUid     \
	) << (Info)          

#define LOG_SVC_WARN_FILMING(Info)             \
	LOG_SVC_WARNING_2(                  \
	MCSF_FILMING_LOGGER_SOURCE,  \
	MCSF_FILMING_LOGGER_UID     \
	) << (Info)                              

#endif      //DEVELOP_DEBUG_MODE

MCSF_FILMING_END_NAMESPACE
#endif  //MCSF_FILMING_CONFIG_H_

