﻿using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class FilmBuffer
    {
        private IFilmRepository _films;
        private const int Capacity = GlobalDefinitions.MaxDisplayMode;
        private int _cursor;

        public FilmBuffer(IFilmRepository filmRepository)
        {
            _films = filmRepository;
            RegisterFilmRepositoryEvent();
        }

        ~FilmBuffer()
        {
            UnRegisterFilmRepositoryEvent();
        }

        #region [--FilmRepository Event Handler--]

        private void UnRegisterFilmRepositoryEvent()
        {
            _films.FocusChanged -= FilmsOnFocusChanged;
        }

        private void RegisterFilmRepositoryEvent()
        {
            _films.FocusChanged += FilmsOnFocusChanged;
        }

        private void FilmsOnFocusChanged(object sender, EventArgs eventArgs)
        {
            
        }

        #endregion

        public int VisibleSize { get; set; }
    }
}