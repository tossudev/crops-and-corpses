using Godot;
using System;

[GlobalClass]
public partial class AudioController : Node {

    AudioStreamPlayer _sfxPlayer;
    AudioStreamPlayer _walkPlayer;

    // AudioStreamPlaybackPolyphonic _sfxPlayback;
    
    AudioStreamPlayer _musicPlayer;
    AudioStreamPlayer _ambiencePlayer;

    string[] walkNoises = {
        "res://assets/Sounds/character_sounds/steps/step1.wav",
        "res://assets/Sounds/character_sounds/steps/step2.wav",
        "res://assets/Sounds/character_sounds/steps/step3.wav",
        "res://assets/Sounds/character_sounds/steps/step4.wav",
        "res://assets/Sounds/character_sounds/steps/step5.wav",
    };

    const string soundDirectory = "res://assets/Sounds/";
    const float _runSpeed = 1.5f;


    public override void _Ready() {
        _sfxPlayer = GetNode<AudioStreamPlayer>("GeneralSFX");
        _walkPlayer = GetNode<AudioStreamPlayer>("WalkingSFX");

        // _sfxPlayback = _sfxPlayer.GetStreamPlayback() as AudioStreamPlaybackPolyphonic;

        _musicPlayer = GetNode<AudioStreamPlayer>("Music"); 
        _ambiencePlayer = GetNode<AudioStreamPlayer>("Ambience");

        // PlayMusic("res://assets/Sounds/music/music_day.ogg");
        PlayAmbience("res://assets/Sounds/ambiences/ambience_day.ogg");
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


    public void PlayWalking(bool fast) {
        if (_walkPlayer.Playing) {
            return;
        }

        int _randSoundIndex = (int) GD.RandRange(0.0, walkNoises.Length);
        string soundFileString = walkNoises[_randSoundIndex];
        _walkPlayer.Stream = GetAudioFromFile(soundFileString);

        _walkPlayer.PitchScale = 1f;
        if (fast) {
            _walkPlayer.PitchScale = _runSpeed;
        }

        _walkPlayer.Play();
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
