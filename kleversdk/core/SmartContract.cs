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


                arg = ABI.Encode(scValue, paramType);
             
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

