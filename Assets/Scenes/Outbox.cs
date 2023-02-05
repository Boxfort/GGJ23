using Godot;
using System;

public partial class Outbox : Control
{
    [Signal]
    public delegate void SuspectSentEventHandler(bool wasGuilty, bool wasStampedGuilty);

    AudioStreamPlayer drawerCloseAudio;
    AudioStreamPlayer drawerOpenAudio;

    CardGenerator cardGenerator;

    Control outboxArrow;

    Vector2 inPosition = new Vector2(30, 535);
    Vector2 outPosition = new Vector2(30, 380);

    float maxDistance;
    bool isOpen = false;
    double openTime = 1;

    Tween drawerTween;
    DraggableCard cardInTray;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        drawerOpenAudio = GetNode<AudioStreamPlayer>("DrawerOpenAudio");
        drawerCloseAudio = GetNode<AudioStreamPlayer>("DrawerCloseAudio");

        outboxArrow = GetNode<Control>("Arrow");
        outboxArrow.GuiInput += OnArrowGuiInput;
        maxDistance = inPosition.DistanceTo(outPosition);
        Position = inPosition;

        CardGenerator cardGenerator = GetNode<CardGenerator>("../Cards");
        cardGenerator.NewCardsCreated += OnCardsCreated;
    }

    private void OnCardsCreated()
    {
        foreach (Node n in GetTree().GetNodesInGroup("outbox_card"))
        {
            if (n is DraggableCard card)
            {
                card.CardDropped += OnCardDropped;
            }
            else
            {
                GD.PrintErr("Node " + n.GetPath() + " is in group 'outbox_card' but is not type 'DraggableCard', ignoring...");
            }
        }
    }

    private void OnCardDropped(DraggableCard card, Vector2 atPosition)
    {
        if (!isOpen || card.StampedWith == null) return;

        bool inXBounds = (atPosition.X < GlobalPosition.X + Size.X) && (atPosition.X > GlobalPosition.X);
        bool inYBounds = (atPosition.Y < GlobalPosition.Y + Size.Y) && (atPosition.Y > GlobalPosition.Y);

        if (inXBounds && inYBounds)
        {
            // Put card in tray
            cardInTray = card;
            cardInTray.CardPickedUp += OnTrayCardPickedUp;
            cardInTray.GlobalPosition = (GlobalPosition + PivotOffset) - cardInTray.PivotOffset;
        }
    }

    private void OnTrayCardPickedUp()
    {
        cardInTray.CardPickedUp -= OnTrayCardPickedUp;
        cardInTray = null;
    }

    private void OnArrowGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton iemb)
        {
            if (iemb.ButtonIndex == MouseButton.Left && iemb.Pressed)
            {
                isOpen = !isOpen;
                if (drawerTween != null)
                {
                    drawerTween.Kill();
                }
                drawerTween = GetTree().CreateTween();

                Vector2 destination = isOpen ? outPosition : inPosition;
                double actualOpenTime = openTime * (Position.DistanceTo(destination) / maxDistance);

                drawerTween
                    .TweenProperty(this, "position", destination, actualOpenTime)
                    .SetTrans(Tween.TransitionType.Elastic)
                    .SetEase(Tween.EaseType.InOut);


                if (isOpen)
                {
                    drawerCloseAudio.Stop();
                    drawerOpenAudio.Play();
                }
                else
                {
                    drawerOpenAudio.Stop();
                    drawerCloseAudio.Play();
                    drawerTween.Finished += OnDrawerClosed;
                }
            }
        }
    }

    private void OnDrawerClosed()
    {
        if (cardInTray != null)
        {
            EmitSignal(SignalName.SuspectSent, ((SuspectCard)cardInTray).isGuilty, cardInTray.StampedWith == StampType.Guilty);

            cardInTray.QueueFree();
            cardInTray = null;
            // TODO: This card is now submitted
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (cardInTray != null)
        {
            // Follow tray
            cardInTray.GlobalPosition = (GlobalPosition + PivotOffset) - cardInTray.PivotOffset;
        }
    }
}
