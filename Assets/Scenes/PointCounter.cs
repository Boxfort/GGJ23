using Godot;
using System;

public partial class PointCounter : Control
{
    int points = 0;
    Label label;
    PopupLabel pointPopup;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        label = GetNode<Label>("Label");

        pointPopup = GetNode<PopupLabel>("PointPopup");
    }

    public void IncrementPoints(int val)
    {
        var sign = val > 0 ? "+" : "";
        pointPopup.Popup(sign + val);

        points += val;
        label.Text = points.ToString("D9");
        SessionStats.score = points;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
