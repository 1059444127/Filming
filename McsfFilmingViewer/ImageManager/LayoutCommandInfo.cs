using System;
using UIH.Mcsf.ComponentModel;

namespace UIH.Mcsf.Filming.ImageManager
{
    public class LayoutCommandInfo
    {
        public LayoutCommandInfo(string layoutInfo)
        {
            var args = layoutInfo.Split(Chars._1);

            if (args.Length < 4)
            {
                Logger.LogError("Command of Setting Layout for Common Filming lack arguments, command Info is: " + layoutInfo);
                return;
            }
            _layoutString = args[0];
            if(!UInt32.TryParse(args[1], out _imageCount))  Logger.LogWarning("wrong format of image count in layout string");
            if(!int.TryParse(args[2], out _orientation)) Logger.LogWarning("wrong format of orientation in layout string");
            _filmingIdentifier = args[3];
        }

        private readonly string _layoutString = string.Empty;
        public string LayoutString { get { return _layoutString; }}

        private readonly uint _imageCount;
        public uint ImageCount { get { return _imageCount; } }

        private readonly int _orientation;
        public int Orientation { get { return _orientation; } }

        private readonly string _filmingIdentifier = string.Empty;
        public string FilmingIdentifier { get { return _filmingIdentifier; } }
    }
}
