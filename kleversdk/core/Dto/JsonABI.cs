using System;
using System.Collections.Generic;
using kleversdk.core;
using Newtonsoft.Json;

public class Input
{
    public string name { get; set; }
    public string type { get; set; }

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

public class TypeStruct
{
    public string type { get; set; }
    public Fields[] fields { get; set; }
}

public class Fields
{
    public string name { get; set; }
    public string type { get; set; }
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

    public TypeStruct GetType(string name)
    {

        foreach (var type in this.types)
        {
            if (type.Key == name)
            {
               return JsonConvert.DeserializeObject<TypeStruct>(type.Value.ToString());
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