using System;
using System.Threading.Tasks;
using kleversdk.provider.Dto;

namespace kleversdk.provider
{
    public interface IKleverProvider
    {
        /// <summary>
        /// This endpoint allows one to retrieve basic information about an Address (Account).
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns><see cref="AccountDataDto"/></returns>
        Task<AccountDto> GetAccount(string address);
    }
}
