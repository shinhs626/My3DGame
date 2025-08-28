using UnityEngine;
using My3DGame.Common;
using My3DGame.ItemSystem;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace My3DGame.InventorySystem
{
    /// <summary>
    /// 인벤토리를 관리하는 클래스
    /// 속성 : 인벤토리 컨터이너, 아이템 데이터 베이스, 인벤토리 타입
    /// 기능 : 인벤토리에 아이템 추가, 아이템 바꿔치기, 아이템 장착하기
    /// </summary>
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
    public class InventorySO : ScriptableObject
    {
        #region Variables
        public Inventory contaner = new Inventory();

        public ItemDataBase database;       //아이템 데이터베이스
        public InventoryType type;          //인벤토리 타입
        #endregion

        #region Property
        //인벤토리 슬롯 가져오기
        public ItemSlot[] Slots => contaner.slots;

        //인벤 풀 체크 - 빈 슬롯 갯수
        public int EmptySlotCount
        {
            get
            {
                int count = 0;
                foreach (var itemSlot in Slots)
                {
                    if(itemSlot.item.id <= -1)
                    {
                        count++;
                    }
                }

                return count;
            }
        }
        #endregion

        #region Custom Method
        //슬롯에 아이템 추가하기
        public bool AddItem(Item item, int amount)
        {
            //슬롯에 아이템 존재 여부 체크
            ItemSlot slot = FindItemInInventory(item);
            if(database.itemObjects[item.id].stackable == false || slot == null)
            {
                //새로 빈슬롯에 아이템 추가
                //빈슬롯 체크
                if(EmptySlotCount <= 0)
                {
                    return false;
                }

                ItemSlot emptySlot = GetEmptySlot();
                emptySlot.UpdateSlot(item, amount);
            }
            else
            {
                //수량만 추가
                slot.AddAmount(amount);
            }
            return true;
        }

        //매개변수로 들어온 아이템이 있는 슬롯 가져오기
        public ItemSlot FindItemInInventory(Item item)
        {
            return Slots.FirstOrDefault(i => i.item.id == item.id);
        }

        //매개변수로 들어온 아이템 존재 여부 체크
        public bool IsContainItem(ItemSO itemObject)
        {
            return Slots.FirstOrDefault(i => i.item.id == itemObject.data.id) != null;
        }

        //빈 슬롯 찾기
        public ItemSlot GetEmptySlot()
        {
            return Slots.FirstOrDefault(i => i.item.id <= -1);
        }

        //아이템 바꿔치기
        public void SwapItems(ItemSlot itemA, ItemSlot itemB)
        {
            //아이템 체크
            if(itemA == itemB)
            {
                return;
            }

            //교환할 아이템이 교환 슬롯 장착가능 여부 체크
            if(itemB.CanPlaceInSlot(itemA.ItemObject) && itemA.CanPlaceInSlot(itemB.ItemObject))
            {
                ItemSlot tempSlot = new ItemSlot(itemA.item, itemA.amount);
                itemA.UpdateSlot(itemB.item, itemB.amount);
                itemB.UpdateSlot(tempSlot.item, tempSlot.amount);
            }
        }

        //아이템 사용하기
        public void UseItem(ItemSlot useSlot)
        {
            //슬롯 체크
            if (useSlot.ItemObject == null || useSlot.item.id <= -1
                || useSlot.amount <= 0 )
                return;

            //
            Debug.Log($"{useSlot.item.name} 효과 구현");
            useSlot.AddAmount(-1);
        }

        //아이템 장착하기
        public void EquipItem(ItemSlot itemSlot)
        {
            //매개변수로 들어온 아이템이 장착될 위치 찾기
            foreach (var slot in Slots)
            {
                if(slot.CanPlaceInSlot(itemSlot.ItemObject))
                {
                    SwapItems(slot, itemSlot);
                    break;
                }
            }
        }
        #endregion

        //인벤토리 데이터 json 파일 저장하기, 불러오기
        #region Svae/Load Methods
        public string savePath = "/Inventory.json"; //저장 파일

        //인벤토리 데이터(container)를 json 데이터로 만들기
        public string ToJson()
        {
            return JsonUtility.ToJson(contaner);
        }

        //json 데이터를 인벤토리 데이터(container)로 읽어들이기
        public void FromJson(string jsonString)
        {
            contaner = JsonUtility.FromJson<Inventory>(jsonString);
        }

        [ContextMenu("Save")]
        public void Save()
        {
            //저장할 경로
            string path = Application.persistentDataPath + savePath;
            Debug.Log($"Save Path: {path}");

            //데이터 2진화 준비
            BinaryFormatter bf = new BinaryFormatter();
            //저장할 파일에 접근
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            //저장할 데이터를 준비
            string saveData = JsonUtility.ToJson(contaner, true);
            Debug.Log(saveData);
            //데이터를 파일에 이진화 저장
            bf.Serialize(fs, saveData);

            //파일 닫기
            fs.Close();
        }

        [ContextMenu("Load")]
        public void Load()
        {
            string path = Application.persistentDataPath + savePath;
            Debug.Log($"Load Path: {path}");

            //파일 존재 여부 체크
            if(File.Exists(path))
            {
                //데이터 2진화 준비
                BinaryFormatter bf = new BinaryFormatter();
                //로드할 파일 접근
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                //이진화 저장된 데이터를 로드 후 역진화한다
                string loadData = bf.Deserialize(fs).ToString();
                //json데이터 인벤토리 형식에 맞추어 읽어들인다
                JsonUtility.FromJsonOverwrite(loadData, contaner);
                //파일 닫기
                fs.Close();
            }
        }

        //인벤토리 비우기
        [ContextMenu("Clear")]
        public void Clear()
        {
            contaner.Clear();
        }
        #endregion
    }
}
