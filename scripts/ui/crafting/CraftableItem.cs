using Godot;
using System;
using System.Diagnostics;

public partial class CraftableItem : Button
{
    [Export] public Item itemRes;
    [Export] public TextureRect smallIcon;

    CraftPanel _craftPanel;
    public override void _Ready()
    {
        try
        {
            smallIcon.Texture = itemRes.IconTexture;
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }

        _craftPanel = (CraftPanel)GetTree().GetNodesInGroup(CraftPanel.GROUP_NAME)[0];
    }

    public override void _Pressed()
    {
        base._Pressed();
        _craftPanel.OpenPanel(itemRes);
    }
}
