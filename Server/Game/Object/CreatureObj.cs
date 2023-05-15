using System;

namespace Server.Game;

public class CreatureObj : GameObject
{
    public Action<GameObject> DamageReflexAction;
}