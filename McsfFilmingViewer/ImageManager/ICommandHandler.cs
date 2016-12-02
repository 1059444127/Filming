namespace UIH.Mcsf.Filming.ImageManager
{
    public abstract class ACommandHandler
    {
        public abstract void HandleCommand();
        public virtual ACommandHandler CurrentJob()
        {
            return this;
        }

        public virtual bool IsReady { get { return true; }}
    }
}
