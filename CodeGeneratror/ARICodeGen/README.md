# Arke.ARI Source Generator

This project contains a source generator for the Asterisk REST Interface (ARI) client. It generates strongly-typed C# classes from Swagger 1.1 API specifications.

## Overview

The source generator reads JSON files containing Swagger 1.1 API specifications and generates:
- Model classes for all API types
- Interface definitions for API actions
- Event handling classes
- Strongly-typed client implementations

## Features

- Full API coverage through code generation
- Async/await support for all operations
- Proper error handling and exceptions
- JSON serialization with proper naming conventions
- Automatic parameter handling and type conversion
- Documentation comments from API specifications

## Usage

1. Add the source generator package to your project:

```xml
<ItemGroup>
    <PackageReference Include="Arke.ARI.SourceGenerator" Version="1.0.0" />
</ItemGroup>
```

2. Create an instance of the API client:

```csharp
var client = new ARIClient(
    baseUrl: "http://your-asterisk-server:8088/ari",
    username: "your-username",
    password: "your-password"
);
```

3. Use the generated interfaces and models:

```csharp
// Example: Working with channels
var channels = await client.Channels.ListAsync();
foreach (var channel in channels)
{
    Console.WriteLine($"Channel ID: {channel.Id}, State: {channel.State}");
}

// Example: Working with bridges
var bridge = await client.Bridges.CreateAsync(
    type: "mixing",
    name: "MyBridge"
);
```

## Generated Code Structure

The source generator creates the following structure:

```
Arke.ARI/
├── Models/
│   ├── Channel.cs
│   ├── Bridge.cs
│   └── ...
├── Actions/
│   ├── IChannelsActions.cs
│   ├── IBridgesActions.cs
│   └── ...
└── Events/
    ├── ChannelStateChangeEvent.cs
    ├── BridgeCreatedEvent.cs
    └── ...
```

## API Coverage

The generator covers all aspects of the ARI API:
- Applications
- Asterisk
- Bridges
- Channels
- DeviceStates
- Endpoints
- Events
- Mailboxes
- Playbacks
- Recordings
- Sounds

## Error Handling

The client throws `ARIException` when API calls fail, containing:
- HTTP status code
- Error message from the server
- Stack trace

## Development

To modify the source generator:

1. Update the JSON files in the `Data` directory
2. Build the project
3. Test the generated code
4. Update version number in .csproj file
5. Create NuGet package

## Migration from T4 Templates

This source generator replaces the previous T4 template-based code generation. Benefits include:
- Better IDE integration
- Faster compilation
- No runtime dependencies
- Better error messages
- Easier to maintain and extend

## License

MIT License - See LICENSE file for details 