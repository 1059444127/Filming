using UIH.Mcsf.Filming.Widgets;

namespace UIH.Mcsf.Filming.Interface
{
    public class Protocol : Notifier
    {
        public Protocol(string name, LayoutBase layout)
        {
            Name = name;
            _layout = layout;
        }
        public string Name { get; private set; }

        public string NameUpper
        {
            get { return this.Name.ToUpper(); }
        }

	    private LayoutBase _layout;

	    public LayoutBase Layout
	    {
		    get { return _layout; }
		    set
		    {
			    if (_layout == value) return;
			    _layout = value;
			    NotifyPropertyChanged(() => Layout);
		    }
	    }

        public override string ToString()
        {
            return string.Format("[Name]{0}[Layout]{1}", Name, _layout);
        }
    }
}
