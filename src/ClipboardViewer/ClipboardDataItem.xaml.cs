using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Win32;

namespace Walterlv.Clipboards
{
    /// <summary>
    /// 表示用于显示到界面上剪贴板数据列表中的数据列表项。
    /// </summary>
    public partial class ClipboardDataItem
    {
        /// <summary>
        /// 创建一个剪贴板数据列表项。此构造方法由 <see cref="ListBox"/> 自动调用。
        /// </summary>
        public ClipboardDataItem()
        {
            InitializeComponent();
        }

        private void ExportToFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.ShowDialog();
            var fileName = saveFileDialog.FileName;
            if (string.IsNullOrEmpty(fileName)) return;

            try
            {
                var file = new FileInfo(fileName);
                if (file.Directory is null)
                {
                    return;
                }

                file.Directory.Create();

                var clipboardData = (ClipboardData) DataContext;
                if (clipboardData.Data is MemoryStream memory)
                {
                    using var fileStream = file.OpenWrite();
                    memory.CopyTo(fileStream);
                }
                else if (clipboardData.Data is { } data)
                {
                    File.WriteAllText(file.FullName, data.ToString());
                }
            }
            catch (Exception)
            {
                // 写文件写错啥的，忽略就好了
            }
        }
    }

    /// <summary>
    /// 依据对象是否为 null 得到 <see cref="Visibility"/> 的转换器。
    /// 与 <see cref="ClipboardDataItem"/> 配合使用。
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 如果对象为 null，则返回 <see cref="Visibility.Collapsed"/>，否则返回 <see cref="Visibility.Visible"/>。
        /// 此方法由 <see cref="Binding"/> 自动调用。
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 不支持从 <see cref="Visibility"/> 转换为对象。
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 从剪贴板数据格式转化为界面显示颜色的转换器。
    /// 与 <see cref="ClipboardDataItem"/> 配合使用。
    /// </summary>
    [ValueConversion(typeof(string), typeof(Brush))]
    public class FormatToBrushConverter : IValueConverter
    {
        /// <summary>
        /// 根据剪贴板中数据类型（音频、文件、图像、文本）转换成表示其类型的颜色。
        /// 此方法由 <see cref="Binding"/> 自动调用。
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = value as string;
            if (format != null)
            {
                switch (format)
                {
                    case "":
                        return Application.Current.FindResource("Clipboard.AudioBrush");
                    case "FileDrop":
                    case "FileNameW":
                    case "FileName":
                        return Application.Current.FindResource("Clipboard.FileListBrush");
                    case "PNG":
                    case "JFIF":
                    case "GIF":
                    case "Bitmap":
                    case "System.Drawing.Bitmap":
                    case "System.Windows.Media.Imaging.BitmapSource":
                    case "DeviceIndependentBitmap":
                        return Application.Current.FindResource("Clipboard.ImageBrush");
                    case "Text":
                    case "UnicodeText":
                    case "System.String":
                    case "HTML Format":
                    case "Rich Text Format":
                        return Application.Current.FindResource("Clipboard.TextBrush");
                    default:
                        return Application.Current.FindResource("Clipboard.OtherBrush");
                }
            }
            return Application.Current.FindResource("Clipboard.OtherBrush");
        }

        /// <summary>
        /// 不支持从 <see cref="Brush"/> 转换为数据类型。
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 从剪贴板数据的转化类型转化为界面显示颜色的转换器。
    /// 与 <see cref="ClipboardDataItem"/> 配合使用。
    /// </summary>
    [ValueConversion(typeof(ConvertType), typeof(string))]
    public class FormatTypeToBrushConverter : IValueConverter
    {
        /// <summary>
        /// 根据剪贴板数据转化类型（原生支持或可自动转化）转换成表示其类型的颜色。
        /// 此方法由 <see cref="Binding"/> 自动调用。
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConvertType format = (ConvertType)value;
            switch (format)
            {
                case ConvertType.Native:
                    return Application.Current.FindResource("Clipboard.Format.Native");
                case ConvertType.AutoConvertable:
                    return Application.Current.FindResource("Clipboard.Format.AutoConvertable");
            }
            throw new NotSupportedException("FormatType can only be Native or AutoConvertale.");
        }

        /// <summary>
        /// 不支持从 <see cref="Brush"/> 转换为数据转化类型。
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 将剪贴板中的数据内容转化为更友好文本的转换器。
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class DataToStringConverter : IValueConverter
    {
        /// <summary>
        /// 将剪贴板中的数据内容转化为更友好的文本。
        /// 此方法由 <see cref="Binding"/> 自动调用。
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return String.Empty;
            }
            StringBuilder builder = new StringBuilder();
            if (value is IEnumerable enumerable && !(value is string))
            {
                builder.AppendLine(enumerable.ToString());
                builder.AppendLine("{");
                foreach (object item in enumerable)
                {
                    builder.AppendLine(string.Format("    {0},", item));
                }
                builder.Append("}");
            }
            else
            {
                builder.Append(value);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 不支持从 <see cref="Brush"/> 转换为数据转化类型。
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
