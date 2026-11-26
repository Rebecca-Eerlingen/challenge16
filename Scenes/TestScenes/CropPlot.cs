using Godot;

public partial class CropPlot : Area2D
{
    [Export] public int FirstGrowthFrame = 1; // First frame of growing wheat (skips frame 0)
    [Export] public int MaxGrowthStage = 3;   // Last frame of growing wheat (skips the final frame)

    private Sprite2D _sprite;
    private Timer _growthTimer;
    private CollisionShape2D _interactionShape;
    private int _currentStage;
    private bool _isPlanted = false;

    public Vector2 InteractionPosition => _interactionShape.GlobalPosition;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _growthTimer = GetNode<Timer>("Timer");
        _interactionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        AddToGroup("crop_plots");
        _growthTimer.Timeout += OnGrowthTimerTimeout;

        _sprite.Visible = false;
    }

    public void Interact()
    {
        if (!_isPlanted)
        {
            PlantWheat();
        }
        else if (_currentStage >= MaxGrowthStage)
        {
            HarvestWheat();
        }
    }

    private void PlantWheat()
    {
        _isPlanted = true;
        _currentStage = FirstGrowthFrame;
        _sprite.Frame = _currentStage;
        _sprite.Visible = true;
        _growthTimer.Start();
    }

    private void OnGrowthTimerTimeout()
    {
        if (_currentStage < MaxGrowthStage)
        {
            _currentStage++;
            _sprite.Frame = _currentStage;
        }

        if (_currentStage >= MaxGrowthStage)
        {
            _growthTimer.Stop();
        }
    }

    private void HarvestWheat()
    {
        Inventory inventory = GetTree().GetFirstNodeInGroup("inventory") as Inventory;
        if (inventory == null)
        {
            GD.PushError("Inventory node is missing from the main scene.");
            return;
        }

        _isPlanted = false;
        _sprite.Visible = false;
        inventory.AddWheat(1);
        GD.Print("Wheat harvested!");
    }
}

