using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace Walterlv.Clipboards
{
    /// <summary>
    /// 表示剪贴板操作的辅助类。
    /// </summary>
    public static class ClipboardHelper
    {
        private const int DefaultRetryTimes = 3;

        /// <summary>
        /// 获取剪贴板数据，在获取失败时会自动尝试。
        /// </summary>
        /// <param name="retryTimes">当获取失败时自动尝试的次数。</param>
        /// <returns>从剪贴板中获取的数据，如果剪贴板中无内容，则为 null。</returns>
        public static IDataObject? GetDataObject(int retryTimes = DefaultRetryTimes)
        {
            return TryDo(() => Clipboard.GetDataObject(), retryTimes);
        }

        /// <summary>
        /// 设置剪贴板数据，在设置失败时会自动尝试。
        /// </summary>
        /// <param name="dataObject">要放入剪贴板中的数据。</param>
        /// <param name="retryTimes">当设置失败时自动尝试的次数。</param>
        public static void SetDataObject(IDataObject dataObject, int retryTimes = DefaultRetryTimes)
        {
            TryDo(() => Clipboard.SetDataObject(dataObject), retryTimes);
        }

        public static object? TryGetData(this IDataObject dataObject, Type format, int retryTimes = DefaultRetryTimes)
        {
            return TryDo(() => dataObject.GetData(format), retryTimes);
        }

        public static object? TryGetData(this IDataObject dataObject, string format, int retryTimes = DefaultRetryTimes)
        {
            return TryDo(() => dataObject.GetData(format), retryTimes);
        }

        public static object? TryGetData(this IDataObject dataObject, string format, bool autoConvert,
            int retryTimes = DefaultRetryTimes)
        {
            return TryDo(() => dataObject.GetData(format, autoConvert), retryTimes);
        }

        public static void TrySetData(this IDataObject dataObject, Type format, object data,
            int retryTimes = DefaultRetryTimes)
        {
            TryDo(() => dataObject.SetData(format, data), retryTimes);
        }

        public static void TrySetData(this IDataObject dataObject, object data, int retryTimes = DefaultRetryTimes)
        {
            TryDo(() => dataObject.SetData(data), retryTimes);
        }

        public static void TrySetData(this IDataObject dataObject, string format, object data,
            int retryTimes = DefaultRetryTimes)
        {
            TryDo(() => dataObject.SetData(format, data), retryTimes);
        }

        public static void TrySetData(this IDataObject dataObject, string format, object data, bool autoConvert,
            int retryTimes = DefaultRetryTimes)
        {
            TryDo(() => dataObject.SetData(format, data, autoConvert), retryTimes);
        }

        /// <summary>
        /// 尝试执行剪贴板操作，在发生 <see cref="ExternalException"/> 时自动重试。
        /// </summary>
        /// <param name="action">进行的剪贴板操作。</param>
        /// <param name="retryTimes">当操作失败时重试的次数。</param>
        private static void TryDo(Action action, int retryTimes = DefaultRetryTimes)
        {
            int loopTimes = retryTimes < 0 ? 1 : retryTimes + 1;
            for (int i = 0; i < loopTimes; i++)
            {
                try
                {
                    action();
                    return;
                }
                catch (ExternalException)
                {
                    Thread.Sleep(10);
                }
            }
        }

        /// <summary>
        /// 尝试执行剪贴板操作，在发生 <see cref="ExternalException"/> 时自动重试。
        /// </summary>
        /// <typeparam name="T">剪贴板操作的返回值类型。</typeparam>
        /// <param name="func">进行的剪贴板操作。</param>
        /// <param name="retryTimes">当操作失败时重试的次数。</param>
        /// <returns>剪贴板操作的返回值。</returns>
        private static T? TryDo<T>(Func<T> func, int retryTimes = DefaultRetryTimes)
            where T : class
        {
            int loopTimes = retryTimes < 0 ? 1 : retryTimes + 1;
            for (int i = 0; i < loopTimes; i++)
            {
                try
                {
                    return func();
                }
                catch (ExternalException)
                {
                    Thread.Sleep(10);
                }
            }
            return default;
        }
    }
}