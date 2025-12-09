using System.Diagnostics.CodeAnalysis;

namespace Wayblazer.Core.Models;

public class ResourceInfo : Resource
{
	public ResourceInfo() { }

	[SetsRequiredMembers]
	public ResourceInfo(string name, ResourceKind kind, int level = 0)
		: base(name, kind, level)
	{
	}
}
