using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using My3DGame.Manager;

namespace My3DGame.InventorySystem
{
    /// <summary>
    /// 가변적인 아이템 슬롯을 가진 인벤토리 UI를 관리하는 클래스, InventoryUI 상속
    /// </summary>
    public class DynamicInventoryUI : InventoryUI
    {
        #region Variables
        public GameObject slotPrefab;   //슬롯 UI 프리팹
        public Transform slotsParent;   //생성시 지정되는 부모 오브젝트

        public InventorySO equipmentInventory;  //장착 인벤토리
        #endregion

        #region Custom Method
        public override void CreateSlots()
        {
            slotUIs = new Dictionary<GameObject, ItemSlot>();

            for (int i = 0; i < inventoryObejct.Slots.Length; i++)
            {
                GameObject go = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity, slotsParent);

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
                go.name += ": " + i.ToString();
            }
        }

        public override void UpdateSelectSlot(GameObject go)
        {
            base.UpdateSelectSlot(go);

            if(selectSlotObejct == null)
            {
                itemInfoUI.gameObject.SetActive(false);
            }
            else
            {
                itemInfoUI.gameObject.SetActive(true);
                itemInfoUI.SetItemInfoUI(slotUIs[selectSlotObejct], false);
            }
        }

        //아이템 장착
        public void EquipItem()
        {
            Debug.Log("선택된 아이템 장착");
            if (selectSlotObejct == null)
                return;

            equipmentInventory.EquipItem(slotUIs[selectSlotObejct]);
            //UIManager.Instance.EquipItemInventory(slotUIs[selectSlotObejct]);

            //선택 해제
            UpdateSelectSlot(null);
        }

        //아이템 사용
        public void UseItem()
        {
            //선택 아이템 체크
            if (selectSlotObejct == null)
                return;

            //소모품 아이템 사용
            inventoryObejct.UseItem(slotUIs[selectSlotObejct]);
            //선택 해제
            UpdateSelectSlot(null);
        }
        
        //아이템 판매(버리기)
        public void SellItem()
        {
            //선택 아이템 체크
            if (selectSlotObejct == null)
                return;

            //상점 판매 대금의 반값을 받는다
            int sellPrice = slotUIs[selectSlotObejct].ItemObject.shopPrice / 2;
            Debug.Log($"{sellPrice} 골드를 보상받고 버린다");

            //슬롯에서 아이템 제거
            slotUIs[selectSlotObejct].AddAmount(-1);
            //선택 해제
            UpdateSelectSlot(null);
        }
        #endregion
    }
}