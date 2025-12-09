using System.Text.Json.Serialization;

namespace Wayblazer.Core.Models;

[JsonDerivedType(typeof(SystemObject), typeDiscriminator: "SystemObject")]
[JsonDerivedType(typeof(Benefit), typeDiscriminator: "Benefit")]
[JsonDerivedType(typeof(Building), typeDiscriminator: "Building")]
[JsonDerivedType(typeof(Energy), typeDiscriminator: "Energy")]
[JsonDerivedType(typeof(EnergyInfo), typeDiscriminator: "EnergyInfo")]
[JsonDerivedType(typeof(EnvironmentalObject), typeDiscriminator: "EnvironmentalObject")]
[JsonDerivedType(typeof(Resource), typeDiscriminator: "Resource")]
[JsonDerivedType(typeof(ResourceInfo), typeDiscriminator: "ResourceInfo")]
[JsonDerivedType(typeof(Upgrade), typeDiscriminator: "Upgrade")]
public class SystemObject : IEquatable<SystemObject>
{
	public required string Name { get; set; }

	public bool Equals(SystemObject? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Name == other.Name;
	}
}
