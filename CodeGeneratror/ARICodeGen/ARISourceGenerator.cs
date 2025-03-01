using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;

namespace Arke.ARI.SourceGenerator
{
    /// <summary>
    /// Configuration for the ARI source generator
    /// </summary>
    internal class AriConfig
    {
        public SchemaSourceConfig SchemaSource { get; set; } = new SchemaSourceConfig();
        public Dictionary<string, ResourceConfig> Resources { get; set; } = new Dictionary<string, ResourceConfig>();
        public CodeGenerationConfig CodeGeneration { get; set; } = new CodeGenerationConfig();
    }

    internal class SchemaSourceConfig
    {
        public string Type { get; set; } = "embedded";
        public string? Endpoint { get; set; }
    }

    internal class ResourceConfig
    {
        public string SourcePath { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
    }

    internal class CodeGenerationConfig
    {
        public string Namespace { get; set; } = "Arke.ARI";
        public string ModelsNamespace { get; set; } = "Arke.ARI.Models";
        public string ActionsNamespace { get; set; } = "Arke.ARI.Actions";
        public bool GenerateNullableAnnotations { get; set; } = true;
        public bool GenerateAsyncMethods { get; set; } = true;
        public bool GenerateXmlDocumentation { get; set; } = true;
    }

    [Generator]
    public class ARISourceGenerator : ISourceGenerator
    {
        private AriConfig _config = new AriConfig();

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization needed
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Load configuration
            LoadConfig(context);

            // Based on schema source type, either use embedded resources or fetch from endpoint
            if (_config.SchemaSource.Type.Equals("endpoint", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrEmpty(_config.SchemaSource.Endpoint))
            {
                // Fetch from endpoint
                LoadFromEndpoint(context, _config.SchemaSource.Endpoint);
            }
            else
            {
                // Use embedded resources
                LoadFromEmbeddedResources(context);
            }

            // Generate the event dispatcher
            GenerateEventDispatcher(context);
        }

