using System.Diagnostics.CodeAnalysis;

namespace Wayblazer.Core.Models;

public class Action : IEquatable<Action>
{
	public Action() { }

	[SetsRequiredMembers]
	public Action(string name, SystemObject input, SystemObject output)
	{
		Name = name;
		Inputs = [input];
		Outputs = [output];
	}

	[SetsRequiredMembers]
	public Action(string name, List<SystemObject> inputs, List<SystemObject> outputs)
	{
		Name = name;
		Inputs = inputs;
		Outputs = outputs;
	}

	public required string Name { get; set; }
	public required List<SystemObject> Inputs { get; set; }
	public required List<SystemObject> Outputs { get; set; }

	public bool Equals(Action? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Name == other.Name &&
			Inputs.SequenceEqual(other.Inputs) &&
			Outputs.SequenceEqual(other.Outputs);
	}
}
