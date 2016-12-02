//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_filming_auto_job_creater.cpp
///  \brief   define the filming job instance initialize function
///
///  \version 1.0
///  \date    Dec. 26, 2011
///  
//////////////////////////////////////////////////////////////////////////

#include <sstream>

#include "boost/lexical_cast.hpp"

#include "McsfNetBase/mcsf_communication_node_name.h"

#include "mcsf_filming_auto_printer_parameters_config.h"

#include "mcsf_filming_auto_config.h"

#include "mcsf_filming_auto_job_creater.h"

//#include "McsfAppToolkit/McsfAppSaveFilming/mcsf_save_filming_cmd_context.pb.h"

#include "McsfAppToolkit/McsfAppSaveFilming/mcsf_save_filming_cmd_context.pb.h"
using namespace McsfCommonSave;

MCSF_FILMING_BEGIN_NAMESPACE

McsfFilmingAutoJobCreater::McsfFilmingAutoJobCreater():
m_sOurAE(""),
m_sPrinterAE(""),
m_sPrinterIP(""),
m_iPrinterPort(0),
m_iCopies(1),
m_ePrintPriority(MEDIUM),
m_iSheetImageCount(1),
m_sLayout(""),
m_sPatientID(""),
m_sPatientName(""),
m_sPatientSex(""),
m_sPatientAge(""),
m_sOperatorName(""),
m_sAccessionNo("")
{
    Init();
}

McsfFilmingAutoJobCreater::~McsfFilmingAutoJobCreater()
{
	if (!m_vcDicomFilePathVector.empty())
	{
		std::vector<std::string> ().swap(m_vcDicomFilePathVector);
	}	
}

