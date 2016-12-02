//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  \author  WANG Hui   mailto:hui.wang@united-imaging.com
///
///  \file mcsf_filming_test_case.h
/// 
///  \brief testing class for common utility used by filming module
///
///  \version 1.0
///  \date    April/10/2012
//////////////////////////////////////////////////////////////////////////

#include "mcsf_filming_logger.h"

#include "mcsf_filming_logger_test_case.h"

MCSF_FILMING_BEGIN_NAMESPACE

McsfFilmingLogger logger("Filming_Utility_Test");

TEST_F(FilmingLoggerTest, Log_Info)
{
    logger.LogInfo(string("an info log") + " test");
}

TEST_F(FilmingLoggerTest, Log_Warn)
{
    logger.LogWarn("a warn log");
}

TEST_F(FilmingLoggerTest, Log_Error)
{
    logger.LogError("an error log");
}

TEST_F(FilmingLoggerTest, Log_Func_Begin)
{
    logger.LogFuncBegin();
}

TEST_F(FilmingLoggerTest, Log_Func_End)
{
    logger.LogFuncEnd();
}

TEST_F(FilmingLoggerTest, Log_Func_Exception)
{
    logger.LogFuncException(EMPTY_STRING);
}

TEST_F(FilmingLoggerTest, Log_Erase_Logger)
{
    logger.EraseLogger();
}

MCSF_FILMING_END_NAMESPACE
