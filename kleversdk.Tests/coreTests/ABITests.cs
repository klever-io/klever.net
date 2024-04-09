using System;
using System.Numerics;
using kleversdk.core;
using Xunit;

namespace kleversdk.Tests.coreTests
{
    public class ABITests
    {
        public ABITests()
        {

        }

        [Fact]
        public static void ABITests_EncodeBigNumber()
        {
            var type = "BigInt";

            var expected = "05F5E100";
            var expectedNegative = "FA0A1F00";

            var expectedNested = "0000000405F5E100";
            var expectedNestedNegative = "00000004FA0A1F00";

            var value = "100000000";
            var valueNegative = "-100000000";

            var EncodedValue = ABI.Encode(value, type, false);
            var EncodedValueNegative = ABI.Encode(valueNegative, type, false);

            Assert.Equal(expected, EncodedValue);
            Assert.Equal(expectedNegative, EncodedValueNegative);

            EncodedValue = ABI.Encode(value, type, true);
            EncodedValueNegative = ABI.Encode(valueNegative, type, true);

            Assert.Equal(expectedNested, EncodedValue);
            Assert.Equal(expectedNestedNegative, EncodedValueNegative);

        }

        [Fact]
        public static void ABITests_EncodeInt64()
        {
            var type = "i64";

            var expected = "03E8";
            var expectedNegative = "FC18";

            var expectedNested = "00000000000003E8";
            var expectedNestedNegative = "FFFFFFFFFFFFFC18";

            var value = "1000";
            var valueNegative = "-1000";

            var EncodedValue = ABI.Encode(value, type, false);
            var EncodedValueNegative = ABI.Encode(valueNegative, type, false);

            Assert.Equal(expected, EncodedValue);
            Assert.Equal(expectedNegative, EncodedValueNegative);

            EncodedValue = ABI.Encode(value, type, true);
            EncodedValueNegative = ABI.Encode(valueNegative, type, true);

            Assert.Equal(expectedNested, EncodedValue);
            Assert.Equal(expectedNestedNegative, EncodedValueNegative);
        }

        [Fact]
        public static void ABITests_EncodeInt32()
        {
            // Works with i32 and isize
            var type = "i32";

            var expected = "03E8";
            var expectedNegative = "FC18";

            var expectedNested = "000003E8";
            var expectedNestedNegative = "FFFFFC18";

            var value = "1000";
            var valueNegative = "-1000";

            var EncodedValue = ABI.Encode(value, type, false);
            var EncodedValueNegative = ABI.Encode(valueNegative, type, false);

            Assert.Equal(expected, EncodedValue);
            Assert.Equal(expectedNegative, EncodedValueNegative);

            EncodedValue = ABI.Encode(value, type, true);
            EncodedValueNegative = ABI.Encode(valueNegative, type, true);

            Assert.Equal(expectedNested, EncodedValue);
            Assert.Equal(expectedNestedNegative, EncodedValueNegative);
        }

        [Fact]
        public static void ABITests_EncodeUint64()
        {
            var type = "u64";

            var expected = "03E8";
            var expectedNested = "00000000000003E8";

            var value = "1000";

            var EncodedValue = ABI.Encode(value, type, false);

            Assert.Equal(expected, EncodedValue);

            EncodedValue = ABI.Encode(value, type, true);

            Assert.Equal(expectedNested, EncodedValue);
        }

        [Fact]
        public static void ABITests_EncodeUint32()
        {
            var type = "u32";

            var expected = "03E8";
            var expectedNested = "000003E8";

            var value = "1000";

            var EncodedValue = ABI.Encode(value, type, false);

            Assert.Equal(expected, EncodedValue);

            EncodedValue = ABI.Encode(value, type, true);

            Assert.Equal(expectedNested, EncodedValue);
        }

        [Fact]
        public static void ABITests_EncodeString()
        {
            var type = "String";

            var expected = "4265206B6C65766572";
            var expectedNested = "000000094265206B6C65766572";

            var valueString = "Be klever";

            var EncodedValue = ABI.Encode(valueString, type, false);
            var EncodedValueNested = ABI.Encode(valueString, type, true);

            Assert.Equal(expected, EncodedValue);
            Assert.Equal(expectedNested, EncodedValueNested);
        }

