using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Walterlv.Clipboards
{
    /// <summary>
    /// 剪贴板查看器的主窗口。
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// 获取剪贴板数据类型集合。
        /// </summary>
        private readonly ObservableCollection<FormatInfo> _clipboardFormatCollection =
            new ObservableCollection<FormatInfo>();

        /// <summary>
        /// 获取剪贴板数据内容集合。
        /// </summary>
        private readonly ObservableCollection<ClipboardData> _clipboardDataCollection =
            new ObservableCollection<ClipboardData>();

        /// <summary>
        /// 获取剪贴板数据文本报表的拼接器。
        /// </summary>
        private readonly StringBuilder _clipboardInfoBuilder = new StringBuilder();

        /// <summary>
        /// 获取文本报表中的文本分隔线。
        /// </summary>
        private const string SepratorLine = "****************************************************";

        /// <summary>
        /// 获取或设置当前正在查看的剪贴板数据。
        /// </summary>
        private IDataObject? _dataObject;

        /// <summary>
        /// 创建剪贴板查看器主窗口。此方法由 <see cref="App"/> 自动调用。
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            AllFormatsListBox.ItemsSource = _clipboardFormatCollection;
            ClipboardContentListBox.ItemsSource = _clipboardDataCollection;
        }

        /// <summary>
        /// 窗口已布局、已呈现且可用于交互时发生。
        /// 此时显示当前剪贴板中的内容。
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs args)
        {
            GetClipboard();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateObjectTypesUI();
        }

        private void CheckClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            GetClipboard();
        }

        private void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            AllFormatsListBox.SelectedItem = ((Button) sender).DataContext;
        }

        private void AllFormatsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = AllFormatsListBox.SelectedIndex;
            if (index > 0)
            {
                ClipboardContentListBox.SelectedIndex = index;
                ClipboardContentListBox.ScrollIntoView(ClipboardContentListBox.SelectedItem);
            }
        }

        /// <summary>
        /// 当拖拽数据到侧边栏时发生，此时获取拖拽的数据，并按照剪贴板查看方式显示。
        /// </summary>
        private void SideBar_OnDrop(object sender, DragEventArgs e)
        {
            _dataObject = e.Data;
            ContainsAudioCheckBox.IsEnabled = false;
            ContainsFileDropListCheckBox.IsEnabled = false;
            ContainsImageCheckBox.IsEnabled = false;
            ContainsTextCheckBox.IsEnabled = false;
            CheckCurrentDataObject();
        }

        /// <summary>
        /// 获取剪贴板内容并显示。
        /// </summary>
        private void GetClipboard()
        {
            _dataObject = Clipboard.GetDataObject();
            UpdateObjectTypesUI();
            CheckCurrentDataObject();
        }

        /// <summary>
        /// 检查剪贴板内容，并将包含的数据格式呈现在界面上。
        /// </summary>
        private void UpdateObjectTypesUI()
        {
            ContainsAudioCheckBox.IsEnabled = Clipboard.ContainsAudio();
            ContainsFileDropListCheckBox.IsEnabled = Clipboard.ContainsFileDropList();
            ContainsImageCheckBox.IsEnabled = Clipboard.ContainsImage();
            ContainsTextCheckBox.IsEnabled = Clipboard.ContainsText();
        }

        /// <summary>
        /// 清空剪贴板数据。
        /// </summary>
        private void ClearClipboard(Object sender, RoutedEventArgs args)
        {
            Clipboard.Clear();
            UpdateObjectTypesUI();
            GetClipboard();
        }

        /// <summary>
        /// 将剪贴板中当前查看到的数据以文本报表的形式复制到剪贴板上。
        /// </summary>
        private void CopyClipboardInfo(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_clipboardInfoBuilder.ToString());
        }

        /// <summary>
        /// 检查 <see cref="DataObject"/> 中的数据内容并呈现在界面上。
        /// </summary>
        private void CheckCurrentDataObject()
        {
            // 清空呈现在界面上的列表数据。
            _clipboardFormatCollection.Clear();
            _clipboardDataCollection.Clear();
            _clipboardInfoBuilder.Clear();

            if (_dataObject == null)
            {
                // 当剪贴板中不存在数据时，设置报表数据。
                _clipboardInfoBuilder.Append("DataObject = null.");
            }
            else
            {
                // 当剪贴板中不存在数据时。

                // 设置报表数据。
                _clipboardInfoBuilder.Append("Clipboard DataObject Type: ");
                _clipboardInfoBuilder.AppendLine(_dataObject.GetType().ToString());
                _clipboardInfoBuilder.AppendLine();
                _clipboardInfoBuilder.AppendLine(SepratorLine);
                _clipboardInfoBuilder.AppendLine();
                _clipboardInfoBuilder.AppendLine(
                    "The following data formats are present in the data object obtained from the clipboard:");

                // 获取剪贴板中粘贴所支持的全部类型，并呈现在数据类型列表中。
                string[] formats = _dataObject.GetFormats();
                if (formats.Any())
                {
                    foreach (string format in formats)
                    {
                        if (_dataObject.GetDataPresent(format, false))
                        {
                            _clipboardInfoBuilder.AppendLine("\t- " + format + " (native)");
                            _clipboardFormatCollection.Add(new FormatInfo(ConvertType.Native, format));
                        }
                        else
                        {
                            _clipboardInfoBuilder.AppendLine("\t- " + format + " (autoconvertable)");
                            _clipboardFormatCollection.Add(new FormatInfo(ConvertType.AutoConvertable, format));
                        }
                    }
                }
                else
                {
                    _clipboardInfoBuilder.AppendLine("\t- no data formats are present");
                }

                // 将剪贴板中粘贴所支持的全部类型呈现在数据内容列表中。
                foreach (string format in formats)
                {
                    _clipboardInfoBuilder.AppendLine();
                    _clipboardInfoBuilder.AppendLine(SepratorLine);
                    _clipboardInfoBuilder.AppendLine(format + " data:");
                    _clipboardInfoBuilder.Append(" - ");

                    ClipboardData clipboardData = new ClipboardData {DataType = format};
                    try
                    {
                        object data = _dataObject.GetData(format, true);
                        if (data == null)
                        {
                            _clipboardInfoBuilder.Append("null");
                            continue;
                        }
                        if (data is IEnumerable enumerable && !(data is string))
                        {
                            _clipboardInfoBuilder.AppendLine(enumerable.ToString());
                            _clipboardInfoBuilder.AppendLine("{");
                            foreach (object item in enumerable)
                            {
                                _clipboardInfoBuilder.AppendLine("    " + item + ",");
                            }
                            _clipboardInfoBuilder.Append("}");
                        }
                        else
                        {
                            _clipboardInfoBuilder.Append(data);
                        }
                        clipboardData.Data = data;
                        _clipboardDataCollection.Add(clipboardData);
                    }
                    catch (Exception ex)
                    {
                        _clipboardInfoBuilder.Append(ex.Message);
                        clipboardData.Exception = ex;
                        _clipboardDataCollection.Add(clipboardData);
                    }
                }
            }
        }
    }
}
