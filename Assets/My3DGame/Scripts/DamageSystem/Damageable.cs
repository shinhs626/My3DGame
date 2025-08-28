using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using My3DGame.Util;

namespace My3DGame
{
    /// <summary>
    /// 캐릭터의 데미지를 관리하는 partial 클래스
    /// partial 클래스 : 하나의 클래스 두개의 클래스 분리해서 구현
    /// 데미지 기능 구현 partial 클래스
    /// </summary>
    public partial class Damageable : MonoBehaviour
    {
        #region Variables
        public float maxHealth = 100f;           //Max 체력
        public float invulnerablityTime = 2f;    //데미지를 받은후 무적 타임

        public float hitAngle = 360f;            //데미지 입는 각도
        public float hitForwordRotation = 360f;

        //
        private float m_timeSinceLastHit = 0f;  //데미지 입은 시간

        //리스트에 등록된 클래스(IMessageReceiver를 상속 받은)에게 데미지 데이터를 전달
        public List<MonoBehaviour> onDamageMessageReceviers;

        public UnityAction onDeath;
        public UnityAction onRecevieDamage;

        private System.Action schedule;         //데미지 계산시 등록한 후 Late업데이트에 연산한다
        #endregion

        #region Property
        public float CurrentHealth { get; private set; }
        public bool IsInvulnerable { get; set; }
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //초기화
            ResetDamage();
        }

        private void Update()
        {
            //무적모드
            if(IsInvulnerable)
            {
                m_timeSinceLastHit += Time.deltaTime;
                if(m_timeSinceLastHit >= invulnerablityTime)
                {
                    IsInvulnerable = false;

                    //초기화
                    m_timeSinceLastHit = 0f;
                }
            }
        }

        private void LateUpdate()
        {
            if(schedule != null)
            {
                schedule();
                schedule = null;
            }
        }
        #endregion

        #region Custom Method
        //데미지 셋팅 초기화
        private void ResetDamage()
        {
            CurrentHealth = maxHealth;
            IsInvulnerable = false;
            m_timeSinceLastHit = 0f;
        }
        
        //데미지 입기
        public void TakeDamage(DamamgeMessage data)
        {
            //체력 체크
            if(CurrentHealth <= 0f)
                return;

            //무적 체크
            if (IsInvulnerable)
                return;

            //데미지 방향 체크
            Vector3 forward = transform.forward;
            forward = Quaternion.AngleAxis(hitForwordRotation, transform.up) * forward;

            //데미지 오는 방향
            Vector3 positionToDamager = data.damageSource - transform.position;
            positionToDamager = -transform.up * Vector3.Dot(transform.up, positionToDamager);

            if (Vector3.Angle(forward, positionToDamager) > hitAngle * 0.5f)
                return;

            IsInvulnerable = true;
            CurrentHealth -= data.amount;

            if(CurrentHealth <= 0)
            {
                if(onDeath != null)
                    schedule += onDeath.Invoke;
            }
            else
            {
                onRecevieDamage?.Invoke();
            }

            //등록된 리시버 리스트에게 데미지 데이터 전달
            var messageType = CurrentHealth <= 0 ? MessageType.DEAD : MessageType.DAMAGED;

            for (int i = 0; i < onDamageMessageReceviers.Count; i++)
            {
                var receiver = onDamageMessageReceviers[i] as IMessageReceiver;
                receiver?.OnRecevieMessage(messageType, this, data);
            }

        }
        #endregion
    }
}
