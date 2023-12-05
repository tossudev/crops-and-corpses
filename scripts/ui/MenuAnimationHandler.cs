using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class MenuAnimationHandler : Node {

    float normalScale = 1f;
    float bigScale = 1.1f;
    float pressedScale = 0.8f;
    float scaleTime = 0.6f;
    static AudioController _audioController;



    public override void _Ready() {
        base._Ready();
        _audioController = GetNode<AudioController>("/root/Audio");

        foreach (Button button in GetTree().GetNodesInGroup("Buttons")) {
            button.MouseEntered += () => OnButtonMouseEntered(button);
            button.MouseExited += () => OnButtonMouseExited(button);

            button.ButtonDown += () => OnButtonDown(button);
            button.ButtonUp += () => OnButtonUp(button);
        }
    }


    public void OnButtonMouseEntered(Button button) {
        ScaleAnimation(button, bigScale);
    }

    public void OnButtonMouseExited(Button button) {
        ScaleAnimation(button, normalScale);
    }

    public void OnButtonDown(Button button) {
        ScaleAnimation(button, pressedScale);
    }

    public void OnButtonUp(Button button) {
        ScaleAnimation(button, bigScale);
        _audioController.PlayEffect("res://assets/Sounds/ui/click.wav");
    }





    void ScaleAnimation(Node node, float scale) {
        Tween tween = GetTree().CreateTween();

        Vector2 newScale = new Vector2(scale, scale);

        tween.TweenProperty(node, "scale", newScale, scaleTime)
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.Out);
        // tween.TweenCallback(Callable.From(GetNode("Sprite").QueueFree));
    }

}
