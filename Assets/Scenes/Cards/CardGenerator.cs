using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class CardGenerator : Control
{
    [Signal]
    public delegate void OutboxCardCreatedEventHandler(DraggableCard card);

    [Export]
    PackedScene suspectCard;

    [Export]
    PackedScene evidenceCard;

    Random rng = new Random();

    Outbox outbox;

    private List<Clue> evidence = new List<Clue>
    {
        new Clue(FieldType.Evidence, new List<Trait> { Trait.Athletic }, "is athletic" )
    };

    private Dictionary<Clue, List<Clue>> evidenceClues = new Dictionary<Clue, List<Clue>> {
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.Tall }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Tall }, "The suspect is tall", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.Short }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Short }, "The suspect is short", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.AverageHeight }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.AverageHeight }, "The suspect is average height", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.Young }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Young }, "The suspect is young", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.MiddleAge }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.MiddleAge }, "The suspect is middle aged", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.Old }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Old }, "The suspect is old", true),
            }
        },
                {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.Skinny }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Skinny }, "The suspect is skinny", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.AverageWeight }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.AverageWeight }, "The suspect is average weight", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.Fat }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Fat }, "The suspect is fat", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.Athletic }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Athletic }, "the suspect is athletic", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.HasHat }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.HasHat }, "the suspect was wearing a hat", true),
            }
        },
    };

    private List<Clue> GetEvidenceClues(Clue evidence)
    {
        foreach (Clue key in evidenceClues.Keys)
        {
            if (key.Equals(evidence))
            {
                return evidenceClues[key];
            }
        }

        GD.PrintErr("CLUE NOT FOUND FOR EVIDENCE: " + evidence);

        return null;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GenerateCards();
    }

    Vector2 suspectStartLocation = new Vector2(-250, 100);
    Vector2 minSuspectLocation = new Vector2(40, 50);
    Vector2 maxSuspectLocation = new Vector2(400, 260);

    private void GenerateCards()
    {
        // Generate a suspect
        SuspectInfo actualSuspect = GenerateSuspect();

        // Generate x clues based on suspect features
        List<Clue> suspectClues = SelectSuspectClues(actualSuspect);

        CreateSuspectCard(actualSuspect);
        CreateEvidenceCard(suspectClues);

        // TODO: Generate some innocents
        for (int i = 0; i < 5; i++)
        {
            SuspectInfo innocentSuspect = GenerateSuspect();
            CreateSuspectCard(innocentSuspect);
        }
    }

    private void CreateSuspectCard(SuspectInfo info)
    {
        SuspectCard cardInstance = (SuspectCard)suspectCard.Instantiate();
        AddChild(cardInstance);
        cardInstance.SetInfo(info);
        cardInstance.GlobalPosition = suspectStartLocation;
        cardInstance.RotationDegrees = rng.Next(-90, 90);

        var dest = new Vector2(rng.Next((int)minSuspectLocation.X, (int)maxSuspectLocation.X), rng.Next((int)minSuspectLocation.Y, (int)maxSuspectLocation.Y));

        var cardTween = GetTree().CreateTween();
        var cardRotTween = GetTree().CreateTween();
        cardTween.TweenProperty(cardInstance, "position", dest, 1).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        cardRotTween.TweenProperty(cardInstance, "rotation", 0, 0.75).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);

        EmitSignal(SignalName.OutboxCardCreated, cardInstance);
    }

    private List<Clue> SelectSuspectClues(SuspectInfo actualSuspect)
    {
        List<Clue> selectedClues = new List<Clue>();

        int clueCount = 0;

        int appearanceClues = rng.Next(1, 3);
        for (int i = 0; i < appearanceClues; i++)
        {
            while (true)
            {
                var evidence = actualSuspect.appearance[rng.Next(0, actualSuspect.appearance.Count)];
                var potentialClues = GetEvidenceClues(evidence);
                var selectedClue = potentialClues[rng.Next(0, potentialClues.Count)];

                if (selectedClues.Contains(selectedClue)) continue;

                selectedClues.Add(selectedClue);
                break;
            }
            clueCount++;
        }

        while (clueCount < 3)
        {
            var evidence = actualSuspect.evidence[rng.Next(0, actualSuspect.evidence.Count)];
            var potentialClues = GetEvidenceClues(evidence);
            var selectedClue = potentialClues[rng.Next(0, potentialClues.Count)];
            selectedClues.Add(selectedClue);
            clueCount++;
        }

        return selectedClues;
    }

    private SuspectInfo GenerateSuspect()
    {
        SuspectInfo info = new SuspectInfo();

        // TODO: Generate a name
        info.name = "John Doe";

        // --- HEIGHT ---
        info.height = rng.Next(130, 200);
        if (info.height >= 180)
        {
            info.appearance.Add(new Clue(FieldType.Appearance, new List<Trait> { Trait.Tall }));
        }
        else if (info.height <= 150)
        {
            info.appearance.Add(new Clue(FieldType.Appearance, new List<Trait> { Trait.Short }));
        }
        else
        {
            info.appearance.Add(new Clue(FieldType.Appearance, new List<Trait> { Trait.AverageHeight }));
        }

        // --- DoB ---
        DateTime now = DateTime.Now;
        int year = now.Year - rng.Next(90 - 15);
        info.dob = new DateTime(
            year,
            rng.Next(1, 13),
            rng.Next(1, 28)
        );

        int age = now.Year - year;

        if (age <= 30)
        {
            info.appearance.Add(new Clue(FieldType.Appearance, new List<Trait> { Trait.Young }));
        }
        else if (age >= 60)
        {
            info.appearance.Add(new Clue(FieldType.Appearance, new List<Trait> { Trait.Old }));
        }
        else
        {
            info.appearance.Add(new Clue(FieldType.Appearance, new List<Trait> { Trait.MiddleAge }));
        }

        // -- Appearance -- 
        // Body Type
        int body = rng.Next(0, 3);

        if (body == 0)
        {
            info.appearance.Add(new Clue(FieldType.Appearance, new List<Trait> { Trait.Skinny }));
        }
        else if (body == 1)
        {
            info.appearance.Add(new Clue(FieldType.Appearance, new List<Trait> { Trait.AverageWeight }));
        }
        else if (body == 2)
        {
            info.appearance.Add(new Clue(FieldType.Appearance, new List<Trait> { Trait.Fat }));
        }

        // Headwear
        int headwear = rng.Next(0, 2);

        if (headwear == 0)
        {
            info.appearance.Add(new Clue(FieldType.Appearance, new List<Trait> { Trait.HasHat }));
        }

        // -- Evidence -- 
        info.evidence.Add(evidence[0]);

        return info;
    }

    private void CreateEvidenceCard(List<Clue> suspectClues)
    {
        EvidenceCard evidenceCardInstance = (EvidenceCard)evidenceCard.Instantiate();
        evidenceCardInstance.GlobalPosition = new Vector2(950, 100);
        evidenceCardInstance.RotationDegrees = rng.Next(-90, 90);
        AddChild(evidenceCardInstance);

        var cardTween = GetTree().CreateTween();
        var cardRotTween = GetTree().CreateTween();
        cardTween.TweenProperty(evidenceCardInstance, "position", new Vector2(450, 200), 1).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        cardRotTween.TweenProperty(evidenceCardInstance, "rotation", 0, 0.75).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);

        evidenceCardInstance.SetClues(suspectClues);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public async void NewRound()
    {
        await Task.Run(() => ClearCards());
        GenerateCards();
    }

    Vector2 clearPoint = new Vector2(320, 640);

    private async void ClearCards()
    {
        foreach (Control c in GetChildren())
        {
            if (c.IsQueuedForDeletion()) continue;

            var cardTween = GetTree().CreateTween();
            cardTween.TweenProperty(c, "position", clearPoint, 1).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);

            cardTween.Finished += () => OnCardCleared(c);
        }
    }

    private void OnCardCleared(Control c)
    {
        c.QueueFree();
    }
}
