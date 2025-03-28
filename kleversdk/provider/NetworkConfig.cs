﻿using System;
namespace kleversdk.provider
{
    public enum Network
    {
        LocalNet,
        MainNet,
        TestNet,
        DevNet
    }

    public class NetworkConfig
    {
        public NetworkConfig(Network network)
        {
            Network = network;
            switch (network)
            {
                case Network.MainNet:
                    APIUri = new Uri("https://api.mainnet.klever.org");
                    NodeUri = new Uri("https://node.mainnet.klever.org");
                    ExplorerUri = new Uri("https://kleverscan.org/");
                    break;
                case Network.TestNet:
                    APIUri = new Uri("https://api.testnet.klever.org");
                    NodeUri = new Uri("https://node.testnet.klever.org");
                    ExplorerUri = new Uri("https://testnet.kleverscan.org/");
                    break;
                case Network.DevNet:
                    APIUri = new Uri("https://api.devnet.klever.org");
                    NodeUri = new Uri("https://node.devnet.klever.org");
                    ExplorerUri = new Uri("https://klever-explorer-oxw5p5ia3q-uc.a.run.app/");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(network), network, null);
            }
        }

        public Network Network { get; }
        public Uri APIUri { get; }
        public Uri NodeUri { get; }
        public Uri ExplorerUri { get; }
    }
}
