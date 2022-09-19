using System;
using System.Collections;
using System.Collections.Generic;
using kleversdk.provider.Helper;

namespace kleversdk.provider.Dto
{
    public class AccountDataDto
    {
        public AccountDto Account { get; set; }
    }

    public class AccountDto
    {
        public string Address { get; set; }
        public long Nonce { get; set; }
        public string Username { get; set; }
        public string RootHash { get; set; }
        public long Balance { get; set; }
        public long FrozenBalance { get; set; }
        public long Allowance { get; set; }

        public Dictionary<string, AssetDto> Assets { get; set; }


        public string String()
        {
            return JsonSerializerWrapper.Serialize(this);
        }
    }
}
