using System.Diagnostics.CodeAnalysis;

namespace Wayblazer.Core.Models;

public class Building : SystemObject, IEquatable<Building>
{
	public Building() {}

	[SetsRequiredMembers]
	public Building(string name, List<BuildingAction> actions)
	{
		Name = name;
		Actions = actions;
	}

	public required List<BuildingAction> Actions { get; set; }

	public bool Equals(Building? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Name == other.Name &&
			Actions.SequenceEqual(other.Actions);
	}
}
