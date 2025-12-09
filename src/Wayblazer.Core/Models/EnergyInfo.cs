using System.Diagnostics.CodeAnalysis;

namespace Wayblazer.Core.Models;

public class EnergyInfo : Energy
{
	public EnergyInfo() { }

	[SetsRequiredMembers]
	public EnergyInfo(string name, EnergyKind kind)
		: base(name, kind)
	{
	}
}
