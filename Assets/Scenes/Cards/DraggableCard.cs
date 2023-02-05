using Godot;
using System;

public partial class DraggableCard : Control
{
    [Signal]
    public delegate void CardDroppedEventHandler(DraggableCard card, Vector2 atPosition);

    [Signal]
    public delegate void CardPickedUpEventHandler();

    AudioStreamPlayer pickupAudio;
    AudioStreamPlayer putDownAudio;

    Vector2? drag_point;

    StampType? stampedWith;

    public StampType? StampedWith { get => stampedWith; set => stampedWith = value; }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton iemb)
        {
            if (iemb.ButtonIndex == MouseButton.Left)
            {
                if (iemb.Pressed)
                {
                    // Grab it.
                    drag_point = GetGlobalMousePosition() - GetScreenPosition();
                    MoveToFront();

                    EmitSignal(SignalName.CardPickedUp);

                    if (pickupAudio != null) pickupAudio.Play();
                }
                else
                {
                    // Release it.
                    drag_point = null;

                    EmitSignal(SignalName.CardDropped, this, GetGlobalMousePosition());

                    if (putDownAudio != null) putDownAudio.Play();
                }
            }
        }

        if (@event is InputEventMouseMotion && drag_point != null)
        {
            Position = GetGlobalMousePosition() - drag_point.Value;
        }

    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        pickupAudio = GetNode<AudioStreamPlayer>("PickupAudio");
        putDownAudio = GetNode<AudioStreamPlayer>("PutDownAudio");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
