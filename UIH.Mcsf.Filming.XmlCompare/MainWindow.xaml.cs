using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace UIH.Mcsf.Filming.XmlCompare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        class DataSource
        {
            public string Tag { get; set; }
            public string Name { get; set; }
            public DataSource(string tag, string name)
            {
                this.Tag = tag;
                this.Name = name;
            }
        }
        class FileNameSource
        {
            public string SafeFileName { get; set; }
            public string FileName { get; set; }
        }

        Dictionary<string, string> TagNameDicSource;
        Dictionary<string, string> TagNameDicTarget;
        ObservableCollection<DataSource> DataSourceList1;
        ObservableCollection<DataSource> DataSourceList2;
        ObservableCollection<FileNameSource> FileNameSourceList1;
        ObservableCollection<FileNameSource> FileNameSourceList2;

        public MainWindow()
        {
            InitializeComponent();
            TagNameDicSource = new Dictionary<string, string>();
            TagNameDicTarget = new Dictionary<string, string>();
            DataSourceList1 = new ObservableCollection<DataSource>();
            DataSourceList2 = new ObservableCollection<DataSource>();
            FileNameSourceList1 = new ObservableCollection<FileNameSource>();
            FileNameSourceList2 = new ObservableCollection<FileNameSource>();
           
        }

        private void chooseBtn1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "xml文件|*.xml|所有文件|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            openFileDialog.DefaultExt = "xml";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            int len = openFileDialog.FileNames.Length;
            string[] fileNames = new string[len];
            fileNames = openFileDialog.FileNames;
            FileNameSourceList1.Clear();
            TagNameDicSource.Clear();
            for (int i = 0; i < len; i++)
            {
                FileNameSource fs= new FileNameSource();
                fs.SafeFileName = openFileDialog.SafeFileNames[i];
                fs.FileName = fileNames[i];
                FileNameSourceList1.Add(fs);
            }
            this.listBox1.ItemsSource = FileNameSourceList1;              //为listBox1添加数据绑定
            
            for (int i = 0; i < len; i++)
            {
                try 
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(fileNames[i]);
                    XmlNodeList xmlListSource = doc.GetElementsByTagName("item");
                    foreach (XmlNode node in xmlListSource)
                    {
                        XmlElement element = (XmlElement)node;
                        string tag = (string)element.GetAttribute("Tag");
                        string name = (string)element.GetAttribute("Name");
                        if (!TagNameDicSource.ContainsKey(tag))
                        {
                            TagNameDicSource.Add(tag, name);
                        }
                        
                    }
                }
                catch
                {
                    System.Windows.MessageBox.Show("配置文件存在异常");
                }
                
            }
            
        }

        private void chooseBtn2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "xml文件|*.xml|所有文件|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            openFileDialog.DefaultExt = "xml";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            int len = openFileDialog.FileNames.Length;
            string[] fileNames = new string[len];
            fileNames = openFileDialog.FileNames;
            this.listBox2.ItemsSource = openFileDialog.SafeFileNames;
            TagNameDicTarget.Clear();
            FileNameSourceList2.Clear();
            for (int i = 0; i < len; i++)
            {
                FileNameSource fs = new FileNameSource();
                fs.SafeFileName = openFileDialog.SafeFileNames[i];
                fs.FileName = fileNames[i];
                FileNameSourceList2.Add(fs);
            }
            this.listBox2.ItemsSource = FileNameSourceList2;              //为listBox2添加数据绑定

            for (int i = 0; i < len; i++)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(fileNames[i]);
                    XmlElement root = doc.DocumentElement;
                    //XmlNodeList xmlListTarget = doc.GetElementsByTagName("McsfMedViewerTags");
                    foreach (XmlNode node in root.ChildNodes)
                    {
                        string name = node.Name;
                        string tag = node.InnerText;
                        if (name == "#comment")
                            continue;
                        if (!TagNameDicTarget.ContainsKey(tag))
                        {
                            TagNameDicTarget.Add(tag, name);
                        }
                    }
                    
                }
                catch
                {
                    System.Windows.MessageBox.Show("配置文件存在异常");
                }

            }
        }

        private void compareBtn_Click(object sender, RoutedEventArgs e)
        {
            DataSourceList1.Clear();
            DataSourceList2.Clear();
            foreach (var key in TagNameDicSource.Keys)
            {
                if (!TagNameDicTarget.ContainsKey(key))
                {
                    DataSource ds = new DataSource(key,TagNameDicSource[key]);
                    DataSourceList1.Add(ds);
                }
            }
            foreach (var key in TagNameDicTarget.Keys)
            {
                if (!TagNameDicSource.ContainsKey(key))
                {
                    DataSource ds = new DataSource(key, TagNameDicTarget[key]);
                    DataSourceList2.Add(ds);
                }
            }
            this.listView1.ItemsSource = DataSourceList1;
            this.listView2.ItemsSource = DataSourceList2;
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader)
            {
                //Get clicked column
                GridViewColumn clickedColumn = (e.OriginalSource as GridViewColumnHeader).Column;
                if (clickedColumn != null)
                {
                    //Get binding property of clicked column
                    string bindingProperty = (clickedColumn.DisplayMemberBinding as System.Windows.Data.Binding).Path.Path;
                    SortDescriptionCollection sdc = listView1.Items.SortDescriptions;
                    ListSortDirection sortDirection = ListSortDirection.Ascending;
                    if (sdc.Count > 0)
                    {
                        SortDescription sd = sdc[0];
                        sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                        sdc.Clear();
                    }
                    sdc.Add(new SortDescription(bindingProperty, sortDirection));
                }
            }

        }

        private void GridViewColumnHeader2_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader)
            {
                //Get clicked column
                GridViewColumn clickedColumn = (e.OriginalSource as GridViewColumnHeader).Column;
                if (clickedColumn != null)
                {
                    //Get binding property of clicked column
                    string bindingProperty = (clickedColumn.DisplayMemberBinding as System.Windows.Data.Binding).Path.Path;
                    SortDescriptionCollection sdc = listView2.Items.SortDescriptions;
                    ListSortDirection sortDirection = ListSortDirection.Ascending;
                    if (sdc.Count > 0)
                    {
                        SortDescription sd = sdc[0];
                        sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                        sdc.Clear();
                    }
                    sdc.Add(new SortDescription(bindingProperty, sortDirection));
                }
            }
        }

        private void toLeftBtn_Click(object sender, RoutedEventArgs e)
        {
            int fileNum = listBox1.SelectedItems.Count;
            if (fileNum < 1 || this.listView2.SelectedItems.Count < 1)
                return;
            string[] filePath = new string[fileNum];
            for (int i = 0; i < fileNum; i++)
            {
                FileNameSource fs = this.listBox1.SelectedItems[i] as FileNameSource;
                filePath[i] = fs.FileName;
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath[i]);
                XmlElement root = doc.DocumentElement;
                
                string str = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var comment = doc.CreateComment("Create at " + str + " by XMLCompare.exe");
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Contains("DicomTags"))
                    {
                        node.AppendChild(comment);
                    }
                }
                
                foreach (var selectedItem in this.listView2.SelectedItems)
                {
                    DataSource ds = selectedItem as DataSource;
                    var items = doc.CreateElement("item");
                    var Tag = XMLHelper.AddAttribute(doc, items, "Tag", ds.Tag);
                    var Name = XMLHelper.AddAttribute(doc, items, "Name", ds.Name);
                    var VR = XMLHelper.AddAttribute(doc, items, "VR", "");
                    var FontFamily = XMLHelper.AddAttribute(doc, items, "FontFamily", "Arial");
                    var FontSize = XMLHelper.AddAttribute(doc, items, "FontSize", "12");
                    var Color = XMLHelper.AddAttribute(doc, items, "Color", "255,255,255,255");
                    var FormatHead = XMLHelper.AddAttribute(doc, items, "FormatHead", "");
                    var FormatType = XMLHelper.AddAttribute(doc, items, "FormatType", "");
                    var FormatSuffix = XMLHelper.AddAttribute(doc, items, "FormatSuffix", "");
                    foreach (XmlNode node in root.ChildNodes)
                    {
                        if (node.Name.Contains("DicomTags"))
                        {
                            node.AppendChild(items);
                        }
                    }
                   
                }
                doc.Save(filePath[i]);
            }
            for (int k = 0; k < this.listView2.SelectedItems.Count;k++ )
            {
                DataSource ds = this.listView2.SelectedItems[k] as DataSource;
                TagNameDicSource.Add(ds.Tag, ds.Name);
                DataSourceList2.Remove(ds);                    //将ListView2中的删除,只留下ListView1中没有的
                //this.listView2.ItemsSource = DataSourceList2;
            }
        }

        private void toRightBtn_Click(object sender, RoutedEventArgs e)
        {
            //将listView1中的某项加入到listBox2的文件中，同时更新listView1
            int fileNum = listBox2.SelectedItems.Count;
            if (fileNum < 1 || this.listView1.SelectedItems.Count<1)
                return;
            string[] filePath = new string[fileNum];

            //ObservableCollection<Object> temp = new ObservableCollection<Object>();
            //temp = (ObservableCollection<Object>)this.listView1.SelectedItems;
            for (int i = 0; i < fileNum; i++)
            {
                FileNameSource fs = this.listBox2.SelectedItems[i] as FileNameSource;
                filePath[i] = fs.FileName;
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath[i]);
                XmlElement root = doc.DocumentElement;
                string str = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");       //添加XmlComment
                var comment = doc.CreateComment("Create at " + str + " by XMLCompare.exe");
                root.AppendChild(comment);
                
                foreach (var selectedItem in this.listView1.SelectedItems)
                {
                    DataSource ds = selectedItem as DataSource;
                    
                    string name = ds.Name.Replace(" ", "");
                    XmlNode newNode = root.SelectSingleNode(name);
                    if (newNode == null)
                    {
                        XMLHelper.AddNode(doc, root, name, ds.Tag);
                    }
                }
                doc.Save(filePath[i]);
            }
            for (int k = 0; k < this.listView1.SelectedItems.Count; k++)
            {
                DataSource ds = this.listView1.SelectedItems[k] as DataSource;
                TagNameDicTarget.Add(ds.Tag, ds.Name);
                DataSourceList1.Remove(ds);                    //将ListView1中的删除,只留下ListView2中没有的
                //this.listView1.ItemsSource = DataSourceList1;
            }
        }
    }

    public static class XMLHelper
    {
        public static XmlNode AddNode(XmlDocument dom, XmlNode ParentNode, string NodeName, string NodeValue)
        {
            XmlElement Elem = dom.CreateElement(NodeName);
            ParentNode.AppendChild(Elem);
            Elem.InnerText = NodeValue;
            return Elem;
        }
        public static XmlAttribute AddAttribute(XmlDocument dom, XmlNode ParentNode, string AttrName, string AttrValue)
        {
            XmlAttribute attr = dom.CreateAttribute(AttrName);
            ParentNode.Attributes.Append(attr);
            attr.Value = AttrValue;
            return attr;
        }
    }
}
