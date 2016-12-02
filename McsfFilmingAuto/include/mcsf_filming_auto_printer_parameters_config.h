//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_dicom_service_config.h
///  \brief   ArchivingConfig.xml parse 
///
///  \version 1.0
///  \date    Nov. 27, 2011
///  
//////////////////////////////////////////////////////////////////////////
#ifndef MCSF_FILMING_AUTO_PRINTIER_PARAM_CONFIG_H
#define MCSF_FILMING_AUTO_PRINTIER_PARAM_CONFIG_H

#include <string>

#include "McsfFileParser/mcsf_file_parser_exports.h"
#include "McsfFileParser/mcsf_file_parser_factory.h"

#include "mcsf_filming_auto_config.h"

MCSF_FILMING_BEGIN_NAMESPACE
struct McsfDicomPacsNodeEntry
{
    std::string sPeerAE;
    int iPeerPort;
    std::string sPeerIP;
    std::string sStorageArea;
    int iAccess;
};

class Mcsf_Filming_Export McsfPrinterParametersConfig
{
public:
    McsfPrinterParametersConfig();

	~McsfPrinterParametersConfig();

    //int GetOurNodeParameters(std::string* sOurAE, int* iTimeout);

    const std::string& GetDeaultNodeAE();

    int GetPacsNodeParametersByAE(const std::string& sNodeAE,
        std::string* sIP, unsigned int* iPort);

    const std::string& GetOurAE();

    bool ParseFilmingConfigFile( const std::string& sConfigFilePath );

    bool ParsePrinterConfigFile( const std::string& sConfigFilePath );
private:
    int Init();

    void GetFilmingConfigFilePath();	
private:
    std::string m_sFilmingConfigFilePath;
    std::string m_sPrinterConfigFilePath;

    std::string m_sOurAE;
    int m_iOurPort;
    std::string m_sCurrentAE;
    int m_iTimeout;

    std::string m_sImageFolder;

    std::vector<McsfDicomPacsNodeEntry> m_vPacsNodeEntryVector;

    IFileParser *m_pFileParser;
};
MCSF_FILMING_END_NAMESPACE
#endif  //MCSF_FILMING_AUTO_PRINTIER_PARAM_CONFIG_H
