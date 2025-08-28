using UnityEngine;
using System;
using System.Collections.Generic;
using My3DGame.Common;

namespace My3DGame
{
    /// <summary>
    /// 캐릭터의 속성 값을 관리하는 클래스
    /// </summary>
    [Serializable]
    public class ModifiableInt
    {
        #region Variables
        private int baseValue;      //stat 기본 값
        [SerializeField]
        private int modifedValue;   //아이템 스탯이 적용된 최종 계산값

        //modifedValue 값 변경(계산)시 등록된 함수 실행하는 이벤트 함수
        private event Action<ModifiableInt> OnModifedValue;

        //modifedValue 값을 계산하기 위한 스탯 값들을 저장하는 리스트
        public List<IModifier> modifiers = new List<IModifier>();
        #endregion

        #region Property
        public int BaseValue
        {
            get { return baseValue; }
            set
            {
                baseValue = value;
                //스탯 계산 업데이트
                UpdateModifedValue();
            }
        }

        public int ModifedValue
        {
            get { return modifedValue; }
            set { modifedValue = value; }
        }
        #endregion

        //생성자 - 값 변경시 호출되는 함수를 매개변수로 받아 이벤트 함수에 등록
        #region Constructor
        public ModifiableInt(Action<ModifiableInt> method = null)
        {
            //기본값으로 초기화
            ModifedValue = baseValue;

            //이벤트 함수 등록
            RegisterModEvent(method);
        }
        #endregion

        #region Custom Method
        //매개변수로 받아 이벤트 함수에 등록
        public void RegisterModEvent(Action<ModifiableInt> method)
        {
            //method 체크
            if (method == null)
                return;

            OnModifedValue += method;
        }

        //매개변수로 받아 이벤트 함수에서 제거
        public void RemoveModEvent(Action<ModifiableInt> method)
        {
            //method
            if (method == null)
                return;

            OnModifedValue -= method;
        }

        //modifedValue 값을 계산하기
        private void UpdateModifedValue()
        {
            //스탯 누적 변수
            int valueToAdd = 0;

            foreach (var modifer in modifiers)
            {
                modifer.AddValue(ref valueToAdd);
            }
            //최종값
            ModifedValue = baseValue + valueToAdd;

            //modifedValue 값 변경(계산)시 등록된 함수 호출
            OnModifedValue?.Invoke(this);
        }

        //속성 값 추가
        public void AddModifier(IModifier modifier)
        {
            //리스트에 추가
            modifiers.Add(modifier);
            //최종값 계산
            UpdateModifedValue();
        }

        //속성 값 제거
        public void RemoveModifier(IModifier modifier)
        {
            //리스트에서 제거
            modifiers.Remove(modifier);
            //최종값 계산
            UpdateModifedValue();
        }
        #endregion
    }
}