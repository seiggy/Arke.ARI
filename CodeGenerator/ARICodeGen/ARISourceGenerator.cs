using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Arke.ARI.SourceGenerator
{
    [Generator(LanguageNames.CSharp)]
    public class ARISourceGenerator : IIncrementalGenerator
    {
    private static readonly HashSet<string> GeneratedModels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // No initialization needed
            var config = new AriConfig
            {
                SchemaSource = new SchemaSourceConfig(),
                Resources = new Dictionary<string, ResourceConfig>(),
                CodeGeneration = new CodeGenerationConfig()
            };

            var configProvider = context.AdditionalTextsProvider
                .Where(file => file.Path.EndsWith("asterisk-config.json", StringComparison.OrdinalIgnoreCase))
                .Select((file, cancellationToken) =>
                {
                    var text = file.GetText(cancellationToken)?.ToString() ?? string.Empty;
                    return (file, text);
                })
                .Select((data, CancellationToken) =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(data.text))
                        {
                            return JsonSerializer.Deserialize<AriConfig>(data.text, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            }) ?? config;
                        }
                    }
                    catch (Exception)
                    {
                        // failed to parse config, use default instead
                    }

                    return config;
                });

            context.RegisterSourceOutput(configProvider, (context, config) =>
            {
                try
                {
                    if (config.SchemaSource.Type.Equals("endpoint", StringComparison.InvariantCultureIgnoreCase)
                        && !string.IsNullOrEmpty(config.SchemaSource.Endpoint))
                    {
                        LoadFromEndpoint(context, config);
                    }
                    else
                    {
                        LoadFromEmbeddedResources(context, config);
                    }
                    GenerateEventDispatcher(context, config);
                }
                catch (Exception e)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "ARI001",
                            title: "Source generation failed",
                            messageFormat: "Failed to generate sources: {0}",
                            category: "AriSourceGenerator",
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true),
                        Location.None,
                        e.ToString()));
                }
            });
        }

        private void LoadFromEmbeddedResources(SourceProductionContext context, AriConfig config)
        {
            var assembly = typeof(ARISourceGenerator).Assembly;

            foreach (var resourceEntry in config.Resources)
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

                GenerateModels(context, json, filename, config);
                GenerateActionInterface(context, json, filename, config);
                GenerateActionImplementation(context, json, filename, config);
            }
        }

        private void LoadFromEndpoint(SourceProductionContext context, AriConfig config)
        {
            try
            {
                // Use System.Net.Http to fetch schemas from endpoint
                using var client = new System.Net.Http.HttpClient();
                var parameters = new Dictionary<string, string>();
                // Add authentication if provided
                if (!string.IsNullOrEmpty(config.SchemaSource.Username) && !string.IsNullOrEmpty(config.SchemaSource.Password))
                {
                    var authValue = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{config.SchemaSource.Username}:{config.SchemaSource.Password}"));
                    parameters.Add("api_key", authValue);
                }

                foreach (var resourceEntry in config.Resources)
                {
                    if (!resourceEntry.Value.Enabled)
                        continue;

                        var resourceUrl = $"{config.SchemaSource.Endpoint!.TrimEnd('/')}/{resourceEntry.Value.SourcePath.TrimStart('/')}";
                    var queryString = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                    var resourceUrlWithParams = $"{resourceUrl}?{queryString}";
                    var response = client.GetAsync(resourceUrlWithParams).GetAwaiter().GetResult();

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

                    GenerateModels(context, json, filename, config);
                    GenerateActionInterface(context, json, filename, config);
                    GenerateActionImplementation(context, json, filename, config);
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

        private void GenerateModels(SourceProductionContext context, string json, string filename, AriConfig config)
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

                if (string.IsNullOrWhiteSpace(className))
                    continue;
                if (!GeneratedModels.Add(className))
                    continue;

                var source = GenerateModelClass(model.Value, className, inherit, config);
                var hintName = $"{config.CodeGeneration.ModelsNamespace}.{className}.g.cs";
                context.AddSource(hintName, source);
            }
        }

        private string GenerateModelClass(JsonElement modelElement, string className, string baseClass, AriConfig config)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated/>");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Text.Json.Serialization;");

            if (config.CodeGeneration.GenerateNullableAnnotations)
            {
                sb.AppendLine("#nullable enable");
            }

            sb.AppendLine();
            sb.AppendLine($"namespace {config.CodeGeneration.ModelsNamespace}");
            sb.AppendLine("{");

            string description = className + " model";
            if (modelElement.TryGetProperty("description", out var descriptionProp))
            {
                description = SanitizeDoc(descriptionProp.GetString() ?? description);
            }

            if (config.CodeGeneration.GenerateXmlDocumentation)
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
                    var jsonName = prop.Name;
                    var propName = GetSafeName(ToPascalCase(prop.Name));
                    var propObj = prop.Value;

                    string propType = "string";
                    if (propObj.TryGetProperty("type", out var typeProp))
                    {
                        propType = TypeConvert(typeProp.GetString() ?? "string");
                    }

                    string propDescription = $"{propName} property";
                    if (propObj.TryGetProperty("description", out var propDescriptionProp))
                    {
                        propDescription = SanitizeDoc(propDescriptionProp.GetString() ?? propDescription);
                    }

                    if (config.CodeGeneration.GenerateXmlDocumentation)
                    {
                        sb.AppendLine($"        /// <summary>");
                        sb.AppendLine($"        /// {propDescription}");
                        sb.AppendLine($"        /// </summary>");
                    }

                    sb.AppendLine($"        [JsonPropertyName(\"{jsonName}\")]\n        public {propType} {propName} {{ get; set; }}");
                    sb.AppendLine();
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private void GenerateActionInterface(SourceProductionContext context, string json, string filename, AriConfig config)
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
            source.AppendLine($"using {config.CodeGeneration.ModelsNamespace};");

            if (config.CodeGeneration.GenerateNullableAnnotations)
            {
                source.AppendLine("#nullable enable");
            }

            source.AppendLine();
            source.AppendLine($"namespace {config.CodeGeneration.ActionsNamespace}");
            source.AppendLine("{");

            if (config.CodeGeneration.GenerateXmlDocumentation)
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
                        var methodName = ToPascalCase(operation.GetProperty("nickname").GetString() ?? "Unknown");

                        var summary = operation.TryGetProperty("summary", out var summaryProp)
                            ? SanitizeDoc(summaryProp.GetString() ?? string.Empty)
                            : string.Empty;

                        var notes = operation.TryGetProperty("notes", out var notesProp)
                            ? SanitizeDoc(notesProp.GetString() ?? string.Empty)
                            : string.Empty;

                        var description = $"{summary}. {notes}";

                        var responseClass = operation.TryGetProperty("responseClass", out var responseProp)
                            ? responseProp.GetString() ?? "void"
                            : "void";

                        var responseType = TypeConvert(responseClass);
                        var isVoid = string.Equals(responseType, "void", StringComparison.OrdinalIgnoreCase);
                        var parameters = GetMethodParameters(operation, config);

                        // Generate async method only
                        if (config.CodeGeneration.GenerateAsyncMethods)
                        {
                            if (config.CodeGeneration.GenerateXmlDocumentation)
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
                                            ? SanitizeDoc(paramDescProp.GetString() ?? string.Empty)
                                            : string.Empty;

                                        source.AppendLine($"        /// <param name=\"{paramName}\">{paramDesc}</param>");
                                    }
                                }
                            }

                            if (isVoid)
                                source.AppendLine($"        Task {methodName}Async({parameters});");
                            else
                                source.AppendLine($"        Task<{responseType}> {methodName}Async({parameters});");
                            source.AppendLine();
                        }
                    }
                }
            }

            source.AppendLine("    }");
            source.AppendLine("}");

            var hintName = $"{config.CodeGeneration.ActionsNamespace}.I{actionClassName}.g.cs";
            context.AddSource(hintName, source.ToString());
        }

        private void GenerateActionImplementation(SourceProductionContext context, string json, string filename, AriConfig config)
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
            source.AppendLine($"using {config.CodeGeneration.ModelsNamespace};");

            if (config.CodeGeneration.GenerateNullableAnnotations)
            {
                source.AppendLine("#nullable enable");
            }

            source.AppendLine();
            source.AppendLine($"namespace {config.CodeGeneration.ActionsNamespace}");
            source.AppendLine("{");

            if (config.CodeGeneration.GenerateXmlDocumentation)
            {
                source.AppendLine($"    /// <summary>");
                source.AppendLine($"    /// Implementation of {filename} operations");
                source.AppendLine($"    /// </summary>");
            }

            source.AppendLine($"    public sealed class {actionClassName} : I{actionClassName}");
            source.AppendLine("    {");
            source.AppendLine($"        private readonly {config.CodeGeneration.Namespace}.AriClient _client;");
            source.AppendLine();
            source.AppendLine($"        public {actionClassName}({config.CodeGeneration.Namespace}.AriClient client)");
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
                        var methodName = ToPascalCase(operation.GetProperty("nickname").GetString() ?? "Unknown");
                        var httpMethod = operation.GetProperty("httpMethod").GetString() ?? "GET";

                        var responseClass = operation.TryGetProperty("responseClass", out var responseProp)
                            ? responseProp.GetString() ?? "void"
                            : "void";

                        var responseType = TypeConvert(responseClass);
                        var isVoid = string.Equals(responseType, "void", StringComparison.OrdinalIgnoreCase);
                        var parameters = GetMethodParameters(operation, config);
                        var endpoint = BuildEndpoint(path, operation);

                        // Sync wrappers remain for convenience but aren't part of the interface
                        source.AppendLine(isVoid
                            ? $"        public void {methodName}({parameters})"
                            : $"        public {responseType} {methodName}({parameters})");
                        source.AppendLine("        {");
                        var namedArgs = GetNamedArguments(operation);
                        if (isVoid)
                            source.AppendLine($"            {methodName}Async({namedArgs}).GetAwaiter().GetResult();");
                        else
                            source.AppendLine($"            return {methodName}Async({namedArgs}).GetAwaiter().GetResult();");
                        source.AppendLine("        }");
                        source.AppendLine();

                        // Async implementation
                        source.AppendLine(isVoid
                            ? $"        public async Task {methodName}Async({parameters})"
                            : $"        public async Task<{responseType}> {methodName}Async({parameters})");
                        source.AppendLine("        {");
                        source.AppendLine($"            var url = \"{endpoint}\";");

                        if (operation.TryGetProperty("parameters", out var paramsForQuery))
                        {
                            foreach (var p in paramsForQuery.EnumerateArray())
                            {
                                var pName = p.GetProperty("name").GetString() ?? string.Empty;
                                var pType = p.GetProperty("paramType").GetString() ?? string.Empty;
                                var pDataType = p.TryGetProperty("dataType", out var dtProp) ? dtProp.GetString() ?? "string" : "string";
                                var csType = TypeConvert(pDataType);

                                if (string.Equals(pType, "query", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (csType == "string")
                                    {
                                        source.AppendLine($"            if (!string.IsNullOrEmpty({pName})) url += (url.Contains(\"?\") ? \"&\" : \"?\") + \"{pName}=\" + System.Uri.EscapeDataString({pName});");
                                    }
                                    else if (csType == "bool")
                                    {
                                        source.AppendLine($"            url += (url.Contains(\"?\") ? \"&\" : \"?\") + \"{pName}=\" + ({pName} ? \"true\" : \"false\");");
                                    }
                                    else if (csType == "int" || csType == "long")
                                    {
                                        source.AppendLine($"            url += (url.Contains(\"?\") ? \"&\" : \"?\") + \"{pName}=\" + System.Uri.EscapeDataString({pName}.ToString(System.Globalization.CultureInfo.InvariantCulture));");
                                    }
                                    else if (csType == "double")
                                    {
                                        source.AppendLine($"            url += (url.Contains(\"?\") ? \"&\" : \"?\") + \"{pName}=\" + System.Uri.EscapeDataString({pName}.ToString(System.Globalization.CultureInfo.InvariantCulture));");
                                    }
                                    else
                                    {
                                        source.AppendLine($"            if ({pName} != null) url += (url.Contains(\"?\") ? \"&\" : \"?\") + \"{pName}=\" + System.Uri.EscapeDataString({pName}.ToString());");
                                    }
                                }
                            }
                        }

                        switch ((httpMethod ?? string.Empty).ToUpperInvariant())
                        {
                            case "GET":
                                if (isVoid)
                                    source.AppendLine("            await _client.GetAsync(url);");
                                else
                                    source.AppendLine($"            return await _client.GetAsync<{responseType}>(url);");
                                break;
                            case "POST":
                                if (isVoid)
                                    source.AppendLine("            await _client.PostAsync(url);");
                                else
                                    source.AppendLine($"            return await _client.PostAsync<{responseType}>(url);");
                                break;
                            case "PUT":
                                if (isVoid)
                                    source.AppendLine("            await _client.PutAsync(url);");
                                else
                                    source.AppendLine($"            return await _client.PutAsync<{responseType}>(url);");
                                break;
                            case "DELETE":
                                if (isVoid)
                                    source.AppendLine("            await _client.DeleteAsync(url);");
                                else
                                    source.AppendLine($"            return await _client.DeleteAsync<{responseType}>(url);");
                                break;
                            default:
                                if (!isVoid)
                                    source.AppendLine("            return default!;");
                                break;
                        }

                        source.AppendLine("        }");
                        source.AppendLine();
                    }
                }
            }

            source.AppendLine("    }");
            source.AppendLine("}");

            var hintName = $"{config.CodeGeneration.ActionsNamespace}.{actionClassName}.g.cs";
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
                }
            }

            if (!string.IsNullOrEmpty(endpoint) && endpoint.StartsWith("/"))
                endpoint = endpoint.Substring(1);
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
                        parameterNames.Add(paramName!);
                    }
                }
            }

            return string.Join(", ", parameterNames);
        }

        private string GetNamedArguments(JsonElement operation)
        {
            var named = new List<string>();
            if (operation.TryGetProperty("parameters", out var parameters))
            {
                foreach (var param in parameters.EnumerateArray())
                {
                    var paramName = param.GetProperty("name").GetString();
                    if (!string.IsNullOrEmpty(paramName))
                    {
                        named.Add($"{paramName}: {paramName}");
                    }
                }
            }
            return string.Join(", ", named);
        }

        private string GetMethodParameters(JsonElement operation, AriConfig config)
        {
            var parameters = new List<string>();

            if (operation.TryGetProperty("parameters", out var parametersElement))
            {
                // Create a list to collect parameters
                var paramList = new List<(string Name, string Type, bool Required, string DefaultValueRaw)>();

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

                    string defaultValueRaw = null;
                    if (param.TryGetProperty("defaultValue", out var defaultValueProp))
                    {
                        defaultValueRaw = defaultValueProp.GetRawText();
                    }

                    paramList.Add((Name: paramName, Type: paramType, Required: isRequired, DefaultValueRaw: defaultValueRaw));
                }

                // Required parameters first, then optional
                foreach (var param in paramList.Where(p => p.Required))
                {
                    parameters.Add($"{param.Type} {param.Name}");
                }
                foreach (var param in paramList.Where(p => !p.Required))
                {
                    var type = param.Type;
                    string defaultLiteral = "null";

                    if (type == "bool")
                    {
                        if (param.DefaultValueRaw != null)
                        {
                            var raw = param.DefaultValueRaw.Trim().Trim('"');
                            if (bool.TryParse(raw, out var b))
                                defaultLiteral = b ? "true" : "false";
                            else
                                defaultLiteral = "false";
                        }
                        else
                        {
                            defaultLiteral = "false";
                        }
                        parameters.Add($"bool {param.Name} = {defaultLiteral}");
                    }
                    else if (type == "int")
                    {
                        if (param.DefaultValueRaw != null && int.TryParse(param.DefaultValueRaw.Trim().Trim('"'), out var i))
                            defaultLiteral = i.ToString();
                        else
                            defaultLiteral = "0";
                        parameters.Add($"int {param.Name} = {defaultLiteral}");
                    }
                    else if (type == "long")
                    {
                        if (param.DefaultValueRaw != null && long.TryParse(param.DefaultValueRaw.Trim().Trim('"'), out var l))
                            defaultLiteral = l.ToString() + "L";
                        else
                            defaultLiteral = "0L";
                        parameters.Add($"long {param.Name} = {defaultLiteral}");
                    }
                    else if (type == "double")
                    {
                        if (param.DefaultValueRaw != null && double.TryParse(param.DefaultValueRaw.Trim().Trim('"'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
                            defaultLiteral = d.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        else
                            defaultLiteral = "0D";
                        parameters.Add($"double {param.Name} = {defaultLiteral}");
                    }
                    else
                    {
                        var t = type;
                        if (config.CodeGeneration.GenerateNullableAnnotations && (t == "string" || t.StartsWith("List<") || t == "object"))
                            t += "?";
                        parameters.Add($"{t} {param.Name} = null");
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
                case "double": return "double";
                case "date": return "string";
                case "void": return "void";
                case "containers": return "Dictionary<string, string>";
                case "object": return "object";
                case "binary": return "byte[]";
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

    private void GenerateEventDispatcher(SourceProductionContext context, AriConfig config)
        {
            try
            {
                var assembly = typeof(ARISourceGenerator).Assembly;
                var eventsResourceName = assembly
                    .GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith("Events.json", StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrEmpty(eventsResourceName))
                    return;

                using var stream = assembly.GetManifestResourceStream(eventsResourceName);
                if (stream == null) return;
                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();

                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;
                if (!root.TryGetProperty("models", out var models)) return;
                if (!models.TryGetProperty("Event", out var eventModel)) return;
                if (!eventModel.TryGetProperty("subTypes", out var subTypes)) return;

                var eventNames = new List<string>();
                foreach (var st in subTypes.EnumerateArray())
                {
                    var name = st.GetString();
                    if (!string.IsNullOrWhiteSpace(name))
                        eventNames.Add(name!);
                }

                var sb = new StringBuilder();
                sb.AppendLine("// <auto-generated/>");
                sb.AppendLine("using System;");
                sb.AppendLine($"using {config.CodeGeneration.ModelsNamespace};");
                if (config.CodeGeneration.GenerateNullableAnnotations)
                {
                    sb.AppendLine("#nullable enable");
                }
                sb.AppendLine();
                sb.AppendLine($"namespace {config.CodeGeneration.Namespace}");
                sb.AppendLine("{");

                foreach (var name in eventNames)
                {
                    var eventType = name + "Event";
                    var handlerName = name + "EventHandler";
                    sb.AppendLine($"    public delegate void {handlerName}(IAriClient sender, {eventType} e);");
                }
                sb.AppendLine("    public delegate void UnhandledEventHandler(object sender, Event eventMessage);");
                sb.AppendLine();
                sb.AppendLine("    public interface IAriEventClient");
                sb.AppendLine("    {");
                foreach (var name in eventNames)
                {
                    var handlerName = name + "EventHandler";
                    sb.AppendLine($"        event {handlerName} On{name}Event;");
                }
                sb.AppendLine("        event UnhandledEventHandler OnUnhandledEvent;");
                sb.AppendLine("    }");
                sb.AppendLine();
                sb.AppendLine("    public class BaseAriClient : IAriEventClient");
                sb.AppendLine("    {");
                sb.AppendLine("        public virtual event UnhandledExceptionEventHandler OnUnhandledException;");
                sb.AppendLine();
                sb.AppendLine("        #region Events");
                foreach (var name in eventNames)
                {
                    var handlerName = name + "EventHandler";
                    sb.AppendLine($"        public virtual event {handlerName} On{name}Event;");
                }
                sb.AppendLine("        public virtual event UnhandledEventHandler OnUnhandledEvent;");
                sb.AppendLine("        #endregion");
                sb.AppendLine();
                sb.AppendLine("        protected bool UnhandledException(object sender, Exception exception)");
                sb.AppendLine("        {");
                sb.AppendLine("            if (OnUnhandledException != null)");
                sb.AppendLine("            {");
                sb.AppendLine("                OnUnhandledException(sender, new UnhandledExceptionEventArgs(exception, false));");
                sb.AppendLine("                return true;");
                sb.AppendLine("            }");
                sb.AppendLine("            return false;");
                sb.AppendLine("        }");
                sb.AppendLine();
                sb.AppendLine("        protected void FireEvent(string eventName, object eventArgs, IAriClient sender)");
                sb.AppendLine("        {");
                sb.AppendLine("            switch(eventName)");
                sb.AppendLine("            {");
                foreach (var name in eventNames)
                {
                    sb.AppendLine($"                case \"{name}\":");
                    sb.AppendLine($"                    if (On{name}Event != null)");
                    sb.AppendLine($"                        On{name}Event(sender, ({name}Event)eventArgs);");
                    sb.AppendLine("                    else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);");
                    sb.AppendLine("                    break;");
                    sb.AppendLine();
                }
                sb.AppendLine("                default:");
                sb.AppendLine("                    if(OnUnhandledEvent!=null)");
                sb.AppendLine("                        OnUnhandledEvent(this, (Event)eventArgs);");
                sb.AppendLine("                    else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);");
                sb.AppendLine("                    break;");
                sb.AppendLine("            }");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                context.AddSource($"{config.CodeGeneration.Namespace}.EventsDispatcher.g.cs", sb.ToString());
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "ARI005",
                        title: "Event generation failed",
                        messageFormat: "Failed to generate event delegates: {0}",
                        category: "AriSourceGenerator",
                        DiagnosticSeverity.Warning,
                        isEnabledByDefault: true),
                    Location.None,
                    e.Message));
            }
        }

        private static string ToPascalCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var parts = name.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                var s = parts[0];
                return s.Length == 1 ? s.ToUpperInvariant() : char.ToUpperInvariant(s[0]) + s.Substring(1);
            }
            var sb = new StringBuilder();
            foreach (var p in parts)
            {
                if (p.Length == 0) continue;
                sb.Append(char.ToUpperInvariant(p[0]));
                if (p.Length > 1) sb.Append(p.Substring(1));
            }
            return sb.ToString();
        }

        private static string SanitizeDoc(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var withoutNewlines = string.Join(" ", text.Replace('\r', ' ').Replace('\n', ' ').Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return withoutNewlines.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}