        [Fact]
        public static void ABITests_EncodeHex()
        {
            // Works with hex
            var type = "hex";

            var expected = "4265206b6c6576657221";

            var valueString = "4265206b6c6576657221";

            var EncodedValue = ABI.Encode(valueString, type);

            Assert.Equal(expected, EncodedValue);
        }

        [Fact]
        public static void ABITests_EncodeAddress()
        {
            // Works with Address
            var type = "Address";

            var expected = "0000000000000000000000000000000000000000000000000000000000000000";

            var valueString = "klv1qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqpgm89z"; // zero address

            var EncodedValue_1 = ABI.Encode(valueString, type);

            Assert.Equal(expected, EncodedValue_1);
        }

        [Fact]
        public static void ABITests_EncodeBool()
        {
            // Works with bool
            var type = "bool";

            var expectedTrue = "01";
            var expectedFalse = "00";

            var valueTrue = "true";
            var valueFalse = "false";

            var EncodedValue_true = ABI.Encode(valueTrue, type);
            var EncodedValue_false = ABI.Encode(valueFalse, type);

            Assert.Equal(expectedTrue, EncodedValue_true);
            Assert.Equal(expectedFalse, EncodedValue_false);
        }



        [Fact]
        public static void ABITests_DecodeBool()
        {
            // Works with bool
            var type = "bool";

            var expectedTrue = true;
            var expectedFalse = false;

            var valueTrue = "01";
            var valueFalse = "00";

            var decodedValueTrue = ABI.Decode(valueTrue, type);
            var decodedValueFalse = ABI.Decode(valueFalse, type);

            Assert.Equal(expectedTrue, decodedValueTrue);
            Assert.Equal(expectedFalse, decodedValueFalse);
            Assert.IsType<bool>(decodedValueTrue);
            Assert.IsType<bool>(decodedValueFalse);
        }

        [Fact]
        public static void ABITests_DecodeAddress()
        {
            var type = "Address";

            var value = "0000000000000000000000000000000000000000000000000000000000000000";
            var expected = "klv1qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqpgm89z";

            var decodedValueAddress = ABI.Decode(value, type);

            Assert.Equal(expected, decodedValueAddress);
            Assert.IsType<string>(decodedValueAddress);
        }


        [Fact]
        public static void ABITests_DecodeString()
        {
            var type = "String";


            var value = "4265206B6C65766572";
            var valueNested = "000000094265206B6C65766572";

            var valueNested_2 = "000000156f746865722d737472696e672d666f722d74657374";

            var expected = "Be klever";
            var expected_2 = "other-string-for-test";


            var decodedValue = ABI.Decode(value, type, false);
            var decodedValueNested = ABI.Decode(valueNested, type, true);

            var decodedValueNested_2 = ABI.Decode(valueNested_2, type, true);

            Assert.Equal(expected, decodedValue);
            Assert.Equal(expected, decodedValueNested);
            Assert.Equal(expected_2, decodedValueNested_2);

            Assert.IsType<string>(decodedValue);
            Assert.IsType<string>(decodedValueNested);
            Assert.IsType<string>(decodedValueNested_2);
        }


        [Fact]
        public static void ABITests_DecodeInt64()
        {
            var type = "i64";


            var value = "03e8";
            var valueNested = "00000000000003e8";


            var valueNegative = "FC18";
            var valueNestedNegative = "FFFFFFFFFFFFFC18";

            Int64 expected = 1000;
            Int64 expectedNegative = -1000;


            var decodedValue = ABI.Decode(value, type, false);
            var decodedValueNegative = ABI.Decode(valueNegative, type, false);

            var decodedValueNested = ABI.Decode(valueNested, type, true);
            var decodedValueNestedNegative = ABI.Decode(valueNestedNegative, type, true);


            Assert.Equal(expected, decodedValue);
            Assert.Equal(expectedNegative, decodedValueNegative);
            Assert.Equal(expected, decodedValueNested);
            Assert.Equal(expectedNegative, decodedValueNestedNegative);


            Assert.IsType<Int64>(decodedValue);
            Assert.IsType<Int64>(decodedValueNegative);
            Assert.IsType<Int64>(decodedValueNested);
            Assert.IsType<Int64>(decodedValueNestedNegative);

        }

