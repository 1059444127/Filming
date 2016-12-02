﻿using UIH.Mcsf.Filming.DataModel;
using UIH.Mcsf.Filming.Wrapper;

namespace UIH.Mcsf.Filming.DemoViewModel
{
    [CallTrace(true)]
    public class CardViewModel
    {
        #region [--Field--]

        public CardViewModel(ICard card)
        {
            InitializeChildViewModel(card);

            card.InitializeFromConfigure();
        }

        private void InitializeChildViewModel(ICard card)
        {
            InfoPanelViewModel = new InfoPanelViewModel(card);
            ControlPanelViewModel = new ControlPanelViewModel(card);
            ImageLoadPanelViewModel = new ImageLoadPanelViewModel(card);
        }

        #endregion [--Field--]

        #region [--Property--]

        public InfoPanelViewModel InfoPanelViewModel { get; private set; }
        public ControlPanelViewModel ControlPanelViewModel { get; private set; }
        public ImageLoadPanelViewModel ImageLoadPanelViewModel { get; private set; }

        #endregion [--Property--]
    }
}