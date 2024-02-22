using System;
using System.Collections.Generic;
using kleversdk.provider.Helper;
using System.Text;

namespace kleversdk.core
{
    public class SmartContract
    {

        private const int MetadataLength = 2;

        private const int MetadataUpgradeable = 1;
        private const int MetadataReadable = 4;

        private const int MetadataPayable = 2;
        private const int MetadataPayableBySC = 4;

        public SmartContract()
        {
        }

        private static string ToSmartContractParams(List<string[]> scParams)
        {
            string parsedArgs = "";

            foreach (string[] scParam in scParams)
            {
                var arg = "";

                if (scParam.Length == 1)
                {
                    try
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(scParam[0]);
                        arg = BitConverter.ToString(bytes).Replace("-", "");
                        parsedArgs += "@" + arg;
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine($"Error converting string {scParam[0]} : {err.Message}");
                    }

                    continue;
                }

                if (scParam.Length != 2)
                {
                    throw new Exception("Parameter Invalid");
                }

                var paramType = scParam[0];
                var scValue = scParam[1];

                var isOption = false;

                if (paramType.StartsWith("option"))
                {
                    isOption = true;
                    paramType = paramType.Replace("option", "");
                }

                switch (paramType)
                {
                    case "bi":
                    case "BI":
                    case "n":
                    case "N":
                    case "bigNumber":
                        try
                        {
                            var value = Converter.StringToBigInteger(scValue);

                            arg = value.ToString("X");
                            if (arg.Length % 2 != 0)
                            {
                                arg = "0" + arg;
                            }
                        }
                        catch (Exception err)
                        {
                            Console.WriteLine($"Error converting bigNumber {scValue} : {err.Message}");
                        }
                        break;
                    case "i":
                    case "I":
                    case "i64":
                    case "I64":
                    case "int64":
                        try
                        {
                            var value = Int64.Parse(scValue);

                            arg = value.ToString("X16");
                        }
                        catch (Exception err)
                        {
                            Console.WriteLine($"Error converting int64 {scValue} : {err.Message}");
                        }

                        break;
                    case "u":
                    case "U":
                    case "u64":
                    case "U64":
                    case "uint64":
                        try
                        {
                            var value = Int64.Parse(scValue);
                            if (value < 0)
                            {
                                throw new Exception("uint64 can't be negative");
                            }

                            arg = value.ToString("X16");
                        }
                        catch (Exception err)
                        {
                            Console.WriteLine($"Error converting uint64 {scValue} : {err.Message}");
                        }

                        break;
                    case "i32":
                    case "I32":
                    case "int32":
                        try
                        {
                            var value = Int32.Parse(scValue);

                            arg = value.ToString("X8");
                        }
                        catch (Exception err)
                        {
                            Console.WriteLine($"Error converting int32 {scValue} : {err.Message}");
                        }

                        break;
                    case "u32":
                    case "U32":
                    case "uint32":
                        try
                        {
                            var value = Int32.Parse(scValue);
                            if (value < 0)
                            {
                                throw new Exception("uint64 can't be negative");
                            }

                            arg = value.ToString("X8");
                        }
                        catch (Exception err)
                        {
                            Console.WriteLine($"Error converting uint32 {scValue} : {err.Message}");
                        }
                        break;
                    case "s":
                    case "S":
                    case "string":
                        try
                        {
                            byte[] bytes = Encoding.UTF8.GetBytes(scValue);
                            arg = BitConverter.ToString(bytes).Replace("-", "");


                        }
                        catch (Exception err)
                        {
                            Console.WriteLine($"Error converting string {scValue} : {err.Message}");
                        }
                        break;
                    // string
                    case "x":
                    case "X":
                    case "hex":
                        // replace if 0x exists
                        scValue = scValue.Replace("0x", "");
                        try
                        {
                            if (scValue.Length % 2 != 0)
                            {
                                throw new ArgumentException("Invalid hex string length: " + scValue.Length);
                            }

                            byte[] bytes = new byte[scValue.Length / 2];
                            for (int i = 0; i < bytes.Length; i++)
                            {
                                bytes[i] = Convert.ToByte(scValue.Substring(i * 2, 2), 16);
                            }

                            arg = scValue;
                        }
                        catch (Exception err)
                        {
                            Console.WriteLine($"Error converting hex {scValue} : {err.Message}");
                        }

                        break;
                    // hex
                    case "a":
                    case "A":
                    case "address":
                        try
                        {
                            var addr = core.Address.FromBech32(scValue);
                            arg = addr.Hex;
                        }
                        catch (Exception err)
                        {
                            Console.WriteLine($"Error converting address {scValue} : {err.Message}");
                        }
                        break;
                    // addrress
                    case "0":
                    case "e":
                    case "E":
                    case "empty":
                        break;
                    default:
                        break;

                }

                if (isOption)
                {
                    arg = "01" + arg;
                }

                parsedArgs += "@" + arg;

            }

            return parsedArgs;
        }

        private static string ToSmartContractFileParams(byte[] file, bool upgradeable = false, bool readable = false, bool payable = false, bool payableBySC = false, string vmType = "0500")
        {
            // Handle Metadata

            byte[] metadata = new byte[MetadataLength];

            if (upgradeable)
            {
                metadata[0] |= MetadataUpgradeable;
            }

            if (readable)
            {
                metadata[0] |= MetadataReadable;
            }


            if (payable)
            {
                metadata[1] |= MetadataPayable;
            }


            if (payableBySC)
            {
                metadata[1] |= MetadataPayableBySC;
            }

            var metadata_hex = metadata.ToHex();

            // Handle File

            var fileHex = BitConverter.ToString(file).Replace("-", "");

            var argsParsed = $"{fileHex}@{vmType}@{metadata_hex}";

            return argsParsed;
        }



        public static string ToEncodeDeploySmartContract(byte[] file, List<string[]> scParams, bool upgradeable = false, bool readable = false, bool payable = false, bool payableBySC = false, string vmType = "0500")
        {
            string scParamsParsed = ToSmartContractParams(scParams);
            string fileParamsParsed = ToSmartContractFileParams(file, upgradeable, readable, payable, payable, vmType);


            return $"{fileParamsParsed}{scParamsParsed}";
        }



        public static string ToEncodeInvokeSmartContract(string functionName, List<string[]> scParams)
        {
            string scParamsParsed = ToSmartContractParams(scParams);

            return $"{functionName}{scParamsParsed}";
        }


    }
}

