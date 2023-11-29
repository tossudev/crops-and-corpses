using Godot;


public partial class FloatingButtonName: PanelContainer
{
	public const string FLOATING_NAME_NODENAME = "%FloatingButtonNamePanel";

    
    Label _itemNameLabel;
    const string ITEM_NAME_LABEL_NODENAME = "%ButtonNameLabel";

    bool _nameInitiated;
    bool _nameFollowMouse;
    [Export] Vector2 offsetVector;
    void InitiateName()
    {
        _itemNameLabel = GetNode<Label>(ITEM_NAME_LABEL_NODENAME);
        _nameInitiated = true;
    }

    public void UpdateName(string name)
    {
        if (!_nameInitiated) InitiateName();
        
        _itemNameLabel.Text = name;
    }
    
    
    void OnMouseEntered()
    {
        if (!_nameInitiated) return;
        if (string.IsNullOrWhiteSpace(_itemNameLabel.Text)) return;
        
        Visible = true;
        _nameFollowMouse = true;
    }
    
    void OnMouseExited()
    {
        if (!_nameInitiated) return;

        Visible = false;
        _nameFollowMouse = false;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if (_nameFollowMouse)
        {
            GlobalPosition = GetGlobalMousePosition() + offsetVector;
        }
    }
}