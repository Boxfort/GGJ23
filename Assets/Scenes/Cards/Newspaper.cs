using Godot;
using System;
using System.Collections.Generic;

public partial class Newspaper : DraggableCard
{
    Dictionary<String, String> stories = new Dictionary<String, String>() {
        {"Grizzly Murder Shocks Town", "A brazen broad daylight murder shocks the small town of Oaksville. A man was left dead in the middle of the a busy street after being assaulted with what appeared to be a large taxidermied otter. The suspect fled the scene before being apprehended by police and is still at large. Police are appealing for witnesses at this time. If you have..."},
        {"Candy Store Raided!", "A thief has made off with 293 kilos of confectionary after a daring ram raid on a local sweet shop. The suspect was seen with a large sack scooping up bon-bons and other sugary snacks. Local dentists have praised the move as 'Radical' and a much needed blow to  the sugar industry. Police condemn the statements of the dentists and..."},
        {"Antique Vase Stolen", "An antique vase dating back to the 1600s has been stolen from the museum today, in an act that staff have been quoted as labelling \"not good\". The vase was lifted from the staff break room where it was being used to store used chewing gum and cigarette butts. A janitor told the Oaksville Gazette \"I'm glad it's gone, i'm sick of emptying that darn..."},
        {"Candy Taken From Baby", "A childs lollypop was heartlessly SNATCHED as they played peacefully yesterday afternoon at a local park. Authorities are on the lookout for the suspect who fled on foot. The mayor has been quoted saying that he \"would like to see the death penality\" for the suspect, and the town has been whipped up into a frenzy. A protest has been staged at..."},
        {"Cat Burgled by Cat Burglar", "Yesterday, a cat was abducted from the Maison Moggy cat cafe. The kidnapped kitty was a small ginger cat called George, and is dearly missed by the cafe's owner. Police are appealing for any witnesses who saw a man in the area clutching several cat toys, and who had earlier been seen chasing neighborhood cats. The cafe's owner Sally had...."},
        {"Secret Recipe Stolen", "Restaurant chain OFC has reported that someone has made off with thier secret blend of 48 herbs and spices. A man was seen exiting the building clutching a bundle of papers, and a 16 piece bucket of chicken. Owner, Colonel Jeffery, pleaded with the thief in his statement: \"Please don't get greasy fingerprints all over my fathers recipe sheet\". The suspect... "},
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
