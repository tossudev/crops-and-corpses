using Godot;
using System;
using System.Diagnostics;

public partial class CraftableItem : Button
{
    [Export] public Item itemRes;

    TextureRect smallIcon;
    const string SMALL_ICON_NODENAME = "%CraftableItemIcon";
    
    FloatingButtonName _floatingNamePanel;
    
    CraftPanel _craftPanel;
    public override void _Ready()
    {
        try
        {
            smallIcon = GetNode<TextureRect>(SMALL_ICON_NODENAME);
            smallIcon.Texture = itemRes.IconTexture;
            
            _floatingNamePanel = GetNode<FloatingButtonName>(FloatingButtonName.FLOATING_NAME_NODENAME);
            _floatingNamePanel.UpdateName(itemRes.Name);
            _craftPanel = (CraftPanel)GetTree().GetNodesInGroup(CraftPanel.GROUP_NAME)[0];
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }
    }
    
    public override void _Pressed()
    {
        base._Pressed();
        if (_craftPanel == null)
        {
            GD.PrintErr("CraftPanel not set");
            return;
        }
        
        _craftPanel.OpenPanel(itemRes);
    }
}
