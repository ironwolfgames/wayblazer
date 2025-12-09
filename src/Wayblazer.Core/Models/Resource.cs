using System.Diagnostics.CodeAnalysis;

namespace Wayblazer.Core.Models;

public class Resource : SystemObject, IEquatable<Resource>
{
	public Resource() {}

	[SetsRequiredMembers]
	public Resource(string name, ResourceKind kind, int level = 0)
	{
		Name = name;
		Kind = kind;
		Level = level;
	}

	public required ResourceKind Kind { get; set; }
	public required int Level { get; set; }

	public bool Equals(Resource? other) => base.Equals(other);
}
