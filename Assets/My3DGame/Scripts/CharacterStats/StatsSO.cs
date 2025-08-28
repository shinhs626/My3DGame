using UnityEngine;
using System;
using My3DGame.Common;
using System.Collections.Generic;

namespace My3DGame
{
    /// <summary>
    /// 캐릭터 스탯 데이터와 유저 게임데이터를 가지고 있는 스크랩터블 오브젝트
    /// </summary>
    [CreateAssetMenu(fileName = "new Stats", menuName = "Stats System/Character Stats")]
    public class StatsSO : ScriptableObject
    {
        #region Variables
        public Attribute[] attributes;      //캐릭터 속성 배열

        [SerializeField]
        private UserData userData;           //유저 게임 데이터

        //스탯 변경시 등록된 함수를 호출하는 이벤트 함수
        public Action<StatsSO> OnChangedStats;

        //초기화 실행 여부 체크
        private bool isInitialized = false;
        #endregion

        #region Property
        public int Level
        {
            get { return userData.level; }
            private set { userData.level = value; }
        }

        public int Exp
        {
            get { return userData.exp; }
            private set { userData.exp = value; }
        }

        public int Gold
        {
            get { return userData.gold; }
            private set { userData.gold = value; }
        }

        public int CurrentHealth
        {
            get { return userData.health; }
            private set { userData.health = value; }
        }

        public int CurrentMana
        {
            get { return userData.mana; }
            private set { userData.mana = value; }
        }

        public int MaxHealth
        {
            get
            {
                int maxHealth = 0;
                foreach (var attribute in attributes)
                {
                    //체력 속성 찾기
                    if(attribute.type == CharacterAttibute.Health)
                    {
                        maxHealth = attribute.value.ModifedValue;
                    }
                }
                return maxHealth;
            }
        }

        public int MaxMana
        {
            get
            {
                int maxMana = 0;
                foreach (var attribute in attributes)
                {
                    if(attribute.type == CharacterAttibute.Mana)
                    {
                        maxMana = attribute.value.ModifedValue;
                    }
                }
                return maxMana;
            }
        }

        public float HealthPercentage
        {
            get
            {
                return (MaxHealth > 0) ? ((float)CurrentHealth/(float)MaxHealth) : 0f;
            }
        }

        public float ManaPrecentage
        {
            get
            {
                return (MaxMana > 0) ? ((float)CurrentMana/(float)MaxMana) : 0f;
            }
        }
        #endregion

        #region Unity Event Method
        private void OnEnable()
        {
            InitializeAttributes();
        }
        #endregion

        #region Custom Method
        //속성 초기화 - 최초 1회만 실행
        private void InitializeAttributes()
        {
            //초기화 실행 여부 체크
            //if (isInitialized)
            //    return;

            //isInitialized = true;

            //속성 초기화
            foreach (var attribute in attributes)
            {
                attribute.value = new ModifiableInt(OnModifiedValue);                
            }
            SetBaseValue(CharacterAttibute.Agility, 100);
            SetBaseValue(CharacterAttibute.Intellect, 100);
            SetBaseValue(CharacterAttibute.Stamina, 100);
            SetBaseValue(CharacterAttibute.Strength, 100);
            SetBaseValue(CharacterAttibute.Health, 100);
            SetBaseValue(CharacterAttibute.Mana, 100);

            //유저 게임 데이터 초기화
            Level = 1;
            Exp = 0;
            Gold = 10000;
            CurrentHealth = MaxHealth;
            CurrentMana = MaxMana;
        }

        #region Stats Value
        //속성의 BaseValue값 초기화
        private void SetBaseValue(CharacterAttibute type, int value)
        {
            foreach (var attribute in attributes)
            {
                if(attribute.type == type)
                {
                    attribute.value.BaseValue = value;
                }
            }
        }

        //속성의 BaseValue값 가져오기
        public int GetBaseValue(CharacterAttibute type)
        {
            foreach (var attribute in attributes)
            {
                if(attribute.type == type)
                {
                    return attribute.value.BaseValue;
                }
            }

            //지정된 타입이 없으면
            return -1;
        }

        //매개변수로 들어온 속성 타입의 최종 계산값 구하기
        public int GetModifiedValue(CharacterAttibute type)
        {
            foreach (var attribute in attributes)
            {
                if(attribute.type == type)
                {
                    return attribute.value.ModifedValue;
                }
            }
            return -1;
        }

        //속성 값 변경시 호출되는 함수
        private void OnModifiedValue(ModifiableInt value)
        {
            OnChangedStats?.Invoke(this);
        }
        #endregion

        #region UserData Value
        public void AddGold(int amount)
        {
            Gold += amount;

            //속성 값 변경시 호출되는 함수
            OnChangedStats?.Invoke(this);
        }

        //소지금 체크
        public bool EnoughGold(int amount)
        {
            return Gold >= amount;
        }

        public bool UseGold(int amount)
        {
            if(Gold < amount)
            {
                Debug.Log("소지금 부족");
                return false;
            }

            Gold -= amount;

            //속성 값 변경시 호출되는 함수
            OnChangedStats?.Invoke(this);

            return true;
        }

        //레벨업 공식: 현재(지정한) 레벨에서 다음 레벨로 가는데 필요한 경험치 계산
        public int GetExpForLevelup(int level)
        {
            //표, 레벨업 공식
            return level * 100;
        }

        //경험치 추가
        public bool AddExp(int amount)
        {
            bool isLevelup = false;

            Exp += amount;

            //레벨업에 필요한 경험치
            int needForLevelup = GetExpForLevelup(Level);

            while(Exp >= needForLevelup)
            {
                Exp -= needForLevelup;
                Level++;

                isLevelup = true;

                //레벨업한 레벨을 가지고 다시 레벨업 필요한 경험치를 구한다
                needForLevelup = GetExpForLevelup(Level);
            }

            //속성 값 변경시 호출되는 함수
            OnChangedStats?.Invoke(this);

            return isLevelup;
        }
        #endregion

        #endregion


    }
}
