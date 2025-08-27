namespace Arke.ARI.SourceGenerator;

internal class CodeGenerationConfig
{
    public string Namespace { get; set; } = "Arke.ARI";
    public string ModelsNamespace { get; set; } = "Arke.ARI.Models";
    public string ActionsNamespace { get; set; } = "Arke.ARI.Actions";
    public bool GenerateNullableAnnotations { get; set; } = true;
    public bool GenerateAsyncMethods { get; set; } = true;
    public bool GenerateXmlDocumentation { get; set; } = true;
}