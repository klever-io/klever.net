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

            var encondedValue = ABI.Encode(value, type, false);
            var encondedValueNegative = ABI.Encode(valueNegative, type, false);

            Assert.Equal(expected, encondedValue);
            Assert.Equal(expectedNegative, encondedValueNegative);

            encondedValue = ABI.Encode(value, type, true);
            encondedValueNegative = ABI.Encode(valueNegative, type, true);

            Assert.Equal(expectedNested, encondedValue);
            Assert.Equal(expectedNestedNegative, encondedValueNegative);

        }



        [Fact]
        public static void ABITests_EncondeInt64()
        {
            var type = "i64";

            var expected = "03E8";
            var expectedNegative = "FC18";

            var expectedNested = "00000000000003E8";
            var expectedNestedNegative = "FFFFFFFFFFFFFC18";

            var value = "1000";
            var valueNegative = "-1000";

            var encondedValue = ABI.Encode(value, type, false);
            var encondedValueNegative = ABI.Encode(valueNegative, type, false);

            Assert.Equal(expected, encondedValue);
            Assert.Equal(expectedNegative, encondedValueNegative);

            encondedValue = ABI.Encode(value, type, true);
            encondedValueNegative = ABI.Encode(valueNegative, type, true);

            Assert.Equal(expectedNested, encondedValue);
            Assert.Equal(expectedNestedNegative, encondedValueNegative);
        }

        [Fact]
        public static void ABITests_EncondeInt32()
        {
            // Works with i32 and isize
            var type = "i32";

            var expected = "03E8";
            var expectedNegative = "FC18";

            var expectedNested = "000003E8";
            var expectedNestedNegative = "FFFFFC18";

            var value = "1000";
            var valueNegative = "-1000";

            var encondedValue = ABI.Encode(value, type, false);
            var encondedValueNegative = ABI.Encode(valueNegative, type, false);

            Assert.Equal(expected, encondedValue);
            Assert.Equal(expectedNegative, encondedValueNegative);

            encondedValue = ABI.Encode(value, type, true);
            encondedValueNegative = ABI.Encode(valueNegative, type, true);

            Assert.Equal(expectedNested, encondedValue);
            Assert.Equal(expectedNestedNegative, encondedValueNegative);
        }

        [Fact]
        public static void ABITests_EncondeUint64()
        {
            var type = "u64";

            var expected = "03E8";
            var expectedNested = "00000000000003E8";

            var value = "1000";

            var encondedValue = ABI.Encode(value, type, false);

            Assert.Equal(expected, encondedValue);

            encondedValue = ABI.Encode(value, type, true);

            Assert.Equal(expectedNested, encondedValue);
        }

        [Fact]
        public static void ABITests_EncondeUint32() {
            var type = "u32";

            var expected = "03E8";
            var expectedNested = "000003E8";

            var value = "1000";

            var encondedValue = ABI.Encode(value, type, false);

            Assert.Equal(expected, encondedValue);

            encondedValue = ABI.Encode(value, type, true);

            Assert.Equal(expectedNested, encondedValue);
        }

        [Fact]
        public static void ABITests_EncondeString() {
            var type = "String";

            var expected = "4265206B6C65766572";
            var expectedNested = "000000094265206B6C65766572";

            var valueString = "Be klever";

            var encondedValue = ABI.Encode(valueString, type, false);
            var encondedValueNested = ABI.Encode(valueString, type, true);

            Assert.Equal(expected, encondedValue);
            Assert.Equal(expectedNested, encondedValueNested);
         }

        [Fact]
        public static void ABITests_EncondeHex() {
            // Works with hex
            var type = "hex";

            var expected = "4265206b6c6576657221";

            var valueString = "4265206b6c6576657221";

            var encondedValue = ABI.Encode(valueString, type);

            Assert.Equal(expected, encondedValue);
        }

        [Fact]
        public static void ABITests_EncondeAddress() {
            // Works with Address
            var type = "Address";

            var expected = "0000000000000000000000000000000000000000000000000000000000000000";

            var valueString = "klv1qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqpgm89z"; // zero address

            var encondedValue_1 = ABI.Encode(valueString, type);

            Assert.Equal(expected, encondedValue_1);
        }

        [Fact]
        public static void ABITests_EncondeBool()
        {
            // Works with bool
            var type = "bool";

            var expectedTrue = "01";
            var expectedFalse = "00";

            var valueTrue = "true"; 
            var valueFalse = "false";

            var encondedValue_true = ABI.Encode(valueTrue, type);
            var encondedValue_false = ABI.Encode(valueFalse, type);

            Assert.Equal(expectedTrue, encondedValue_true);
            Assert.Equal(expectedFalse, encondedValue_false);
        }

    }
}