        private void LoadConfig(GeneratorExecutionContext context)
        {
            // Check if configuration file exists in additional files
            foreach (var file in context.AdditionalFiles)
            {
                if (Path.GetFileName(file.Path).Equals("asterisk-config.json", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var jsonContent = file.GetText(context.CancellationToken)?.ToString();
                        if (!string.IsNullOrEmpty(jsonContent))
                        {
                            _config = JsonSerializer.Deserialize<AriConfig>(jsonContent, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            }) ?? new AriConfig();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error parsing config
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                id: "ARI001",
                                title: "Error parsing configuration",
                                messageFormat: "Failed to parse asterisk-config.json: {0}",
                                category: "AriSourceGenerator",
                                DiagnosticSeverity.Warning,
                                isEnabledByDefault: true),
                            Location.None,
                            ex.Message));
                    }
                    break;
                }
            }
        }

        private void LoadFromEmbeddedResources(GeneratorExecutionContext context)
        {
            var assembly = typeof(ARISourceGenerator).Assembly;
            
            foreach (var resourceEntry in _config.Resources)
            {
                if (!resourceEntry.Value.Enabled)
                    continue;

                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(name => name.EndsWith($"{resourceEntry.Key}.json", StringComparison.OrdinalIgnoreCase));

                if (resourceName == null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "ARI002",
                            title: "Resource not found",
                            messageFormat: "Resource {0} not found in embedded resources",
                            category: "AriSourceGenerator",
                            DiagnosticSeverity.Warning,
                            isEnabledByDefault: true),
                        Location.None,
                        resourceEntry.Key));
                    continue;
                }

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;
                
                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();
                var filename = resourceEntry.Key;

                GenerateModels(context, json, filename);
                GenerateActionInterface(context, json, filename);
                GenerateActionImplementation(context, json, filename);
            }
        }

        private void LoadFromEndpoint(GeneratorExecutionContext context, string endpoint)
        {
            try
            {
                // Use System.Net.Http to fetch schemas from endpoint
                using var client = new System.Net.Http.HttpClient();
                
                foreach (var resourceEntry in _config.Resources)
                {
                    if (!resourceEntry.Value.Enabled)
                        continue;

                    var resourceUrl = $"{endpoint.TrimEnd('/')}/{resourceEntry.Value.SourcePath.TrimStart('/')}";
                    var response = client.GetAsync(resourceUrl).GetAwaiter().GetResult();
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                id: "ARI003",
                                title: "Failed to fetch resource",
                                messageFormat: "Failed to fetch {0} from {1}: {2}",
                                category: "AriSourceGenerator",
                                DiagnosticSeverity.Warning,
                                isEnabledByDefault: true),
                            Location.None,
                            resourceEntry.Key, resourceUrl, response.StatusCode));
                        continue;
                    }

                    var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var filename = resourceEntry.Key;

                    GenerateModels(context, json, filename);
                    GenerateActionInterface(context, json, filename);
                    GenerateActionImplementation(context, json, filename);
                }
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "ARI004",
                        title: "Error fetching from endpoint",
                        messageFormat: "Error fetching from endpoint: {0}",
                        category: "AriSourceGenerator",
                        DiagnosticSeverity.Warning,
                        isEnabledByDefault: true),
                    Location.None,
                    ex.Message));
            }
        }

        private void GenerateModels(GeneratorExecutionContext context, string json, string filename)
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            
            if (!root.TryGetProperty("models", out var models))
                return;
                
            var subtypes = new Dictionary<string, List<string>>();

            // First pass - collect subtypes
            foreach (var model in models.EnumerateObject())
            {
                try
                {
                    if (model.Value.TryGetProperty("subTypes", out var subTypesProp))
                    {
                        var id = model.Value.GetProperty("id").GetString();
                        if (id != null)
                        {
                            subtypes[id] = new List<string>();
                            foreach (var subType in subTypesProp.EnumerateArray())
                            {
                                subtypes[id].Add(subType.GetString() ?? string.Empty);
                            }
                        }
                    }
                }
                catch { }
            }

            // Second pass - generate models
            foreach (var model in models.EnumerateObject())
            {
                if (!model.Value.TryGetProperty("id", out var idProp))
                    continue;
                    
                var className = idProp.GetString() ?? string.Empty;
                var inherit = subtypes.FirstOrDefault(x => x.Value.Contains(className)).Key;
                var isEvent = inherit == "Event";

                if (isEvent)
                {
                    className += "Event";
                }

                var source = GenerateModelClass(model.Value, className, inherit);
                var hintName = $"{_config.CodeGeneration.ModelsNamespace}.{className}.g.cs";
                context.AddSource(hintName, source);
            }
        }

        private string GenerateModelClass(JsonElement modelElement, string className, string baseClass)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated/>");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            
            if (_config.CodeGeneration.GenerateNullableAnnotations)
            {
                sb.AppendLine("#nullable enable");
            }

            sb.AppendLine();
            sb.AppendLine($"namespace {_config.CodeGeneration.ModelsNamespace}");
            sb.AppendLine("{");

            string description = className + " model";
            if (modelElement.TryGetProperty("description", out var descriptionProp))
            {
                description = descriptionProp.GetString() ?? description;
            }
            
            if (_config.CodeGeneration.GenerateXmlDocumentation)
            {
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// {description}");
                sb.AppendLine($"    /// </summary>");
            }

            if (string.IsNullOrEmpty(baseClass))
            {
                sb.AppendLine($"    public class {className}");
            }
            else
            {
                sb.AppendLine($"    public class {className} : {baseClass}");
            }

            sb.AppendLine("    {");

            // Add properties
            if (modelElement.TryGetProperty("properties", out var properties))
            {
                foreach (var prop in properties.EnumerateObject())
                {
                    var propName = GetSafeName(prop.Name);
                    var propObj = prop.Value;
                    
                    string propType = "string";
                    if (propObj.TryGetProperty("type", out var typeProp))
                    {
                        propType = TypeConvert(typeProp.GetString() ?? "string");
                    }
                    
                    string propDescription = $"{propName} property";
                    if (propObj.TryGetProperty("description", out var propDescriptionProp))
                    {
                        propDescription = propDescriptionProp.GetString() ?? propDescription;
                    }

                    if (_config.CodeGeneration.GenerateXmlDocumentation)
                    {
                        sb.AppendLine($"        /// <summary>");
                        sb.AppendLine($"        /// {propDescription}");
                        sb.AppendLine($"        /// </summary>");
                    }
                    
                    sb.AppendLine($"        public {propType} {propName} {{ get; set; }}");
                    sb.AppendLine();
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private void GenerateActionInterface(GeneratorExecutionContext context, string json, string filename)
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            
            if (!root.TryGetProperty("apis", out var apis))
                return;
                
            var actionClassName = filename + "Actions";

            var source = new StringBuilder();
            source.AppendLine("// <auto-generated/>");
            source.AppendLine("using System;");
            source.AppendLine("using System.Collections.Generic;");
            source.AppendLine("using System.Threading.Tasks;");
            source.AppendLine($"using {_config.CodeGeneration.ModelsNamespace};");
            
            if (_config.CodeGeneration.GenerateNullableAnnotations)
            {
                source.AppendLine("#nullable enable");
            }

            source.AppendLine();
            source.AppendLine($"namespace {_config.CodeGeneration.ActionsNamespace}");
            source.AppendLine("{");

            if (_config.CodeGeneration.GenerateXmlDocumentation)
            {
                source.AppendLine($"    /// <summary>");
                source.AppendLine($"    /// Interface for {filename} operations");
                source.AppendLine($"    /// </summary>");
            }
            
            source.AppendLine($"    public interface I{actionClassName}");
            source.AppendLine("    {");

            foreach (var api in apis.EnumerateArray())
            {
                if (api.TryGetProperty("operations", out var operations))
                {
                    foreach (var operation in operations.EnumerateArray())
                    {
                        var methodName = operation.GetProperty("nickname").GetString() ?? "Unknown";
                        
                        var summary = operation.TryGetProperty("summary", out var summaryProp) 
                            ? summaryProp.GetString() ?? string.Empty 
                            : string.Empty;
                            
                        var notes = operation.TryGetProperty("notes", out var notesProp) 
                            ? notesProp.GetString() ?? string.Empty 
                            : string.Empty;
                            
                        var description = $"{summary}. {notes}";
                        
                        var responseClass = operation.TryGetProperty("responseClass", out var responseProp)
                            ? responseProp.GetString() ?? "void"
                            : "void";
                            
                        var responseType = TypeConvert(responseClass);
                        var parameters = GetMethodParameters(operation);

                        // Generate sync method
                        if (_config.CodeGeneration.GenerateXmlDocumentation)
                        {
                            source.AppendLine($"        /// <summary>");
                            source.AppendLine($"        /// {description}");
                            source.AppendLine($"        /// </summary>");
                            
                            if (operation.TryGetProperty("parameters", out var parametersProp))
                            {
                                foreach (var param in parametersProp.EnumerateArray())
                                {
                                    var paramName = param.GetProperty("name").GetString() ?? string.Empty;
                                    var paramDesc = param.TryGetProperty("description", out var paramDescProp)
                                        ? paramDescProp.GetString() ?? string.Empty
                                        : string.Empty;
                                        
                                    source.AppendLine($"        /// <param name=\"{paramName}\">{paramDesc}</param>");
                                }
                            }
                        }
                        
                        source.AppendLine($"        {responseType} {methodName}({parameters});");
                        source.AppendLine();

                        // Generate async method if enabled
                        if (_config.CodeGeneration.GenerateAsyncMethods)
                        {
                            if (_config.CodeGeneration.GenerateXmlDocumentation)
                            {
                                source.AppendLine($"        /// <summary>");
                                source.AppendLine($"        /// Asynchronously {description.ToLower()}");
                                source.AppendLine($"        /// </summary>");
                                
                                if (operation.TryGetProperty("parameters", out var paramsForAsync))
                                {
                                    foreach (var param in paramsForAsync.EnumerateArray())
                                    {
                                        var paramName = param.GetProperty("name").GetString() ?? string.Empty;
                                        var paramDesc = param.TryGetProperty("description", out var paramDescProp)
                                            ? paramDescProp.GetString() ?? string.Empty
                                            : string.Empty;
                                            
                                        source.AppendLine($"        /// <param name=\"{paramName}\">{paramDesc}</param>");
                                    }
                                }
                            }
                            
                            source.AppendLine($"        Task<{responseType}> {methodName}Async({parameters});");
                            source.AppendLine();
                        }
                    }
                }
            }

            source.AppendLine("    }");
            source.AppendLine("}");

            var hintName = $"{_config.CodeGeneration.ActionsNamespace}.I{actionClassName}.g.cs";
            context.AddSource(hintName, source.ToString());
        }

        private void GenerateActionImplementation(GeneratorExecutionContext context, string json, string filename)
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            
            if (!root.TryGetProperty("apis", out var apis))
                return;
                
            var actionClassName = filename + "Actions";

            var source = new StringBuilder();
            source.AppendLine("// <auto-generated/>");
            source.AppendLine("using System;");
            source.AppendLine("using System.Collections.Generic;");
            source.AppendLine("using System.Threading.Tasks;");
            source.AppendLine($"using {_config.CodeGeneration.ModelsNamespace};");
            
            if (_config.CodeGeneration.GenerateNullableAnnotations)
            {
                source.AppendLine("#nullable enable");
            }

            source.AppendLine();
            source.AppendLine($"namespace {_config.CodeGeneration.ActionsNamespace}");
            source.AppendLine("{");

            if (_config.CodeGeneration.GenerateXmlDocumentation)
            {
                source.AppendLine($"    /// <summary>");
                source.AppendLine($"    /// Implementation of {filename} operations");
                source.AppendLine($"    /// </summary>");
            }
            
            source.AppendLine($"    internal sealed class {actionClassName} : I{actionClassName}");
            source.AppendLine("    {");
            source.AppendLine($"        private readonly {_config.CodeGeneration.Namespace}.ARIClient _client;");
            source.AppendLine();
            source.AppendLine($"        public {actionClassName}({_config.CodeGeneration.Namespace}.ARIClient client)");
            source.AppendLine("        {");
            source.AppendLine("            _client = client ?? throw new ArgumentNullException(nameof(client));");
            source.AppendLine("        }");
            source.AppendLine();

            foreach (var api in apis.EnumerateArray())
            {
                var path = api.GetProperty("path").GetString() ?? string.Empty;
                
                if (api.TryGetProperty("operations", out var operations))
                {
                    foreach (var operation in operations.EnumerateArray())
                    {
                        var methodName = operation.GetProperty("nickname").GetString() ?? "Unknown";
                        var httpMethod = operation.GetProperty("httpMethod").GetString() ?? "GET";
                        
                        var responseClass = operation.TryGetProperty("responseClass", out var responseProp)
                            ? responseProp.GetString() ?? "void"
                            : "void";
                            
                        var responseType = TypeConvert(responseClass);
                        var parameters = GetMethodParameters(operation);
                        var endpoint = BuildEndpoint(path, operation);

                        // Generate sync method
                        source.AppendLine($"        public {responseType} {methodName}({parameters})");
                        source.AppendLine("        {");
                        source.AppendLine($"            return {methodName}Async({GetParameterNames(operation)}).GetAwaiter().GetResult();");
                        source.AppendLine("        }");
                        source.AppendLine();

                        // Generate async method if enabled
                        if (_config.CodeGeneration.GenerateAsyncMethods)
                        {
                            source.AppendLine($"        public async Task<{responseType}> {methodName}Async({parameters})");
                            source.AppendLine("        {");
                            source.AppendLine($"            var endpoint = $\"{endpoint}\";");

                            if (httpMethod.ToUpper() == "GET")
                            {
                                source.AppendLine($"            return await _client.GetAsync<{responseType}>(endpoint);");
                            }
                            else if (httpMethod.ToUpper() == "POST")
                            {
                                source.AppendLine($"            return await _client.PostAsync<{responseType}>(endpoint);");
                            }
                            else if (httpMethod.ToUpper() == "PUT")
                            {
                                source.AppendLine($"            return await _client.PutAsync<{responseType}>(endpoint);");
                            }
                            else if (httpMethod.ToUpper() == "DELETE")
                            {
                                source.AppendLine($"            await _client.DeleteAsync(endpoint);");
                                source.AppendLine($"            return default;");
                            }

                            source.AppendLine("        }");
                            source.AppendLine();
                        }
                    }
                }
            }

            source.AppendLine("    }");
            source.AppendLine("}");

            var hintName = $"{_config.CodeGeneration.ActionsNamespace}.{actionClassName}.g.cs";
            context.AddSource(hintName, source.ToString());
        }

        private string BuildEndpoint(string path, JsonElement operation)
        {
            var endpoint = path;
            
            if (operation.TryGetProperty("parameters", out var parameters))
            {
                foreach (var param in parameters.EnumerateArray())
                {
                    var paramName = param.GetProperty("name").GetString() ?? string.Empty;
                    var paramType = param.GetProperty("paramType").GetString() ?? string.Empty;

                    if (paramType == "path")
                    {
                        endpoint = endpoint.Replace("{" + paramName + "}", $"{{{paramName}}}");
                    }
                    else if (paramType == "query")
                    {
                        // Add query parameters
                        if (!endpoint.Contains("?"))
                            endpoint += "?";
                        else
                            endpoint += "&";
                        endpoint += $"{paramName}={{{paramName}}}";
                    }
                }
            }

            return endpoint;
        }

        private string GetParameterNames(JsonElement operation)
        {
            var parameterNames = new List<string>();
            
            if (operation.TryGetProperty("parameters", out var parameters))
            {
                foreach (var param in parameters.EnumerateArray())
                {
                    var paramName = param.GetProperty("name").GetString();
                    if (!string.IsNullOrEmpty(paramName))
                    {
                        parameterNames.Add(paramName);
                    }
                }
            }
            
            return string.Join(", ", parameterNames);
        }

        private string GetMethodParameters(JsonElement operation)
        {
            var parameters = new List<string>();
            
            if (operation.TryGetProperty("parameters", out var parametersElement))
            {
                // Create a list to collect parameters
                var paramList = new List<(string Name, string Type, bool Required)>();
                
                // Collect parameter info
                foreach (var param in parametersElement.EnumerateArray())
                {
                    var paramName = param.GetProperty("name").GetString() ?? string.Empty;
                    var dataType = param.TryGetProperty("dataType", out var dataTypeProp) 
                        ? dataTypeProp.GetString() ?? "string"
                        : "string";
                    var paramType = TypeConvert(dataType);
                    
                    var isRequired = false;
                    if (param.TryGetProperty("required", out var requiredProp))
                    {
                        isRequired = requiredProp.GetBoolean();
                    }
                    
                    paramList.Add((Name: paramName, Type: paramType, Required: isRequired));
                }
                
                // Sort required parameters first
                foreach (var param in paramList.OrderByDescending(p => p.Required))
                {
                    if (!param.Required)
                    {
                        string paramType = param.Type;
                        if (_config.CodeGeneration.GenerateNullableAnnotations)
                        {
                            if (paramType == "string" || paramType.StartsWith("List<"))
                                paramType += "?";
                        }
                        else if (paramType == "int" || paramType == "bool" || paramType == "long")
                        {
                            paramType += "?";
                        }
                        parameters.Add($"{paramType} {param.Name} = null");
                    }
                    else
                    {
                        parameters.Add($"{param.Type} {param.Name}");
                    }
                }
            }

            return string.Join(", ", parameters);
        }

        private static string TypeConvert(string type)
        {
            if (string.IsNullOrEmpty(type)) return "void";

            if (type.StartsWith("List["))
            {
                var innerType = type.Substring(5, type.Length - 6);
                return $"List<{TypeConvert(innerType)}>";
            }

            switch (type.ToLower())
            {
                case "string": return "string";
                case "int": return "int";
                case "long": return "long";
                case "boolean": return "bool";
                case "date": return "DateTime";
                case "void": return "void";
                default: return type;
            }
        }

        private static string GetSafeName(string name)
        {
            // Handle C# keywords
            switch (name)
            {
                case "event":
                case "object":
                case "string":
                case "class":
                case "namespace":
                case "interface":
                    return "@" + name;
                default:
                    return name;
            }
        }

        private void GenerateEventDispatcher(GeneratorExecutionContext context)
        {
            // Implementation for event dispatcher generation
            // This would create a class that handles all the different event types
            // and provides a type-safe way to subscribe to specific events
        }
    }
} 