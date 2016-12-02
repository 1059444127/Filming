//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  /author  Wang Hui   mailto:pengxuan.mu@united-imaging.com
///
///  /file mcsf_filing_libary_factory.cpp
/// 
///  /brief filming libary factory
///
///  /version 1.0
///  /date    Oct/26/2011
//////////////////////////////////////////////////////////////////////////

#include "mcsf_filming_libary_factory.h"

MCSF_FILMING_BEGIN_NAMESPACE
McsfFilmingLibaryFactory::McsfFilmingLibaryFactory(void)
{
}

McsfFilmingLibaryFactory::~McsfFilmingLibaryFactory(void)
{
}

McsfFilmingLibaryFactory* McsfFilmingLibaryFactory::Instance(void)
{
    static McsfFilmingLibaryFactory obj;
    return &obj;
}

IFilmingLibary* McsfFilmingLibaryFactory::CreateFilmingLibary(
    McsfPrintJobObject* pPrintJobObject,
    McsfFilmingDB* pFilmingDB)
{
    //McsfDcmPrintObjectInstance* pPrintObjectInstance = NULL;
    try
    {
		if(m_pPrintObjectInstance == NULL)
			m_pPrintObjectInstance = new McsfDcmPrintObjectInstance(pPrintJobObject,pFilmingDB);
		else
        {
		    m_pPrintObjectInstance->Initialize(pPrintJobObject, pFilmingDB);
            m_pPrintObjectInstance->UpdatePrintJobObject(pPrintJobObject);
        }
    }
    catch (...)
    {
        if(NULL != m_pPrintObjectInstance)
        {
            delete m_pPrintObjectInstance;
            m_pPrintObjectInstance = NULL;
        }
        
        LOG_ERROR_FILMING("new DICOMConvertorInstance bad!");
    }

    //IDICOMConvertorPtr pDICOMConvertorInstance(pConvertor);
    return m_pPrintObjectInstance;
}

MCSF_FILMING_END_NAMESPACE
