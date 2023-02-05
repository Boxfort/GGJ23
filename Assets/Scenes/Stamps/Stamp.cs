using Godot;
using System;

public enum StampType
{
    Guilty,
    NotGuilty
}

public partial class Stamp : TextureRect
{
    [Signal]
    public delegate void StampedEventHandler(StampType stampType);

    [Export]
    StampType stampType = StampType.NotGuilty;

    AudioStreamPlayer stampAudio;
    Vector2 startPosition;
    Vector2 pushedPosition;
    Vector2 pushOffset = new Vector2(0, 40);
    Tween stampTween;
    double stampTime = 0.1;

    bool hasBeenPushed = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        stampAudio = GetNode<AudioStreamPlayer>("StampAudio");
        startPosition = Position;
        pushedPosition = Position + pushOffset;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton iemb)
        {
            if (iemb.ButtonIndex == MouseButton.Left && iemb.Pressed)
            {
                if (hasBeenPushed)
                {
                    return;
                }

                stampTween = GetTree().CreateTween();
                stampTween.TweenProperty(this, "position", pushedPosition, stampTime).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
                hasBeenPushed = true;
                stampTween.Finished += OnFirstTweenFinished;
            }
        }
    }

    private void OnFirstTweenFinished()
    {
        EmitSignal(SignalName.Stamped, (int)stampType);
        stampAudio.Play();
        stampTween.Kill();
        stampTween = GetTree().CreateTween();
        stampTween.TweenProperty(this, "position", startPosition, stampTime * 2).SetEase(Tween.EaseType.In);
        stampTween.Finished += OnSecondTweenFinished;
    }

    private void OnSecondTweenFinished()
    {
        stampTween.Kill();
        hasBeenPushed = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}