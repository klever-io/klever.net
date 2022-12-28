﻿using System;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using kleversdk.provider.Dto;
using kleversdk.provider.Helper;
using kleversdk.provider.Exceptions;
using kleversdk.core;


namespace kleversdk.provider
{
    public class KleverProvider : IKleverProvider
    {
        
        private readonly HttpClient _nodeClient;
        private readonly HttpClient _apiClient;

        public KleverProvider(NetworkConfig config = null)
        {
            _nodeClient = new HttpClient();
            _apiClient = new HttpClient();
            if (config != null)
            {
                _nodeClient.BaseAddress = config.NodeUri;
                _apiClient.BaseAddress = config.APIUri;
            }
        }

        public async Task<AccountDto> GetAccount(string address)
        {
            var response = await _apiClient.GetAsync($"address/{address}");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializerWrapper.Deserialize<APIResponseDto<AccountDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data.Account;
        }

        public async Task<KDADto> GetAsset(string assetID)
        {
            var response = await _apiClient.GetAsync($"assets/{assetID}");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializerWrapper.Deserialize<APIResponseDto<KDADataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data.Asset;
        }

        public async Task<TransactionAPI> Decode(Transaction tx)
        {

            var data = tx.String();
            var dataContent = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _nodeClient.PostAsync($"transaction/decode", dataContent);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializerWrapper.Deserialize<APIResponseDto<TransactionAPIResult>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data.Tx;
        }


        public async Task<Transaction> Send(string fromAddr, long nonce, string toAddr, float amount, string kda = "KLV")
        {
            ToAmount[] values = { new ToAmount(toAddr, amount)};

            return await this.MultiTransfer(fromAddr, nonce, kda, values);
        }

