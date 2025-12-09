namespace Wayblazer.Core.Models;

public class BuildingAction
{
	/// <summary>
	/// The action that the building can perform
	/// </summary>
	public required Action Action { get; set; }
	/// <summary>
	/// Time in seconds for the action to complete
	/// </summary>
	public required int Time { get; set; }
}
