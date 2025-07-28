# Technical Implementation Guide: T4 to Roslyn Migration

## Implementation Examples

### Current T4 Template Analysis

The existing T4 template (`Templates/Models.tt`) performs several key operations:

1. **JSON Schema Parsing**
```csharp
// Current T4 approach
var o = (Newtonsoft.Json.Linq.JObject)JToken.ReadFrom(new JsonTextReader(reader));
var models = o["models"];
```

2. **Type Conversion**
```csharp
// Current SwaggerHelper.TypeConvert method
public static string TypeConvert(string inputType)
{
    if (inputType.Contains("["))
        return inputType.Replace("[", "<").Replace("]", ">");
    if (inputType.ToLower() == "date")
        return "DateTime";
    if (inputType.ToLower() == "boolean")
        return "bool";
    // ... more conversions
}
```

3. **Code Generation Pattern**
```csharp
// T4 template generates classes like:
public class <#= className #> <#= !string.IsNullOrEmpty(inherit) ? string.Format(" : {0}", inherit) : "" #>
{
    /// <summary>
    /// <#= description #>
    /// </summary>
    public <#= propertyType #> <#= propertyName #> { get; set; }
}
```

### Proposed Roslyn Source Generator Implementation

#### 1. Generator Entry Point

```csharp
[Generator]
public class ARISourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization needed for this generator
    }

    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            var schemas = LoadEmbeddedSchemas();
            var generators = new List<ICodeGenerator>
            {
                new ModelGenerator(),
                new ActionInterfaceGenerator(),
                new ActionClassGenerator(),
                new EventGenerator(),
                new ClientGenerator()
            };

            foreach (var schema in schemas)
            {
                foreach (var generator in generators)
                {
                    var sourceCode = generator.Generate(schema);
                    if (!string.IsNullOrEmpty(sourceCode.Code))
                    {
                        context.AddSource(sourceCode.FileName, sourceCode.Code);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.GenerationError,
                Location.None,
                ex.Message));
        }
    }

    private Dictionary<string, ApiSchema> LoadEmbeddedSchemas()
    {
        var assembly = typeof(ARISourceGenerator).Assembly;
        var schemas = new Dictionary<string, ApiSchema>();
        
        var resourceNames = new[]
        {
            "Applications.json", "Asterisk.json", "Bridges.json",
            "Channels.json", "DeviceStates.json", "Endpoints.json",
            "Events.json", "Mailboxes.json", "Playbacks.json",
            "Recordings.json", "Sounds.json"
        };

        foreach (var resourceName in resourceNames)
        {
            using var stream = assembly.GetManifestResourceStream($"Arke.ARI.SourceGenerator.Resources.{resourceName}");
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var schema = JsonSerializer.Deserialize<ApiSchema>(json);
            schemas[Path.GetFileNameWithoutExtension(resourceName)] = schema;
        }

        return schemas;
    }
}
```

#### 2. Schema Models

```csharp
public class ApiSchema
{
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; }

    [JsonPropertyName("apis")]
    public List<ApiEndpoint> Apis { get; set; }

    [JsonPropertyName("models")]
    public Dictionary<string, ModelDefinition> Models { get; set; }
}

public class ApiEndpoint
{
    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("operations")]
    public List<Operation> Operations { get; set; }
}

public class Operation
{
    [JsonPropertyName("httpMethod")]
    public string HttpMethod { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; }

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; }

    [JsonPropertyName("responseClass")]
    public string ResponseClass { get; set; }

    [JsonPropertyName("parameters")]
    public List<Parameter> Parameters { get; set; }

    [JsonPropertyName("errorResponses")]
    public List<ErrorResponse> ErrorResponses { get; set; }
}

public class ModelDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, PropertyDefinition> Properties { get; set; }

    [JsonPropertyName("subTypes")]
    public List<string> SubTypes { get; set; }
}

public class PropertyDefinition
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; }
}
```

#### 3. Model Generator Implementation

