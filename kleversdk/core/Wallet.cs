using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using dotnetstandard_bip39;
using kleversdk.provider.Helper;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using kleversdk.crypto.Values;

namespace kleversdk.core
{
    public class Wallet
    {
        private readonly byte[] _privateKey;
        private readonly byte[] _publicKey;
        private static readonly RNGCryptoServiceProvider RngCsp = new RNGCryptoServiceProvider();

        public Wallet(string privateKeyHex)
            : this(Converter.FromHexString(privateKeyHex))
        {
        }

        /// <summary>
        /// Get the account wallet
        /// </summary>
        /// <returns>Account</returns>
        public Account GetAccount()
        {
            return new Account(Address.FromBytes(_publicKey));
        }

        /// <summary>
        /// Build a wallet
        /// </summary>
        /// <param name="privateKey">The private key</param>
        public Wallet(byte[] privateKey)
        {
            var privateKeyParameters = new Ed25519PrivateKeyParameters(privateKey, 0);
            var publicKeyParameters = privateKeyParameters.GeneratePublicKey();
            _publicKey = publicKeyParameters.GetEncoded();
            _privateKey = privateKey;
        }

        /// <summary>
        /// Derive a wallet from Mnemonic phrase
        /// </summary>
        /// <param name="mnemonic">The mnemonic phrase</param>
        /// <param name="accountIndex">The account index, default 0</param>
        /// <returns>Wallet</returns>
        public static Wallet DeriveFromMnemonic(string mnemonic, int accountIndex = 0)
        {
            try
            {
                var bip39 = new BIP39();
                var seedHex = bip39.MnemonicToSeedHex(mnemonic, "");

                var hdPath = $"{Constants.HDPrefix}/{accountIndex}'";
                var kv = DerivePath(hdPath, seedHex);
                var privateKey = kv.Key;
                return new Wallet(privateKey);
            }
            catch (Exception ex)
            {
                throw new Exception("CannotDeriveKeysException", ex);
            }
        }

        /// <summary>
        /// Sign data string with the wallet
        /// </summary>
        /// <param name="data">The data to signed</param>
        /// <returns>Signature</returns>
        public string Sign(string data)
        {
            return Sign(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Sign HEX string with the wallet
        /// </summary>
        /// <param name="data">The data to signed</param>
        /// <returns>Signature</returns>
        public string SignHex(string data)
        {    
            return Sign(Converter.FromHexString(data));
        }

        /// <summary>
        /// Sign data with the wallet
        /// </summary>
        /// <param name="data">The data to signed</param>
        /// <returns>Signature</returns>
        public string Sign(byte[] data)
        {
            var parameters = new Ed25519PrivateKeyParameters(_privateKey, 0);
            var signer = new Ed25519Signer();
            signer.Init(true, parameters);
            signer.BlockUpdate(data, 0, data.Length);
            var signature = signer.GenerateSignature();

            return Converter.ToHexString(signature).ToLowerInvariant();
        }

        /// <summary>
        /// The private key. Do not share
        /// </summary>
        /// <returns>Private key</returns>
        public byte[] GetPrivateKey()
        {
            return _privateKey;
        }

        /// <summary>
        /// The public key
        /// </summary>
        /// <returns>Public key</returns>
        public byte[] GetPublicKey()
        {
            return _publicKey;
        }

        
        private static string CreateSha256Signature(byte[] key, string targetText)
        {
            var data = Converter.FromHexString(targetText);
            byte[] mac;
            using (var hmac = new HMACSHA256(key))
            {
                mac = hmac.ComputeHash(data);
            }

            return Converter.ToHexString(mac).ToLowerInvariant();
        }


        private static (byte[] Key, byte[] ChainCode) DerivePath(string path, string seed)
        {
            var masterKeyFromSeed = GetMasterKeyFromSeed(seed);
            var segments = path
                          .Split('/')
                          .Skip(1)
                          .Select(a => a.Replace("'", ""))
                          .Select(a => Convert.ToUInt32(a, 10));

            var results = segments
               .Aggregate(masterKeyFromSeed, (mks, next) => GetChildKeyDerivation(mks.Key, mks.ChainCode, next + 0x80000000));

            return results;
        }

        private static (byte[] Key, byte[] ChainCode) GetMasterKeyFromSeed(string seed)
        {
            using (var hmacSha512 = new HMACSHA512(Encoding.UTF8.GetBytes("ed25519 seed")))
            {
                var i = hmacSha512.ComputeHash(Converter.FromHexString(seed));

                var il = i.Slice(0, 32);
                var ir = i.Slice(32);

                return (Key: il, ChainCode: ir);
            }
        }

        private static (byte[] Key, byte[] ChainCode) GetChildKeyDerivation(byte[] key, byte[] chainCode, uint index)
        {
            var buffer = new BigEndianBuffer();

            buffer.Write(new byte[] { 0 });
            buffer.Write(key);
            buffer.WriteUInt(index);

            using (var hmacSha512 = new HMACSHA512(chainCode))
            {
                var i = hmacSha512.ComputeHash(buffer.ToArray());

                var il = i.Slice(0, 32);
                var ir = i.Slice(32);

                return (Key: il, ChainCode: ir);
            }
        }
    }
}
