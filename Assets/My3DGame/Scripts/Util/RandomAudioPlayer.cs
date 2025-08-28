using NUnit.Framework.Constraints;
using UnityEngine;

namespace My3DGame.Util
{
    /// <summary>
    /// 등록되어 있는 오디오 클립중 하나를 랜덤하게 선택하여 플레이 한다
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class RandomAudioPlayer : MonoBehaviour
    {
        //내부 클래스(직렬화된 클래스)
        [System.Serializable]
        public class SoundBank
        {
            public string name;
            public AudioClip[] clips;
        }

        #region Variables
        public SoundBank defalutBank = new SoundBank();

        public bool randomPitch = true;
        public float pitchRandomRange = 0.2f;
        public float playDelay = 0;

        protected AudioSource m_Audiosource;
        #endregion

        #region Property
        public AudioSource audioSource => m_Audiosource;
        public AudioClip Clip { get; private set; }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            m_Audiosource = this.GetComponent<AudioSource>();
        }
        #endregion

        #region Custom Method
        public void PlayRandomClip()
        {
            Clip = InternalPlayRandomClip();
        }

        private AudioClip InternalPlayRandomClip()
        {
            var bank = defalutBank;
            //bank 체크
            if (bank.clips == null || bank.clips.Length == 0)
                return null;

            var clip = bank.clips[Random.Range(0, bank.clips.Length)];

            //클립 체크
            if (clip == null)
                return null;

            float rand = Random.Range(1f - pitchRandomRange, 1 + pitchRandomRange);
            m_Audiosource.pitch = randomPitch ? rand : 1f;
            m_Audiosource.clip = clip;
            m_Audiosource.PlayDelayed(playDelay);

            return clip;
        }
        #endregion
    }
}
