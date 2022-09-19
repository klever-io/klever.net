using System;
using System.Collections.Generic;
using kleversdk.provider.Helper;

namespace kleversdk.provider.Dto
{
    public enum TXContract_ContractType : ushort
    {
        TXContract_TransferContractType,
        TXContract_CreateAssetContractType,
        TXContract_CreateValidatorContractType,
        TXContract_ValidatorConfigContractType,
        TXContract_FreezeContractType,
        TXContract_UnfreezeContractType,
        TXContract_DelegateContractType,
        TXContract_UndelegateContractType,
        TXContract_WithdrawContractType,
        TXContract_ClaimContractType,
        TXContract_UnjailContractType,
        TXContract_AssetTriggerContractType,
        TXContract_SetAccountNameContractType,
        TXContract_ProposalContractType,
        TXContract_VoteContractType,
        TXContract_ConfigITOContractType,
        TXContract_SetITOPricesContractType,
        TXContract_BuyContractType,
        TXContract_SellContractType,
        TXContract_CancelMarketOrderContractType,
        TXContract_CreateMarketplaceContractType,
        TXContract_ConfigMarketplaceContractType,
        TXContract_UpdateAccountPermissionContractType
    }

    public class SendRequest
    {
        public TXContract_ContractType Type { get; set; }
        public string Sender { get; set; }
        public long Nonce { get; set; }
        public long PermID { get; set; }
        public byte[][] Data { get; set; }
        public IContract Contract { get; set; }
        public List<IContract> Contracts { get; set; }

        public string String()
        {
            return JsonSerializerWrapper.Serialize(this);
        }

    }

    public interface IContract
    {
        
    }

    public class TransferContract: IContract
    {
        public TransferContract(string receiver, long amount, string kda)
        {
            Receiver = receiver;
            Amount = amount;
            Kda = kda;
        }

        public string Receiver { get; set; }
        public long Amount { get; set; }
        public string Kda { get; set; }


    }
}
