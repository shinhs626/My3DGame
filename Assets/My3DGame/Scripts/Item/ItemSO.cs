using UnityEngine;
using My3DGame.Common;

namespace My3DGame.ItemSystem
{
    /// <summary>
    /// 아이템 데이터를 관리하는 스크립터블 오브젝트 클래스
    /// 아이템 데이터베이스, 인벤토리 시스템에서 사용하는 오브젝트 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item")]
    public class ItemSO : ScriptableObject
    {
        #region Variables
        public Item data = new Item();      //아이템아이디, 아이템 이름, 아이템 능력치

        public ItemType type;               //아이템 타입
        public bool stackable;              //인벤 저장시 하나의 슬롯에 다수를 누적 저장 가능 여부

        public int shopPrice;               //유저가 상점에서 구매하는 금액

        public Sprite icon;                 //아이템 아이콘 이미지
        public GameObject modelPrefab;      //장비 아이템 모델 오브젝트

        [TextArea(15,20)]
        public string description;          //아이템 설명글
        #endregion

        #region Custom Method
        //게임에서 사용하는 아이템 생성
        public Item CreateItem()
        {
            Item newItem = new Item(this);
            return newItem;
        }
        #endregion
    }
}