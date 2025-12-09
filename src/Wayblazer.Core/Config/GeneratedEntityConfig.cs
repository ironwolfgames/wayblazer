namespace Wayblazer.Core.Config;

public class GeneratedEntityConfig : GeneratedNameConfig
{
	public required int Count { get; set; }

	public bool IsValid()
	{
		// ensure at least one of prefixes, suffixes, and stems is non-empty OR unique names is non-empty
		if (Prefixes.Count == 0 && Suffixes.Count == 0 && Stems.Count == 0 && UniqueNames.Count == 0)
		{
			Console.Error.WriteLine($"At least one of stems, prefixes, suffixes, or unique names must be specified to generate names.");
			return false;
		}

		// ensure there are enough unique names for the resources
		var totalPossibleNames = (Prefixes.Count == 0 && Suffixes.Count == 0 && Stems.Count == 0) ?
			UniqueNames.Count :
			(Stems.Count == 0 ? 1 : Stems.Count) *
			(Prefixes.Count == 0 ? 1 : Prefixes.Count) *
			(Suffixes.Count == 0 ? 1 : Suffixes.Count);
		if (Count > totalPossibleNames)
		{
			Console.Error.WriteLine($"Not enough unique names.");
			return false;
		}

		return true;
	}
}
