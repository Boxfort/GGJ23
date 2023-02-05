using Godot;
using System;

public partial class GameHandler : CanvasLayer
{
    Outbox outbox;
    PointCounter points;
    TimeLimit timeLimit;
    CardGenerator cardGenerator;
    PopupLabel popupLabel;

    AudioStreamPlayer successAudio;
    AudioStreamPlayer goodAudio;
    AudioStreamPlayer failAudio;

    int correctConvictions = 0;
    int wrongfulConviction = 0;
    int innocenceProved = 0;
    int criminalsReleased = 0;

    double timeSinceLastConviction = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        outbox = GetNode<Outbox>("Outbox");
        points = GetNode<PointCounter>("Points");
        timeLimit = GetNode<TimeLimit>("TimeLimit");
        cardGenerator = GetNode<CardGenerator>("Cards");

        popupLabel = GetNode<PopupLabel>("PopupLabel");

        successAudio = GetNode<AudioStreamPlayer>("SuccessAudio");
        goodAudio = GetNode<AudioStreamPlayer>("GoodAudio");
        failAudio = GetNode<AudioStreamPlayer>("FailAudio");

        outbox.SuspectSent += OnSuspectSent;
        timeLimit.TimeLimitReached += OnTimeLimitReached;

        SessionStats.Reset();
    }

    private void OnTimeLimitReached()
    {
        GetTree().ChangeSceneToFile("res://Assets/Scenes/EndScreen.tscn");
    }

    private void BeginNewRound()
    {
        cardGenerator.NewRound();
    }

    private void OnSuspectSent(bool wasGuilty, bool wasStampedGuilty)
    {
        if (wasGuilty && wasStampedGuilty)
        {
            // Successful conviction
            points.IncrementPoints(10000);
            timeLimit.IncrementTimer(20);
            successAudio.Play();

            SessionStats.criminalsCaught += 1;
            correctConvictions += 1;

            popupLabel.Popup("Criminal Convicted!");

            BeginNewRound();
        }
        else if (!wasGuilty && wasStampedGuilty)
        {
            // Wrongful conviction
            points.IncrementPoints(-5000);
            timeLimit.IncrementTimer(-15);
            failAudio.Play();

            SessionStats.innocentsConvicted += 1;
            wrongfulConviction += 1;

            popupLabel.Popup("Wrongful Conviction.");

            BeginNewRound();
        }
        else if (!wasGuilty && !wasStampedGuilty)
        {
            // Innocent
            points.IncrementPoints(1000);
            timeLimit.IncrementTimer(5);
            goodAudio.Play();

            SessionStats.innocentsReleased += 1;
            innocenceProved += 1;

            popupLabel.Popup("Innocent Released!");
        }
        else
        {
            // Let the suspect go
            points.IncrementPoints(-5000);
            timeLimit.IncrementTimer(-15);
            failAudio.Play();

            SessionStats.criminalsReleased += 1;
            criminalsReleased += 1;

            popupLabel.Popup("Criminal Released.");
        }
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        timeSinceLastConviction += delta;
    }
}
