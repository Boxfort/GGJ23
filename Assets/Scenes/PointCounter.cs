using Godot;
using System;

public partial class PointCounter : Control
{
    int points = 0;
    Label label;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        label = GetNode<Label>("Label");
    }

    public void IncrementPoints(int val)
    {
        points += val;
        label.Text = points.ToString("D9");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
