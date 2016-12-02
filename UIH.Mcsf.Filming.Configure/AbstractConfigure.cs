using System;
using UIH.Mcsf.Core;

namespace UIH.Mcsf.Filming.Configure
{
    public abstract class AbstractConfigure
    {

        #region statics

        private static IFileParserCSharp _fileParser;

        protected static IFileParserCSharp GetFileParser()
        {
            try
            {
                if (_fileParser == null)
                {
                    _fileParser = ConfigParserFactory.Instance().CreateCSharpFileParser();
                    _fileParser.Initialize();
                }
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevWarning(e.Message);
            }
            return _fileParser;
        }

        #endregion

        #region [--Protected--]
        protected AbstractConfigure(string filePath)
        {
            FilePath = filePath;
        }

        protected void WriteBackAttribute(string value, string attributeName, string tagName, string path="/")
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[WriteBack]" + "[Value]" + value +
                                           "[Attribute]" + attributeName + "[Tag]" + tagName + "[Path]" + path + "[File]" + FilePath);

                var fileParser = GetFileParser();

                if (!fileParser.ParseByURI(FilePath))
                {
                    Logger.Instance.LogDevWarning("[Fail to parse file]" + FilePath);
          //          Logger.Instance.LogSvcWarning(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnConfigureFile,
          //"Fail to parse file " + FilePath);
                }
                else if (!fileParser.SetAttributeStringValue(path+tagName, attributeName, value))
                {
                    Logger.Instance.LogDevWarning("[Fail to Set Attribute]" + attributeName + "[Tag]" + tagName + "[Value]" + value + "[Path]" + path);
                    Logger.Instance.LogSvcWarning(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnConfigureFile,
                              "[Fail to Set Attribute]" + attributeName + "[Tag]" + tagName + "[Value]" + value + "[Path]" + path);
                }
                else if (!fileParser.SaveDocument())
                {
                    Logger.Instance.LogDevWarning("[Fail to Save Config file]" + FilePath);
                    Logger.Instance.LogSvcWarning(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnConfigureFile,
                              "[Fail to Save Config file]" + FilePath);
                }

            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError("[Exception]" + e + "[Message]" + e.Message + "[Fail to]" + "[WriteBack]" + "[Value]" + value +
                          "[Attribute]" + attributeName + "[Tag]" + tagName + "[Path]" + path + "[File]" + FilePath);
            }
        }

        protected void WriteBack(string value, string tagName, string path = "/")
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[WriteBack]" + "[Value]" + value +
                                           "[Tag]" + tagName + "[Path]" + path + "[File]" + FilePath);

                var fileParser = GetFileParser();

                if (!fileParser.ParseByURI(FilePath))
                {
                    Logger.Instance.LogDevWarning("[Fail to parse file]" + FilePath);
          //          Logger.Instance.LogSvcWarning(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnConfigureFile,
          //"Fail to parse file " + FilePath);
                }
                else if (!fileParser.SetStringValueByTag(tagName, value)
                         && !fileParser.InsertElement(path, tagName, value))
                {
                    Logger.Instance.LogDevWarning("[Fail to Set Tag]" + tagName + "[Value]" + value + "[Path]" + path);
                    Logger.Instance.LogSvcWarning(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnConfigureFile,
                              "[Fail to Set Tag]" + tagName + "[Value]" + value + "[Path]" + path);
                }
                else if (!fileParser.SaveDocument())
                {
                    Logger.Instance.LogDevWarning("[Fail to Save Config file]" + FilePath);
                    Logger.Instance.LogSvcWarning(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnConfigureFile,
                              "[Fail to Save Config file]" + FilePath);
                }

            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError("[Exception]" + e + "[Message]" + e.Message + "[Fail to]" + "[WriteBack]" + "[Value]" + value +
                           "[Tag]" + tagName + "[Path]" + path + "[File]" + FilePath);
            }
        }

        protected string FilePath { get; set; }
        #endregion [--Protected--]

        #region [--Template Functions--]
        protected abstract void ReadConfigures(IFileParserCSharp fileParser);
        #endregion [--Template Functions--]

        #region [--Interface--]
        public void ParseConfigures()
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ParseConfigures]" + FilePath);

                var fileParser =  GetFileParser();

                if (!fileParser.ParseByURI(FilePath))
                {
                    Logger.Instance.LogDevWarning("[Fail to parse file]" + FilePath);
                    //Logger.Instance.LogSvcWarning(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnConfigureFile,
                    //          "Fail to parse file " + FilePath);
                }
                else
                    ReadConfigures(fileParser);

            }
            catch (Exception e)
            {
                Logger.Instance.LogDevWarning("[Exception]" + e + "[Message]" + e.Message + "[When ParseConfigures from File]" + FilePath);
            }
        }

        #endregion [--Interface--]
    }
}
