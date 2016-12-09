using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class ImageCell
    {
        public ImageCell()
        {
            DisplayData = GlobalDefinitions.EmptyDisplayData;
        }

        public DisplayData DisplayData { get; private set; }
    }
}
