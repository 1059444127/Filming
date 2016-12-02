//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_dcm_print_config.h
///  \brief   parse the printer register *.xml file,get the printer list
///
///  \version 1.0
///  \date    Oct. 24, 2011
///  
//////////////////////////////////////////////////////////////////////////
#ifndef MCSF_DCM_PRINTER_CONFIG_H_
#define MCSF_DCM_PRINTER_CONFIG_H_

#include <vector>

#include "McsfFileParser/mcsf_file_parser_exports.h"
#include "McsfFileParser/mcsf_file_parser_factory.h"

#include "mcsf_dcm_film_session_object.h"

MCSF_FILMING_BEGIN_NAMESPACE
class Mcsf_Filming_Export PrinterConfig
{
public:
    /// \brief constructor
    PrinterConfig();

    /// \brief deconstructor
    ~PrinterConfig(void);

    /// \brief get a printer parameters by printer ID
    McsfDcmFilmSessionObject* GetFilmSessionObject(const int iIndex);

	/// \brief get a printer parameters by printer AE title
	McsfDcmFilmSessionObject* GetFilmSessionObject(const string sAETitle);

    /// \brief get the printer parameters list
    const std::vector<McsfDcmFilmSessionObject>* GetFilmSesionObjectVector();

    /// \brief get the printer list size
    int GetFilmSessionObjectSize();

    /// \brief get the index printer's root storage path,default return the first printer's setting value
    //const std::string& GetPrinterStorageRootPath(const int iIndex=0) const;

private:
    /// \brief get the printerconfig.xml file's path
    void GetPrinterConfigFilePath();

    /// \brief parse the PrinterConfig.xml file,get the printer parameters
    bool ParseConfigFile(const std::string& sConfigFilePath);

    /// \brief the pirnter list
    std::vector<McsfDcmFilmSessionObject>* m_vFilmSessionObjectVector;

    /// \brief File Parse module's interface class
    IFileParser *m_pFileParser;

    /// \brief PrinterConfig.xml file full path
    std::string m_sPrinterConfigFilePath;
};

MCSF_FILMING_END_NAMESPACE
#endif  //_MCSF_DCM_PRINTER_CONFIG_H_
