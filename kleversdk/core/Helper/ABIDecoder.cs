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

        private const int LengthHexSize = 8;
        private const int BitsByHexDigit = 4;

        private const int AddressSize = 64;
        private const int INT64Size = 16;
        private const int INT32Size = 8;
        private const int INT16Size = 4;
        private const int INT8Size = 2;

        private static Tuple<string, string> CheckDelimiter(string type)
        {
            var nestedType = type;

            if (type.StartsWith("List<"))
            {
                type = type.Substring(0, 4);
                nestedType = nestedType.Substring(5, nestedType.Length - type.Length - 2);

                return Tuple.Create(type, nestedType);
            }

            return null;
        }

        public static object SelectDecoder(string hex, string type, bool isNested = false)
        {

            var tupleType = CheckDelimiter(type);

            if (tupleType != null)
            {
                type = tupleType.Item1;
            }
            
            switch (type)
            {
                case "List":
                    return DecodeList(hex, tupleType.Item2);
                case "Option":
                    return DecodeOption(hex, tupleType.Item2);
                case "tuple":
                    throw new Exception($"not implemented type: {type}");
                case "variadic":
                    throw new Exception($"not implemented type: {type}");
                default:
                    return DecodeSingleValue(hex, type, isNested);
            }
        }

        public static object DecodeOption(string hex, string type)
        {
            if (hex.StartsWith("00"))
            {
                return "";
            }

            return SelectDecoder(hex, type, true);
        }

        public static int GetTypeSize(string type)
        {
            switch (type)
            {
                case "i64":
                case "u64":
                    return INT64Size;
                case "i32":
                case "u32":
                case "usize":
                case "isize":
                    return INT32Size;
                case "i16":
                case "u16":
                    return INT16Size;
                case "i8":
                case "u8":
                    return INT8Size;
                case "Address":
                    return AddressSize;
                default:
                    return LengthHexSize;
            }
        }

        public static Tuple<string,string> ListHandleValue(string hex, string type)
        {
            string toDecode = "";

            var typeSize = GetTypeSize(type);

            switch (type)
            {
                case "BigInt":
                case "BigUint":
                case "String":
                case "ManagedBuffer":
                case "BoxedBytes":
                case "bytes":
                case "TokenIdentifier":
                    Int32 hexLen = (Int32)DecodeInt(hex.Substring(0, typeSize), typeSize * BitsByHexDigit);
                    var cutLen = typeSize + 2 * hexLen;

                    toDecode = hex.Substring(0, cutLen);

                    hex = hex.Substring(cutLen);
                    break;
                case "i64":
                case "u64":
                case "i32":
                case "u32":
                case "usize":
                case "isize":
                case "i16":
                case "u16":
                case "i8":
                case "u8":
                case "Address":
                    toDecode = hex.Substring(0, typeSize);
                    hex = hex.Substring(typeSize);
                    break;
                default:
                    if (type.StartsWith("List<"))
                    {
                        Int32 ListSize = (Int32)DecodeInt(hex.Substring(0, LengthHexSize), LengthHexSize * BitsByHexDigit);

                        var auxHex = hex;

                        var newType = type.Substring(5, type.Length - 5 - 1);

                        var cutSize = GetTypeSize(newType);

                        auxHex = auxHex.Substring(8);
                        var totalLen = 0;

                        for (int i = 0; i < ListSize; i++)
                        {
                            Int32 auxHexLen = (Int32)DecodeInt(auxHex.Substring(0, LengthHexSize), LengthHexSize * BitsByHexDigit);

                            auxHex = auxHex.Substring(8);
                            auxHex = auxHex.Substring(auxHexLen * 2);
                            totalLen += 8 + (2 * auxHexLen);
                        }

                        toDecode = hex.Substring(8, totalLen);
                        hex = hex.Substring(8 + totalLen);
                    }
                    break;
                }

            return Tuple.Create(hex, toDecode);
        }


        public static object DecodeList(string hex, string type)
        {
            List<object> result = new List<object>();

            do
            { 
                var tupleDecode = ListHandleValue(hex, type);

                var target = SelectDecoder(tupleDecode.Item2, type, true);
                result.Add(target);

                hex = tupleDecode.Item1;
            } while (hex.Length > 0);
            return result;
        }

        public static object DecodeSingleValue(string hex, string type, bool isNested = false)
        {
             switch (type)
            {
                case "BigInt":
                    return DecodeBigInt(hex, false, isNested);
                case "BigUint":
                    return DecodeBigInt(hex, true, isNested);
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


        public static BigInteger DecodeBigInt(string hex, bool isUnsigned = false,bool isNested = false)
        {

            if (isNested)
            {
                int len = int.Parse(hex.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);

                hex = hex.Substring(8, len * 2);
            }

            var targetString = DecodeString(hex);
            //check if is a string representing a decimal number
            if (BigInteger.TryParse(targetString, NumberStyles.Number, null, out BigInteger targetValue))
            {
                return targetValue;
            }

            var bytes = Converter.FromHexString(hex);
            var bigIntegerParsed = Converter.ToBigInteger(bytes, isUnsigned, true);

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

