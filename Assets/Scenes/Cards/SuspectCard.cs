using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SuspectCard : DraggableCard
{
    [Export]
    PackedScene evidenceEntry;

    SelectableField nameValue;
    SelectableField dobValue;
    SelectableField heightValue;
    SelectableField mugshotField;
    VBoxContainer evidence;

    // Appearance Toggles
    Control skinny;
    Control average;
    Control fat;
    Control hat;
    Control glasses;

    public bool isGuilty = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        nameValue = GetNode<SelectableField>("Card/Name_value");
        dobValue = GetNode<SelectableField>("Card/Age_value");
        heightValue = GetNode<SelectableField>("Card/Height_value");
        mugshotField = GetNode<SelectableField>("Card/MugShotContainer/MugshotField");
        evidence = GetNode<VBoxContainer>("Card/EvidenceContainer");

        skinny = mugshotField.GetNode<Control>("MugShotSkinny");
        average = mugshotField.GetNode<Control>("MugShot");
        fat = mugshotField.GetNode<Control>("MugShotFat");
        hat = mugshotField.GetNode<Control>("Hat");
        glasses = mugshotField.GetNode<Control>("Glasses");
    }

    public void SetInfo(SuspectInfo info)
    {
        isGuilty = info.isGuilty;

        nameValue.Text = info.name;
        dobValue.Text = String.Format("{0}-{1}-{2}", info.dob.Day.ToString("D2"), info.dob.Month.ToString("D2"), info.dob.Year.ToString("D4"));
        heightValue.Text = String.Format("{0}cm", info.height.ToString("D3"));

        foreach (Clue c in info.appearance)
        {
            if (c.traits.Intersect(new List<Trait> { Trait.Tall, Trait.AverageHeight, Trait.Short }).Any())
            {
                heightValue.clue = c;
            }
            else if (c.traits.Intersect(new List<Trait> { Trait.Young, Trait.Old, Trait.MiddleAge }).Any())
            {
                dobValue.clue = c;
            }
            else if (c.traits.Intersect(new List<Trait> { Trait.Skinny, Trait.AverageWeight, Trait.Fat, Trait.HasHat, Trait.HasGlasses }).Any())
            {
                if (c.traits.Contains(Trait.HasHat))
                {
                    hat.Show();
                }

                if (c.traits.Contains(Trait.HasGlasses))
                {
                    glasses.Show();
                }

                if (c.traits.Contains(Trait.Skinny))
                {
                    skinny.Show();
                }
                else if (c.traits.Contains(Trait.Fat))
                {
                    fat.Show();
                }
                else if (c.traits.Contains(Trait.AverageWeight))
                {
                    average.Show();
                }

                if (mugshotField.clue.type == FieldType.Nothing)
                {
                    mugshotField.clue = c;
                }
                else
                {
                    mugshotField.clue.traits = mugshotField.clue.traits.Concat(c.traits).ToList();
                }
            }
        }

        foreach (Clue c in info.evidence)
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
