using System;
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
