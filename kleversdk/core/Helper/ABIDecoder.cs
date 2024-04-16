using System;
using kleversdk.provider.Helper;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Globalization;
using System.Drawing;

namespace kleversdk.core.Helper
{
    public class DecodeResult
    {
        public DecodeResult(string hex)
        {
            this.Hex = hex;
        }

        public DecodeResult(string hex, List<object> dataArray)
        {
            this.Hex = hex;
            this.DataArray = dataArray;
        }


        public DecodeResult(string hex, string error)
        {
            this.Hex = hex;
            this.Error = error;
        }


        public DecodeResult(string hex, object data)
        {
            this.Hex = hex;
            this.Data = data;
        }

        public object Data { get; set; }
        public List<object> DataArray { get; set; }
        public string Hex { get; set; }
        public string Error { get; set; }
    }

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
        private const int SomeSize = 2;
        private const int BoolSize = 2;


        private const int ListCutLen = 5;
        private const int OptionCutLen = 7;

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

        public static int GetTypeSize(string type)
        {
            switch (type)
            {
                case "i64":
                case "u64":
                case "64":
                    return INT64Size;
                case "i32":
                case "u32":
                case "32":
                case "usize":
                case "isize":
                    return INT32Size;
                case "i16":
                case "u16":
                case "16":
                    return INT16Size;
                case "i8":
                case "u8":
                case "8":
                    return INT8Size;
                case "bool":
                    return BoolSize;
                case "Address":
                    return AddressSize;
                default:
                    return LengthHexSize;
            }
        }

        public static object SelectDecoder(JsonABI abi, string hex, string type, bool isNested = false)
        {

            var tupleType = CheckDelimiter(type);

            if (tupleType != null)
            {
                type = tupleType.Item1;
            }

            switch (type)
            {
                case "List":
                    return DecodeList(abi, hex, tupleType.Item2).DataArray;
                case "Option":
                    return DecodeOption(abi, hex, tupleType.Item2);
                case "tuple":
                    throw new Exception($"not implemented type: {type}");
                case "variadic":
                    return DecodeList(abi, hex, tupleType.Item2).DataArray;
                default:
                    TypeStruct typeStruct = abi.GetType(type);

                    if(typeStruct == null)
                    {
                        return DecodeSingleValue(hex, type, isNested).Data;
                    }

                    return DecodeStruct(abi,hex,typeStruct);
            }
        }

        private static List<object> DecodeStruct(JsonABI abi, string hex, TypeStruct typeStruct)
        {
            List<object> result = new List<object>();


            foreach (Fields field in typeStruct.fields)
            {
                if (field.type.StartsWith("List<"))
                {
                    var decodedList = DecodeListValue(abi, hex, field.type);
                    if (decodedList.Error != null)
                    {
                        throw new Exception(decodedList.Error);
                    }

                    result.Add(decodedList.DataArray); 
                  
                    hex = decodedList.Hex;
                    continue;
                }

                var decoded = DecodeSingleValue(hex, field.type, true);

                result.Add(decoded.Data);
                hex = decoded.Hex;
            }


            return result;
        }

        private static DecodeResult DecodeListValue(JsonABI abi, string hex, string type)
        {
            if (type.StartsWith("Option<"))
            {
                bool some = hex.Substring(0, SomeSize) == "01";
                hex = hex.Substring(SomeSize);

                if (!some)
                {
                    return new DecodeResult(hex);
                }

                type = type.Substring(OptionCutLen, type.Length - OptionCutLen - 1);
            }

            Int32 listSize = (Int32)DecodeInt(hex.Substring(0, LengthHexSize), LengthHexSize * BitsByHexDigit).Data;


            type = type.Substring(ListCutLen, type.Length - ListCutLen - 1);
            hex = hex.Substring(LengthHexSize);


            var result = new List<object>();

            for (int i = 0; i < listSize; i++)
            {
                var decodeResult = DecodeListHandler(abi, hex, type);

                if (decodeResult.DataArray != null)
                {
                    result.Add(decodeResult.DataArray);
                }
                else
                {
                    result.Add(decodeResult.Data);
                }

                hex = decodeResult.Hex;

            }

            return new DecodeResult(hex, result);
        }

