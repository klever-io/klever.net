using System;
using kleversdk.provider.Helper;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Globalization;

namespace kleversdk.core.Helper
{
    //public class DecodeResult
    //{
    //    public DecodeResult()
    //    {
    //    }

    //    public DecodeResult(string hex, dynamic data)
    //    {
    //        this.NewHex = hex;
    //        this.Data = data;
    //    }

    //    public DecodeResult(string hex, string error)
    //    {
    //        this.NewHex = hex;
    //        this.Error = error;
    //    }

    //    public DecodeResult(string hex, List<dynamic> dataArray)
    //    {
    //        this.NewHex = hex;
    //        this.DataArray = dataArray;
    //    }

    //    public dynamic Data { get; set; }
    //    public List<dynamic> DataArray { get; set; }
    //    public string NewHex { get; set; }
    //    public string Error { get; set; }

    //}

    public class ABIDecoder
	{
		public ABIDecoder()
		{
		}

        public static object SelectDecoder(JsonABI abi, string hex, string type)
        {
            switch (type)
            {
                case "List":
                    throw new Exception($"not implemented type: {type}");
                case "Option":
                    throw new Exception($"not implemented type: {type}");
                case "tuple":
                    throw new Exception($"not implemented type: {type}");
                case "variadic":
                    throw new Exception($"not implemented type: {type}");
                default:
                    return DecodeSingleValue(hex, type);
            }
        }

        public static object DecodeSingleValue(string hex, string type, bool isNested = false)
        {
             switch (type)
            {
                case "BigInt":
                case "BigUint":
                    return DecodeBigInt(hex, isNested);
                case "i64":
                    return DecodeInt(hex, 64);
                case "i32":
                case "isize":
                    return DecodeInt(hex, 32);
                case "i16":
                    return DecodeInt(hex, 16);
                case "i8":
                    return DecodeInt(hex, 8);
                case "u64":
                    return DecodeUint(hex, 64);
                case "u32":
                case "usize":
                    return DecodeUint(hex, 32);
                case "u16":
                    return DecodeUint(hex, 16);
                case "u8":
                    return DecodeUint(hex, 8);
                case "bool":
                    return DecodeBool(hex);
                case "Address":
                    return DecodeAddress(hex);
                case "ManagedBuffer":
                case "TokenIdentifier":
                case "bytes":
                case "BoxedBytes":
                case "String":
                case "&str":
                case "Vec<u8>":
                case "&[u8]":
                case "string":
                    return DecodeString(hex, isNested);
                default:
                    throw new Exception($"Invalid type: {type}");
            }
        }

        private static BigInteger DecodeStringBigNumber(string hex) {
            byte[] asciiBytes = Encoding.ASCII.GetBytes(hex);
            return new BigInteger(asciiBytes);
        }

        public static BigInteger DecodeBigInt(string hex, bool isNested = false)
        {
            var targetString = DecodeString(hex);
            //check if is a string representing a decimal number
            if (BigInteger.TryParse(targetString, NumberStyles.Number,null, out BigInteger targetValue))
            {
                    return targetValue;
            }
          
            if (isNested)
            {
                int len = int.Parse(hex.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);

                hex = hex.Substring(8, len * 2);
            }

            var bytes = Converter.FromHexString(hex);
            var bigIntegerParsed = Converter.ToBigInteger(bytes, false, true);

            return bigIntegerParsed;
        
        }

        public static object DecodeInt(string hex, int size) {

            var bytes = Converter.FromHexString(hex);
            var bigIntegerParsed = Converter.ToBigInteger(bytes, false, true);

            switch (size)
            {
                case 64:
                    return Int64.Parse(bigIntegerParsed.ToString());
                case 32:
                    return Int32.Parse(bigIntegerParsed.ToString());
                case 16:
                    return Int16.Parse(bigIntegerParsed.ToString());
                case 8:
                    return sbyte.Parse(bigIntegerParsed.ToString());
                default:
                    throw new Exception($"can't decode type: Int{size}");
            }

        }

        public static object DecodeUint(string hex, int size)
        {

            var bytes = Converter.FromHexString(hex);
            var bigIntegerParsed = Converter.ToBigInteger(bytes, true, true);

            switch (size)
            {
                case 64:
                    return UInt64.Parse(bigIntegerParsed.ToString());
                case 32:
                    return UInt32.Parse(bigIntegerParsed.ToString());
                case 16:
                    return UInt16.Parse(bigIntegerParsed.ToString());
                case 8:
                    return byte.Parse(bigIntegerParsed.ToString());
                default:
                    throw new Exception($"can't decode type: UInt{size}");
            }

        }


        public static bool DecodeBool(string hex)
        {
            return hex == "01";
        }

        public static string DecodeAddress(string hex)
        {
            return core.Address.FromHex(hex).Bech32;
        }

        public static string DecodeString(string hex, bool isNested = false)
        {

            if (isNested)
            {
                int len = int.Parse(hex.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);

                hex = hex.Substring(8, len * 2);
            }

            byte[] decodedBytes = new byte[hex.Length / 2];
            for (int i = 0; i < decodedBytes.Length; i++)
            {
                decodedBytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return Encoding.UTF8.GetString(decodedBytes);
        }








        //public static string DecodeTuple(JsonABI abi, string hex, string type)
        //{

        //	return "";
        //}

        //      private static DecodeResult DecodeListValue(JsonABI abi, string hex, string type)
        //      {

        //          if (type.StartsWith("Option<"))
        //          {
        //              var some = hex.Substring(0, 2) == "01";

        //              hex = hex.Substring(2, hex.Length);

        //              if (!some)
        //              {
        //                  return new DecodeResult(hex: hex, data: null);
        //              }

        //              type = type.Substring(7, -1);

        //          }

        //          // parse Int

        //          // slice again

        //          // new hex

        //          // for all something -> decodeListHandle


        //          return new DecodeResult();

        //      }

        //      private static DecodeResult DecodeListHandle(JsonABI abi, string hex, string type)
        //      {
        //          if (type.StartsWith("List<"))
        //          {
        //              var decoded = DecodeListValue(abi, hex, type);

        //              if (decoded.Error.Length > 0)
        //              {
        //                  return new DecodeResult(hex: hex, error: decoded.Error);
        //              }

        //              hex = decoded.NewHex;

        //              return new DecodeResult(hex: hex, dataArray: decoded.DataArray);
        //          }

        //          // typeDefinition


        //          return null;
        //      }

        //      public static List<object> DecodeList(JsonABI abi, string hex, string type)
        //      {
        //          List<object> result = new List<object>();

        //          if (type.StartsWith("Option<")) {
        //              var some = hex.Substring(0, 2) == "01";

        //              hex = hex.Substring(2, hex.Length);

        //              if (!some)
        //              {
        //                  result.Add(new DecodeResult(hex: hex, data: null));

        //                  return result;
        //              }

        //              type = type.Substring(7, -1);

        //          }
        //          // check some types

        //          do {
        //              var decoded = DecodeListHandle(abi, hex, type);

        //              if (decoded.DataArray.Count > 0)
        //              {
        //                  result.Add(decoded.DataArray);
        //              } else {
        //                  result.Add(decoded.Data);
        //              }

        //              hex = decoded.NewHex;

        //          } while (hex.Length > 0);


        //          return result;
        //      }

        //      public static List<object> DecodeVariadic(JsonABI abi, string hex, string type)
        //      {
        //          hex = hex.Substring(8, hex.Length);
        //          return DecodeList(abi, hex, type);
        //      }
         }
    }

