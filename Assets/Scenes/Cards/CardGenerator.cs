using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class CardGenerator : Control
{
    [Signal]
    public delegate void NewCardsCreatedEventHandler();

    [Export]
    PackedScene suspectCard;

    [Export]
    PackedScene newspaper;

    [Export]
    PackedScene evidenceCard;

    Random rng = new Random();

    Outbox outbox;

    List<String> firstNames = new List<String>();
    List<String> surnames = new List<String>();

    private List<String> LoadTextFromFile(String filename)
    {
        FileAccess file = FileAccess.Open(filename, FileAccess.ModeFlags.Read);
        List<String> lines = new List<String>();

        while (file.GetPosition() < file.GetLength())
        {
            lines.Add(file.GetLine());
        }

        return lines;
    }

    private List<Clue> evidence = new List<Clue>
    {
        new Clue(FieldType.Evidence, new List<Trait> { Trait.Athletic }, "is a member of the athletics club" ),
        new Clue(FieldType.Evidence, new List<Trait> { Trait.Divorced }, "is recently divorced" ),
        new Clue(FieldType.Evidence, new List<Trait> { Trait.Depressed }, "is sufferring from depression" ),
        new Clue(FieldType.Evidence, new List<Trait> { Trait.Quiet }, "is introverted" ),
        new Clue(FieldType.Evidence, new List<Trait> { Trait.Sick }, "is suffering from a cold" ),
        new Clue(FieldType.Evidence, new List<Trait> { Trait.Injured}, "has an unexplained injury" ),
        new Clue(FieldType.Evidence, new List<Trait> { Trait.Evil }, "has an evil nature" ),
        new Clue(FieldType.Evidence, new List<Trait> { Trait.Relaxed }, "takes things at a relaxed pace" ),
        new Clue(FieldType.Evidence, new List<Trait> { Trait.ShortTemper }, "has a short temper" ),
        new Clue(FieldType.Evidence, new List<Trait> { Trait.Calculating }, "is cold and calculating" ),
        new Clue(FieldType.Evidence, new List<Trait> { Trait.Foreign }, "has dual nationality" ),
        new Clue(FieldType.Evidence, new List<Trait> { Trait.Stylish }, "is a fashion model" )
    };

    private Dictionary<Clue, List<Clue>> evidenceClues = new Dictionary<Clue, List<Clue>> {
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.Tall }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Tall }, "The suspect was tall", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.Short }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Short }, "The suspect was short", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.AverageHeight }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.AverageHeight }, "The suspect was average height", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.Young }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Young }, "The suspect was youthful", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.MiddleAge }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.MiddleAge }, "The suspect was middle aged", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.Old }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Old }, "The suspect appeared old", true),
            }
        },
                {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.Skinny }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Skinny }, "The suspect was skinny", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.AverageWeight }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.AverageWeight }, "The suspect was average weight", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.Fat }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Fat }, "The suspect was fat", true),
                new Clue(FieldType.Appearance, new List<Trait> { Trait.Fat }, "The suspect was overweight", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.Athletic }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Athletic }, "The suspect made a swift getaway", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.HasHat }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.HasHat }, "The suspect was wearing a hat", true),
            }
        },
        {
            new Clue(FieldType.Appearance, new List<Trait> { Trait.HasGlasses }),
            new List<Clue>() {
                new Clue(FieldType.Appearance, new List<Trait> { Trait.HasGlasses }, "The suspect was wearing glasses", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.Divorced }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Divorced }, "The suspect was muttering a womans name", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.Depressed }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Depressed }, "The suspect was seen crying", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.Quiet }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Quiet }, "The suspect would not make eye contact", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.Sick }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Sick }, "The suspect was sneezing heavily", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.Injured }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Injured }, "The suspect was injured during the crime", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.Evil }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Evil }, "The suspect had a crazed look in his eye", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.ShortTemper }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.ShortTemper }, "The suspect seemed furious", true),
                new Clue(FieldType.Evidence, new List<Trait> { Trait.ShortTemper }, "The suspect snapped at onlookers", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.Relaxed }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Relaxed }, "The suspect made no effort to escape quickly", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.Calculating }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Calculating }, "The suspect had prepared thoroughly", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.Foreign }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Foreign}, "The suspect spoke in a foreign language", true),
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Foreign}, "Witnesses did not recognise the language the suspect spoke", true),
            }
        },
        {
            new Clue(FieldType.Evidence, new List<Trait> { Trait.Stylish }),
            new List<Clue>() {
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Stylish }, "Witnesses said the suspect was very stylish", true),
                new Clue(FieldType.Evidence, new List<Trait> { Trait.Stylish }, "The suspect was wearing beautiful clothes", true),
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
        firstNames = LoadTextFromFile("res://Data/firstnames.txt");
        surnames = LoadTextFromFile("res://Data/surnames.txt");

        GenerateCards();
    }

    Vector2 suspectStartLocation = new Vector2(-250, 100);
    Vector2 minSuspectLocation = new Vector2(40, 50);
    Vector2 maxSuspectLocation = new Vector2(400, 260);

    private void GenerateCards()
    {
        AddNewspaper();

        // Generate a suspect
        SuspectInfo actualSuspect = GenerateSuspect();
        actualSuspect.isGuilty = true;

        // Generate x clues based on suspect features
        List<Clue> suspectClues = SelectSuspectClues(actualSuspect);

        CreateSuspectCard(actualSuspect);
        CreateEvidenceCard(suspectClues);

        for (int i = 0; i < 5; i++)
        {
            SuspectInfo innocentSuspect = GenerateSuspect();
            CreateSuspectCard(innocentSuspect);
        }

        EmitSignal(SignalName.NewCardsCreated);
    }

    private void AddNewspaper()
    {
        Newspaper newspaperInstance = (Newspaper)newspaper.Instantiate();
        newspaperInstance.GlobalPosition = new Vector2(596, -270);
        newspaperInstance.RotationDegrees = rng.Next(-90, 90);
        AddChild(newspaperInstance);

        var cardTween = GetTree().CreateTween();
        var cardRotTween = GetTree().CreateTween();
        cardTween.TweenProperty(newspaperInstance, "position", new Vector2(430, 84), 1).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        cardRotTween.TweenProperty(newspaperInstance, "rotation", 0, 0.75).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
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
        var firstName = firstNames[rng.Next(0, firstNames.Count)];
        var lastName = surnames[rng.Next(0, surnames.Count)];
        info.name = firstName + " " + lastName;

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

        // Glasses
        int glasses = rng.Next(0, 2);

        if (glasses == 0)
        {
            info.appearance.Add(new Clue(FieldType.Appearance, new List<Trait> { Trait.HasGlasses }));
        }

        // -- Evidence -- 
        for (int i = 0; i < 3; i++)
        {
        }

        for (int i = 0; i < 3; i++)
        {
            while (true)
            {
                var selectedEvidence = evidence[rng.Next(0, evidence.Count)];
                if (info.evidence.Contains(selectedEvidence)) continue;

                info.evidence.Add(selectedEvidence);
                break;
            }
        }


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

    public void NewRound()
    {
        ClearCards();
        GenerateCards();
    }

    Vector2 clearPoint = new Vector2(320, 640);

    private void ClearCards()
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
        if (IsInstanceValid(c))
            c.QueueFree();
    }
}
