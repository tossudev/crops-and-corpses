using Godot;
using System;

public partial class MainMenu : Node2D {

	float transitionTime = 1.0f;
	ColorRect transitionOverlay;

	Sprite2D title;
	Camera2D camMain;
	Camera2D camSettings;
	Camera2D camCredits;
	static AudioController _audioController;

	double time = 0.0d;


    public override void _Ready() {
        base._Ready();
		transitionOverlay = GetNodeOrNull<ColorRect>("Overlay/TransitionOverlay");

		title = GetNodeOrNull<Sprite2D>("Main/Title");
		camMain = GetNodeOrNull<Camera2D>("Main/Camera");
		camSettings = GetNodeOrNull<Camera2D>("Settings/Camera");
		camCredits = GetNodeOrNull<Camera2D>("Credits/Camera");
		_audioController = GetNode<Node>("/root/Audio") as AudioController;

		_audioController.StopMusic();
    }


    public override void _Process(double delta) {
        base._Process(delta);
		time += delta;

		// little title animation thingy
		double titleScale = (Mathf.Cos(time * 1.5) / 30) + 0.5;
		title.Scale = new Vector2((float)titleScale, (float)titleScale);
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

	void OnCreditsPressed() {
		Transition("Credits");
	}

	void OnQuitPressed() {
		GetTree().Quit();
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
		camCredits.Enabled = false;

		switch (to) {
			case "Menu":
				camMain.Enabled = true;
				break;
			case "Settings":
				camSettings.Enabled = true;
				break;
			case "Credits":
				camCredits.Enabled = true;
				break;
			case "Game":
				GetTree().ChangeSceneToFile("res://scenes/town.tscn");
				_audioController.PlayBackground();
				return;
		}

		Tween tween_back = GetTree().CreateTween();
        tween_back.TweenProperty(
				transitionOverlay, "color",
				new Color(0f, 0f, 0f, 0f), transitionTime / 4
				);
	}

}
