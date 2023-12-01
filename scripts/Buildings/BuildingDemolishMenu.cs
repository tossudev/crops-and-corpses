using Godot;
using System;
using System.Diagnostics;

public partial class BuildingDemolishMenu : Control
{
    Panel _mainPanel;
    const string MAIN_PANEL_NODENAME = "%MainPanel";

    public Label buildingNameLabel;
    const string BUILDING_NAME_LABEL_NODENAME = "%Header";

    Button _closeButton;
    const string CLOSE_BUTTON_NODENAME = "%CloseButton";

    Button _closeMainPanelButton;
    const string MAIN_PANEL_CLOSE_BUTTON_CONTAINER_NODENAME = "%MainCloseButtonContainer";

    Button _demolishButton;
    const string DEMOLISH_BUTTON_NODENAME = "%DemolishButton";

    public string buildingName;

    Label _demolishLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _mainPanel = GetNode<Panel>(MAIN_PANEL_NODENAME);

        buildingNameLabel = GetNode<Label>(BUILDING_NAME_LABEL_NODENAME);

        _demolishButton = GetNode<Button>(DEMOLISH_BUTTON_NODENAME);
        _demolishButton.ButtonUp += Demolish;

        _closeMainPanelButton = GetCloseButton(MAIN_PANEL_CLOSE_BUTTON_CONTAINER_NODENAME);
        _closeMainPanelButton.ButtonUp += CloseMainPanel;

        SetBuildingName();
    }

    public void SetBuildingName()
    {
        buildingNameLabel.Text = buildingName;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            CloseMainPanel();
        }
    }

    void OnBuildingInput(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is not InputEventMouseButton { Pressed: true } mouseEvent) return;

        if (mouseEvent.ButtonIndex == MouseButton.Left)
        {
            OpenMainPanel();
        }
    }

    public void OpenMainPanel()
	{
        _mainPanel.Visible = true;
	}

    public void CloseMainPanel() 
    {
        _mainPanel.Visible = false;
    }

    public void Demolish() 
    {
        GetParent().QueueFree();
    }

    Button GetCloseButton(string containerNodePath)
    {
        return GetNodeOrNull(containerNodePath)?.GetNode<Button>(CLOSE_BUTTON_NODENAME);
    }
}
