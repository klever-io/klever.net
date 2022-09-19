using System;
using System.Threading.Tasks;
using kleversdk.core;
using kleversdk.provider;
using kleversdk.provider.Dto;

namespace demo
{
    class MainClass
    {
        public static async Task Main(string[] args)
        {
            

            var kp = new KleverProvider(new NetworkConfig(Network.MainNet));
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
                Console.WriteLine("account does no exsist in blockchain yet: {0}", e.ToString());
                return;
            }

            Console.WriteLine("Acc Balance: {0}", acc.Balance);

            try
            {
                var tx = await kp.Send(acc.Address.Bech32, acc.Nonce, acc2.Address.Bech32, 100);
                var decoded = await kp.Decode(tx);
                var signature = wallet.SignHex(decoded.Hash);
                tx.AddSiganture(signature);

                // broadcast
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
