namespace UIH.Mcsf.Filming
{
    public class ImgTextItem
    {
        public string Tag { get; set; }
        public string Format { get; set; }
        public bool Show { get; set; }

        public ImgTextItem Clone()
        {
            return new ImgTextItem
            {
                Tag = this.Tag,
                Format = this.Format,
                Show = this.Show
            };
        }
    }
}
