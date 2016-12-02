using System;

namespace UIH.Mcsf.Filming
{
    internal class InterleavedSelectionViewModel : ViewModelBase
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
                    MaxFirstImageIndex = _totalImages;
                    MaxLastImageIndex = _totalImages;
                    LastImage = _totalImages;
                    Every = 2;
                    CalulateImageNumbers(null, null);
                    MaxEvery = _totalImages;
                }
            }
            get
            {
                return _totalImages;
            }
        }


        private uint _firstImage = 0;
        public uint FirstImage
        {
            get { return _firstImage; }
            set
            {
                if (_firstImage != value && value > 0)
                {
                    _firstImage = value;

                    OnPropertyChanged("FirstImage");

                    if (_firstImage > LastImage)
                    {
                        LastImage = _firstImage;
                    }

                    CalulateImageNumbers(null, null);
                }
            }
        }

        private uint _maxFirstImageIndex = 1;
        public uint MaxFirstImageIndex
        {
            get { return _maxFirstImageIndex; }
            set
            {
                if (_maxFirstImageIndex != value)
                {
                    _maxFirstImageIndex = value;
                    OnPropertyChanged("MaxFirstImageIndex");
                }
            }
        }


        private uint _lastImage = 0;
        public uint LastImage
        {
            get { return _lastImage; }
            set
            {
                if (_lastImage != value && value > 0)
                {
                    _lastImage = value;
                    OnPropertyChanged("LastImage");
                    CalulateImageNumbers(null, null);
                }
            }
        }

        private uint _maxLastImageIndex = 1;
        public uint MaxLastImageIndex
        {
            get { return _maxLastImageIndex; }
            set
            {
                if (_maxLastImageIndex != value)
                {
                    _maxLastImageIndex = value;
                    OnPropertyChanged("MaxLastImageIndex");
                }
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
                    OnPropertyChanged("Every");
                    CalulateImageNumbers(null, null);
                }
            }
        }

        private uint _maxEvery = 2;
        public uint MaxEvery
        {
            get { return _maxEvery; }
            set
            {
                if (_maxEvery != value)
                {
                    _maxEvery = value;
                    OnPropertyChanged("MaxEvery");
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
                    IsOkEnable = _imageNumbers > 0;
                    OnPropertyChanged("ImageNumbers");
                    
                }
            }
        }

        private string _loadingImages;

        public string LoadingImages
        {
            get { return _loadingImages; }
            set
            {
                if (_loadingImages != value)
                {
                    _loadingImages = value;
                    OnPropertyChanged("LoadingImages");
                   
                }
            }
        }

        private bool _isOkEnable = true;
        public bool IsOkEnable
        {
            get { return _isOkEnable; }
            set
            {
                _isOkEnable = value;
                OnPropertyChanged("IsOkEnable");
            }
        }
        public void CalulateImageNumbers(object sender, EventArgs args)
        {
            if (Every > 0 && LastImage >= FirstImage)
            {
                ImageNumbers = (LastImage - FirstImage) / Every + 1;
                LoadingImages = "";
                if (ImageNumbers >= 1)
                {
                    LoadingImages = FirstImage.ToString();
                    if (ImageNumbers >= 2)
                    {
                        var num2 = FirstImage + Every;
                        LoadingImages += "、" + num2.ToString();
                        if (ImageNumbers >= 3)
                        {
                            var num3 = FirstImage + 2 * Every;
                            LoadingImages += "、" + num3.ToString();
                            if (ImageNumbers == 4)
                            {
                                var num4 = FirstImage + 3 * Every;
                                LoadingImages += "、" + num4.ToString();
                            }
                            if (ImageNumbers > 4)
                            {
                                var num4 = FirstImage + (ImageNumbers - 1) * Every;
                                LoadingImages += "、...、" + num4.ToString();
                            }
                        }

                    }
                }
            }
            else
            {
                ImageNumbers = 0;
                LoadingImages = "0";
            }
        }

        #endregion

        #region [--Memento--]

        public InterleavedSelectionViewModelMemento CreateMemento()
        {
            return new InterleavedSelectionViewModelMemento(this);
        }

        public void RestoreMemento(InterleavedSelectionViewModelMemento memento)
        {
            FirstImage = memento.First;
            LastImage = memento.Last;
            Every = memento.Every;
        }

        #endregion
    }

    class InterleavedSelectionViewModelMemento
    {
        public InterleavedSelectionViewModelMemento(InterleavedSelectionViewModel viewModel)
        {
            First = viewModel.FirstImage;
            Last = viewModel.LastImage;
            Every = viewModel.Every;
        }

        public uint Every { get; private set; }
        public uint Last { get; private set; }
        public uint First { get; private set; }
    }
}
