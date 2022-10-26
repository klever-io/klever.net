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

    public class TransferContract : IContract
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
          public class ClaimContract : IContract
        {
            public ClaimContract(int claimType, string id)
            {
                ClaimType = claimType;
                Id = id;
            }

            public int ClaimType { get; set; }
            public string Id { get; set; }
        }
        public class FreezeContract : IContract
        {
            public FreezeContract(long amount, string kda = "klv")
            {
                this.amount = amount;
                this.kda = kda;
            }

            public long amount { get; set; }
            public string kda { get; set; }
        }
        public class UnfreezeContract : IContract
        {
            public UnfreezeContract(string bucketID, string kda = "KLV")
            {
                BucketID = bucketID;
                Kda = kda;
            }

            public string BucketID { get; set; }
            public string Kda { get; set; }
        }
        public class DelegateContract : IContract
        {
            public DelegateContract(string receiver, string bucketID)
            {
                Receiver = receiver;
                BucketID = bucketID;
            }

            public string Receiver { get; set; }
            public string BucketID { get; set; }

        }
        public class UndelegateContract : IContract
        {
            public UndelegateContract(string bucketID)
            {
                BucketID = bucketID;
            }

            public string BucketID { get; set; }
        }
        public class WithdrawContract : IContract
        {
            public WithdrawContract(string kda)
            {
                Kda = kda;
            }

            public string Kda { get; set; }
        }
        public class ProposalContract : IContract
        {
            public ProposalContract(Dictionary<Int32, string> parameter, long epochsDuration, string description = "")
            {
                Parameter = parameter;
                EpochsDuration = epochsDuration;
                Description = description;
            }

            public Dictionary<Int32, string> Parameter { get; set; }
            public long EpochsDuration { get; set; }
            public string Description { get; set; }
        }

        public class VoteContract : IContract
        {
            public VoteContract(long amount, long proposalID, int type)
            {
                Amount = amount;
                ProposalID = proposalID;
                Type = type;
            }

            public long Amount { get; set; }
            public long ProposalID { get; set; }
            public int Type { get; set; }
        }
    public class CreateAssetContract : IContract
    {
        public CreateAssetContract(string name, string ticker, string ownerAddress, int precision, Dictionary<string, string> uris = null, string logo = "", long initialSupply = default, long maxSupply = default, int type = default, StakingObject staking = default, Royaltiesobject royalties = default, List<Role> roles = null, Propertiesobject properties = default, Attributesobject attributes = default)
        {
            Name = name;
            Ticker = ticker;
            this.ownerAddress = ownerAddress;
            this.precision = precision;
            Uris = uris;
            this.logo = logo;
            this.initialSupply = initialSupply;
            this.maxSupply = maxSupply;
            this.type = type;
            this.staking = staking;
            this.royalties = royalties;
            this.roles = roles;
            this.properties = properties;
            this.attributes = attributes;

        }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public string ownerAddress { get; set; }
        public int precision { get; set; }
        public Dictionary<string, string> Uris { get; set; }
        public string logo { get; set; }
        public long initialSupply { get; set; }
        public long maxSupply { get; set; }
        public int type { get; set; }
        public StakingObject staking { get; set; }
        public Royaltiesobject royalties { get; set; }
        public List<Role> roles { get; set; }
        public Propertiesobject properties { get; set; }
        public Attributesobject attributes { get; set; }
    }
            public struct StakingObject
            {
                public int type;
                public long apr;
                public long minEpochsToClaim;
                public long minEpochsToUnstake;
                public long minEpochsToWithdraw;
            }

            public struct Royaltiesobject
            {
                public string address;
                public List<RoyalityInfo> transferPercentage;
                public long transferFixed;
                public long marketPercentage;
                public long marketFixed;
            }


            public struct Propertiesobject
            {
                public bool canFreeze;
                public bool canWipe;
                public bool canPause;
                public bool canMint;
                public bool canBurn;
                public bool canChangeOwner;
                public bool canAddRoles;
            }
            public struct Attributesobject
            {
                public bool isPaused;
                public bool isNFTMintStopped;
            }
        

        public class Role
        {
            public Role(string Address, bool HasRoleMint, bool HasRoleSetITOPrices)
            {
                this.Address = Address;
                this.HasRoleMint = HasRoleMint;
                this.HasRoleSetITOPrices = HasRoleSetITOPrices;
            }

            public string Address { get; set; }
            public bool HasRoleMint { get; set; }
            public bool HasRoleSetITOPrices { get; set; }
        }

        public class TriggerAssetContract : IContract
        {
            public TriggerAssetContract(int triggerType, string assetID, string receiver = null, float amount = default, Dictionary<string, string> uris = null, string logo = null, string mime = null, Role role = null, StakingObject staking = default)
            {
                this.triggerType = triggerType;
                AssetID = assetID;
                Receiver = receiver;
                Amount = amount;
                Uris = uris;
                Logo = logo;
                this.mime = mime;
                this.role = role;
                this.staking = staking;
            }

            public int triggerType { get; set; }
            public string AssetID { get; set; }
            public string Receiver { get; set; }
            public float Amount { get; set; }
            public Dictionary<string, string> Uris { get; set; }
            public string Logo { get; set; }
            public string mime { get; set; }
            public Role role { get; set; }
            public StakingObject staking { get; set; }

        }

        public class ConfigITOContract : IContract
        {
            public ConfigITOContract(string receiverAddress, string kda, float maxAmount, int status, packInfo packInfo)
            {
                this.receiverAddress = receiverAddress;
                this.kda = kda;
                this.maxAmount = maxAmount;
                this.status = status;
                this.packInfo = packInfo;
            }

            public string receiverAddress { get; set; }
            public string kda { get; set; }
            public float maxAmount { get; set; }
            public int status { get; set; }
            public packInfo packInfo { get; set; }
        }

        public class SetITOContract : IContract
        {
            public SetITOContract(string kda, packInfo packInfo)
            {
                this.kda = kda;
                this.packInfo = packInfo;
            }
            public string kda { get; set; }
            public packInfo packInfo { get; set; }
        }
        public struct packInfo
        {
            public string key;
            public List<packs> packs;
        }
        public struct packs
        {
            public float amount;
            public float price;
        }
        public class CreateMarketplace : IContract
        {
            public CreateMarketplace(string name, string referralAddress = null, float referralPercentage = default)
            {
                this.name = name;
                this.referralAddress = referralAddress;
                this.referralPercentage = referralPercentage;
            }

            public string name { get; set; }
            public string referralAddress { get; set; }
            public float referralPercentage { get; set; }
        }
        public class ConfigMarketplace : IContract
        {
            public ConfigMarketplace(string name, string marketID, float referralPercentage, string referralAddress = null)
            {
                this.name = name;
                this.referralAddress = referralAddress;
                this.referralPercentage = referralPercentage;
                this.marketID = marketID;
            }

            public string name { get; set; }
            public string marketID { get; set; }
            public string referralAddress { get; set; }
            public float referralPercentage { get; set; }
        }

        public class Sell : IContract
        {
            public Sell(int marketType, string marketplaceId, float assetId, string currencyId, float endTime, float price = default, float reservePrice = default)
            {

                this.marketType = marketType;
                this.marketplaceId = marketplaceId;
                this.assetId = assetId.ToString();
                this.currencyId = currencyId;
                this.price = price;
                this.reservePrice = reservePrice;
                this.endTime = endTime;
            }
            public int marketType { get; set; }
            public string marketplaceId { get; set; }
            public string assetId { get; set; }
            public string currencyId { get; set; }
            public float price { get; set; }
            public float reservePrice { get; set; }
            public float endTime { get; set; }
        }
        public class Buy : IContract
        {
            public Buy(int buyType, string id, string currencyId, float amount)
            {

                this.buyType = buyType;
                this.id = id;
                this.currencyId = currencyId;
                this.amount = amount;

            }
            public int buyType { get; set; }
            public string id { get; set; }
            public string currencyId { get; set; }
            public float amount { get; set; }

        }
        public class CancelMarketOrder : IContract
        {
            public CancelMarketOrder(string orderId)
            {

                this.orderId = orderId;
            }
            public string orderId { get; set; }
        }
        public class CreateValidator : IContract
        {
            public CreateValidator(string name, string address, string rewardAddress, string bls = null, bool canDelegate = default, float maxDelegationAmount = default, float comission = default, string logo = null, Dictionary<string, string> uris = null)
            {
                this.name = name;
                this.address = address;
                this.rewardAddress = rewardAddress;
                this.bls = bls;
                this.canDelegate = canDelegate;
                this.maxDelegationAmount = maxDelegationAmount;
                this.comission = comission;
                this.logo = logo;
                this.uris = uris;
            }
            public string name { get; set; }
            public string address { get; set; }
            public string rewardAddress { get; set; }
            public string bls { get; set; }
            public bool canDelegate { get; set; }
            public float maxDelegationAmount { get; set; }
            public float comission { get; set; }
            public string logo { get; set; }
            public Dictionary<string, string> uris { get; set; }

        }
        public class ConfigValidator : IContract
        {
            public ConfigValidator(string name, string rewardAddress, string bls = null, bool canDelegate = default, float maxDelegationAmount = default, float comission = default, string logo = null, Dictionary<string, string> uris = null)
            {
                this.name = name;
                this.rewardAddress = rewardAddress;
                this.bls = bls;
                this.canDelegate = canDelegate;
                this.maxDelegationAmount = maxDelegationAmount;
                this.comission = comission;
                this.logo = logo;
                this.uris = uris;
            }
            public string name { get; set; }
            public string rewardAddress { get; set; }
            public string bls { get; set; }
            public bool canDelegate { get; set; }
            public float maxDelegationAmount { get; set; }
            public float comission { get; set; }
            public string logo { get; set; }
            public Dictionary<string, string> uris { get; set; }

        }

        public class SetAccountNameContract : IContract
        {
            public SetAccountNameContract(string name)
            {
                this.name = name;
            }

            public string name { get; set; }
        }

        public class UpdateAccountPermissionContract : IContract
        {
            public UpdateAccountPermissionContract(List<AccPermission> permissions)
            {
                this.permissions = permissions;
            }

            public List<AccPermission> permissions { get; set; }
        }
        public struct AccPermission
        {
            public int type;
            public string permissionName;
            public long threshold;
            public string operations;
            public List<AccKey> signers;
        }
        public struct AccKey
        {
            public string address;
            public int weight;
        }
        [Serializable]
        public class RoyalityInfo
        {
            private long _amount;
            private long _percentage;

            public RoyalityInfo(long amount, long percentage)
            {
                _amount = amount;
                _percentage = percentage;
            }

            public long Amount
            {
                get
                {
                    return _amount;
                }
                set
                {
                    _amount = value;
                }
            }

            public long Percentage
            {
                get
                {
                    return _percentage;
                }
                set
                {
                    _percentage = value;
                }
            }
        }
    }


