using Godot;
using System;

public partial class Credits : CanvasLayer
{
    MenuOption back;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        back = GetNode<MenuOption>("Back");

        back.OptionPressed += OnBackPressed;
    }

    private void OnBackPressed()
    {
        GetTree().ChangeSceneToFile("res://Assets/Scenes/TitleScreen.tscn");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
