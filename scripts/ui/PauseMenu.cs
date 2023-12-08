using Godot;
using System;

public partial class PauseMenu : Node2D {

	bool paused = false;
	Settings settings;


    public override void _Ready() {
        settings = GetNodeOrNull<Node2D>("Settings") as Settings;
    }


    public override void _Process(double delta) {
        if (Input.IsActionJustPressed("pause")) {
			TogglePause();
		}
    }


	void TogglePause() {
		paused = !paused;
		
		if (paused) {
			settings.LoadSettings();
		}

		else {
			settings.SaveSettings();
		}

		Visible = paused;

		GetTree().Paused = paused;
	}


	public void OnResumePressed() {
		TogglePause();
	}


	public void OnBackPressed() {
		TogglePause();

        if (SceneManager.IsCurrentScene(this, Scene.Town))
        {
            BuildingMenu.buildMenu?.SaveBuildings();
        }

        GetTree().ChangeSceneToFile("res://scenes/ui/main_menu.tscn");
	}

}
