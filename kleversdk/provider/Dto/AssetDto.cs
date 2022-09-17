using System;
using kleversdk.provider.Helper;

namespace kleversdk.provider.Dto
{
    public class AssetDataDto
    {
        public AssetDto Asset { get; set; }
    }

    public class AssetDto
    {
        public string Address { get; set; }
        public string AssetId { get; set; }
        public long AssetType { get; set; }
        public long Balance { get; set; }
        public long Precision { get; set; }
        public long FrozenBalance { get; set; }
        public long UnfrozenBalance { get; set; }
        public BuketDto[] Buckets { get; set; }


        public string String()
        {
            return JsonSerializerWrapper.Serialize(this);
        }
    }

    public class BuketDto
    {
        public string Id { get; set; }
        public long StakedAt { get; set; }
        public long StakedEpoch { get; set; }
        public long UnstakedEpoch { get; set; }
        public long Balance { get; set; }
        public string Delegation { get; set; }
        public string ValidatorName { get; set; }
    }
}
