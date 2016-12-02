using System;
using System.Xml;
using System.Xml.Linq;
using UIH.Mcsf.Core;

namespace UIH.Mcsf.Filming
{
    public class MiscellaneousConfigureReader
    {
        public readonly static MiscellaneousConfigureReader Instance = new MiscellaneousConfigureReader();
        private readonly string _miscellaneousConfigPath;

        public int TimeOfWaitingForFilmBurning
        {
            get
            {
                try
                {
                    Logger.LogFuncUp();

                    var xDoc = new XmlDocument();
                    xDoc.Load(_miscellaneousConfigPath);
                    var xn = xDoc.SelectSingleNode("/Miscellaneous/AutoFilming/TimeOfWaitingForFilmBurning");
                    if (xn != null)
                        _timeOfWaitingForFilmBurning = int.Parse(xn.InnerText);

                    Logger.LogFuncDown();
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.Message + ex.StackTrace);
                }
                return _timeOfWaitingForFilmBurning;
            }
        }

        public int TimeOfWaitingForFirstFilmBurning
        {
            get
            {
                try
                {
                    Logger.LogFuncUp();

                    var xDoc = new XmlDocument();
                    xDoc.Load(_miscellaneousConfigPath);
                    var xn = xDoc.SelectSingleNode("/Miscellaneous/AutoFilming/TimeOfWaitingForFirstFilmBurning");
                    if (xn != null)
                        if (xn != null)
                            _timeOfWaitingForFirstFilmBurning = int.Parse(xn.InnerText);

                    Logger.LogFuncDown();
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.Message + ex.StackTrace);
                }
                return _timeOfWaitingForFirstFilmBurning;
            }
        }

        private MiscellaneousConfigureReader()
        {
            var sEntryPath = mcsf_clr_systemenvironment_config.GetApplicationPath("FilmingConfigPath");

            _miscellaneousConfigPath = sEntryPath + "Miscellaneous.xml";
        }

        private int _timeOfWaitingForFilmBurning = 7000;

        private int _timeOfWaitingForFirstFilmBurning = 10000;
    }
}
