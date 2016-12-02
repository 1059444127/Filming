//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_filming_auto_printer_parameters_config.cpp
///  \brief   filming auto printer parameter config
///
///  \version 1.0
///  \date    Dec. 26, 2011
///  
//////////////////////////////////////////////////////////////////////////

#include "mcsf_filming_auto_printer_parameters_config.h"

#include "Common/McsfSystemEnvironmentConfig/mcsf_systemenvironment_factory.h"

#include <boost/shared_ptr.hpp>

MCSF_FILMING_BEGIN_NAMESPACE
	
std::string GetSysFilmingRootConfigFilePath()
{
	ISystemEnvironmentConfig * pSysConfig = ConfigSystemEnvironmentFactory::Instance()->GetSystemEnvironment();
	if (pSysConfig != NULL)
	{
		boost::shared_ptr<ISystemEnvironmentConfig> SysConfig(pSysConfig);
		std::string sFilmingRootConfigFilePath = SysConfig->GetApplicationPath("FilmingConfigPath");

		return sFilmingRootConfigFilePath;
	}		
	else
	{
		return "";
	}
}	
std::string g_SysFilmingRootConfigFilePath = GetSysFilmingRootConfigFilePath();

McsfPrinterParametersConfig::McsfPrinterParametersConfig()
: m_iOurPort(0), m_iTimeout(30), m_pFileParser(NULL), m_sCurrentAE(EMPTY_STRING),
m_sFilmingConfigFilePath(EMPTY_STRING), m_sImageFolder(EMPTY_STRING), m_sOurAE(EMPTY_STRING),
m_sPrinterConfigFilePath(EMPTY_STRING)
{
    if( -1 == Init() )
    {
        LOG_ERROR("Initialization failure");
    }
}

void McsfPrinterParametersConfig::GetFilmingConfigFilePath()
{
    m_sFilmingConfigFilePath = MCSF_FILMING_CONFIG_FILE_NAME;
    m_sPrinterConfigFilePath = MCSF_PINTER_CONFIG_FILE_NAME;
    return ;
}

int McsfPrinterParametersConfig::Init()
{
    try
    {
        //step1: get the Archiving config file full path
        GetFilmingConfigFilePath();

        //step 2: parse this file by using FileParser.dll
        m_pFileParser = Mcsf::ConfigParserFactory::Instance()->GetXmlFileParser();
        if (NULL == m_pFileParser)
        {
            LOG_ERROR("Can't get FileParser instance");
            return -1;
        }
        m_pFileParser->Initialize();

        std::string sLog = "begin to parse filming config file " + m_sFilmingConfigFilePath;
        LOG_INFO(sLog);
        if (!ParseFilmingConfigFile(m_sFilmingConfigFilePath))
        {
            sLog="McsfFilming.xml parse Failed";
            LOG_WARN(sLog);
        }
        if (!ParsePrinterConfigFile(m_sPrinterConfigFilePath))
        {
            sLog="PriterConfig.xml parse Failed";
            LOG_WARN(sLog);
        }
    }
    catch (...)
    {
        LOG_ERROR(" exception when initializing FileParser");
        return -1;
    }
    return 0;
}

bool McsfPrinterParametersConfig::ParseFilmingConfigFile( const std::string& sConfigFilePath )
{
    std::wstring wsConfigFilePath = m_pFileParser->ToWString(sConfigFilePath);

    std::string sLog;

    if (false == m_pFileParser->OpenFromUserSettingsDir(wsConfigFilePath))
    {
        LOG_WARN("Printer Config File " + sConfigFilePath + " has been parsed failed") ;
        return false;
    }

    //----------get CurrentParameters value------------------
    std::wstring wsOurAE;
    std::string sLogNodeIDPath = " error when parsing Tag: OurAE";
    if (false == m_pFileParser->GetStringValueByTag(L"OurAE", &wsOurAE))
    {	
        LOG_WARN( sLog );
    } 
    else
    {
        m_sOurAE= m_pFileParser->FromWString(wsOurAE);
    }

    //----------get CurrentParameters value------------------
    std::wstring wsDefaultAE;
    sLogNodeIDPath = " error when parsing Tag: DefaultPeerAE";
    if (false == m_pFileParser->GetStringValueByTag(L"DefaultPeerAE", &wsDefaultAE))
    {	
        LOG_WARN( sLog );
    } 
    else
    {
        m_sCurrentAE= m_pFileParser->FromWString(wsDefaultAE);
    }

    //----------get CurrentParameters value------------------
    sLogNodeIDPath = " error when parsing Tag: Timeout";
    if (false == m_pFileParser->GetIntValueByTag(L"Timeout", &m_iTimeout))
    {	
        LOG_WARN( sLog );
    } 

    return true;
}

