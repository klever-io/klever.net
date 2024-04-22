using System;
using kleversdk.provider.Helper;
using System.Numerics;
using System.Text;
using Newtonsoft.Json.Linq;

namespace kleversdk.core.Helper
{
    public class ABIEncoder
    {
        public ABIEncoder()
        {
        }

        public static string EncodeSingleValue(string value, string type, bool isNested = false)
        {
            switch (type)
            {
                case "bi":
                case "BI":
                case "n":
                case "N":
                case "bigNumber":
                case "BigNumber":
                case "BigInt":
                case "BigUint":
                    return EncodeBigNumber(value, isNested);
                case "i":
                case "I":
                case "i64":
                case "I64":
                case "int64":
                    return EncodeInt64(value, isNested);
                case "u":
                case "U":
                case "u64":
                case "U64":
                case "uint64":
                    return EncodeUint64(value, isNested);
                case "i32":
                case "I32":
                case "int32":
                case "isize":
                    return EncodeInt32(value, isNested);
                case "u32":
                case "U32":
                case "uint32":
                case "usize":
                    return EncodeUint32(value, isNested);
                case "s":
                case "S":
                case "string":
                case "ManagedBuffer":
                case "BoxedBytes":
                case "Vec":
                case "String":
                case "bytes":
                case "&[u8]":
                case "&str":
                case "TokenIdentifier":
                case "List":
                case "Array":
                    return EncodeString(value, isNested);
                case "x":
                case "X":
                case "hex":
                    return EncodeHex(value);
                case "a":
                case "A":
                case "address":
                case "Address":
                    return EncodeAddress(value);
                case "bool":
                    return EncodeBool(value);
                case "0":
                case "e":
                case "E":
                case "empty":
                default:
                    return "";
            }

        }


        private static string AddPad(string value, bool isPositive, int size)
        {
            int encodedSize = size / 4;

            while (true)
            {
                if (value.Length == encodedSize)
                {
                    break;
                }

                if (isPositive)
                {
                    value = "0" + value;
                }
                else
                {
                    value = "F" + value;
                }
            }

            return value;
        }

        private static string EncodeBigNumber(string value, bool isNested)
        {
            var convertedValue = Converter.StringToBigInteger(value);

            var bigNumberParsed = convertedValue.ToString("X");

            if (bigNumberParsed.Length % 2 != 0)
            {
                if (value.Contains("-"))
                {
                    bigNumberParsed = "F" + bigNumberParsed;
                }
                else
                {
                    bigNumberParsed = "0" + bigNumberParsed;
                }
            }

            if (!isNested)
            {
                return bigNumberParsed;
            }

            var len = (bigNumberParsed.Length / 2).ToString("X8");



            return len + bigNumberParsed;
        }

        private static string EncodeInt64(string value, bool isNested)
        {
            var convertedValue = Converter.StringToBigInteger(value);
            var int64Parsed = convertedValue.ToString("X");

            if (int64Parsed.Length % 2 != 0)
            {
                if (convertedValue < 0)
                {
                    int64Parsed = "F" + int64Parsed;
                }
                else
                {
                    int64Parsed = "0" + int64Parsed;
                }
            }

            if (!isNested)
            {
                return int64Parsed;
            }

            return AddPad(int64Parsed, convertedValue >= 0, 64);
        }



        private static string EncodeInt32(string value, bool isNested)
        {
            var convertedValue = Converter.StringToBigInteger(value);
            var int64Parsed = convertedValue.ToString("X");

            if (int64Parsed.Length % 2 != 0)
            {
                if (convertedValue < 0)
                {
                    int64Parsed = "F" + int64Parsed;
                }
                else
                {
                    int64Parsed = "0" + int64Parsed;
                }
            }

            if (!isNested)
            {
                return int64Parsed;
            }

            return AddPad(int64Parsed, convertedValue >= 0, 32);
        }


        private static string EncodeUint64(string value, bool isNested)
        {
            if (value.Contains("-"))
            {
                throw new Exception("Uint64 can't be negative");
            }

            return EncodeInt64(value, isNested);
        }


        private static string EncodeUint32(string value, bool isNested)
        {
            if (value.Contains("-"))
            {
                throw new Exception("Uint32 can't be negative");
            }

            return EncodeInt32(value, isNested);
        }


        private static string EncodeString(string value, bool isNested)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);

            var stringParsed = BitConverter.ToString(bytes).Replace("-", "");

            if (!isNested)
            {
                return stringParsed;
            }

            var len = (stringParsed.Length / 2).ToString("X8");

            return len + stringParsed;

        }

        private static string EncodeHex(string value)
        {
            value = value.Replace("0x", "");

            if (value.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid hex string length: " + value.Length);
            }

            byte[] bytes = new byte[value.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
            }

            return value;
        }

        private static string EncodeBool(string value)
        {
            bool convertedValue = false;

            try
            {
                convertedValue = Boolean.Parse(value);
            }
            catch (Exception err)
            {
                Console.WriteLine($"Error converting boolean {value} : {err.Message}");
            }

            if (convertedValue)
            {
                return "01";
            }
            else
            {
                return "00";
            }
        }

        private static string EncodeAddress(string value)
        {
            var addr = core.Address.FromBech32(value);

            return addr.Hex;
        }
    }
}

