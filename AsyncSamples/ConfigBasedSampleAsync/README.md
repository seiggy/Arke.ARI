# ConfigBasedSampleAsync

This sample demonstrates how to use the modernized Arke.ARI client with dependency injection and configuration.

## Features

- Uses the modern ARIClient constructor with `IOptions<ARIClientOptions>` and `IHttpClientFactory`
- Implements proper resource management with disposable patterns
- Shows how to set up dependency injection for the ARI client
- Demonstrates configuration from code and from App.config
- Includes event handling for Stasis events and DTMF processing

## Getting Started

### Prerequisites

- .NET 7.0 or later
- Access to an Asterisk server with ARI enabled

### Configuration

You can configure the application in one of two ways:

1. **Hardcoded configuration** - Edit the values in the `SetupDependencyInjection` method in `Program.cs`
2. **App.config configuration** - Edit the values in `App.config` and use the `ConfigurationBasedProgram.cs` file (rename it to `Program.cs` first)

The following settings can be configured:

- `BaseUrl` - The URL of your Asterisk ARI endpoint (e.g., "http://asterisk-server:8088/ari")
- `Username` - Your ARI username
- `Password` - Your ARI password
- `ApplicationName` - The Stasis application name to connect to

### Building and Running

```bash
cd AsyncSamples/ConfigBasedSampleAsync
dotnet build
dotnet run
```

## How It Works

1. The application sets up dependency injection with the necessary services
2. It configures and registers the ARI client with the dependency injection container
3. It registers event handlers for Stasis events
4. It connects to the Asterisk server and waits for events
5. When a channel enters the Stasis application, it answers the call and plays a greeting
6. It processes DTMF input and plays different sounds based on the digit pressed
7. When the '#' key is pressed, it plays a goodbye message and hangs up

## Code Structure

- `Program.cs` - Main application code with hardcoded configuration
- `ConfigurationBasedProgram.cs` - Alternative implementation using App.config
- `App.config` - Configuration file with ARI settings

## Notes

- This sample uses the modern ARIClient implementation which includes improved error handling, reconnection logic, and resource management
- The WebSocket event producer is configured with the AsyncTaskDispatcher for event handling 