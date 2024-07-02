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
        
        public static JsonABI LoadABIByFile(string filepath)
        {
            string json = File.ReadAllText(filepath);

            JsonABI abiParsed = JsonConvert.DeserializeObject<JsonABI>(json);

            return abiParsed;
        }

        public static JsonABI LoadABIByString(string abi)
        {
            return JsonConvert.DeserializeObject<JsonABI>(abi);
        }


        public static string Encode(string value,string type, bool isNested = false) {
            return ABIEncoder.EncodeSingleValue(value, type, isNested);
        }

        public static object Decode(string hex, string type, bool isNested = false)
        {
           return ABIDecoder.DecodeSingleValue(hex, type, isNested);
        }

        public static object DecodeByAbi(JsonABI abi, string hex, string endpointName, bool isNested = false)
        {
            // check if is a valid endpoint
            var endpoint = abi.GetEndpoint(endpointName);
            if (endpoint == null) {
                throw new Exception("invalid endpoint");
            }

            if (endpoint.mutability != "readonly") {
                throw new Exception("mutability must be readonly");
            }

            if (endpoint.outputs.Count != 1) {
                throw new Exception("invalid output lenght");
            }

            var type = endpoint.outputs[0].type;


            return ABIDecoder.SelectDecoder(abi, hex, type, isNested);
        }
    }
}

