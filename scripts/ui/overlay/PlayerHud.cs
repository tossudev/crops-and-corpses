using Godot;
using System;

public partial class PlayerHud : Control {
    
	string _tempLoseText = "You died";

    Label _hpText;

    bool _initialized;

    void Init()
	{
		if (_initialized) return;

		_hpText = GetNode<Label>("%HealthText"); 
		
		_initialized = true;
	}


}