        private static DecodeResult DecodeListHandler(JsonABI abi, string hex, string type)
        {
            if (type.StartsWith("List<"))
            {
                DecodeResult decodeResult = DecodeListValue(abi, hex, type);

                if (decodeResult.Error != null)
                {
                    return new DecodeResult(hex, decodeResult.Error);
                }

                hex = decodeResult.Hex;

                return new DecodeResult(hex, decodeResult.DataArray);
            }

            if (type != "struct")
            {
                var decodedValue = DecodeSingleValue(hex, type, true);
                if (decodedValue.Error != null)
                {
                    throw new Exception(decodedValue.Error);
                }

                hex = decodedValue.Hex;
                return new DecodeResult(hex, decodedValue);
            }

            TypeStruct typeStruct = abi.GetType(type);

            if (typeStruct == null)
            {
                throw new Exception($"invalid type: {type}");
            }


            List<object> decodedStruct = DecodeStruct(abi, hex, typeStruct);

            DecodeResult lastResult = decodedStruct[decodedStruct.Count] as DecodeResult;

            return new DecodeResult(lastResult.Hex, decodedStruct);

        }

        private static DecodeResult DecodeList(JsonABI abi, string hex, string type)
        {
            if (type.StartsWith("Option<"))
            {
                bool some = hex.Substring(0, SomeSize) == "01";
                hex = hex.Substring(SomeSize);

                if (!some)
                {
                    return new DecodeResult(hex);
                }

                type = type.Substring(OptionCutLen, type.Length - OptionCutLen - 1);
            }



            List<object> result = new List<object>();

            do
            {
                DecodeResult decodeResult = DecodeListHandler(abi, hex, type);

                if (decodeResult.DataArray != null)
                {
                    result.Add(decodeResult.DataArray);
                }
                else
                {
                    result.Add(decodeResult.Data);
                }

                hex = decodeResult.Hex;


            } while (hex.Length > 0);

            return new DecodeResult(hex, result as List<object>);
        }

        public static object DecodeOption(JsonABI abi, string hex, string type, bool isNested = false) {

                bool some = hex.Substring(0, SomeSize) == "01";
                hex = hex.Substring(SomeSize);

                if (!some)
                {
                    return new DecodeResult(hex);
                }

                type = type.Substring(OptionCutLen, type.Length - OptionCutLen - 1);


            return SelectDecoder(abi,hex,type,isNested);
        }

