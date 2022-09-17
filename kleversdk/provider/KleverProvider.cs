using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using kleversdk.provider.Dto;
using kleversdk.provider.Helper;


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

    }
}
