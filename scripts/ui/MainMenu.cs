using Godot;
using System;

public partial class MainMenu : Node2D {

	float transitionTime = 1.0f;
	ColorRect transitionOverlay;

	Camera2D camMain;
	Camera2D camSettings;
	Camera2D camCredits;


    public override void _Ready() {
        base._Ready();
		transitionOverlay = GetNodeOrNull<ColorRect>("Overlay/TransitionOverlay");

		camMain = GetNodeOrNull<Camera2D>("Main/Camera");
		camSettings = GetNodeOrNull<Camera2D>("Settings/Camera");
    }


    void OnPlayPressed() {
		Transition("Game");
	}


	void OnBackToMenuPressed() {
		Transition("Menu");
	}
	

	void OnSettingsPressed() {
		Transition("Settings");
	}


	void OnQuitPressed() {
		GetTree().Quit();
	}


	void OnFullscreenToggled(bool buttonPressed) {
		if (buttonPressed) {
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		else {
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
	}


	public async void Transition(string to) {
		Tween tween = GetTree().CreateTween();
        tween.TweenProperty(
				transitionOverlay, "color",
				new Color(0f, 0f, 0f, 1f), transitionTime / 4
				);

		await ToSignal(GetTree().CreateTimer(transitionTime / 2), "timeout");

		camMain.Enabled = false;
		camSettings.Enabled = false;

		switch (to) {
			case "Menu":
				camMain.Enabled = true;
				break;
			case "Settings":
				camSettings.Enabled = true;
				break;
			case "Game":
				GetTree().ChangeSceneToFile("res://scenes/town.tscn");
				return;
		}

		Tween tween_back = GetTree().CreateTween();
        tween_back.TweenProperty(
				transitionOverlay, "color",
				new Color(0f, 0f, 0f, 0f), transitionTime / 4
				);
	}

}
