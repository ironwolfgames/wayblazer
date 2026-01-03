namespace Wayblazer;

public static class Constants
{
	public const int WORLD_SIZE = 64;
	public const float INTERACTION_RANGE = 72.0f;

	public const float HARVEST_DURATION = 1.3f;

	public static class Music
	{
		public const string BACKGROUND_1 = "res://Assets/Music/track_1.mp3";
		public const string BACKGROUND_2 = "res://Assets/Music/track_2.mp3";
	}

	public static class NodeGroups
	{
		public const string RESOURCES = "Resources";
	}

	public static class Scenes
	{
		public const string PINE_TREE = "res://Scenes/Objects/pine_tree.tscn";
		public const string GOLD_ORE = "res://Scenes/Objects/gold_ore.tscn";
	}

	public static class Sounds
	{
		public const string HARVEST_WOOD = "res://Assets/Sounds/harvest_wood.wav";
		public const string HARVEST_ORE = "res://Assets/Sounds/harvest_ore.wav";
		public const string FOOTSTEPS = "res://Assets/Sounds/footsteps.wav";
	}

	public static class Sprites
	{
		public const string RESOURCE_ORE = "res://Assets/Resources/resource_ore_1.tres";
		public const string RESOURCE_WOOD = "res://Assets/Resources/resource_wood_1.tres";
	}
}
