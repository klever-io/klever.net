using System;
using System.Collections.Generic;
using System.Numerics;
using kleversdk.core;
using kleversdk.Tests.coreTests.examples;
using kleversdk.core.Helper;
using Newtonsoft.Json.Linq;
using Xunit;
using System.Linq;
using Org.BouncyCastle.Math.EC.Multiplier;

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

            var decodedValueTrue = ABI.Decode(valueTrue, type) as DecodeResult;
            var decodedValueFalse = ABI.Decode(valueFalse, type) as DecodeResult;

            Assert.Equal(expectedTrue, decodedValueTrue.Data);
            Assert.Equal(expectedFalse, decodedValueFalse.Data);
            Assert.IsType<bool>(decodedValueTrue.Data);
            Assert.IsType<bool>(decodedValueFalse.Data);
        }

        [Fact]
        public static void ABITests_DecodeAddress()
        {
            var type = "Address";

            var value = "0000000000000000000000000000000000000000000000000000000000000000";
            var expected = "klv1qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqpgm89z";

            var decodedValueAddress = ABI.Decode(value, type) as DecodeResult;

            Assert.Equal(expected, decodedValueAddress.Data);
            Assert.IsType<string>(decodedValueAddress.Data);
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


            var decodedValue = ABI.Decode(value, type, false) as DecodeResult;
            var decodedValueNested = ABI.Decode(valueNested, type, true) as DecodeResult;
            var decodedValueNested_2 = ABI.Decode(valueNested_2, type, true) as DecodeResult;

            Assert.Equal(expected, decodedValue.Data);
            Assert.Equal(expected, decodedValueNested.Data);
            Assert.Equal(expected_2, decodedValueNested_2.Data);

            Assert.IsType<string>(decodedValue.Data);
            Assert.IsType<string>(decodedValueNested.Data);
            Assert.IsType<string>(decodedValueNested_2.Data);
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


            var decodedValue = ABI.Decode(value, type, false) as DecodeResult;
            var decodedValueNegative = ABI.Decode(valueNegative, type, false) as DecodeResult;

            var decodedValueNested = ABI.Decode(valueNested, type, true) as DecodeResult;
            var decodedValueNestedNegative = ABI.Decode(valueNestedNegative, type, true) as DecodeResult;


            Assert.Equal(expected, decodedValue.Data);
            Assert.Equal(expectedNegative, decodedValueNegative.Data);
            Assert.Equal(expected, decodedValueNested.Data);
            Assert.Equal(expectedNegative, decodedValueNestedNegative.Data);


            Assert.IsType<Int64>(decodedValue.Data);
            Assert.IsType<Int64>(decodedValueNegative.Data);
            Assert.IsType<Int64>(decodedValueNested.Data);
            Assert.IsType<Int64>(decodedValueNestedNegative.Data);

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


            var decodedValue = ABI.Decode(value, type, false) as DecodeResult;
            var decodedValueNegative = ABI.Decode(valueNegative, type, false) as DecodeResult;

            var decodedValueNested = ABI.Decode(valueNested, type, true) as DecodeResult;
            var decodedValueNestedNegative = ABI.Decode(valueNestedNegative, type, true) as DecodeResult;


            Assert.Equal(expected, decodedValue.Data);
            Assert.Equal(expectedNegative, decodedValueNegative.Data);
            Assert.Equal(expected, decodedValueNested.Data);
            Assert.Equal(expectedNegative, decodedValueNestedNegative.Data);


            Assert.IsType<Int32>(decodedValue.Data);
            Assert.IsType<Int32>(decodedValueNegative.Data);
            Assert.IsType<Int32>(decodedValueNested.Data);
            Assert.IsType<Int32>(decodedValueNestedNegative.Data);

        }

        [Fact]
        public static void ABITests_DecodeUint64()
        {
            var type = "u64";

            var value = "03e8";
            var valueNested = "00000000000003e8";


            UInt64 expected = 1000;

            var decodedValue = ABI.Decode(value, type, false) as DecodeResult;
            var decodedValueNested = ABI.Decode(valueNested, type, true) as DecodeResult;

            Assert.Equal(expected, decodedValue.Data);
            Assert.Equal(expected, decodedValueNested.Data);

            Assert.IsType<UInt64>(decodedValue.Data);
            Assert.IsType<UInt64>(decodedValueNested.Data);

        }

        [Fact]
        public static void ABITests_DecodeUint32()
        {
            var type = "u32";

            var value = "03E8";
            var valueNested = "000003E8";


            UInt32 expected = 1000;

            var decodedValue = ABI.Decode(value, type, false) as DecodeResult;
            var decodedValueNested = ABI.Decode(valueNested, type, true) as DecodeResult;

            Assert.Equal(expected, decodedValue.Data);
            Assert.Equal(expected, decodedValueNested.Data);

            Assert.IsType<UInt32>(decodedValue.Data);
            Assert.IsType<UInt32>(decodedValueNested.Data);
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



            var decodedValue = ABI.Decode(value, type, false) as DecodeResult;
            var decodedValueNegative = ABI.Decode(valueNegative, type, false) as DecodeResult;

            var decodedValueNested = ABI.Decode(valueNested, type, true) as DecodeResult;
            var decodedValueNestedNegative = ABI.Decode(valueNestedNegative, type, true) as DecodeResult;


            Assert.Equal(expected, decodedValue.Data);
            Assert.Equal(expectedNegative, decodedValueNegative.Data);
            Assert.Equal(expected, decodedValueNested.Data);
            Assert.Equal(expectedNegative, decodedValueNestedNegative.Data);


            Assert.IsType<BigInteger>(decodedValue.Data);
            Assert.IsType<BigInteger>(decodedValueNegative.Data);
            Assert.IsType<BigInteger>(decodedValueNested.Data);
            Assert.IsType<BigInteger>(decodedValueNestedNegative.Data);
        }


        [Fact]
        public static void ABITests_DecodeSingleValues()
        {
            JsonABI abi = ABI.LoadABIByString(ABIMock.SingleValueABI());

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

            endpoint = "big_minus_i64";
            hex = "000000072d383233343732";
            expected = BigInteger.Parse("-823472");

            value = ABI.DecodeByAbi(abi, hex, endpoint, true); // forcing nested to test reasons
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
        public static void ABITests_DecodeSingleList()
        {
            JsonABI abi = ABI.LoadABIByString(ABIMock.ListABI());

            var endpoint = "list_token_identifier";
            var hex = "000000034b4c56000000034b4649000000084b49442d38473941000000084458422d483838470000000a43484950532d4e383941";
            List<object> expected = new List<object>() { "KLV", "KFI", "KID-8G9A", "DXB-H88G", "CHIPS-N89A" };


            var values = ABI.DecodeByAbi(abi, hex, endpoint) as List<object>;

            for (int i = 0; i < values.Count(); i++)
            {
                DecodeResult v = values[i] as DecodeResult;
                Assert.Equal(expected[i], v.Data);
                Assert.IsType<string>(v.Data);
            }


            endpoint = "list_i32";
            hex = "000000080000005700000065fffffffb";
            expected = new List<object>() { 8, 87, 101, -5 };


            values = ABI.DecodeByAbi(abi, hex, endpoint) as List<object>;
            for (int i = 0; i < values.Count(); i++)
            {
                DecodeResult v = values[i] as DecodeResult;
                Assert.Equal(expected[i], v.Data);
                Assert.IsType<int>(v.Data);
            }



            endpoint = "list_u8";
            hex = "085765DD";
            expected = new List<object> { byte.Parse("8"), byte.Parse("87"), byte.Parse("101"), byte.Parse("221") };

            values = ABI.DecodeByAbi(abi, hex, endpoint) as List<object>;

            for (int i = 0; i < values.Count(); i++)
            {
                DecodeResult v = values[i] as DecodeResult;
                Assert.Equal(expected[i], v.Data);
                Assert.IsType<byte>(v.Data);
            }


            endpoint = "list_bigint";
            hex = "000000050577f695350000000109000000072d38323334373200000006353334323337";
            expected = new List<object> { BigInteger.Parse("23487485237"), BigInteger.Parse("9"), BigInteger.Parse("-823472"), BigInteger.Parse("534237") };

            values = ABI.DecodeByAbi(abi, hex, endpoint) as List<object>;
            for (int i = 0; i < values.Count(); i++)
            {
                DecodeResult v = values[i] as DecodeResult;
                Assert.Equal(expected[i], v.Data);
                Assert.IsType<BigInteger>(v.Data);
            }



            endpoint = "list_address";
            hex = "667fd274481cf5b07418b2fdc5d8baa6ae717239357f338cde99c2f612a96a9e667fd274481cf5b07418b2fdc5d8baa6ae717239357f338cde99c2f612a96a9e";
            expected = new List<object> { "klv1velayazgrn6mqaqckt7utk9656h8zu3ex4ln8rx7n8p0vy4fd20qmwh4p5", "klv1velayazgrn6mqaqckt7utk9656h8zu3ex4ln8rx7n8p0vy4fd20qmwh4p5" };

            values = ABI.DecodeByAbi(abi, hex, endpoint) as List<object>;

            for (int i = 0; i < values.Count(); i++)
            {
                DecodeResult v = values[i] as DecodeResult;
                Assert.Equal(expected[i], v.Data);
                Assert.IsType<string>(v.Data);
            }

        }

        [Fact]
        public static void ABITests_DecodeNestedList()
        {
            JsonABI abi = ABI.LoadABIByString(ABIMock.ListABI());

            var endpoint = "list_of_lists_tokens";
            var hex = "00000003000000034b4c56000000034b4649000000084b49442d3847394100000003000000084458422d483838470000000a43484950532d4e383941000000084646542d32424836";
            List<object> A = new List<object>() { "KLV", "KFI", "KID-8G9A" };
            List<object> B = new List<object>() { "DXB-H88G", "CHIPS-N89A", "FFT-2BH6" };

            var value = ABI.DecodeByAbi(abi, hex, endpoint) as List<object>;


            List<object> listA = value[0] as List<object>;
            List<object> listB = value[1] as List<object>;

            Assert.NotNull(value);


            for (int i = 0; i < listA.Count(); i++)
            {
                DecodeResult v = listA[i] as DecodeResult;
                Assert.Equal(A[i], v.Data);
                Assert.IsType<string>(v.Data);
            }

            for (int i = 0; i < listB.Count(); i++)
            {
                DecodeResult v = listB[i] as DecodeResult;
                Assert.Equal(B[i], v.Data);
                Assert.IsType<string>(v.Data);
            }



            endpoint = "list_of_lists_i64";
            hex = "0000000319ffee93a36dc12a000000000000001cd7e571502441e18400000003fffffffffffffffe000000000001e308fffffffff1b3cfdc";
            A = new List<object>() { 1873478287878897962, Int64.Parse("28"), -2889778996868685436 };
            B = new List<object>() { Int64.Parse("-2"), Int64.Parse("123656"), Int64.Parse("-239874084") };

            value = ABI.DecodeByAbi(abi, hex, endpoint) as List<object>;
            listA = value[0] as List<object>;
            listB = value[1] as List<object>;

            Assert.NotNull(value);

            for (int i = 0; i < listA.Count(); i++)
            {
                DecodeResult v = listA[i] as DecodeResult;
                Assert.Equal(A[i], v.Data);
                Assert.IsType<long>(v.Data);
            }

            for (int i = 0; i < listB.Count(); i++)
            {
                DecodeResult v = listB[i] as DecodeResult;
                Assert.Equal(B[i], v.Data);
                Assert.IsType<long>(v.Data);
            }



        }

        [Fact]
        public static void ABITests_DecodeMultipleNestedList()
        {
            JsonABI abi = ABI.LoadABIByString(ABIMock.ListABI());

            var endpoint = "list_list_list_token";
            var hex = "00000002000000030000000a43484950532d4e383941000000034b4c56000000034b464900000003000000085446542d3738364a00000008534a412d4c4b394800000008514b552d37484831";
            var value = ABI.DecodeByAbi(abi, hex, endpoint);

            Assert.NotNull(value);

            endpoint = "list_list_list_i64";
            hex = "0000000200000003000000006fab0276000000000000001cffffffffffff9ca400000003fffffffffffffffe000000000001e308fffffffff1b3cfdc0000000200000003fffffffffe4932e1000007f24bf1555700000000000000800000000300000010d9e95b690000000000000001ffffffffffffff9c";
            value = ABI.DecodeByAbi(abi, hex, endpoint);

            Assert.NotNull(value);
        }

        [Fact]
        public static void ABITests_DecodeStruct()
        {
            JsonABI abi = ABI.LoadABIByString(ABIMock.StructABI());

            var endpoint = "getLastResult";
            var hex = "000000000000005a0000003a0000006f01"; // u32 u32 u32 u32 bool
            var value = ABI.DecodeByAbi(abi, hex, endpoint) as List<object>;

            var betType = UInt32.Parse("0"); // UNDER
            var betValue = UInt32.Parse("90");
            var diceValue = UInt32.Parse("58");
            var multiplier = UInt32.Parse("111");
            var isWinner = true;

            // BET UNDER 90 -> RESULT 58 = WIN

            Assert.NotNull(value);
            Assert.Equal(betType, value[0]);
            Assert.Equal(betValue, value[1]);
            Assert.Equal(diceValue, value[2]);
            Assert.Equal(multiplier, value[3]);
            Assert.Equal(isWinner, value[4]);
        }


        [Fact]
        public static void ABITests_DecodeVariadic()
        {
            JsonABI abi = ABI.LoadABIByString(ABIMock.StructABI());

            var endpoint = "winnersInfo";
            var hex = "00000003667fd274481cf5b07418b2fdc5d8baa6ae717239357f338cde99c2f612a96a9e0000000403938700"; // struct -> u32 Address BigUint
            var valueNested = ABI.DecodeByAbi(abi, hex, endpoint) as List<object>;

            Assert.Equal(UInt32.Parse("3"), valueNested[0]);
            Assert.Equal("klv1velayazgrn6mqaqckt7utk9656h8zu3ex4ln8rx7n8p0vy4fd20qmwh4p5", valueNested[1]);
            Assert.Equal(BigInteger.Parse("60000000"), valueNested[2]);

            endpoint = "variadic_i64";
            hex = "049075ea";

            var value = ABI.DecodeByAbi(abi, hex, endpoint);


            Assert.Equal(Int64.Parse("76576234"), value); ;
        }

        [Fact]
        public static void ABITests_DecodeTuple()
        {
            JsonABI abi = ABI.LoadABIByString(ABIMock.TupleABI());

            var endpoint = "first_tuple";
            var hex = "0000000e3736333435373839343336383937667fd274481cf5b07418b2fdc5d8baa6ae717239357f338cde99c2f612a96a9efffffffe84291d30"; // struct -> u32 Address BigUint
            var valueNested = ABI.DecodeByAbi(abi, hex, endpoint) as List<object>;

            Assert.Equal(BigInteger.Parse("76345789436897"), valueNested[0]);
            Assert.Equal("klv1velayazgrn6mqaqckt7utk9656h8zu3ex4ln8rx7n8p0vy4fd20qmwh4p5", valueNested[1]);
            Assert.Equal(Int64.Parse("-6372647632"), valueNested[2]);

        }
    }
}
