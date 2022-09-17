using System;
using System.Threading.Tasks;
using kleversdk.provider;
using kleversdk.provider.Dto;

namespace demo
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var kp = new KleverProvider(new NetworkConfig(Network.MainNet));
            Task<AccountDto> t = kp.GetAccount("klv1j6z5yzhaxdp0am0ewnv0tczr9nhnwdt08maz9p6xegcq3uyhcxtq6eu9qz");
            t.Wait();
            var acc = t.Result;
            
            Console.WriteLine(acc.String());
        }
    }
}
