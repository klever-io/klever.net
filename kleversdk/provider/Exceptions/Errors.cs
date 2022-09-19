using System;
namespace kleversdk.provider.Exceptions
{
    public class ContractsSizeException : Exception
    {
        public ContractsSizeException(long count)
        : base($"Error invalid len of contracts to build request: {count}")
        {
        }
    }
}
