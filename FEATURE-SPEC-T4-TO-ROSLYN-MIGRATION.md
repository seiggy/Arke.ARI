# Feature Specification: Migration from T4 Templates to Roslyn Source Generators

## Document Information
- **Document Version**: 1.0
- **Date**: December 2024
- **Author**: Generated Feature Specification
- **Project**: Arke.ARI - Asterisk REST Interface Library

## Executive Summary

This feature specification outlines the migration of the Arke.ARI library's code generation system from T4 (Text Template Transformation Toolkit) templates to Roslyn Source Generators. This migration will modernize the build process, improve performance, eliminate external dependencies, and provide better IDE integration while maintaining full backward compatibility.

## Current State Analysis

### Existing T4 Template System

The current code generation system consists of:

1. **ARICodeGen Project** (`CodeGeneratror/ARICodeGen/`)
   - .NET Framework 4.8 console application
   - Contains T4 template `Templates/Models.tt` (733 lines)
   - Processes JSON schema files from `Data/` folder
   - Generates C# classes in the main Arke.ARI project

2. **JSON Schema Files** (`Data/` folder)
   - 11 JSON files containing Swagger-like API definitions:
     - Applications.json, Asterisk.json, Bridges.json, Channels.json
     - DeviceStates.json, Endpoints.json, Events.json, Mailboxes.json
     - Playbacks.json, Recordings.json, Sounds.json

3. **Generated Code Structure**
   - **Models**: Data transfer objects (e.g., `Application`, `Bridge`, `Channel`)
   - **Actions**: Interface and implementation classes (e.g., `IApplicationsActions`, `ApplicationsActions`)
   - **Events**: Event classes inheriting from base `Event` class
   - **ARIClient**: Main client class with event handling

4. **Code Generation Features**
   - Generates both synchronous and asynchronous method variants
   - Creates strongly-typed C# classes from JSON schemas
   - Handles inheritance relationships (subTypes)
   - Generates proper XML documentation
   - Creates REST API client methods with proper HTTP verb mapping
   - Exception handling with specific error codes

### Current Limitations

1. **Build Dependencies**: Requires Visual Studio T4 engine and .NET Framework
2. **Performance**: T4 templates execute at design-time, slowing development
3. **IDE Integration**: Limited IntelliSense and debugging support in T4 templates
4. **Maintenance**: Complex T4 syntax makes the template difficult to maintain
5. **Cross-Platform**: T4 has limited support in non-Windows environments
6. **Framework Dependency**: ARICodeGen project stuck on .NET Framework 4.8

## Migration Goals

### Primary Objectives

1. **Modernization**: Replace T4 templates with modern Roslyn Source Generators
2. **Performance**: Improve build-time performance through compile-time generation
3. **Cross-Platform**: Enable full cross-platform development and CI/CD
4. **Maintainability**: Simplify code generation logic with modern C# features
5. **IDE Integration**: Provide better debugging and IntelliSense support
6. **Zero Breaking Changes**: Maintain 100% API compatibility

### Secondary Objectives

1. **Framework Alignment**: Migrate to .NET Standard 2.0/.NET 6+ for code generation
2. **Dependency Reduction**: Eliminate T4 and Visual Studio dependencies
3. **Build Simplification**: Single `dotnet build` command for complete solution
4. **Testing**: Add unit tests for code generation logic
5. **Documentation**: Improve generated code documentation

## Technical Approach

### Roslyn Source Generator Architecture

```
Arke.ARI.SourceGenerator/
├── Generators/
│   ├── ModelGenerator.cs           # Generates model classes
│   ├── ActionInterfaceGenerator.cs # Generates action interfaces  
│   ├── ActionClassGenerator.cs     # Generates action implementations
│   ├── EventGenerator.cs           # Generates event classes
│   └── ClientGenerator.cs          # Generates main client class
├── Schema/
│   ├── SwaggerSchema.cs           # JSON schema models
│   ├── ApiDefinition.cs           # API definition models
│   └── SchemaParser.cs            # JSON parsing logic
├── Utilities/
│   ├── TypeConverter.cs           # Type conversion utilities
│   ├── CodeBuilder.cs             # Code generation helpers
│   └── NamingHelper.cs            # Naming convention utilities
└── Resources/
    ├── applications.json          # Embedded resource files
    ├── asterisk.json
    └── ...
```

### Source Generator Implementation

#### 1. Generator Registration
```csharp
[Generator]
public class ARISourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register syntax receivers
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // Generate all required source files
    }
}
```

#### 2. Schema Processing
- Embed JSON schema files as resources in the generator assembly
- Parse JSON schemas into strongly-typed C# models
- Validate schema consistency and dependencies

#### 3. Code Generation Strategy
- **Model Generation**: Create POCOs with proper property types and attributes
- **Action Generation**: Generate interface/implementation pairs for REST operations
- **Event Generation**: Create event classes with proper inheritance hierarchy
- **Client Generation**: Generate main client with event handling and dependency injection

#### 4. Template System Replacement
Replace T4 template logic with:
- StringBuilder-based code generation
- Roslyn SyntaxFactory for complex scenarios
- Template method pattern for reusable code blocks

### Migration Benefits

#### Performance Improvements
- **Compile-time generation**: No runtime template processing
- **Incremental compilation**: Only regenerate when schemas change
- **Faster builds**: Eliminate T4 preprocessing overhead