        public static DecodeResult DecodeSingleValue(string hex, string type, bool isNested = false)
        {
            switch (type)
            {
                case "BigInt":
                    return DecodeBigInt(hex, false, isNested);
                case "BigUint":
                    return DecodeBigInt(hex, true, isNested);
                case "i64":
                    return DecodeInt(hex, 64, isNested);
                case "i32":
                case "isize":
                    return DecodeInt(hex, 32, isNested);
                case "i16":
                    return DecodeInt(hex, 16, isNested);
                case "i8":
                    return DecodeInt(hex, 8, isNested);
                case "u64":
                    return DecodeUint(hex, 64, isNested);
                case "u32":
                case "usize":
                    return DecodeUint(hex, 32, isNested);
                case "u16":
                    return DecodeUint(hex, 16, isNested);
                case "u8":
                    return DecodeUint(hex, 8, isNested);
                case "bool":
                    return DecodeBool(hex, isNested);
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


        public static DecodeResult DecodeBigInt(string hex, bool isUnsigned = false, bool isNested = false)
        {
            string newHex = "";
            string toDecode = hex;


            if (isNested)
            {
                int len = int.Parse(hex.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);
                toDecode = hex.Substring(8, len * 2);
                newHex = hex.Substring(8 + len * 2);
            }

            var decodeResult = DecodeString(toDecode);
            //check if is a string representing a decimal number
            if (BigInteger.TryParse(decodeResult.Data as string, NumberStyles.Number, null, out BigInteger targetValue))
            {
                return new DecodeResult(newHex, targetValue);
            }

            var bytes = Converter.FromHexString(toDecode);
            var bigIntegerParsed = Converter.ToBigInteger(bytes, isUnsigned, true);

            return new DecodeResult(newHex, bigIntegerParsed);
        }



        public static DecodeResult DecodeInt(string hex, int size, bool isNested = false)
        {
            object parsedResult = "";
            string newHex = "";


            int cutLen = hex.Length;
            if (isNested)
            {
                cutLen = GetTypeSize(size.ToString());
            }

            var toDecode = hex.Substring(0, cutLen);
            var bytes = Converter.FromHexString(toDecode);
            var bigIntegerParsed = Converter.ToBigInteger(bytes, false, true);

            switch (size)
            {
                case 64:
                    parsedResult = Int64.Parse(bigIntegerParsed.ToString());
                    newHex = hex.Substring(cutLen);
                    break;
                case 32:
                    parsedResult = Int32.Parse(bigIntegerParsed.ToString());
                    newHex = hex.Substring(cutLen);
                    break;
                case 16:
                    parsedResult = Int16.Parse(bigIntegerParsed.ToString());
                    newHex = hex.Substring(cutLen);
                    break;
                case 8:
                    parsedResult = sbyte.Parse(bigIntegerParsed.ToString());
                    newHex = hex.Substring(cutLen);
                    break;
                default:
                    throw new Exception($"can't decode type: Int{size}");
            }


            return new DecodeResult(newHex, parsedResult);

        }

        public static DecodeResult DecodeUint(string hex, int size, bool isNested = false)
        {
            object parsedResult = "";
            string newHex = "";

            int cutLen = hex.Length;
            if (isNested)
            {
                cutLen = GetTypeSize(size.ToString());
            }

            var toDecode = hex.Substring(0, cutLen);
            var bytes = Converter.FromHexString(toDecode);
            var bigIntegerParsed = Converter.ToBigInteger(bytes, true, true);

            switch (size)
            {
                case 64:
                    parsedResult = UInt64.Parse(bigIntegerParsed.ToString());
                    newHex = hex.Substring(cutLen);
                    break;
                case 32:
                    parsedResult = UInt32.Parse(bigIntegerParsed.ToString());
                    newHex = hex.Substring(cutLen);
                    break;
                case 16:
                    parsedResult = UInt16.Parse(bigIntegerParsed.ToString());
                    newHex = hex.Substring(cutLen);
                    break;
                case 8:
                    parsedResult = byte.Parse(bigIntegerParsed.ToString());
                    newHex = hex.Substring(cutLen);
                    break;
                default:
                    throw new Exception($"can't decode type: UInt{size}");
            }


            return new DecodeResult(newHex, parsedResult);
        }


        public static DecodeResult DecodeBool(string hex, bool isNested = false)
        {
            string toDecode = hex;
            int cutLen = hex.Length;
            if (isNested)
            {
                cutLen = GetTypeSize("bool");
                toDecode = toDecode.Substring(0, cutLen);
            }

            return new DecodeResult(hex.Substring(cutLen), toDecode == "01");
        }

        public static DecodeResult DecodeAddress(string hex)
        {
            int cutLen = GetTypeSize("Address");
            return new DecodeResult(hex.Substring(cutLen), core.Address.FromHex(hex.Substring(0, cutLen)).Bech32 as object);
        }

        public static DecodeResult DecodeString(string hex, bool isNested = false)
        {
            var newHex = "";
            var toDecode = hex;

            if (isNested)
            {
                int len = int.Parse(hex.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);
                toDecode = hex.Substring(8, len * 2);

                newHex = hex.Substring(8 + len * 2);
            }

            byte[] decodedBytes = new byte[toDecode.Length / 2];
            for (int i = 0; i < decodedBytes.Length; i++)
            {
                decodedBytes[i] = Convert.ToByte(toDecode.Substring(i * 2, 2), 16);
            }

            return new DecodeResult(newHex, Encoding.UTF8.GetString(decodedBytes) as object);
        }

    }
}

