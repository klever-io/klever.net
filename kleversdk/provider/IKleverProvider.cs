﻿using System;
using System.Collections.Generic;
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
        Task<KDADto> GetAsset(string assetID);
        Task<TransactionAPI> Decode(Transaction tx);
        Task<Transaction> Send(string fromAddr, long nonce, string toAddr, float amount, string kda = "KLV", long permID = 0);
        Task<provider.Dto.Transaction> Claim(string fromAddr, long nonce, int claimType, string id = "KLV", long permID = 0);
        Task<provider.Dto.Transaction> Freeze(string fromAddr, long nonce, float Amount, string kda = "KLV", long permID = 0);
        Task<provider.Dto.Transaction> Unfreeze(string fromAddr, long nonce, string BucketID, string kda = "KLV", long permID = 0);
        Task<provider.Dto.Transaction> DelegateValidator(string fromAddr, long nonce, string receiver, string BucketID, long permID = 0);
        Task<provider.Dto.Transaction> UndelegateValidator(string fromAddr, long nonce, string BucketID, long permID = 0);    
        Task<provider.Dto.Transaction> Withdraw(string fromAddr, long nonce, string kda, long permID = 0);
        Task<provider.Dto.Transaction> Proposal(string fromAddr, long nonce, Dictionary<Int32, string> Parameter, long epochsDuration, string Description = null, long permID = 0);
        Task<provider.Dto.Transaction> Vote(string fromAddr, long nonce, float amount, long proposalID, int type, long permID = 0);
        Task<provider.Dto.Transaction> CreateAsset(string name, string ticker, string owner, long nonce, int precision, Dictionary<string, string> Uris = null, string logo = null, long initialSupply = default, long maxSupply = default, int type = default, provider.Dto.StakingObject staking = default, provider.Dto.Royaltiesobject royalties = default, System.Collections.Generic.List<provider.Dto.Role> roles = null, provider.Dto.Propertiesobject properties = default, provider.Dto.Attributesobject attributes = default, long permID = 0);
        Task<provider.Dto.Transaction> TriggerAsset(string fromAddr, long nonce, int triggerType, string assetID, string receiver = null, float amount = default, Dictionary<string, string> uris = null, string logo = null, string mime = null, provider.Dto.Role role = null, provider.Dto.StakingObject staking = default, long permID = 0);
        Task<provider.Dto.Transaction> ConfigITO(string fromAddr, long nonce, string receiverAddress, string kda, float maxAmount, int status, provider.Dto.packInfo packInfo, long permID = 0);
        Task<provider.Dto.Transaction> SetITOPrices(string fromAddr, long nonce, string kda, provider.Dto.packInfo packInfo, long permID = 0);
        Task<provider.Dto.Transaction> CreateMarketplace(string fromAddr, long nonce, string kda, string name, string referralAddress = null, float referralPercentage = default, long permID = 0);
        Task<provider.Dto.Transaction> ConfigMarketplace(string fromAddr, long nonce, string kda, string name, string marketID, float referralPercentage, string referralAddress = null, long permID = 0);
        Task<provider.Dto.Transaction> Sell(string fromAddr, long nonce, int marketType, string marketplaceId, float assetId, string currencyId, float endTime, float price = default, float reservePrice = default, long permID = 0);
        Task<provider.Dto.Transaction> Buy(string fromAddr, long nonce, int buyType, string id, string currencyId, Int64 amount, long permID = 0);
        Task<provider.Dto.Transaction> CancelMarketOrder(string fromAddr, long nonce, string orderId, long permID = 0);
        Task<provider.Dto.Transaction> CreateValidator(string fromAddr, long nonce, string name, string address, string rewardAddress, string blsPublicKey, bool canDelegate = default, float maxDelegationAmount = default, float comission = default, string logo = null, Dictionary<string, string> uris = null, long permID = 0);
        Task<provider.Dto.Transaction> ConfigValidator(string fromAddr, long nonce, string name, string rewardAddress, string blsPublicKey = null, bool canDelegate = default, float maxDelegationAmount = default, float comission = default, string logo = null, Dictionary<string, string> uris = null, long permID = 0);
        Task<provider.Dto.Transaction> Unjail(string fromAddr, long nonce, long permID = 0);
        Task<provider.Dto.Transaction> SetAccountName(string fromAddr, long nonce, string name, long permID = 0);
        Task<provider.Dto.Transaction> UpdateAccountPermission(string fromAddr, long nonce, List<provider.Dto.AccPermission> permission, long permID = 0);
        Task<BroadcastResult> Broadcast(Transaction tx);
    }
}
