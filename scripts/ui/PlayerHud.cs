using Godot;
using System;

public partial class PlayerHud : Control {
	
	public ProgressBar PlayerHealth;
	public CharacterBody2D player;
	public HealthComponent playerHealthComponent;

	string _tempLoseText = "get fucked looooll";


    public override void _Ready() {
		PlayerHealth = GetNode<ProgressBar>("PlayerHealth");

		foreach (CharacterBody2D playerNode in GetTree().GetNodesInGroup("player")) {
			player = playerNode;
			playerHealthComponent = player.GetNode<HealthComponent>("HealthComponent");
		}
    }


    public override void _Process(double delta) {
        _UpdateHud();
    }


	void _UpdateHud() {
		float healthValue = playerHealthComponent.health;
		PlayerHealth.Value = healthValue;

		Label hpLabel = PlayerHealth.GetNode<Label>("Text");
		hpLabel.Text = healthValue.ToString();

		if (healthValue <= 0) {
			hpLabel.Text = _tempLoseText;
		}
	}
}
