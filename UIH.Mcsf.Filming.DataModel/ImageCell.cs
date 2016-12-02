using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Database;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.DataModel
{
    public abstract class ImageCell : /*ObjectWithAspects,*/ ISelect
    {
        protected DisplayData _displayData;
        private bool _isFocused;
        private bool _isSelected;
        protected object RawData { private get; set; } //maybe string/imageBase/byte[]/null

        public DisplayData DisplayData
        {
            get { return _displayData = _displayData ?? CreateDisplayData(); }
            set { _displayData = value; }
        }

        public bool IsEmpty
        {
            get { return RawData == null; }
        }

        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                if (_isFocused == value) return;
                _isFocused = value;
                FocusedChanged(this, new BoolEventArgs {Bool = value});
            }
        }

        public void Click(ClickStatus clickStatus)
        {
            Clicked(this, new ClickStatusEventArgs(clickStatus));
        }

        protected abstract DisplayData CreateDisplayData();

        public virtual void Prepare()
        {
        }

        #region [--Event--]

        public event EventHandler<BoolEventArgs> FocusedChanged = delegate { };

        public event EventHandler<BoolEventArgs> SelectedChanged = delegate { };

        #endregion [--Event--]

        #region [--ISelect--]

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                SelectedChanged(this, new BoolEventArgs {Bool = value});
            }
        }

        public event EventHandler<ClickStatusEventArgs> Clicked = delegate { };

        #endregion [--ISelect--]

        #region [--DataLoader--]

        static ImageCell()
        {
            StudyTree = new StudyTree();
            DataLoader = DataLoaderFactory.Instance().CreateLoader(StudyTree, DBWrapperHelper.DBWrapper);
        }

        public static IViewerConfiguration ViewerConfiguration
        {
            set
            {
                _viewerConfiguration = value;
                DataAccessor = new DataAccessor(_viewerConfiguration);
            }
        }


        protected static DataAccessor DataAccessor;

        public static readonly IDataLoader DataLoader;
        protected static readonly StudyTree StudyTree;

        protected static readonly IDictionary<string, DisplayData> DisplayDataTable =
            new Dictionary<string, DisplayData>();

        private static IViewerConfiguration _viewerConfiguration;

        #endregion [--DataLoader--]
    }

    internal class EmptyCell : ImageCell
    {
        protected override DisplayData CreateDisplayData()
        {
            return null;
        }
    }

    internal class AppCell : ImageCell
    {
        public AppCell(byte[] bytes)
        {
            RawData = bytes;
        }

        protected override DisplayData CreateDisplayData()
        {
            throw new NotImplementedException();
        }
    }

    //Todo: 多分格Cell
    //[CallTrace(true)]
    internal class Cell : ImageCell
    {
        private readonly ImageBase _imageBase;

        public Cell(ImageBase imageBase)
        {
            RawData = imageBase;
            _imageBase = imageBase;
        }

        protected override DisplayData CreateDisplayData()
        {
            var sopInstanceUid = _imageBase.SOPInstanceUID;
            if (DisplayDataTable.ContainsKey(sopInstanceUid)) return DisplayDataTable[sopInstanceUid];

            var sop = StudyTree.GetSop(sopInstanceUid);

            var displayData = sop == null
                ? DataAccessor.CreateImageDataBySOPInstanceUID(sopInstanceUid, true)
                : CreateDisplayDataBy(sop);


            DisplayDataTable.Add(sopInstanceUid, displayData);
            return displayData;
        }

        private DisplayData CreateDisplayDataBy(Sop sop)
        {
            var imageSop = sop as ImageSop;
            Debug.Assert(imageSop != null);
            return DataAccessor.CreateImageData(imageSop.DicomSource, imageSop.GetNormalizedPixelData,
                imageSop.PresentationState);
        }

        public override void Prepare()
        {
            if (_displayData != null) return;
            //Console.Write("Prepare for a cell\t");
            var sopInstanceUid = _imageBase.SOPInstanceUID;
            if (StudyTree.GetSop(sopInstanceUid) == null)
                DataLoader.LoadSopByUid(sopInstanceUid);
        }
    }
}