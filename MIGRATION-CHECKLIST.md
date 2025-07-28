# Migration Checklist and Action Items

This document provides a practical checklist for implementing the T4 to Roslyn Source Generator migration.

## Pre-Migration Setup

### Development Environment
- [ ] Install .NET 6 SDK or later
- [ ] Ensure Visual Studio 2022 or JetBrains Rider with Roslyn support
- [ ] Install NuGet packages: `Microsoft.CodeAnalysis.CSharp` and `Microsoft.CodeAnalysis.Analyzers`

### Repository Preparation
- [ ] Create feature branch: `feature/t4-to-roslyn-migration`
- [ ] Back up current generated files in `ARI_1_0/` folder
- [ ] Document current T4 template functionality
- [ ] Identify all dependencies on generated code

## Phase 1: Foundation Setup

### Create Source Generator Project
- [ ] Create new .NET Standard 2.0 class library: `Arke.ARI.SourceGenerator`
- [ ] Add required NuGet packages:
  ```xml
  <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.5.0" PrivateAssets="all" />
  <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.4" PrivateAssets="all" />
  ```
- [ ] Configure project for source generation:
  ```xml
  <PropertyGroup>
    <IncludeBuildOutput>false</IncludeBuildOutput>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
  </PropertyGroup>
  ```

### Copy Schema Files
- [ ] Copy JSON files from `CodeGeneratror/ARICodeGen/Data/` to `Resources/` folder
- [ ] Mark JSON files as embedded resources:
  ```xml
  <ItemGroup>
    <EmbeddedResource Include="Resources\*.json" />
  </ItemGroup>
  ```

### Create Base Infrastructure
- [ ] Implement `ICodeGenerator` interface
- [ ] Create `GeneratedCode` record/class
- [ ] Implement `ApiSchema` and related models
- [ ] Create `TypeConverter` utility class
- [ ] Implement `NamingHelper` for safe C# identifiers

## Phase 2: Model Generation

### Model Generator Implementation
- [ ] Create `ModelGenerator` class implementing `ICodeGenerator`
- [ ] Parse JSON schema models section
- [ ] Generate model classes with properties
- [ ] Handle inheritance relationships (subTypes)
- [ ] Generate XML documentation comments
- [ ] Handle special property name cases (e.g., snake_case to PascalCase)

### Testing Model Generation
- [ ] Create unit tests for `ModelGenerator`
- [ ] Test basic property generation
- [ ] Test inheritance scenarios
- [ ] Test edge cases (empty models, missing descriptions)
- [ ] Validate generated code compiles

## Phase 3: Action Generation

### Action Interface Generator
- [ ] Create `ActionInterfaceGenerator` class
- [ ] Parse API endpoints and operations
- [ ] Generate interface methods with proper signatures
- [ ] Generate both sync and async method signatures
- [ ] Handle optional parameters correctly
- [ ] Generate XML documentation from operation summaries

### Action Class Generator
- [ ] Create `ActionClassGenerator` class
- [ ] Generate class implementing action interface
- [ ] Implement constructor taking `IActionConsumer`
- [ ] Generate method implementations with REST calls
- [ ] Handle different parameter types (path, query, body, header)
- [ ] Generate error handling with specific HTTP status codes
- [ ] Generate both sync and async implementations

### Testing Action Generation
- [ ] Unit tests for interface generation
- [ ] Unit tests for class generation
- [ ] Test parameter handling variations
- [ ] Test error response generation
- [ ] Validate HTTP method mapping

## Phase 4: Event and Client Generation

### Event Generator
- [ ] Create `EventGenerator` class
- [ ] Identify event models (subTypes contain "Event")
- [ ] Generate event classes with proper inheritance
- [ ] Place event classes in separate namespace/folder structure

### Client Generator
- [ ] Create `ClientGenerator` class
- [ ] Generate main `BaseAriClient` class
- [ ] Generate event handler delegates
- [ ] Generate event firing logic with switch statement
- [ ] Generate IAriEventClient interface

### Testing Event/Client Generation
- [ ] Unit tests for event generation
- [ ] Unit tests for client generation
- [ ] Test event handler registration
- [ ] Test event firing mechanism

## Phase 5: Integration

### Source Generator Registration
- [ ] Create main `ARISourceGenerator` class with `[Generator]` attribute
- [ ] Implement `ISourceGenerator` interface
- [ ] Register all sub-generators
- [ ] Add error handling and diagnostics
- [ ] Implement resource loading for JSON schemas

