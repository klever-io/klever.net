# Contributing to klever.net SDK

Thank you for your interest in contributing to the Klever .NET SDK! This document provides guidelines and instructions for contributing.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [How to Contribute](#how-to-contribute)
- [Pull Request Process](#pull-request-process)
- [Code Style](#code-style)
- [Testing](#testing)

## Code of Conduct

Please be respectful and constructive in all interactions. We are committed to providing a welcoming and inclusive environment for everyone.

## Getting Started

1. Fork the repository
2. Clone your fork locally
3. Set up the development environment (see below)
4. Create a branch for your changes
5. Make your changes and test them
6. Submit a pull request

## Development Setup

### Prerequisites

- [.NET SDK 6.0+](https://dotnet.microsoft.com/download)
- Visual Studio 2022, VS Code, or JetBrains Rider

### Building the Project

```bash
# Clone the repository
git clone https://github.com/klever-io/klever.net.git
cd klever.net

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test
```

### Project Structure

```
klever.net/
├── kleversdk/           # Main SDK library
│   ├── core/            # Core functionality (wallet, crypto)
│   └── provider/        # Network provider and DTOs
├── kleversdk.Tests/     # Unit tests
├── demo/                # Demo application
└── docs/                # Documentation
```

## How to Contribute

### Reporting Issues

- Check if the issue already exists
- Use a clear and descriptive title
- Provide detailed reproduction steps
- Include .NET version and OS information
- Add relevant code samples or error messages

### Suggesting Features

- Open an issue with the `enhancement` label
- Describe the use case and expected behavior
- Explain why this feature would be useful

### Contributing Code

1. **Pick an issue** - Look for issues labeled `good first issue` or `help wanted`
2. **Discuss** - Comment on the issue to let others know you're working on it
3. **Branch** - Create a branch from `master` using the naming convention:
   - `feature/description` - for new features
   - `bugfix/description` - for bug fixes
   - `chore/description` - for maintenance tasks
4. **Code** - Implement your changes following our [code style](CODE_STYLE.md)
5. **Test** - Add or update tests as needed
6. **Submit** - Open a pull request

## Pull Request Process

1. **Title Format**: Use a clear, descriptive title
   - For Jira issues: `[KLC-XXX] Brief description`
   - Otherwise: `type: Brief description` (e.g., `fix: Correct amount conversion`)

2. **Description**: Include:
   - Summary of changes
   - Related issue number(s)
   - Testing performed
   - Breaking changes (if any)

3. **Checklist**:
   - [ ] Code follows the project style guidelines
   - [ ] Tests pass locally (`dotnet test`)
   - [ ] New code has appropriate test coverage
   - [ ] Documentation updated (if needed)

4. **Review**: Address any feedback from reviewers

5. **Merge**: Once approved, a maintainer will merge your PR

## Code Style

We follow [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions). Key points:

- **Indentation**: 4 spaces (no tabs)
- **Braces**: New line for opening braces
- **Naming**: PascalCase for public members, camelCase for private fields
- **Line endings**: LF (Unix-style)

See [CODE_STYLE.md](CODE_STYLE.md) for detailed guidelines.

The repository includes an `.editorconfig` file that most IDEs will automatically respect.

## Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "FullyQualifiedName~ABITests"
```

### Writing Tests

- Use xUnit framework
- Place tests in `kleversdk.Tests/`
- Follow the naming convention: `MethodName_Scenario_ExpectedResult`
- Include both positive and negative test cases

### Test Coverage

We aim to maintain and improve test coverage. When adding new features:
- Add unit tests for new public methods
- Include edge cases and error conditions
- Consider integration tests for network-dependent code

## Questions?

If you have questions, feel free to:
- Open an issue with the `question` label
- Check existing documentation and issues

Thank you for contributing!
