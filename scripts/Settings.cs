using Godot;
using System;

public partial class Settings : Node2D {
	
	HSlider volumeSliderSfx;
	HSlider volumeSliderMusic;
	HSlider volumeSliderAmbience;
	static AudioController _audioController;


	public override void _Ready() {
		_audioController = GetNode<AudioController>("/root/Audio");

		volumeSliderSfx = GetNodeOrNull<HSlider>("AudioSfx/Slider");
		volumeSliderMusic = GetNodeOrNull<HSlider>("AudioMusic/Slider");
		volumeSliderAmbience = GetNodeOrNull<HSlider>("AudioAmbience/Slider");
	}

	void OnFullscreenToggled(bool buttonPressed) {
		if (buttonPressed) {
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		else {
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
		_audioController.PlayEffect("res://assets/Sounds/ui/click.wav");
	}

	void OnSfxChanged(float value) {
		AudioServer.SetBusVolumeDb(1, ConvertVolumeValue(value));
	}

	void OnAmbienceChanged(float value) {
		AudioServer.SetBusVolumeDb(3, ConvertVolumeValue(value));
	}


	float ConvertVolumeValue(float value) {
		return Mathf.Log(value) * 17.3123f;
	}

}
