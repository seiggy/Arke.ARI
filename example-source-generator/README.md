# Arke.ARI Source Generator

This is an example implementation of a Roslyn Source Generator that replaces the T4 templates used in the Arke.ARI library.

## Purpose

This source generator automatically generates C# code from JSON schema files, eliminating the need for T4 templates and providing better build-time performance and cross-platform compatibility.

## Architecture

- **ARISourceGenerator.cs**: Main generator class with `[Generator]` attribute
- **ICodeGenerator**: Interface for individual code generators
- **Schema Models**: Classes for deserializing JSON schema files
- **Resources/**: Embedded JSON schema files

## Generated Code

The generator produces:
- Model classes from JSON schemas
- Action interfaces and implementations
- Event classes with proper inheritance
- Main ARIClient with event handling

## Usage

This is a demonstration/prototype. In the actual implementation:

1. Add this project as an analyzer reference to the main Arke.ARI project
2. The generator will run at compile time
3. Generated files will be available in the compilation

## Integration

In the main `Arke.ARI.csproj`:

```xml
<ItemGroup>
  <Analyzer Include="..\Arke.ARI.SourceGenerator\bin\$(Configuration)\netstandard2.0\Arke.ARI.SourceGenerator.dll" />
</ItemGroup>
```

## Benefits over T4 Templates

- Compile-time generation (no design-time execution)
- Better IDE support and debugging
- Cross-platform compatibility
- Incremental compilation support
- No Visual Studio dependencies
- Modern C# language features

This example provides the foundation for the complete migration from T4 templates to Roslyn Source Generators.