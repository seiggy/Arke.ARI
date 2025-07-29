# Arke.ARI

Asterisk ARI (Asterisk REST Interface) API wrapper for .NET - forked from AsterNET.ARI

## Overview

Arke.ARI is a comprehensive .NET library for interacting with Asterisk's ARI (Asterisk REST Interface). It provides a strongly-typed, async-first API for building Asterisk applications in C#.

## Features

- **Full ARI Coverage**: Complete implementation of all ARI endpoints
- **Async/Await Support**: Modern async programming patterns
- **Strongly Typed**: Full IntelliSense support and compile-time safety
- **Event-Driven**: Real-time event handling via WebSocket connections
- **Dependency Injection**: Built-in support for .NET DI container
- **Comprehensive Testing**: Unit and integration test suite
- **Code Generation**: Roslyn-based code generator for ARI actions and models

## Requirements

- .NET 8.0 or later
- Asterisk 20+ with ARI enabled
- Visual Studio 2022 or VS Code

## Installation

### NuGet Package

```bash
dotnet add package Arke.ARI
```

### From Source

```bash
git clone https://github.com/seiggy/arke.ari.git
cd arke.ari
dotnet build
```

## Quick Start

### Basic Usage

```csharp
using Arke.ARI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLogging();
builder.Services.AddHttpClient();

using var host = builder.Build();
var services = host.Services;

// Create ARI client
var client = new AriClient(
    new StasisEndpoint("192.168.1.165", 8088, "asterisk", "asterisk"),
    services,
    "my-app");

// Connect to Asterisk
await client.Connect();

// Handle events
client.OnStasisStartEvent += async (sender, e) =>
{
    await client.Channels.AnswerAsync(e.Channel.Id);
    await client.Channels.PlayAsync(e.Channel.Id, "sound:hello-world");
};

// Keep the application running
await host.RunAsync();
```

### Event Handling

```csharp
// Stasis start event
client.OnStasisStartEvent += async (sender, e) =>
{
    Console.WriteLine($"Channel {e.Channel.Id} entered the application");
    await client.Channels.AnswerAsync(e.Channel.Id);
};

// DTMF events
client.OnChannelDtmfReceivedEvent += async (sender, e) =>
{
    Console.WriteLine($"Received DTMF: {e.Digit}");
    await client.Channels.PlayAsync(e.Channel.Id, $"sound:digits/{e.Digit}");
};

// Connection state changes
client.OnConnectionStateChanged += async (sender) =>
{
    Console.WriteLine($"Connection state: {client.Connected}");
};
```

### Working with Bridges

```csharp
// Create a bridge
var bridge = await client.Bridges.CreateAsync("mixing", "my-bridge", "my-app");

// Add a channel to the bridge
await client.Bridges.AddChannelAsync(bridge.Id, channelId, "member");

// Start MOH on the bridge
await client.Bridges.StartMohAsync(bridge.Id, "default");
```

### Recording

```csharp
// Start recording
var recording = await client.Channels.RecordAsync(
    channelId: channelId,
    name: "my-recording",
    format: "wav",
    maxDurationSeconds: 30,
    maxSilenceSeconds: 5,
    ifExists: "overwrite",
    beep: true,
    terminateOn: "#");

// Stop recording
await client.Recordings.StopAsync(recording.Id);
```

## Configuration

### appsettings.json

```json
{
  "ARI": {
    "Host": "192.168.1.165",
    "Port": 8088,
    "Username": "asterisk",
    "Password": "asterisk",
    "Application": "my-app"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Arke.ARI": "Debug"
    }
  }
}
```

## Samples

The project includes several sample applications demonstrating different ARI features:

### SimpleTestApplicationAsync
Basic ARI application with DTMF handling and sound playback.

### SimpleBridgeAsync
Demonstrates bridge creation, channel management, and MOH (Music on Hold).

### SimpleRecordAndPlaybackAsync
Shows recording functionality with automatic playback.

### SimpleConfAsync
Conference management with REST API for web interface.

### ConfigBasedSampleAsync
Configuration-driven ARI application with dependency injection.

### SimpleConferenceWeb
Web-based conference management using ASP.NET Core.

## Testing

The project includes a comprehensive test suite:

### Unit Tests
- AriClientTests: Core client functionality
- CodeGeneratorTests: Roslyn code generator validation

### Integration Tests
- AriIntegrationTests: Real Asterisk environment testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test Arke.ARI.Tests/Arke.ARI.Tests.csproj

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Development

### Building

```bash
dotnet build
```

### Testing

```bash
dotnet test
```

### Code Generation

The project uses a Roslyn-based code generator to create ARI action classes and models. The generator:

- Reads Asterisk ARI specifications
- Generates strongly-typed C# classes
- Creates async method signatures
- Implements proper error handling

### Project Structure

```
Arke.ARI/
├── Arke.ARI/                    # Main library
│   ├── Actions/                 # Generated ARI actions
│   ├── Events/                  # Generated event classes
│   ├── Models/                  # Generated model classes
│   ├── Middleware/              # HTTP and WebSocket middleware
│   └── Dispatchers/             # Event dispatching strategies
├── AsyncSamples/                # Sample applications
├── Arke.ARI.Tests/              # Test suite
└── CodeGeneratror/              # Roslyn code generator
```

## Asterisk Configuration

### Enable ARI

In `asterisk.conf`:

```ini
[ari]
enabled = yes
pretty = yes
```

### Create ARI User

In `ari.conf`:

```ini
[asterisk]
type = user
read_only = no
password = asterisk
```

### Dialplan Integration

```asterisk
exten => 1000,1,NoOp()
same => n,Stasis(my-app)
same => n,Hangup()
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE.txt file for details.

## Acknowledgments

- Original AsterNET.ARI project
- Asterisk development team
- .NET community contributors

## Version History

### v3.0.0
- Migrated to .NET 8
- Refactored to use HttpClient instead of RestSharp
- Added comprehensive test suite
- Improved dependency injection support
- Enhanced logging and error handling
- Updated all samples to use new Asterisk environment

### v2.0.0
- Migrated to .NET Standard 2.0
- Upgraded to Asterisk 20
- Improved async/await patterns
- Enhanced event handling

## Support

For issues and questions:
- GitHub Issues: https://github.com/seiggy/arke.ari/issues
- Documentation: See samples and test projects
- Asterisk Documentation: https://wiki.asterisk.org/wiki/display/AST/Asterisk+REST+Interface
