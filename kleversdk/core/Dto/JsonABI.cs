using System;
using System.Collections.Generic;

public class Input
{
    public string name { get; set; }
    public string type { get; set; }

    public bool IsPrimitive()
    {
         string[] primitiveInputs = { "BigUint", "BigInt", "u8", "u16", "u32", "u64", "i8", "i16", "i32", "i64", "usize", "isize",
         "TokenIdentifier", "String", "Address", "Bytes", "Hash", "PublicKey", "Signature", "ManagedBuffer", "BoxedBytes", "&[u8]",
         "Vec<u8>", "&str", "bytes", "ManagedVec", "bool", "List","Array", "Tuple"};


        return Array.Exists(primitiveInputs, element => element == this.name);
    }

}

public class Endpoint
{
    public string name { get; set; }
    public string mutability { get; set; }
    public List<Input> outputs { get; set; }

    public Input GetOutput(string name)
    {
        foreach (var output in this.outputs)
        {
            if (output.name == name)
            {
                return output;
            }

        }

        return null;
    }

}

public class JsonABI
{
    public List<Endpoint> endpoints { get; set; }
    public Dictionary<string, object> types { get; set; }

    public Endpoint GetEndpoint(string name)
    {
        foreach (var endpoint in this.endpoints)
        {
            if (endpoint.name == name) {
                return endpoint;
            }

        }

        return null;
    }


    public Input GetEndpointOutput(string endpointName,string outputName)
    {
        var findedEndpoint = new Endpoint();

        foreach (var endpoint in this.endpoints)
        {
            if (endpoint.name == endpointName)
            {
                findedEndpoint = endpoint;
            }

        }


        return findedEndpoint.GetOutput(outputName);
    }
}