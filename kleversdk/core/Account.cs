using System;
using System.Threading.Tasks;
using kleversdk.provider;

namespace kleversdk.core
{
    public class Account
    {
        public Address Address { get; }
        public long Balance { get; private set; }
        public long Nonce { get; private set; }

        public Account(Address address)
        {
            Address = address;
            Nonce = 0;
            Balance = 0;
        }

        /// <summary>
        /// Synchronizes account properties (such as nonce, balance) with the ones queried from the Network
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public async Task Sync(IKleverProvider provider)
        {
            var acc = await provider.GetAccount(Address.Bech32);

            Balance = acc.Balance;
            Nonce = acc.Nonce;
        }

        /// <summary>
        /// Increments (locally) the nonce (the account sequence number).
        /// </summary>
        public void IncrementNonce()
        {
            Nonce++;
        }
    }
}
