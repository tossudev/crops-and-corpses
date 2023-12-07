using Godot;
using System;

public partial class PauseMenu : Node2D {

	bool paused = false;

    public override void _Process(double delta) {
        if (Input.IsActionJustPressed("pause")) {
			TogglePause();
		}
    }


	void TogglePause() {
		paused = !paused;
		Visible = paused;

		GetTree().Paused = paused;
	}


	public void OnResumePressed() {
		TogglePause();
	}


	public void OnBackPressed() {
		TogglePause();
		GetTree().ChangeSceneToFile("res://scenes/ui/main_menu.tscn");
	}

}
