using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UIH.Mcsf.Core;
using UIH.Mcsf.Utility;
using UIH.Mcsf.Log;

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// logger wrapper, a singleton based on UIH.Mcsf.Log.Logger
    /// </summary>
    public static class Logger
    {
        #region Constructor
        static Logger()
        {
            try
            {
                DebugHelper.Source = "Filming";
                DebugHelper.ShowInConsole = true;
                DebugHelper.ShowInOutput = true;

                _sourceName = "MCSF/Filming";
                _logUID = 001035002;

                _logger = CLRLogger.GetInstance();
                
                
                //read configure file to create logger

                _logger.CreateLogger(mcsf_clr_systemenvironment_config.GetApplicationPath("FilmingConfigPath") + "McsfFilmingLog.xml");
            
                //_fileLogger = new StreamWriter(@"e:\LoadImageTimeStamps.txt", false);
            
            }
            catch (Exception ex)
            {
                DebugHelper.Trace(TraceLevel.Error, ex.StackTrace);
            }
        }

        /// <summary>
        /// get logger singleton instance
        /// </summary>
        public static CLRLogger Instance
        {
            get { return _logger; }
        }
        #endregion

        #region LoggerFunctions
        /// <summary>
        /// info logger
        /// </summary>
        /// <param name="info"></param>
        public static void LogInfo(string info)
        {
            //try
            //{
                //GetDetailCallingMethodLocation();
                _logger.LogDevInfo(_sourceName, _logUID, info);
                DebugHelper.Trace(TraceLevel.Info,  info);
            //}
            //catch (System.Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    Debug.WriteLine(e.Message);
            //}
        }

        public static void LogSvcInfo(string info)
        {
            try
            {
                //GetDetailCallingMethodLocation();
                _logger.LogSvcInfo(_sourceName, _logUID, info);
                DebugHelper.Trace(TraceLevel.Info, info);
            }
            catch (Exception e)
            {
                DebugHelper.Trace(TraceLevel.Error, e.Message);
            }
        }

        /// <summary>
        /// info logger of which, info is a format string
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void LogInfo(string format, params object[] args)
        {
            try
            {
                LogInfo(string.Format(format, args));
            }
            catch (Exception e)
            {
                DebugHelper.Trace(TraceLevel.Error, e.Message);
            }
        }

        /// <summary>
        /// warning logger
        /// </summary>
        /// <param name="info"></param>
        public static void LogWarning(string info)
        {
            //try
            //{
                //GetDetailCallingMethodLocation();
                _logger.LogDevWarning(_sourceName, _logUID, info);
                DebugHelper.Trace(TraceLevel.Warning, info);
            //}
            //catch (System.Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    Debug.WriteLine(e.Message);
            //}
        }

        /// <summary>
        /// error logger
        /// </summary>
        /// <param name="info"></param>
        public static void LogError(string info)
        {
            //try
            //{
                //GetDetailCallingMethodLocation();
                _logger.LogDevError(_sourceName, _logUID, info);
                DebugHelper.Trace(TraceLevel.Error, info);
            //}
            //catch (System.Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    Debug.WriteLine(e.Message);
            //}
        }

        /// <summary>
        /// write log when entering in to a function
        /// </summary>
        /// <param name="sParam">additions </param>

        [Conditional("DEBUG")]
        public static void LogFuncUp(string sParam = "")
        {
            //try
            //{
                GetDetailCallingMethodLocation();
                _logger.LogTraceInfo(_sourceName, "----" + _callFunctionName + " " + sParam + " is Up----" + Location);
                //_logger.LogDevInfo("----" + _callFunctionName + " " + sParam + " is Up----" + Location);
                DebugHelper.Trace(TraceLevel.Info, _logIndent + "----" + _callFunctionName + " " + sParam + " is Up @Line: " + _line);
            //}
            //catch (System.Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    Debug.WriteLine(e.Message);
            //}
        }

        /// <summary>
        /// write log when getting out of a function
        /// </summary>
        /// <param name="sParam">additions</param>
        [Conditional("DEBUG")]
        public static void LogFuncDown(string sParam = "")
        {
            try
            {
                GetDetailCallingMethodLocation();
                _logger.LogTraceInfo(_sourceName, "++++" + _callFunctionName + " " + sParam + " is Down normally++++" + Location);
                //_logger.LogDevInfo("++++" + _callFunctionName + " " + sParam + " is Down normally++++" + Location);
                DebugHelper.Trace(TraceLevel.Info, _logIndent + "++++" + _callFunctionName + " " + sParam + " is Down @Line: " + _line);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// write log when there is an exception in a Function
        /// </summary>
        /// <param name="sParam">additons</param>
        public static void LogFuncException(string sParam = "")
        {
            try
            {
                GetDetailCallingMethodLocation();


                _logger.LogDevError(_sourceName, _logUID, _callFunctionName + "  is Down abnormally!");

	            int i = 0;
                for (; i+80 < sParam.Length; i+=80)
                {
                    _logger.LogDevError(_sourceName, _logUID, sParam.Substring(i, 80));
                }
	            _logger.LogDevError(_sourceName, _logUID, sParam.Substring(i, sParam.Length - i));
	            //_logger.LogDevError(_sourceName, _logUID, _callFunctionName + " " + sParam + " Crash!" );
	            ////_logger.LogDevError("!!!!" + _callFunctionName + " " + sParam + " is Down abnormally!!!!" + Location);
	            //DebugHelper.Trace(TraceLevel.Error, _logIndent + "!!!!" + _callFunctionName + " " + sParam + " @Line: " + _line);

	            //const string logPath = @"D:\UIH\appdata\filming\config\Debug\CrashLog.txt";
	            //var fileLogger = new StreamWriter(logPath, true);
	            //fileLogger.WriteLine("[" + DateTime.Now.ToShortDateString() +"]" + sParam);
	            //fileLogger.Flush();
	            //fileLogger.Close();
            }
            catch (System.Exception e)
            {
                //Console.WriteLine(e.Message);
                Debug.WriteLine(e.Message);
            }
        }

        [Conditional("DEBUG")]
        public static void LogTimeStamp(string info)
        {
            try
            {

                var oldTimeStamp = _timeStamp;
                _timeStamp = DateTime.Now;

                var ts = _timeStamp - oldTimeStamp;

                string span = "[" + ts.TotalMilliseconds + "ms]";

                string timeStamp = "[" + _timeStamp.Hour + ":" + _timeStamp.Minute + ":" + _timeStamp.Second + ":" + _timeStamp.Millisecond + "]";

                info = timeStamp + " " + span + " " + info;
            
				_logger.LogDevInfo(info);
                //_fileLogger.WriteLine(info);
                //_fileLogger.Flush();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        [Conditional("DEBUG")]
        public static void LogTracing(string info)
        {
            DebugHelper.Trace(TraceLevel.Warning, info);
        }

        #endregion

        #region Properties
        ///  \brief  Get source name.
        public static string Source
        {
            get { return _sourceName; }
            set { _sourceName = value; }
        }

        public static string LogUidSource = "MCSF/Filming";

        ///  \brief  Get log UID.
        public static uint LogUid
        {
            get { return _logUID; }
            set { _logUID = value; }
        }

        public static string Location
        {
            get 
            {
                return " Stack LeveL: " + _stackLevel
                    + " Line: " + _line
                    + ", Func: " + _callFunctionName
                    + ", Class: " + _class
                    + ", File: " + _file; 
            }
        }

        #endregion

        #region Fields

        private static CLRLogger _logger;

        private static string _sourceName;

        private static uint _logUID;

        //private static readonly string _solutionName = "McsfFilmingViewerFE";

        private static readonly ushort _callLevel = 2;

        private static int _stackLevel = 1;

        private static string _logIndent;

        private static string _callFunctionName;

        //in which line logged
        private static int _line;

        //in which file logged
        private static string _file;

        //in which class logged
        private static string _class;

      //  private static StreamWriter _fileLogger;

        private static DateTime _timeStamp;

        #endregion

        #region Private Method

        private static void GetDetailCallingMethodLocation()
        {
            try
            {
                var st = new StackTrace(true);

                _stackLevel = st.FrameCount;

                _logIndent = string.Empty.PadRight(_stackLevel);

                var sf = st.GetFrame(_callLevel);
                var mb = sf.GetMethod();


                _file = sf.GetFileName();
                _line = sf.GetFileLineNumber();
                if (mb.DeclaringType != null) _class = mb.DeclaringType.Name;
                _callFunctionName = _class + "." + mb.Name;
            }
            catch (Exception e)
            {
                DebugHelper.Trace(TraceLevel.Error, e.Message);
            }
        }
        #endregion
    }
    //
    public class FilmingSvcLogUid
    {
        public static ulong LogUidSvcErrorConnectPrinter    = 0x0000320000000001;//SVC connect printer failed
        public static ulong LogUidSvcErrorImgTxtConfig      = 0x0000320000000002;//SVC image text config 
        public static ulong LogUidSvcErrorResfile           = 0x0000320000000003;//SVC Resource file
        public static ulong LogUidSvcWarnConfigureFile      = 0x0001320000000003;//SVC Config file
        //public static ulong LogUidSvcInfoUnsupportImage     = 0x0002320000000004;//SVC unsupport image
        public static ulong LogUidSvcWarnResolutionHigh     = 0x0001320000000005;//SVC Resolution high
        public static ulong LogUidSvcErrorCommunication     = 0x0000320000000006; //SVC IP端口配置错误/无法连接到打印机/进入service检查IP端口配置/Error
        public static ulong LogUidSvcInfoSwitchToFilming    = 0x0002320000000007; //SVC switch to fiming application
        public static ulong LogUidSvcInfoImageLoaded        = 0x0002320000000008; //SVC image loaded
        public static ulong LogUidSvcInfoPrintButtonClicked = 0x0002320000000009; //SVC print button clicked
        //public static ulong LogUidSvcInfoPixel              = 0x0002320000000007; //info
        //public static ulong LogUidSvcInfoAnnotation         = 0x0002320000000008; //info
        //public static ulong LogUidSvcInfoArrow              = 0x0002320000000009; //info
        //public static ulong LogUidSvcInfoRectangle          = 0x000232000000000A; //info
        //public static ulong LogUidSvcInfoEllipse            = 0x000232000000000B; //info
        //public static ulong LogUidSvcInfoFreeHand           = 0x000232000000000C; //info
        //public static ulong LogUidSvcInfoPolygon            = 0x000232000000000D; //info
        //public static ulong LogUidSvcInfoLine               = 0x000232000000000E; //info
        //public static ulong LogUidSvcInfoAngle              = 0x000232000000000F; //info
        //public static ulong LogUidSvcInfoSpline             = 0x0002320000000010; //info
    }
}
