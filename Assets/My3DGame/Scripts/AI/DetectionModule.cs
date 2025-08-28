using UnityEngine;

namespace My3DGame.AI
{
    /// <summary>
    /// 타겟(적)을 찾는 클래스
    /// 속성: 타겟 오브젝트, 타겟과의 거리
    /// 기능: 0.1초마다 적을 디텍팅,  디텍션 범위를 기즈모로 표시
    /// </summary>
    public class DetectionModule : MonoBehaviour
    {
        #region Variables
        private Transform m_Target;

        public LayerMask targetMask;            //타겟팅할 적의 레이어

        [SerializeField]
        private float detectionRange = 5f;      //디텍션 범위
        [SerializeField]
        private float detectionDelayTime = 0.1f;    //디텍션 딜레이 타임
        private float m_DistanceToTarget;           //타겟과의 거리
        #endregion

        #region Property
        public Transform Target => m_Target;
        public float DistanceToTarget => m_DistanceToTarget;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //0.1초마다 적을 디텍팅
            InvokeRepeating("UpdateDetection", 0f, detectionDelayTime);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, detectionRange);
        }
        #endregion

        #region Custom Method
        private void UpdateDetection()
        {
            m_DistanceToTarget = 0;

            //가장 가까운 적 찾기
            float shotestDistance = Mathf.Infinity;
            Transform nearestEnemy = null;

            Collider[] enemies = Physics.OverlapSphere(this.transform.position, detectionRange, 
                targetMask);

            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if(distance < shotestDistance)
                {
                    shotestDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }

            if(nearestEnemy != null && shotestDistance <= detectionRange)
            {
                m_DistanceToTarget = shotestDistance;
                m_Target = nearestEnemy;
            }
            else
            {
                m_Target = null;
            }
        }
        #endregion
    }
}
