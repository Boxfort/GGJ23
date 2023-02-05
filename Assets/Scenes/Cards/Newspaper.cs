using Godot;
using System;
using System.Collections.Generic;

public partial class Newspaper : DraggableCard
{
    Dictionary<String, String> stories = new Dictionary<String, String>() {
        {"Grizzly Murder Shocks Town", "A brazen broad daylight murder shocks the small town of Oaksville. A man was left dead in the middle of the a busy street after being assaulted with what appeared to be a large taxidermied otter. The suspect fled the scene before being apprehended by police and is still at large. Police are appealing for witnesses at this time. If you have..."},
        {"Candy Store Raided!", "A thief has made off with 293 kilos of confectionary after a daring ram raid on a local sweet shop. The suspect was seen with a large sack scooping up bon-bons and other sugary snacks. Local dentists have praised the move as 'Radical' and a much needed blow to  the sugar industry. Police condemn the statements of the dentists and..."},
    };

    Label headline;
    Label storyText;

    Random rng = new Random();


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        headline = GetNode<Label>("Card/StoryHeadline");
        storyText = GetNode<Label>("Card/StoryText");

        RandomStory();
    }

    private void RandomStory()
    {
        var keys = new List<String>(stories.Keys);
        String headlineText = keys[rng.Next(0, keys.Count)];
        String story = stories[headlineText];

        headline.Text = headlineText;
        storyText.Text = story;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
