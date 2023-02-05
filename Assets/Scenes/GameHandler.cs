using Godot;
using System;

public partial class GameHandler : CanvasLayer
{
    Outbox outbox;
    PointCounter points;
    TimeLimit timeLimit;
    CardGenerator cardGenerator;

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

        outbox.SuspectSent += OnSuspectSent;
        timeLimit.TimeLimitReached += OnTimeLimitReached;
    }

    private void OnTimeLimitReached()
    {
        // TODO: End the game
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
            timeLimit.IncrementTimer(30);

            correctConvictions += 1;

            BeginNewRound();
        }
        else if (!wasGuilty && wasStampedGuilty)
        {
            // Wrongful conviction
            points.IncrementPoints(-5000);
            timeLimit.IncrementTimer(-15);

            wrongfulConviction += 1;

            BeginNewRound();
        }
        else if (!wasGuilty && !wasStampedGuilty)
        {
            // Innocent
            points.IncrementPoints(1000);
            timeLimit.IncrementTimer(5);

            innocenceProved += 1;
        }
        else
        {
            // Let the suspect go
            points.IncrementPoints(-5000);
            timeLimit.IncrementTimer(-15);

            criminalsReleased += 1;
        }
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        timeSinceLastConviction += delta;
    }
}
