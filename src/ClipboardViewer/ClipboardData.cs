using System;
using System.Windows;

namespace Walterlv.Clipboards
{
    /// <summary>
    /// 表示剪贴板中一种格式数据的记录。
    /// </summary>
    public class ClipboardData : DependencyObject
    {
        /// <summary>
        /// 标识 <see cref="DataType"/> 的依赖项属性。
        /// </summary>
        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register(
            "DataType", typeof (string), typeof (ClipboardData), new PropertyMetadata(default(string)));

        /// <summary>
        /// 标识 <see cref="Data"/> 的依赖项属性。
        /// </summary>
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof (object), typeof (ClipboardData), new PropertyMetadata(default(object)));

        /// <summary>
        /// 标识 <see cref="Exception"/> 的依赖项属性。
        /// </summary>
        public static readonly DependencyProperty ExceptionProperty = DependencyProperty.Register(
            "Exception", typeof (Exception), typeof (ClipboardData), new PropertyMetadata(default(Exception)));

        /// <summary>
        /// 表示剪贴板上一种数据的数据类型。如果有字符串别名，则显示为别名；如果没有字符串别名，则显示为类型全名（包含命名空间）。
        /// </summary>
        public string DataType
        {
            get { return (string) GetValue(DataTypeProperty); }
            set { SetValue(DataTypeProperty, value); }
        }

        /// <summary>
        /// 表示剪贴板上一种数据类型的数据内容。
        /// </summary>
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// 表示剪贴板上数据获取失败时出现的异常。
        /// </summary>
        public Exception Exception
        {
            get { return (Exception) GetValue(ExceptionProperty); }
            set { SetValue(ExceptionProperty, value); }
        }
    }
}
