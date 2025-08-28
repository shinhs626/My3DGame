using MyPet.AI;
using UnityEngine;
using UnityEngine.AI;

namespace My3DGame.AI
{
    /// <summary>
    /// 등록되어있는 Waypoint들을 순회하는 패트롤 기능 구현
    /// </summary>
    public class PatrolState : State
    {
        #region Variables
        private Animator m_Animator;
        private NavMeshAgent m_Agent;

        //패트롤
        private EnemyPatrol enemyPatrol;

        private Transform m_TargetWaypoint = null;  //목표 웨이 포인트
        private int m_WaypointIndex = 0;            //현재 웨이 포인트 Index

        //애니메이션 파라미터
        readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        #endregion

        #region Property
        private Transform[] WayPoints => enemyPatrol?.wayPoints;
        #endregion

        //상태 초기화, 초기값 설정
        public override void OnInitialize()
        {
            //참조
            m_Animator = enemy.GetComponent<Animator>();
            m_Agent = enemy.GetComponent<NavMeshAgent>();

            //부모 객체로 부터 자식 객체 가져오기
            enemyPatrol = enemy as EnemyPatrol;
        }

        //상태 시작하기
        public override void OnEnter()
        {
            m_Agent.stoppingDistance = 0.5f;

            //목표 웨이포인트 찾기
            if(m_TargetWaypoint == null)
            {
                FindNextWayPoint();
            }

            //목표 웨이포인트 찾으면
            if(m_TargetWaypoint)
            {
                m_Agent.SetDestination(m_TargetWaypoint.position);
            }
            else
            {
                stateMachine.ChangeState(new IdleState());
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            //적 detecting 체크
            if (enemy.Target)
            {
                if (enemy.IsAttackable)
                {
                    stateMachine.ChangeState(new AttackState());
                }
                else
                {
                    stateMachine.ChangeState(new WalkState());
                }
            }
            else //적 감지가 안되면 계속 패트롤
            {
                //도착 판정
                if (m_Agent.remainingDistance <= m_Agent.stoppingDistance)
                {
                    FindNextWayPoint();
                    //아이들 상태로 보내고 아이들 상태에서 0~3초후 다음 목표로 이동
                    stateMachine.ChangeState(new IdleState());
                }
                else
                {
                    //애니메이션 변경
                    m_Animator.SetFloat(m_HashForwardSpeed, m_Agent.velocity.magnitude);
                }
            }
        }

        //상태 나가기
        public override void OnExit()
        {
            //m_Agent 길찾기 초기화
            m_Agent.ResetPath();
        }

        //다음 목표 지점 찾기
        private void FindNextWayPoint()
        {
            m_TargetWaypoint = null;

            if(WayPoints != null && WayPoints.Length > 0)
            {
                m_WaypointIndex = (m_WaypointIndex + 1) % WayPoints.Length;
                m_TargetWaypoint = WayPoints[m_WaypointIndex];
            }
        }
    }
}
