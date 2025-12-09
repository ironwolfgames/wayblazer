using System.Diagnostics.CodeAnalysis;

namespace Wayblazer.Core.Models;

public class Energy : SystemObject, IEquatable<Energy>
{
	public Energy() { }

	[SetsRequiredMembers]
	public Energy(string name, EnergyKind kind)
	{
		Name = name;
		Kind = kind;
	}

	public required EnergyKind Kind { get; set; }

	public bool Equals(Energy? other) => base.Equals(other);
}
