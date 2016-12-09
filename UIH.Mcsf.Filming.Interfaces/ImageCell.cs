using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class ImageCell
    {
        public ImageCell()
        {
            DisplayData = GlobalDefinitions.EmptyDisplayData;
        }
        // TODO: ImageCell.Create An Image DisplayData  for Displaying An Image in ViewerControlAdapter
        public DisplayData DisplayData { get; private set; }
    }
}
