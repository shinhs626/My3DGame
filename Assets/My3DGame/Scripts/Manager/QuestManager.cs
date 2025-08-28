using UnityEngine;
using My3DGame;
using My3DGame.Common;
using My3DGame.Util;
using My3DGame.InventorySystem;
using My3DGame.ItemSystem;
using System;
using System.Collections.Generic;

namespace My3DGame.Manager
{
    /// <summary>
    /// 게임에서 진행하는 퀘스트를 관리하는 매니저 클래스
    /// </summary>
    public class QuestManager : Singleton<QuestManager>
    {
        #region Variablse
        public InventorySO playerInventory;
        public StatsSO playerStats;

        public List<QuestObject> playerQuests;          //플레어가 현재 진행중인 퀘스트 리스트

        public QuestObject currentQuest;                //퀘스트 UI에 전달되는(보여지는) 퀘스트

        public Action<QuestObject> onAcceptQuest;       //퀘스트 수락시 호출되는 이벤트 함수
        public Action<QuestObject> onGiveupQuest;       //퀘스트 포기시 호출되는 이벤트 함수
        public Action<QuestObject> onCompletedQuest;    //퀘스트 완료시 호출되는 이벤트 함수
        #endregion

        #region Unity Event Method
        protected void Start()
        {
            playerQuests = new List<QuestObject>();

            //TODO :
            //파일(서버)에 저장된 데이터를 가져와서 현재 진행 중인 퀘스트 셋팅
        }
        #endregion

        #region Custom Method
        //현재 선택된 퀘스트 셋팅
        public void SetCurrentQuest(QuestObject quest)
        {
            currentQuest = quest;
        }

        //플레이어 퀘스트 리스트에 현재 선택된 퀘스트를 추가
        public void AddPlayerQuests()
        {
            //선택된 퀘스트 체크
            if (currentQuest == null)
                return;

            onAcceptQuest?.Invoke(currentQuest);

            //추가할 퀘스트 오브젝트 만들기
            Quest quest = DataManger.GetQuestData().quests.quests[currentQuest.number];
            QuestObject newQuest = new QuestObject(quest);
            newQuest.qusetState = Common.QusetState.Accept;

            playerQuests.Add(newQuest);
        }

        //플레이어 퀘스트 리스트에 현재 선택된 퀘스트를 제거
        public void GiveupPlayerQuest()
        {
            //선택된 퀘스트 체크
            if (currentQuest == null)
                return;

            onGiveupQuest?.Invoke(currentQuest);

            playerQuests.Remove(currentQuest);
            Debug.Log($"playerQuests.Count: {playerQuests.Count}");
        }

        //퀘스트 완료시 보상 받는다
        public void RewardQuest()
        {
            //선택된 퀘스트 체크
            if (currentQuest == null)
                return;

            Quest quest = DataManger.GetQuestData().quests.quests[currentQuest.number];

            //Debug.Log($"{quest.rewardGold}골드를 보상 받았습니다");
            //Debug.Log($"{quest.rewardExp}경험치를 보상 받았습니다");
            playerStats.AddGold(quest.rewardGold);
            playerStats.AddExp(quest.rewardExp);

            quest.rewardItem = 1;
            if (quest.rewardItem >= 0)
            {
                //Debug.Log($"{quest.rewardItem}번 아이템을 보상 받았습니다");
                ItemSO itemObject = playerInventory.database.itemObjects[quest.rewardItem];
                //아이템 생성
                Item newItem = new Item(itemObject);
                playerInventory.AddItem(newItem, 1);
            }

            //리스트에서 제거
            foreach (var q in playerQuests)
            {
                if(currentQuest.number == q.number)
                {
                    playerQuests.Remove(q);
                }
            }
        }

        //퀘스트 진행사항 업데이트
        //매개변수 진행한 퀘스트 타입과 퀘스트 인덱스를 전달
        public void UpdatePlayerQuests(QuestType type, int goalIndex)
        {
            switch(type)
            {
                case QuestType.Kill:
                    foreach (var quest in playerQuests)
                    {
                        quest.EnemyKill(goalIndex);
                        if(quest.questGoal.IsReached)
                        {
                            quest.qusetState = QusetState.Complete;
                            onCompletedQuest?.Invoke(quest);
                        }
                    }
                    break;

                case QuestType.Collect:
                    foreach (var quest in playerQuests)
                    {
                        quest.ItemCollect(goalIndex);
                        if (quest.questGoal.IsReached)
                        {
                            quest.qusetState = QusetState.Complete;
                            onCompletedQuest?.Invoke(quest);
                        }
                    }
                    break;
            }
        }
        #endregion

    }
}