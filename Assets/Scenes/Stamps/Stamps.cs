using Godot;
using System;
using System.Linq;

public partial class Stamps : Control
{
    Control cardsContainer;

    [Export]
    PackedScene guiltyStampOverlay;

    [Export]
    PackedScene notGuiltyStampOverlay;

    AudioStreamPlayer drawerCloseAudio;
    AudioStreamPlayer drawerOpenAudio;
    Control stampsArrow;
    Vector2 closedPosition = new Vector2(750, 50);
    Vector2 openPosition = new Vector2(550, 50);

    float maxDistance;
    bool isOpen = false;
    double openTime = 1;

    Tween drawerTween;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        cardsContainer = GetNode<Control>("../Cards");
        drawerOpenAudio = GetNode<AudioStreamPlayer>("DrawerOpenAudio");
        drawerCloseAudio = GetNode<AudioStreamPlayer>("DrawerCloseAudio");
        stampsArrow = GetNode<Control>("Arrow");
        Position = closedPosition;
        stampsArrow.GuiInput += OnArrowGuiInput;
        maxDistance = openPosition.DistanceTo(closedPosition);

        GetNode<Stamp>("GuiltyStamp").Stamped += OnStamped;
        GetNode<Stamp>("NotGuiltyStamp").Stamped += OnStamped;
    }

    private void OnStamped(StampType stampType)
    {
        Vector2 stampLocation = Vector2.Zero;

        if (stampType == StampType.Guilty)
        {
            var node = GetNode<Control>("GuiltyHighlight");
            stampLocation = node.GlobalPosition + node.PivotOffset;
        }
        else
        {
            var node = GetNode<Control>("NotGuiltyHighlight");
            stampLocation = node.GlobalPosition + node.PivotOffset;
        }

        GD.Print("Stamping " + stampType + " at " + stampLocation);

        // Find out which cards are under the stamp position
        foreach (Node n in cardsContainer.GetChildren().Reverse())
        {
            if (n is DraggableCard card)
            {
                bool inXBounds = (stampLocation.X < card.GlobalPosition.X + card.Size.X) && (stampLocation.X > card.GlobalPosition.X);
                bool inYBounds = (stampLocation.Y < card.GlobalPosition.Y + card.Size.Y) && (stampLocation.Y > card.GlobalPosition.Y);

                if (inXBounds && inYBounds)
                {
                    // Stamp him
                    Control overlayInstance = (Control)(stampType == StampType.Guilty ? guiltyStampOverlay.Instantiate() : notGuiltyStampOverlay.Instantiate());

                    //TODO: Fix this hack
                    card.GetNode<Control>("Card").AddChild(overlayInstance);
                    card.StampedWith = stampType;

                    overlayInstance.GlobalPosition = stampLocation - overlayInstance.PivotOffset;

                    GD.Print("Stamped " + card.Name);
                    return;
                }
            }
        }

        GD.Print("Nothing to stamp.");
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

                Vector2 destination = isOpen ? openPosition : closedPosition;
                double actualOpenTime = openTime * (Position.DistanceTo(destination) / maxDistance);
                drawerTween.TweenProperty(this, "position", destination, actualOpenTime).SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.InOut);

                if (isOpen)
                {
                    drawerCloseAudio.Stop();
                    drawerOpenAudio.Play();
                }
                else
                {
                    drawerOpenAudio.Stop();
                    drawerCloseAudio.Play();
                }
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
}
