using Godot;
using System;

public partial class PopupLabel : Label
{
    [Export]
    Vector2 startPosition = new Vector2(270, 360);

    [Export]
    Vector2 endPosition = new Vector2(270, 288);

    [Export]
    float tweenTime = 1.0f;

    Tween opacityTween;
    Tween moveTween;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Position = startPosition;
        SetTransparent();
    }

    public void Popup(String withText)
    {
        Text = withText;

        SetTransparent();
        Show();

        if (opacityTween != null) opacityTween.Kill();
        if (moveTween != null) moveTween.Kill();

        Position = startPosition;
        opacityTween = GetTree().CreateTween();

        opacityTween
            .TweenProperty(this, "modulate", new Color(1, 1, 1, 1), tweenTime / 3);

        moveTween = GetTree().CreateTween();

        moveTween
            .TweenProperty(this, "position", endPosition, tweenTime)
            .SetTrans(Tween.TransitionType.Bounce)
            .SetEase(Tween.EaseType.Out);

        moveTween
            .TweenProperty(this, "modulate", new Color(1, 1, 1, 0), 0.1f);

        moveTween.Finished += OnTweenFinished;
    }

    private void OnTweenFinished()
    {
        if (opacityTween != null) opacityTween.Kill();
        if (moveTween != null) moveTween.Kill();

        SetTransparent();
        Hide();
    }

    private void SetTransparent()
    {
        var mod = Modulate;
        mod.A = 0;
        Modulate = mod;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
