//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_dcm_print_object_interface.h
///  \brief   parameter init, use McsfDcmPrintObject class to create  
///           hardcopy grayscale object file. and insert DB.
///
///  \version 1.0
///  \date    Oct. 17, 2011
///  
//////////////////////////////////////////////////////////////////////////
#ifndef MCSF_DCM_PRINT_OBJECT_INTERFACE_H_
#define MCSF_DCM_PRINT_OBJECT_INTERFACE_H_

#pragma warning(disable:4267)

#include "mcsf_filming_DB.h"
#include "mcsf_dcm_print_object_base.h"

#include <string>
#include <list>
using namespace std;

/* make sure OS specific configuration is included first */
#include "dcmtk/config/osconfig.h"    

#define INCLUDE_CCTYPE
#include "dcmtk/ofstd/ofstdinc.h"

#ifdef HAVE_GUSI_H
#include <GUSI.h>
#endif

#include "dcmtk/ofstd/ofstream.h"
#include "dcmtk/dcmpstat/dviface.h"
#include "dcmtk/dcmpstat/dvpssp.h"
#include "dcmtk/dcmimgle/dcmimage.h"
#include "dcmtk/dcmdata/cmdlnarg.h"
#include "dcmtk/ofstd/ofcmdln.h"
#include "dcmtk/ofstd/ofconapp.h"
#include "dcmtk/dcmdata/dcuid.h"       /* for dcmtk version name */
#include "dcmtk/ofstd/oflist.h"

#ifdef WITH_ZLIB
#include <zlib.h>        /* for zlibVersion() */
#endif

#include "dcmtk/config/osconfig.h"    /* make sure OS specific configuration is included first */
#include "dcmtk/dcmnet/dcompat.h"     /* compatibility code, needs to be included before dirent.h */

#ifdef HAVE_GUSI_H
#include <GUSI.h>
#endif

#define INCLUDE_CCTYPE
#include "dcmtk/ofstd/ofstdinc.h"

BEGIN_EXTERN_C
    /* This #if code is suggested by the gnu autoconf documentation */
#ifdef HAVE_DIRENT_H
#include <dirent.h>
#define NAMLEN(dirent) strlen((dirent)->d_name)
#else
#define dirent direct
#define NAMELEN(dirent) (dirent)->d_namlen
#ifdef HAVE_SYS_NDIR_H
#include <sys/ndir.h>
#endif
#ifdef HAVE_SYS_DIR_H
#include <sys/dir.h>
#endif
#ifdef HAVE_NDIR_H
#include <ndir.h>
#endif
#endif
#ifdef HAVE_IO_H
#include <io.h>
#endif
#ifdef HAVE_FCNTL_H
#include <fcntl.h>      /* for O_RDONLY */
#endif
    END_EXTERN_C

#include "dcmtk/ofstd/ofstream.h"
#include "dcmtk/dcmpstat/dvpsdef.h"    /* for constants */
#include "dcmtk/dcmpstat/dviface.h"    /* for DVInterface */
#include "dcmtk/ofstd/ofstring.h"   /* for OFString */
#include "dcmtk/ofstd/ofbmanip.h"   /* for OFBitmanipTemplate */
#include "dcmtk/ofstd/ofdatime.h"   /* for OFDateTime */
#include "dcmtk/dcmdata/dcuid.h"      /* for dcmtk version name */
#include "dcmtk/dcmdata/cmdlnarg.h"   /* for prepareCmdLineArgs */
#include "dcmtk/ofstd/ofconapp.h"   /* for OFConsoleApplication */
#include "dcmtk/dcmimgle/dcmimage.h"
#include "dcmtk/dcmpstat/dvpspr.h"
#include "dcmtk/dcmpstat/dvpssp.h"
#include "dcmtk/dcmpstat/dvpshlp.h"     /* for class DVPSHelper */
#include "dcmtk/ofstd/ofstd.h"

#ifdef WITH_OPENSSL
#include "dcmtk/dcmtls/tlstrans.h"
#include "dcmtk/dcmtls/tlslayer.h"
#endif

#ifdef WITH_ZLIB
#include <zlib.h>        /* for zlibVersion() */
#endif

#include <vector>

#include "mcsf_filming_libary_interface.h"
#include "mcsf_filming_DB.h"
//class McsfFilmingDB;

MCSF_FILMING_BEGIN_NAMESPACE

class Mcsf_Filming_Export McsfDcmPrintObjectInstance : 
    public IFilmingLibary,
    public McsfDcmPrintObject
{
public:
    /// \brief constructor
    McsfDcmPrintObjectInstance(McsfPrintJobObject* pPrintJobObject,
        McsfFilmingDB* pFilmingDB);

	void Initialize( McsfPrintJobObject* pPrintJobObject, McsfFilmingDB* pFilmingDB );

    /// \brief deconstructor
    virtual ~McsfDcmPrintObjectInstance(void);

    /// \brief init the print object interface class
    //void Initalize();

    /// \brief create stored print DICOM object and hardcopy grayscale DICOM object
    int CreatePrintObject();

    /// \brief record the SP & HG files' path to DB.
    //bool InsertPrintObjectToDB();

    /// \brief set file list
    //void SetFileList(const std::list<const char*> fileList);
    //void SetFileList(const std::vector<std::string> fileList);

    int DoPrint();

	int ConnectPrinter();
	int DisConnectPrinter();
	void SetSetFilmBoxTimeOut(int time);
    //bool UpdatePrintDB();

    //bool DeletePrintFile();

	/// \brief delete SP,HG file when print completely with success.
	void DeletePrintObjectFile();

    OFCondition ConnectToPrinter();

    OFCondition CreateSession(DVPSStoredPrint& stprint);

    OFCondition DeleteFilmSession(DVPSStoredPrint& stprint);

    OFCondition PrintFilmSession(DVPSStoredPrint& stprint);

    OFCondition ReleaseAssociation(/*DVPSStoredPrint& stprint*/);

    /// \brief save hardcopy grayscale file
    int SaveHGfile();

    /// \brief load print image and mapped PS file.
    OFCondition LoadPrintFile(const char* psFilePath, const char* imageFilePath);

    OFCondition SetFilmBox(DVPSStoredPrint& stprint);
private:
    /// \brief get the McsfFilming.xml file path, parse it 
    void GetStorageRootPath();

    /// \brief parse the McsfFilming.xml, get the 
    ///  <PrintObjectStorgaePath> value
    void ParseFilmingConfig(const std::string& sFilmingConfigPath);

    /// \brief set look up table file
    void SetLut();

    /// \brief set annotation text info
    void SetAnnotation();

    /// \brief save stored print file
    OFCondition SaveSPfile();

	/// set Print Info
    OFCondition SetPrintHandlerInfo();
    //McsfPrintJobObject* m_pPirntJobObject;
    McsfPrintJobObject* m_pPrintJobObject;

    McsfFilmingDB* m_pFilmingDB;

    std::vector<std::string>           m_FileNameList;
    
    OFCondition LoadSPfile(std::string sSPfilePath, DVPSStoredPrint& stprint);

    OFCondition SetAnnotationBox(DVPSStoredPrint& stprint);

    //SP and HG file path
    //std::string m_sSPfilePath;
    //std::vector<std::string> m_sHGfilePathList;

    DVPSPrintMessageHandler printHandler;
};
MCSF_FILMING_END_NAMESPACE
#endif  //MCSF_DCM_PRINT_OBJECT_INTERFACE_H_