bool McsfPrinterParametersConfig::ParsePrinterConfigFile( const std::string& sConfigFilePath )
{
    std::wstring wsConfigFilePath = m_pFileParser->ToWString(sConfigFilePath);
    std::string sLog;

    if (false ==m_pFileParser->OpenFromUserSettingsDir(wsConfigFilePath))
    {
        sLog = "Printer Config File " + sConfigFilePath + " has been parsed failed";
        LOG_WARN(sLog);
        return false;
    }

    //parse all configure items
    vector<wstring> wsPrinterVector;
    if(false == m_pFileParser->GetStringValueByPath(L"Printer/Item",&wsPrinterVector))
    {
        LOG_WARN("error when parsing Tag  \"Printer\\Item\" ");
        return false;
    }

    //parse all attributes
    std::ostringstream os;
    std::wstring wsValue(L"");
    std::vector<std::wstring> wsValueVector;
    //vector<int> iValueVector;
    std::string sNodeIDPath;
    std::wstring wsNodeIDPath;


    //bool bValue;
    int i=0;
    for (std::vector<std::wstring>::const_iterator it = wsPrinterVector.begin(); 
        it != wsPrinterVector.end(); it++, i++)
    {
        McsfDicomPacsNodeEntry pacsNodeEntry;

        //Get all attributes
        os <<"/Printer/Item[" << i <<"]";
        sNodeIDPath = os.str();
        wsNodeIDPath = m_pFileParser->ToWString(sNodeIDPath);

        //----------get PeerAE value------------------
        std::string sLogNodeIDPath = " error when parsing Tag " + sNodeIDPath;

        sLog = sLogNodeIDPath + "/peerAE";
        if (false == m_pFileParser->GetAttributeStringValue(wsNodeIDPath,L"peerAE",&wsValue))
        {
            LOG_WARN( sLog );
        } 
        else
        {
            pacsNodeEntry.sPeerAE= m_pFileParser->FromWString(wsValue);
        }

        //----------get PeerPort value------------------
        sLog = sLogNodeIDPath + "/peerPort";
        if (false == m_pFileParser->GetAttributeStringValue(wsNodeIDPath,L"peerPort", &wsValue))
        {
            LOG_WARN(sLog);
        } 
        else
        {
            pacsNodeEntry.iPeerPort = std::atoi(m_pFileParser->FromWString(wsValue).c_str());
        }

        //----------get PeerIP value------------------
        sLogNodeIDPath = " error when parsing Tag " + sNodeIDPath;
        sLog = sLogNodeIDPath + "/peerIP";
        if (false == m_pFileParser->GetAttributeStringValue(wsNodeIDPath,L"peerIP", &wsValue))
        {	
            LOG_WARN( sLog );
        } 
        else
        {
            pacsNodeEntry.sPeerIP= m_pFileParser->FromWString(wsValue);
        }

        m_vPacsNodeEntryVector.push_back(pacsNodeEntry);
        os.str("");
    }
	std::vector<std::wstring> ().swap(wsPrinterVector);
    return true;
}

const std::string& McsfPrinterParametersConfig::GetDeaultNodeAE()
{
    return m_sCurrentAE;
}

const std::string& McsfPrinterParametersConfig::GetOurAE()
{
    return m_sOurAE;
}

//int McsfPrinterParametersConfig::GetTimeOut()
//{
//    return m_iTimeout;
//}

int McsfPrinterParametersConfig::GetPacsNodeParametersByAE(const std::string& sNodeAE,
    std::string* sIP, unsigned int* iPort)
{
    int iPeerNodeCount = m_vPacsNodeEntryVector.size();
    for(int i=0; i< iPeerNodeCount; i++)
    {
        if(sNodeAE == m_vPacsNodeEntryVector[i].sPeerAE)
        {
            if(sIP != NULL)
            {
                *sIP = m_vPacsNodeEntryVector[i].sPeerIP;
                //lint -e665
                LOG_INFO("Peer IP:"+*sIP);
            }   //lint +e665
            if(iPort != NULL)
            {
                *iPort = m_vPacsNodeEntryVector[i].iPeerPort;
            }

            return 0;
        }
    }
    //lint -e665
    LOG_ERROR("Can't find Node:"+sNodeAE+"from config file!");
    //lint +e665
    return -1;
}
McsfPrinterParametersConfig::~McsfPrinterParametersConfig()
{	
	if (!m_vPacsNodeEntryVector.empty())
	{
		std::vector<McsfDicomPacsNodeEntry>().swap(m_vPacsNodeEntryVector);
	}	
	m_pFileParser->Terminate();
}
MCSF_FILMING_END_NAMESPACE
