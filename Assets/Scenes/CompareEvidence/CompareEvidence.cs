using Godot;
using System;

enum ComparingState
{
    Disabled,
    NoSelected,
    OneSelected,
    TwoSelected,
    EvidenceMatch,
    EvidenceNotMatch,
    Closing,
}

public partial class CompareEvidence : Control
{
    ComparingState state = ComparingState.Disabled;
    Random rng = new Random();

    Control button;
    Control shadow;
    Highlight highlight1;
    Highlight highlight2;
    RichTextLabel matchLabel;
    RichTextLabel noMatchLabel;
    AudioStreamPlayer typingAudio;
    AudioStreamPlayer dingAudio;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        button = GetNode<Control>("Button");
        button.GuiInput += OnButtonGuiInput;

        shadow = GetNode<Control>("Shadow");
        highlight1 = shadow.GetNode<Highlight>("Highlight");
        highlight2 = shadow.GetNode<Highlight>("Highlight2");

        matchLabel = GetNode<RichTextLabel>("MatchLabel");
        noMatchLabel = GetNode<RichTextLabel>("NoMatchLabel");

        typingAudio = GetNode<AudioStreamPlayer>("TypingAudio");
        dingAudio = GetNode<AudioStreamPlayer>("DingAudio");

        CardGenerator cardGenerator = GetNode<CardGenerator>("../Cards");
        cardGenerator.NewCardsCreated += OnCardsCreated;

        OnCardsCreated();
    }

    private void OnCardsCreated()
    {
        // Setup signals from every bit of evidence
        foreach (Node n in GetTree().GetNodesInGroup("evidence"))
        {
            if (n is SelectableField field)
            {
                field.FieldSelected += OnFieldSelected;
            }
            else
            {
                GD.PushWarning("Node " + n.GetPath() + " has group 'evidence' but is not a 'SelectableField', ignoring...");
            }
        }
    }

    private void OnFieldSelected(SelectableField field)
    {
        switch (state)
        {
            case ComparingState.Disabled:
                return;
            case ComparingState.NoSelected:
                highlight1.SetSelectedField(field);
                highlight1.Show();
                state = ComparingState.OneSelected;
                return;
            case ComparingState.OneSelected:
                highlight2.SetSelectedField(field);
                highlight2.Show();
                state = ComparingState.TwoSelected;
                CompareSelectedEvidence();
                return;
            default:
                return;
        }
    }

    private void OnButtonGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton iemb)
        {
            if (iemb.ButtonIndex == MouseButton.Left && iemb.Pressed)
            {
                if (state == ComparingState.Disabled)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
            else if (iemb.ButtonIndex == MouseButton.Right && iemb.Pressed)
            {
                StepBack();
            }
        }
    }

    private void Start()
    {
        state = ComparingState.NoSelected;
        shadow.Show();
        highlight1.Hide();
        highlight2.Hide();
    }

    private void Stop()
    {
        state = ComparingState.Disabled;
        shadow.Hide();
        highlight1.Hide();
        highlight2.Hide();
        highlight1.UnsetFollowingNode();
        highlight2.UnsetFollowingNode();

        matchLabel.Hide();
        noMatchLabel.Hide();
    }

    private void CompareSelectedEvidence()
    {
        SelectableField field1 = highlight1.GetSelectedField();
        SelectableField field2 = highlight2.GetSelectedField();

        if (field1 == null || field2 == null)
        {
            GD.PrintErr("Attempted to compare a null field");
            Stop();
            return;
        }

        bool isAMatch = field1.CompareWith(field2);

        // Show a random result
        if (isAMatch)
        {
            state = ComparingState.EvidenceMatch;
            matchLabel.VisibleCharacters = 0;
            matchLabel.Show();
        }
        else
        {
            state = ComparingState.EvidenceNotMatch;
            noMatchLabel.VisibleCharacters = 0;
            noMatchLabel.Show();
        }

    }

    private void StepBack()
    {
        switch (state)
        {
            case ComparingState.NoSelected:
                Stop();
                return;
            case ComparingState.OneSelected:
                highlight1.Hide();
                highlight1.UnsetFollowingNode();
                return;
            case ComparingState.TwoSelected:
                return;
            default:
                return;
        }
    }

    double characterDisplayTimeMax = 0.1f;
    double characterDisplayTimeMin = 0.075f;
    double characterDisplayTime = 0.075f;
    double characterDisplayTimer = 0;

    double closingTime = 2f;
    double closingTimer = 0;


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        switch (state)
        {
            case ComparingState.EvidenceMatch:
            case ComparingState.EvidenceNotMatch:
                var activeLabel = state == ComparingState.EvidenceMatch ? matchLabel : noMatchLabel;

                if (characterDisplayTimer >= characterDisplayTime)
                {
                    characterDisplayTimer = 0;
                    if (activeLabel.VisibleCharacters >= activeLabel.GetTotalCharacterCount())
                    {
                        dingAudio.Play();
                        state = ComparingState.Closing;
                        return;
                    }

                    typingAudio.Play();
                    activeLabel.VisibleCharacters += 1;
                    characterDisplayTime = rng.NextDouble() * (characterDisplayTimeMax - characterDisplayTimeMin) + characterDisplayTimeMin;
                }
                else
                {
                    characterDisplayTimer += delta;
                }

                return;
            case ComparingState.Closing:
                if (closingTimer >= closingTime)
                {
                    closingTimer = 0;
                    Stop();
                }
                else
                {
                    closingTimer += delta;
                }

                return;
            default:
                return;
        }
    }
}
