using Godot;
using System;

public partial class EndScreen : CanvasLayer
{
    MenuOption play;
    MenuOption credits;
    MenuOption exit;

    Label score;
    Label time;
    Label criminalsConvicted;
    Label innocentsCleared;
    Label wrongfulConvictions;
    Label criminalsCleared;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        play = GetNode<MenuOption>("Play");
        credits = GetNode<MenuOption>("Credits");
        exit = GetNode<MenuOption>("Exit");

        play.OptionPressed += OnPlayPressed;
        credits.OptionPressed += OnCreditsPresed;
        exit.OptionPressed += OnExitPressed;

        score = GetNode<Label>("Stats/ScoreContainer/Value");
        time = GetNode<Label>("Stats/TimeContainer/Value");
        criminalsConvicted = GetNode<Label>("Stats/CriminalsContainer/Value");
        innocentsCleared = GetNode<Label>("Stats/InnocentsContainer/Value");
        wrongfulConvictions = GetNode<Label>("Stats/FalsePositiveContainer/Value");
        criminalsCleared = GetNode<Label>("Stats/FalseNegativeContainer/Value");

        score.Text = SessionStats.score.ToString("D9");
        time.Text = String.Format("{0}:{1}", Mathf.FloorToInt(SessionStats.time / 60).ToString("D2"), (SessionStats.time - (Mathf.FloorToInt(SessionStats.time / 60) * 60)).ToString("D2"));
        criminalsConvicted.Text = SessionStats.criminalsCaught.ToString();
        innocentsCleared.Text = SessionStats.innocentsReleased.ToString();
        wrongfulConvictions.Text = SessionStats.innocentsConvicted.ToString();
        criminalsCleared.Text = SessionStats.criminalsReleased.ToString();
    }

    private void OnPlayPressed()
    {
        GetTree().ChangeSceneToFile("res://Assets/Scenes/GameScene.tscn");
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }

    private void OnCreditsPresed()
    {
        GetTree().ChangeSceneToFile("res://Assets/Scenes/Credits.tscn");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
