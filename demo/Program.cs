using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using kleversdk.core;
using kleversdk.provider;
using System.IO;
using kleversdk.provider.Dto;


namespace demo
{
    class MainClass
    {
        public static async Task ScInvokeDemo()
        {
            Console.WriteLine("SmartContracts Invoke Demo");

            var kp = new KleverProvider(new NetworkConfig(Network.TestNet));
            var wallet = new Wallet("hex-private-key");
            var acc = wallet.GetAccount();

            try
            {
                await acc.Sync(kp);
            }
            catch (Exception e)
            {
                Console.WriteLine("account does no exist in blockchain yet: {0}", e.ToString());
                return;
            }

            Console.WriteLine("Address: {0}", acc.Address.Bech32);
            Console.WriteLine("Balance: {0}", acc.Balance);


            // Handle Params

            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds() + 2000;
            List<string[]> scParamsLotery = new List<string[]> {
                new string[] { "TEST" },
                new string[] { "KLV" },
                new string[] { "n","10000" },
                new string[] { "empty", ""},
                new string[] { "optionu",$"{timestamp}"},
                new string[] { "empty", ""},
                new string[] { "empty", ""},
                new string[] { "empty", ""},
            };
            var functionName = "createLotteryPool"; // Name of the method you gonna call in smart contract
            var parameters = kleversdk.core.SmartContract.ToEncodeInvokeSmartContract(functionName, scParamsLotery);

            var scType = 0; // Invoke Type
            var smartContractAddress = "klv1qqqqqqqqqqqqqpgqlg5l6y5mx2zyysgwh37qjzv3e6ywwd5cxgds82pc09"; // Lottery Testnet Address
            var callValue = new Dictionary<string, long> { };

            try
            {
                var tx = await kp.SmartContract(acc.Address.Bech32, acc.Nonce, null, scType, smartContractAddress, callValue, parameters);
                var decoded = await kp.Decode(tx);
                var signature = wallet.SignHex(decoded.Hash);
                tx.AddSignature(signature);
                var broadcastResult = await kp.Broadcast(tx);
                Console.WriteLine("Broadcast result: {0}", broadcastResult.String());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error sending: {0}", e.ToString());
                return;
            }

            return;
        }


        public static async Task SCDeployDemo()
        {
            Console.WriteLine("SmartContracts Deploy Demo");

            var kp = new KleverProvider(new NetworkConfig(Network.TestNet));
            var wallet = new Wallet("hex-private-key");
            var acc = wallet.GetAccount();

            try
            {
                await acc.Sync(kp);
            }
            catch (Exception e)
            {
                Console.WriteLine("account does no exist in blockchain yet: {0}", e.ToString());
                return;
            }

            Console.WriteLine("Address: {0}", acc.Address.Bech32);
            Console.WriteLine("Balance: {0}", acc.Balance);

            // Read the Wasm SC file
            var yourPath = "your-path";
            var scPath = Path.Combine(yourPath, "smartContract", "helloWorld.wasm");

            if (!File.Exists(scPath))
            {
                Console.WriteLine("make sure your path exists");
                return;
            }

            byte[] file = File.ReadAllBytes(scPath);

            var scType = 1; // Deploy Type
            var smartContractAddress = ""; // needs to be empty
            var callValue = new Dictionary<string, long> { };

            List<string[]> scParams = new List<string[]> { }; // in this case we don't need additional params
            var parameters = kleversdk.core.SmartContract.ToEncodeDeploySmartContract(file, scParams, true, false, false, false);

            try
            {
                var tx = await kp.SmartContract(acc.Address.Bech32, acc.Nonce, null, scType, smartContractAddress, callValue, parameters);
                var decoded = await kp.Decode(tx);
                var signature = wallet.SignHex(decoded.Hash);
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

        public static async Task Main(string[] args)
        {

            var kp = new KleverProvider(new NetworkConfig(Network.TestNet));
            Task<AccountDto> t = kp.GetAccount("klv1usdnywjhrlv4tcyu6stxpl6yvhplg35nepljlt4y5r7yppe8er4qujlazy");
            t.Wait();
            var result = t.Result;
            Console.WriteLine(result.String());

            // create wallet with private key
            // var wallet = new Wallet("HEX PRIVATE KEY");
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
            catch (Exception e)
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


            // Send paying with kda

            try
            {
                var kdaFee = "kda here";
                var tx = await kp.Send(acc.Address.Bech32, acc.Nonce, acc2.Address.Bech32, 100, "KLV", kdaFee);

                var decoded = await kp.Decode(tx);
                var signature = wallet.SignHex(decoded.Hash);
                tx.AddSignature(signature);
                Console.WriteLine("tx - Kda Fee: {0}", tx.String());
                // broadcast
                var broadcastResult = await kp.Broadcast(tx);
                Console.WriteLine("Broadcast result - Kda Fee: {0}", broadcastResult.String());
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

            await ScInvokeDemo();
            await SCDeployDemo();
        }
    }
}
