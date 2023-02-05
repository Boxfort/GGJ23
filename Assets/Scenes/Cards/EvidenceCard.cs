using Godot;
using System;
using System.Collections.Generic;

public partial class EvidenceCard : DraggableCard
{
    [Export]
    PackedScene evidenceEntry;

    VBoxContainer evidence;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        evidence = GetNode<VBoxContainer>("Card/EvidenceContainer");
    }

    public void SetClues(List<Clue> clues)
    {
        foreach (Clue c in clues)
        {
            SelectableField entry = (SelectableField)evidenceEntry.Instantiate();
            entry.Text = "- " + c.text;
            entry.clue = c;

            evidence.AddChild(entry);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
