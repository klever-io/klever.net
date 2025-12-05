namespace kleversdk.provider.Dto
{
    /// <summary>
    /// Represents a single transfer in a multi-asset transaction
    /// Used with MultiAssetTransfer to send different assets in one transaction
    /// </summary>
    public class MultiAssetTx
    {
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public string Asset { get; set; }
        public int Precision { get; set; }
        
        public MultiAssetTx(string address, decimal amount, string asset, int precision)
        {
            Address = address;
            Amount = amount;
            Asset = asset;
            Precision = precision;
        }
    }
}
