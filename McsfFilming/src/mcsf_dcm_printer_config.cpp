//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  /author  Wang Hui   mailto:hui.wang@united-imaging.com
///
///  /file mcsf_dcm_printer_config.cpp
/// 
///  /brief parse printer setting xml file
///
///  /version 1.0
///  /date    Oct/26/2011
//////////////////////////////////////////////////////////////////////////

#include "boost/lexical_cast.hpp"

#include "mcsf_dcm_printer_config.h"

#include <sstream>

#include "Common/McsfSystemEnvironmentConfig/mcsf_systemenvironment_factory.h"

MCSF_FILMING_BEGIN_NAMESPACE

PrinterConfig::PrinterConfig() : 
m_vFilmSessionObjectVector(NULL), 
m_pFileParser(NULL),
m_sPrinterConfigFilePath("")
{
    try
    {
        m_pFileParser = Mcsf::ConfigParserFactory::Instance()->GetXmlFileParser();
        if (NULL == m_pFileParser)
        {
            LOG_ERROR_FILMING("Can't get Printer FileParser instance");
            return;
        }
        LOG_INFO_FILMING("Get Printer FileParser instance");
        m_pFileParser->Initialize();
        LOG_INFO_FILMING("Printer FileParser Initialized");

        m_vFilmSessionObjectVector = new std::vector<McsfDcmFilmSessionObject>();

        GetPrinterConfigFilePath();

        string sLog = "begin to parse printer config file " + m_sPrinterConfigFilePath;
        LOG_INFO_FILMING(sLog);

        if (!ParseConfigFile(m_sPrinterConfigFilePath))
        {
            LOG_WARN_FILMING("Printer Config File " + m_sPrinterConfigFilePath + " has not been parsed correctly");
        }

        sLog = "end to parse printer config file " + m_sPrinterConfigFilePath;
        LOG_INFO_FILMING(sLog);
    }
    catch (...)
    {
        LOG_WARN_FILMING("exception when initializing FileParser");
    }
}


PrinterConfig::~PrinterConfig(void)
{
    try
    {
        m_pFileParser->Terminate();
        delete m_pFileParser;
        delete m_vFilmSessionObjectVector;
    }
    catch (...)
    {
    }
}

void PrinterConfig::GetPrinterConfigFilePath()
{
    m_sPrinterConfigFilePath = MCSF_PINTER_CONFIG_FILE_NAME;
    return ;
}

//TODO: Has some question about return type, if the DcmFilmSessionObject of iPrinterID is not found, what can I do
McsfDcmFilmSessionObject* PrinterConfig::GetFilmSessionObject( const int iIndex )
{

    int iSessionCount = static_cast<int>(m_vFilmSessionObjectVector->size());
    if(iIndex>=0 && iIndex<iSessionCount)
    {
        return &m_vFilmSessionObjectVector->at(iIndex);
    }

    return NULL;
}

McsfDcmFilmSessionObject* PrinterConfig::GetFilmSessionObject(const string sAETitle )
{
    int iSessionCount = static_cast<int>(m_vFilmSessionObjectVector->size());
    for(int iIndex = 0;iIndex<iSessionCount;iIndex++)
    {
        string printerAETitle = m_vFilmSessionObjectVector->at(iIndex).GetTargetAETitle();
        if(sAETitle == printerAETitle)
        {
            return &m_vFilmSessionObjectVector->at(iIndex);            
        }
    }
    return NULL;
}

const std::vector<McsfDcmFilmSessionObject>* PrinterConfig::GetFilmSesionObjectVector()
{
    return this->m_vFilmSessionObjectVector;
}

int PrinterConfig::GetFilmSessionObjectSize()
{
    return static_cast<int>(this->m_vFilmSessionObjectVector->size());
}

