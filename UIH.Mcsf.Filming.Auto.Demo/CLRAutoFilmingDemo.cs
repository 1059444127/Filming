using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UIH.Mcsf.Core;
using System.Threading;

namespace UIH.Mcsf.Filming.Auto
{
    public class CLRAutoFilmingDemo : CLRContaineeBase
    {
        //CLRAutoFilmingDemo()
        //{
        //    System.Console.WriteLine("waiting for input, just for pausing: ");
        //    System.Console.ReadLine();
        //}

        public override void  Startup()
        {
            //base.Startup();
        }

        private int _imageCount = 0;

        public override void DoWork()
        {
            //base.DoWork();
            //Thread thread = new Thread(CommandLoop);
           // Command();
            while (true)
            {
                Console.WriteLine("Sure to add  jobs? please input Job Number ...");
                // Console.ReadKey();
                int iLoopNum = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("you  chose to start {0} Jobs ", iLoopNum);
                for (int i = 0; i < iLoopNum; i++)
                {
                    Command();
                    //Thread.Sleep(30 * 1000);
                }
            }
            
        }

        void Command()
        {
            try
            {
                Job target = new Job(); // 

                //////////////////////////////////////////////////////////////////////////
                //interface display
                //////////////////////////////////////////////////////////////////////////
                //System.Console.WriteLine(target.GetPrinterList().ToString());
                foreach (PeerNode node in target.GetPrinterList())
                {
                    //System.Console.WriteLine(node.PeerAE);
                    //System.Console.WriteLine(node.PeerIP);
                    //System.Console.WriteLine(node.PeerPort);
                }
                target.GetAutoFilmStrategy();

                //foreach (string lutFile in target.LutFiles)
                //{
                //    System.Console.WriteLine(lutFile);
                //}
                //target.LutFile = target.LutFiles[0];
    
                //////////////////////////////////////////////////////////////////////////
                //Set parameters before autofilming
                //////////////////////////////////////////////////////////////////////////
                bool canAutoFilming = target.CanAutoFilming;
                //if (!canAutoFilming)
                //{
                //    System.Console.WriteLine("not allow to do auto-filming");
                //    canAutoFilming = true;
                //    return;
                //}
                    
                //target.SetPrinter(target.GetPrinterList()[0]);

                target.AccessionNO = "AccessionNO";
                target.OperatorName = "OperatorName";
                target.PatientAge = "PatientAge";
                target.PatientID = "PatientID";
                target.PatientName = "PatientName";
                target.PatientSex = "PatientSex";
                //target.SetLayout(LAYOUT_TYPE.STANDARD, 2, 2);
                //target.PrintPriority = PRINT_PRIORITY.HIGH;
                target.Copies = 1;
                target.IfSaveEFilm = true;
                IList<string> fileList = new List<string>();

                bool bTestAutoFilming = false;

                String sEntryPath = mcsf_clr_systemenvironment_config.GetApplicationPath("FilmingConfigPath") + "Miscellaneous.xml";


                XmlDocument doc = new XmlDocument();
                doc.Load(sEntryPath);
                XmlNode xn = doc.SelectSingleNode("/Miscellaneous/TestAutoFilming");
                XmlElement xe = (XmlElement)xn;
                
                if(xe!=null)
                    bTestAutoFilming = Convert.ToBoolean(xe.InnerText);

                if (!bTestAutoFilming)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        //fileList.Add("//10.1.2.13/Public/03-SW/SW-WangHui/Images/dx.dcm");
                        fileList.Add("e:/1.dcm");
                    }
                }
                else // Test AutoFilming
                {
                    for (int i = 0; i < 3; i++ )
                    {
                        var str = "e:/" + _imageCount + ".dcm";
                        Console.WriteLine(str);
                        fileList.Add(str);
                        _imageCount = (_imageCount + 1) % 3;
                    }

                    target.SetLayout(LAYOUT_TYPE.STANDARD, 2, 2);
                }
                target.DicomFilePathList = fileList;
    
                IList<string> originalImages = new List<string>();
                originalImages.Add("");
                target.OriginalImageUIDList = originalImages;

                target.SendFilmingJobCommand(this.GetCommunicationProxy(), bTestAutoFilming);

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
