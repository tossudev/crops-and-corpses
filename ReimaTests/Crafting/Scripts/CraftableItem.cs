using Godot;
using System;
using System.Diagnostics;

public partial class CraftableItem : TextureRect
{
    [Export] public Resource itemRes;

    public override void _Ready()
    {
        try
        {
            Item item = (Item)itemRes;
            Texture = item.IconTexture;
        }
        catch (Exception e)
        {
            Debug.Fail(e.Message);
        } 
    }
}
