using System;
using My3DGame.Common;
using My3DGame.ItemSystem;
using UnityEngine;

namespace My3DGame.InventorySystem
{
    /// <summary>
    /// 인벤토리에 들어가는 아이템 슬롯을 관리하는 클래스
    /// 속성: 아이템, 아이템 갯수, 장착가능 타입
    /// 기능: 슬롯업데이트, 슬롯 비우기, 아이템 수량 연산, 슬롯에 장착 여부 체크
    /// </summary>
    [System.Serializable]
    public class ItemSlot
    {
        #region Variables
        public Item item;
        public int amount;

        //장착 가능 타입
        public ItemType[] allowedItems = new ItemType[0];

        [NonSerialized]
        public InventorySO parent;
        [NonSerialized]
        public GameObject slotUI;   //슬롯 데이터가 보여질 UI 오브젝트

        [NonSerialized]
        public Action<ItemSlot> OnPreUpdate;    //슬롯 갱신하기전에 등록된 함수를 호출해서 실행
        [NonSerialized]
        public Action<ItemSlot> OnPostUpdate;   //슬롯 갱신후에 등록된 함수를 호출해서 실행
        #endregion

        #region Property
        //item의 아이템 데이터
        public ItemSO ItemObject
        {
            get
            {
                return item.id >= 0 ? parent.database.itemObjects[item.id] : null;
            }
        }
        #endregion

        //생성자
        #region Contructor
        //빈 슬롯 만들기
        public ItemSlot()
        {
            UpdateSlot(new Item(), 0);
        }

        //매개변수로 들어온 아이템과 수량으로 슬롯 채우기
        public ItemSlot(Item _item, int _amount)
        {
            UpdateSlot(_item, _amount);
        }
        #endregion

        #region Custom Method
        //슬롯 업데이트
        public void UpdateSlot(Item _item, int _amount)
        {
            //수량 체크
            if(_amount <= 0)
            {
                _item = new Item();
            }

            OnPreUpdate?.Invoke(this);
            this.item = _item;
            this.amount = _amount;
            OnPostUpdate?.Invoke(this);
        }

        //슬롯의 아이템 제거하기
        public void RemoveItem()
        {
            UpdateSlot(new Item(), 0);
        }

        //슬롯의 아이템 갯수 연산
        public void AddAmount(int value)
        {
            UpdateSlot(this.item, this.amount += value);
        }

        //슬롯에 아이템이 장착가능 여부 체크
        public bool CanPlaceInSlot(ItemSO itemObject)
        {
            //들어온 아이템 무조건 장착 가능, 장착할 아이템이 없으면
            if(allowedItems.Length <= 0 || 
                itemObject == null || itemObject.data.id < 0)
            {
                return true;
            }
            
            foreach (var itemType in allowedItems)
            {
                if(itemObject.type == itemType)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}