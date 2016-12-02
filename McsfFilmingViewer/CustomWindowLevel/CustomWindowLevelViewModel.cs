using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIH.Mcsf.Controls.Toolkit;
using UIH.Mcsf.Utility;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming
{
    class CustomWindowLevelViewModel : ViewModelBase
    {
        public CustomWindowLevelViewModel()
        {
        }

        #region CurrentCenterValue

        /// <summary>
        /// The <see cref="CurrentCenterValue" /> property's name.
        /// </summary>
        public const string CurrentCenterValuePropertyName = "CurrentCenterValue";

        private double _currentCenterValue;

        /// <summary>
        /// Sets and gets the CurrentCenterValue property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double CurrentCenterValue
        {
            get
            {
                return _currentCenterValue;
            }

            set
            {
                if (Math.Abs(_currentCenterValue - value) < 1E-9)
                {
                    return;
                }

                _currentCenterValue = value;

                OnPropertyChanged(CurrentCenterValuePropertyName);
            }
        }

        #endregion // CurrentCenterValue

        #region CurrentWidthValue

        /// <summary>
        /// The <see cref="CurrentWidthValue" /> property's name.
        /// </summary>
        public const string CurrentWidthValuePropertyName = "CurrentWidthValue";

        private double _currentWidthValue;

        /// <summary>
        /// Sets and gets the CurrentWidthValue property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double CurrentWidthValue
        {
            get
            {
                return _currentWidthValue;
            }

            set
            {
                if (Math.Abs(_currentWidthValue - value) < 1E-9)
                {
                    return;
                }

                _currentWidthValue = value;

                OnPropertyChanged(CurrentWidthValuePropertyName);
            }
        }

        #endregion // CurrentWidthValue

        #region WindowWidthOrTUID

        /// <summary>
        /// The <see cref="WindowWidthOrTUID" /> property's name.
        /// </summary>
        public const string WindowWidthOrTUIDPropertyName = "WindowWidthOrTUID";

        private string _windowWidthOrTUID;

        /// <summary>
        /// Sets and gets the WindowWidthOrTUID property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WindowWidthOrTUID
        {
            get
            {
                return _windowWidthOrTUID;
            }

            set
            {
                if (_windowWidthOrTUID == value)
                {
                    return;
                }

                _windowWidthOrTUID = value;

                OnPropertyChanged(WindowWidthOrTUIDPropertyName);

                Logger.LogInfo("[CustomWindowLevelViewModel]WindowWidthOrTUID Changes into {0}",
                    ObjectInfoHelper.GetNullableObjectInfo(value));
            }
        }

        #endregion // WindowWidthOrTUID

        #region WindowCenterOrBUID

        /// <summary>
        /// The <see cref="WindowCenterOrBUID" /> property's name.
        /// </summary>
        public const string WindowCenterOrBUIDPropertyName = "WindowCenterOrBUID";

        private string _windowCenterOrBUID;

        /// <summary>
        /// Sets and gets the WindowCenterOrBUID property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WindowCenterOrBUID
        {
            get
            {
                return _windowCenterOrBUID;
            }

            set
            {
                if (_windowCenterOrBUID == value)
                {
                    return;
                }

                _windowCenterOrBUID = value;

                OnPropertyChanged(WindowCenterOrBUIDPropertyName);

                Logger.LogInfo("[CustomWindowLevelViewModel]WindowCenterOrBUID Changes into {0}",
                    ObjectInfoHelper.GetNullableObjectInfo(value));
            }
        }

        #endregion // WindowCenterOrBUID

        #region CurrentNumeralType

        /// <summary>
        /// The <see cref="CurrentNumeralType" /> property's name.
        /// </summary>
        public const string CurrentNumeralTypePropertyName = "CurrentNumeralType";

        private NumeralType _currentNumberalType = NumeralType.Integer;

        /// <summary>
        /// Sets and gets the CurrentNumeralType property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public NumeralType CurrentNumeralType
        {
            get
            {
                return _currentNumberalType;
            }

            set
            {
                if (_currentNumberalType == value)
                {
                    return;
                }

                _currentNumberalType = value;

                OnPropertyChanged(CurrentNumeralTypePropertyName);

                Logger.LogInfo("[CustomWindowLevelViewModel]CurrentNumeralType Changes into {0}",
                    ObjectInfoHelper.GetNullableObjectInfo(value));
            }
        }

        #endregion // CurrentNumeralType

        #region CurrentDecimalNumber

        /// <summary>
        /// The <see cref="CurrentDecimalNumber" /> property's name.
        /// </summary>
        public const string CurrentDecimalNumberPropertyName = "CurrentDecimalNumber";

        private int _currentDecimalNumber;

        /// <summary>
        /// Sets and gets the CurrentDecimalNumber property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int CurrentDecimalNumber
        {
            get
            {
                return _currentDecimalNumber;
            }

            set
            {
                if (_currentDecimalNumber == value)
                {
                    return;
                }

                _currentDecimalNumber = value;

                OnPropertyChanged(CurrentDecimalNumberPropertyName);

                Logger.LogInfo("[CustomWindowLevelViewModel]CurrentDecimalNumber Changes into {0}",
                    ObjectInfoHelper.GetNullableObjectInfo(value));
            }
        }

        #endregion // CurrentDecimalNumber

        private Modality _modality;

        public Modality Modality
        {
            get { return _modality; }
            set
            {
                _modality = value;

                switch (value)
                {
                    case Modality.PT:
                        WindowWidthOrTUID = "UID_Filming_PreSet_Windowing_Top";
                        WindowCenterOrBUID = "UID_Filming_PreSet_Windowing_Bottom";
                        CurrentNumeralType = NumeralType.Decimal;
                        CurrentDecimalNumber = 2;
                        break;
                    case Modality.CT:
                        WindowWidthOrTUID = "UID_Filming_PreSet_Windowing_WW";
                        WindowCenterOrBUID = "UID_Filming_PreSet_Windowing_WC";
                        CurrentNumeralType = NumeralType.Integer;
                        CurrentDecimalNumber = 0;
                        break;
                    default:
                        //mr and default
                        WindowWidthOrTUID = "UID_Filming_PreSet_Windowing_WW";
                        WindowCenterOrBUID = "UID_Filming_PreSet_Windowing_WC";
                        CurrentNumeralType = NumeralType.Integer;
                        CurrentDecimalNumber = 0;
                        break;
                }
            }
        }
    }
}
