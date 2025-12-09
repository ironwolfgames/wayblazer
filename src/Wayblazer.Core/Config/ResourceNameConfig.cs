using Wayblazer.Core.Models;

namespace Wayblazer.Core.Config;

public class ResourceNameConfig
{
	public required Dictionary<ResourceKind, List<string>> Names { get; set; } = new();
	public required List<string> EnergyNames { get; set; } = new();
}