        [Fact]
        public static void ABITests_DecodeInt32()
        {
            var type = "i32";

            var value = "03E8";
            var valueNested = "000003E8";


            var valueNegative = "FC18";
            var valueNestedNegative = "FFFFFC18";

            Int32 expected = 1000;
            Int32 expectedNegative = -1000;


            var decodedValue = ABI.Decode(value, type, false);
            var decodedValueNegative = ABI.Decode(valueNegative, type, false);

            var decodedValueNested = ABI.Decode(valueNested, type, true);
            var decodedValueNestedNegative = ABI.Decode(valueNestedNegative, type, true);


            Assert.Equal(expected, decodedValue);
            Assert.Equal(expectedNegative, decodedValueNegative);
            Assert.Equal(expected, decodedValueNested);
            Assert.Equal(expectedNegative, decodedValueNestedNegative);


            Assert.IsType<Int32>(decodedValue);
            Assert.IsType<Int32>(decodedValueNegative);
            Assert.IsType<Int32>(decodedValueNested);
            Assert.IsType<Int32>(decodedValueNestedNegative);

        }

        [Fact]
        public static void ABITests_DecodeUint64()
        {
            var type = "u64";

            var value = "03e8";
            var valueNested = "00000000000003e8";


            UInt64 expected = 1000;

            var decodedValue = ABI.Decode(value, type, false);
            var decodedValueNested = ABI.Decode(valueNested, type, true);

            Assert.Equal(expected, decodedValue);
            Assert.Equal(expected, decodedValueNested);

            Assert.IsType<UInt64>(decodedValue);
            Assert.IsType<UInt64>(decodedValueNested);

        }

        [Fact]
        public static void ABITests_DecodeUint32()
        {
            var type = "u32";

            var value = "03E8";
            var valueNested = "000003E8";


            UInt32 expected = 1000;

            var decodedValue = ABI.Decode(value, type, false);
            var decodedValueNested = ABI.Decode(valueNested, type, true);

            Assert.Equal(expected, decodedValue);
            Assert.Equal(expected, decodedValueNested);

            Assert.IsType<UInt32>(decodedValue);
            Assert.IsType<UInt32>(decodedValueNested);
        }

        [Fact]
        public static void ABITests_DecodeBigNumber()
        {
            var type = "BigInt";

            var value = "05F5E100";
            var valueNested = "0000000405F5E100";
            var valueNegative = "FA0A1F00";
            var valueNestedNegative = "00000004FA0A1F00";

            BigInteger expected = 100000000;
            BigInteger expectedNegative = -100000000;



            var decodedValue = ABI.Decode(value, type, false);
            var decodedValueNegative = ABI.Decode(valueNegative, type, false);

            var decodedValueNested = ABI.Decode(valueNested, type, true);
            var decodedValueNestedNegative = ABI.Decode(valueNestedNegative, type, true);


            Assert.Equal(expected, decodedValue);
            Assert.Equal(expectedNegative, decodedValueNegative);
            Assert.Equal(expected, decodedValueNested);
            Assert.Equal(expectedNegative, decodedValueNestedNegative);


            Assert.IsType<BigInteger>(decodedValue);
            Assert.IsType<BigInteger>(decodedValueNegative);
            Assert.IsType<BigInteger>(decodedValueNested);
            Assert.IsType<BigInteger>(decodedValueNestedNegative);
        }


        [Fact]
        public static void ABITests_SingleValuesDecode()
        {
            string path = "./singleValue.json.example";

            JsonABI abi = ABI.LoadABIByFile(path);

            var endpoint = "managed_buffer";
            var hex = "74657374696e67206f757470757473207479706573";
            object expected = "testing outputs types";

            var value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "bool_false";
            hex = "";
            expected = false;

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "bool_true";
            hex = "01";
            expected = true;

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "usize_number";
            hex = "FDC20CBF";
            expected = UInt32.Parse("4257352895");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);


