//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  WangHui hui.wang@united-imaging.com
///
///  \file    mcsf_miscellaneous_config.h
///  \brief   parse miscellaneous.xml
///
///  \version 1.0
///  \date    April. 21, 2014
///  
//////////////////////////////////////////////////////////////////////////
#ifndef MCSF_MISCELLANEOUS_CONFIG_H_
#define MCSF_MISCELLANEOUS_CONFIG_H_

#include "McsfFileParser/mcsf_file_parser_exports.h"
#include "McsfFileParser/mcsf_file_parser_factory.h"

using namespace std;

MCSF_FILMING_BEGIN_NAMESPACE
class Mcsf_Filming_Export MiscellaneousConfig
{
public:
	/// \brief constructor
	MiscellaneousConfig();

	/// \brief deconstructor
	~MiscellaneousConfig() {}

	/// \brief get media type
	std::string GetMediaType()  { std::cout << "media type : " << m_sMediaType << std::endl; return m_sMediaType;}
	std::string GetFilmDestination() {std::cout << "film destination : " << m_sFilmDestination << std::endl; return m_sFilmDestination;}
	std::string GetFilmSessionLabel() {std::cout << "film session label : " << m_sFilmSessionLabel << std::endl; return m_sFilmSessionLabel;}
	std::string GetFilmPriority() {std::cout << "film priority : " << m_sPriority << std::endl; return m_sPriority;}
	std::string GetOwnerID() {std::cout << "owner id : " << m_sOwnerID << std::endl; return m_sOwnerID;}

private:
	/// \brief get the miscellaneous.xml file's path
	std::string GetMiscellaneousConfigFilePath();

	///// \brief parse the Miscellaneous.xml file,get the parameters
	//bool ParseConfigFile(const std::string& sConfigFilePath);

	///// \brief the pirnter list
	//std::vector<McsfDcmFilmSessionObject>* m_vFilmSessionObjectVector;

	///// \brief File Parse module's interface class
	//IFileParser *m_pFileParser;

	/// \brief PrinterConfig.xml file full path
	std::string m_sMediaType;
	std::string m_sFilmDestination;
	std::string m_sFilmSessionLabel;
	std::string m_sPriority;
	std::string m_sOwnerID;
};

MCSF_FILMING_END_NAMESPACE
#endif  //MCSF_MISCELLANEOUS_CONFIG_H_