```csharp
public class ModelGenerator : ICodeGenerator
{
    public GeneratedCode Generate(ApiSchema schema)
    {
        var code = new StringBuilder();
        var fileName = $"Generated.Models.{schema.GetType().Name}.cs";

        // Generate using directives
        code.AppendLine("/*");
        code.AppendLine("   Arke ARI Framework");
        code.AppendLine($"   Automatically generated file @ {DateTime.Now}");
        code.AppendLine("   Generated by Roslyn Source Generator");
        code.AppendLine("*/");
        code.AppendLine("using System;");
        code.AppendLine("using System.Collections.Generic;");
        code.AppendLine("using Arke.ARI.Actions;");
        code.AppendLine();
        code.AppendLine("namespace Arke.ARI.Models");
        code.AppendLine("{");

        // Generate model classes
        foreach (var model in schema.Models.Values)
        {
            GenerateModelClass(code, model, schema);
        }

        code.AppendLine("}");

        return new GeneratedCode(fileName, code.ToString());
    }

    private void GenerateModelClass(StringBuilder code, ModelDefinition model, ApiSchema schema)
    {
        var className = model.Id;
        var inheritance = GetInheritanceString(model, schema);

        // XML documentation
        code.AppendLine("    /// <summary>");
        code.AppendLine($"    /// {EscapeXmlDoc(model.Description ?? "No description provided")}");
        code.AppendLine("    /// </summary>");

        // Class declaration
        code.AppendLine($"    public class {className}{inheritance}");
        code.AppendLine("    {");

        // Generate properties
        if (model.Properties != null)
        {
            foreach (var property in model.Properties)
            {
                GenerateProperty(code, property.Key, property.Value);
            }
        }

        code.AppendLine("    }");
        code.AppendLine();
    }

    private void GenerateProperty(StringBuilder code, string propertyName, PropertyDefinition property)
    {
        var safePropertyName = NamingHelper.GetSafeName(propertyName);
        var propertyType = TypeConverter.ConvertType(property.Type);

        // XML documentation
        code.AppendLine("        /// <summary>");
        code.AppendLine($"        /// {EscapeXmlDoc(property.Description ?? "No description provided")}");
        code.AppendLine("        /// </summary>");

        // Property declaration
        code.AppendLine($"        public {propertyType} {safePropertyName} {{ get; set; }}");
        code.AppendLine();
    }

    private string GetInheritanceString(ModelDefinition model, ApiSchema schema)
    {
        // Find if this model is a subtype of another
        var parentType = schema.Models.Values
            .FirstOrDefault(m => m.SubTypes?.Contains(model.Id) == true);

        return parentType != null ? $" : {parentType.Id}" : string.Empty;
    }

    private string EscapeXmlDoc(string text)
    {
        return text?.Replace("&", "&amp;")
                   .Replace("<", "&lt;")
                   .Replace(">", "&gt;")
                   .Replace("\n", " ")
                   .Replace("\r", "");
    }
}
```

#### 4. Action Generator Implementation

