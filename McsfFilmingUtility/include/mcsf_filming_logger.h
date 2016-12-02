////////////////////////////////////////////////////////////////////////////////
/// Copyright (c) Shanghai United Imaging Healthcare Inc., 2012
///    All rights reserved.
///
///    /author    WANG Hui mailto:hui.wang@united-imaging.com
///
///    /file     mcsf_filming_logger.h
///
///    /brief   logger wrapper for all c++ filming projects
///
///    /version    1.0
///    /date 10/4/2012
/////////////////////////////////////////////////////////////////////////////////

#ifndef MCSF_FILMING_LOGGER_H__
#define MCSF_FILMING_LOGGER_H__

using namespace std;

#include <string>
#include "mcsf_filming_common.h"

#include "McsfLogger/mcsf_logger.h"
//#include "McsfLogger/mcsf_logger_ex.h"

MCSF_FILMING_BEGIN_NAMESPACE

class Mcsf_Filming_Export McsfFilmingLogger
{
public:
    /////////////////////////////////////////////////////////////////
    ///  \brief     Filming Logger constructor
    ///
    ///  \param[in]    sLoggerName  
    ///  \param[out]   sSourceName
    ///  \return       None
    ///  \pre \e    put it in the global region or in a constructor
    /////////////////////////////////////////////////////////////////
    McsfFilmingLogger(const std::string& sLoggerName = EMPTY_STRING, const std::string& sSourceName=EMPTY_STRING);

    void EraseLogger();

    void LogInfo(const std::string& info);

    void LogWarn(const std::string& info);

    void LogError(const std::string& info);

    void LogFuncBegin()
    {
        LogInfo(std::string("<<<<Begin ") + __FUNCTION__ + "<<<<");
    }

    void LogFuncEnd()
    {
        LogInfo(std::string(">>>>End ") + __FUNCTION__ + ">>>>");
    }

    void LogFuncException(const string& info)
    {
        LogError(std::string("!!!!End ") + __FUNCTION__ + " " + info + " Abnormally!!!!");
    }

private:

    std::string m_sLoggerName;

    std::string m_sSourceName;

    //later will be extended
    static const int MCSF_FILMING_LOGGER_UID = 001035001;

    static const std::string MCSF_FILMING_BE_LOGGER_CONFIG_PATH;

    void InitLogger();

public:
};

MCSF_FILMING_END_NAMESPACE

#endif // MCSF_FILMING_LOGGER_H__