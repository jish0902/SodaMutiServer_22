using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using System.Text;

namespace Server.Game
{
    class Monster : GameObject
    {
        public int TemplateId { get; private set; }

        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }


        public void Init(int templateId)
        {
            TemplateId = templateId;

            //MonsterData monsterData = null;
            //DataManager.MonsterDict.TryGetValue(templateId, out monsterData);
            //stat.MergeFrom(monsterData.stat);
            //stat.Hp = monsterData.stat.MaxHp;
            //State = CreatureState.Idle;


        }
    }



}
