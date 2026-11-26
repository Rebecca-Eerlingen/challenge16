using Godot;

public partial class CropPlot : Area2D
{
    [Export] public int FirstGrowthFrame = 1; // First frame of growing wheat (skips frame 0)
    [Export] public int MaxGrowthStage = 3;   // Last frame of growing wheat (skips the final frame)
    [Export] public PackedScene WheatDropScene;

    private Sprite2D _sprite;
    private Timer _growthTimer;
    private int _currentStage;
    private bool _isPlanted = false;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _growthTimer = GetNode<Timer>("Timer");

        InputEvent += OnInputEvent;
        _growthTimer.Timeout += OnGrowthTimerTimeout;

        // Hide the sprite when nothing is planted (hides frame 0)
        _sprite.Visible = false;
    }

    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.Pressed &&
            mouseEvent.ButtonIndex == MouseButton.Left)
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
        _isPlanted = false;
        _sprite.Visible = false; // Hide wheat again after harvesting

        SpawnWheatDrop();
        GD.Print("Wheat harvested!");
    }

    private void SpawnWheatDrop()
    {
        if (WheatDropScene == null)
        {
            GD.PrintErr("WheatDropScene is not assigned in the Inspector!");
            return;
        }

        Node2D wheatDrop = WheatDropScene.Instantiate<Node2D>();
        float offsetX = (float)GD.RandRange(-12.0, 12.0);
        float offsetY = (float)GD.RandRange(-12.0, 12.0);
        wheatDrop.GlobalPosition = GlobalPosition + new Vector2(offsetX, offsetY);

        GetParent().AddChild(wheatDrop);
    }
}

