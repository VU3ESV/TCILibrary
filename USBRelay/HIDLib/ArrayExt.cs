using System.Text;

namespace USBRelay.HIDLib
{
    internal static class ArrayExt
    {
        public static string GetString(this byte[] array)
        {
            var str = Encoding.Unicode.GetString(array);
            str = str.Trim('\0');
            return str;
        }
    }
}
