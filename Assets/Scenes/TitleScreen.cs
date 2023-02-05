using Godot;
using System;

public partial class TitleScreen : CanvasLayer
{

    MenuOption playOption;
    MenuOption creditsOption;
    MenuOption exitOption;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        playOption = GetNode<MenuOption>("Play");
        creditsOption = GetNode<MenuOption>("Credits");
        exitOption = GetNode<MenuOption>("Exit");

        playOption.OptionPressed += OnPlayPressed;
        creditsOption.OptionPressed += OnCreditsPressed;
        exitOption.OptionPressed += OnExitPressed;
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }

    private void OnCreditsPressed()
    {
        GetTree().ChangeSceneToFile("res://Assets/Scenes/Credits.tscn");
    }

    private void OnPlayPressed()
    {
        GetTree().ChangeSceneToFile("res://Assets/Scenes/GameScene.tscn");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
