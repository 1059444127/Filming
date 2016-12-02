////////////////////////////////////////////////////////////////////////////////
/// Copyright (c) Shanghai United Imaging Healthcare Inc., 2012
///    All rights reserved.
///
///    /author    WANG Hui mailto:hui.wang@united-imaging.com
///
///    /file     mcsf_filming_common.h
///
///    /brief   common macro and definitions used by all filming c++ projects
///
///    /version    1.0
///    /date 10/4/2012
////////////////////////////////////////////////////////////////////////////////

#ifndef mcsf_filming_common_h__
#define mcsf_filming_common_h__

#include <string>

#ifdef MCSF_BUILD_FILMING_LIB
#define Mcsf_Filming_Export __declspec (dllexport)
#else //!defined(MCSF_BUILD_FILMING_DB_LIB)
#define Mcsf_Filming_Export __declspec (dllimport)
#endif

#ifndef MCSF_FILMING_VERSIONED_NAMESPACE
#define MCSF_FILMING_NAMESPACE       Mcsf
#define MCSF_FILMING_BEGIN_NAMESPACE \
    namespace MCSF_FILMING_NAMESPACE {    /* begin namespace Mcsf */
#define MCSF_FILMING_END_NAMESPACE   }    /* end namespace Mcsf   */
#endif

/// \def define disallowing copy and assign operation
#ifndef MCSF_FILMING_DISALLOW_COPY_AND_ASSIGN_DEFINED
#define MCSF_FILMING_DISALLOW_COPY_AND_ASSIGN_DEFINED
#define MCSF_FILMING_DISALLOW_COPY(classname) \
    classname(const classname & );
#define MCSF_FILMING_DISALLOW_ASSIGN(classname) \
    void operator = (const classname & );
#define MCSF_FILMING_DISALLOW_COPY_AND_ASSIGN(classname) \
    MCSF_FILMING_DISALLOW_COPY  (classname)\
    MCSF_FILMING_DISALLOW_ASSIGN(classname)
#endif



MCSF_FILMING_BEGIN_NAMESPACE

const std::string EMPTY_STRING = "";

enum LAYOUT_TYPE_ENUM
{
    STANDARD,
    ROW,
    COL
};

enum PRINT_PRIORITY_ENUM
{
    HIGH = 1, 
    MEDIUM,
    LOW
};

MCSF_FILMING_END_NAMESPACE

#endif // mcsf_filming_common_h__
