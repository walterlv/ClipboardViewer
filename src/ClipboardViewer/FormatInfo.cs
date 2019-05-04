using System.Windows;

namespace Walterlv.Clipboards
{
    public class FormatInfo : DependencyObject
    {
        /// <summary>
        /// 创建表示剪贴板一个格式数据类型的依赖项。
        /// </summary>
        /// <param name="formatType">数据转化类型。</param>
        /// <param name="formatName">数据类型名。</param>
        public FormatInfo(ConvertType formatType, string formatName)
        {
            ConvertType = formatType;
            FormatName = formatName;
        }

        /// <summary>
        /// 标识 <see cref="ConvertType"/> 的依赖项属性。
        /// </summary>
        public static readonly DependencyProperty FormatTypeProperty = DependencyProperty.Register(
            "ConvertType", typeof (ConvertType), typeof (FormatInfo), new PropertyMetadata(default(ConvertType)));

        /// <summary>
        /// 标识 <see cref="FormatName"/> 的依赖项属性。
        /// </summary>
        public static readonly DependencyProperty FormatNameProperty = DependencyProperty.Register(
            "FormatName", typeof (string), typeof (FormatInfo), new PropertyMetadata(default(string)));

        /// <summary>
        /// 表示剪贴板的数据转化类型。
        /// </summary>
        public ConvertType ConvertType
        {
            get { return (ConvertType) GetValue(FormatTypeProperty); }
            set { SetValue(FormatTypeProperty, value); }
        }

        /// <summary>
        /// 表示剪贴板的数据类型在剪贴板中的表示名称。
        /// </summary>
        public string FormatName
        {
            get { return (string) GetValue(FormatNameProperty); }
            set { SetValue(FormatNameProperty, value); }
        }
    }

    /// <summary>
    /// 表示剪贴板的数据转化类型。
    /// </summary>
    public enum ConvertType
    {
        /// <summary>
        /// 表示数据是剪贴板中自带的。
        /// </summary>
        Native,

        /// <summary>
        /// 表示数据是从剪贴板中的其它数据格式转化而来的。
        /// </summary>
        AutoConvertable,
    }
}
