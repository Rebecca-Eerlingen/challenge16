using Godot;

public partial class Inventory : CanvasLayer
{
	// This first version has 16 slots and stores wheat.
	private const int SlotCount = 16;
	private const string WheatIconPath = "res://images/SAMPLE PACK - Pixel Gnome Crop Pack/SAMPLE PACK - Pixel Gnome Crop Pack/Individual Files/Wheat/6 - Wheat Inventory.png";

	private Texture2D _wheatIcon;
	private TextureRect _wheatTextureRect;
	private Label _wheatCountLabel;
	private Control _panel;
	private int _wheatCount;

	public bool IsOpen { get; private set; }

	public override void _EnterTree()
	{
		// CropPlot finds this node through the "inventory" group when wheat is harvested.
		AddToGroup("inventory");
	}

	public override void _Ready()
	{
		// Build the interface in code so the inventory does not need a separate scene.
		_wheatIcon = GD.Load<Texture2D>(WheatIconPath);
		if (_wheatIcon == null)
		{
			GD.PushError($"Could not load inventory icon at '{WheatIconPath}'.");
		}

		BuildInterface();
		UpdateWheatSlot();
	}

	public void Toggle()
	{
		// Keep the state and panel visibility in sync.
		IsOpen = !IsOpen;
		_panel.Visible = IsOpen;
	}

	public void AddWheat(int amount)
	{
		if (amount <= 0)
		{
			GD.PushError("Inventory item amount must be greater than zero.");
			return;
		}

		// Update the stored amount, then refresh the visible slot.
		_wheatCount += amount;
		UpdateWheatSlot();
	}

	private void BuildInterface()
	{
		var root = new Control();
		root.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
		AddChild(root);

		// Equal center anchors place the panel in the middle of the game window.
		var panel = new PanelContainer
		{
			AnchorLeft = 0.5f,
			AnchorTop = 0.5f,
			AnchorRight = 0.5f,
			AnchorBottom = 0.5f,
			OffsetLeft = -230,
			OffsetTop = -250,
			OffsetRight = 230,
			OffsetBottom = 250,
			Visible = false
		};
		panel.AddThemeStyleboxOverride("panel", CreatePanelStyle());
		root.AddChild(panel);
		_panel = panel;

		var margin = new MarginContainer();
		margin.AddThemeConstantOverride("margin_left", 16);
		margin.AddThemeConstantOverride("margin_top", 16);
		margin.AddThemeConstantOverride("margin_right", 16);
		margin.AddThemeConstantOverride("margin_bottom", 16);
		panel.AddChild(margin);

		var content = new VBoxContainer();
		content.AddThemeConstantOverride("separation", 12);
		margin.AddChild(content);

		var title = new Label
		{
			Text = "Inventory",
			HorizontalAlignment = HorizontalAlignment.Center
		};
		title.AddThemeFontSizeOverride("font_size", 28);
		content.AddChild(title);

		// A grid keeps the inventory slots evenly arranged in four columns.
		var grid = new GridContainer
		{
			Columns = 4
		};
		grid.AddThemeConstantOverride("h_separation", 8);
		grid.AddThemeConstantOverride("v_separation", 8);
		content.AddChild(grid);

		for (int i = 0; i < SlotCount; i++)
		{
			var slot = new PanelContainer
			{
				CustomMinimumSize = new Vector2(88, 88)
			};
			slot.AddThemeStyleboxOverride("panel", CreateSlotStyle());
			grid.AddChild(slot);

			var slotContent = new VBoxContainer
			{
				Alignment = BoxContainer.AlignmentMode.Center
			};
			slotContent.AddThemeConstantOverride("separation", 2);
			slot.AddChild(slotContent);

			var icon = new TextureRect
			{
				CustomMinimumSize = new Vector2(48, 48),
				ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
				StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
			};
			slotContent.AddChild(icon);

			var count = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center
			};
			slotContent.AddChild(count);

			if (i == 0)
			{
				// Wheat is displayed in the first slot for now.
				_wheatTextureRect = icon;
				_wheatCountLabel = count;
			}
		}

		var hint = new Label
		{
			Text = "Press I to close",
			HorizontalAlignment = HorizontalAlignment.Center
		};
		content.AddChild(hint);
	}

	private void UpdateWheatSlot()
	{
		// The controls are assigned while building the interface in _Ready().
		if (_wheatTextureRect == null || _wheatCountLabel == null)
		{
			return;
		}

		_wheatTextureRect.Texture = _wheatCount > 0 ? _wheatIcon : null;
		_wheatTextureRect.TooltipText = _wheatCount > 0 ? "Wheat" : string.Empty;
		_wheatCountLabel.Text = _wheatCount > 0 ? $"Wheat x {_wheatCount}" : string.Empty;
	}

	private static StyleBoxFlat CreatePanelStyle()
	{
		// Use a darker, larger frame to distinguish the inventory window from its slots.
		var style = new StyleBoxFlat
		{
			BgColor = new Color(0.16f, 0.13f, 0.12f, 0.96f),
			BorderColor = new Color(0.78f, 0.62f, 0.38f)
		};
		style.SetBorderWidthAll(3);
		style.SetCornerRadiusAll(6);
		return style;
	}

	private static StyleBoxFlat CreateSlotStyle()
	{
		// Each slot gets a lighter background and a subtle border.
		var style = new StyleBoxFlat
		{
			BgColor = new Color(0.25f, 0.21f, 0.19f, 0.96f),
			BorderColor = new Color(0.52f, 0.43f, 0.34f)
		};
		style.SetBorderWidthAll(2);
		style.SetCornerRadiusAll(4);
		return style;
	}
}
