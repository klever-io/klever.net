using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using kleversdk.core;
using kleversdk.provider;
using kleversdk.provider.Dto;

namespace demo
{
    class MainClass
    {
        public static async Task Main(string[] args)
        {
            

            var kp = new KleverProvider(new NetworkConfig(Network.TestNet));
            Task<AccountDto> t = kp.GetAccount("klv1usdnywjhrlv4tcyu6stxpl6yvhplg35nepljlt4y5r7yppe8er4qujlazy");
            t.Wait();
            var result = t.Result;
            Console.WriteLine(result.String());



            // create wallet with private key
            //var wallet = new Wallet("HEX PRIVATE KEY");
            // Mnemonic
var mnemonic = "word1 word2 ....";
            var wallet = Wallet.DeriveFromMnemonic(mnemonic);
            var acc = wallet.GetAccount();
            var wallet2 = Wallet.DeriveFromMnemonic(mnemonic, 1);
            var acc2 = wallet2.GetAccount();
            Console.WriteLine("Acc Address: {0}", acc.Address);
            try
            {
                await acc.Sync(kp);
            }
            catch(Exception e)
            {
                Console.WriteLine("account does no exist in blockchain yet: {0}", e.ToString());
                return;
            }

            Console.WriteLine("Acc Balance: {0}", acc.Balance);

            try
            {
                var tx = await kp.Send(acc.Address.Bech32, acc.Nonce, acc2.Address.Bech32, 100);
                var decoded = await kp.Decode(tx);
                var signature = wallet.SignHex(decoded.Hash);
                tx.AddSignature(signature);

                // broadcast
                var broadcastResult = await kp.Broadcast(tx);
                Console.WriteLine("Broadcast result: {0}", broadcastResult.String());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error sending: {0}", e.ToString());
                return;
            }

            // Create Asset Test

            StakingObject Staking;
            Staking.apr = 1000L; // APR has 2 decimals precision use 1000L for 10%
            Staking.minEpochsToClaim = 2L;
            Staking.minEpochsToUnstake = 2L;
            Staking.minEpochsToWithdraw = 2L;
            Staking.type = 1;

            var RoyalityInfo = new List<RoyalityInfo>();
            RoyalityInfo.Add(new RoyalityInfo(amount: 100L, percentage: 100L));
            RoyalityInfo.Add(new RoyalityInfo(amount: 200L, percentage: 150L));

            Royaltiesobject Royalitys = new Royaltiesobject();
            Royalitys.address = acc.Address.Bech32;
            // MarketFixed and MarketPercentage can only be used for NFTs
            //Royalitys.marketFixed = 10L; 
            //Royalitys.marketPercentage = 10L;
            Royalitys.transferFixed = 1000L;
            Royalitys.transferPercentage = RoyalityInfo;

            var Roles = new List<Role>();
            Roles.Add(new Role("klv1nkyn8z4pxa88w7q8mdf6r92pjmwtm7e306td0vnsggml345ah3pq2l8z6n", HasRoleMint: true, HasRoleSetITOPrices: false));
            
            var Properties = new Propertiesobject();
            Properties.canAddRoles = true;
            Properties.canBurn = true;
            Properties.canChangeOwner = false;
            Properties.canFreeze = true;
            Properties.canMint = true;
            Properties.canPause = true;
            Properties.canWipe = false;

            var Attributes = new Attributesobject();
            Attributes.isNFTMintStopped = false;
            Attributes.isPaused = false;

            var Uris = new Dictionary<string, string>();
            Uris.Add("Twitter", "https://twitter.de");
            try
            {
                var tx = await kp.CreateAsset("TestAsset", "TAS", acc.Address.Bech32, acc.Nonce, 6, Uris, "https://logos/logo1.png", 10000000, 10000000, 0, Staking, Royalitys, Roles, Properties, Attributes);
                var decoded = await kp.Decode(tx);
                string signature = wallet.SignHex(decoded.Hash);
                tx.AddSignature(signature);
                var broadcastResult = await kp.Broadcast(tx);
                Console.WriteLine("Broadcast result: {0}", broadcastResult.String());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error sending: {0}", e.ToString());
                return;
            }
        }
    }
}
