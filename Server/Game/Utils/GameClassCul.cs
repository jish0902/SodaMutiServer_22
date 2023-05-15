namespace Server.Game.Utils;

public static class Utills
{
    public static int GetStatFormClass(int characterClass, int level = 1)
    {
        return characterClass * 100 + level;
    }
}