using Godot;
using System;

public partial class AudioController : Node {

    AudioStreamPlayer sfxPlayer;
    AudioStreamPlayer musicPlayer;
    AudioStreamPlayer ambiencePlayer;

    const string soundDirectory = "res://assets/Sounds/";


    public override void _Ready() {
        sfxPlayer = GetNode<AudioStreamPlayer>("GeneralSFX");
        musicPlayer = GetNode<AudioStreamPlayer>("Music");
        ambiencePlayer = GetNode<AudioStreamPlayer>("Ambience");

        PlayMusic("music/music_day.ogg");
        PlayAmbience("ambiences/ambience_day.ogg");
    }


    public void PlayMusic(string soundFile) {
        AudioStream audio = GetAudioFromFile(soundFile);

        musicPlayer.Stream = audio;
        musicPlayer.Play();
    }

    public void PlayAmbience(string soundFile) {
        AudioStream audio = GetAudioFromFile(soundFile);

        ambiencePlayer.Stream = audio;
        ambiencePlayer.Play();
    }


    AudioStream GetAudioFromFile(string soundFile) {
        string fullPath = soundDirectory + soundFile;
        AudioStream audio = (AudioStream)GD.Load(fullPath);

        return audio;
    }
}
