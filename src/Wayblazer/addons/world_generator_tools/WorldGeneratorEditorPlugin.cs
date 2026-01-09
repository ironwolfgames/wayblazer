#if TOOLS
using Godot;

namespace Wayblazer;

[Tool]
public partial class WorldGeneratorEditorPlugin : EditorPlugin
{
	private Control? _buttonContainer;
	private Button? _generateButton;
	private WorldGenerator? _currentWorldGenerator;

	public override void _EnterTree()
	{
		// This is called when the plugin is enabled
		GD.Print("WorldGeneratorEditorPlugin enabled");
	}

	public override void _ExitTree()
	{
		// This is called when the plugin is disabled
		// Clean up the button if it exists
		if (_buttonContainer is not null)
		{
			_buttonContainer.QueueFree();
			_buttonContainer = null;
		}
		GD.Print("WorldGeneratorEditorPlugin disabled");
	}

	public override bool _Handles(GodotObject @object)
	{
		// This plugin handles WorldGenerator objects
		return @object is WorldGenerator;
	}

	public override void _Edit(GodotObject @object)
	{
		// This is called when a WorldGenerator is selected in the editor
		_currentWorldGenerator = @object as WorldGenerator;
	}

	public override void _MakeVisible(bool visible)
	{
		// Show/hide the custom UI when a WorldGenerator is selected/deselected
		if (visible)
		{
			ShowGenerateButton();
		}
		else
		{
			HideGenerateButton();
		}
	}

	private void ShowGenerateButton()
	{
		if (_buttonContainer is null)
		{
			// Create the button container
			_buttonContainer = new VBoxContainer();

			// Add a label
			var label = new Label();
			label.Text = "World Generator Tools";
			label.AddThemeStyleboxOverride("normal", new StyleBoxFlat { BgColor = new Color(0.2f, 0.2f, 0.2f) });
			_buttonContainer.AddChild(label);

			// Create the generate button
			_generateButton = new Button();
			_generateButton.Text = "Generate World";
			_generateButton.Pressed += OnGenerateButtonPressed;
			_buttonContainer.AddChild(_generateButton);

			// Add some instructions
			var instructions = new Label();
			instructions.Text = "Adjust parameters in the inspector, then click Generate World to see results.";
			instructions.AutowrapMode = TextServer.AutowrapMode.Word;
			instructions.CustomMinimumSize = new Vector2(0, 40);
			_buttonContainer.AddChild(instructions);

			// Add the button to the editor's bottom panel
			AddControlToBottomPanel(_buttonContainer, "World Generator");
		}

		MakeBottomPanelItemVisible(_buttonContainer);
	}

	private void HideGenerateButton()
	{
		if (_buttonContainer is not null)
		{
			HideBottomPanel();
		}
	}

	private void OnGenerateButtonPressed()
	{
		if (_currentWorldGenerator is not null && _currentWorldGenerator.IsInsideTree())
		{
			GD.Print("Regenerating world...");
			_currentWorldGenerator.GenerateWorldFromZero();
			GD.Print("World regeneration complete!");
		}
		else
		{
			GD.PrintErr("No WorldGenerator selected or it's not in the scene tree");
		}
	}
}
#endif