            endpoint = "isize_number";
            hex = "40CF4061";
            expected = Int32.Parse("1087324257");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);


            endpoint = "isize_minus_number";
            hex = "BF30BF9F";
            expected = Int32.Parse("-1087324257");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "token_identifier";
            hex = "4B4C56";
            expected = "KLV";

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "owner_address";
            hex = "667fd274481cf5b07418b2fdc5d8baa6ae717239357f338cde99c2f612a96a9e";
            expected = "klv1velayazgrn6mqaqckt7utk9656h8zu3ex4ln8rx7n8p0vy4fd20qmwh4p5";

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_s_10";
            hex = "3130";
            expected = BigInteger.Parse("10");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_minus_s_10";
            hex = "2D3130";
            expected = BigInteger.Parse("-10");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_minus_i8";
            hex = "AE";
            expected = BigInteger.Parse("-82");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_i8";
            hex = "52";
            expected = BigInteger.Parse("82");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_minus_i16";
            hex = "DEDE";
            expected = BigInteger.Parse("-8482");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_i16";
            hex = "2122";
            expected = BigInteger.Parse("8482");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_minus_i32";
            hex = "E69FA284";
            expected = BigInteger.Parse("-425745788");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_i32";
            hex = "19605D7C";
            expected = BigInteger.Parse("425745788");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_minus_i64";
            hex = "C91131A14FC23DAC";
            expected = BigInteger.Parse("-3958328028584329812");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_i64";
            hex = "36EECE5EB03DC254";
            expected = BigInteger.Parse("3958328028584329812");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_s_number";
            hex = "393833343735393337343536383932343739363738383930313736393831393038353637383935373639303738353132393836373938323537";
            expected = BigInteger.Parse("983475937456892479678890176981908567895769078512986798257");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_minus_s_number";
            hex = "2d393833343735393337343536383932343739363738383930313736393831393038353637383935373639303738353132393836373938323537";
            expected = BigInteger.Parse("-983475937456892479678890176981908567895769078512986798257");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_u_s_10";
            hex = "3130";
            expected = BigInteger.Parse("10");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_u_s_number";
            hex = "3832373432383733363433343735393733353933343736393733393637393337363938333435373836393833393035363938393739373839373839";
            expected = BigInteger.Parse("82742873643475973593476973967937698345786983905698979789789");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_u8";
            hex = "52";
            expected = BigInteger.Parse("82");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_u16";
            hex = "2122";
            expected = BigInteger.Parse("8482");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_u32";
            hex = "19605D7C";
            expected = BigInteger.Parse("425745788");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_u64";
            hex = "36EECE5EB03DC254";
            expected = BigInteger.Parse("3958328028584329812");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "big_u128";
            hex = "1DC7766516260B32B52FF11612D5710E";
            expected = BigInteger.Parse("39583280285843298128735477835272384782");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "number_i8";
            hex = "52";
            expected = sbyte.Parse("82");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "number_minus_i8";
            hex = "AE";
            expected = sbyte.Parse("-82");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "number_i16";
            hex = "2122";
            expected = Int16.Parse("8482");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "number_minus_i16";
            hex = "DEDE";
            expected = Int16.Parse("-8482");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "number_i32";
            hex = "19605D7C";
            expected = Int32.Parse("425745788");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "number_minus_i32";
            hex = "E69FA284";
            expected = Int32.Parse("-425745788");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "number_i64";
            hex = "36EECE5EB03DC254";
            expected = Int64.Parse("3958328028584329812");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

            endpoint = "number_minus_i64";
            hex = "C91131A14FC23DAC";
            expected = Int64.Parse("-3958328028584329812");

            value = ABI.DecodeByAbi(abi, hex, endpoint);
            Assert.Equal(expected, value);

        }

        [Fact]
        public static void ABITests_DecodeList()
        {

        }
    }
}
