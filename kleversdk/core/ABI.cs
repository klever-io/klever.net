using System;
using kleversdk.provider.Helper;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using kleversdk.core.Helper;

namespace kleversdk.core
{
	public class ABI
	{
		public ABI()
		{
        }
        
        public static JsonABI ReadABIByFile(string filepath)
        {
            string json = File.ReadAllText(filepath);

            JsonABI abiParsed = JsonConvert.DeserializeObject<JsonABI>(json);

            return abiParsed;
        }

        public static JsonABI ReadABIByString(string abi)
        {
            return JsonConvert.DeserializeObject<JsonABI>(abi);
        }


        public static string Encode(string value,string type, bool isNested = false) {
            switch (type)
            {
                case "List":
                    return "not implemented";
                case "Option":
                    return "not implemented";
                case "tuple":
                    return "not implemented";
                case "variadic":
                    return "not implemented";
                default:
                    return ABIEncoder.EncodeSingleValue(value, type, isNested);
                  
            }
      
        }

    }
}

