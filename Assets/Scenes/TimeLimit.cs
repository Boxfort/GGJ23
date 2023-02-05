using Godot;
using System;

public partial class TimeLimit : Control
{
    [Signal]
    public delegate void TimeLimitReachedEventHandler();

    int timeLeft = 300;

    Timer timer;
    Label label;
    PopupLabel timePopup;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        timer = GetNode<Timer>("Timer");
        timePopup = GetNode<PopupLabel>("TimePopup");

        timer.Timeout += OnTimerTimeout;
    }

    private void OnTimerTimeout()
    {
        if (timeLeft == 0)
        {
            EmitSignal(SignalName.TimeLimitReached);
            timer.Autostart = false;
            timer.Stop();
        }

        SessionStats.time += 1;
        timeLeft--;
        label.Text = String.Format("{0}:{1}", Mathf.FloorToInt(timeLeft / 60).ToString("D2"), (timeLeft - (Mathf.FloorToInt(timeLeft / 60) * 60)).ToString("D2"));
    }

    public void IncrementTimer(int seconds)
    {
        var sign = seconds > 0 ? "+" : "";
        timePopup.Popup(sign + seconds);

        timeLeft += seconds;
        label.Text = String.Format("{0}:{1}", Mathf.FloorToInt(timeLeft / 60).ToString("D2"), (timeLeft - (Mathf.FloorToInt(timeLeft / 60) * 60)).ToString("D2"));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
