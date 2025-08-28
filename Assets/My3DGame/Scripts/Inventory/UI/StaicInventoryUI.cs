using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using My3DGame.Manager;

namespace My3DGame.InventorySystem
{
    /// <summary>
    /// 갯수와 자리가 고정된 아이템 슬롯을 가진 인벤토리 UI를 관리하는 클래스, InventoryUI 상속
    /// </summary>
    public class StaicInventoryUI : InventoryUI
    {
        #region Variables
        public GameObject[] staticSlots;

        public InventorySO playerInventory; //플레이어 인벤토리
        
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //현재 장착 아이템의 buff값을 stastObject에 적용

        }
        #endregion

        #region Custom Method
        public override void CreateSlots()
        {
            slotUIs = new Dictionary<GameObject, ItemSlot>();

            for (int i = 0; i < inventoryObejct.Slots.Length; i++)
            {
                GameObject go = staticSlots[i];

                //생성된 슬롯 오브젝트의 트리거에 이벤트 등록
                AddEvent(go, EventTriggerType.PointerEnter, delegate { OnEnter(go); });
                AddEvent(go, EventTriggerType.PointerExit, delegate { OnExit(go); });
                AddEvent(go, EventTriggerType.BeginDrag, delegate { OnStartDrag(go); });
                AddEvent(go, EventTriggerType.Drag, delegate { OnDrag(go); });
                AddEvent(go, EventTriggerType.EndDrag, delegate { OnEndDrag(go); });
                AddEvent(go, EventTriggerType.PointerClick, delegate { OnClick(go); });

                //slotUIs 등록
                inventoryObejct.Slots[i].slotUI = go;
                slotUIs.Add(go, inventoryObejct.Slots[i]);
            }
        }

        public override void UpdateSelectSlot(GameObject go)
        {
            base.UpdateSelectSlot(go);

            if (selectSlotObejct == null)
            {
                itemInfoUI.gameObject.SetActive(false);
            }
            else
            {
                itemInfoUI.gameObject.SetActive(true);
                itemInfoUI.SetItemInfoUI(slotUIs[selectSlotObejct], true);
            }
        }

        //아이템 장착 해제
        public void UnEquip()
        {
            //선택 아이템 오브젝트 체크
            if (selectSlotObejct == null)
                return;
            
            //인벤토리에 제거하는 아이템 추가 - 인벤 풀 체크
            if (playerInventory.AddItem(slotUIs[selectSlotObejct].item, 1))
            {
                //아이템 제거
                slotUIs[selectSlotObejct].RemoveItem();
                //선택 해제
                UpdateSelectSlot(null);
            }
        }

        //모든 아이템 장착 해제
        public void UnEquipAll()
        {
            foreach (var slotObejct in staticSlots)
            {
                //빈 슬롯 체크
                if (slotUIs[slotObejct].item.id <= -1 || slotUIs[slotObejct].amount <= 0)
                    continue;

                //인벤토리에 제거하는 아이템 추가 - 인벤 풀 체크
                if (playerInventory.AddItem(slotUIs[slotObejct].item, 1))
                {
                    //아이템 제거
                    slotUIs[slotObejct].RemoveItem();
                }
            }
            //선택 해제
            UpdateSelectSlot(null);
        }

        //매개변수로 들어온 아이템이 장착될 아이템 슬롯을 리턴
        public void Equip(ItemSlot itemSlot)
        {
            //매개변수로 들어온 아이템이 장착될 위치 찾기
            foreach(var go in staticSlots)
            {
                ItemSlot slot = slotUIs[go];
                if(slot.CanPlaceInSlot(itemSlot.ItemObject))
                {
                    inventoryObejct.SwapItems(slot, itemSlot);
                    break;
                }
            }
        }
        #endregion
    }
}
