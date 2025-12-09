using Wayblazer.Core.Models;

namespace Wayblazer.Core.Config;

public class WorldConfig : IEquatable<WorldConfig>
{
	public required List<EnvironmentalObject> Environment { get; set; }
	public required List<Resource> Resources { get; set; }
	public required List<Energy> Energy { get; set; }
	public required List<Models.Action> Actions { get; set; }
	public required List<Building> Buildings { get; set; }
	public required List<Upgrade> Upgrades { get; set; }

	public bool Equals(WorldConfig? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;

		return Environment.SequenceEqual(other.Environment) &&
			Resources.SequenceEqual(other.Resources) &&
			Energy.SequenceEqual(other.Energy) &&
			Actions.SequenceEqual(other.Actions) &&
			Buildings.SequenceEqual(other.Buildings) &&
			Upgrades.SequenceEqual(other.Upgrades);
	}
}
