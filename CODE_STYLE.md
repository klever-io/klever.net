# Code Style Guide

This project follows [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions) with some project-specific guidelines.

## Formatting

### Indentation and Spacing

- Use **4 spaces** for indentation (no tabs)
- Use **LF** line endings (Unix-style)
- Include a **final newline** at the end of files
- Remove trailing whitespace

These settings are enforced via `.editorconfig`.

### Braces

Place opening braces on a new line (Allman style):

```csharp
// Correct
public void Method()
{
    if (condition)
    {
        DoSomething();
    }
}

// Incorrect
public void Method() {
    if (condition) {
        DoSomething();
    }
}
```

### Line Length

- Aim for lines under 120 characters
- Break long method chains or parameter lists

## Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Namespace | PascalCase | `kleversdk.provider` |
| Class | PascalCase | `KleverProvider` |
| Interface | IPascalCase | `IKleverProvider` |
| Method | PascalCase | `GetAccount()` |
| Property | PascalCase | `Balance` |
| Public field | PascalCase | `MaxRetries` |
| Private field | _camelCase | `_httpClient` |
| Parameter | camelCase | `accountAddress` |
| Local variable | camelCase | `transactionResult` |
| Constant | PascalCase | `DefaultTimeout` |

## Code Organization

### Using Statements

- Place `using` statements at the top of the file
- Sort `System` namespaces first
- Group related namespaces together

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using kleversdk.core;
using kleversdk.provider.Dto;
```

### Class Structure

Organize class members in this order:

1. Constants
2. Private fields
3. Constructors
4. Public properties
5. Public methods
6. Private methods

## Documentation

### XML Comments

Add XML documentation for public APIs:

```csharp
/// <summary>
/// Retrieves account information from the blockchain.
/// </summary>
/// <param name="address">The bech32 encoded address.</param>
/// <returns>Account details including balance and nonce.</returns>
/// <exception cref="ApiException">Thrown when the API request fails.</exception>
public async Task<AccountDto> GetAccount(string address)
```

### Inline Comments

- Use comments to explain "why", not "what"
- Keep comments up to date with code changes
- Avoid obvious comments

## Best Practices

### Async/Await

- Use `async`/`await` for I/O operations
- Suffix async methods with `Async` (optional but recommended for new code)
- Avoid `.Result` or `.Wait()` on tasks

```csharp
// Correct
var account = await provider.GetAccount(address);

// Avoid
var account = provider.GetAccount(address).Result;
```

### Null Handling

- Use null-conditional operators where appropriate
- Validate parameters in public methods

```csharp
public void Process(string input)
{
    if (string.IsNullOrEmpty(input))
        throw new ArgumentException("Input cannot be null or empty", nameof(input));
}
```

### Error Handling

- Use specific exception types
- Include meaningful error messages
- Don't catch exceptions you can't handle

## IDE Configuration

### Visual Studio / VS Code

The `.editorconfig` file will automatically configure your IDE. Ensure your editor respects EditorConfig files.

### JetBrains Rider

Rider supports `.editorconfig` out of the box. Enable "Format on Save" for automatic formatting.

## References

- [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [.NET Naming Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines)
- [EditorConfig](https://editorconfig.org/)
