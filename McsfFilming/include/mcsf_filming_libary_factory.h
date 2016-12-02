//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_filming_libary_factory.h
///  \brief   filming libary factory
///
///  \version 1.0
///  \date    Oct. 25, 2011
///  
//////////////////////////////////////////////////////////////////////////
#pragma once
#include "boost/shared_ptr.hpp"                       // for boost shared_ptr
#include "mcsf_filming_libary_interface.h"
#include "mcsf_dcm_print_object_instance.h"

MCSF_FILMING_BEGIN_NAMESPACE

class Mcsf_Filming_Export McsfFilmingLibaryFactory
{
public:
    static McsfFilmingLibaryFactory* Instance(void);

    IFilmingLibary* CreateFilmingLibary(
        McsfPrintJobObject* pPrintJobObject,
        McsfFilmingDB* pFilmingDB);
private:
    McsfFilmingLibaryFactory(void);
    ~McsfFilmingLibaryFactory(void);
	McsfDcmPrintObjectInstance* m_pPrintObjectInstance;
};

MCSF_FILMING_END_NAMESPACE
