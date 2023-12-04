using Godot;
using System;

public partial class MainMenu : Node2D {

	void OnPlayPressed() {
		GetTree().ChangeSceneToFile("res://scenes/town.tscn");
	}


	void OnQuitPressed() {
		GetTree().Quit();
	}
}
