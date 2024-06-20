using System;
using System.Collections.Generic;
using kleversdk.core;
using Newtonsoft.Json;

public class SCInput
{
    public string name { get; set; }
    public string type { get; set; }

}

public class SCEndpoint
{
    public string name { get; set; }
    public string mutability { get; set; }
    public List<SCInput> outputs { get; set; }

    public SCInput GetOutput(string name)
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

public class SCTypeStruct
{
    public string type { get; set; }
    public SCFields[] fields { get; set; }
}

public class SCFields
{
    public string name { get; set; }
    public string type { get; set; }
}



public class JsonABI
{
    public List<SCEndpoint> endpoints { get; set; }
    public Dictionary<string, object> types { get; set; }

    public SCEndpoint GetEndpoint(string name)
    {
        foreach (var endpoint in this.endpoints)
        {
            if (endpoint.name == name) {
                return endpoint;
            }

        }

        return null;
    }

    public SCTypeStruct GetType(string name)
    {
        if (this.types == null) {
            return null;
        }

        foreach (var type in this.types)
        {
            if (type.Key == name)
            {
               return JsonConvert.DeserializeObject<SCTypeStruct>(type.Value.ToString());
            }

        }
        return null;
    }


    public SCInput GetEndpointOutput(string endpointName,string outputName)
    {
        var findedEndpoint = new SCEndpoint();

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