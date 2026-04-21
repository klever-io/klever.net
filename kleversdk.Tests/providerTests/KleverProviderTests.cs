using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using kleversdk.provider;
using kleversdk.provider.Dto;
using kleversdk.provider.Exceptions;
using Xunit;

namespace kleversdk.Tests.providerTests
{
    /// <summary>
    /// Minimal HttpMessageHandler that records request bodies and returns a
    /// caller-supplied response. Inject it via:
    ///
    ///   var handler = new MockHttpMessageHandler(myResponse);
    ///   var client  = new HttpClient(handler) { BaseAddress = new Uri("http://test/") };
    ///   var provider = new KleverProvider(nodeClient: client, apiClient: new HttpClient());
    ///
    /// After the call, inspect handler.CapturedRequestBodies[0] to verify
    /// what was serialized and sent to the node.
    /// </summary>
    internal sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

        public List<string> CapturedRequestBodies { get; } = new List<string>();

        public MockHttpMessageHandler(HttpResponseMessage response)
            : this(_ => response) { }

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
        {
            _respond = respond;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Content != null)
            {
                CapturedRequestBodies.Add(await request.Content.ReadAsStringAsync(cancellationToken));
            }

            return _respond(request);
        }
    }

    public class KleverProviderTests
    {
        // -----------------------------------------------------------------------
        // BuildRequest — pure unit tests, no HTTP
        // -----------------------------------------------------------------------

        [Fact]
        public void BuildRequest_EmptyContracts_ThrowsContractsSizeException()
        {
            var provider = new KleverProvider();
            var ex = Assert.Throws<ContractsSizeException>(() =>
                provider.BuildRequest(
                    TXContract_ContractType.TXContract_TransferContractType,
                    "sender",
                    1,
                    new List<IContract>()));

            Assert.Contains("0", ex.Message);
        }

        [Fact]
        public void BuildRequest_TwentyOneContracts_ThrowsContractsSizeException()
        {
            var provider = new KleverProvider();
            var contracts = new List<IContract>();
            for (int i = 0; i < 21; i++)
            {
                contracts.Add(new TransferContract("addr", 1_000_000, "KLV"));
            }

            Assert.Throws<ContractsSizeException>(() =>
                provider.BuildRequest(
                    TXContract_ContractType.TXContract_TransferContractType,
                    "sender",
                    1,
                    contracts));
        }

        [Fact]
        public void BuildRequest_ValidSingleContract_ReturnsSendRequestWithCorrectFields()
        {
            var provider = new KleverProvider();
            var contracts = new List<IContract>
            {
                new TransferContract("receiver", 1_000_000, "KLV")
            };

            var request = provider.BuildRequest(
                TXContract_ContractType.TXContract_TransferContractType,
                "sender",
                42,
                contracts);

            Assert.Equal("sender", request.Sender);
            Assert.Equal(42, request.Nonce);
            Assert.Equal(TXContract_ContractType.TXContract_TransferContractType, request.Type);
            Assert.Single(request.Contracts);
        }

        // -----------------------------------------------------------------------
        // MultiAssetTransfer validation — throw before any HTTP call
        // -----------------------------------------------------------------------

        [Fact]
        public async Task MultiAssetTransfer_EmptyAddress_ThrowsArgumentException()
        {
            var provider = new KleverProvider();
            var transfers = new List<MultiAssetTx>
            {
                new MultiAssetTx("", 1.0m, "KLV", 6)
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                provider.MultiAssetTransfer("sender", 1, transfers));

            Assert.Contains("Address cannot be empty", ex.Message);
        }

        [Fact]
        public async Task MultiAssetTransfer_ZeroAmount_ThrowsArgumentException()
        {
            var provider = new KleverProvider();
            var transfers = new List<MultiAssetTx>
            {
                new MultiAssetTx("receiver", 0m, "KLV", 6)
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                provider.MultiAssetTransfer("sender", 1, transfers));

            Assert.Contains("Amount must be greater than 0", ex.Message);
        }

        [Fact]
        public async Task MultiAssetTransfer_NegativePrecision_ThrowsArgumentException()
        {
            var provider = new KleverProvider();
            var transfers = new List<MultiAssetTx>
            {
                new MultiAssetTx("receiver", 1.0m, "KLV", -1)
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                provider.MultiAssetTransfer("sender", 1, transfers));

            Assert.Contains("Precision cannot be negative", ex.Message);
        }

        // -----------------------------------------------------------------------
        // Amount conversion — uses MockHttpMessageHandler for PrepareTransaction
        // -----------------------------------------------------------------------

        [Fact]
        public async Task MultiAssetTransfer_Precision6_ConvertsOneKLVToOneMillion()
        {
            var nodeHandler = new MockHttpMessageHandler(OkTransactionResponse());
            var provider = new KleverProvider(
                nodeClient: new HttpClient(nodeHandler) { BaseAddress = new Uri("http://node.test/") },
                apiClient: new HttpClient());

            var transfers = new List<MultiAssetTx>
            {
                new MultiAssetTx("receiver", 1.0m, "KLV", 6)
            };

            await provider.MultiAssetTransfer("sender", 1, transfers);

            Assert.Single(nodeHandler.CapturedRequestBodies);
            // JsonSerializerWrapper uses NamingStrategy = null → PascalCase property names
            Assert.Contains("\"Amount\":1000000", nodeHandler.CapturedRequestBodies[0]);
        }

        [Fact]
        public async Task MultiAssetTransfer_Precision0_ConvertsNFTAmountAsWhole()
        {
            var nodeHandler = new MockHttpMessageHandler(OkTransactionResponse());
            var provider = new KleverProvider(
                nodeClient: new HttpClient(nodeHandler) { BaseAddress = new Uri("http://node.test/") },
                apiClient: new HttpClient());

            // NFT quantities are whole numbers; precision 0 means no multiplication
            var transfers = new List<MultiAssetTx>
            {
                new MultiAssetTx("receiver", 3m, "MYNFT/1", 0)
            };

            await provider.MultiAssetTransfer("sender", 1, transfers);

            Assert.Contains("\"Amount\":3,", nodeHandler.CapturedRequestBodies[0]);
        }

        [Fact]
        public async Task MultiAssetTransfer_Precision8_ConvertsSmallestUnitCorrectly()
        {
            var nodeHandler = new MockHttpMessageHandler(OkTransactionResponse());
            var provider = new KleverProvider(
                nodeClient: new HttpClient(nodeHandler) { BaseAddress = new Uri("http://node.test/") },
                apiClient: new HttpClient());

            // 0.00000001 BTC with precision 8 → 1 satoshi
            var transfers = new List<MultiAssetTx>
            {
                new MultiAssetTx("receiver", 0.00000001m, "BTC", 8)
            };

            await provider.MultiAssetTransfer("sender", 1, transfers);

            Assert.Contains("\"Amount\":1,", nodeHandler.CapturedRequestBodies[0]);
        }

        [Fact]
        public async Task MultiTransfer_KLV_UsesPrecision6ByDefault()
        {
            var nodeHandler = new MockHttpMessageHandler(OkTransactionResponse());
            var provider = new KleverProvider(
                nodeClient: new HttpClient(nodeHandler) { BaseAddress = new Uri("http://node.test/") },
                apiClient: new HttpClient());

            // 1.5 KLV × 10^6 = 1 500 000
            var values = new[] { new ToAmount("receiver", 1.5f) };
            await provider.MultiTransfer("sender", 1, "KLV", values, "");

            Assert.Contains("\"Amount\":1500000", nodeHandler.CapturedRequestBodies[0]);
        }

        // -----------------------------------------------------------------------
        // APIResponseDto — error handling, no HTTP needed
        // -----------------------------------------------------------------------

        [Fact]
        public void APIResponseDto_SuccessfulCode_DoesNotThrow()
        {
            var dto = new APIResponseDto<string>
            {
                Data = "ok",
                Error = "",
                Code = "successful"
            };

            // Must not throw
            dto.EnsureSuccessStatusCode();
        }

        [Fact]
        public void APIResponseDto_ErrorCode_ThrowsAPIException()
        {
            var dto = new APIResponseDto<string>
            {
                Data = null,
                Error = "not found",
                Code = "error"
            };

            Assert.Throws<APIException>(() => dto.EnsureSuccessStatusCode());
        }

        // -----------------------------------------------------------------------
        // PrepareTransaction — mocked HTTP error path
        // -----------------------------------------------------------------------

        [Fact]
        public async Task PrepareTransaction_ApiErrorResponse_ThrowsAPIException()
        {
            const string errorJson =
                """{"data":null,"error":"insufficient funds","code":"error"}""";

            var nodeHandler = new MockHttpMessageHandler(
                _ => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(errorJson, Encoding.UTF8, "application/json")
                });

            var provider = new KleverProvider(
                nodeClient: new HttpClient(nodeHandler) { BaseAddress = new Uri("http://node.test/") },
                apiClient: new HttpClient());

            var request = provider.BuildRequest(
                TXContract_ContractType.TXContract_TransferContractType,
                "sender",
                1,
                new List<IContract> { new TransferContract("receiver", 1_000_000, "KLV") });

            await Assert.ThrowsAsync<APIException>(() => provider.PrepareTransaction(request));
        }

        // -----------------------------------------------------------------------
        // Helper
        // -----------------------------------------------------------------------

        private static HttpResponseMessage OkTransactionResponse()
        {
            // Minimal valid APIResponseDto<TransactionResult> that passes
            // EnsureSuccessStatusCode and deserializes without error.
            const string json =
                """{"data":{"result":{}},"error":"","code":"successful"}""";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
