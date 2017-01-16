using System;
using System.Collections.Generic;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class Board : IBoard
    {
        public Board()
        {
            PageRepository = new PageRepositoryStub();
        }

        #region Implementation of IBoard

        public int CellCount
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public event EventHandler CellCountChanged = delegate { };

        public IList<BoardCell> BoardCells
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void NewPage()
        {
            PageRepository.AppendPage();
        }

        #endregion


        public IPageRepository PageRepository { get; set; }
    }

    class PageRepositoryStub : IPageRepository
    {
        #region Implementation of IPageRepository

        public void AppendPage()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}