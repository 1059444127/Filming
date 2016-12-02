//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_filming_auto_config.h
///  \brief   global macro and variable define
///
///  \version 1.0
///  \date    Dec. 26, 2011
///  
//////////////////////////////////////////////////////////////////////////
#ifndef MCSF_FILMING_AUTO_CONFIG_H_
#define MCSF_FILMING_AUTO_CONFIG_H_

#include "mcsf_filming_common.h"
#include "McsfLogger/mcsf_logger.h"
//#include "McsfLogger/mcsf_logger_ex.h"

//#define DEVELOPING

MCSF_FILMING_BEGIN_NAMESPACE

const std::string DEFAULT_AE = "UNITEDIMAGING";
const std::string DEFAULT_LAYOUT = "STANDARD\\1,1";
const std::string DEFAULT_STORAGE_ROOT_PATH = "";
const std::string MCSF_PINTER_CONFIG_FILE_NAME = "config\\filming\\configPrinterConfig.xml";
const std::string MCSF_FILMING_CONFIG_FILE_NAME = "config\\filming\\configMcsfFilming.xml";


const std::string MCSF_FILMING_NAME = "FilmingService";
const std::string FRONT_END = "FE";
const std::string BACK_END = "BE";
const int MCSF_AUTO_FILMING_COMMAND_ID = 7076;
const int MCSF_ADD_FILMING_JOB_COMMAND_ID = 7088;
const int SUSPEND_ALL_PRINT_JOB_COMMAND = 7092;
const int RESUME_ALL_PRINT_JOB_COMMAND = 7093;


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

const std::string MCSF_AUTO_FILMING_LOGGER_NAME = "SW_FILMING_AUTO_DLL";
const std::string MCSF_AUTO_FILMING_LOGGER_SOURCE = "SW_FILMING_AUTO_DLL";
const int MCSF_AUTO_FILMING_LOGGER_UID = 001035005;

//extern McsfFilmingLogger gAutoFilmingLogger;

#define LOG_INFO(Info)              \
    LOG_DEV_INFO_2(            \
    MCSF_AUTO_FILMING_LOGGER_SOURCE,  \
    MCSF_AUTO_FILMING_LOGGER_UID     \
    ) << (Info)                               

#define LOG_WARN(Info)              \
    LOG_DEV_WARNING_2(            \
    MCSF_AUTO_FILMING_LOGGER_SOURCE,  \
    MCSF_AUTO_FILMING_LOGGER_UID     \
    ) << (Info)                               

#define LOG_ERROR(Info)             \
    LOG_DEV_ERROR_2(                  \
    MCSF_AUTO_FILMING_LOGGER_SOURCE,  \
    MCSF_AUTO_FILMING_LOGGER_UID     \
    ) << (Info)                              


MCSF_FILMING_END_NAMESPACE
#endif  //MCSF_FILMING_AUTO_CONFIG_H_

