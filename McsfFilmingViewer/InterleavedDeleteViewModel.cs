using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace UIH.Mcsf.Filming
{
    internal class InterleavedDeleteViewModel : ViewModelBase
    {

        #region "Binding Properties"

        private uint _totalImages = 0;

        public uint TotalImages
        {
            set
            {
                if (_totalImages != value)
                {
                    _totalImages = value;
                    CalulateImageNumbers(null, null);
                }
            }
            get
            {
                return _totalImages;
            }
        }

        private uint _every = 2;
        public uint Every
        {
            get { return _every; }
            set
            {
                if (_every != value)
                {
                    _every = value;
                    this.OnPropertyChanged("Every");
                    CalulateImageNumbers(null, null);
                }
            }
        }



        private uint _imageNumbers;
        public uint ImageNumbers
        {
            get { return _imageNumbers; }
            set
            {
                if (_imageNumbers != value)
                {
                    _imageNumbers = value;
                    OnPropertyChanged("ImageNumbers");
                }
            }
        }

        public void CalulateImageNumbers(object sender, EventArgs args)
        {
            if (Every > 0)
            {
                ImageNumbers = TotalImages % Every == 0 ? ((TotalImages) / Every) : ((TotalImages) / Every) + 1;
            }
        }

        #endregion
    }
}
