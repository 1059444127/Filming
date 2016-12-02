using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace UIH.Mcsf.Filming
{
    public class CellContextMenu
    {
        private static ObservableCollection<FilmImageObject> _clipBoardImageObjectCollection =
            new ObservableCollection<FilmImageObject>();
        public static ObservableCollection<FilmImageObject> ClipBoardImageObjectCollection
        {
            get { return _clipBoardImageObjectCollection; }
            set { _clipBoardImageObjectCollection = value; }
        }

        private void IfNullCreateMenuItem(ref MenuItem item, string headerName, Boolean enable = true)
        {
            if (item == null)
            {
                item = new MenuItem { Header = headerName, IsEnabled = enable};
            }
        }

        private MenuItem _cutMenuItem;
        public MenuItem CutMenuItem
        {
            get
            {
                IfNullCreateMenuItem(ref _cutMenuItem, "Cut Selected Images");
                return _cutMenuItem;
            }
        }

        private MenuItem _copyMenuItem;
        public MenuItem CopyMenuItem
        {
            get
            {
                IfNullCreateMenuItem(ref _copyMenuItem, "Copy Selected Images");
                return _copyMenuItem;
            }
        }

        private MenuItem _pasteMenuItem;
        public MenuItem PasteMenuItem
        {
            get
            {
                IfNullCreateMenuItem(ref _pasteMenuItem, "Paste", false);
                return _pasteMenuItem;
            }
        }

        private MenuItem _deleteMenuItem;
        public MenuItem DeleteMenuItem
        {
            get
            {
                IfNullCreateMenuItem(ref _deleteMenuItem, "Delete Selected Images");
                return _deleteMenuItem;
            }
        }

        private MenuItem _deleteAllMenuItem;
        public MenuItem DeleteAllMenuItem
        {
            get
            {
                IfNullCreateMenuItem(ref _deleteAllMenuItem, "Delete All Images");
                return _deleteAllMenuItem;
            }
        }

        private MenuItem _insertMenu;
        public MenuItem InsertMenu
        {
            get
            {
                if (_insertMenu == null)
                {
                    var blackCellItem = new MenuItem { Header = "Black Cell", IsEnabled = true };
                    var pageBreakCellItem = new MenuItem { Header = "Page Break", IsEnabled = true };
                    var parameterCellItem = new MenuItem { Header = "Parameter", IsEnabled = true };

                    _insertMenu = new MenuItem { Header = "Insert", IsEnabled = false };
                    _insertMenu.Items.Add(blackCellItem);
                    _insertMenu.Items.Add(pageBreakCellItem);
                    _insertMenu.Items.Add(parameterCellItem);
                }

                return _insertMenu;
            }
        }

        private ContextMenu _cellContextMenu;
        public ContextMenu FilmCellContextMenu
        {
            get
            {
                if (_cellContextMenu == null)
                {
                    _cellContextMenu = new ContextMenu();
                    _cellContextMenu.Items.Add(CutMenuItem);
                    _cellContextMenu.Items.Add(CopyMenuItem);
                    _cellContextMenu.Items.Add(PasteMenuItem);
                    _cellContextMenu.Items.Add(DeleteMenuItem);
                    _cellContextMenu.Items.Add(DeleteAllMenuItem);
                    _cellContextMenu.Items.Add(InsertMenu);
                }

                return _cellContextMenu;
            }
        }
    }

    public class FilmPageContextMenu
    {
        private void IfNullCreateMenuItem(ref MenuItem item, string headerName, Boolean enable = true)
        {
            if (item == null)
            {
                item = new MenuItem { Header = headerName, IsEnabled = enable };
            }
        }

        private MenuItem _cutMenuItem;
        public MenuItem CutMenuItem
        {
            get
            {
                IfNullCreateMenuItem(ref _cutMenuItem, "Cut Selected Images", false);
                return _cutMenuItem;
            }
        }

        private MenuItem _copyMenuItem;
        public MenuItem CopyMenuItem
        {
            get
            {
                IfNullCreateMenuItem(ref _copyMenuItem, "Copy Selected Images", false);
                return _copyMenuItem;
            }
        }

        private MenuItem _pasteMenuItem;
        public MenuItem PasteMenuItem
        {
            get
            {
                IfNullCreateMenuItem(ref _pasteMenuItem, "Paste");
                return _pasteMenuItem;
            }
        }

        private MenuItem _deleteMenuItem;
        public MenuItem DeleteMenuItem
        {
            get
            {
                IfNullCreateMenuItem(ref _deleteMenuItem, "Delete Selected Images", false);
                return _deleteMenuItem;
            }
        }

        private MenuItem _deleteAllMenuItem;
        public MenuItem DeleteAllMenuItem
        {
            get
            {
                IfNullCreateMenuItem(ref _deleteAllMenuItem, "Delete All Images", false);
                return _deleteAllMenuItem;
            }
        }

        private MenuItem _insertBlackCellMenu;
        public MenuItem InsertBlackCellMenu
        {
            get
            {
                IfNullCreateMenuItem(ref _insertBlackCellMenu, "Black Cell");
                return _insertBlackCellMenu;
            }
        }

        private MenuItem _insertMenu;
        public MenuItem InsertMenu
        {
            get
            {
                if (_insertMenu == null)
                {
                    var blackCellItem = new MenuItem { Header = "Black Cell", IsEnabled = true };
                    var pageBreakCellItem = new MenuItem { Header = "Page Break", IsEnabled = true };
                    var parameterCellItem = new MenuItem { Header = "Parameter", IsEnabled = true };

                    _insertMenu = new MenuItem { Header = "Insert", IsEnabled = false };
                    _insertMenu.Items.Add(blackCellItem);
                    _insertMenu.Items.Add(pageBreakCellItem);
                    _insertMenu.Items.Add(parameterCellItem);
                }

                return _insertMenu;
            }
        }

        private ContextMenu _filmingPageContextMenu;
        public ContextMenu FilmingControlContextMenu
        {
            get
            {
                if (_filmingPageContextMenu == null)
                {
                    _filmingPageContextMenu = new ContextMenu();
                    _filmingPageContextMenu.Items.Add(CutMenuItem);
                    _filmingPageContextMenu.Items.Add(CopyMenuItem);
                    _filmingPageContextMenu.Items.Add(PasteMenuItem);
                    _filmingPageContextMenu.Items.Add(DeleteMenuItem);
                    _filmingPageContextMenu.Items.Add(DeleteAllMenuItem);
                    _filmingPageContextMenu.Items.Add(InsertMenu);
                }

                return _filmingPageContextMenu;
            }
        }
    }
}
