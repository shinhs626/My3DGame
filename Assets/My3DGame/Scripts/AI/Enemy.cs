using UnityEngine;
using My3DGame.Util;
using My3DGame.Manager;
using My3DGame.Common;

namespace My3DGame.AI
{
    /// <summary>
    /// 적(Enemy)을 관리하는 클래스, 적 클래스들의 부모 클래스
    /// 속성: 상태머신, 공격범위, 공격 지연 시간
    /// 기능: 공격 가능 여부 체크, 상태 변경, 타겟을 바라본다
    /// </summary>
    public class Enemy : MonoBehaviour, IMessageReceiver
    {
        #region Variables
        //참조
        protected DetectionModule m_DetectionModule;
        protected Damageable m_Damageable;

        //상태를 관리하는 상태머신
        protected StateMachine stateMachine;

        //공격 가능 범위
        [SerializeField]
        private float attackRange = 2.0f;

        //공격 데미지 값
        [SerializeField]
        private float attackDamage = 20f;

        //회전 속도
        private float rotateSpeed = 5f;
        #endregion

        #region Property
        public Transform Target => m_DetectionModule.Target;

        //공격 가능 범위
        public float AttackRange => attackRange;
        //공격 지연 시간
        public float AttackDelay { get; set; }
        //공격 가능 여부
        public bool IsAttackable
        {
            get
            {
                if(Target)
                {
                    float distance = Vector3.Distance(transform.position, Target.position);
                    return (distance <= AttackRange);
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region Unity Event Method
        protected virtual void Awake()
        {
            //참조
            m_DetectionModule = this.GetComponent<DetectionModule>();
            m_Damageable = this.GetComponent<Damageable>();
        }

        protected virtual void OnEnable()
        {
            //데미지 메시지 리시버 추가
            m_Damageable.onDamageMessageReceviers.Add(this);
            m_Damageable.IsInvulnerable = true;
        }

        protected virtual void OnDisable()
        {
            //데미지 메시지 리시버 제거
            m_Damageable.onDamageMessageReceviers.Remove(this);
        }

        protected virtual void Start()
        {
            //상태머신 생성 및 상태 등록
            stateMachine = new StateMachine(this, new IdleState());
            stateMachine.RegisterState(new WalkState());
            stateMachine.RegisterState(new AttackState());
            stateMachine.RegisterState(new DeathState());
            //상속받은 후 추가로 새로운 상태 등록 가능

            //초기화
            AttackDelay = 2f;
        }

        protected virtual void Update()
        {
            //상태머신의 업데이트가 현재 상태의 업데이트를 실행
            stateMachine.Update(Time.deltaTime);
        }
        #endregion

        #region Custom Method
        //상태 변경
        public State ChangeState(State newState)
        {
            return stateMachine.ChangeState(newState);
        }

        //타겟을 바라본다
        public void FaceToTarget()
        {
            if (Target == null)
                return;

            //방향을 구하고 그 방향으로 회전
            Vector3 direction = (Target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 
                Time.deltaTime * rotateSpeed);
        }

        //데미지 처리
        public void OnRecevieMessage(MessageType type, object sender, object msg)
        {
            switch (type)
            {
                case MessageType.DAMAGED:
                    {
                        Damageable.DamamgeMessage damageData = (Damageable.DamamgeMessage)msg;
                        Damaged(damageData);
                    }
                    break;
                case MessageType.DEAD:
                    {
                        Damageable.DamamgeMessage damageData = (Damageable.DamamgeMessage)msg;
                        Die(damageData);
                    }
                    break;
            }
        }

        private void Damaged(Damageable.DamamgeMessage damageMessage)
        {
            //TODO
        }

        private void Die(Damageable.DamamgeMessage damageMessage)
        {
            //TODO
            stateMachine.ChangeState(new DeathState());

            //퀘스트 진행사항 체크
            QuestManager.Instance.UpdatePlayerQuests(QuestType.Kill, 0);

            //오브젝트 킬
            Destroy(gameObject, 2f);
        }

        //공격 애니메이션 프레임에서 자동으로 데미지 준다
        public void CheckDamage()
        {
            if (Target == null)
                return;

            Damageable damageable = Target.GetComponent<Damageable>();

            if(damageable)
            {
                //데미지 데이터 구성
                Damageable.DamamgeMessage data;

                data.amount = attackDamage;
                data.damager = this;
                data.direction = Vector3.zero;
                data.damageSource = Vector3.zero;
                data.thorwing = false;
                data.stopCamera = false;

                damageable.TakeDamage(data);
            }
        }
        #endregion
    }
}
