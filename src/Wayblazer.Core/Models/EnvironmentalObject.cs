using System.Diagnostics.CodeAnalysis;

namespace Wayblazer.Core.Models;

public class EnvironmentalObject : SystemObject, IEquatable<EnvironmentalObject>
{
	public EnvironmentalObject() {}

	[SetsRequiredMembers]
	public EnvironmentalObject(string name)
	{
		Name = name;
	}

	public bool Equals(EnvironmentalObject? other) => base.Equals(other);
}
