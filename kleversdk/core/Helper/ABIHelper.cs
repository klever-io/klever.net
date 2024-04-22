using System;
namespace kleversdk.core
{
    public class ABIHelper
    {
        public ABIHelper()
        {
        }

        public static bool isPrimitive(string typeName)
        {

            string[] primitiveInputs = { "BigUint", "BigInt", "u8", "u16", "u32", "u64", "i8", "i16", "i32", "i64", "usize", "isize",
         "TokenIdentifier", "String", "Address", "Bytes", "Hash", "PublicKey", "Signature", "ManagedBuffer", "BoxedBytes", "&[u8]",
         "Vec<u8>", "&str", "bytes", "ManagedVec", "bool", "List","Array", "Tuple"};


            return Array.Exists(primitiveInputs, element => element == typeName);
        }
    }
}