void McsfFilmingAutoJobCreater::Init()
{
    try
    {
        McsfPrinterParametersConfig printerParaConfig;
        m_sOurAE = printerParaConfig.GetOurAE();
        m_sPrinterAE = printerParaConfig.GetDeaultNodeAE();
        if(-1 == printerParaConfig.GetPacsNodeParametersByAE(m_sPrinterAE,&m_sPrinterIP,&m_iPrinterPort) )
        {
            LOG_ERROR("Not found default Pacs Node for filming");
        }

        std::ostringstream os;
        os <<  "PrintConfig is : (OurAE: " << m_sOurAE << ",  PrinterAE: " << m_sPrinterAE
            << ", PrinterIP: " << m_sPrinterIP << ", PrinterPort: " << m_iPrinterPort;

        std::cout << os.str() << std::endl;
        LOG_INFO(os.str());
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingAutoJobCreater::SetLayout(
    LAYOUT_TYPE_ENUM eLayoutType, int iFirtNo, int iSecondNo)
{
    try
    {
        switch((int)eLayoutType)
        {
        case 0:
            m_sLayout = "STANDARD\\"+boost::lexical_cast<std::string>(iFirtNo)
                +","+boost::lexical_cast<std::string>(iSecondNo);
            m_iSheetImageCount = iFirtNo*iSecondNo;
            break;
        case 1:
            m_sLayout = "ROW\\"+boost::lexical_cast<std::string>(iFirtNo)
                +","+boost::lexical_cast<std::string>(iSecondNo);
            m_iSheetImageCount = iFirtNo*iSecondNo;
            LOG_WARN("don't support ROW model now!");
            break;
        case 2:
            m_sLayout = "COL\\"+boost::lexical_cast<std::string>(iFirtNo)
                +","+boost::lexical_cast<std::string>(iSecondNo);
            m_iSheetImageCount = iFirtNo*iSecondNo;
            LOG_WARN("don't support COL model now!");
            break;
        }
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingAutoJobCreater::CreateFilmingJobInstance(std::string& sSerializedJob)
{
    FilmingPrintJob filmingPrintJob;
    try
    {
        FilmingPrintJob_FilmBox* pFilmBox = NULL;
        FilmingPrintJob_ImageBox* pImageBox = NULL;

        filmingPrintJob.set_printer_ae(m_sPrinterAE);
        filmingPrintJob.set_our_ae(m_sOurAE);
        filmingPrintJob.set_printer_ip(m_sPrinterIP);
        filmingPrintJob.set_port(m_iPrinterPort);

        filmingPrintJob.set_print_priority(static_cast<FilmingPrintJob_PrintPriority>(m_ePrintPriority));
        filmingPrintJob.set_print_timing(FilmingPrintJob_PrintTiming_IMMEDIATELY);

        filmingPrintJob.set_patient_id(m_sPatientID);
        filmingPrintJob.set_patient_name(m_sPatientName);
        filmingPrintJob.set_patient_sex(m_sPatientSex);
        filmingPrintJob.set_patient_age(m_sPatientAge);
        filmingPrintJob.set_operator_name(m_sOperatorName);
        filmingPrintJob.set_accession_no(m_sAccessionNo);

        filmingPrintJob.set_copies(m_iCopies);

        //the total image of per sheet
        int iSheetCount = 0;
        //the total image of this print job
        int iTotalImage = m_vcDicomFilePathVector.size();
        //if (iTotalImage% m_iSheetImageCount == 0)
        //{
        //    iSheetCount = iTotalImage / m_iSheetImageCount;
        //}
        //else
        //{
        //    iSheetCount = iTotalImage / m_iSheetImageCount + 1;
        //}
        iSheetCount = (int) ceil( (0.0+iTotalImage)/m_iSheetImageCount );

        for (int i = 0, iCurrentIndex = 0; i < iSheetCount; i++)
        {
            pFilmBox = filmingPrintJob.add_film_box();
            pFilmBox->set_layout(m_sLayout);

            for (int j = 0; j < m_iSheetImageCount && iCurrentIndex < iTotalImage; j++, iCurrentIndex++)
            {
                pImageBox = pFilmBox->add_image_box();
                pImageBox->set_image_path(m_vcDicomFilePathVector[iCurrentIndex]);
            }
        }
        sSerializedJob = filmingPrintJob.SerializeAsString();
        /*return 0;*/
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingAutoJobCreater::CreateFilmingJobCommandContext(CommandContext& commandContext)
{
    try
    {
#ifdef DEVELOPING
    commandContext.sReceiver = CommunicationNodeName::CreateCommunicationProxyName(MCSF_FILMING_NAME, FRONT_END);    
    commandContext.iCommandId = MCSF_AUTO_FILMING_COMMAND_ID;
#else
    commandContext.sReceiver = CommunicationNodeName::CreateCommunicationProxyName(MCSF_FILMING_NAME);//CreateCommunicationProxyName(MCSF_FILMING_NAME, BACK_END);
    commandContext.iCommandId = MCSF_ADD_FILMING_JOB_COMMAND_ID;
#endif // DEVELOPING
    std::string sSerializedJob(EMPTY_STRING);
    CreateFilmingJobInstance(sSerializedJob);
    commandContext.sSerializeObject = sSerializedJob;
   // commandContext.bServiceAsyncDispatch = true;
        //return 0;
    }
    catch(std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...)
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingAutoJobCreater::SendFilmingJobCommand(ICommunicationProxy& proxy)
{
    try
    {
        CommandContext autoFilmingCommandContext;
        CreateFilmingJobCommandContext(autoFilmingCommandContext);
        //std::string sSerializeObject("");
        if (-1 == proxy.AsyncSendCommand(&autoFilmingCommandContext))
        {
            LOG_ERROR(string("failed to send filming command to filming module, where patient id is ") + m_sPatientID) ;
        }
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingAutoJobCreater::SuspendAllFilmingJob(ICommunicationProxy& proxy)
{
    try
    {
        CommandContext commandContext;

#ifdef DEVELOPING
        commandContext.sReceiver = CommunicationNodeName::CreateCommunicationProxyName(MCSF_FILMING_NAME, FRONT_END);
        commandContext.iCommandId = SUSPEND_ALL_PRINT_JOB_COMMAND;
#else
        commandContext.sReceiver = CommunicationNodeName::CreateCommunicationProxyName(MCSF_FILMING_NAME);//CreateCommunicationProxyName(MCSF_FILMING_NAME, BACK_END);
        commandContext.iCommandId = SUSPEND_ALL_PRINT_JOB_COMMAND;
#endif // DEVELOPING

       // commandContext.bServiceAsyncDispatch = true;
        if (-1 == proxy.AsyncSendCommand(&commandContext))
        {
            LOG_ERROR(string("failed to send SuspendAllFilmingJob command to filming module!")) ;
        }
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingAutoJobCreater::ResumeAllFilmingJob(ICommunicationProxy& proxy)
{
    try
    {
        CommandContext commandContext;

#ifdef DEVELOPING
        commandContext.sReceiver = CommunicationNodeName::CreateCommunicationProxyName(MCSF_FILMING_NAME, FRONT_END);
        commandContext.iCommandId = RESUME_ALL_PRINT_JOB_COMMAND;
#else
        commandContext.sReceiver = CommunicationNodeName::CreateCommunicationProxyName(MCSF_FILMING_NAME);//CreateCommunicationProxyName(MCSF_FILMING_NAME, BACK_END);
        commandContext.iCommandId = RESUME_ALL_PRINT_JOB_COMMAND;
#endif // DEVELOPING

        //commandContext.bServiceAsyncDispatch = true;
        if (-1 == proxy.AsyncSendCommand(&commandContext))
        {
            LOG_ERROR(string("failed to send ResumeAllFilmingJob command to filming module!")) ;
        }
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

MCSF_FILMING_END_NAMESPACE