        public async Task<provider.Dto.Transaction> Claim(string fromAddr, long nonce, int claimType, string id = "KLV")
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.ClaimContract(claimType, id));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_ClaimContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> Freeze(string fromAddr, long nonce, float Amount, string kda = "KLV")
        {
            long precision = 6L;
            long parsedAmount = Convert.ToInt64(Amount * Math.Pow(10d, precision));
            if (!(kda.ToLower() == "klv"))
            {
                try
                {
                    var asset = await GetAsset(kda);
                    precision = asset.Precision;
                    parsedAmount = Convert.ToInt64(Amount * Math.Pow(10d, precision));
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.FreezeContract(parsedAmount, kda));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_FreezeContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> Unfreeze(string fromAddr, long nonce, string BucketID, string kda = "KLV")
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.UnfreezeContract(BucketID, kda));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_UnfreezeContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> DelegateValidator(string fromAddr, long nonce, string receiver, string BucketID)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.DelegateContract(receiver, BucketID));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_DelegateContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> UndelegateValidator(string fromAddr, long nonce, string BucketID)
        {
            List<IContract> list = new List<IContract>();
            list.Add(new UndelegateContract(BucketID));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_UndelegateContractType, fromAddr, nonce, list);
            return await this.PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> Withdraw(string fromAddr, long nonce, string kda)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.WithdrawContract(kda));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_WithdrawContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> Proposal(string fromAddr, long nonce, Dictionary<Int32, string> parameter, long ePochsDuration, string Description = null)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.ProposalContract(parameter, ePochsDuration, Description));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_ProposalContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> Vote(string fromAddr, long nonce, float amount, long proposalID, int type)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.VoteContract((long)Math.Round(amount), proposalID, type));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_VoteContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> CreateAsset(string name, string ticker, string owner, long nonce, int precision, Dictionary<string, string> uris = null, string logo = null, long initialSupply = default, long maxSupply = default, int type = default, provider.Dto.StakingObject staking = default, provider.Dto.Royaltiesobject royalties = default, List<provider.Dto.Role> roles = null, provider.Dto.Propertiesobject properties = default, provider.Dto.Attributesobject attributes = default)
        {
            var list = new List<provider.Dto.IContract>();

            list.Add(new provider.Dto.CreateAssetContract(name, ticker, owner, precision, uris, logo, initialSupply, maxSupply, type, staking, royalties, roles, properties, attributes));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_CreateAssetContractType, owner, nonce, list);
            return await PrepareTransaction(data);
        }

        public async Task<provider.Dto.Transaction> TriggerAsset(string fromAddr, long nonce, int triggerType, string assetID, string receiver = null, float amount = default, Dictionary<string, string> uris = null, string logo = null, string mime = null, provider.Dto.Role role = null, provider.Dto.StakingObject staking = default)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.TriggerAssetContract(triggerType, assetID, receiver, amount, uris, logo, mime, role, staking));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_AssetTriggerContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }

        public async Task<provider.Dto.Transaction> ConfigITO(string fromAddr, long nonce, string receiverAddress, string kda, float maxAmount, int status, provider.Dto.packInfo packInfo)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.ConfigITOContract(receiverAddress, kda, maxAmount, status, packInfo));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_ConfigITOContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> SetITOPrices(string fromAddr, long nonce, string kda, provider.Dto.packInfo packInfo)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.SetITOContract(kda, packInfo));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_SetITOPricesContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> CreateMarketplace(string fromAddr, long nonce, string kda, string name, string referralAddress = null, float referralPercentage = default)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.CreateMarketplace(name, referralAddress, referralPercentage));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_CreateMarketplaceContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> ConfigMarketplace(string fromAddr, long nonce, string kda, string name, string marketID, float referralPercentage, string referralAddress = null)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.ConfigMarketplace(name, marketID, referralPercentage, referralAddress));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_ConfigMarketplaceContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> Sell(string fromAddr, long nonce, int marketType, string marketplaceId, float assetId, string currencyId, float endTime, float price = default, float reservePrice = default)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.Sell(marketType, marketplaceId, assetId, currencyId, endTime, price, reservePrice));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_SellContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> Buy(string fromAddr, long nonce, int buyType, string id, string currencyId, float amount)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.Buy(buyType, id, currencyId, amount));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_BuyContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }

        public async Task<provider.Dto.Transaction> CancelMarketOrder(string fromAddr, long nonce, string orderId)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.CancelMarketOrder(orderId));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_CancelMarketOrderContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }

        public async Task<provider.Dto.Transaction> CreateValidator(string fromAddr, long nonce, string name, string address, string rewardAddress, string blsPublicKey, bool canDelegate = default, float maxDelegationAmount = default, float comission = default, string logo = null, Dictionary<string, string> uris = null)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.CreateValidator(name, address, rewardAddress, blsPublicKey, canDelegate, maxDelegationAmount, comission, logo, uris));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_CreateValidatorContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }

        public async Task<provider.Dto.Transaction> ConfigValidator(string fromAddr, long nonce, string name, string rewardAddress, string blsPublicKey = null, bool canDelegate = default, float maxDelegationAmount = default, float comission = default, string logo = null, Dictionary<string, string> uris = null)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.ConfigValidator(name, rewardAddress, blsPublicKey, canDelegate, maxDelegationAmount, comission, logo, uris));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_ValidatorConfigContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }

        public async Task<provider.Dto.Transaction> Unjail(string fromAddr, long nonce)
        {
            var list = new List<provider.Dto.IContract>();
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_UnjailContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }
        public async Task<provider.Dto.Transaction> SetAccountName(string fromAddr, long nonce, string name)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.SetAccountNameContract(name));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_SetAccountNameContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }

        public async Task<provider.Dto.Transaction> UpdateAccountPermission(string fromAddr, long nonce, List<provider.Dto.AccPermission> permission)
        {
            var list = new List<provider.Dto.IContract>();
            list.Add(new provider.Dto.UpdateAccountPermissionContract(permission));
            var data = this.BuildRequest(provider.Dto.TXContract_ContractType.TXContract_UpdateAccountPermissionContractType, fromAddr, nonce, list);
            return await PrepareTransaction(data);
        }

        public async Task<Transaction> MultiTransfer(string fromAddr, long nonce, string kda, ToAmount[] values)
        {
            long precision = 6;
            bool isNFT = false;
            if (kda.Contains("/")) {
                isNFT = true;
                precision = 0;
            }

            if (!isNFT && kda.Length > 0 && kda != Constants.KLV && kda != Constants.KFI)
            {
                try
                {
                    var asset = await this.GetAsset(kda);
                    precision = asset.Precision;
                }catch(Exception e)
                {
                    throw e;
                }
                
            }

            List<IContract> contracts = new List<IContract>();

            foreach (var to in values)
            {
                long parsedAmount = Convert.ToInt64(to.Amount * (Math.Pow(10, precision)));
                contracts.Add(new TransferContract(to.To, parsedAmount, kda));
            }

            var data = this.BuildRequest(TXContract_ContractType.TXContract_TransferContractType, fromAddr, nonce, contracts);

            return await this.PrepareTransaction(data);
        }




        public SendRequest BuildRequest(TXContract_ContractType cType, string fromAddress, long nonce, List<IContract> contracts)
        {
            if (contracts.Count == 0 || contracts.Count > 20)
            {
                throw new ContractsSizeException(contracts.Count);
            }

            var request = new SendRequest
            {
                Type = cType,
                Sender = fromAddress,
                Nonce = nonce,
                //PermID = ,
                //Data =,
                Contracts = contracts,
            };

            return request;
        }


        public async Task<Transaction> PrepareTransaction(SendRequest request)
        {
  
            var dataContent = new StringContent(request.String(), Encoding.UTF8, "application/json");
            var response = await _nodeClient.PostAsync($"transaction/send", dataContent);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializerWrapper.Deserialize<APIResponseDto<TransactionResult>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data.Result;
        }

        public async Task<BroadcastResult> Broadcast(Transaction tx)
        {
            var data = new ToBoradcast { Tx = tx }.String();
            var dataContent = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _nodeClient.PostAsync($"transaction/broadcast", dataContent);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializerWrapper.Deserialize<APIResponseDto<BroadcastResult>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }
    }
}
