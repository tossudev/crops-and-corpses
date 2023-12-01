using Godot;
using System;

[GlobalClass]
public partial class AudioController : Node {

    AudioStreamPlayer _sfxPlayer;
    AudioStreamPlaybackPolyphonic _sfxPlayback;
    
    AudioStreamPlayer _musicPlayer;
    AudioStreamPlayer _ambiencePlayer;

    const string soundDirectory = "res://assets/Sounds/";


    public override void _Ready() {
        _sfxPlayer = GetNode<AudioStreamPlayer>("GeneralSFX");
        _sfxPlayback = _sfxPlayer.GetStreamPlayback() as AudioStreamPlaybackPolyphonic;
        _musicPlayer = GetNode<AudioStreamPlayer>("Music"); 
        _ambiencePlayer = GetNode<AudioStreamPlayer>("Ambience");

        
        // PlayMusic("music/music_day.ogg");
        // PlayAmbience("ambiences/ambience_day.ogg");
    }


    public void PlayMusic(string soundFile) {

        _musicPlayer.Stream = GetAudioFromFile(soundFile);
        _musicPlayer.Play();
    }

    public void PlayAmbience(string soundFile) {
        
        _ambiencePlayer.Stream = GetAudioFromFile(soundFile);
        _ambiencePlayer.Play();
    }
    
    public async void PlayEffect(string soundFile) {
        Node mainPlayer = _sfxPlayer as Node;
        AudioStreamPlayer playerDuplicate = mainPlayer.Duplicate() as AudioStreamPlayer;

        AddChild(playerDuplicate);
        AudioStreamPlaybackPolyphonic duplicatePlayback = playerDuplicate.GetStreamPlayback() as AudioStreamPlaybackPolyphonic;
        
        duplicatePlayback.PlayStream(GetAudioFromFile(soundFile));

        await ToSignal(GetTree().CreateTimer(15.0), "timeout");

        if (playerDuplicate != null) {
            playerDuplicate.QueueFree();
        }
    }


    AudioStream GetAudioFromFile(string soundFile) {
        string fullPath = soundFile;
            
        AudioStream audio = (AudioStream) FileLoader.LoadCustomResource(fullPath);

        return audio;
    }
}

public class ZombieNoises
{
    ZombieNoises(string value) { Value = value; }

    public string Value { get; }

    public static ZombieNoises ZOMBIE_HISS_1 => new("res://assets/Sounds/zombies/zombie_hiss1.wav");
    public static ZombieNoises ZOMBIE_HISS_2 => new("res://assets/Sounds/zombies/zombie_hiss2.wav");
    public static ZombieNoises ZOMBIE_GROWL => new("res://assets/Sounds/zombies/zombie_growl.wav");
    public static ZombieNoises ZOMBIE_BREATH => new("res://assets/Sounds/zombies/zombie_breath.wav");

    public override string ToString()
    {
        return Value;
    }
}