bool PrinterConfig::ParseConfigFile( const std::string& sConfigFilePath )
{//lint -e665

    std::wstring wsConfigFilePath = m_pFileParser->ToWString(sConfigFilePath);
    LOG_INFO_FILMING("Printer config fire " + sConfigFilePath + " is  valid") ;

    if (false == m_pFileParser->OpenFromUserSettingsDir(sConfigFilePath))
    {
        LOG_WARN_FILMING("Printer Config File " + sConfigFilePath + " has been parsed failed") ;
        return false;
    }
    LOG_INFO_FILMING("Printer Config File " + sConfigFilePath + " has been parsed");
    
    vector<wstring> wsPrinterVector;
    if(false == m_pFileParser->GetStringValueByPath(L"Printer/Item",&wsPrinterVector))
    {
        LOG_WARN_FILMING("error when parsing Tag  \"Printer\\Item\" ");
        return false;
    }
    //parse all attributes
    ostringstream os;
    wstring wsValue(L"");
    vector<wstring> wsValueVector;
    //vector<int> iValueVector;
    string sPrinterIDPath;
    wstring wsPrinterIDPath;

    int i=0;
    for (vector<wstring>::const_iterator it = wsPrinterVector.begin(); 
        it != wsPrinterVector.end(); it++, i++)
    {
        LOG_INFO_FILMING  ("Begin to Parse Printer " + boost::lexical_cast<string>(i));
        McsfDcmFilmSessionObject SessionObjectTemp;
        istringstream is(m_pFileParser->FromWString(*it));
        //Get printerID
        is >> SessionObjectTemp.m_sPrinterID;

        //Get all attributes
        os << "/Printer/Item[" << i <<"]";
        sPrinterIDPath = os.str();
        wsPrinterIDPath = m_pFileParser->ToWString(sPrinterIDPath);
        
        string sLogPrinterIDPath = "parsing Tag " + sPrinterIDPath;

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"peerAE", &wsValue))
        {
            LOG_WARN_FILMING( sLogPrinterIDPath + "/peerAE" + " wrong");
        } 
        else
        {
            SessionObjectTemp.m_sPrinterAE = m_pFileParser->FromWString(wsValue);
            LOG_INFO_FILMING( sLogPrinterIDPath + "/peerAE");
        }

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"peerIP", &wsValue))
        {
            LOG_WARN_FILMING( sLogPrinterIDPath + "/peerIP" + " wrong");
        } 
        else
        {
            SessionObjectTemp.m_sPrinterIP = m_pFileParser->FromWString(wsValue);
            LOG_INFO_FILMING( sLogPrinterIDPath + "/peerIP");
        }

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"peerPort", &wsValue))
        {
            LOG_WARN_FILMING(sLogPrinterIDPath + "/peerPort" + " wrong");
        }
        else
        {
            SessionObjectTemp.m_iPrinterPort =std::atoi(m_pFileParser->FromWString(wsValue).c_str());
            LOG_INFO_FILMING(sLogPrinterIDPath + "/peerPort");
        }

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"description", &wsValue))
        {
            LOG_WARN_FILMING(sLogPrinterIDPath + "/description" + " wrong");
        }
        else
        {
            SessionObjectTemp.m_sPrinterDescription = m_pFileParser->FromWString(wsValue);
            LOG_INFO_FILMING(sLogPrinterIDPath + "/description");
        }

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"BorderDensity", &wsValue))
        {
            LOG_WARN_FILMING(sLogPrinterIDPath + "/BorderDensity" + " wrong");
        }
        else
        {
            SessionObjectTemp.m_sBorderDensity = m_pFileParser->FromWString(wsValue);
            LOG_INFO_FILMING(sLogPrinterIDPath + "/BorderDensity");
        }

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"EmptyImageDensity", &wsValue))
        {
            LOG_WARN_FILMING(sLogPrinterIDPath + "/EmptyImageDensity" + " wrong");
        }
        else
        {
            SessionObjectTemp.m_sEmptyImageDensity = m_pFileParser->FromWString(wsValue);
            LOG_INFO_FILMING(sLogPrinterIDPath + "/EmptyImageDensity");
        }


        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"FilmDestination", &wsValue))
        {
            LOG_WARN_FILMING(sLogPrinterIDPath + "/FilmDestination" + " wrong");
        }
        else
        {
            SessionObjectTemp.m_sFilmDestination = m_pFileParser->FromWString(wsValue);
            LOG_INFO_FILMING(sLogPrinterIDPath + "/FilmDestination");
        }

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"FilmSizeID", &wsValue))
        {
            LOG_WARN_FILMING(sLogPrinterIDPath + "/FilmSizeID" + " wrong");
        }
        else
        {
            SessionObjectTemp.m_sFilmSize = m_pFileParser->FromWString(wsValue);
            LOG_INFO_FILMING(sLogPrinterIDPath + "/FilmSizeID");
        }


        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"ImplicitOnly", &wsValue))
        {
            LOG_WARN_FILMING(sLogPrinterIDPath + "/ImplicitOnly" + " wrong");
        }
        else
        {
            is.str(m_pFileParser->FromWString(wsValue));
            is >> SessionObjectTemp.m_bImplicitOnly;
            LOG_INFO_FILMING(sLogPrinterIDPath + "/ImplicitOnly");
        }

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"MagnificationType", &wsValue))
        {
            LOG_WARN_FILMING(sLogPrinterIDPath + "/MagnificationType" + " wrong");
        }
        else
        {
            SessionObjectTemp.m_sMagnification = m_pFileParser->FromWString(wsValue);
            LOG_INFO_FILMING(sLogPrinterIDPath + "/MagnificationType");
        }

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"MaxDensity", &wsValue))
        {
            LOG_WARN_FILMING(sLogPrinterIDPath + "/MaxDensity" + " wrong");
        }
        else
        {
            SessionObjectTemp.m_iMaxDensity = atoi(m_pFileParser->FromWString(wsValue).c_str());
            LOG_INFO_FILMING(sLogPrinterIDPath + "/MaxDensity");
        }

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"MaxPDU", &wsValue))
        {
            LOG_WARN_FILMING(sLogPrinterIDPath + "/MaxPDU" + " wrong");
        }
        else
        {
            SessionObjectTemp.m_lMaxPDU = static_cast<unsigned long>(atoi(m_pFileParser->FromWString(wsValue).c_str()));
            LOG_INFO_FILMING(sLogPrinterIDPath + "/MaxPDU");
        }

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"MediumType", &wsValue))
        {
            LOG_WARN_FILMING(sLogPrinterIDPath + "/MediumType" + " wrong");
        }
        else
        {
            SessionObjectTemp.m_sMediumType = m_pFileParser->FromWString(wsValue);
            LOG_INFO_FILMING(sLogPrinterIDPath + "/MediumType");
        }

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"MinDensity", &wsValue))
        {
            LOG_WARN_FILMING(sLogPrinterIDPath + "/MinDensity" + " wrong");
        }
        else
        {
            SessionObjectTemp.m_iMinDensity = atoi(m_pFileParser->FromWString(wsValue).c_str());
            LOG_INFO_FILMING(sLogPrinterIDPath + "/MinDensity");
        }

        if (false == m_pFileParser->GetAttributeStringValue(wsPrinterIDPath,L"PresentationLUTinFilmSession", &wsValue))
        {        
            SessionObjectTemp.m_bPresentationLUTinFilmSession = false;
            LOG_WARN_FILMING(sLogPrinterIDPath + "/PresentationLUTinFilmSession");
        }
        else
        {
            is.str(m_pFileParser->FromWString(wsValue));
            is >>SessionObjectTemp.m_bPresentationLUTinFilmSession;
        }
        m_vFilmSessionObjectVector->push_back(SessionObjectTemp);
        os.str("");

        LOG_INFO_FILMING("end to parse Printer " + boost::lexical_cast<string>(i));
    }
    std::vector<std::wstring> ().swap(wsPrinterVector);
    return true;
}

MCSF_FILMING_END_NAMESPACE
