using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIH.Mcsf.Filming
{
    class FilmingMIPresetCellLayoutViewModel : ViewModelBase
    {
        #region "Binding Properties"

        private bool _isEnablePresetCellLayoutButton = true;
        public bool IsEnablePresetCellLayoutButton
        {
            get { return _isEnablePresetCellLayoutButton; }
            set
            {
                if (_isEnablePresetCellLayoutButton != value)
                {
                    _isEnablePresetCellLayoutButton = value;
                    OnPropertyChanged("IsEnablePresetCellLayoutButton");
                }
            }
        }
        
        private string _preSetCellLayout1Content = "1x1";
        public string PresetCellLayout1Content
        {
            get { return _preSetCellLayout1Content; }
            set
            {
                if (_preSetCellLayout1Content != value)
                {
                    _preSetCellLayout1Content = value;
                    OnPropertyChanged("PresetCellLayout1Content");
                }
            }
        }

        private string _preSetCellLayout1Icon;
        public string PresetCellLayout1Icon
        {
            get { return _preSetCellLayout1Icon; }
            set
            {
                if (_preSetCellLayout1Icon != value)
                {
                    _preSetCellLayout1Icon = value;
                    OnPropertyChanged("PresetCellLayout1Icon");
                }
            }
        }

        private string _preSetCellLayout2Content = "2x1";
        public string PresetCellLayout2Content
        {
            get { return _preSetCellLayout2Content; }
            set
            {
                if (_preSetCellLayout2Content != value)
                {
                    _preSetCellLayout2Content = value;
                    OnPropertyChanged("PresetCellLayout2Content");
                }
            }
        }

        private string _preSetCellLayout2Icon;
        public string PresetCellLayout2Icon
        {
            get { return _preSetCellLayout2Icon; }
            set
            {
                if (_preSetCellLayout2Icon != value)
                {
                    _preSetCellLayout2Icon = value;
                    OnPropertyChanged("PresetCellLayout2Icon");
                }
            }
        }

        private string _preSetCellLayout3Content = "4x5";
        public string PresetCellLayout3Content
        {
            get { return _preSetCellLayout3Content; }
            set
            {
                if (_preSetCellLayout3Content != value)
                {
                    _preSetCellLayout3Content = value;
                    OnPropertyChanged("PresetCellLayout3Content");
                }
            }
        }

        private string _preSetCellLayout3Icon;
        public string PresetCellLayout3Icon
        {
            get { return _preSetCellLayout3Icon; }
            set
            {
                if (_preSetCellLayout3Icon != value)
                {
                    _preSetCellLayout3Icon = value;
                    OnPropertyChanged("PresetCellLayout3Icon");
                }
            }
        }

        private string _preSetCellLayout4Content = "5x6";
        public string PresetCellLayout4Content
        {
            get { return _preSetCellLayout4Content; }
            set
            {
                if (_preSetCellLayout4Content != value)
                {
                    _preSetCellLayout4Content = value;
                    OnPropertyChanged("PresetCellLayout4Content");
                }
            }
        }

        private string _preSetCellLayout4Icon;
        public string PresetCellLayout4Icon
        {
            get { return _preSetCellLayout4Icon; }
            set
            {
                if (_preSetCellLayout4Icon != value)
                {
                    _preSetCellLayout4Icon = value;
                    OnPropertyChanged("PresetCellLayout4Icon");
                }
            }
        }

        private string _preSetCellLayout5Content = "6x7";
        public string PresetCellLayout5Content
        {
            get { return _preSetCellLayout5Content; }
            set
            {
                if (_preSetCellLayout5Content != value)
                {
                    _preSetCellLayout5Content = value;
                    OnPropertyChanged("PresetCellLayout5Content");
                }
            }
        }

        private string _preSetCellLayout5Icon;
        public string PresetCellLayout5Icon
        {
            get { return _preSetCellLayout5Icon; }
            set
            {
                if (_preSetCellLayout5Icon != value)
                {
                    _preSetCellLayout5Icon = value;
                    OnPropertyChanged("PresetCellLayout5Icon");
                }
            }
        }

        private string _preSetCellLayout6Content = "6x7";
        public string PresetCellLayout6Content
        {
            get { return _preSetCellLayout6Content; }
            set
            {
                if (_preSetCellLayout6Content != value)
                {
                    _preSetCellLayout6Content = value;
                    OnPropertyChanged("PresetCellLayout6Content");
                }
            }
        }

        private string _preSetCellLayout6Icon;
        public string PresetCellLayout6Icon
        {
            get { return _preSetCellLayout6Icon; }
            set
            {
                if (_preSetCellLayout6Icon != value)
                {
                    _preSetCellLayout6Icon = value;
                    OnPropertyChanged("PresetCellLayout6Icon");
                }
            }
        }

     

        private string _preSetCellLayout1ToolTip = "2X3";
        public string PresetCellLayout1ToolTip
        {
            get { return _preSetCellLayout1ToolTip; }
            set
            {
                if (_preSetCellLayout1ToolTip != value)
                {
                    _preSetCellLayout1ToolTip = value;
                    OnPropertyChanged("PresetCellLayout1ToolTip");
                }
            }
        }

        private string _preSetCellLayout2ToolTip = "3X4";
        public string PresetCellLayout2ToolTip
        {
            get { return _preSetCellLayout2ToolTip; }
            set
            {
                if (_preSetCellLayout2ToolTip != value)
                {
                    _preSetCellLayout2ToolTip = value;
                    OnPropertyChanged("PresetCellLayout2ToolTip");
                }
            }
        }

        private string _preSetCellLayout3ToolTip = "4X5";
        public string PresetCellLayout3ToolTip
        {
            get { return _preSetCellLayout3ToolTip; }
            set
            {
                if (_preSetCellLayout3ToolTip != value)
                {
                    _preSetCellLayout3ToolTip = value;
                    OnPropertyChanged("PresetCellLayout3ToolTip");
                }
            }
        }

        private string _preSetCellLayout4ToolTip = "5X6";
        public string PresetCellLayout4ToolTip
        {
            get { return _preSetCellLayout4ToolTip; }
            set
            {
                if (_preSetCellLayout4ToolTip != value)
                {
                    _preSetCellLayout4ToolTip = value;
                    OnPropertyChanged("PresetCellLayout4ToolTip");
                }
            }
        }

        private string _preSetCellLayout5ToolTip = "6X7";
        public string PresetCellLayout5ToolTip
        {
            get { return _preSetCellLayout5ToolTip; }
            set
            {
                if (_preSetCellLayout5ToolTip != value)
                {
                    _preSetCellLayout5ToolTip = value;
                    OnPropertyChanged("PresetCellLayout5ToolTip");
                }
            }
        }
        #endregion
    }
}
