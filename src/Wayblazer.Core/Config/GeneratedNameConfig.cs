namespace Wayblazer.Core.Config;

public class GeneratedNameConfig
{
	public required List<string> Stems { get; set; } = new();
	public required List<string> Prefixes { get; set; } = new();
	public required List<string> Suffixes { get; set; } = new();
	public required List<string> UniqueNames { get; set; } = new();
}
