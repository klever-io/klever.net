using System;
namespace kleversdk.provider.Helper
{
    public static class StringExtensions
    {
        public static string ToHex(this byte[] bytes)
        {
            return Converter.ToHexString(bytes).ToLowerInvariant();
        }

        public static byte[] FromHex(this string value)
        {
            return Converter.FromHexString(value);
        }
    }
}
