namespace UIH.Mcsf.Filming
{
    public class FilmingWindowLevel
    {
        public FilmingWindowLevel() { Name = string.Empty; Center = 0; Width = 0; }
        public string Name { get; set; }
        public double Center { get; set; }
        public double Width { get; set; }
        public override string ToString() { return Name; }
    }
}
