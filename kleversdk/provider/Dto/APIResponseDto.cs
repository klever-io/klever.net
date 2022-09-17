using System;
using kleversdk.provider.Exceptions;

namespace kleversdk.provider.Dto
{
    public class APIResponseDto<T>
    {
        /// <summary>
        /// <see cref="T"/>
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Human-readable description of the issue
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 'successful' in case of success
        /// </summary>
        public string Code { get; set; }

        public void EnsureSuccessStatusCode()
        {
            if (string.IsNullOrEmpty(Error) && Code == "successful")
                return;

            throw new APIException(Error, Code);
        }
    }
}