```csharp
public class ActionClassGenerator : ICodeGenerator
{
    public GeneratedCode Generate(ApiSchema schema)
    {
        var fileName = Path.GetFileNameWithoutExtension(schema.ResourcePath);
        var actionClassName = $"{fileName}Actions";
        
        var code = new StringBuilder();
        
        // Generate file header
        GenerateFileHeader(code);
        
        // Generate class
        code.AppendLine($"namespace Arke.ARI.Actions");
        code.AppendLine("{");
        code.AppendLine($"    public class {actionClassName} : ARIBaseAction, I{actionClassName}");
        code.AppendLine("    {");
        
        // Constructor
        code.AppendLine($"        public {actionClassName}(IActionConsumer consumer)");
        code.AppendLine("            : base(consumer)");
        code.AppendLine("        { }");
        code.AppendLine();
        
        // Generate methods
        foreach (var api in schema.Apis)
        {
            foreach (var operation in api.Operations)
            {
                GenerateSyncMethod(code, api, operation);
                GenerateAsyncMethod(code, api, operation);
            }
        }
        
        code.AppendLine("    }");
        code.AppendLine("}");
        
        return new GeneratedCode($"Generated.Actions.{actionClassName}.cs", code.ToString());
    }

    private void GenerateSyncMethod(StringBuilder code, ApiEndpoint api, Operation operation)
    {
        var methodName = NamingHelper.GetSafeName(operation.Nickname);
        var returnType = TypeConverter.ConvertType(operation.ResponseClass);
        var parameters = GenerateParameterList(operation.Parameters);

        // XML documentation
        GenerateMethodDocumentation(code, operation);

        // Method signature
        code.AppendLine($"        public virtual {returnType} {methodName}({parameters})");
        code.AppendLine("        {");

        // Method implementation
        GenerateMethodBody(code, api.Path, operation);

        code.AppendLine("        }");
        code.AppendLine();
    }

    private void GenerateAsyncMethod(StringBuilder code, ApiEndpoint api, Operation operation)
    {
        var methodName = NamingHelper.GetSafeName(operation.Nickname) + "Async";
        var returnType = TypeConverter.ConvertTaskType(operation.ResponseClass);
        var parameters = GenerateParameterList(operation.Parameters);

        // XML documentation
        GenerateMethodDocumentation(code, operation);

        // Method signature
        code.AppendLine($"        public virtual async {returnType} {methodName}({parameters})");
        code.AppendLine("        {");

        // Method implementation
        GenerateAsyncMethodBody(code, api.Path, operation);

        code.AppendLine("        }");
        code.AppendLine();
    }

    private void GenerateMethodBody(StringBuilder code, string path, Operation operation)
    {
        code.AppendLine($"            string path = \"{path.Substring(1)}\";");
        code.AppendLine($"            var request = GetNewRequest(path, HttpMethod.{operation.HttpMethod.ToUpper()});");

        // Add parameters
        if (operation.Parameters != null)
        {
            foreach (var parameter in operation.Parameters)
            {
                GenerateParameterHandling(code, parameter);
            }
        }

        // Execute request and handle response
        var returnType = TypeConverter.ConvertType(operation.ResponseClass);
        GenerateResponseHandling(code, returnType, operation);
    }

    private void GenerateParameterHandling(StringBuilder code, Parameter parameter)
    {
        code.AppendLine($"            if ({parameter.Name} != null)");
        
        switch (parameter.ParamType.ToLower())
        {
            case "path":
                code.AppendLine($"                request.AddUrlSegment(\"{parameter.Name}\", {parameter.Name});");
                break;
            case "query":
                code.AppendLine($"                request.AddParameter(\"{parameter.Name}\", {parameter.Name}, ParameterType.QueryString);");
                break;
            case "body":
                code.AppendLine("            {");
                code.AppendLine($"                request.AddParameter(\"application/json\", new {{ {parameter.Name} = {parameter.Name} }}, ParameterType.RequestBody);");
                code.AppendLine("            }");
                break;
            case "header":
                code.AppendLine($"                request.AddParameter(\"{parameter.Name}\", {parameter.Name}, ParameterType.HttpHeader);");
                break;
        }
    }

    private void GenerateResponseHandling(StringBuilder code, string returnType, Operation operation)
    {
        if (returnType == "void")
        {
            code.AppendLine("            var response = Execute(request);");
            code.AppendLine("            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)");
            code.AppendLine("                return;");
        }
        else if (returnType == "byte[]")
        {
            code.AppendLine("            var response = Execute(request);");
            code.AppendLine("            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)");
            code.AppendLine("                return response.RawData;");
        }
        else
        {
            code.AppendLine($"            var response = Execute<{returnType}>(request);");
            code.AppendLine("            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)");
            code.AppendLine("                return response.Data;");
        }

        // Error handling
        code.AppendLine("            switch ((int)response.StatusCode)");
        code.AppendLine("            {");

        if (operation.ErrorResponses != null)
        {
            foreach (var error in operation.ErrorResponses)
            {
                code.AppendLine($"                case {error.Code}:");
                code.AppendLine($"                    throw new AriException(\"{error.Reason}\", (int)response.StatusCode);");
            }
        }

        code.AppendLine("                default:");
        code.AppendLine("                    throw new AriException(string.Format(\"Unknown response code {0} from ARI.\", response.StatusCode), (int)response.StatusCode);");
        code.AppendLine("            }");
    }
}
```

