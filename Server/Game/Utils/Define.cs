public class Define
{
    public enum RewardsType
    {
        RandomItem = 0,
        LevelUp = 1,
        RandomSpawn = 2
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        loading,
        Game
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount
    }

    public enum UIEvent
    {
        Click,
        Drag
    }
}