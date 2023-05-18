namespace Server.Game.Utils;

public static class Time
{
    public static ushort Millis2ServerTick(float time)
    {
        return (ushort)(time / Program.ServerIntervalTick);

    }
    
}