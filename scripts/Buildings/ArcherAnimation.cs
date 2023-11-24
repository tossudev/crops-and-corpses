using Godot;
using System;

public partial class ArcherAnimation : Skeleton2D
{
    AnimationPlayer _animPlayer;

    [Export]
    bool _isShooting;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

    }

    public void ShootAnimation(float TimePerShot)
    {
        _animPlayer.Play("Hands");

        float animationLength = (float)_animPlayer.CurrentAnimationLength;

        _animPlayer.SpeedScale = animationLength / TimePerShot;
    }

    public float RealAnimationLength()
    {
        return (float)_animPlayer.CurrentAnimationLength / _animPlayer.SpeedScale;
    }
    public void StopAnimations()
    {
        _animPlayer.Stop();
    }

}
