using UnityEngine;

namespace My3DGame.ItemSystem
{
    /// <summary>
    /// 아이템 데이터를 관리하는 클래스
    /// 속성: 아이디(아이템 데이터베이스 아이디), 이름, 능력치
    /// </summary>
    [System.Serializable]
    public class Item
    {
        #region Variables
        public int id;
        public string name;

        public ItemBuff[] buffs;
        #endregion

        #region Constructor
        //아이템 클리어
        public Item()
        {
            id = -1;
            name = null;
        }

        //게임에서 사용하는 아이템 생성
        public Item(ItemSO itemObejct)
        {
            id = itemObejct.data.id;
            name = itemObejct.name;

            buffs = new ItemBuff[itemObejct.data.buffs.Length];
            for (int i = 0; i < buffs.Length; i++)
            {
                buffs[i] = new ItemBuff(itemObejct.data.buffs[i].Min, itemObejct.data.buffs[i].Max);
                buffs[i].stat = itemObejct.data.buffs[i].stat;
            }
        }
        #endregion
    }
}