using System;
using kleversdk.provider.Helper;

namespace kleversdk.provider.Dto
{
    public class KDADataDto
    {
        public KDADto Asset { get; set; }
    }

    public class KDADto
    {
        public string AssetType { get; set; }
        public string AssetID { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public string OwnerAddress { get; set; }
        public string Logo { get; set; }
        //URIs              []*URI          `json:"uris,omitempty"`
        public long Precision { get; set; }
        public long InitialSupply { get; set; }
        public long CirculatingSupply { get; set; }
        public long MaxSupply { get; set; }
        public long MintedValue { get; set; }
        public long BurnedValue { get; set; }

        //IssueDate int64           `json:"issueDate"`
        //Royalties* RoyaltiesInfo  `json:"royalties"`
        //Staking* StakingData    `json:"staking,omitempty"`
        //Properties* PropertiesInfo `json:"properties"`
        //Attributes* AttributesInfo `json:"attributes"`
        //Roles[]*RolesInfo    `json:"roles,omitempty"`
        //ITO* ITOInfo        `json:"ito,omitempty"`

        public bool Hidden { get; set; }
        public bool Verified { get; set; }


        public string Metadata { get; set; }
        public string Mime { get; set; }

        public string String()
        {
            return JsonSerializerWrapper.Serialize(this);
        }
    }
}
