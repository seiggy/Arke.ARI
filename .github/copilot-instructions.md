# Arke.ARI Development Instructions

Arke.ARI is a .NET Standard 2.0 library that provides a C# wrapper for Asterisk ARI (Application Resource Interface). It allows developers to create telephony applications that interact with Asterisk servers using modern .NET platforms.

**Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively

### Initial Setup and Build
- **CRITICAL**: All builds work correctly on Linux/Ubuntu with .NET 8 SDK
- Bootstrap and build the repository:
  - `dotnet restore` -- takes 2 seconds with package warnings (RestSharp vulnerability, .NET 7 EOL). NEVER CANCEL.
  - `dotnet build --configuration Release --no-restore` -- takes 2 seconds with warnings. NEVER CANCEL.
- Test the repository:
  - `dotnet test --no-restore --verbosity normal` -- takes 1 second. No unit tests exist in this repository.
- **Build Timeouts**: Use 60+ seconds timeout for restore, 30+ seconds for build commands

### Key Projects Structure
- **Main Library**: `Arke.ARI/Arke.ARI.csproj` (netstandard2.0) - Core ARI wrapper library
- **Samples**: `Samples/` - Synchronous example applications (net7.0)
- **AsyncSamples**: `AsyncSamples/` - Asynchronous example applications (net7.0)  
- **CodeGenerator**: `CodeGeneratror/ARICodeGen/` - **WINDOWS ONLY** (.NET Framework 4.8)

### Sample Applications (All require Asterisk ARI server to run)
- `SimpleTestApplication` - Basic channel handling with DTMF
- `SimpleBridge` - Bridge creation and channel management
- `SimpleConf` - Conference application example
- `SimpleRecordAndPlayback` - Recording and playback functionality
- `RecordingSample` - Advanced recording features
- All samples have async variants in `AsyncSamples/`

## Validation

### Build Validation
- Always run the full build sequence to validate changes:
  - `dotnet restore` -- NEVER CANCEL, expect package warnings
  - `dotnet build --configuration Release --no-restore` -- NEVER CANCEL
  - Build warnings are expected (RestSharp vulnerability, .NET 7 EOL) and do not indicate failure
- **Expected Build Time**: 2 seconds total for full solution build

### Sample Application Validation  
- Build individual samples: `cd Samples/SimpleTestApplication && dotnet build`
- **IMPORTANT**: Samples cannot be runtime tested without an Asterisk ARI server
- Samples will fail at runtime with connection errors - this is expected behavior
- Validate sample builds succeed without errors

### Code Generation Limitations
- **CodeGenerator project only works on Windows** (.NET Framework 4.8 requirement)
- Do not attempt to build `CodeGeneratror/ARICodeGen/ARICodeGen.csproj` on Linux
- Generated code is already present in `Arke.ARI/ARI_1_0/` directory
- Code generation is used to update ARI definitions from Asterisk swagger specs

## Common Development Tasks

### Building the Library
```bash
cd /path/to/Arke.ARI
dotnet restore                                          # 2 seconds, NEVER CANCEL
dotnet build --configuration Release --no-restore      # 2 seconds, NEVER CANCEL
```

### Building Sample Applications
```bash
cd Samples/SimpleTestApplication
dotnet build                                           # 2 seconds
```

### Running Sample Applications (Testing Only)
```bash
cd Samples/SimpleTestApplication  
dotnet run                                             # Will fail with connection error (expected)
```

### Creating New Applications
- Reference the main library: `<ProjectReference Include="..\..\Arke.ARI\Arke.ARI.csproj" />`
- Use `StasisEndpoint` for connection configuration
- Implement event handlers for `OnStasisStartEvent`, `OnChannelDtmfReceivedEvent`, etc.
- Choose synchronous or asynchronous patterns based on `Samples/` or `AsyncSamples/`

## Key API Patterns

### Basic Connection Pattern
```csharp
var client = new AriClient(
    new StasisEndpoint("127.0.0.1", 8088, "username", "password"),
    "ApplicationName");
client.OnStasisStartEvent += OnStasisStart;
client.Connect();
```

### Synchronous vs Asynchronous
- **Synchronous**: `client.Channels.Answer(channelId)`  
- **Asynchronous**: `await client.Channels.AnswerAsync(channelId)`

## Known Issues and Limitations

### Expected Warnings
- RestSharp package vulnerability warning (non-blocking)
- .NET 7 EOL warnings for sample projects (non-blocking)
- Compiler warnings in `MailboxesActions.cs` (generated code, non-blocking)

### Platform Limitations  
- Code generation only works on Windows (.NET Framework 4.8)
- Sample applications require Asterisk ARI server for runtime testing
- No unit test infrastructure exists in this repository

### Dependencies
- **Newtonsoft.Json** (13.0.3) - JSON serialization
- **RestSharp** (110.2.0) - HTTP REST client (has known vulnerability warning)
- **WebSocket4Net** (0.15.2) - WebSocket communication
- **System.Threading.Thread/ThreadPool** - Threading support

## Validation Scenarios

### After Making Changes
1. **Always run full build validation**:
   ```bash
   dotnet restore                                       # NEVER CANCEL, 60+ second timeout
   dotnet build --configuration Release --no-restore   # NEVER CANCEL, 30+ second timeout  
   ```

2. **Build sample applications to verify API compatibility**:
   ```bash
   cd Samples/SimpleTestApplication && dotnet build
   cd ../../AsyncSamples/SimpleTestApplicationAsync && dotnet build
   ```

3. **Verify NuGet package generation** (main library):
   - Check for successful package creation in build output
   - Package files created in `Arke.ARI/bin/Debug/` or `Arke.ARI/bin/Release/`

### Manual Testing Scenarios
- **Cannot perform runtime testing** without Asterisk ARI server setup
- Build validation is the primary verification method
- Sample compilation confirms API surface compatibility

## Solution Structure Reference

### Root Directory Contents
```
Arke.ARI.sln                    # Main solution file
Arke.ARI/                       # Main library project
Samples/                        # Synchronous examples
AsyncSamples/                   # Asynchronous examples  
CodeGeneratror/                 # Windows-only code generation
RecordingSample/                # Recording functionality example
README.md                       # Project documentation
changelog.txt                   # Change history
```

### Common File Locations
- **Main library source**: `Arke.ARI/`
- **Generated ARI models**: `Arke.ARI/ARI_1_0/Models/`
- **Generated ARI actions**: `Arke.ARI/ARI_1_0/Actions/`
- **Connection logic**: `Arke.ARI/ARIClient.cs`, `Arke.ARI/StasisEndpoint.cs`
- **Sample applications**: `Samples/` and `AsyncSamples/`