### Main Project Integration
- [ ] Add analyzer reference to `Arke.ARI.csproj`:
  ```xml
  <ItemGroup>
    <Analyzer Include="..\Arke.ARI.SourceGenerator\bin\$(Configuration)\netstandard2.0\Arke.ARI.SourceGenerator.dll" />
  </ItemGroup>
  ```
- [ ] Remove old generated files from compilation
- [ ] Update .gitignore to exclude generated files
- [ ] Test build generates correct files

### Validation
- [ ] Compare generated files with current T4 output
- [ ] Run existing unit tests
- [ ] Run integration tests
- [ ] Performance testing of build process

## Phase 6: Migration and Cleanup

### T4 Removal
- [ ] Remove `CodeGeneratror/ARICodeGen` project from solution
- [ ] Remove T4 template references
- [ ] Delete old generated files from source control
- [ ] Update build scripts and CI/CD pipelines

### Documentation Updates
- [ ] Update README.md with new build instructions
- [ ] Update contributor documentation
- [ ] Update API documentation if needed
- [ ] Create migration notes for developers

### Final Testing
- [ ] Full regression test suite
- [ ] Cross-platform build testing (Windows, macOS, Linux)
- [ ] Performance benchmarking
- [ ] Memory usage validation

## Quality Gates

### Code Quality
- [ ] All unit tests pass (target: 100% for generator code)
- [ ] All integration tests pass
- [ ] Code coverage > 90% for generator logic
- [ ] No compiler warnings in generated code
- [ ] Static analysis checks pass

### Performance Metrics
- [ ] Build time improvement ≥ 20% compared to T4
- [ ] Generator memory usage < 100MB during build
- [ ] Generated code size matches T4 output ±5%
- [ ] No performance regression in runtime behavior

### Compatibility
- [ ] Generated API matches existing API exactly
- [ ] All existing client code continues to work
- [ ] Serialization/deserialization works identically
- [ ] Event handling behavior unchanged

## Rollback Plan

### If Migration Fails
- [ ] Revert to T4 templates
- [ ] Restore `CodeGeneratror/ARICodeGen` project
- [ ] Restore generated files from backup
- [ ] Update CI/CD to use T4 generation
- [ ] Document lessons learned

### Partial Rollback
- [ ] Keep source generator infrastructure
- [ ] Run both T4 and source generator in parallel
- [ ] Compare outputs for discrepancies
- [ ] Gradual migration of individual components

## Success Metrics

### Immediate Success Indicators
- [ ] Solution builds successfully with `dotnet build`
- [ ] All unit tests pass
- [ ] Generated code is functionally identical to T4 output
- [ ] Build time is faster than T4 approach

### Long-term Success Indicators
- [ ] Easier to maintain and extend
- [ ] Better IDE support for generator development
- [ ] Cross-platform development works seamlessly
- [ ] New team members can contribute more easily

## Timeline Checkpoints

### Week 1-2: Foundation
- [ ] Source generator project created and configured
- [ ] Base infrastructure classes implemented
- [ ] JSON schema loading working
- [ ] Type conversion utilities complete

### Week 3: Model Generation
- [ ] Model generator fully implemented
- [ ] Unit tests written and passing
- [ ] Generated models compile and match T4 output

### Week 4: Action Generation
- [ ] Action interface and class generators complete
- [ ] REST API method generation working
- [ ] Error handling properly implemented

### Week 5: Event/Client Generation
- [ ] Event generator complete
- [ ] Client generator complete
- [ ] Event handling working correctly

### Week 6: Integration
- [ ] Full integration with main project
- [ ] All tests passing
- [ ] Performance benchmarks complete

### Week 7: Migration
- [ ] T4 templates removed
- [ ] Documentation updated
- [ ] Final validation complete

## Communication Plan

### Weekly Status Reports
- [ ] Progress against checklist items
- [ ] Blockers and risks identified
- [ ] Performance metrics
- [ ] Next week's goals

### Stakeholder Updates
- [ ] Technical team briefings
- [ ] Demo of generated code comparison
- [ ] Migration timeline updates
- [ ] Post-migration benefits realization

This checklist provides a comprehensive roadmap for successfully migrating from T4 templates to Roslyn Source Generators while minimizing risk and ensuring quality.