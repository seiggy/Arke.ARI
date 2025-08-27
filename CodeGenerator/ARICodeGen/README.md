# Arke.ARI Source Generator

This package provides a Roslyn Incremental Generator that generates an Asterisk ARI (Asterisk REST Interface) client based on API specifications.

## Features

- Generates C# models from Asterisk ARI JSON schemas
- Generates interfaces and implementations for all ARI actions
- Generates WebSocket client for event handling
- Supports both embedded schemas and remote endpoint schemas
- Includes authentication support for remote endpoints

## Installation

Add the package to your project:

```bash
dotnet add package Arke.ARI
```

## Usage

### Option 1: Using Embedded Schemas

By default, the generator uses embedded schemas from the `Data` folder. No additional configuration is needed.

### Option 2: Using Remote Endpoint

You can configure the generator to use a remote endpoint in two ways:

#### Method 1: Using MSBuild Properties

Add the following properties to your project file:

```xml
<PropertyGroup>
  <AriEndpoint>http://your-asterisk-server:8088/ari/api-docs/resources.json</AriEndpoint>
  <AriUsername>your-username</AriUsername>
  <AriPassword>your-password</AriPassword>
</PropertyGroup>
```

#### Method 2: Using Configuration File

Create or modify the `asterisk-config.json` file in your project:

```json
{
  "schemaSource": {
    "type": "endpoint",
    "endpoint": "http://your-asterisk-server:8088/ari/api-docs/resources.json",
    "username": "your-username",
    "password": "your-password"
  },
  "resources": {
    "Applications": {
      "sourcePath": "applications.json",
      "enabled": true
    },
    "Asterisk": {
      "sourcePath": "asterisk.json",
      "enabled": true
    },
    "Bridges": {
      "sourcePath": "bridges.json",
      "enabled": true
    },
    "Channels": {
      "sourcePath": "channels.json",
      "enabled": true
    },
    "DeviceStates": {
      "sourcePath": "deviceStates.json",
      "enabled": true
    },
    "Endpoints": {
      "sourcePath": "endpoints.json",
      "enabled": true
    },
    "Events": {
      "sourcePath": "events.json",
      "enabled": true
    },
    "Playbacks": {
      "sourcePath": "playbacks.json",
      "enabled": true
    },
    "Recordings": {
      "sourcePath": "recordings.json",
      "enabled": true
    },
    "Sounds": {
      "sourcePath": "sounds.json",
      "enabled": true
    },
    "Mailboxes": {
      "sourcePath": "mailboxes.json",
      "enabled": true
    }
  },
  "codeGeneration": {
    "namespace": "Arke.ARI",
    "modelsNamespace": "Arke.ARI.Models",
    "actionsNamespace": "Arke.ARI.Actions",
    "generateNullableAnnotations": true,
    "generateAsyncMethods": true,
    "generateXmlDocumentation": true
  }
}
```

Make sure to add this file as an AdditionalFile in your project:

```xml
<ItemGroup>
  <AdditionalFiles Include="asterisk-config.json" />
</ItemGroup>
```

## Using the Generated Client

Once the client is generated, you can use it in your code:

```csharp
// Create the client
var client = new Arke.ARI.ARIClient("http://your-asterisk-server:8088", "your-username", "your-password");

// Use the actions
var channels = await client.Channels.ListAsync();
var bridges = await client.Bridges.ListAsync();

// Connect to WebSocket for events
await client.ConnectAsync("your-app-name");

// Register event handlers
client.OnEvent<BridgeCreatedEvent>(async evt => 
{
    Console.WriteLine($"Bridge created: {evt.Bridge.Id}");
});

// Or use the generic event handler
client.OnEvent<Event>(async evt => 
{
    Console.WriteLine($"Received event: {evt.GetType().Name}");
});
```

## Configuration Options

### Schema Source Configuration

- `type`: Type of schema source (`embedded` or `endpoint`)
- `endpoint`: URL of the remote endpoint (when type is `endpoint`)
- `username`: Username for authentication (when type is `endpoint`)
- `password`: Password for authentication (when type is `endpoint`)

### Resource Configuration

- `sourcePath`: Path to the resource schema (relative to the endpoint or embedded resource)
- `enabled`: Whether to generate code for this resource

### Code Generation Configuration

- `namespace`: Root namespace for generated code
- `modelsNamespace`: Namespace for generated models
- `actionsNamespace`: Namespace for generated actions
- `generateNullableAnnotations`: Whether to generate nullable annotations
- `generateAsyncMethods`: Whether to generate async methods
- `generateXmlDocumentation`: Whether to generate XML documentation

## License

This project is licensed under the MIT License - see the LICENSE file for details. 
