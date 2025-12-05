# Klever .NET SDK

## Overview

The Klever .NET SDK is a library for integrating with the Klever blockchain, enabling you to create, sign, send, and decode transactions, as well as interact with smart contracts. The project is organized into three main directories:

## Using the Klever .NET SDK in Your Project

### 1. Add the SDK to Your Project

Clone or add the `kleversdk` library to your .NET solution.

### 2. Import the SDK Namespaces

```csharp
using kleversdk.core;
using kleversdk.provider;
```

### 3. Initialize and Use SDK Features

You can use the SDK to interact with the Klever blockchain. For example, to create a wallet, query an account, and send a transaction:

```csharp
// Initialize provider for TestNet
var kp = new KleverProvider(new NetworkConfig(Network.TestNet));

// Create wallet from mnemonic
var mnemonic = "word1 word2 ...";
var wallet = Wallet.DeriveFromMnemonic(mnemonic);
var acc = wallet.GetAccount();

// Sync account data
await acc.Sync(kp);

// Send a transaction
var tx = await kp.Send(acc.Address.Bech32, acc.Nonce, "recipient_address", 100);
var decoded = await kp.Decode(tx);
var signature = wallet.SignHex(decoded.Hash);
tx.AddSignature(signature);
var broadcastResult = await kp.Broadcast(tx);
Console.WriteLine($"Broadcast result: {broadcastResult.String()}");
```

---

## Examples

The [`demo`](demo/) directory provides runnable examples that demonstrate how to use the Klever .NET SDK for different blockchain operations. Here are some of the included examples:

---

## Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details on:

- Setting up your development environment
- Submitting issues and pull requests
- Code style guidelines

## Code Style

This project follows [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions). See [CODE_STYLE.md](CODE_STYLE.md) for project-specific guidelines.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
