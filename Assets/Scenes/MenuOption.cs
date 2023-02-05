using Godot;
using System;

public partial class MenuOption : Label
{

    [Signal]
    public delegate void OptionPressedEventHandler();

    bool validPress;

    AudioStreamPlayer hoverAudio;

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton iemb)
        {
            if (iemb.ButtonIndex == MouseButton.Left && iemb.Pressed)
            {
                validPress = true;
            }

            if (iemb.ButtonIndex == MouseButton.Left && !iemb.Pressed && validPress)
            {
                GD.Print("Pressed " + Name);
                EmitSignal(SignalName.OptionPressed);
                validPress = false;
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        hoverAudio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private void OnMouseExited()
    {
        Set("theme_override_colors/font_color", "#d08159");
        validPress = false;
    }

    private void OnMouseEntered()
    {
        hoverAudio.Play();
        Set("theme_override_colors/font_color", "#ffecd6");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
