using System.Diagnostics.CodeAnalysis;

namespace Wayblazer.Core.Models;

public class Upgrade : SystemObject, IEquatable<Upgrade>
{
	public Upgrade() { }

	[SetsRequiredMembers]
	public Upgrade(string name, List<SystemObject> costs, List<Benefit> benefits)
	{
		Name = name;
		Costs = costs;
		Benefits = benefits;
	}

	public required List<SystemObject> Costs { get; set; }
	public required List<Benefit> Benefits { get; set; }

	public bool Equals(Upgrade? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Name == other.Name &&
			Costs.SequenceEqual(other.Costs) &&
			Benefits.SequenceEqual(other.Benefits);
	}
}
