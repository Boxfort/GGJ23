using Godot;
using System.Linq;
using System.Collections.Generic;

public partial class SelectableField : Label
{
    [Signal]
    public delegate void FieldSelectedEventHandler(SelectableField field);

    public Clue clue = new Clue(FieldType.Nothing, new List<Trait> { });

    bool validPress = false;

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
                EmitSignal(SignalName.FieldSelected, this);
                validPress = false;
            }
        }

        if (@event is InputEventMouseMotion)
        {
            validPress = false;
        }
    }

    public bool CompareWith(SelectableField otherField)
    {
        if (clue.isEvidence == otherField.clue.isEvidence) return false;

        if (clue.type == FieldType.Nothing || otherField.clue.type == FieldType.Nothing)
        {
            GD.Print("FIELD IS NOTHING");
            return false;
        }
        if (clue.type != otherField.clue.type)
        {
            GD.Print("FIELDS ARE NOT SAME TYPE: " + clue.type + " & " + otherField.clue.type);
            return false;
        }


        if (!otherField.clue.traits.Intersect(clue.traits).Any())
        {
            GD.Print("---");
            GD.Print(string.Join(",", clue.traits));
            GD.Print("FIELD DOES NOT CONTAIN SAME TRAITS");
            GD.Print(string.Join(",", otherField.clue.traits));
            return false;
        }

        return true;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
