using System;
namespace kleversdk.provider.Dto
{
    public class ToAmount
    {
        public string To { get; set; }
        public float Amount { get; set; }

        public ToAmount(string to, float amount)
        {
            To = to;
            Amount = amount;
        }
    }

}
