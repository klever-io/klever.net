using System;
namespace kleversdk.provider.Exceptions
{
    public class APIException : Exception
    {
        public APIException(string errorMessage, string code)
        : base($"Error when calling API : {errorMessage}: {code}")
        {
        }
    }
}
