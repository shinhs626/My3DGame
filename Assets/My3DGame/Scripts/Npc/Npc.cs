using System;
using My3DGame.Common;

namespace My3DGame
{
    /// <summary>
    /// Npc 데이터 모델 클래스
    /// </summary>
    [Serializable]
    public class Npc
    {
        public NpcType type;        //npc 타입
        public int number;          //npc 인덱스
        public string name;         //npc 이름
    }
}
