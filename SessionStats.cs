using Godot;
using System;

public static class SessionStats
{
    public static long score = 9123912321;
    public static long time = 345;
    public static int criminalsCaught = 2;
    public static int innocentsReleased = 8;
    public static int criminalsReleased = 2;
    public static int innocentsConvicted = 1;

    internal static void Reset()
    {
        score = 0;
        time = 0;
        criminalsCaught = 0;
        innocentsConvicted = 0;
        innocentsReleased = 0;
        criminalsReleased = 0;
    }
}
