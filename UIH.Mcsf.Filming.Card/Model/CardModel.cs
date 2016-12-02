using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.Utility;

namespace UIH.Mcsf.Filming.Card.Model
{
    public class CardModel
    {
        //public void AddFilm()
        //{
        //    FilmAdded(this, new EventArgs());
        //}

        public CardModel()
        {
            _regions.Add(new RegionModel());
        }

        #region [--Events--]

        //todo: need to be polished
        //public event EventHandler FilmAdded = delegate { };
        public event EventHandler DisplayModeChanged = delegate { };

        #endregion //[--Events--]

        #region [--Fields--]

        private int _displayMode = GlobalDefinition.DefaultDisplayMode;

        private IList<RegionModel> _regions = new List<RegionModel>();

        #endregion //[--Fields--]

        #region [--Properties--]

        public int DisplayMode
        {
            get { return _displayMode; }
            set
            {
                var log = string.Format("[Mode][DisplayMode][Changed] [OldValue]{0} [NewValue] {1}", _displayMode, value);
                Logger.Instance.LogDevInfo(log);
                DebugHelper.Trace(TraceLevel.Info, log);

                if (_displayMode == value) return;
                _displayMode = value;
                DisplayModeChanged(this, new EventArgs());
            }
        }

        #endregion //[--Properties--]
    }
}
