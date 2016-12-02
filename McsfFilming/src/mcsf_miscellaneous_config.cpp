//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  /author  Wang Hui   mailto:hui.wang@united-imaging.com
///
///  /file mcsf_miscellaneous_config.cpp
/// 
///  /brief parse miscellaneous xml file
///
///  /version 1.0
///  /date    Apr/21/2014
//////////////////////////////////////////////////////////////////////////

#include "Common/McsfSystemEnvironmentConfig/mcsf_systemenvironment_factory.h"

#include "mcsf_miscellaneous_config.h"

MCSF_FILMING_BEGIN_NAMESPACE

MiscellaneousConfig::MiscellaneousConfig() 
		//:	m_sMediaType("CLEAR FILM")
		//,	m_sFilmDestination("BIN_1")
		//,	m_sFilmSessionLabel("PRINT")
		//,	m_sPriority("MED")
		//,	m_sOwnerID("UIH")
{
	try
	{
		IFileParser* pFileParser = Mcsf::ConfigParserFactory::Instance()->GetXmlFileParser();
		if (NULL == pFileParser)
		{
			LOG_ERROR_FILMING("Can't get FileParser instance");
			return;
		}
		LOG_INFO_FILMING("Get FileParser instance");
		pFileParser->Initialize();
		LOG_INFO_FILMING("Printer FileParser Initialized");

		string sMiscellaneousConfigFilePath = GetMiscellaneousConfigFilePath();

		string sLog = "begin to parse miscellaneous config file " + sMiscellaneousConfigFilePath;
		LOG_INFO_FILMING(sLog);

		std::wstring wsConfigFilePath = pFileParser->ToWString(sMiscellaneousConfigFilePath);

		if (false == pFileParser->Validate(wsConfigFilePath))
		{
			LOG_WARN_FILMING("Miscellaneous Config File " + sMiscellaneousConfigFilePath + " is not valid") ;
			return ;
		}
		LOG_INFO_FILMING("Miscellaneous config fire " + sMiscellaneousConfigFilePath + " is  valid") ;

		if (false == pFileParser->ParseByURI(wsConfigFilePath))
		{
			LOG_WARN_FILMING("Printer Config File " + sMiscellaneousConfigFilePath + " has been parsed failed") ;
			return ;
		}
		LOG_INFO_FILMING("Printer Config File " + sMiscellaneousConfigFilePath + " has been parsed");

		wstring wsTemp(L"");

		//parse media type
		if (false == pFileParser->GetStringValueByTag(L"MediumType", &wsTemp) )
		{
			LOG_WARN_FILMING("error when parsing Tag  \"MediumType\" ");     
		}
		LOG_WARN_FILMING("parsed Tag  \"MediumType\" ");     
		
		m_sMediaType = pFileParser->FromWString(wsTemp);


		//parse Film Destination
		if (false == pFileParser->GetStringValueByTag(L"FilmDestination", &wsTemp) )
		{
			LOG_WARN_FILMING("error when parsing Tag  \"FilmDestination\" ");     
		}
		LOG_WARN_FILMING("parsed Tag  \"FilmDestination\" ");     

		m_sFilmDestination = pFileParser->FromWString(wsTemp);

		//parse Film Session Label
		if (false == pFileParser->GetStringValueByTag(L"FilmSessionLabel", &wsTemp) )
		{
			LOG_WARN_FILMING("error when parsing Tag  \"FilmSessionLabel\" ");     
		}
		LOG_WARN_FILMING("parsed Tag  \"FilmSessionLabel\" ");     

		m_sFilmSessionLabel = pFileParser->FromWString(wsTemp);

		//parse priority
		if (false == pFileParser->GetStringValueByTag(L"Priority", &wsTemp) )
		{
			LOG_WARN_FILMING("error when parsing Tag  \"Priority\" ");     
		}
		LOG_WARN_FILMING("parsed Tag  \"Priority\" ");     

		m_sPriority = pFileParser->FromWString(wsTemp);

		//parse owner ID
		if (false == pFileParser->GetStringValueByTag(L"OwnerID", &wsTemp) )
		{
			LOG_WARN_FILMING("error when parsing Tag  \"OwnerID\" ");     
		}
		LOG_WARN_FILMING("parsed Tag  \"OwnerID\" ");     

		m_sOwnerID = pFileParser->FromWString(wsTemp);


		//sLog = "end to parse printer config file " + m_sPrinterConfigFilePath;
		//LOG_INFO_FILMING(sLog);
	}
	catch (...)
	{
		LOG_WARN_FILMING("exception when initializing FileParser");
	}
}

std::string MiscellaneousConfig::GetMiscellaneousConfigFilePath()
{
	std::string sMiscellaneousConfigFilePath = "";
	ISystemEnvironmentConfig *pSysConfig = 
		ConfigSystemEnvironmentFactory::Instance()->GetSystemEnvironment();
	if (NULL != pSysConfig)
	{
		sMiscellaneousConfigFilePath = pSysConfig->GetApplicationPath("FilmingConfigPath");
		LOG_INFO_FILMING("Filming config file path is: "+sMiscellaneousConfigFilePath);
	}
	else
	{
		LOG_ERROR_FILMING("ISystemEnvironmentConfig get Instance failed!");
		return sMiscellaneousConfigFilePath;
	}

	if(sMiscellaneousConfigFilePath.empty())
	{
		LOG_ERROR_FILMING("GetPrinterConfigFilePath function get null path by using \
						  GetApplicationPath('FilmingConfigPath'),please check!!");
	}

	sMiscellaneousConfigFilePath += MCSF_MISCELLANEOUS_CONFIG_FILE_NAME;

	std::string sLogStr = "PrinterConfig.xml's full path is: "+sMiscellaneousConfigFilePath;
	LOG_INFO_FILMING(sLogStr);

	delete pSysConfig;
	pSysConfig = NULL;

	return sMiscellaneousConfigFilePath;
}


MCSF_FILMING_END_NAMESPACE