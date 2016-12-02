//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_filming_containee_config.h
///  \brief   the filming containee config  
///
///  \version 1.0
///  \date    Nev. 3, 2011
///  
//////////////////////////////////////////////////////////////////////////
#ifndef MCSF_FILMING_CONTAINEE_CONFIG_H_
#define MCSF_FILMING_CONTAINEE_CONFIG_H_

#include "mcsf_filming_common.h"
#include "McsfLogger/mcsf_logger.h"
//#include "McsfLogger/mcsf_logger_ex.h"

#include "McsfJobManagerInfo/mcsf_jobmanager_info_wrapper.h"

MCSF_FILMING_BEGIN_NAMESPACE

    using namespace JobManager;

#define MCSF_FILMING_CONFIG_PATH_TAG_IN_SYSTEM_ENVIRONMENT   "FilmingConfigPath"

#define MCSF_FILMING_SEVICE_LOGGER_NAME          "SW_FILMING_SERVICE_CONTAINEE"
#define MCSF_FILMING_SEVICE_LOGGER_SOURCE        "SW_FILMING_SERVICE_CONTAINEE"

//#define MAINFRAME_NAME "MainFrame_FE"

//extern McsfFilmingLogger gFilmingServiceLogger;
const int MCSF_FILMING_CONTAINEE_LOGGER_UID = 001035010;

const int URGENT_JOB_PRIORITY = 1;

#define LOG_INFO(Info)              \
    LOG_DEV_INFO_2(            \
    MCSF_FILMING_SEVICE_LOGGER_SOURCE,  \
    MCSF_FILMING_CONTAINEE_LOGGER_UID     \
    ) << (Info)                               

#define LOG_WARN(Info)              \
    LOG_DEV_WARNING_2(            \
    MCSF_FILMING_SEVICE_LOGGER_SOURCE,  \
    MCSF_FILMING_CONTAINEE_LOGGER_UID     \
    ) << (Info)                               

#define LOG_ERROR(Info)             \
    LOG_DEV_ERROR_2(                  \
    MCSF_FILMING_SEVICE_LOGGER_SOURCE,  \
    MCSF_FILMING_CONTAINEE_LOGGER_UID     \
    ) << (Info)

#define LOG_FUNC_BEGIN            \
    LOG_INFO(std::string("<<<<Begin ") + __FUNCTION__ + "<<<<");

#define LOG_FUNC_END                \
    LOG_INFO(std::string(">>>>End ") + __FUNCTION__ + ">>>>");

#define  LOG_FUNC_EXCEPTION(Info)     \
    LOG_ERROR(std::string("!!!!End ") + __FUNCTION__ + " " + Info + " Abnormally!!!!");

enum eFilmingCommandID
{
    //send to PA
    FILMING_COMPLETE_COMMAND                = 3997,

    //Filming Command ID
    ADJUST_PRIORITY_OF_PRINT_JOB_COMMAND    = 7081,
    DELETE_PRINT_JOB_COMMAND                = 7082,//
    GET_PRINTER_CONFIG_COMMAND              = 7083,
    QUERY_HISTORY_PRINT_JOB_COMMAND         = 7084,
    REPRINT_COMMAND                         = 7085,//
    PAUSE_PRINT_JOB_COMMAND                 = 7086,
    RESUME_PRINT_JOB_COMMAND                = 7087,
    ADD_PRINT_JOB_COMMAND                   = 7088,//
    QUERY_CURRENT_PRINT_JOBS_COMMAND        = 7089,
    PAUSE_PRINT_COMMAND                     = 7090,
    RESUME_PRINT_COMMAND                    = 7091,
    SUSPEND_ALL_PRINT_JOB_COMMAND            = 7092,
    RESUME_ALL_PRINT_JOB_COMMAND            = 7093,

    SAVE_EFILMS_COMMAND                     = 7078,
	SAVE_EFILMS_COMMON_SAVE                 = 16000,

	//send back to filming card FE
	SAVE_EFILM_COMPLETE_COMMAND				= 7299,
    TRANSFER_TO_PRINTER                     = 15058,//×ª´òÓ¡»ú

    //listen to mainframe
    //ToMainFrameCmd = 15050,
    //FromMainFramePauseCmd = 15051,
    //FromMainFrameContinueCmd = 15052,
    //FromMainFrameRestartCmd = 15053,
    //FromMainFrameDeleteCmd = 15054,
    //FromMainFrameUrgentCmd = 15055,
    //FromMainFrameRefreshCmd = 15056,
    //FromMainFrameStopCmd = 15057

};


#define FILMING_JOB_LIST_VOLUME 200

#define MIN_PRIORITY FILMING_JOB_LIST_VOLUME*2

#define MAX_PRIORITY 1

#define TIME_FOR_ONE_FILM_SENDING_TO_PRINTER 60		//unit: seconds

#define MIXED_FILM_PATIENT_NAME "Mixed"

MCSF_FILMING_END_NAMESPACE

#endif          //MCSF_FILMING_CONTAINEE_CONFIG_H_
