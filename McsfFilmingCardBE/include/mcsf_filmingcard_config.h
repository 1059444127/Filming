//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Wang Hui  hui.wang@united-imaging.com
///
///  \file    mcsf_filmingcard_config.h
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

#define MCSF_FILMING_CONFIG_PATH_TAG_IN_SYSTEM_ENVIRONMENT   "FilmingConfigPath"

#define MCSF_FILMING_CARD_BE_LOGGER_NAME          "SW_FILMING_CARD_BE"
#define MCSF_FILMING_CARD_BE_LOGGER_SOURCE        "SW_FILMING_CARD_BE"


MCSF_FILMING_BEGIN_NAMESPACE

//extern McsfFilmingLogger gFilmingCardBELogger;
const int MCSF_FILMING_CARD_BE_LOGGER_UID = 001035006;

const int ReservedVideoMemoryMBs = 100;

#define LOG_INFO(Info)              \
    LOG_DEV_INFO_2(            \
    MCSF_FILMING_CARD_BE_LOGGER_SOURCE,  \
    MCSF_FILMING_CARD_BE_LOGGER_UID     \
    ) << (Info)                               

#define LOG_WARN(Info)              \
    LOG_DEV_WARNING_2(            \
    MCSF_FILMING_CARD_BE_LOGGER_SOURCE,  \
    MCSF_FILMING_CARD_BE_LOGGER_UID     \
    ) << (Info)                               

#define LOG_ERROR(Info)             \
    LOG_DEV_ERROR_2(                  \
    MCSF_FILMING_CARD_BE_LOGGER_SOURCE,  \
    MCSF_FILMING_CARD_BE_LOGGER_UID     \
    ) << (Info)

enum eFilmingCommandID
{
    //send to PA
    FILMING_COMPLETE_COMMAND                = 3997,

    //Image Command ID
    LOAD_STUDY_COMMAND                      = 7071,
    LOAD_IMAGE_COMMAND                      = 7070,
    SAVE_IMAGE_COMMAND                      = 7073,
    REMOVE_ALL_COMMAND                      = 7074,
    REMOVE_CELL_COMMAND                     = 7075,

    SAVE_EFILMS_COMMAND                     = 7078,
    SAVE_FILMS_COMMAND                      = 7079,

    //Command send to Filming Viewer FE
    COUNT_OF_IMAGES_LOADING_COMMAND = 7097,
    REMOVING_ALL_IMAGES_COMMAND = 7098,
    SHOW_ON_TOP_COMMAND = 7099,

    //Create new viewer Controller
    CREATE_NEW_VIEWER_CONTROLLER = 7100
};

MCSF_FILMING_END_NAMESPACE

#endif          //MCSF_FILMING_CONTAINEE_CONFIG_H_
