using UnityEngine;

namespace My3DGame.Util
{
    //오디오 플레이 관련 클래스
    public class AudioUtility
    {
        //게임 오브젝트 생성하여 지정하는 효과음을 플레이하는 함수
        public static void CreateSFX(AudioClip clip, Vector3 point, float spatialBlend, float rolloffDistancMin = 1f)
        {
            //하이라키창에서 빈 오브젝트 만들기
            GameObject impactSfxInstance = new GameObject();
            impactSfxInstance.transform.position = point;   //위치 지정

            //새로 생성한 게임 오브젝트에 AudioSource 컴포넌트 추가
            AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
            source.clip = clip;     //플레이할 오디오 클립
            source.spatialBlend = spatialBlend; //3D 사운드 효과 설정
            source.minDistance = rolloffDistancMin; //3D 사운드 효과 최소 거리
            source.Play();

            //사운드 플레이 후 자동 킬
            TimeSlefDestruct timeSlefDestruct = impactSfxInstance.AddComponent<TimeSlefDestruct>();
            timeSlefDestruct.lifeTime = clip.length;
        }
    }
}
