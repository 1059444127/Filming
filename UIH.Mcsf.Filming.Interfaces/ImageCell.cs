using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class ImageCell
    {
        public ImageCell()
        {
            DisplayData = new DisplayData();
        }

        public DisplayData DisplayData { get; private set; }
    }
}
