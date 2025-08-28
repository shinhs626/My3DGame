using UnityEngine;
using My3DGame.Util;
using My3DGame.GameData;

namespace My3DGame.Manager
{
    /// <summary>
    /// 사운드 클립을 가져와서 사운드 플레이
    /// </summary>
    public class SoundManager : Singleton<SoundManager>
    {
        #region Variables
        //생성한 사운드 플레이 게임오브젝트의 부모 오브젝트
        private Transform soundRoot = null;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //생성한 사운드 플레이 게임오브젝트의 부모 오브젝트 생성
            if (soundRoot == null)
            {
                soundRoot = new GameObject("SoundRoot").transform;
                soundRoot.SetParent(this.transform);
            }
        }
        #endregion

        #region Custom Method
        //게임 오브젝트 생성하여 지정하는 효과음을 플레이하는 함수
        public void CreateSound(SoundList soundList, Vector3 point)
        {
            int index = (int)soundList;
            SoundClip clip = DataManger.GetSoundData().GetSoundClip(index);

            //하이라키창에서 빈 오브젝트 만들기
            GameObject impactSfxInstance = new GameObject();
            impactSfxInstance.transform.position = point;   //위치 지정

            //새로 생성한 게임 오브젝트에 AudioSource 컴포넌트 추가
            AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
            source.clip = clip.GetAudioClip();     //플레이할 오디오 클립
            //audiosource 설정
            
            source.Play();

            //사운드 플레이 후 자동 킬
            TimeSlefDestruct timeSlefDestruct = impactSfxInstance.AddComponent<TimeSlefDestruct>();
            timeSlefDestruct.lifeTime = clip.GetAudioClip().length;
        }
        #endregion
    }
}
