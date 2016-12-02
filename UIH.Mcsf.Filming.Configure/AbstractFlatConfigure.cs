using System;
using System.Linq.Expressions;
using UIH.Mcsf.Core;

namespace UIH.Mcsf.Filming.Configure
{
    public class AbstractFlatConfigure : AbstractConfigure
    {
        protected AbstractFlatConfigure(string filePath, bool ifReadConfigures=false) : base(filePath)
        {
            if(ifReadConfigures) ParseConfigures();
        }



        protected override void ReadConfigures(IFileParserCSharp fileParser)
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "ReadConfigures");

            var type = GetType();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var propertyType = propertyInfo.PropertyType;
                var propertyTypeName = propertyType.Name;
                var sTagName = propertyInfo.Name;
                object configure = null;
                switch (propertyTypeName)
                {
                    case "Boolean":
                        bool bTemp;
                        if (fileParser.GetBoolValueByTag(sTagName, out bTemp))
                            configure = bTemp;
                        else
                        {
                            Logger.Instance.LogDevWarning("[Fail to Read Bool Tag]" + sTagName);
          //                  Logger.Instance.LogSvcWarning(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnConfigureFile,
          //"[Fail to Read Bool Tag]" + sTagName + "[From file]" + FilePath);
                        }
                        break;
                    case "Int32":
                        int iTemp;
                        if (fileParser.GetIntValueByTag(sTagName, out iTemp))
                            configure = iTemp;
                        else
                        {
                            Logger.Instance.LogDevWarning("[Fail to Read Int Tag]" + sTagName);
          //                  Logger.Instance.LogSvcWarning(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnConfigureFile,
          //"[Fail to Read Int Tag]" + sTagName + "[From file]" + FilePath);
                        }
                        break;
                    case "Double":
                        double dTemp;
                        if (fileParser.GetDoubleValueByTag(sTagName, out dTemp))
                            configure = dTemp;
                        else
                        {
                            Logger.Instance.LogDevWarning("[Fail to Read Double Tag]" + sTagName);
          //                  Logger.Instance.LogSvcWarning(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnConfigureFile,
          //"[Fail to Read Double Tag]" + sTagName + "[From file]" + FilePath);
                        }
                        break;
                    default:
                        configure = fileParser.GetStringValueByTag(sTagName);
                        break;
                }

                if(configure != null)
                    propertyInfo.SetValue(this, configure, null);
            }
        }


        protected void WriteBack<T>(string value, Expression<Func<T>> expression)
        {
            WriteBack(value, ((MemberExpression)expression.Body).Member.Name);
        }

    }
}
