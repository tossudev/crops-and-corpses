using Godot;
using System;
using System.Diagnostics;

public partial class CraftableItem : Button
{
    [Export] public Item itemRes;
    [Export] public TextureRect smallIcon;

    
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
    }

    public override void _Pressed()
    {
        base._Pressed();
        CraftPanel.instance.OpenPanel(itemRes);
    }
}
