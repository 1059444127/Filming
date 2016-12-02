//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_filming_libary_interface.h
///  \brief   filming libary interface
///
///  \version 1.0
///  \date    Oct. 25, 2011
///  
//////////////////////////////////////////////////////////////////////////
#pragma once
#include "mcsf_print_job_object.h"
#include "mcsf_filming_DB.h"

MCSF_FILMING_BEGIN_NAMESPACE

class IFilmingLibary
{
public:
	IFilmingLibary(){};

	virtual ~IFilmingLibary(void){};

	/// \brief init the print object interface class
	//virtual void Initialize() = 0;

	/// \brief create stored print DICOM object and hardcopy grayscale DICOM object
	virtual int CreatePrintObject() = 0;

	/// \brief record the SP & HG files' path to DB.
	//virtual bool InsertPrintObjectToDB() = 0;

	virtual int DoPrint() = 0;

	//virtual bool UpdatePrintDB() = 0;

	//virtual bool DeletePrintFile() = 0;

	virtual void DeletePrintObjectFile() = 0;

	virtual int ConnectPrinter() = 0;

	virtual int DisConnectPrinter() = 0;

	virtual void SetSetFilmBoxTimeOut(int time)  = 0;
};

MCSF_FILMING_END_NAMESPACE
