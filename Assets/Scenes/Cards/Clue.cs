using System;
using System.Linq;
using System.Collections.Generic;

public enum FieldType
{
    Nothing,
    Name,
    Dob,
    Height,
    Evidence,
    Appearance
}

public enum Trait
{
    Fancy,
    Scruffy,
    Skinny,
    AverageWeight,
    Fat,
    Tall,
    Short,
    AverageHeight,
    Old,
    Young,
    MiddleAge,
    HasHat,
    Athletic
}

public class Clue
{
    public FieldType type;
    public List<Trait> traits = new List<Trait>();
    public String text = "";
    public bool isEvidence = false;

    public Clue(FieldType type, List<Trait> traits, string text = "", bool isEvidence = false)
    {
        this.type = type;
        this.traits = traits;
        this.text = text;
        this.isEvidence = isEvidence;
    }

    public override bool Equals(object obj)
    {
        if (obj is Clue clue)
        {
            if (type != clue.type) return false;
            if (isEvidence != clue.isEvidence) return false;
            foreach (Trait t in traits)
            {
                if (!clue.traits.Contains(t)) return false;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public override string ToString()
    {
        return String.Format("TYPE: {0} | TRAITS: {1} | TEXT: {2} | IS_EVIDENCE: {3}", type, string.Join(",", traits), text, isEvidence);
    }
}