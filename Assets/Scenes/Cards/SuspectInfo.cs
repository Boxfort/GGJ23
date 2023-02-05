using System;
using System.Collections.Generic;

public class SuspectInfo
{
    public string name;
    public int height;
    public DateTime dob;
    public List<Clue> appearance = new List<Clue>();
    public List<Clue> evidence = new List<Clue>();

    public bool isGuilty = false;
}