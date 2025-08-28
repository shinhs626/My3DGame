using UnityEngine;
using System.Collections.Generic;
using My3DGame.Manager;
using My3DGame.Common;

namespace My3DGame
{
    /// <summary>
    /// 퀘스트를 주는 NPC
    /// </summary>
    public class PickupQuestGiver : PickupNpc
    {
        #region Variables
        public List<QuestObject> quests;        //해당 Npc가 줄수있는 Quest 목록

        private UIManager uiManager;
        private QuestManager questManager;
        #endregion

        #region Unity Event Method
        protected override void Start()
        {
            base.Start();

            //참조
            uiManager = UIManager.Instance;

            //해당 Npc가 줄수있는 Quest 목록을 가져오기
            quests = GetNpcQuests(npc.number);
        }

        private void OnEnable()
        {
            if(questManager == null)
            {
                questManager = QuestManager.Instance;
            }

            //퀘스트 수락, 포기, 완료 함수 등록
            questManager.onAcceptQuest += OnAcceptQuest;
            questManager.onGiveupQuest += OnGiveupQuest;
            questManager.onCompletedQuest += OnCompletedQuest;
        }

        private void OnDisable()
        {
            //퀘스트 수락, 포기, 완료 함수 제거
            questManager.onAcceptQuest -= OnAcceptQuest;
            questManager.onGiveupQuest -= OnGiveupQuest;
            questManager.onCompletedQuest -= OnCompletedQuest;
        }
        #endregion

        #region Custom Method
        public List<QuestObject> GetNpcQuests(int npcNumber)
        {
            List<QuestObject> questObjects = new List<QuestObject>();

            foreach (var quest in DataManger.GetQuestData().quests.quests)
            {
                //퀘스트 클리어 여부 체크

                if(quest.npcNumber == npcNumber)
                {
                    QuestObject questObject = new QuestObject(quest);
                    questObjects.Add(questObject);
                }
            }
            return questObjects;
        }

        protected override void DoAction()
        {
            //퀘스트 체크
            if(quests.Count == 0)
            {
                //해당 npc의 퀘스트를 모두 클리어
                return;
            }

            //quests[0] : 지금 npc가 진행할 Quest
            questManager.SetCurrentQuest(quests[0]);
;           int index = DataManger.GetQuestData().quests.quests[quests[0].number].dialogIndex;

            switch(quests[0].qusetState)
            {
                case QusetState.Ready:
                    uiManager.OpenDialogUI(index, NpcType.QuestGiver);
                    break;
                case QusetState.Accept:
                    uiManager.OpenDialogUI(index+1);
                    break;
                case QusetState.Complete:
                    uiManager.OpenDialogUI(index+2, NpcType.QuestGiver);
                    //퀘스트 완료 보상 받는다
                    CompletedQuest();
                    break;
            }
        }

        //퀘스트 완료 보상 받는다
        private void CompletedQuest()
        {
            //보상 받기
            questManager.RewardQuest();

            //npc 퀘스트 제거
            quests.Remove(quests[0]);
        }


        //퀘스트 수락시 호출되는 함수
        private void OnAcceptQuest(QuestObject quest)
        {
            //수락시 npc가 가지고 있는 퀘스트와 비교
            foreach (var q in quests)
            {
                if(q.number == quest.number)
                {
                    q.qusetState = QusetState.Accept;
                }    
            }
        }

        //퀘스트 포기시 호출되는 함수
        private void OnGiveupQuest(QuestObject quest)
        {
            //포기시 npc가 가지고 있는 퀘스트와 비교
            foreach (var q in quests)
            {
                if (q.number == quest.number)
                {
                    q.qusetState = QusetState.Ready;
                }
            }
        }

        //퀘스트 완료시 호출되는 함수
        private void OnCompletedQuest(QuestObject quest)
        {
            //완료시 npc가 가지고 있는 퀘스트와 비교
            foreach (var q in quests)
            {
                if (q.number == quest.number)
                {
                    q.qusetState = QusetState.Complete;
                }
            }
        }
        #endregion
    }
}