#### Development Experience
- **Better debugging**: Full C# debugging in generator code
- **IntelliSense support**: Rich IDE experience for generator development
- **Cross-platform**: Works on Windows, macOS, and Linux
- **Modern tooling**: Leverage latest C# language features

#### Maintenance Benefits
- **Simplified codebase**: Remove T4 complexity
- **Unit testable**: Test generation logic independently
- **Version control friendly**: Generated code lives in output folder
- **Documentation**: Auto-generated XML documentation

## Implementation Plan

### Phase 1: Foundation (Week 1-2)
- [ ] Create `Arke.ARI.SourceGenerator` project
- [ ] Set up basic source generator infrastructure
- [ ] Implement JSON schema parsing
- [ ] Create type conversion utilities
- [ ] Port `SwaggerHelper` functionality

### Phase 2: Model Generation (Week 3)
- [ ] Implement model class generation
- [ ] Handle inheritance relationships (subTypes)
- [ ] Generate proper XML documentation
- [ ] Add unit tests for model generation

### Phase 3: Action Generation (Week 4)
- [ ] Implement action interface generation
- [ ] Implement action class generation
- [ ] Handle synchronous and asynchronous variants
- [ ] Add proper error handling and HTTP status codes

### Phase 4: Event and Client Generation (Week 5)
- [ ] Implement event class generation
- [ ] Generate main ARIClient class
- [ ] Implement event handler registration
- [ ] Add client factory and dependency injection support

### Phase 5: Integration and Testing (Week 6)
- [ ] Integrate source generator into main project
- [ ] Run comprehensive compatibility tests
- [ ] Performance benchmarking
- [ ] Update build scripts and CI/CD

### Phase 6: Migration and Cleanup (Week 7)
- [ ] Remove T4 template dependencies
- [ ] Delete ARICodeGen project
- [ ] Update documentation
- [ ] Final testing and validation

## Testing Strategy

### Unit Testing
- **Schema Parsing**: Validate JSON schema processing
- **Type Conversion**: Test all type mapping scenarios  
- **Code Generation**: Verify generated code correctness
- **Edge Cases**: Handle malformed schemas gracefully

### Integration Testing
- **API Compatibility**: Ensure generated APIs match current behavior
- **Serialization**: Test JSON serialization/deserialization
- **Event Handling**: Validate event subscription and firing
- **Error Scenarios**: Test exception handling

### Performance Testing
- **Build Time**: Compare T4 vs Source Generator build times
- **Memory Usage**: Monitor generator memory consumption
- **Incremental Builds**: Verify incremental compilation works

### Compatibility Testing
- **Framework Versions**: Test on .NET Standard 2.0, .NET 6, .NET 8
- **Platforms**: Validate Windows, macOS, Linux compatibility
- **IDE Support**: Test Visual Studio, VS Code, JetBrains Rider

## Migration Timeline

| Phase | Duration | Deliverables | Dependencies |
|-------|----------|-------------|--------------|
| Foundation | 2 weeks | Base generator infrastructure | None |
| Model Generation | 1 week | Model class generation | Foundation |
| Action Generation | 1 week | Action interface/class generation | Foundation |
| Event/Client | 1 week | Event classes and main client | Foundation |
| Integration | 1 week | Full integration and testing | All previous phases |
| Migration | 1 week | T4 removal and cleanup | Integration complete |

**Total Duration**: 7 weeks

## Risk Assessment

### Technical Risks
1. **Compatibility**: Risk of breaking existing APIs
   - **Mitigation**: Comprehensive automated testing
2. **Performance**: Source generator performance concerns
   - **Mitigation**: Performance benchmarking and optimization
3. **Complexity**: Understanding Roslyn Source Generator APIs
   - **Mitigation**: Prototype development and documentation review

### Project Risks
1. **Timeline**: Underestimating migration complexity
   - **Mitigation**: Phased approach with regular checkpoints
2. **Resources**: Developer availability and expertise
   - **Mitigation**: Knowledge sharing and documentation

## Success Criteria

### Functional Requirements
- [ ] All existing APIs remain unchanged
- [ ] Generated code is functionally identical
- [ ] No breaking changes for library consumers
- [ ] Full event handling compatibility

### Non-Functional Requirements
- [ ] Build time improvement ≥ 20%
- [ ] Cross-platform build support
- [ ] Simplified development setup
- [ ] Improved maintainability score

### Quality Gates
- [ ] 100% unit test coverage for generators
- [ ] All integration tests pass
- [ ] Performance benchmarks meet targets
- [ ] Code review approval

## Post-Migration Benefits

### Immediate Benefits
- Faster build times and improved developer experience
- Cross-platform development support
- Simplified CI/CD pipeline
- Reduced external dependencies

### Long-term Benefits
- Easier maintenance and feature additions
- Better IDE integration and debugging
- Future-proof architecture
- Enhanced testability

## Conclusion

The migration from T4 templates to Roslyn Source Generators represents a significant modernization of the Arke.ARI library's code generation infrastructure. This change will improve developer productivity, reduce build complexity, and position the library for future growth while maintaining complete backward compatibility.

The phased approach ensures minimal risk while delivering incremental value throughout the migration process. The expected timeline of 7 weeks is reasonable given the scope of changes and the comprehensive testing strategy.

This migration aligns with modern .NET development practices and will significantly improve the maintainability and cross-platform compatibility of the Arke.ARI library.