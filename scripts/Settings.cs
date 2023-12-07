using Godot;
using System;

public partial class Settings : Node2D {

	const string SettingsFilePath = "user://saves/Settings.cfg";

	HSlider volumeSliderSfx;
	HSlider volumeSliderMusic;
	HSlider volumeSliderAmbience;
	CheckButton fullscreenButton;
	static AudioController _audioController;

	bool fullscreen;
	float volumeSfx;
	float volumeMusic;
	float volumeAmbience;


	public override void _Ready() {
		_audioController = GetNode<AudioController>("/root/Audio");

		volumeSliderSfx = GetNodeOrNull<HSlider>("AudioSfx/Slider");
		volumeSliderMusic = GetNodeOrNull<HSlider>("AudioMusic/Slider");
		volumeSliderAmbience = GetNodeOrNull<HSlider>("AudioAmbience/Slider");
		fullscreenButton = GetNodeOrNull<CheckButton>("Fullscreen/Button");
	
		LoadSettings();
	}


	void InitSettings() {
		fullscreen = false;
		volumeSfx = 0.6f;
		volumeAmbience = 0.6f;
		volumeMusic = 0.6f;

		SaveSettings();
	}


	void SaveSettings() {
		var settings = new ConfigFile();

		settings.SetValue("Settings", "fullscreen", fullscreen);
		settings.SetValue("Settings", "volumeSfx", volumeSfx);
		settings.SetValue("Settings", "volumeMusic", volumeMusic);
		settings.SetValue("Settings", "volumeAmbience", volumeAmbience);

		settings.Save(SettingsFilePath);
	}


	void LoadSettings() {
		var settings = new ConfigFile();

		Error err = settings.Load(SettingsFilePath);

		// If the file doesn't exist, make it
		if (err != Error.Ok) {
			InitSettings();
			return;
		}

		fullscreen = (bool)settings.GetValue("Settings", "fullscreen");
		volumeSfx = (float)settings.GetValue("Settings", "volumeSfx");
		volumeMusic = (float)settings.GetValue("Settings", "volumeMusic");
		volumeAmbience = (float)settings.GetValue("Settings", "volumeAmbience");

		AudioServer.SetBusVolumeDb(1, ConvertVolumeValue(volumeSfx));
		AudioServer.SetBusVolumeDb(2, ConvertVolumeValue(volumeMusic));
		AudioServer.SetBusVolumeDb(3, ConvertVolumeValue(volumeAmbience));
		ToggleFullscreen();

		UpdateSettingsUI();
	}


	void UpdateSettingsUI() {
		fullscreenButton.ButtonPressed = fullscreen;

		volumeSliderSfx.Value = volumeSfx;
		volumeSliderMusic.Value = volumeMusic;
		volumeSliderAmbience.Value = volumeAmbience;
	}


	void OnFullscreenToggled(bool buttonPressed) {
		fullscreen = buttonPressed;
		ToggleFullscreen();
		_audioController.PlayEffect("ui/click.wav");
	}

	void ToggleFullscreen() {
		if (fullscreen) {
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		else {
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
	}


	void OnSettingsSaved() {
		SaveSettings();
	}


	void OnSfxChanged(float value) {
		volumeSfx = value;
		AudioServer.SetBusVolumeDb(1, ConvertVolumeValue(value));
	}

	void OnMusicChanged(float value) {
		volumeMusic = value;
		AudioServer.SetBusVolumeDb(2, ConvertVolumeValue(value));
	}

	void OnAmbienceChanged(float value) {
		volumeAmbience = value;
		AudioServer.SetBusVolumeDb(3, ConvertVolumeValue(value));
	}


	float ConvertVolumeValue(float value) {
		return Mathf.Log(value) * 17.3123f;
	}

}
