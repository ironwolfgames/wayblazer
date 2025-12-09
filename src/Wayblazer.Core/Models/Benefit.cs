using System.Diagnostics.CodeAnalysis;

namespace Wayblazer.Core.Models;

public class Benefit : SystemObject, IEquatable<Benefit>
{
	public Benefit() { }

	[SetsRequiredMembers]
	public Benefit(string name, BuildingAction target, BenefitKind kind, int amount)
	{
		Name = name;
		Target = target;
		Kind = kind;
		Amount = amount;
	}

	public required BuildingAction Target { get; set; }
	public required BenefitKind Kind { get; set; }
	public required int Amount { get; set; }

	public bool Equals(Benefit? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Name == other.Name &&
			Target == other.Target &&
			Kind == other.Kind &&
			Amount == other.Amount;
	}
}
