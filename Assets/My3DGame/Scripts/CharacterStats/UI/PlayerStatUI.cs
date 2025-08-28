using UnityEngine;
using TMPro;
using My3DGame.InventorySystem;
using My3DGame.Common;

namespace My3DGame
{
    /// <summary>
    /// 플레이어 스탯 UI를 관리하는 클래스
    /// </summary>
    public class PlayerStatUI : MonoBehaviour
    {
        #region Variables
        public StatsSO statsObejct;
        public TextMeshProUGUI[] attributesText;

        public InventorySO equipmentInventory;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //equipmentInventory의 슬롯의 이벤트함수에 등록
            if(statsObejct != null && equipmentInventory != null)
            {
                foreach (var slot in equipmentInventory.Slots)
                {
                    slot.OnPreUpdate += OnUnEquipItem;
                    slot.OnPostUpdate += OnEquipItem;
                }
            }
        }

        private void OnEnable()
        {
            UpdateAttributesText();
            statsObejct.OnChangedStats += OnChangedStats;
        }

        private void OnDisable()
        {
            statsObejct.OnChangedStats -= OnChangedStats;
        }
        #endregion

        #region Custom Method
        //UI Text 적용
        private void UpdateAttributesText()
        {
            attributesText[0].text = statsObejct.GetModifiedValue(Common.CharacterAttibute.Agility).ToString();
            attributesText[1].text = statsObejct.GetModifiedValue(Common.CharacterAttibute.Intellect).ToString();
            attributesText[2].text = statsObejct.GetModifiedValue(Common.CharacterAttibute.Stamina).ToString();
            attributesText[3].text = statsObejct.GetModifiedValue(Common.CharacterAttibute.Strength).ToString();

        }

        // 아이템 장착시 stats에 아이템 buff 값 추가
        private void OnEquipItem(ItemSlot itemSlot)
        {
            //슬롯 체크
            if (itemSlot.ItemObject == null)
                return;

            //장착 인벤토리 여부 체크
            if(itemSlot.parent.type == InventoryType.Equipment)
            {
                foreach (var buff in itemSlot.item.buffs)
                {
                    foreach (var attribute in statsObejct.attributes)
                    {
                        if(attribute.type == buff.stat)
                        {
                            attribute.value.AddModifier(buff);
                        }
                    }
                }
            }
        }

        // 아이템 탈착시 stats에 아이템 buff 값 추가
        private void OnUnEquipItem(ItemSlot itemSlot)
        {
            //슬롯 체크
            if (itemSlot.ItemObject == null)
                return;

            //탈착 인벤토리 여부 체크
            if (itemSlot.parent.type == InventoryType.Equipment)
            {
                foreach (var buff in itemSlot.item.buffs)
                {
                    foreach (var attribute in statsObejct.attributes)
                    {
                        if (attribute.type == buff.stat)
                        {
                            attribute.value.RemoveModifier(buff);
                        }
                    }
                }
            }
        }

        private void OnChangedStats(StatsSO statsObject)
        {
            UpdateAttributesText();
        }
        #endregion
    }
}