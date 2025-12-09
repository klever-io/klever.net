using System;
using System.Collections.Generic;
using kleversdk.provider.Helper;

namespace kleversdk.provider.Dto
{

    public class TransactionAPIResult
    {
        public TransactionAPI Tx { get; set; }
    }

    public class TransactionAPI
    {
        public string Hash { get; set; }
        public long BlockNum { get; set; }
        public string Sender { get; set; }
        public long Nonce { get; set; }

        public long PermissionID { get; set; }
        public string[] Data { get; set; }
        public long Timestamp { get; set; }
        public long    KAppFee { get; set; }
        public long    BandwidthFee { get; set; }
        public string Status { get; set; }
        public string ResultCode { get; set; }
        public long Version { get; set; }
        public string ChainID { get; set; }

        public string[] Signature { get; set; }
        public Dictionary<string,object> Receipts { get; set; }
        public IContract[] Contracts { get; set; }

        public string String()
        {
            return JsonSerializerWrapper.Serialize(this);
        }
    }

    public class KDAFee {
        public string KDA { get; set; }
        public long Amount { get; set; }
    }

    public class Transaction_Raw
    {
        public UInt64 Nonce { get; set; }
        public byte[] Sender { get; set; }
        public object[] Contract { get; set; }
        public Int32 PermissionId { get; set; }
        public byte[][] Data { get; set; }
        public Int64 KAppFee { get; set; }
        public Int64 BandwidthFee { get; set; }
        public UInt32 Version { get; set; }
        public byte[] ChainId { get; set; }
        public KDAFee KDAFee  { get; set;}
    }

    public class Receipt
    {
        public byte[][] Data { get; set; }
    }

    public class TransactionResult
    {
        public Transaction Result { get; set; }
    }

    public class ToBroadcast
    {
        public Transaction Tx { get; set; }

        public string String()
        {
            return JsonSerializerWrapper.Serialize(this);
        }
    }

    public class BroadcastResult
    {
        public long TxCount { get; set; }
        public string TxHash { get; set; }

        public string String()
        {
            return JsonSerializerWrapper.Serialize(this);
        }
    }

    public class Transaction
    {
        public Transaction_Raw RawData { get; set; }
        public List<byte[]> Signature { get; set; }
        public Int32 Result{ get; set; }
        public Int32 ResultCode { get; set; }
        public List<Receipt> Receipts{ get; set; }
        public UInt64 Block { get; set; }
        public UInt64 GasLimit { get; set; }
        public UInt64 GasMultiplier { get; set; }


        public Transaction()
        {
            Signature = new List<byte[]>();
            Receipts = new List<Receipt>();
        }

        public string String()
        {
            return JsonSerializerWrapper.Serialize(this);
        }

        public void AddSignature(byte[] signature)
        {
            Signature.Add(signature);
        }

        public void AddSignature(string hexData)
        {
            var signature = Converter.FromHexString(hexData);
            Signature.Add(signature);
        }
    }
}
