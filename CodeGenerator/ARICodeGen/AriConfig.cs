using System.Collections.Generic;

namespace Arke.ARI.SourceGenerator;

/// <summary>
/// Configuration for the ARI source generator
/// </summary>
internal class AriConfig
{
    public SchemaSourceConfig SchemaSource { get; set; } = new SchemaSourceConfig();
    public Dictionary<string, ResourceConfig> Resources { get; set; } = new Dictionary<string, ResourceConfig>();
    public CodeGenerationConfig CodeGeneration { get; set; } = new CodeGenerationConfig();
}