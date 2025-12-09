using Wayblazer.Core.Models;

namespace Wayblazer.Core.Config;

public class WorldGeneratorConfig
{
	public int? Seed { get; set; }
	public required List<string> EnergyNames { get; set; }
	public required Dictionary<ResourceKind, List<string>> ResourceNames { get; set; }
	public required bool HasElectricalEnergy { get; set; }
	public required int MagicEnergyCount { get; set; }
	public required Dictionary<ResourceKind, int> ResourceKindCounts { get; set; }
}