#### 5. Type Converter Utility

```csharp
public static class TypeConverter
{
    private static readonly Dictionary<string, string> TypeMappings = new()
    {
        { "void", "void" },
        { "string", "string" },
        { "int", "int" },
        { "long", "long" },
        { "double", "double" },
        { "float", "float" },
        { "boolean", "bool" },
        { "date", "DateTime" },
        { "binary", "byte[]" },
        { "containers", "Dictionary<string, string>" }
    };

    public static string ConvertType(string swaggerType)
    {
        if (string.IsNullOrEmpty(swaggerType))
            return "object";

        // Handle array types List[Type] -> List<Type>
        if (swaggerType.Contains("[") && swaggerType.Contains("]"))
        {
            return swaggerType.Replace("[", "<").Replace("]", ">");
        }

        // Handle generic types
        if (swaggerType.StartsWith("List["))
        {
            var innerType = swaggerType.Substring(5, swaggerType.Length - 6);
            return $"List<{ConvertType(innerType)}>";
        }

        // Map basic types
        var lowerType = swaggerType.ToLowerInvariant();
        return TypeMappings.TryGetValue(lowerType, out var mappedType) ? mappedType : swaggerType;
    }

    public static string ConvertTaskType(string swaggerType)
    {
        var baseType = ConvertType(swaggerType);
        return baseType == "void" ? "Task" : $"Task<{baseType}>";
    }
}
```

## Project Structure

```
Arke.ARI.SourceGenerator/
├── Arke.ARI.SourceGenerator.csproj
├── ARISourceGenerator.cs
├── Generators/
│   ├── ICodeGenerator.cs
│   ├── ModelGenerator.cs
│   ├── ActionInterfaceGenerator.cs
│   ├── ActionClassGenerator.cs
│   ├── EventGenerator.cs
│   └── ClientGenerator.cs
├── Schema/
│   ├── ApiSchema.cs
│   ├── ModelDefinition.cs
│   ├── Operation.cs
│   └── Parameter.cs
├── Utilities/
│   ├── TypeConverter.cs
│   ├── NamingHelper.cs
│   └── CodeBuilder.cs
├── Resources/
│   ├── Applications.json
│   ├── Asterisk.json
│   ├── Bridges.json
│   ├── Channels.json
│   ├── DeviceStates.json
│   ├── Endpoints.json
│   ├── Events.json
│   ├── Mailboxes.json
│   ├── Playbacks.json
│   ├── Recordings.json
│   └── Sounds.json
└── GeneratedCode.cs
```

## Integration with Main Project

Update `Arke.ARI.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <!-- other properties -->
  </PropertyGroup>

  <ItemGroup>
    <Analyzer Include="..\Arke.ARI.SourceGenerator\bin\$(Configuration)\netstandard2.0\Arke.ARI.SourceGenerator.dll" />
  </ItemGroup>

  <!-- Remove old generated files from source control -->
  <ItemGroup>
    <Compile Remove="ARI_1_0\**\*.cs" />
  </ItemGroup>

  <!-- other package references -->
</Project>
```

## Testing Strategy

### Unit Tests for Source Generator

```csharp
[TestClass]
public class ModelGeneratorTests
{
    [TestMethod]
    public void GenerateModelClass_WithBasicProperties_GeneratesCorrectCode()
    {
        // Arrange
        var schema = new ApiSchema
        {
            Models = new Dictionary<string, ModelDefinition>
            {
                ["Application"] = new ModelDefinition
                {
                    Id = "Application",
                    Description = "Details of a Stasis application",
                    Properties = new Dictionary<string, PropertyDefinition>
                    {
                        ["name"] = new PropertyDefinition { Type = "string", Description = "Application name" }
                    }
                }
            }
        };

        var generator = new ModelGenerator();

        // Act
        var result = generator.Generate(schema);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Code.Contains("public class Application"));
        Assert.IsTrue(result.Code.Contains("public string Name { get; set; }"));
    }
}
```

This technical implementation guide provides the detailed code examples and architecture needed to successfully migrate from T4 templates to Roslyn Source Generators while maintaining full API compatibility.