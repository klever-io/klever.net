using System;
namespace kleversdk.core.Exceptions
{
    public class CannotCreateAddressException : Exception
    {
        public CannotCreateAddressException(string input)
            : base($"Cannot create address from {input}")
        {
        }
    }
}
