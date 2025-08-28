using UnityEngine;

namespace My3DGame
{
    /// <summary>
    /// 캐릭터의 데미지를 관리하는 partial 클래스
    /// partial 클래스 : 하나의 클래스 두개의 클래스 분리해서 구현
    /// 데미지 데이터 구조체를 관리하는 partial 클래스
    /// </summary>
    public partial class Damageable : MonoBehaviour
    {
        //데미지 데이터 구조체
        public struct DamamgeMessage
        {
            public MonoBehaviour damager;
            public float amount;            //데미지량
            public Vector3 direction;
            public Vector3 damageSource;
            public bool thorwing;

            public bool stopCamera;
        }
    }
}
